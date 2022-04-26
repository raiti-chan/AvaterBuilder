#if UNITY_EDITOR
using System.Collections.Generic;
using System.ComponentModel;
using Raitichan.Script.VRCAvatarBuilder.AnimatorControllerGenerator;
using AnimLayerType = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor.AnimLayerType;

namespace Raitichan.Script.VRCAvatarBuilder.Context {
	public class VRCAvatarBuilderContext {
		public VRCAvatarBuilder Builder { get; set; }

		public AnimatorControllerGenerators AnimatorControllerGenerators { get; set; }


		public VRCAvatarBuilderContext(VRCAvatarBuilder builder) {
			this.Builder = builder;
			this.AnimatorControllerGenerators = new AnimatorControllerGenerators();
		}
	}

	public class AnimatorControllerGenerators {
		public List<IAnimatorControllerLayerGenerator> BaseLayerGenerators { get; set; }
		public List<IAnimatorControllerLayerGenerator> AdditiveLayerGenerators { get; set; }
		public List<IAnimatorControllerLayerGenerator> GestureLayerGenerators { get; set; }
		public List<IAnimatorControllerLayerGenerator> ActionLayerGenerators { get; set; }
		public List<IAnimatorControllerLayerGenerator> FxLayerGenerators { get; set; }
		public List<IAnimatorControllerLayerGenerator> SittingLayerGenerators { get; set; }
		public List<IAnimatorControllerLayerGenerator> TPoseLayerGenerators { get; set; }
		public List<IAnimatorControllerLayerGenerator> IKPoseLayerGenerators { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <exception cref="InvalidEnumArgumentException">指定されたタイプが無効な場合</exception>
		public List<IAnimatorControllerLayerGenerator> this[AnimLayerType type] {
			get {
				switch (type) {
					case AnimLayerType.Base:
						return this.BaseLayerGenerators;
					case AnimLayerType.Additive:
						return this.AdditiveLayerGenerators;
					case AnimLayerType.Gesture:
						return this.GestureLayerGenerators;
					case AnimLayerType.Action:
						return this.ActionLayerGenerators;
					case AnimLayerType.FX:
						return this.FxLayerGenerators;
					case AnimLayerType.Sitting:
						return this.SittingLayerGenerators;
					case AnimLayerType.TPose:
						return this.TPoseLayerGenerators;
					case AnimLayerType.IKPose:
						return this.IKPoseLayerGenerators;
					case AnimLayerType.Deprecated0:
					default:
						throw new InvalidEnumArgumentException(
							$"Invalid Animation LayerType : {nameof(type)} = {type}");
				}
			}

			set {
				switch (type) {
					case AnimLayerType.Base:
						this.BaseLayerGenerators = value;
						return;
					case AnimLayerType.Additive:
						this.AdditiveLayerGenerators = value;
						return;
					case AnimLayerType.Gesture:
						this.GestureLayerGenerators = value;
						return;
					case AnimLayerType.Action:
						this.ActionLayerGenerators = value;
						return;
					case AnimLayerType.FX:
						this.FxLayerGenerators = value;
						return;
					case AnimLayerType.Sitting:
						this.SittingLayerGenerators = value;
						return;
					case AnimLayerType.TPose:
						this.TPoseLayerGenerators = value;
						return;
					case AnimLayerType.IKPose:
						this.IKPoseLayerGenerators = value;
						return;
					case AnimLayerType.Deprecated0:
					default:
						throw new InvalidEnumArgumentException(
							$"Invalid Animation LayerType : {nameof(type)} = {type}");
				}
			}
		}

		public AnimatorControllerGenerators() {
			this.BaseLayerGenerators = new List<IAnimatorControllerLayerGenerator>();
			this.AdditiveLayerGenerators = new List<IAnimatorControllerLayerGenerator>();
			this.GestureLayerGenerators = new List<IAnimatorControllerLayerGenerator>();
			this.ActionLayerGenerators = new List<IAnimatorControllerLayerGenerator>();
			this.FxLayerGenerators = new List<IAnimatorControllerLayerGenerator>();
			this.SittingLayerGenerators = new List<IAnimatorControllerLayerGenerator>();
			this.TPoseLayerGenerators = new List<IAnimatorControllerLayerGenerator>();
			this.IKPoseLayerGenerators = new List<IAnimatorControllerLayerGenerator>();
		}
	}
}

#endif