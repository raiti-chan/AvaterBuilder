using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Raitichan.Script.Util;
using Raitichan.Script.Util.Extension;
using Raitichan.Script.VRCAvatarBuilder.AnimatorControllerGenerator;
using Raitichan.Script.VRCAvatarBuilder.Context;
using Raitichan.Script.VRCAvatarBuilder.Module;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDKBase;
using AnimLayerType = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor.AnimLayerType;

namespace Raitichan.Script.VRCAvatarBuilder.Editor {
	[CustomEditor(typeof(VRCAvatarBuilder))]
	public class VRCAvatarBuilderEditor : UnityEditor.Editor {
		private static readonly Regex VERSION_PATTERN;

		static VRCAvatarBuilderEditor() {
			VERSION_PATTERN = new Regex(@".+_(?<Version>[0-9]+)", RegexOptions.Compiled);
		}


		private VRCAvatarBuilder _target;

		private VRCAvatarBuilderModuleBase[] _modules;

		private AnimatorController _emptyController;

		private Rect _addModuleWindow;

		private SerializedProperty _languageProperty;
		private SerializedProperty _avatarDescriptorProperty;
		private SerializedProperty _workingDirectoryPathProperty;
		private SerializedProperty _scaleProperty;
		private SerializedProperty _uploadSceneProperty;
		private SerializedProperty _expressionMenuProperty;
		
		private SerializedProperty _basicSettingFoldoutProperty;
		private SerializedProperty _moduleFoldoutProperty;
		
		private void OnEnable() {
			this._target = this.target as VRCAvatarBuilder;
			if (this._target == null) return;

			this.UpdateModuleList();
			Undo.undoRedoPerformed += this.UpdateModuleList;

			this._emptyController =
				AssetDatabase.LoadAssetAtPath<AnimatorController>(ConstantPath.EMPTY_CONTROLLER_PATH);

			this._languageProperty = this.serializedObject.FindProperty(VRCAvatarBuilder.LanguagePropertyName);
			this._avatarDescriptorProperty =
				this.serializedObject.FindProperty(VRCAvatarBuilder.AvatarDescriptorPropertyName);
			this._workingDirectoryPathProperty =
				this.serializedObject.FindProperty(VRCAvatarBuilder.WorkingDirectoryPathPropertyName);
			this._scaleProperty = this.serializedObject.FindProperty(VRCAvatarBuilder.ScalePropertyName);
			this._uploadSceneProperty = this.serializedObject.FindProperty(VRCAvatarBuilder.UploadScenePropertyName);
			this._expressionMenuProperty =
				this.serializedObject.FindProperty(VRCAvatarBuilder.ExpressionsMenuPropertyName);
			
			this._basicSettingFoldoutProperty =
				this.serializedObject.FindProperty(VRCAvatarBuilder.BasicSettingFoldoutPropertyName);
			this._moduleFoldoutProperty =
				this.serializedObject.FindProperty(VRCAvatarBuilder.ModuleFoldoutPropertyName);
		}

		private void OnDisable() {
			Undo.undoRedoPerformed -= this.UpdateModuleList;
		}

		public override void OnInspectorGUI() {
			if (this._emptyController == null) {
				EditorGUILayout.HelpBox(Strings.NotFoundEmptyTemplateLayer,
					MessageType.Error);
			}

			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this._languageProperty, new GUIContent(Strings.Language));
			Strings.Lang = (Strings.Languages)this._languageProperty.intValue;
			
			EditorGUILayout.PropertyField(this._avatarDescriptorProperty, new GUIContent(Strings.TargetAvatar));
			GUILayout.Space(8);

			this._basicSettingFoldoutProperty.boolValue =
				RaitisEditorUtil.HeaderFoldout(this._target.BasicSettingFoldout, Strings.BasicSetting);
			if (this._target.BasicSettingFoldout) {
				GUILayout.Space(4);
				using (new EditorGUI.DisabledScope(true)) {
					EditorGUILayout.ObjectField(Strings.EmptyTemplateLayer, this._emptyController,
						typeof(AnimatorController), false);
				}
				EditorGUILayout.PropertyField(this._workingDirectoryPathProperty, new GUIContent(Strings.WorkingDirectory));
				EditorGUILayout.PropertyField(this._uploadSceneProperty, new GUIContent(Strings.UploadScene));
				EditorGUILayout.PropertyField(this._scaleProperty, new GUIContent(Strings.AvatarScale));
				EditorGUILayout.PropertyField(this._expressionMenuProperty);
			}
			
			this._moduleFoldoutProperty.boolValue =
				RaitisEditorUtil.HeaderFoldout(this._target.ModuleFoldout, Strings.Module);
			if (this._target.ModuleFoldout) {
				GUILayout.Space(2);
				foreach (VRCAvatarBuilderModuleBase module in this._modules) {
					module.IsOpenInAvatarBuilder = RaitisEditorUtil.Foldout(module.IsOpenInAvatarBuilder, module.name,
						OnModuleMenuClick, module, Strings.Delete);
					if (!module.IsOpenInAvatarBuilder) continue;
					Undo.RecordObject(module.gameObject, "rename");
					module.name = EditorGUILayout.TextField(module.name);
					UnityEditor.Editor editor = CreateEditor(module);
					editor.OnInspectorGUI();
				}

				using (new GUILayout.HorizontalScope()) {
					GUILayout.FlexibleSpace();
					if (GUILayout.Button(Strings.VRCAvatarBuilderEditor_AddModule, GUILayout.Width(300),
						    GUILayout.Height(30))) {
						ModuleSearchWindow moduleSearchWindow = CreateInstance<ModuleSearchWindow>();
						moduleSearchWindow.AvatarBuilder = this._target.gameObject;
						moduleSearchWindow.Inspector = this;
						moduleSearchWindow.ShowAsDropDown(this._addModuleWindow,
							new Vector2(this._addModuleWindow.width, 300));
					}

					if (Event.current.type == EventType.Repaint) {
						this._addModuleWindow = GUIUtility.GUIToScreenRect(GUILayoutUtility.GetLastRect());
					}

					GUILayout.FlexibleSpace();
				}
				GUILayout.Space(4);
				RaitisEditorUtil.Footer();
			}

			GUILayout.Space(5);
			using (new GUILayout.HorizontalScope())
			using (new EditorGUI.DisabledScope(!this.CanBuild())) {
				if (GUILayout.Button(Strings.Build, GUILayout.Height(25))) {
					this.Build();
				}
			}

			this.serializedObject.ApplyModifiedProperties();
		}

		public void UpdateModuleList() {
			this._modules = this._target.gameObject.GetComponentsInChildren<VRCAvatarBuilderModuleBase>();
		}

		// ReSharper disable Unity.PerformanceAnalysis
		private void OnModuleMenuClick(object data, int index) {
			if (!(data is VRCAvatarBuilderModuleBase module)) return;
			switch (index) {
				case 0:
					GameObject obj = module.gameObject;
					Undo.DestroyObjectImmediate(module);
					if (obj.GetComponents<VRCAvatarBuilderModuleBase>().Length <= 0) {
						Undo.DestroyObjectImmediate(obj);
					}

					this.UpdateModuleList();
					break;
			}
		}


		[SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
		private bool CanBuild() {
			if (this._target.AvatarDescriptor == null) return false;
			if (string.IsNullOrEmpty(this._target.WorkingDirectoryPath)) return false;
			if (this._target.UploadScene == null) return false;
			return true;
		}

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