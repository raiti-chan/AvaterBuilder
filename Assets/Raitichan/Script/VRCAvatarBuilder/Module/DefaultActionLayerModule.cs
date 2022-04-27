#if UNITY_EDITOR
using Raitichan.Script.VRCAvatarBuilder.AnimatorControllerGenerator;
using Raitichan.Script.VRCAvatarBuilder.Context;
using UnityEditor;
using UnityEditor.Animations;

#endif

namespace Raitichan.Script.VRCAvatarBuilder.Module {
	public class DefaultActionLayerModule : VRCAvatarBuilderModuleBase {
#if UNITY_EDITOR

		public override void Build(VRCAvatarBuilderContext context) {
			AnimatorController controller =
				AssetDatabase.LoadAssetAtPath<AnimatorController>(ConstantPath.DEFAULT_ACTION_LAYER);

			context.AnimatorControllerLayerGenerators.ActionLayerGenerators.Add(
				new SimpleLayerGenerator {
					SrcController = controller,
				});
		}

#endif
	}
}