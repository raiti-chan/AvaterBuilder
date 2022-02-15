using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Raitichan.Script.VRCAvatarBuilder.Editor {
	public static class Util {

		public static string GetAssetsPath(string fullPath) {
			int startIndex = fullPath.IndexOf("Assets/", StringComparison.Ordinal);
			if (startIndex == -1) startIndex = fullPath.IndexOf("Assets\\", StringComparison.Ordinal);
			if (startIndex == -1) return "";

			string assetPath = fullPath.Substring(startIndex);

			return assetPath;
		}
	}
}
