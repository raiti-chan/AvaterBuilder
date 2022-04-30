using UnityEngine;

#if UNITY_EDITOR
using Raitichan.Script.VRCAvatarBuilder.Context;
using UnityEditor;
#endif

namespace Raitichan.Script.VRCAvatarBuilder.Module {
	public abstract class VRCAvatarBuilderModuleBase : MonoBehaviour {
#if UNITY_EDITOR

		#region IsOpenInAvatarBuilder Parameter

		[SerializeField] private bool _isOpenInAvatarBuilder;

		public bool IsOpenInAvatarBuilder {
			get => this._isOpenInAvatarBuilder;
			set {
				if (this._isOpenInAvatarBuilder == value) return;
				this._isOpenInAvatarBuilder = value;
				this.Update();
			}
		}

		public static string IsOpenInAvatarBuilderPropertyName => nameof(_isOpenInAvatarBuilder);


		#endregion
		
		public virtual void Build(VRCAvatarBuilderContext context) { }

		public VRCAvatarBuilder GetTargetBuilder() {
			return this.GetComponentInParent<VRCAvatarBuilder>();
		}
		
		#region Private Method

		protected void BeginUpdate() {
			Undo.RecordObject(this, "Change Property");
		}

		protected void Update() {
			EditorUtility.SetDirty(this);
		}

		#endregion
#endif
	}
}