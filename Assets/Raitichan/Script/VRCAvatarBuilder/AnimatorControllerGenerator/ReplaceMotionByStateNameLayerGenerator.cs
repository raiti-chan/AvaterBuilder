#if UNITY_EDITOR
using System.Collections.Generic;
using Raitichan.Script.Util.Extension;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Raitichan.Script.VRCAvatarBuilder.AnimatorControllerGenerator {
	public class ReplaceMotionByStateNameLayerGenerator : IAnimatorControllerLayerGenerator {
		public Dictionary<int, Dictionary<string, Motion>> ReplaceData { get; set; }
		public AnimatorController SrcController { get; set; }
		public string TempFileDir { get; set; }

		private string _tempFilePath;

		public ReplaceMotionByStateNameLayerGenerator() {
			this.ReplaceData = new Dictionary<int, Dictionary<string, Motion>>();
		}

		public void Generate(AnimatorController controller) {
			AnimatorController srcClonedController = this.CopyController();

			for (int i = 0; i < srcClonedController.layers.Length; i++) {
				if (!this.ReplaceData.TryGetValue(i, out Dictionary<string, Motion> replaceDictionary)) continue;
				if (replaceDictionary == null) continue;

				AnimatorControllerLayer targetLayer = srcClonedController.layers[i];
				int syncedLayerIndex = targetLayer.syncedLayerIndex;

				if (syncedLayerIndex == -1) {
					IEnumerable<AnimatorState> allStates = targetLayer.stateMachine.GetAllState();
					foreach (AnimatorState state in allStates) {
						if (replaceDictionary.TryGetValue(state.name, out Motion motion)) {
							state.motion = motion;
						}
					}
				} else {
					IEnumerable<AnimatorState> allStates =
						srcClonedController.layers[syncedLayerIndex].stateMachine.GetAllState();

					foreach (AnimatorState animatorState in allStates) {
						if (replaceDictionary.TryGetValue(animatorState.name, out Motion motion)) {
							targetLayer.SetOverrideMotion(animatorState, motion);
						}
					}
				}
			}

			controller.AppendLayerAll(srcClonedController);
			this.DeleteCopiedController();
		}

		private AnimatorController CopyController() {
			string path = AssetDatabase.GetAssetPath(SrcController);
			this._tempFilePath = AssetDatabase.GenerateUniqueAssetPath(TempFileDir + "/ReplaceTemp.controller");
			AssetDatabase.CopyAsset(path, this._tempFilePath);
			AnimatorController copyController = AssetDatabase.LoadAssetAtPath<AnimatorController>(this._tempFilePath);
			return copyController;
		}

		private void DeleteCopiedController() {
			AssetDatabase.DeleteAsset(this._tempFilePath);
		}
	}
}
#endif