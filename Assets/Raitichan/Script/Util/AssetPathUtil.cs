#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;

namespace Raitichan.Script.Util {
	public static class AssetPathUtil {

		private static readonly char[] _INVALID_CHARS = Path.GetInvalidPathChars();

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
		
		/// <summary>
		/// ファイル名から無効な文字を指定した文字に置き換えます。
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		/// <param name="replace">置き換える文字</param>
		/// <returns>置き換えられたファイル名</returns>
		public static string ReplaceInvalidFileNameChars(string fileName, char replace) {
			return string.Concat(fileName.Select(c => _INVALID_CHARS.Contains(c) ? replace : c));
		}
	}
}
#endif