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

		#region OK

		private static readonly string[] _OK = {
			"はい",
			"OK"
		};

		public static string OK => _OK[(int)Lang];

		#endregion

		#region Cansel

		private static readonly string[] _Cansel = {
			"キャンセル",
			"Cansel"
		};

		public static string Cansel => _Cansel[(int)Lang];

		#endregion

		#region Warning

		private static readonly string[] _Warning = {
			"警告",
			"Warning"
		};

		public static string Warning => _Warning[(int)Lang];

		#endregion

		#region TargetAvatar

		private static readonly string[] _TargetAvatar = {
			"対象アバター",
			"Target Avatar"
		};

		public static string TargetAvatar => _TargetAvatar[(int)Lang];

		#endregion

		#region TargetLayer

		private static readonly string[] _TargetLayer = {
			"対象レイヤー",
			"Target Layer"
		};

		public static string TargetLayer => _TargetLayer[(int)Lang];

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

		#region AutoFix

		private static readonly string[] _AutoFix = {
			"自動修正",
			"Auto Fix"
		};

		public static string AutoFix => _AutoFix[(int)Lang];

		#endregion

		#region LeftHand

		private static readonly string[] _LeftHand = {
			"左手",
			"Left Hand"
		};

		public static string LeftHand => _LeftHand[(int)Lang];

		#endregion

		#region RightHand

		private static readonly string[] _RightHand = {
			"右手",
			"Right Hand"
		};

		public static string RightHand => _RightHand[(int)Lang];

		#endregion

		#region AnimatorController

		private static readonly string[] _AnimatorController = {
			"コントローラー",
			"Controller"
		};

		public static string AnimatorController => _AnimatorController[(int)Lang];

		#endregion

		#region NotFoundIdleTemplateLayer

		private static readonly string[] _NotFoundIdleTemplateLayer = {
			"Idle用テンプレートレイヤーが見つかりません。\n" +
			"アセットを再度インポートしてください。",
			"Cannot find template layer for Idle.\n" +
			"Please re-import the asset."
		};

		public static string NotFoundIdleTemplateLayer => _NotFoundIdleTemplateLayer[(int)Lang];

		#endregion


		#region DefaultBaseLayerModuleEditor

		#region DefaultBaseLayerModuleEditor_Info

		private static readonly string[] _DefaultBaseLayerModuleEditor_Info = {
			"このモジュールは、Baseレイヤーにデフォルトのレイヤーを組み込みます。",
			"This module incorporates a default layer into the Base layer."
		};

		public static string DefaultBaseLayerModuleEditor_Info => _DefaultBaseLayerModuleEditor_Info[(int)Lang];

		#endregion

		#region DefaultBaseLayerModueEditor_NotFoundDefaultController

		private static readonly string[] _DefaultBaseLayerModuleEditor_NotFoundDefaultController = {
			"デフォルトのBaseレイヤーが見つかりません。\n" +
			"アセットを再度インポートしてください",
			"Default Base layer not found.\n" +
			"Please re-import the asset."
		};

		public static string DefaultBaseLayerModuleEditor_NotFoundDefaultController =>
			_DefaultBaseLayerModuleEditor_NotFoundDefaultController[(int)Lang];

		#endregion

		#endregion


		#region DefaultAdditiveLayerModuleEditor

		#region DefaultAdditiveLayerModuleEditor_Info

		private static readonly string[] _DefaultAdditiveLayerModuleEditor_Info = {
			"このモジュールは、Additiveレイヤーにデフォルトのレイヤーを組み込みます。",
			"This module incorporates a default layer into the Additive layer."
		};

		public static string DefaultAdditiveLayerModuleEditor_Info => _DefaultAdditiveLayerModuleEditor_Info[(int)Lang];

		#endregion

		#region DefaultAdditiveLayerModueEditor_NotFoundDefaultController

		private static readonly string[] _DefaultAdditiveLayerModuleEditor_NotFoundDefaultController = {
			"デフォルトのAdditiveレイヤーが見つかりません。\n" +
			"アセットを再度インポートしてください",
			"Default Additive layer not found.\n" +
			"Please re-import the asset."
		};

		public static string DefaultAdditiveLayerModuleEditor_NotFoundDefaultController =>
			_DefaultAdditiveLayerModuleEditor_NotFoundDefaultController[(int)Lang];

		#endregion

		#endregion


		#region DefaultActionLayerModuleEditor

		#region DefaultActionLayerModuleEditor_Info

		private static readonly string[] _DefaultActionLayerModuleEditor_Info = {
			"このモジュールは、Actionレイヤーにデフォルトのレイヤーを組み込みます。",
			"This module incorporates a default layer into the Action layer."
		};

		public static string DefaultActionLayerModuleEditor_Info => _DefaultActionLayerModuleEditor_Info[(int)Lang];

		#endregion

		#region DefaultActionLayerModueEditor_NotFoundDefaultController

		private static readonly string[] _DefaultActionLayerModuleEditor_NotFoundDefaultController = {
			"デフォルトのAdditiveレイヤーが見つかりません。\n" +
			"アセットを再度インポートしてください",
			"Default Additive layer not found.\n" +
			"Please re-import the asset."
		};

		public static string DefaultActionLayerModuleEditor_NotFoundDefaultController =>
			_DefaultActionLayerModuleEditor_NotFoundDefaultController[(int)Lang];

		#endregion

		#endregion


		#region GestureLayerModuleEditor

		#region GestureLayerModuleEditor_Info

		private static readonly string[] _GestureLayerModuleEditor_Info = {
			"このモジュールは標準的なGestureレイヤーを生成します。\n" +
			"指定したアニメーションでジェスチャーを上書きできます。",
			"This module generates a standard Gesture layer.\n" +
			"You can override the gesture with a specified animation."
		};

		public static string GestureLayerModuleEditor_Info => _GestureLayerModuleEditor_Info[(int)Lang];

		#endregion

		#region GestureLayerModuleEditor_UseDifferentAnimationProperty

		private static readonly string[] _GestureLayerModuleEditor_UseDifferentAnimationProperty = {
			"左右別々のアニメーションを指定する",
			"Specify separate animations for left and right."
		};

		public static string GestureLayerModuleEditor_UseDifferentAnimationProperty =>
			_GestureLayerModuleEditor_UseDifferentAnimationProperty[(int)Lang];

		#endregion

		#region GestureLayerModuleEditor_SetDefault

		private static readonly string[] _GestureLayerModuleEditor_SetDefault = {
			"標準設定",
			"Set Default"
		};

		public static string GestureLayerModuleEditor_SetDefault => _GestureLayerModuleEditor_SetDefault[(int)Lang];

		#endregion

		#region GestureLayerModuleEditor_SetDefault_WarningMessage

		private static readonly string[] _GestureLayerModuleEditor_SetDefault_WarningMessage = {
			"標準の設定に置き換えます。\n" +
			"現在の設定は失われます。\n" +
			"よろしいですか?",
			"Replace with standard settings.\n" +
			"Current settings will be lost. \n" +
			"Are you sure?"
		};

		public static string GestureLayerModuleEditor_SetDefault_WarningMessage =>
			_GestureLayerModuleEditor_SetDefault_WarningMessage[(int)Lang];

		#endregion

		#region GestureLayerModuleEditor_NotFoundDefaultController

		private static readonly string[] _GestureLayerModuleEditor_NotFoundDefaultController = {
			"デフォルトのGestureレイヤーが見つかりません。\n" +
			"アセットを再度インポートしてください",
			"Default Gesture layer not found.\n" +
			"Please re-import the asset."
		};

		public static string GestureLayerModuleEditor_NotFoundDefaultController =>
			_GestureLayerModuleEditor_NotFoundDefaultController[(int)Lang];

		#endregion

		#endregion

		#region GestureExpressionModuleEditor

		#region GestureExpressionModuleEditor_Info

		private static readonly string[] _GestureExpressionModuleEditor_Info = {
			"このモジュールは標準的なジェスチャーで切り替わる表情のレイヤーを生成します。",
			"This module generates a layer of facial expressions that switch with standard gestures."
		};

		public static string GestureExpressionModuleEditor_Info => _GestureExpressionModuleEditor_Info[(int)Lang];

		#endregion

		#region GestureExpressionModuleEditor_UseUserDefinedIdleAnimation

		private static readonly string[] _GestureExpressionModuleEditor_UseUserDefinedIdleAnimation = {
			"Idleレイヤーを生成する。",
			"Generate Idle layer."
		};

		public static string GestureExpressionModuleEditor_UseUserDefinedIdleAnimation =>
			_GestureExpressionModuleEditor_UseUserDefinedIdleAnimation[(int)Lang];

		#endregion

		#region GestureExpressionModuleEditor_NotFoundDefaultController

		private static readonly string[] _GestureExpressionModuleEditor_NotFoundDefaultController = {
			"デフォルトの表情用のテンプレートレイヤーが見つかりません。\n" +
			"アセットを再度インポートしてください。",
			"Cannot find the template layer for the default expression.\n" +
			"Please re-import the asset."
		};

		public static string GestureExpressionModuleEditor_NotFoundDefaultController =>
			_GestureExpressionModuleEditor_NotFoundDefaultController[(int)Lang];

		#endregion

		#endregion

		#region AnimatorControllerMergerModuleEditor

		#region AnimatorControllerMergerModuleEditor_Info

		private static readonly string[] _AnimatorControllerMergerModuleEditor_Info = {
			"このモジュールは指定されたレイヤーに複数のアニメーターを追加します。",
			"This module adds multiple animators to a given layer."
		};

		public static string AnimatorControllerMergerModuleEditor_Info => _AnimatorControllerMergerModuleEditor_Info[(int)Lang];

		#endregion

		#region AnimatorControllerMergerModuleEditor_FirstLayersWeightIsZero

		private static readonly string[] _AnimatorControllerMergerModuleEditor_FirstLayersWeightIsZero = {
			"一番上のレイヤーのウェイトが0です。",
			"The weight of the top layer is 0."
		};

		public static string AnimatorControllerMergerModuleEditor_FirstLayersWeightIsZero => _AnimatorControllerMergerModuleEditor_FirstLayersWeightIsZero[(int)Lang];

		#endregion

		#endregion
	}
}