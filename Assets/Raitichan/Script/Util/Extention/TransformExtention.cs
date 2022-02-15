using Assets.Raitichan.Script.BoneRemapper;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Raitichan.Script.Util.Extention {
	public static class TransformExtention {

		public static IEnumerable<Transform> Childs(this Transform transform) {
			for (int i = 0; i < transform.childCount; i++) {
				yield return transform.GetChild(i);
				
			}
		}

		public static Transform Find(this Transform transform, BoneTreeItem item) {
			if (item == null) {
				return null;
			}
			foreach(Transform child in transform.Childs()) {
				if (item.Match(child.name)) {
					return child;
				}
			}
			return null;
		}

		public static Matrix4x4 GetLocalMatrix(this Transform transform) {
			return Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);
		}
		
		public static Matrix4x4 GetWorldMatrix(this Transform transform) {
			return Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
		}

	}
}
