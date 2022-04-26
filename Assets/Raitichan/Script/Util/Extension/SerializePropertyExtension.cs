#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Raitichan.Script.Util.Extension {
	public static class SerializePropertyExtension {
		/// <summary>
		/// <see cref="SerializedProperty"/>から<see cref="IEnumerable"/>オブジェクトを取得します。
		/// </summary>
		/// <param name="serializedProperty"></param>
		/// <returns></returns>
		public static IEnumerable<SerializedProperty> GetEnumerable(this SerializedProperty serializedProperty) {
			IEnumerator enumerator = serializedProperty.GetEnumerator();
			while (enumerator.MoveNext()) {
				yield return enumerator.Current as SerializedProperty;
			}
		}
	}
}
#endif