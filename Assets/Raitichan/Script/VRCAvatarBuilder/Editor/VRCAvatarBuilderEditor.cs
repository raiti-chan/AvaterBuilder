using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
		private UnityEditor.Editor[] _moduleEditors;

		private AnimatorController _emptyController;

		private Rect _addModuleWindow;

		private SerializedProperty _languageProperty;
		private SerializedProperty _avatarDescriptorProperty;
		private SerializedProperty _workingDirectoryPathProperty;
		private SerializedProperty _scaleProperty;
		private SerializedProperty _uploadSceneProperty;
		private SerializedProperty _expressionMenuProperty;

		public void UpdateModuleList() {
			this._modules = this._target.gameObject.GetComponentsInChildren<VRCAvatarBuilderModuleBase>();
			this._moduleEditors = this._modules.Select(CreateEditor).ToArray();
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
		}

		private void OnDisable() {
			Undo.undoRedoPerformed -= this.UpdateModuleList;
			foreach (UnityEditor.Editor moduleEditor in this._moduleEditors) {
				DestroyImmediate(moduleEditor);
			}
		}

		public override void OnInspectorGUI() {
			// 自身がPrefabならインスペクタを表示しないように。
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
			bool hasError = this.DrawInitError();
			this.DrawWarning(hasError);

			this._target.BasicSettingFoldout =
				RaitisEditorUtil.HeaderFoldout(this._target.BasicSettingFoldout, Strings.VRCAvatarBuilderEditor_BasicSetting);
			if (this._target.BasicSettingFoldout) {
				this.DrawBasicSetting();
			}

			this._target.ModuleFoldout =
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
		private bool DrawInitError() {
			bool hasError = false;
			if (this._target.AvatarDescriptor == null) {
				hasError = true;
				EditorGUILayout.HelpBox(Strings.VRCAvatarBuilderEditor_AvatarIsNotSet, MessageType.Error);
			}

			if (string.IsNullOrEmpty(this._target.WorkingDirectoryPath)) {
				hasError = true;
				if (RaitisEditorUtil.HelpBoxWithButton(Strings.VRCAvatarBuilderEditor_WorkingDirectoryIsNotSet,
					    MessageType.Error, Strings.Setting)) {
					string defaultPath = AssetPathUtil.GetFullPath(ConstantPath.ASSET_DIR_PATH);
					string path = EditorUtility.OpenFolderPanel(Strings.VRCAvatarBuilderEditor_WorkingDirectorySetup,
						defaultPath, "");
					this._workingDirectoryPathProperty.stringValue = AssetPathUtil.GetAssetsPath(path);
				}
			}

			if (this._target.UploadScene == null) {
				hasError = true;
				if (RaitisEditorUtil.HelpBoxWithButton(Strings.VRCAvatarBuilderEditor_UploadSceneIsNotSet,
					    MessageType.Error, Strings.AutoSetting)) {
					if (string.IsNullOrEmpty(this._target.WorkingDirectoryPath)) {
						EditorUtility.DisplayDialog(Strings.Warning,
							Strings.VRCAvatarBuilderEditor_SetUpTheWorkingDirectoryFirst, Strings.OK);
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
							return true;
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

			return hasError;
		}

		// ReSharper disable Unity.PerformanceAnalysis
		[SuppressMessage("ReSharper", "InvertIf")]
		private void DrawWarning(bool hasError) {
			if (hasError) return;
			if (this._modules.Length <= 0) {
				if (RaitisEditorUtil.HelpBoxWithButton(Strings.VRCAvatarBuilderEditor_ModuleIsNotSet,
					    MessageType.Warning, Strings.Setting)) {
					Transform parent = this._target.gameObject.transform;
					GameObject[] generateObject = new[] {
						new GameObject(Strings.BaseLayer, typeof(DefaultBaseLayerModule)) {
							transform = { parent = parent }
						},
						new GameObject(Strings.AdditiveLayer, typeof(DefaultAdditiveLayerModule)) {
							transform = { parent = parent }
						},
						new GameObject(Strings.GestureLayer, typeof(GestureLayerModule)) {
							transform = { parent = parent }
						},
						new GameObject(Strings.ActionLayer, typeof(DefaultActionLayerModule)) {
							transform = { parent = parent }
						},
						new GameObject(Strings.GestureExpressionLayer, typeof(GestureExpressionModule)) {
							transform = { parent = parent }
						}
					};
					foreach (GameObject gameObject in generateObject) {
						Undo.RegisterCreatedObjectUndo(gameObject, "Standard Setting");
					}

					this.UpdateModuleList();
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
		// ReSharper disable Unity.PerformanceAnalysis
		private void DrawModules() {
			GUILayout.Space(2);
			bool updateFlag = false;
			for (int i = 0; i < this._modules.Length; i++) {
				VRCAvatarBuilderModuleBase module = this._modules[i];
				EditorGUI.indentLevel--;
				module.IsOpenInAvatarBuilder = RaitisEditorUtil.Foldout(
					module.IsOpenInAvatarBuilder,
					$"{module.name}    ({module.GetType().Name})",
					OnModuleMenuClick, module, Strings.GoToObject, Strings.Delete);
				EditorGUI.indentLevel++;
				if (!module.IsOpenInAvatarBuilder) continue;
				Undo.RecordObject(module.gameObject, "rename");
				module.name = EditorGUILayout.TextField(module.name);
				this._moduleEditors[i].OnInspectorGUI();
				updateFlag = updateFlag || module.ModuleUpdateFlag;
			}

			if (updateFlag) {
				this.UpdateModuleList();
			}

			using (new GUILayout.HorizontalScope()) {
				GUILayout.FlexibleSpace();
				if (GUILayout.Button(Strings.AddModule, GUILayout.Width(300),
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
					Selection.activeObject = module;
					break;
				case 1:
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