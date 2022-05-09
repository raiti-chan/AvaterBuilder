using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDKBase;
using Raitichan.Script.Util;
using Raitichan.Script.Util.Extension;
using Raitichan.Script.VRCAvatarBuilder.AnimatorControllerGenerator;
using Raitichan.Script.VRCAvatarBuilder.Context;
using Raitichan.Script.VRCAvatarBuilder.Module;
using AnimLayerType = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor.AnimLayerType;

namespace Raitichan.Script.VRCAvatarBuilder.Editor {
	public partial class VRCAvatarBuilderEditor {
		// ReSharper disable Unity.PerformanceAnalysis
		/// <summary>
		/// ビルドの実行
		/// </summary>
		private void Build() {
			this.WorkingDirectoryCheck();
			Scene uploadScene = this.CheckScene();
			CleanupScene(uploadScene);

			uint version = this.GetNextBuildVersion(uploadScene);
			VRCAvatarDescriptor avatar = this.CloneAvatar(uploadScene, version);
			this.ApplyAvatarScale(avatar);
			VRCAvatarBuilderContext context = new VRCAvatarBuilderContext {
				Builder = this._target,
				Avatar = avatar,
				BuildVersion = version,
				OutputPath = this.CheckOutputDirectory(this._target.gameObject.name, avatar)
			};
			foreach (VRCAvatarBuilderModuleBase module in this._modules) {
				module.Build(context);
			}

			this.GenerateLayer(context, AnimLayerType.Base, ConstantPath.GENERATE_BASE_LAYER_FILENAME);
			this.GenerateLayer(context, AnimLayerType.Additive, ConstantPath.GENERATE_ADDITIVE_LAYER_FILENAME);
			this.GenerateLayer(context, AnimLayerType.Gesture, ConstantPath.GENERATE_GESTURE_LAYER_FILENAME);
			this.GenerateLayer(context, AnimLayerType.Action, ConstantPath.GENERATE_ACTION_LAYER_FILENAME);
			this.GenerateLayer(context, AnimLayerType.FX, ConstantPath.GENERATE_FX_LAYER_FILENAME);
			this.GenerateLayer(context, AnimLayerType.Sitting, ConstantPath.GENERATE_SITTING_LAYER_FILENAME);
			this.GenerateLayer(context, AnimLayerType.TPose, ConstantPath.GENERATE_T_POSE_LAYER_FILENAME);
			this.GenerateLayer(context, AnimLayerType.IKPose, ConstantPath.GENERATE_IK_POSE_LAYER_FILENAME);

			CopyExpressionMenu(avatar, context.OutputPath);
			Undo.RegisterCreatedObjectUndo(avatar.gameObject, "Avatar Build");
		}

		/// <summary>
		/// 対象のレイヤーを生成します。
		/// </summary>
		/// <param name="context"></param>
		/// <param name="type"></param>
		/// <param name="fileName"></param>
		private void GenerateLayer(VRCAvatarBuilderContext context, AnimLayerType type, string fileName) {
			List<IAnimatorControllerLayerGenerator> generators = context.AnimatorControllerLayerGenerators[type];
			if (generators.Count <= 0) {
				// ジェネレーターが無い場合デフォルトに設定
				context.Avatar.SetLayer(type, null);
				return;
			}

			string emptyControllerPath = AssetDatabase.GetAssetPath(this._emptyController);
			string dstAssetPath = context.OutputPath + fileName;
			AssetDatabase.CopyAsset(emptyControllerPath, dstAssetPath);
			AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(dstAssetPath);
			foreach (IAnimatorControllerLayerGenerator generator in generators) {
				generator.Generate(controller);
			}

			context.Avatar.SetLayer(type, controller);
		}


		/// <summary>
		/// 作業フォルダの存在チェック
		/// 存在しない場合作成。
		/// </summary>
		private void WorkingDirectoryCheck() {
			string fullPath = AssetPathUtil.GetFullPath(this._target.WorkingDirectoryPath) +
			                  ConstantPath.OUTPUT_DIRECTORY;
			if (Directory.Exists(fullPath)) return;
			Directory.CreateDirectory(fullPath);
			AssetDatabase.Refresh();
		}

		/// <summary>
		/// ビルド用シーンが開かれているかをチェックし、開かれていない場合は開く
		/// </summary>
		private Scene CheckScene() {
			string scenePath = AssetDatabase.GetAssetPath(this._target.UploadScene);
			Scene uploadScene = SceneManager.GetSceneByPath(scenePath);
			if (uploadScene.path != scenePath || !uploadScene.isLoaded) {
				// ビルド用シーンが開かれて居ない場合開く
				uploadScene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
			}

			EditorSceneManager.MoveSceneBefore(uploadScene, this._target.gameObject.scene);
			return uploadScene;
		}

		/// <summary>
		/// シーン上のオブジェクトを非アクティブ化しクリーンアップします。
		/// </summary>
		private static void CleanupScene(Scene scene) {
			foreach (GameObject gameObject in scene.GetRootGameObjects().Where(o => o.activeSelf)) {
				Undo.RecordObject(gameObject, "Avatar Build");
				gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// 次のビルドバージョンを求める。
		/// </summary>
		/// <param name="scene"></param>
		/// <returns></returns>
		private uint GetNextBuildVersion(Scene scene) {
			return scene.GetRootGameObjects()
				.Where(o => o.name.Contains(this._target.gameObject.name))
				.Select(o => VERSION_PATTERN.Match(o.name))
				.Where(o => o.Success)
				.Select(o => Convert.ToUInt32(o.Groups["Version"].Value))
				.DefaultIfEmpty()
				.Max() + 1;
		}

		// ReSharper disable Unity.PerformanceAnalysis
		/// <summary>
		/// アバターをクローンし、名前を変更し、シーンに移動。
		/// </summary>
		/// <param name="uploadScene"></param>
		/// <param name="version"></param>
		/// <returns></returns>
		private VRCAvatarDescriptor CloneAvatar(Scene uploadScene, uint version) {
			GameObject avatarObject = Instantiate(this._target.AvatarDescriptor.gameObject);
			VRCAvatarDescriptor vrcAvatarDescriptor = avatarObject.GetComponent<VRCAvatarDescriptor>();

			avatarObject.name = this._target.gameObject.name + "_" + version;

			SceneManager.MoveGameObjectToScene(avatarObject, uploadScene);
			return vrcAvatarDescriptor;
		}

		/// <summary>
		/// アバターに指定されたスケールを適用する。
		/// </summary>
		/// <param name="avatar"></param>
		private void ApplyAvatarScale(VRC_AvatarDescriptor avatar) {
			float scale = this._target.Scale;
			avatar.transform.localScale = new Vector3(scale, scale, scale);
			Vector3 viewPosition = avatar.ViewPosition;
			viewPosition *= scale;
			avatar.ViewPosition = viewPosition;
		}

		/// <summary>
		/// 出力用ディレクトリを作成します。
		/// すでに存在している場合別の名前で作成します。
		/// </summary>
		/// <param name="avatarName"></param>
		/// <param name="avatar"></param>
		/// <returns></returns>
		private string CheckOutputDirectory(string avatarName, Component avatar) {
			string assetPath = this._target.WorkingDirectoryPath
			                   + "/" + ConstantPath.OUTPUT_DIRECTORY
			                   + "/" + AssetPathUtil.ReplaceInvalidFileNameChars(avatarName, '_')
			                   + "/" + AssetPathUtil.ReplaceInvalidFileNameChars(avatar.gameObject.name, '_')
			                   + ConstantPath.GENERATED_SUFFIX;

			string fullPath = AssetPathUtil.GetFullPath(assetPath);
			if (Directory.Exists(fullPath)) {
				assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
				fullPath = AssetPathUtil.GetFullPath(assetPath);
			}

			Directory.CreateDirectory(fullPath);
			AssetDatabase.Refresh();

			return assetPath;
		}

		private void CopyExpressionMenu(VRCAvatarDescriptor avatar, string outPutPath) {
			if (this._target.ExpressionsMenu == null) return;
			string dstAssetPath = outPutPath + "/ExpressionMenu.asset";
			VRCExpressionsMenu clonedMenu = this._target.ExpressionsMenu.DeepClone();
			AssetDatabase.CreateAsset(clonedMenu, dstAssetPath);
			clonedMenu.SaveSubMenu();
			avatar.expressionsMenu = clonedMenu;
		}
	}
}