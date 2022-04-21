using System.Collections.Immutable;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;

namespace Raitichan.Script.Util.Editor.Extension {
	public static class AnimatorControllerExtension {
		public static void AppendLayer(this AnimatorController dst, AnimatorController src, int layerIndex) {
			AnimatorControllerLayer srcLayer = src.layers[layerIndex];

			StateMachineCloner cloner = new StateMachineCloner(srcLayer.stateMachine);
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
		}

		public static void AppendLayer(this AnimatorController controller, AnimatorController content) {
			// TODO: 実際はコントローラー全体のクローン用のコード(SyncedLayer辺り)の実装が必要
		}
	}
}