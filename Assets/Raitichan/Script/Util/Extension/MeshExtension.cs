using System.Collections.Generic;
using UnityEngine;

namespace Raitichan.Script.Util.Extension {
	public static class MeshExtension {
		
		/// <summary>
		/// 全てのシェイプキーの名前を取得します。
		/// 順序はインデックスと一致します。
		/// </summary>
		/// <param name="mesh"></param>
		/// <returns></returns>
		public static string[] GetAllBlendShapeName(this Mesh mesh) {
			int count = mesh.blendShapeCount;
			List<string> names = new List<string>();
			for (int i = 0; i < count; i++) {
				names.Add(mesh.GetBlendShapeName(i));
			}

			return names.ToArray();
		}
	}
}