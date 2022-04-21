using System;
using System.IO;

namespace Raitichan.Script.Util.Editor {
	public static class AssetPathUtil {

		public static string GetAssetsPath(string fullPath) {
			int startIndex = fullPath.IndexOf("Assets/", StringComparison.Ordinal);
			if (startIndex == -1) startIndex = fullPath.IndexOf("Assets\\", StringComparison.Ordinal);
			if (startIndex == -1) return "";

			string assetPath = fullPath.Substring(startIndex);

			return assetPath;
		}

		public static string GetFullPath(string assetPath) {
			return Path.GetFullPath(assetPath);
		}
	}
}
