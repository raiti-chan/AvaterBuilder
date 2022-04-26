using Assets.Raitichan.Script.BoneRemapper;
using System.IO;
using Raitichan.Script.Util;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRC.SDK3.Avatars.Components;

namespace Assets.Raitichan.Script.VRCAvatarBuilder.Editor {

	[CustomEditor(typeof(OldAvatarBuilder))]
	public class OldAvatarBuilderEditor : UnityEditor.Editor {

		private OldAvatarBuilder _target;

		private bool _initAdvancedMode = false;
		private bool _advancedSetting = false;

		public void OnEnable() {
			this._target = this.target as OldAvatarBuilder;
		}

		public override void OnInspectorGUI() {
			this.serializedObject.Update();

			if (this._target.IsInitialized) this.InspectorGUI();
			else this.InitGUI();

			// 反映処理
			this.serializedObject.ApplyModifiedProperties();
		}

		private void InitGUI() {
			// AvatarDescripter
			SerializedProperty avatarProperty = this.serializedObject.FindProperty(this._target.AvatarPropertyName);
			EditorGUILayout.PropertyField(avatarProperty);

			this._initAdvancedMode = EditorGUILayout.Foldout(this._initAdvancedMode, "Advanced Initialze Settings");
			if (this._initAdvancedMode) {
				EditorGUI.indentLevel++;

				// Scene
				EditorGUILayout.PropertyField(this.serializedObject.FindProperty(this._target.ScenePropertyName), new GUIContent("Upload Scene"));

				// BoneNameProfile
				EditorGUILayout.PropertyField(this.serializedObject.FindProperty(this._target.BoneNameProfilePropertyName));

				EditorGUI.indentLevel--;
			}

			using (new EditorGUI.DisabledGroupScope(!this.CanInit())) {
				// 初期設定ボタン
				if (GUILayout.Button("Initialize Avatar Builder")) {
					// 作業ディレクトリの設定
					string fullPath = EditorUtility.OpenFolderPanel("Setting WorkingDirectry", Application.dataPath, string.Empty);
					string path = AssetPathUtil.GetAssetsPath(fullPath);
					if (string.IsNullOrEmpty(path)) {
						return;
					}
					if (this._target.Scene == null) {
						// アップロード用シーンの生成
						string uploadSceneFullPath = $"{fullPath}/{this._target.gameObject.scene.name}_{this._target.name}_Upload.unity";
						string uploadScenePath = AssetPathUtil.GetAssetsPath(uploadSceneFullPath);
						// シーンファイルの存在チェック
						Scene uploadScene = SceneManager.GetSceneByPath(uploadScenePath);
						if (uploadScene.name == null) {
							if (Directory.Exists(uploadSceneFullPath)) {
								// 存在すればそれを開く
								uploadScene = EditorSceneManager.OpenScene(uploadScenePath, OpenSceneMode.Additive);
							} else {
								uploadScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
							}
						}

						if (!uploadScene.isLoaded) {
							EditorSceneManager.CloseScene(uploadScene, true);
							uploadScene = EditorSceneManager.OpenScene(uploadScenePath, OpenSceneMode.Additive);
						}
						EditorSceneManager.MoveSceneBefore(uploadScene, this._target.gameObject.scene);


						if (EditorSceneManager.SaveScene(uploadScene, uploadScenePath)) {
							AssetDatabase.Refresh();
							SceneAsset uploadSceneAssset = AssetDatabase.LoadAssetAtPath<SceneAsset>(uploadScenePath);
							this.serializedObject.FindProperty(this._target.ScenePropertyName).objectReferenceValue = uploadSceneAssset;
						} else {
							EditorUtility.DisplayDialog("Error", "Failed to create the scene.", "OK");
							EditorSceneManager.CloseScene(uploadScene, true);
							return;
						}

					}

					this.serializedObject.FindProperty(this._target.WorkingDirectryPropertyName).stringValue = path;

					// Armatureの取得
					Animator avatarAnimator = this._target.Avatar.GetComponent<Animator>();
					GameObject armatureObject = avatarAnimator.GetBoneTransform(HumanBodyBones.Hips).parent.gameObject;
					this.serializedObject.FindProperty(this._target.ArmaturePropertyName).objectReferenceValue = armatureObject;
					this.serializedObject.ApplyModifiedProperties();
					this.serializedObject.Update();
					// Armature内のボーンの順序を適切に
					for (int i = (int)HumanBodyBones.UpperChest; i >= 0; i--) {
						Transform bone = avatarAnimator.GetBoneTransform((HumanBodyBones)i);
						if (bone == null) continue;
						bone.SetAsFirstSibling();
					}
					avatarAnimator.GetBoneTransform(HumanBodyBones.Spine).SetAsFirstSibling();

					if (this._target.BoneNameProfile == null) {

						string profilePath = $"{path}/{this._target.Avatar.name}_BoneNameProfile.asset";
						BoneNameProfile profile = AssetDatabase.LoadAssetAtPath<BoneNameProfile>(profilePath);
						if (profile == null) {
							profile = CreateInstance<BoneNameProfile>();
							profile.ImportArmature(this._target.Armature.transform, this._target.Avatar.GetComponent<Animator>().avatar);
							AssetDatabase.CreateAsset(profile, profilePath);
							AssetDatabase.Refresh();
						}
						this.serializedObject.FindProperty(this._target.BoneNameProfilePropertyName).objectReferenceValue = profile;
					}


					// 必要なオブジェクトの存在チェック
					bool hasHairs = false, hasWears = false, hasAccessorys = false, hasObjects = false;

					foreach (Transform childObject in this._target.transform) {
						switch (childObject.gameObject.name) {
							case "Hairs":
								hasHairs = true;
								this.serializedObject.FindProperty(this._target.HairsObjectPropertyName).objectReferenceValue = childObject.gameObject;
								break;

							case "Wears":
								hasWears = true;
								this.serializedObject.FindProperty(this._target.WearsObjectPropertyName).objectReferenceValue = childObject.gameObject;
								break;

							case "Accessorys":
								hasAccessorys = true;
								this.serializedObject.FindProperty(this._target.AccessorysObjectPropertyName).objectReferenceValue = childObject.gameObject;
								break;

							case "Objects":
								hasObjects = true;
								this.serializedObject.FindProperty(this._target.ObjectsObjectPropertyName).objectReferenceValue = childObject.gameObject;
								break;

						}
					}

					// 必要オブジェクトの生成
					if (!hasHairs) {
						GameObject hairs = new GameObject("Hairs");
						hairs.transform.parent = this._target.gameObject.transform;
						this.serializedObject.FindProperty(this._target.HairsObjectPropertyName).objectReferenceValue = hairs;
					}

					if (!hasWears) {
						GameObject wears = new GameObject("Wears");
						wears.transform.parent = this._target.gameObject.transform;
						this.serializedObject.FindProperty(this._target.WearsObjectPropertyName).objectReferenceValue = wears;
					}

					if (!hasAccessorys) {
						GameObject accessorys = new GameObject("Accessorys");
						accessorys.transform.parent = this._target.gameObject.transform;
						this.serializedObject.FindProperty(this._target.AccessorysObjectPropertyName).objectReferenceValue = accessorys;
					}

					if (!hasObjects) {
						GameObject objects = new GameObject("Objects");
						objects.transform.parent = this._target.gameObject.transform;
						this.serializedObject.FindProperty(this._target.ObjectsObjectPropertyName).objectReferenceValue = objects;
					}

					this.serializedObject.FindProperty(this._target.IsInitializedPropertyName).boolValue = true;
				}
			}

		}

		private void InspectorGUI() {


			// Scale
			SerializedProperty scaleProperty = this.serializedObject.FindProperty(this._target.ScalePropertyName);
			float oldScaleValue = scaleProperty.floatValue;
			EditorGUILayout.PropertyField(scaleProperty);
			if (scaleProperty.floatValue <= 0) {
				scaleProperty.floatValue = oldScaleValue;
			}

			if (GUILayout.Button("Import Wear")) {
				WearImportWindow.ShowWindow(this._target);
			}

			// 詳細プロパティ
			this._advancedSetting = EditorGUILayout.Foldout(this._advancedSetting, "Advanced Settings");
			if (this._advancedSetting) {
				EditorGUI.indentLevel++;

				// 作業ディレクトリ
				GUI.enabled = false;
				EditorGUILayout.PropertyField(this.serializedObject.FindProperty(this._target.WorkingDirectryPropertyName));
				GUI.enabled = true;
				if (GUILayout.Button("Change WorkingDirectry")) {
					string path = AssetPathUtil.GetAssetsPath(EditorUtility.OpenFolderPanel("Setting WorkingDirectry", Application.dataPath, string.Empty));
					this.serializedObject.FindProperty(this._target.WorkingDirectryPropertyName).stringValue = path;
				}

				// Scene
				EditorGUILayout.PropertyField(this.serializedObject.FindProperty(this._target.ScenePropertyName), new GUIContent("Upload Scene"));

				// BoneNameProfile
				EditorGUILayout.PropertyField(this.serializedObject.FindProperty(this._target.BoneNameProfilePropertyName));


				// AvatarDescripter
				SerializedProperty avatarProperty = this.serializedObject.FindProperty(this._target.AvatarPropertyName);
				EditorGUILayout.PropertyField(avatarProperty);

				// 次回ビルド時バージョン
				EditorGUILayout.PropertyField(this.serializedObject.FindProperty(this._target.NextBuildVersionPropertyName));

				EditorGUI.indentLevel--;
			}

			GUILayout.Space(5);
			using (new EditorGUI.DisabledGroupScope(!this.CanBuild())) {
				// ビルドボタン
				if (GUILayout.Button("Build")) this.Build();
			}

		}


		/// <summary>
		/// アバタービルドの実行
		/// </summary>
		private void Build() {
			// シーンのチェック
			string scenePath = AssetDatabase.GetAssetPath(this._target.Scene);
			Scene targetScene = SceneManager.GetSceneByPath(scenePath);
			if (targetScene.path != scenePath || !targetScene.isLoaded) {
				// 生成先シーンが開かれてないかロードされていない場合開く
				targetScene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
			}
			EditorSceneManager.MoveSceneBefore(targetScene, this._target.gameObject.scene);

			GameObject avatarObject = Instantiate(this._target.Avatar.gameObject);
			VRCAvatarDescriptor avatarDescriptor = avatarObject.GetComponent<VRCAvatarDescriptor>();

			avatarObject.name = this._target.Avatar.gameObject.name + "_" + this._target.NextBuildVersion;
			float scale = this._target.Scale;
			avatarObject.transform.localScale = new Vector3(scale, scale, scale);
			Vector3 viewPosition = avatarDescriptor.ViewPosition;
			viewPosition *= scale;
			avatarDescriptor.ViewPosition = viewPosition;

			this.serializedObject.FindProperty(this._target.NextBuildVersionPropertyName).intValue++;

			SceneManager.MoveGameObjectToScene(avatarObject, targetScene);
			Undo.RegisterCreatedObjectUndo(avatarObject, "Build");
		}

		/// <summary>
		/// 初期化が実行可能かチェック
		/// </summary>
		/// <returns></returns>
		private bool CanInit() {
			if (this._target.Avatar == null) return false;
			return true;
		}

		/// <summary>
		/// ビルドが実行可能かチェック
		/// </summary>
		/// <returns>実行可能な場合true</returns>
		private bool CanBuild() {
			if (string.IsNullOrEmpty(this._target.WorkingDirectry)) return false;
			if (this._target.Scene == null) return false;
			if (this._target.BoneNameProfile == null) return false;
			if (this._target.Avatar == null) return false;
			if (this._target.HairsObject == null) return false;
			if (this._target.WearsObject == null) return false;
			if (this._target.AccessorysObject == null) return false;
			if (this._target.ObjectsObject == null) return false;
			if (this._target.Armature == null) return false;

			return true;
		}

	}
}
