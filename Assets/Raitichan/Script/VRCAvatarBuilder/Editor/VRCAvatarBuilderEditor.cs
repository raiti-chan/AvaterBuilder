using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Raitichan.Script.Util.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRC.SDK3.Avatars.Components;
using VRC.SDKBase;

namespace Raitichan.Script.VRCAvatarBuilder.Editor {
	[CustomEditor(typeof(VRCAvatarBuilder))]
	public class VRCAvatarBuilderEditor : UnityEditor.Editor {
		private static readonly Regex VERSION_PATTERN;

		static VRCAvatarBuilderEditor() {
			VERSION_PATTERN = new Regex(@".+_(?<Version>[0-9]+)", RegexOptions.Compiled);
		}


		private VRCAvatarBuilder _target;

		private SerializedProperty _languageProperty;
		private SerializedProperty _avatarDescriptorProperty;
		private SerializedProperty _workingDirectoryPathProperty;
		private SerializedProperty _scaleProperty;
		private SerializedProperty _uploadSceneProperty;
		private SerializedProperty _baseLayersProperty;
		private SerializedProperty _additiveLayersProperty;
		private SerializedProperty _gestureLayersProperty;
		private SerializedProperty _actionLayersProperty;
		private SerializedProperty _fxLayersProperty;
		private SerializedProperty _leftGestureAnimationsProperty;
		private SerializedProperty _rightGestureAnimationsProperty;
		private SerializedProperty _leftExpressionAnimationsProperty;
		private SerializedProperty _rightExpressionAnimationsProperty;

		private void OnEnable() {
			this._target = this.target as VRCAvatarBuilder;

			this._languageProperty = this.serializedObject.FindProperty(VRCAvatarBuilder.LanguagePropertyName);
			this._avatarDescriptorProperty =
				this.serializedObject.FindProperty(VRCAvatarBuilder.AvatarDescriptorPropertyName);
			this._workingDirectoryPathProperty =
				this.serializedObject.FindProperty(VRCAvatarBuilder.WorkingDirectoryPathPropertyName);
			this._scaleProperty = this.serializedObject.FindProperty(VRCAvatarBuilder.ScalePropertyName);
			this._uploadSceneProperty = this.serializedObject.FindProperty(VRCAvatarBuilder.UploadScenePropertyName);
			this._baseLayersProperty = this.serializedObject.FindProperty(VRCAvatarBuilder.BaseLayersPropertyName);
			this._additiveLayersProperty =
				this.serializedObject.FindProperty(VRCAvatarBuilder.AdditiveLayersPropertyName);
			this._gestureLayersProperty =
				this.serializedObject.FindProperty(VRCAvatarBuilder.GestureLayersPropertyName);
			this._actionLayersProperty = this.serializedObject.FindProperty(VRCAvatarBuilder.ActionLayersPropertyName);
			this._fxLayersProperty = this.serializedObject.FindProperty(VRCAvatarBuilder.FxLayersPropertyName);
			this._leftGestureAnimationsProperty =
				this.serializedObject.FindProperty(VRCAvatarBuilder.LeftGestureAnimationsPropertyName);
			this._rightGestureAnimationsProperty =
				this.serializedObject.FindProperty(VRCAvatarBuilder.RightGestureAnimationsPropertyName);
			this._leftExpressionAnimationsProperty =
				this.serializedObject.FindProperty(VRCAvatarBuilder.LeftExpressionAnimationsPropertyName);
			this._rightExpressionAnimationsProperty =
				this.serializedObject.FindProperty(VRCAvatarBuilder.RightExpressionAnimationsPropertyName);


		}

		public override void OnInspectorGUI() {
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this._languageProperty, new GUIContent(Strings.Language));
			Strings.Lang = (Strings.Languages)this._languageProperty.intValue;

			EditorGUILayout.PropertyField(this._avatarDescriptorProperty, new GUIContent(Strings.TargetAvatar));
			EditorGUILayout.PropertyField(this._workingDirectoryPathProperty, new GUIContent(Strings.WorkingDirectory));
			EditorGUILayout.PropertyField(this._scaleProperty, new GUIContent(Strings.AvatarScale));
			EditorGUILayout.PropertyField(this._uploadSceneProperty, new GUIContent(Strings.UploadScene));
			EditorGUILayout.PropertyField(this._baseLayersProperty);
			EditorGUILayout.PropertyField(this._additiveLayersProperty);
			EditorGUILayout.PropertyField(this._gestureLayersProperty);
			EditorGUILayout.PropertyField(this._actionLayersProperty);
			EditorGUILayout.PropertyField(this._fxLayersProperty);
			EditorGUILayout.PropertyField(this._leftGestureAnimationsProperty);
			EditorGUILayout.PropertyField(this._rightGestureAnimationsProperty);
			EditorGUILayout.PropertyField(this._leftExpressionAnimationsProperty);
			EditorGUILayout.PropertyField(this._rightExpressionAnimationsProperty);
			using (new EditorGUI.DisabledScope(!this.CanBuild())) {
				if (GUILayout.Button(Strings.Build)) {
					this.Build();
				}
			}

			this.serializedObject.ApplyModifiedProperties();
		}


		[SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
		private bool CanBuild() {
			if (this._target.AvatarDescriptor == null) return false;
			if (string.IsNullOrEmpty(this._target.WorkingDirectoryPath)) return false;
			if (this._target.UploadScene == null) return false;
			if (this._target.BaseLayers == null) return false;
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
			
			

			Undo.RegisterCreatedObjectUndo(avatar.gameObject, "Avatar Build");
		}

		/// <summary>
		/// 作業フォルダの存在チェック
		/// 存在しない場合作成。
		/// </summary>
		private void WorkingDirectoryCheck() {
			string fullPath = AssetPathUtil.GetFullPath(this._target.WorkingDirectoryPath);
			if (!Directory.Exists(fullPath)) {
				Directory.CreateDirectory(fullPath);
			}
			AssetDatabase.Refresh();
			AssetDatabase.CreateFolder(this._target.WorkingDirectoryPath, "out");
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
	}
}