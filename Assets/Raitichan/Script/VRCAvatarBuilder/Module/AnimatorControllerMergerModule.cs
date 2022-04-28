using UnityEngine;

#if UNITY_EDITOR
using Raitichan.Script.VRCAvatarBuilder.AnimatorControllerGenerator;
using UnityEditor;
using Raitichan.Script.VRCAvatarBuilder.Context;
using UnityEditor.Animations;
using AnimLayerType = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor.AnimLayerType;
using System.Collections.Generic;
#endif

namespace Raitichan.Script.VRCAvatarBuilder.Module {
	public class AnimatorControllerMergerModule : VRCAvatarBuilderModuleBase {
#if UNITY_EDITOR

		#region TargetLayerType Parameter

		[SerializeField] private AnimLayerType _targetLayerType = AnimLayerType.FX;

		public AnimLayerType TargetLayerType {
			get => this._targetLayerType;
			set {
				if (this._targetLayerType == value) return;
				this.BeginUpdate();
				this._targetLayerType = value;
				this.Update();
			}
		}

		public static string TargetLayerTypePropertyName => nameof(_targetLayerType);

		#endregion

		#region AnimatorControllers Parameter

		[SerializeField] private RuntimeAnimatorController[] _animatorControllers;

		public RuntimeAnimatorController[] AnimatorControllers {
			get => this._animatorControllers;
			set {
				if (this._animatorControllers == value) return;
				this.BeginUpdate();
				this._animatorControllers = value;
				this.Update();
			}
		}

		public static string AnimatorControllersPropertyName => nameof(_animatorControllers);

		#endregion

		#region ModuleMethod

		public override void Build(VRCAvatarBuilderContext context) {
			List<IAnimatorControllerLayerGenerator> targetList =
				context.AnimatorControllerLayerGenerators[this._targetLayerType];
			foreach (RuntimeAnimatorController runtimeAnimatorController in this._animatorControllers) {
				if (runtimeAnimatorController is AnimatorController animatorController) {
					targetList.Add(new SimpleLayerGenerator { SrcController = animatorController });
				}
			}
		}

		#endregion


		#region Private Method

		private void BeginUpdate() {
			Undo.RecordObject(this, "Change Property");
		}

		private void Update() {
			EditorUtility.SetDirty(this);
		}

		#endregion

#endif
	}
}