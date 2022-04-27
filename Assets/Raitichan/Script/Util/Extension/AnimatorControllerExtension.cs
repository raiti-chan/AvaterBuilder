#if UNITY_EDITOR
using System.Collections.Immutable;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;

namespace Raitichan.Script.Util.Extension {
	public static class AnimatorControllerExtension {
		/// <summary>
		/// ソースの指定されたレイヤーを複製し、自身に追加します。
		/// 指定されたレイヤーがSyncedLayerの場合このメソッドは失敗します。
		/// </summary>
		/// <param name="dst">複製先</param>
		/// <param name="src">ソース</param>
		/// <param name="layerIndex">複製するレイヤーのインデックス</param>
		/// <returns>追加に失敗た場合false</returns>
		public static bool AppendLayer(this AnimatorController dst, AnimatorController src, int layerIndex) {
			AnimatorControllerLayer srcLayer = src.layers[layerIndex];
			if (srcLayer.syncedLayerIndex != -1) return false;
			StateMachineCloner cloner = new Util.StateMachineCloner(srcLayer.stateMachine);
			AnimatorStateMachine stateMachine = cloner.CloneStateMachine();
			cloner.SaveAsset(dst);
			AnimatorControllerLayer cloned = new AnimatorControllerLayer {
				name = dst.MakeUniqueLayerName(srcLayer.name),
				stateMachine = stateMachine,
				avatarMask = srcLayer.avatarMask,
				blendingMode = srcLayer.blendingMode,
				syncedLayerIndex = srcLayer.syncedLayerIndex,
				iKPass = srcLayer.iKPass,
				defaultWeight = srcLayer.defaultWeight,
				syncedLayerAffectsTiming = srcLayer.syncedLayerAffectsTiming
			};

			dst.AddLayer(cloned);

			ImmutableHashSet<string> dstUsedParameterNames = dst.parameters.Select(o => o.name).ToImmutableHashSet();

			foreach (AnimatorControllerParameter srcParameter in
			         src.parameters.Where(o =>
				         cloner.IsUsedPropertyName(o.name) && !dstUsedParameterNames.Contains(o.name))) {
				AnimatorControllerParameter clonedParameter = new AnimatorControllerParameter {
					name = srcParameter.name,
					type = srcParameter.type,
					defaultFloat = srcParameter.defaultFloat,
					defaultInt = srcParameter.defaultInt,
					defaultBool = srcParameter.defaultBool
				};
				dst.AddParameter(clonedParameter);
			}

			return true;
		}

		public static void AppendLayerAll(this AnimatorController dst, AnimatorController src) {
			int dstLayerSize = dst.layers.Length;
			StateMachineCloner[] cloners = new StateMachineCloner[src.layers.Length];
			for (int i = 0; i < src.layers.Length; i++) {
				AnimatorControllerLayer srcLayer = src.layers[i];
				cloners[i] = new StateMachineCloner(srcLayer.stateMachine);
				AnimatorStateMachine stateMachine = cloners[i].CloneStateMachine();
				cloners[i].SaveAsset(dst);

				AnimatorControllerLayer cloned = new AnimatorControllerLayer {
					name = dst.MakeUniqueLayerName(srcLayer.name),
					stateMachine = stateMachine,
					avatarMask = srcLayer.avatarMask,
					blendingMode = srcLayer.blendingMode,
					syncedLayerIndex = srcLayer.syncedLayerIndex,
					iKPass = srcLayer.iKPass,
					defaultWeight = srcLayer.defaultWeight,
					syncedLayerAffectsTiming = srcLayer.syncedLayerAffectsTiming
				};
				

				if (cloned.syncedLayerIndex != -1) {
					// TODO: 後ろにあるレイヤーも参照できるのでその場合null
					cloners[cloned.syncedLayerIndex].CloneSyncedLayer(src, i, cloned);
					cloned.syncedLayerIndex += dstLayerSize;
				}

				dst.AddLayer(cloned);
			}

			ImmutableHashSet<string> dstUsedParameterNames = dst.parameters.Select(o => o.name).ToImmutableHashSet();
			foreach (AnimatorControllerParameter srcParameter in src.parameters.Where(o =>
				         !dstUsedParameterNames.Contains(o.name))) {
				AnimatorControllerParameter clonedParameter = new AnimatorControllerParameter {
					name = srcParameter.name,
					type = srcParameter.type,
					defaultFloat = srcParameter.defaultFloat,
					defaultInt = srcParameter.defaultInt,
					defaultBool = srcParameter.defaultBool
				};
				dst.AddParameter(clonedParameter);
			}
		}
	}
}
#endif