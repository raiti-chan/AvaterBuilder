using System;
using System.Collections.Generic;

namespace Raitichan.Script.VRCAvatarBuilder {
	public static class Strings {
		#region Others

		public enum Languages {
			日本語,
			English
		}

		public static Languages Lang { set; get; } = Languages.日本語;

		#endregion

		#region Language

		private static readonly string[] _Language = {
			"言語",
			"Language"
		};

		public static string Language => _Language[(int)Lang];

		#endregion

		#region TargetAvatar

		private static readonly string[] _TargetAvatar = {
			"対象アバター",
			"Target Avatar"
		};

		public static string TargetAvatar => _TargetAvatar[(int)Lang];

		#endregion

		#region WorkingDirectory

		private static readonly string[] _WorkingDirectory = {
			"作業ディレクトリ",
			"Working Directory"
		};

		public static string WorkingDirectory => _WorkingDirectory[(int)Lang];

		#endregion

		#region AvatarScale

		private static readonly string[] _AvatarScale = {
			"アバタースケール",
			"Avatar Scale"
		};

		public static string AvatarScale => _AvatarScale[(int)Lang];

		#endregion

		#region UploadScene

		private static readonly string[] _UploadScene = {
			"アップロード用シーン",
			"Upload Scene"
		};

		public static string UploadScene => _UploadScene[(int)Lang];

		#endregion

		#region Build

		private static readonly string[] _Build = {
			"ビルド",
			"Build"
		};

		public static string Build => _Build[(int)Lang];

		#endregion

	}
}