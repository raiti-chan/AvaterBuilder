using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raitichan.Script.Util;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Assets.Raitichan.Script.VRCAvatarBuilder.Editor {
	public class MenuContent {

		private VRCExpressionsMenu _menu;
		public VRCExpressionsMenu Menu { get => this._menu; }

		private VRCExpressionParameters _parameters;
		public VRCExpressionParameters Parameters { get => this._parameters; }

		private string _wrokingDirectry;
		public string WorkingDirectry { get => this._wrokingDirectry; }

		private bool[] _isMenuOpen = new bool[8];
		private MenuContent[] _subMenus = new MenuContent[8];

		public MenuContent(VRCExpressionsMenu menu, VRCExpressionParameters parameters, string workingDirectry) {
			this._menu = menu;
			this._parameters = parameters;
			this._wrokingDirectry = workingDirectry;
		}

		public void DrawMenu() {
			SerializedObject serializedObject = new SerializedObject(this._menu);
			serializedObject.Update();
			SerializedProperty controls = serializedObject.FindProperty(nameof(VRCExpressionsMenu.controls));
			EditorGUI.indentLevel++;

			for (int i = 0; i < controls.arraySize; i++) {
				SerializedProperty control = controls.GetArrayElementAtIndex(i);
				EditorGUILayout.BeginVertical(GUI.skin.box);
				this.DrawControl(controls, control, i);
				EditorGUILayout.EndVertical();
			}

			EditorGUI.BeginDisabledGroup(controls.arraySize >= VRCExpressionsMenu.MAX_CONTROLS);
			{
				if (GUILayout.Button("コントロール追加")) {
					int addIndex = controls.arraySize;
					controls.InsertArrayElementAtIndex(addIndex);
					controls.GetArrayElementAtIndex(addIndex).FindPropertyRelative(nameof(VRCExpressionsMenu.Control.name))
						.stringValue = "新規コントロール";
				}
			}
			EditorGUI.EndDisabledGroup();

			EditorGUI.indentLevel--;
			serializedObject.ApplyModifiedProperties();
		}


		private void DrawControl(SerializedProperty controls, SerializedProperty control, int index) {
			SerializedProperty name = control.FindPropertyRelative(nameof(VRCExpressionsMenu.Control.name));
			SerializedProperty icon = control.FindPropertyRelative(nameof(VRCExpressionsMenu.Control.icon));
			SerializedProperty type = control.FindPropertyRelative(nameof(VRCExpressionsMenu.Control.type));
			SerializedProperty parameter = control.FindPropertyRelative(nameof(VRCExpressionsMenu.Control.parameter));
			SerializedProperty value = control.FindPropertyRelative(nameof(VRCExpressionsMenu.Control.value));
			SerializedProperty subMenu = control.FindPropertyRelative(nameof(VRCExpressionsMenu.Control.subMenu));
			SerializedProperty subParameters = control.FindPropertyRelative(nameof(VRCExpressionsMenu.Control.subParameters));
			SerializedProperty labels = control.FindPropertyRelative(nameof(VRCExpressionsMenu.Control.labels));

			VRCExpressionsMenu.Control.ControlType controlType = (VRCExpressionsMenu.Control.ControlType)type.intValue;

			this._isMenuOpen[index] = EditorGUILayout.Foldout(this._isMenuOpen[index], name.stringValue);
			this.DrawConrtoleButtons(controls, control, index);
			if (!this._isMenuOpen[index]) return;


			if (controls.arraySize <= index) return;


			EditorGUI.indentLevel++;
			{
				EditorGUILayout.PropertyField(name);
				EditorGUILayout.PropertyField(icon);
				EditorGUILayout.PropertyField(type);


				this.DrawControleInfo(controlType);

				this.DrawParameterDropDown(parameter, "パラメータ");
				this.DrawParameterValue(parameter, value);

				EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

				switch (controlType) {
					case VRCExpressionsMenu.Control.ControlType.TwoAxisPuppet:
						subParameters.arraySize = 2;
						labels.arraySize = 4;

						DrawParameterDropDown(subParameters.GetArrayElementAtIndex(0), "横軸", false);
						DrawParameterDropDown(subParameters.GetArrayElementAtIndex(1), "縦軸", false);

						DrawLabel(labels.GetArrayElementAtIndex(0), "ラベル 上");
						DrawLabel(labels.GetArrayElementAtIndex(1), "ラベル 右");
						DrawLabel(labels.GetArrayElementAtIndex(2), "ラベル 下");
						DrawLabel(labels.GetArrayElementAtIndex(3), "ラベル 左");
						break;
					case VRCExpressionsMenu.Control.ControlType.FourAxisPuppet:
						subParameters.arraySize = 4;
						labels.arraySize = 4;

						DrawParameterDropDown(subParameters.GetArrayElementAtIndex(0), "上", false);
						DrawParameterDropDown(subParameters.GetArrayElementAtIndex(1), "右", false);
						DrawParameterDropDown(subParameters.GetArrayElementAtIndex(2), "下", false);
						DrawParameterDropDown(subParameters.GetArrayElementAtIndex(3), "左", false);

						DrawLabel(labels.GetArrayElementAtIndex(0), "ラベル 上");
						DrawLabel(labels.GetArrayElementAtIndex(1), "ラベル 右");
						DrawLabel(labels.GetArrayElementAtIndex(2), "ラベル 下");
						DrawLabel(labels.GetArrayElementAtIndex(3), "ラベル 左");
						break;
					case VRCExpressionsMenu.Control.ControlType.RadialPuppet:
						subParameters.arraySize = 1;
						labels.arraySize = 0;

						DrawParameterDropDown(subParameters.GetArrayElementAtIndex(0), "パラメータ", false);
						break;
					case VRCExpressionsMenu.Control.ControlType.SubMenu:
						EditorGUILayout.PropertyField(subMenu);
						if (subMenu.objectReferenceValue is VRCExpressionsMenu sub) {
							bool isOpen = EditorGUILayout.Foldout(this._subMenus[index] != null, "サブメニュー");
							if (isOpen) {
								if (this._subMenus[index] == null || this._subMenus[index].Menu != sub) {
									this._subMenus[index] = new MenuContent(sub, this.Parameters, this.WorkingDirectry);
								}
							} else {
								this._subMenus[index] = null;
							}
							if (this._subMenus[index] != null) {
								EditorGUILayout.BeginVertical(GUI.skin.box);
								this._subMenus[index].DrawMenu();
								EditorGUILayout.EndVertical();
							}
						} else {
							if (GUILayout.Button("サブメニュー作成")) {
								string fileName = EditorUtility.SaveFilePanel("サブメニュー保存先", this.WorkingDirectry, name.stringValue, "asset");
								if (!string.IsNullOrEmpty(fileName)) {
									VRCExpressionsMenu asset = ScriptableObject.CreateInstance<VRCExpressionsMenu>();

									AssetDatabase.CreateAsset(asset, AssetPathUtil.GetAssetsPath(fileName));
									AssetDatabase.Refresh();
									subMenu.objectReferenceValue = asset;
								}
							}
						}
						subParameters.arraySize = 0;
						labels.arraySize = 0;
						break;
					default:
						subParameters.arraySize = 0;
						labels.arraySize = 0;
						break;
				}

			}
			EditorGUI.indentLevel--;

		}

		private void DrawConrtoleButtons(SerializedProperty controls, SerializedProperty control, int index) {
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			if (GUILayout.Button("Up", GUILayout.Width(64))) {
				if (index > 0) {
					controls.MoveArrayElement(index, index - 1);
				}
			}
			if (GUILayout.Button("Down", GUILayout.MaxWidth(64))) {
				if (index < controls.arraySize - 1) {
					controls.MoveArrayElement(index, index + 1);
				}
			}
			if (GUILayout.Button("Delete", GUILayout.Width(64))) {
				controls.DeleteArrayElementAtIndex(index);
			}

			GUILayout.EndHorizontal();
		}

		private void DrawControleInfo(VRCExpressionsMenu.Control.ControlType controlType) {
			switch (controlType) {
				case VRCExpressionsMenu.Control.ControlType.Button:
					EditorGUILayout.HelpBox("ホールドで起動します。ボタンは最短で0.2sの間、アクティブになります。", MessageType.Info);
					break;
				case VRCExpressionsMenu.Control.ControlType.Toggle:
					EditorGUILayout.HelpBox("オンとオフが切り替わります。\nオンにすると、(パラメータ)が(値)に設定されます。\nオフにすると、（パラメータ）がゼロになります。", MessageType.Info);
					break;
				case VRCExpressionsMenu.Control.ControlType.SubMenu:
					EditorGUILayout.HelpBox("別の表現メニューを開きます。\n開くと（パラメータ）が（値）に設定され、\n閉じると（パラメータ）がゼロになります。", MessageType.Info);
					break;
				case VRCExpressionsMenu.Control.ControlType.TwoAxisPuppet:
					EditorGUILayout.HelpBox("ジョイスティックを2つのパラメータ（-1～＋1）に対応させるパペットメニューです。\n開くと（パラメータ）が（値）に設定され、閉じると（パラメータ）がゼロにリセットされます。", MessageType.Info);
					break;
				case VRCExpressionsMenu.Control.ControlType.FourAxisPuppet:
					EditorGUILayout.HelpBox("ジョイスティックを4つのパラメータの（0～1）に対応させるパペットメニューです。\n開くと（パラメータ）が（値）に設定され、閉じると（パラメータ）が0にリセットされます。", MessageType.Info);
					break;
				case VRCExpressionsMenu.Control.ControlType.RadialPuppet:
					EditorGUILayout.HelpBox("ジョイスティックの回転に応じて値を設定するパペットメニューです。(0～1)\n開くと(パラメータ)が(値)に設定されます。\n閉じると(Parameter)が0になります。", MessageType.Info);
					break;
			}
		}

		private void DrawParameterDropDown(SerializedProperty parameter, string name, bool allowBool = true) {
			SerializedProperty paramName = parameter.FindPropertyRelative(nameof(VRCExpressionsMenu.Control.Parameter.name));
			string value = paramName.stringValue;

			VRCExpressionParameters.Parameter[] parameters = this.GetExpressionParameters(allowBool);
			int currentIndex = this.GetExpressionParameterIndex(value, allowBool) + 1;

			string[] parameterNames = parameters
				.Select(param => $"{param.name} : {param.valueType}")
				.Prepend("[None]")
				.ToArray();

			currentIndex = EditorGUILayout.Popup(name, currentIndex, parameterNames) - 1;

			if (currentIndex == -1) {
				paramName.stringValue = "";
			} else if (currentIndex == -2) {
				EditorGUILayout.HelpBox($"\"{paramName.stringValue}\" パラメータは指定されたパラメータリストに見つかりません。", MessageType.Warning);
			} else {
				paramName.stringValue = this.GetExpressionParameter(currentIndex, allowBool).name;
			}

		}

		private void DrawParameterValue(SerializedProperty parameter, SerializedProperty value) {
			string paramName = parameter.FindPropertyRelative(nameof(VRCExpressionsMenu.Control.Parameter.name)).stringValue;
			if (string.IsNullOrEmpty(paramName)) {
				return;
			}
			int index = this.GetExpressionParameterIndex(paramName);
			if (index < 0) {
				EditorGUI.BeginDisabledGroup(true);
				value.floatValue = EditorGUILayout.FloatField("値", value.floatValue);
				EditorGUI.EndDisabledGroup();
				return;
			}
			VRCExpressionParameters.Parameter param = this.GetExpressionParameter(index);
			switch (param.valueType) {
				case VRCExpressionParameters.ValueType.Int:
					value.floatValue = EditorGUILayout.IntField("値", Mathf.Clamp((int)value.floatValue, 0, 255));
					break;
				case VRCExpressionParameters.ValueType.Float:
					value.floatValue = EditorGUILayout.FloatField("値", Mathf.Clamp(value.floatValue, -1f, 1f));
					break;
				case VRCExpressionParameters.ValueType.Bool:
					EditorGUI.BeginDisabledGroup(true);
					value.floatValue = 1f;
					EditorGUILayout.Toggle("値", true);
					EditorGUI.EndDisabledGroup();
					break;
			}
		}

		private void DrawLabel(SerializedProperty subControl, string name) {
			SerializedProperty nameProperty = subControl.FindPropertyRelative(nameof(VRCExpressionsMenu.Control.Label.name));
			SerializedProperty icon = subControl.FindPropertyRelative(nameof(VRCExpressionsMenu.Control.Label.icon));

			EditorGUILayout.LabelField(name);
			EditorGUI.indentLevel += 2;
			EditorGUILayout.PropertyField(nameProperty);
			EditorGUILayout.PropertyField(icon);
			EditorGUI.indentLevel -= 2;

		}


		private VRCExpressionParameters.Parameter[] GetExpressionParameters(bool allowBool = true) {
			return this.Parameters.parameters
				.Where(param => allowBool || param.valueType != VRCExpressionParameters.ValueType.Bool)
				.ToArray();
		}

		private VRCExpressionParameters.Parameter GetExpressionParameter(int index, bool allowBool = true) {
			return this.Parameters.parameters
				.Where(param => allowBool || param.valueType != VRCExpressionParameters.ValueType.Bool)
				.ElementAt(index);
		}

		private int GetExpressionParameterIndex(string name, bool allowBool = true) {
			if (string.IsNullOrEmpty(name)) {
				return -1;
			}

			return this.Parameters.parameters
				.Where(param => allowBool || param.valueType != VRCExpressionParameters.ValueType.Bool)
				.Select((param, i) => (param.name, i))
				.Where(item => item.name == name)
				.Select(item => item.i)
				.DefaultIfEmpty(-2)
				.First();
		}
	}
}
