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

		#region All

		private static readonly string[] _All = {
			"全て",
			"All"
		};

		public static string All => _All[(int)Lang];

		#endregion

		#region None

		private static readonly string[] _None = {
			"無し",
			"None"
		};

		public static string None => _None[(int)Lang];

		#endregion

		#region Warning

		private static readonly string[] _Warning = {
			"警告",
			"Warning"
		};

		public static string Warning => _Warning[(int)Lang];

		#endregion

		#region Error

		private static readonly string[] _Error = {
			"エラー",
			"Error"
		};

		public static string Error => _Error[(int)Lang];

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

		#region TargetShapeKey

		private static readonly string[] _TargetBlendShape = {
			"対象ブレンドシェイプ",
			"Target Blend Shape"
		};

		public static string TargetBlendShape => _TargetBlendShape[(int)Lang];

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

		#region Delete

		private static readonly string[] _Delete = {
			"削除",
			"Delete"
		};

		public static string Delete => _Delete[(int)Lang];

		#endregion

		#region Build

		private static readonly string[] _Build = {
			"ビルド",
			"Build"
		};

		public static string Build => _Build[(int)Lang];

		#endregion

		#region Preview

		private static readonly string[] _Preview = {
			"プレビュー",
			"Preview"
		};

		public static string Preview => _Preview[(int)Lang];

		#endregion

		#region ReleasePreview

		private static readonly string[] _ReleasePreview = {
			"プレビュー解除",
			"Release Preview"
		};

		public static string ReleasePreview => _ReleasePreview[(int)Lang];

		#endregion

		#region AllCheck

		private static readonly string[] _AllCheck = {
			"全てチェック",
			"All Check"
		};

		public static string AllCheck => _AllCheck[(int)Lang];

		#endregion

		#region Utility

		private static readonly string[] _Utility = {
			"ユーティリティ",
			"Utility"
		};

		public static string Utility => _Utility[(int)Lang];

		#endregion

		#region AutoFix

		private static readonly string[] _AutoFix = {
			"自動修正",
			"Auto Fix"
		};

		public static string AutoFix => _AutoFix[(int)Lang];

		#endregion

		#region Setting

		private static readonly string[] _Setting = {
			"設定",
			"Setting"
		};

		public static string Setting => _Setting[(int)Lang];

		#endregion

		#region AutoSetting

		private static readonly string[] _AutoSetting = {
			"自動設定",
			"Auto Setting"
		};

		public static string AutoSetting => _AutoSetting[(int)Lang];

		#endregion

		#region GoToObject

		private static readonly string[] _GoToObject = {
			"オブジェクトへ",
			"Go to Object"
		};

		public static string GoToObject => _GoToObject[(int)Lang];

		#endregion

		#region Module

		private static readonly string[] _Module = {
			"モジュール",
			"Module"
		};

		public static string Module => _Module[(int)Lang];

		#endregion

		#region AddModule

		private static readonly string[] _AddModule = {
			"モジュールの追加",
			"Add Module"
		};

		public static string AddModule => _AddModule[(int)Lang];

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

		#region BaseLayer

		private static readonly string[] _BaseLayer = {
			"ベース レイヤー",
			"Base Layer"
		};

		public static string BaseLayer => _BaseLayer[(int)Lang];

		#endregion

		#region AdditiveLayer

		private static readonly string[] _AdditiveLayer = {
			"Additive レイヤー",
			"Additive Layer"
		};

		public static string AdditiveLayer => _AdditiveLayer[(int)Lang];

		#endregion

		#region GestureLayer

		private static readonly string[] _GestureLayer = {
			"ジェスチャー レイヤー",
			"Gesture Layer"
		};

		public static string GestureLayer => _GestureLayer[(int)Lang];

		#endregion

		#region ActionLayer

		private static readonly string[] _ActionLayer = {
			"アクション レイヤー",
			"Action Layer"
		};

		public static string ActionLayer => _ActionLayer[(int)Lang];

		#endregion

		#region GestureExpressionLayer

		private static readonly string[] _GestureExpressionLayer = {
			"ジェスチャー表情 レイヤー",
			"Gesture Expression Layer"
		};

		public static string GestureExpressionLayer => _GestureExpressionLayer[(int)Lang];

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

		#region NotFoundEmptyTemplateLayer

		private static readonly string[] _NotFoundEmptyTemplateLayer = {
			"空のテンプレートレイヤーが見つかりません\n" +
			"アセットを再度インポートしてください。",
			"Empty template layer not found.\n" +
			"Please re-import the asset."
		};

		public static string NotFoundEmptyTemplateLayer => _NotFoundEmptyTemplateLayer[(int)Lang];

		#endregion

		#region EmptyTemplateLayer

		private static readonly string[] _EmptyTemplateLayer = {
			"空のテンプレートレイヤー",
			"Empty template layer"
		};

		public static string EmptyTemplateLayer => _EmptyTemplateLayer[(int)Lang];

		#endregion

		#region VRCAvatarBuilderEditor

		#region VRCAvatarBuilderEditor_BasicSetting

		private static readonly string[] _VRCAvatarBuilderEditor_BasicSetting = {
			"基本設定",
			"Basic Setting"
		};

		public static string VRCAvatarBuilderEditor_BasicSetting => _VRCAvatarBuilderEditor_BasicSetting[(int)Lang];

		#endregion

		#region VRCAvatarBuilderEditor_AvatarIsNotSet

		private static readonly string[] _VRCAvatarBuilderEditor_AvatarIsNotSet = {
			"アバターが設定されていません。",
			"Avatar is not set."
		};

		public static string VRCAvatarBuilderEditor_AvatarIsNotSet => _VRCAvatarBuilderEditor_AvatarIsNotSet[(int)Lang];

		#endregion

		#region VRCAvatarBuilderEditor_WorkingDirectoryIsNotSet

		private static readonly string[] _VRCAvatarBuilderEditor_WorkingDirectoryIsNotSet = {
			"作業ディレクトリが設定されていません。",
			"Working directory is not set."
		};

		public static string VRCAvatarBuilderEditor_WorkingDirectoryIsNotSet =>
			_VRCAvatarBuilderEditor_WorkingDirectoryIsNotSet[(int)Lang];

		#endregion

		#region VRCAvatarBuilderEditor_UploadSceneIsNotSet

		private static readonly string[] _VRCAvatarBuilderEditor_UploadSceneIsNotSet = {
			"アップロード用シーンが設定されていません。",
			"Scene is not set for upload."
		};

		public static string VRCAvatarBuilderEditor_UploadSceneIsNotSet =>
			_VRCAvatarBuilderEditor_UploadSceneIsNotSet[(int)Lang];

		#endregion

		#region VRCAvatarBuilderEditor_WorkingDirectorySetup

		private static readonly string[] _VRCAvatarBuilderEditor_WorkingDirectorySetup = {
			"作業ディレクトリ設定",
			"Working directory setup"
		};

		public static string VRCAvatarBuilderEditor_WorkingDirectorySetup =>
			_VRCAvatarBuilderEditor_WorkingDirectorySetup[(int)Lang];

		#endregion

		#region VRCAvatarBuilderEditor_SetUpTheWorkingDirectoryFirst

		private static readonly string[] _VRCAvatarBuilderEditor_SetUpTheWorkingDirectoryFirst = {
			"先に作業ディレクトリを設定してください。",
			"Set up the working directory first."
		};

		public static string VRCAvatarBuilderEditor_SetUpTheWorkingDirectoryFirst =>
			_VRCAvatarBuilderEditor_SetUpTheWorkingDirectoryFirst[(int)Lang];

		#endregion

		#region VRCAvatarBuilderEditor_ModuleIsNotSet

		private static readonly string[] _VRCAvatarBuilderEditor_ModuleIsNotSet = {
			"モジュールが設定されていません。\n" +
			"標準設定を使用しますか。",
			"Module is not set.\n" +
			"Do you want to use the standard settings?"
		};

		public static string VRCAvatarBuilderEditor_ModuleIsNotSet => _VRCAvatarBuilderEditor_ModuleIsNotSet[(int)Lang];

		#endregion

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

		#region GestureLayerModuleEditor_FoundGestureLayerInAvatar

		private static readonly string[] _GestureLayerModuleEditor_FoundGestureLayerInAvatar = {
			"アバターに設定されているジェスチャーレイヤーが見つかりました。",
			"A gesture layer is found that is set in the avatar."
		};

		public static string GestureLayerModuleEditor_FoundGestureLayerInAvatar =>
			_GestureLayerModuleEditor_FoundGestureLayerInAvatar[(int)Lang];

		#endregion

		#region GestureLayerModuleEditor_SetupFromAnimatorController

		private static readonly string[] _GestureLayerModuleEditor_SetupFromAnimatorController = {
			"アニメーターコントローラーから設定する",
			"Setup from the Animator Controller"
		};

		public static string GestureLayerModuleEditor_SetupFromAnimatorController =>
			_GestureLayerModuleEditor_SetupFromAnimatorController[(int)Lang];

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

		#region GestureLayerModuleEditor_NotSupportType

		private static readonly string[] _GestureLayerModuleEditor_NotSupportType = {
			"この形式のアニメーターはサポートしていません。" +
			"AnimatorController か AnimatorOverrideControllerを使用してください。",
			"This type is not supported by the animator." +
			"Use AnimatorController or AnimatorOverrideController."
		};

		public static string GestureLayerModuleEditor_NotSupportType =>
			_GestureLayerModuleEditor_NotSupportType[(int)Lang];

		#endregion

		#endregion


		#region IdleExpressionModuleEditor

		#region IdleExpressionModuleEditor_Info

		private static readonly string[] _IdleExpressionModuleEditor_Info = {
			"このデフォルトの表情アニメーションレイヤーを追加します。\n" +
			"基本的に全ての表情のシェイプキーにチェックを付けることをお勧めします。",
			"Add this default facial expression animation layer.\n" +
			"It is recommended that you check the shape key for basically all expressions."
		};

		public static string IdleExpressionModuleEditor_Info => _IdleExpressionModuleEditor_Info[(int)Lang];

		#endregion

		#region IdleExpressionModuleEditor_UseAdditionalAnimation

		private static readonly string[] _IdleExpressionModuleEditor_UseAdditionalAnimation = {
			"追加アニメーションを使用する",
			"Use additional animation"
		};

		public static string IdleExpressionModuleEditor_UseAdditionalAnimation =>
			_IdleExpressionModuleEditor_UseAdditionalAnimation[(int)Lang];

		#endregion

		#region IdleExpressionModuleEditor_AdditionalAnimationClip

		private static readonly string[] _IdleExpressionModuleEditor_AdditionalAnimationClip = {
			"追加アニメーションクリップ",
			"Additional Animation Clips"
		};

		public static string IdleExpressionModuleEditor_AdditionalAnimationClip =>
			_IdleExpressionModuleEditor_AdditionalAnimationClip[(int)Lang];

		#endregion

		#region IdleExpressionModuleEditor_UseSimpleBlink

		private static readonly string[] _IdleExpressionModuleEditor_UseSimpleBlink = {
			"シンプルなまばたきアニメーションを追加",
			"Added simple blink animation"
		};

		public static string IdleExpressionModuleEditor_UseSimpleBlink =>
			_IdleExpressionModuleEditor_UseSimpleBlink[(int)Lang];

		#endregion

		#region IdleExpressionModuleEditor_NotFoundBlinkAnimationClip

		private static readonly string[] _IdleExpressionModuleEditor_NotFoundBlinkAnimationClip = {
			"まばたき用アニメーションクリップが見つかりません。\n" +
			"アセットを再度インポートしてください。",
			"Cannot find animation clip for blinking.\n" +
			"Import the assets again."
		};

		public static string IdleExpressionModuleEditor_NotFoundBlinkAnimationClip =>
			_IdleExpressionModuleEditor_NotFoundBlinkAnimationClip[(int)Lang];

		#endregion

		#region IdleExpressionModuleEditor_FaceMesh

		private static readonly string[] _IdleExpressionModuleEditor_FaceMesh = {
			"顔のメッシュ",
			"Face Mesh"
		};

		public static string IdleExpressionModuleEditor_FaceMesh => _IdleExpressionModuleEditor_FaceMesh[(int)Lang];

		#endregion

		#region IdleExpressionModuleEditor_SkinnedMeshRendererDisplayMode

		private static readonly string[] _IdleExpressionModuleEditor_SkinnedMeshRendererDisplayMode = {
			"SkinnedMeshRenderer表示モード",
			"SkinnedMeshRenderer display mode"
		};

		public static string IdleExpressionModuleEditor_SkinnedMeshRendererDisplayMode => _IdleExpressionModuleEditor_SkinnedMeshRendererDisplayMode[(int)Lang];

		#endregion

		#region IdleExpressionModuleEditor_UsedOnly

		private static readonly string[] _IdleExpressionModuleEditor_UsedOnly = {
			"使用されているオブジェクトのみ",
			"Only objects used"
		};

		public static string IdleExpressionModuleEditor_UsedOnly => _IdleExpressionModuleEditor_UsedOnly[(int)Lang];

		#endregion

		#region IdleExpressionModuleEditor_NonUsedOnly

		private static readonly string[] _IdleExpressionModuleEditor_NonUsedOnly = {
			"使用されていないオブジェクトのみ",
			"Only unused objects"
		};

		public static string IdleExpressionModuleEditor_NonUsedOnly => _IdleExpressionModuleEditor_NonUsedOnly[(int)Lang];

		#endregion

		#region IdleExpressionModuleEditor_DirectBlendShapeChangeWarning

		private static readonly string[] _IdleExpressionModuleEditor_DirectBlendShapeChangeWarning = {
			"ブレンドシェイプが直接変更されています。\n" +
			"デフォルト表情として設定し、全て0に置き換えますか?",
			"The blend shape has been directly modified.\n" +
			"Set as default expression and replace all with 0?"
		};

		public static string IdleExpressionModuleEditor_DirectBlendShapeChangeWarning => _IdleExpressionModuleEditor_DirectBlendShapeChangeWarning[(int)Lang];

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

		#region GestureExpressionModuleEditor_FoundFXLayerInAvatar

		private static readonly string[] _GestureExpressionModuleEditor_FoundFXLayerInAvatar = {
			"アバターに設定されているFXレイヤーが見つかりました。",
			"A FX layer is found that is set in the avatar."
		};

		public static string GestureExpressionModuleEditor_FoundFXLayerInAvatar =>
			_GestureExpressionModuleEditor_FoundFXLayerInAvatar[(int)Lang];

		#endregion

		#region GestureExpressionModuleEditor_NotFoundIdleExpressionModule

		private static readonly string[] _GestureExpressionModuleEditor_NotFoundIdleExpressionModule = {
			"デフォルト表情モジュールが存在しません。\n" +
			"モジュールを追加しますか。",
			"The default expression module does not exist.\n" +
			"Would you like to add a module?"
		};

		public static string GestureExpressionModuleEditor_NotFoundIdleExpressionModule =>
			_GestureExpressionModuleEditor_NotFoundIdleExpressionModule[(int)Lang];

		#endregion

		#region GestureExpressionModuleEditor_IdleExpression

		private static readonly string[] _GestureExpressionModuleEditor_IdleExpression = {
			"デフォルト表情",
			"Default Expression"
		};

		public static string GestureExpressionModuleEditor_IdleExpression =>
			_GestureExpressionModuleEditor_IdleExpression[(int)Lang];

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

		public static string AnimatorControllerMergerModuleEditor_Info =>
			_AnimatorControllerMergerModuleEditor_Info[(int)Lang];

		#endregion

		#region AnimatorControllerMergerModuleEditor_FirstLayersWeightIsZero

		private static readonly string[] _AnimatorControllerMergerModuleEditor_FirstLayersWeightIsZero = {
			"一番上のレイヤーのウェイトが0です。",
			"The weight of the top layer is 0."
		};

		public static string AnimatorControllerMergerModuleEditor_FirstLayersWeightIsZero =>
			_AnimatorControllerMergerModuleEditor_FirstLayersWeightIsZero[(int)Lang];

		#endregion

		#endregion
	}
}