using UnityEngine;

#if UNITY_EDITOR
using Raitichan.Script.VRCAvatarBuilder.Context;
using UnityEditor.Animations;
#endif

namespace Raitichan.Script.VRCAvatarBuilder.Module {
	public abstract class VRCAvatarBuilderModuleBase : MonoBehaviour {
#if UNITY_EDITOR
		public virtual void Build(VRCAvatarBuilderContext context) { }

		public VRCAvatarBuilder GetTargetBuilder() {
			return this.GetComponentInParent<VRCAvatarBuilder>();
		}
#endif
	}
}