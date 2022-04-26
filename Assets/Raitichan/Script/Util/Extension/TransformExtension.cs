#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Assets.Raitichan.Script.BoneRemapper;
using UnityEngine;

namespace Raitichan.Script.Util.Extension {
	public static class TransformExtension {

		public static IEnumerable<GameObject> ChildObjects(this Transform transform) {
			return from Transform child in transform select child.gameObject;
		}

		public static Transform Find(this Transform transform, BoneTreeItem item) {
			return item == null ? null : transform.Cast<Transform>().FirstOrDefault(child => item.Match(child.name));
		}

		public static Transform Next(this Transform transform, Transform before) {
			bool flag = false;
			foreach (Transform child in transform) {
				if (flag) return child;
				flag = child == before;
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
#endif