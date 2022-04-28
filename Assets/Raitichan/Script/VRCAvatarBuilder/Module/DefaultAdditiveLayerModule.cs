using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Animations;
using Raitichan.Script.VRCAvatarBuilder.AnimatorControllerGenerator;
using Raitichan.Script.VRCAvatarBuilder.Context;
using UnityEditor;
#endif

namespace Raitichan.Script.VRCAvatarBuilder.Module {
	[AddComponentMenu("Raitichan/VRCAvatarBuilder/Module/DefaultAdditiveLayerModule")]
	public class DefaultAdditiveLayerModule : VRCAvatarBuilderModuleBase {
#if UNITY_EDITOR

		public override void Build(VRCAvatarBuilderContext context) {
			AnimatorController controller =
				AssetDatabase.LoadAssetAtPath<AnimatorController>(ConstantPath.DEFAULT_ADDITIVE_LAYER);

			context.AnimatorControllerLayerGenerators.AdditiveLayerGenerators.Add(
				new SimpleLayerGenerator {
					SrcController = controller,
				});
		}

#endif
	}
}