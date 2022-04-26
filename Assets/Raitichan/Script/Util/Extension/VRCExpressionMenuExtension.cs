#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Raitichan.Script.Util.Extension {
	public static class VRCExpressionMenuExtension {
		public static VRCExpressionsMenu DeepClone(this VRCExpressionsMenu src) {
			VRCExpressionsMenu cloned = Object.Instantiate(src);
			cloned.name = src.name;
			Undo.RegisterCreatedObjectUndo(cloned, "Clone ExpressionMenu");
			Dictionary<VRCExpressionsMenu, VRCExpressionsMenu> clonedList =
				new Dictionary<VRCExpressionsMenu, VRCExpressionsMenu> {
					[src] = cloned
				};
			foreach (VRCExpressionsMenu.Control clonedControl in cloned.controls.Where(clonedControl =>
				         clonedControl.subMenu != null)) {
				clonedControl.subMenu = clonedControl.subMenu.DeepClone(clonedList);
			}

			return cloned;
		}

		// ReSharper disable once SuggestBaseTypeForParameter
		private static VRCExpressionsMenu DeepClone(this VRCExpressionsMenu src,
			Dictionary<VRCExpressionsMenu, VRCExpressionsMenu> clonedList) {
			if (clonedList.TryGetValue(src, out VRCExpressionsMenu alreadyCloned)) {
				// すでにクローンされている物の場合それを返す。
				return alreadyCloned;
			}

			VRCExpressionsMenu cloned = Object.Instantiate(src);
			cloned.name = src.name;
			Undo.RegisterCreatedObjectUndo(cloned, "Clone ExpressionMenu");
			clonedList[src] = cloned;
			foreach (VRCExpressionsMenu.Control clonedControl in cloned.controls.Where(clonedControl =>
				         clonedControl.subMenu != null)) {
				clonedControl.subMenu = clonedControl.subMenu.DeepClone(clonedList);
			}

			return cloned;
		}

		/// <summary>
		/// サブメニューを自身のサブアセットとして保存します。
		/// すでにどこかに保存されている場合は保存されません。
		/// </summary>
		/// <param name="content"></param>
		public static void SaveSubMenu(this VRCExpressionsMenu content) {
			string savePath = AssetDatabase.GetAssetPath(content);
			if (string.IsNullOrEmpty(savePath)) return; // contentがアセットとして保存されていない場合
			
			foreach (VRCExpressionsMenu.Control control in content.controls.Where(control => control.subMenu != null)) {
				VRCExpressionsMenu subMenu = control.subMenu;
				if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(subMenu))) continue;
				subMenu.name = content.name + "/" + subMenu.name;
				AssetDatabase.AddObjectToAsset(subMenu, savePath);
				subMenu.SaveSubMenu();
			}
		}
	}
}
#endif