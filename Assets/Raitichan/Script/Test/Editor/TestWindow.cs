using Raitichan.Script.Util.Extension;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;
using StateMachineCloner = Raitichan.Script.Util.StateMachineCloner;

namespace Raitichan.Script.Test.Editor {
	public class TestWindow : EditorWindow {
		[MenuItem("Raitichan/Test/TestWindow")]
		private static void ShowWindow() {
			TestWindow window = GetWindow<TestWindow>();
			window.titleContent = new GUIContent("Test Window");
			window.Show();
		}

		private VRCExpressionsMenu _target;

		public void OnGUI() {
			this._target =
				EditorGUILayout.ObjectField("target", this._target, typeof(VRCExpressionsMenu), false) as
					VRCExpressionsMenu;

			if (!GUILayout.Button("Execute")) return;

			if (this._target == null) return;
			VRCExpressionsMenu menu = this._target.DeepClone();
			string path = AssetDatabase.GetAssetPath(this._target);
			string savePath = path + "_copy.asset";
			AssetDatabase.CreateAsset(menu, savePath);
			menu.SaveSubMenu();
		}
	}
}