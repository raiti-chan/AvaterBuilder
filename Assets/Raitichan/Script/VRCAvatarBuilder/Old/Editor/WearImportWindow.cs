using Raitichan.Script.Util.Editor.Extension;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Raitichan.Script.VRCAvatarBuilder.Editor {
	public class WearImportWindow : EditorWindow {

		public static void ShowWindow(OldAvatarBuilder target) {
			WearImportWindow window = GetWindow<WearImportWindow>();
			window.titleContent = new GUIContent("Wear Import");
			window._target = target;
			window.Show();
		}

		private GUIStyle _richTextLabel;

		private bool _canImport;
		private OldAvatarBuilder _target;
		private WearAsset _wearAsset;

		private void OnGUI() {
			_richTextLabel = new GUIStyle(EditorStyles.label) { richText = true };

			this._wearAsset = EditorGUILayout.ObjectField("Wear Asset", this._wearAsset, typeof(WearAsset), true) as WearAsset;
			if (this._wearAsset == null) return;
			this._canImport = true;

			GUILayout.Space(15);
			
			if (!this._wearAsset.gameObject.IsSceneObject()) {
				// 指定されたオブジェクトがシーン上に無い。
				this._canImport = false;
				using (new GUILayout.HorizontalScope()) {
					GUILayout.Label(EditorGUIUtility.IconContent("d_console.warnicon.sml"), GUILayout.Width(20));
					GUILayout.Label("<color=red>WearSetObject is not an object in the scene. </color>", _richTextLabel);
				}
				if (GUILayout.Button("Instantiate")) {
					// シーン上に生成する
					GameObject newObj = Instantiate(this._wearAsset.gameObject, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
					newObj.name = this._wearAsset.name;
					SceneManager.MoveGameObjectToScene(newObj, this._target.gameObject.scene);
					Undo.RegisterCreatedObjectUndo(newObj, "Instantiate Prefab Object");
					this._wearAsset = newObj.GetComponent<WearAsset>();
				}
			} else if (this._wearAsset.gameObject.IsPrefabObject()) {
				// 指定された服がプレハブオブジェクト
				this._canImport = false;
				using (new GUILayout.HorizontalScope()) {
					GUILayout.Label(EditorGUIUtility.IconContent("d_console.warnicon.sml"), GUILayout.Width(20));
					GUILayout.Label("<color=red>WearSetObject is a prefab object.</color>", _richTextLabel);
				}
				if (GUILayout.Button("Unpack Prefab")) {
					// プレハブの解除
					PrefabUtility.UnpackPrefabInstance(this._wearAsset.gameObject, PrefabUnpackMode.Completely, InteractionMode.UserAction);
				}
			}

			// セットアップされてないアセットのバリテーション


			GUILayout.Space(30);

			using (new EditorGUI.DisabledGroupScope(!this._canImport)) {
				if (GUILayout.Button("Import")) {
					Undo.RecordObject(this._wearAsset.gameObject.transform, "Import Wear asset.");
					this._wearAsset.gameObject.transform.parent = this._target.WearsObject.transform;
				}
			}
		}

	}
}
