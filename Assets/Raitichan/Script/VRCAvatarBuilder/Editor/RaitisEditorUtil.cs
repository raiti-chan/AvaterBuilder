using System;
using UnityEditor;
using UnityEngine;

namespace Raitichan.Script.VRCAvatarBuilder.Editor {
	public static class RaitisEditorUtil {
		private static readonly GUIStyle FOLDOUT_STYLE = new GUIStyle("ShurikenModuleTitle") {
			font = new GUIStyle(EditorStyles.label).font,
			border = new RectOffset(15, 7, 2, 2),
			fontSize = 12,
			fixedHeight = 26,
			contentOffset = new Vector2(20, -2),
		};

		private static readonly GUIStyle BUTTON_MID = new GUIStyle("ButtonMid") {
			font = new GUIStyle(EditorStyles.label).font,
			fontSize = 12,
			fontStyle = FontStyle.Bold
		};

		private static readonly GUIContent FOLDOUT_MENU_ICON = new GUIContent {
			image = (Texture2D)EditorGUIUtility.Load("pane options")
		};
		
		private static readonly Color TOGGLE_BUTTON_ACTIVE_COLOR = new Color(0.5f, 1, 0.5f, 1);

		public static bool ToggleButton(string text, bool active, string activeText = null) {
			GUI.backgroundColor = active ? TOGGLE_BUTTON_ACTIVE_COLOR : Color.white;
			if (GUILayout.Button(active ? activeText ?? text : text)) {
				active = !active;
			}
			GUI.backgroundColor = Color.white;
			return active;
		}

		public static bool HelpBoxWithButton(string message, MessageType type, string buttonText) {
			EditorGUILayout.HelpBox(message, type);
			Rect buttonRect = GUILayoutUtility.GetLastRect();
			buttonRect.x += buttonRect.width - 110;
			buttonRect.width = 100;
			buttonRect.y += 5;
			buttonRect.height -= 10;
			return GUI.Button(buttonRect, buttonText);
		}

		public static bool HeaderFoldout(bool isOpen, string title) {
			EditorGUILayout.BeginHorizontal();

			Rect rect = GUILayoutUtility.GetRect(16, 19, BUTTON_MID);
			rect.x = 0;
			rect.width = EditorGUIUtility.currentViewWidth;
			rect.height = 22;
			GUI.Box(rect, title, BUTTON_MID);

			Rect toggleRect = new Rect {
				x = rect.x + 4,
				y = rect.y + 2,
				width = 16,
				height = 16
			};
			
			Event e = Event.current;
			// ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
			switch (e.type) {
				case EventType.Repaint:
					EditorStyles.foldout.Draw(toggleRect, false, false, isOpen, false);
					break;
				case EventType.MouseDown when rect.Contains(e.mousePosition):
					isOpen = !isOpen;
					e.Use();
					break;
			}
			
			
			GUILayout.EndHorizontal();
			return isOpen;
		}

		public static void Footer() {
			Rect rect = GUILayoutUtility.GetRect(16, 2, BUTTON_MID);
			rect.x = 0;
			rect.width = EditorGUIUtility.currentViewWidth;
			GUI.Box(rect, "", BUTTON_MID);
		}

		public static bool Foldout(bool isOpen, string title, Action<object, int> func = null, object data = null, params string[] menuContent) {
			GUILayout.Space(2);
			EditorGUILayout.BeginHorizontal();
			Rect rect = GUILayoutUtility.GetRect(16, 20, FOLDOUT_STYLE);
			rect = EditorGUI.IndentedRect(rect);
			GUI.Box(rect, title, FOLDOUT_STYLE);

			Rect toggleRect = new Rect {
				x = rect.x + 4,
				y = rect.y + 3,
				width = 16,
				height = 16
			};

			Rect buttonRect = new Rect {
				x = rect.x + rect.width - 24,
				y = rect.y + 3,
				width = 16,
				height = 16
			};

			Event e = Event.current;

			if (func != null) {
				GUI.Box(buttonRect, FOLDOUT_MENU_ICON, GUIStyle.none);
			}

			// ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
			switch (e.type) {
				case EventType.Repaint:
					EditorStyles.foldout.Draw(toggleRect, false, false, isOpen, false);
					break;
				case EventType.MouseDown when func != null && buttonRect.Contains(e.mousePosition):
					GenericMenu toolMenu = new GenericMenu();
					for (int i = 0; i < menuContent.Length; i++) {
						toolMenu.AddItem(new GUIContent(menuContent[i]), false, OnHoldoutDropdownMenuClick,
							new HoldoutMenuData(func, data, i));
					}

					Rect dropDownRect = new Rect {
						x = buttonRect.x,
						y = buttonRect.y + 16,
					};
					toolMenu.DropDown(dropDownRect);
					break;
				case EventType.MouseDown when rect.Contains(e.mousePosition):
					isOpen = !isOpen;
					e.Use();
					break;
			}

			GUILayout.EndHorizontal();
			GUILayout.Space(2);
			return isOpen;
		}

		private class HoldoutMenuData {
			private readonly Action<object, int > _action;
			private readonly object _data;
			private readonly int _index;

			public HoldoutMenuData(Action<object, int> action, object data, int index) {
				this._action = action;
				this._data = data;
				this._index = index;
			}

			public void Invoke() {
				this._action(this._data, this._index);
			}
		}

		private static void OnHoldoutDropdownMenuClick(object data) {
			if (data is HoldoutMenuData menuData) {
				menuData.Invoke();
			}
		}
	}
}