using UnityEditor;
using UnityEditor.Animations;

namespace Raitichan.Script.VRCAvatarBuilder.Module.Editor {
	[CustomEditor(typeof(DefaultAdditiveLayerModule))]
	public class DefaultAdditiveLayerModuleEditor : UnityEditor.Editor {
		private AnimatorController _controller;
		private void OnEnable() {
			this._controller =
				AssetDatabase.LoadAssetAtPath<AnimatorController>(ConstantPath.DEFAULT_ADDITIVE_LAYER);
		}

		public override void OnInspectorGUI() {
			EditorGUILayout.HelpBox(Strings.DefaultAdditiveLayerModuleEditor_Info, MessageType.Info);
			if (this._controller == null) {
				EditorGUILayout.HelpBox(Strings.DefaultAdditiveLayerModuleEditor_NotFoundDefaultController,
					MessageType.Error);
			} else {
				using (new EditorGUI.DisabledScope(true)) {
					EditorGUILayout.ObjectField(this._controller, typeof(AnimatorController), false);
				}
			}
		}
	}
}