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


		public override void Build(VRCAvatarBuilderContext context) {
			context.AnimatorControllerLayerGenerators.FxLayerGenerators.Add(
				new IdleExpressionGenerator(this)
			);
		}

#endif
	}
}