
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
using Raitichan.Script.VRCAvatarBuilder.AnimatorControllerGenerator;
using Raitichan.Script.VRCAvatarBuilder.Context;
#endif

namespace Raitichan.Script.VRCAvatarBuilder.Module {
	[AddComponentMenu("Raitichan/VRCAvatarBuilder/Module/DefaultBaseLayerModule")]
	public class DefaultBaseLayerModule : VRCAvatarBuilderModuleBase {
#if UNITY_EDITOR
		public override void Build(VRCAvatarBuilderContext context) {
			// TODO: Force Locomotion の際のアニメーターの切り替え
			AnimatorController controller =
				AssetDatabase.LoadAssetAtPath<AnimatorController>(ConstantPath.DEFAULT_BASE_FORCE_LOCOMOTION_LAYER);
			context.AnimatorControllerLayerGenerators.BaseLayerGenerators.Add(new SimpleLayerGenerator {
				SrcController = controller,
			});
		}
#endif
	}
}