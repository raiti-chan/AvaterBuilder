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
		private readonly IdleExpressionModule _module;
		private readonly VRCAvatarDescriptor _avatar;

		private readonly AnimatorController _controller;
		private readonly AnimationClip _blinkClip;

		public IdleExpressionGenerator(IdleExpressionModule module) {
			this._module = module;
			this._avatar = module.GetTargetBuilder().AvatarDescriptor;
			this._controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ConstantPath.UTIL_IDLE_LAYER);
			this._blinkClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(ConstantPath.BLINK_ANIM_FILE_PATH);
		}

		public void Generate(AnimatorController controller) {
			AnimationClip clip = new AnimationClip {
				name = ConstantPath.IDLE_EXPRESSION_ANIMATION_FILE_NAME,
			};
			{
				SerializedObject clipObject = new SerializedObject(clip);
				clipObject.FindProperty("m_AnimationClipSettings.m_LoopTime").boolValue = true;
				clipObject.ApplyModifiedPropertiesWithoutUndo();
			}
			foreach (SkinnedMeshRendererAdditionalData data in this._module.SkinnedMeshRendererAdditional) {
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

			if (this._module.UseSimpleBlink && this._blinkClip != null) {
				EditorCurveBinding blinkBinding = AnimationUtility.GetCurveBindings(this._blinkClip)[0];
				AnimationCurve curve = AnimationUtility.GetEditorCurve(this._blinkClip, blinkBinding);
				SkinnedMeshRenderer renderer = this._module.SimpleBlinkSkinnedMeshRenderer;

				EditorCurveBinding binding = new EditorCurveBinding {
					path = AnimationUtility.CalculateTransformPath(renderer.transform, this._avatar.transform),
					type = typeof(SkinnedMeshRenderer)
				};

				foreach (int blendShapeIndex in this._module.SimpleBlinkBlendShapeIndex) {
					if (blendShapeIndex == -1) continue;
					string blendShapeName = renderer.sharedMesh.GetBlendShapeName(blendShapeIndex);
					EditorCurveBinding shapeBinding = binding;
					shapeBinding.propertyName = ConstantPath.BLEND_SHAPE_PROPERTY_NAME_PREFIX + blendShapeName;
					AnimationUtility.SetEditorCurve(clip, shapeBinding, curve);
				}
			}

			if (this._module.UseAdditionalAnimation && this._module.AdditionalAnimation != null) {
				foreach (EditorCurveBinding binding in
				         AnimationUtility.GetCurveBindings(this._module.AdditionalAnimation)) {
					if (binding.isPPtrCurve) {
						ObjectReferenceKeyframe[] frames =
							AnimationUtility.GetObjectReferenceCurve(this._module.AdditionalAnimation, binding);
						AnimationUtility.SetObjectReferenceCurve(clip, binding, frames);
					} else {
						AnimationCurve curve =
							AnimationUtility.GetEditorCurve(this._module.AdditionalAnimation, binding);
						AnimationUtility.SetEditorCurve(clip, binding, curve);
					}
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