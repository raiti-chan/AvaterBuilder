using UnityEngine;

#if UNITY_EDITOR
using Raitichan.Script.VRCAvatarBuilder.Context;
#endif

namespace Raitichan.Script.VRCAvatarBuilder.Module {
	public abstract class VRCAvatarBuilderModuleBase : MonoBehaviour {
#if UNITY_EDITOR
		public virtual void Build(VRCAvatarBuilderContext context) { }
#endif
	}
}