using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Raitichan.Script.VRCAvatarBuilder.Editor.Dialog {
	public class SingleObjectDialog : EditorWindow {
		public static T DisplayDialog<T>(string title, string message, bool allowSceneObjects) where T : class {
			SingleObjectDialog dialog = CreateInstance<SingleObjectDialog>();
			dialog.titleContent = new GUIContent(title);
			dialog._message = message;
			dialog._type = typeof(T);
			dialog._allowSceneObjects = allowSceneObjects;
			dialog.ShowModalUtility();
			if (dialog._result) {
				return dialog._obj as T;
			}
			return null;
		}

		private string _message;
		private Type _type;
		private bool _allowSceneObjects;
		private Object _obj;

		private bool _result;
		

		private void OnGUI() {
			GUILayout.Label(this._message);
			this._obj = EditorGUILayout.ObjectField(_obj, _type, this._allowSceneObjects);
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("OK")) {
				this._result = true;
				this.Close();
			}

			if (GUILayout.Button("Cansel")) {
				this._result = false;
				this.Close();
			}
			GUILayout.EndHorizontal();
		}
	}
}