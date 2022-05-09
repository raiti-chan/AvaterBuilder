using Raitichan.Script.Util.Extension;
using Raitichan.Script.VRCAvatarBuilder.Module;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using static Raitichan.Script.VRCAvatarBuilder.Module.IdleExpressionModule;
using static Raitichan.Script.VRCAvatarBuilder.Module.IdleExpressionModule.SkinnedMeshRendererAdditionalData;

namespace Raitichan.Script.VRCAvatarBuilder.AnimatorControllerGenerator {
	public class IdleExpressionGenerator : IAnimatorControllerLayerGenerator {
		private readonly IdleExpressionModule _target;
		private readonly VRCAvatarDescriptor _avatar;

		private readonly AnimatorController _controller;

		public IdleExpressionGenerator(IdleExpressionModule module) {
			this._target = module;
			this._avatar = module.GetTargetBuilder().AvatarDescriptor;
			this._controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ConstantPath.UTIL_IDLE_LAYER);
		}

		public void Generate(AnimatorController controller) {
			AnimationClip clip = new AnimationClip {
				name = ConstantPath.IDLE_EXPRESSION_ANIMATION_FILE_NAME
			};

			foreach (SkinnedMeshRendererAdditionalData data in this._target.SkinnedMeshRendererAdditional) {
				EditorCurveBinding binding = new EditorCurveBinding {
					path = AnimationUtility.CalculateTransformPath(data.Renderer.transform, this._avatar.transform),
					type = typeof(SkinnedMeshRenderer),
				};
				foreach (BlendShapeWeightData weightData in data.BlendShapeData) {
					if (!weightData.Use) continue;
					EditorCurveBinding shapeBinding = binding;
					shapeBinding.propertyName = ConstantPath.BLEND_SHAPE_PROPERTY_NAME_PREFIX + weightData.Name;

					AnimationCurve curve = new AnimationCurve();
					curve.AddKey(0f, weightData.Weight);

					AnimationUtility.SetEditorCurve(clip, shapeBinding, curve);
				}
			}

			int index = controller.layers.Length;
			controller.AppendLayer(this._controller, 0);
			controller.layers[index].stateMachine.states[0].state.motion = clip;
			AssetDatabase.AddObjectToAsset(clip, controller);
			AssetDatabase.SaveAssets();
		}
	}
}