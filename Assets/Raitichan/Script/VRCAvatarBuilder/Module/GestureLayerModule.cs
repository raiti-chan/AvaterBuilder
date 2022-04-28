using Raitichan.Script.Util.Enum;
#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using Raitichan.Script.VRCAvatarBuilder.AnimatorControllerGenerator;
using UnityEngine;
using UnityEditor;
using Raitichan.Script.VRCAvatarBuilder.Context;
using UnityEditor.Animations;
#endif

namespace Raitichan.Script.VRCAvatarBuilder.Module {
	[AddComponentMenu("Raitichan/VRCAvatarBuilder/Module/GestureLayerModule")]
	public class GestureLayerModule : VRCAvatarBuilderModuleBase {
#if UNITY_EDITOR

		#region UseDifferentAnimation Parameter

		[SerializeField] private bool _useDifferentAnimation;

		public bool UseDifferentAnimation {
			get => this._useDifferentAnimation;
			set {
				if (this._useDifferentAnimation == value) return;
				this.BeginUpdate();
				this._useDifferentAnimation = value;
				this.Update();
			}
		}

		public static string UseDifferentAnimationPropertyName => nameof(_useDifferentAnimation);

		#endregion

		#region LeftAnimation Parameter

		[SerializeField] private AnimationClip[] _leftAnimation = new AnimationClip[8];

		public AnimationClip[] LeftAnimation {
			get => this._leftAnimation;
			set {
				if (this._leftAnimation == value) return;
				this.BeginUpdate();
				this._leftAnimation = value;
				this.Update();
			}
		}

		public static string LeftAnimationPropertyName => nameof(_leftAnimation);

		#endregion

		#region RightAnimation Parameter

		[SerializeField] private AnimationClip[] _rightAnimation = new AnimationClip[8];

		public AnimationClip[] RightGesture {
			get => this._rightAnimation;
			set {
				if (this._rightAnimation == value) return;
				this.BeginUpdate();
				this._rightAnimation = value;
				this.Update();
			}
		}

		public static string RightAnimationPropertyName => nameof(_rightAnimation);

		#endregion

		#region ModuleMethod

		public override void Build(VRCAvatarBuilderContext context) {
			AnimatorController controller =
				AssetDatabase.LoadAssetAtPath<AnimatorController>(ConstantPath.DEFAULT_GESTURE_LAYER);

			Dictionary<string, Motion> leftReplaceData = CreateReplaceData(this._leftAnimation);
			ReplaceMotionByStateNameLayerGenerator generator = new ReplaceMotionByStateNameLayerGenerator {
				SrcController = controller,
				TempFileDir = context.OutputPath,
				ReplaceData = {
					[1] = leftReplaceData,
					[2] = this.UseDifferentAnimation ? CreateReplaceData(this._rightAnimation) : leftReplaceData
				}
			};
			context.AnimatorControllerLayerGenerators.GestureLayerGenerators.Add(generator);
		}

		// ReSharper disable once SuggestBaseTypeForParameter
		private static Dictionary<string, Motion> CreateReplaceData(AnimationClip[] gestureClips) {
			Dictionary<string, Motion> data = new Dictionary<string, Motion>();
			for (int i = 0; i < gestureClips.Length; i++) {
				string key = ((GestureTypes)i).ToString();
				data[key] = gestureClips[i];
			}

			return data;
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