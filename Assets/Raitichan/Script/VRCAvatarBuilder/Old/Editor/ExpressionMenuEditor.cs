using System;
using Raitichan.Script.Util;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Assets.Raitichan.Script.VRCAvatarBuilder.Editor {

	[CustomEditor(typeof(ExpressionMenuEditor))]
	public class ExpressionMenuEditorEditor : UnityEditor.Editor {

		private ReorderableList _parameterReorderableList;

		public void OnEnable() {
			this._parameterReorderableList = null;
		}

		public override void OnInspectorGUI() {
			this.serializedObject.Update();
			ExpressionMenuEditor editor = this.target as ExpressionMenuEditor;

			do {
				this.DrawWorkingDirectry(editor);
				if (!this.DrawSetup(editor)) break;

				this.DrawParams(editor.Parameters);
				this.DrawMenu(editor.MenuRoot, editor.Parameters);

			} while (false);

			this.serializedObject.ApplyModifiedProperties();

		}

		private bool _isOpenWorkingDirectry;
		private void DrawWorkingDirectry(ExpressionMenuEditor editor) {
			if (string.IsNullOrEmpty(editor.WorkingDirectry)) {
				this._isOpenWorkingDirectry = true;
			}

			this._isOpenWorkingDirectry = EditorGUILayout.Foldout(this._isOpenWorkingDirectry, "作業ディレクトリ");
			if (!this._isOpenWorkingDirectry) return;

			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.PropertyField(this.serializedObject.FindProperty("_workingDirectry"));
			EditorGUI.EndDisabledGroup();
			if (string.IsNullOrEmpty(editor.WorkingDirectry)) {
				if (GUILayout.Button("作業ディレクトリの選択")) {
					string path = EditorUtility.OpenFolderPanel("作業ディレクトリの設定", Application.dataPath, string.Empty);
					this.serializedObject.FindProperty("_workingDirectry").stringValue = AssetPathUtil.GetAssetsPath(path);
					this._isOpenWorkingDirectry = false;
				}
			} else {
				if (GUILayout.Button("作業ディレクトリの変更")) {
					string path = EditorUtility.OpenFolderPanel("作業ディレクトリの変更", editor.WorkingDirectry, string.Empty);
					this.serializedObject.FindProperty("_workingDirectry").stringValue = AssetPathUtil.GetAssetsPath(path);
					this._isOpenWorkingDirectry = false;
				}
			}
		}

		private bool DrawSetup(ExpressionMenuEditor editor) {
			EditorGUILayout.PropertyField(this.serializedObject.FindProperty("_parameters"), new GUIContent("パラメータ"));
			if (editor.Parameters == null) {
				if (GUILayout.Button("パラメータの作成")) {
					string fileName = EditorUtility.SaveFilePanel("パラメータの保存先", editor.WorkingDirectry, "ExpressionParams", "asset");
					if (!string.IsNullOrEmpty(fileName)) {
						VRCExpressionParameters asset = CreateInstance<VRCExpressionParameters>();

						AssetDatabase.CreateAsset(asset, AssetPathUtil.GetAssetsPath(fileName));
						AssetDatabase.Refresh();
						this.serializedObject.FindProperty("_parameters").objectReferenceValue = asset;
						asset.parameters = new VRCExpressionParameters.Parameter[0];
					}
				}
				return false;
			}

			EditorGUILayout.PropertyField(this.serializedObject.FindProperty("_menuRoot"), new GUIContent("メインメニュー"));
			if (editor.MenuRoot == null) {
				if (GUILayout.Button("メインメニューの作成")) {
					string fileName = EditorUtility.SaveFilePanel("メインメニュー保存先", editor.WorkingDirectry, "RootMenu", "asset");
					if (!string.IsNullOrEmpty(fileName)) {
						VRCExpressionsMenu asset = CreateInstance<VRCExpressionsMenu>();

						AssetDatabase.CreateAsset(asset, AssetPathUtil.GetAssetsPath(fileName));
						AssetDatabase.Refresh();
						this.serializedObject.FindProperty("_menuRoot").objectReferenceValue = asset;
					}
				}
				return false;
			}
			return true;
		}

		private void DrawParams(VRCExpressionParameters vrcParameters) {
			ExpressionMenuEditor editor = this.target as ExpressionMenuEditor;
			editor.IsOpenParams = EditorGUILayout.Foldout(editor.IsOpenParams, "パラメータ");
			if (!editor.IsOpenParams) return;

			SerializedObject parametersSerializedObject = new SerializedObject(vrcParameters);
			parametersSerializedObject.Update();

			SerializedProperty parameters = parametersSerializedObject.FindProperty("parameters");

			if (this._parameterReorderableList == null) {
				this._parameterReorderableList = new ReorderableList(parametersSerializedObject, parameters);
				this._parameterReorderableList.onAddCallback += (list) => {
					parametersSerializedObject.Update();
					parameters.arraySize++;
					parametersSerializedObject.ApplyModifiedProperties();
				};

				this._parameterReorderableList.onRemoveCallback += (list) => {
					parametersSerializedObject.Update();
					parameters.DeleteArrayElementAtIndex(list.index);
					parametersSerializedObject.ApplyModifiedProperties();
				};

				this._parameterReorderableList.drawElementCallback += (rect, index, isActive, isFocused) => {
					parametersSerializedObject.Update();

					if (parameters.arraySize < index + 1) {
						parameters.InsertArrayElementAtIndex(index);
					}

					SerializedProperty item = parameters.GetArrayElementAtIndex(index);

					SerializedProperty name = item.FindPropertyRelative("name");
					SerializedProperty valueType = item.FindPropertyRelative("valueType");
					SerializedProperty defaultValue = item.FindPropertyRelative("defaultValue");
					SerializedProperty saved = item.FindPropertyRelative("saved");

					rect.width -= 10f * 3f;
					rect.width /= 2f;
					EditorGUI.PropertyField(rect, name, new GUIContent(""));

					rect.x += rect.width + 10f;
					rect.width /= 4f;
					EditorGUI.PropertyField(rect, valueType, new GUIContent(""));

					rect.x += rect.width + 10f;
					rect.width *= 2f;
					switch ((VRCExpressionParameters.ValueType)valueType.intValue) {
						case VRCExpressionParameters.ValueType.Int:
							defaultValue.floatValue = Mathf.Clamp(EditorGUI.IntField(rect, (int)defaultValue.floatValue), 0, 255);
							break;
						case VRCExpressionParameters.ValueType.Float:
							defaultValue.floatValue = Mathf.Clamp(EditorGUI.FloatField(rect, defaultValue.floatValue), -1f, 1f);
							break;
						case VRCExpressionParameters.ValueType.Bool:
							defaultValue.floatValue = EditorGUI.Toggle(rect, defaultValue.floatValue != 0 ? true : false) ? 1f : 0f;
							break;
					}

					rect.x += rect.width + 10f;
					EditorGUI.PropertyField(rect, saved, new GUIContent(""));
					parametersSerializedObject.ApplyModifiedProperties();
				};

				this._parameterReorderableList.drawHeaderCallback += (rect) => {
					rect.x += 10f;
					rect.width -= 10f;
					rect.width -= 10f * 3f;
					rect.width /= 2f;
					EditorGUI.LabelField(rect, "名前");
					rect.x += rect.width + 10f;
					rect.width /= 4f;
					EditorGUI.LabelField(rect, "種類");
					rect.x += rect.width + 10f;
					rect.width *= 2f;
					EditorGUI.LabelField(rect, "デフォルト");
					rect.x += rect.width + 10f;
					EditorGUI.LabelField(rect, "保存");
				};
			}
			this._parameterReorderableList.DoLayoutList();


			int cost = vrcParameters.CalcTotalCost();
			if (cost <= VRCExpressionParameters.MAX_PARAMETER_COST) {
				EditorGUILayout.HelpBox($"使用メモリ: {cost}/{VRCExpressionParameters.MAX_PARAMETER_COST}", MessageType.Info);
			} else {
				EditorGUILayout.HelpBox($"使用メモリ: {cost}/{VRCExpressionParameters.MAX_PARAMETER_COST}\nパラメータのメモリ使用量が多すぎます。 パラメータを削除するか、メモリ使用量の少ないboolを使用してください。", MessageType.Error);
			}

			EditorGUILayout.HelpBox("ここで定義されたパラメータのみが、エクスプレッションメニューと全レイヤー間の同期、ネットワークを介したリモートクライアントへの同期で使用できます。", MessageType.Info);
			EditorGUILayout.HelpBox("パラメータ名とタイプは、アニメーションコントローラに定義されているパラメータと一致している必要があります。", MessageType.Info);
			EditorGUILayout.HelpBox("デフォルトのアニメーションコントローラーが使用するパラメータ(オプション)\nVRCEmote, Int\nVRCFaceBlendH, Float\nVRCFaceBlendV, Float", MessageType.Info);

			if (GUILayout.Button("パラメータの削除")) {
				if (EditorUtility.DisplayDialogComplex("警告", "本当にすべてのパラメータを削除しますか?", "削除", "キャンセル", "") == 0) {
					parameters.ClearArray();
					this._parameterReorderableList = null;
				}
			}
			parametersSerializedObject.ApplyModifiedProperties();
		}

		private MenuContent _rootMenuContent;

		private void DrawMenu(VRCExpressionsMenu menu, VRCExpressionParameters parameters) {
			ExpressionMenuEditor editor = this.target as ExpressionMenuEditor;
			editor.IsOpenMenu = EditorGUILayout.Foldout(editor.IsOpenMenu, "メニュー");
			if (!editor.IsOpenMenu) return;

			if (menu == null || parameters == null) {
				this._rootMenuContent = null;
				return;
			}

			if (this._rootMenuContent == null || this._rootMenuContent.Menu != menu || this._rootMenuContent.Parameters != parameters || this._rootMenuContent.WorkingDirectry != editor.WorkingDirectry) {
				this._rootMenuContent = new MenuContent(menu, parameters, editor.WorkingDirectry);
			}

			this._rootMenuContent.DrawMenu();

		}


	}

}