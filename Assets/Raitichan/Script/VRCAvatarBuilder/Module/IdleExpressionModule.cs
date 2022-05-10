using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using Raitichan.Script.VRCAvatarBuilder.AnimatorControllerGenerator;
using Raitichan.Script.VRCAvatarBuilder.Context;
#endif

namespace Raitichan.Script.VRCAvatarBuilder.Module {
	[AddComponentMenu("Raitichan/VRCAvatarBuilder/Module/GestureExpressionModule")]
	public class IdleExpressionModule : VRCAvatarBuilderModuleBase {
#if UNITY_EDITOR
		public enum DisplayMode {
			All,
			Used,
			NonUsed,
		}

		[Serializable]
		public struct SkinnedMeshRendererAdditionalData {
			[Serializable]
			public struct BlendShapeWeightData {
				public string Name;
				public int Index;
				public bool Use;
				public float Weight;
			}

			public SkinnedMeshRenderer Renderer;
			public bool IsOpen;
			public BlendShapeWeightData[] BlendShapeData;
		}

		#region SkinnedMeshRendererDisplayMode Parameter

		[SerializeField] private DisplayMode _skinnedMeshRendererDisplayMode;

		public DisplayMode SkinnedMeshRendererDisplayMode {
			get => this._skinnedMeshRendererDisplayMode;
			set {
				if (this._skinnedMeshRendererDisplayMode == value) return;
				this._skinnedMeshRendererDisplayMode = value;
				this.Update();
			}
		}

		public static string SkinnedMeshRendererDisplayModePropertyName => nameof(_skinnedMeshRendererDisplayMode);

		#endregion

		#region SkinnedMeshRendererHoldout Parameter

		[SerializeField] private List<SkinnedMeshRendererAdditionalData> _skinnedMeshRendererAdditional =
			new List<SkinnedMeshRendererAdditionalData>();

		public List<SkinnedMeshRendererAdditionalData> SkinnedMeshRendererAdditional {
			get {
				if (this._skinnedMeshRendererAdditional != null) return this._skinnedMeshRendererAdditional;
				this._skinnedMeshRendererAdditional = new List<SkinnedMeshRendererAdditionalData>();
				this.Update();
				return this._skinnedMeshRendererAdditional;
			}

			set {
				if (this._skinnedMeshRendererAdditional == value) return;
				this._skinnedMeshRendererAdditional = value;
				this.Update();
			}
		}

		public static string SkinnedMeshRendererAdditionalPropertyName => nameof(_skinnedMeshRendererAdditional);

		/// <summary>
		/// 追加データのインデックスを検索します。
		/// </summary>
		/// <param name="meshRenderer"></param>
		/// <returns></returns>
		public int FindAdditionalDataIndex(SkinnedMeshRenderer meshRenderer) {
			for (int i = 0; i < this._skinnedMeshRendererAdditional.Count; i++) {
				if (this._skinnedMeshRendererAdditional[i].Renderer == meshRenderer) return i;
			}

			return -1;
		}

		#endregion

		#region SimpleBlinkHoldout Parameter

		[SerializeField] private bool _simpleBlinkHoldout;

		public bool SimpleBlinkHoldout {
			get => this._simpleBlinkHoldout;
			set {
				if (this._simpleBlinkHoldout == value) return;
				this._simpleBlinkHoldout = value;
				this.Update();
			}
		}

		public static string SimpleBlinkHoldoutPropertyName => nameof(_simpleBlinkHoldout);


		#endregion
		
		#region UseSimpleBlink Parameter

		[SerializeField] private bool _useSimpleBlink;

		public bool UseSimpleBlink {
			get => this._useSimpleBlink;
			set {
				if (this._useSimpleBlink == value) return;
				this.BeginUpdate();
				this._useSimpleBlink = value;
				this.Update();
			}
		}

		public static string UseSimpleBlinkPropertyName => nameof(_useSimpleBlink);


		#endregion

		#region SimpleBlinkSkinnedMeshRenderer Parameter

		[SerializeField] private SkinnedMeshRenderer _simpleBlinkSkinnedMeshRenderer;

		public SkinnedMeshRenderer SimpleBlinkSkinnedMeshRenderer {
			get => this._simpleBlinkSkinnedMeshRenderer;
			set {
				if (this._simpleBlinkSkinnedMeshRenderer == value) return;
				this.BeginUpdate();
				this._simpleBlinkSkinnedMeshRenderer = value;
				this.Update();
			}
		}

		public static string SimpleBlinkSkinnedMeshRendererPropertyName => nameof(_simpleBlinkSkinnedMeshRenderer);


		#endregion

		#region SimpleBlinkBlendShapeIndex Parameter

		[SerializeField] private int[] _simpleBlinkBlendShapeIndex = new int[1];

		public int[] SimpleBlinkBlendShapeIndex {
			get => this._simpleBlinkBlendShapeIndex;
			set {
				if (this._simpleBlinkBlendShapeIndex == value) return;
				this.BeginUpdate();
				this._simpleBlinkBlendShapeIndex = value;
				this.Update();
			}
		}

		public static string SimpleBlinkBlendShapeIndexPropertyName => nameof(_simpleBlinkBlendShapeIndex);


		#endregion

		#region UseAdditionalAnimation Parameter

		[SerializeField] private bool _useAdditionalAnimation;

		public bool UseAdditionalAnimation {
			get => this._useAdditionalAnimation;
			set {
				if (this._useAdditionalAnimation == value) return;
				this.BeginUpdate();
				this._useAdditionalAnimation = value;
				this.Update();
			}
		}

		public static string UseAdditionalAnimationPropertyName => nameof(_useAdditionalAnimation);


		#endregion

		#region AdditionalAnimation Parameter

		[SerializeField] private AnimationClip _additionalAnimation;

		public AnimationClip AdditionalAnimation {
			get => this._additionalAnimation;
			set {
				if (this._additionalAnimation == value) return;
				this.BeginUpdate();
				this._additionalAnimation = value;
				this.Update();
			}
		}

		public static string AdditionalAnimationPropertyName => nameof(_additionalAnimation);


		#endregion


		public override void Build(VRCAvatarBuilderContext context) {
			context.AnimatorControllerLayerGenerators.FxLayerGenerators.Add(
				new IdleExpressionGenerator(this)
			);
		}

#endif
	}
}