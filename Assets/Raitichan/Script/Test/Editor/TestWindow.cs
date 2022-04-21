using Raitichan.Script.Util.Editor.Extension;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using StateMachineCloner = Raitichan.Script.Util.Editor.StateMachineCloner;

namespace Raitichan.Script.Test.Editor {
	public class TestWindow : EditorWindow {
		[MenuItem("Raitichan/Test/TestWindow")]
		private static void ShowWindow() {
			TestWindow window = GetWindow<TestWindow>();
			window.titleContent = new GUIContent("Test Window");
			window.Show();
		}

		private AnimatorController _baseController;
		private AnimatorController _addController;

		public void OnGUI() {
			this._baseController =
				EditorGUILayout.ObjectField("base", this._baseController, typeof(AnimatorController), false) as
					AnimatorController;
			this._addController =
				EditorGUILayout.ObjectField("add", this._addController, typeof(AnimatorController), false) as
					AnimatorController;
			if (!GUILayout.Button("Execute")) return;

			if (this._addController == null) return;
			if (this._baseController == null) return;
			
			this._baseController.AppendLayer(this._addController, 0);
		}
	}
}