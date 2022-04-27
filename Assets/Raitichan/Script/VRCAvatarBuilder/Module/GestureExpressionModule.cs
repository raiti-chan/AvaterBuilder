using System.Collections.Generic;
using Raitichan.Script.Util.Enum;
using Raitichan.Script.VRCAvatarBuilder.AnimatorControllerGenerator;
using Raitichan.Script.VRCAvatarBuilder.Context;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace Raitichan.Script.VRCAvatarBuilder.Module {
	/// <summary>
	/// ジェスチャーによって切り替わる表情を追加するモジュール
	/// </summary>
	public class GestureExpressionModule : VRCAvatarBuilderModuleBase {
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

		[SerializeField] private AnimationClip[] _leftAnimation = new AnimationClip[7];

		public AnimationClip[] LeftGesture {
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

		[SerializeField] private AnimationClip[] _rightAnimation = new AnimationClip[7];

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
				AssetDatabase.LoadAssetAtPath<AnimatorController>(ConstantPath.DEFAULT_FX_EXPRESSION_LAYER);

			Dictionary<string, Motion> leftReplaceData = CreateReplaceData(this._leftAnimation);
			ReplaceMotionByStateNameLayerGenerator generator = new ReplaceMotionByStateNameLayerGenerator {
				SrcController = controller,
				TempFileDir = context.OutputPath,
				ReplaceData = {
					[0] = leftReplaceData,
					[1] = this.UseDifferentAnimation ? CreateReplaceData(this._rightAnimation) : leftReplaceData
				}
			};
			context.AnimatorControllerLayerGenerators.FxLayerGenerators.Add(generator);
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