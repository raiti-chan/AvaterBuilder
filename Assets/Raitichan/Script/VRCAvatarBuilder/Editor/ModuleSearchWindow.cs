using System;
using System.Linq;
using System.Reflection;
using Raitichan.Script.VRCAvatarBuilder.Module;
using UnityEditor;
using UnityEngine;

namespace Raitichan.Script.VRCAvatarBuilder.Editor {
	[InitializeOnLoad]
	public class AllModules {
		public static readonly Type[] MODULE_TYPES;

		static AllModules() {
			MODULE_TYPES = Assembly.GetAssembly(typeof(VRCAvatarBuilderModuleBase))
				.GetTypes()
				.Where(type => type.IsSubclassOf(typeof(VRCAvatarBuilderModuleBase)) && !type.IsAbstract)
				.ToArray();
		}
	}

	public class ModuleSearchWindow : EditorWindow {
		private static readonly GUIStyle SEARCH_BOX = new GUIStyle("SearchTextField");
		private static readonly GUIStyle BACK_BUTTON = new GUIStyle("ButtonMid");

		private string _SearchText;

		public VRCAvatarBuilderEditor Inspector { get; set; }

		public GameObject AvatarBuilder { get; set; }

		private Vector2 _scrollPos;


		private void OnGUI() {
			GUI.Box(GUIUtility.ScreenToGUIRect(this.position), "", GUI.skin.window);
			GUILayout.Space(5);
			this.GUIContent();
			GUI.FocusControl("SearchBox");
		}

		// ReSharper disable Unity.PerformanceAnalysis
		private void GUIContent() {
			GUI.SetNextControlName("SearchBox");
			this._SearchText = GUILayout.TextField(this._SearchText, SEARCH_BOX);
			GUILayout.Space(2);
			Rect backButtonRect = GUILayoutUtility.GetRect(this.position.width, 25, BACK_BUTTON);
			backButtonRect.width -= 3;
			backButtonRect.x = 3;
			GUI.Box(backButtonRect, Strings.Module, BACK_BUTTON);
			this._scrollPos = GUILayout.BeginScrollView(this._scrollPos);
			foreach (Type module in AllModules.MODULE_TYPES) {
				if (!string.IsNullOrEmpty(this._SearchText) && !module.Name.Contains(this._SearchText)) continue;
				if (!GUILayout.Button(module.Name)) continue;
				string objectName =
					GameObjectUtility.GetUniqueNameForSibling(this.AvatarBuilder.transform, module.Name);
				GameObject obj = new GameObject(objectName, module) {
					transform = {
						parent = this.AvatarBuilder.transform
					}
				};
				Undo.RegisterCreatedObjectUndo(obj, "Add Module");
				this.Inspector.UpdateModuleList();
				this.Close();
			}
			GUILayout.EndScrollView();
		}
	}
}