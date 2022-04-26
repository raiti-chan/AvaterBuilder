#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Raitichan.Script.Util.Extension {
	public static class GameObjectExtension {

		public static IEnumerable<GameObject> Childs(this GameObject gameObject) {
			return gameObject.transform.ChildObjects();
		}

		public static bool IsSceneObject(this GameObject gameObject) {
			return gameObject.scene.name != null;
		}
#if UNITY_EDITOR
		public static bool IsPrefabObject(this GameObject gameObject) {
			return PrefabUtility.GetCorrespondingObjectFromOriginalSource(gameObject) != null;
		}
#endif

	}
}
#endif