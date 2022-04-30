using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Raitichan.Script.Util;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using Raitichan.Script.VRCAvatarBuilder.Module;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

namespace Raitichan.Script.VRCAvatarBuilder.Editor {
	[CustomEditor(typeof(VRCAvatarBuilder))]
	public partial class VRCAvatarBuilderEditor : UnityEditor.Editor {
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

		public void UpdateModuleList() {
			this._modules = this._target.gameObject.GetComponentsInChildren<VRCAvatarBuilderModuleBase>();
		}


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
			EditorGUILayout.PropertyField(this._scaleProperty, new GUIContent(Strings.AvatarScale));
			GUILayout.Space(8);
			this.DrawInitError();

			this._basicSettingFoldoutProperty.boolValue =
				RaitisEditorUtil.HeaderFoldout(this._target.BasicSettingFoldout, Strings.BasicSetting);
			if (this._target.BasicSettingFoldout) {
				this.DrawBasicSetting();
			}

			this._moduleFoldoutProperty.boolValue =
				RaitisEditorUtil.HeaderFoldout(this._target.ModuleFoldout, Strings.Module);
			if (this._target.ModuleFoldout) {
				this.DrawModules();
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

		[SuppressMessage("ReSharper", "InvertIf")]
		private void DrawInitError() {
			if (this._target.AvatarDescriptor == null) {
				EditorGUILayout.HelpBox(Strings.VRCAvatarBuilderEditor_AvatarIsNotSet, MessageType.Error);
			}

			if (string.IsNullOrEmpty(this._target.WorkingDirectoryPath)) {
				EditorGUILayout.HelpBox(Strings.VRCAvatarBuilderEditor_WorkingDirectoryIsNotSet, MessageType.Error);
				Rect buttonRect = GUILayoutUtility.GetLastRect();
				buttonRect.x += buttonRect.width - 110;
				buttonRect.width = 100;
				buttonRect.y += 5;
				buttonRect.height -= 10;
				if (GUI.Button(buttonRect, Strings.Setting)) {
					string defaultPath = AssetPathUtil.GetFullPath(ConstantPath.ASSET_DIR_PATH);
					string path = EditorUtility.OpenFolderPanel("作業ディレクトリ設定", defaultPath, "");
					this._workingDirectoryPathProperty.stringValue = AssetPathUtil.GetAssetsPath(path);
				}
			}

			if (this._target.UploadScene == null) {
				EditorGUILayout.HelpBox(Strings.VRCAvatarBuilderEditor_UploadSceneIsNotSet, MessageType.Error);
				Rect buttonRect = GUILayoutUtility.GetLastRect();
				buttonRect.x += buttonRect.width - 110;
				buttonRect.width = 100;
				buttonRect.y += 5;
				buttonRect.height -= 10;
				if (GUI.Button(buttonRect, Strings.AutoSetting)) {
					if (string.IsNullOrEmpty(this._target.WorkingDirectoryPath)) {
						EditorUtility.DisplayDialog(Strings.Warning, "先に作業ディレクトリを設定してください。", Strings.OK);
					} else {
						string sceneDirectoryPath = this._target.WorkingDirectoryPath + ConstantPath.SCENE_DIRECTORY;
						string uploadScenePath = sceneDirectoryPath + AssetPathUtil.ReplaceInvalidFileNameChars(
							$"/{this._target.gameObject.scene.name}_{this._target.name}_Upload.unity",
							'_');
						// シーンアセットが存在するかチェック
						SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(uploadScenePath);
						if (sceneAsset != null) {
							// 存在したら設定して終了
							this._uploadSceneProperty.objectReferenceValue = sceneAsset;
							return;
						}

						string sceneDirectoryFullPath = AssetPathUtil.GetFullPath(sceneDirectoryPath);
						if (!Directory.Exists(sceneDirectoryPath)) {
							Directory.CreateDirectory(sceneDirectoryFullPath);
						}

						// 新規シーンの生成
						Scene uploadScene =
							EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
						EditorSceneManager.SaveScene(uploadScene, uploadScenePath);
						EditorSceneManager.CloseScene(uploadScene, true);
						AssetDatabase.Refresh();
						this._uploadSceneProperty.objectReferenceValue =
							AssetDatabase.LoadAssetAtPath<SceneAsset>(uploadScenePath);
					}
				}
			}
		}

		private void DrawBasicSetting() {
			GUILayout.Space(4);
			using (new EditorGUI.DisabledScope(true)) {
				EditorGUILayout.ObjectField(Strings.EmptyTemplateLayer, this._emptyController,
					typeof(AnimatorController), false);
			}

			EditorGUILayout.PropertyField(this._workingDirectoryPathProperty,
				new GUIContent(Strings.WorkingDirectory));
			EditorGUILayout.PropertyField(this._uploadSceneProperty, new GUIContent(Strings.UploadScene));
			EditorGUILayout.PropertyField(this._expressionMenuProperty);
		}

		private void DrawModules() {
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
	}
}