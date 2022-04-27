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

		#region LeftGesture Parameter

		[SerializeField] private AnimationClip[] _leftGesture = new AnimationClip[8];

		public AnimationClip[] LeftGesture {
			get => this._leftGesture;
			set {
				if (this._leftGesture == value) return;
				this.BeginUpdate();
				this._leftGesture = value;
				this.Update();
			}
		}

		public static string LeftAnimationPropertyName => nameof(_leftGesture);

		#endregion

		#region RightGesture Parameter

		[SerializeField] private AnimationClip[] _rightGesture = new AnimationClip[8];

		public AnimationClip[] RightGesture {
			get => this._rightGesture;
			set {
				if (this._rightGesture == value) return;
				this.BeginUpdate();
				this._rightGesture = value;
				this.Update();
			}
		}

		public static string RightGesturePropertyName => nameof(_rightGesture);

		#endregion

		#region ModuleMethod

		public override void Build(VRCAvatarBuilderContext context) {
			AnimatorController controller =
				AssetDatabase.LoadAssetAtPath<AnimatorController>(ConstantPath.DEFAULT_GESTURE_LAYER);

			Dictionary<string, Motion> leftReplaceData = CreateReplaceData(this._leftGesture);
			ReplaceMotionByStateNameLayerGenerator generator = new ReplaceMotionByStateNameLayerGenerator {
				SrcController = controller,
				TempFileDir = context.OutputPath,
				ReplaceData = {
					[1] = leftReplaceData,
					[2] = this.UseDifferentAnimation ? CreateReplaceData(this._rightGesture) : leftReplaceData
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