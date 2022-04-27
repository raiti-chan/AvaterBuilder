using System.Diagnostics.CodeAnalysis;

namespace Raitichan.Script.VRCAvatarBuilder {
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
	public static class ConstantPath {
		public const string OUTPUT_DIRECTORY = "/out";
		public const string GENERATED_SUFFIX = "_Generated";
		public const string RESOURCE_DIR_PATH = "Assets/Raitichan/Script/VRCAvatarBuilder/resource";
		public const string ANIMATION_DIR_PATH = RESOURCE_DIR_PATH + "/Animation";
		public const string CONTROLLER_DIR_PATH = ANIMATION_DIR_PATH + "/Controller";

		public const string EMPTY_CONTROLLER_PATH = CONTROLLER_DIR_PATH + "/EmptyAnimationControler.controller";
			
		public const string BASE_DIR_PATH = CONTROLLER_DIR_PATH + "/Base";
		public const string ADDITIVE_DIR_PATH = CONTROLLER_DIR_PATH + "/Additive";
		public const string GESTURE_DIR_PATH = CONTROLLER_DIR_PATH + "/Gesture";
		public const string ACTION_DIR_PATH = CONTROLLER_DIR_PATH + "/Action";
		public const string FX_DIR_PATH = CONTROLLER_DIR_PATH + "/Fx";
		
		public const string DEFAULT_BASE_FORCE_LOCOMOTION_LAYER = BASE_DIR_PATH + "/VRCAvatarBuilderBaseLayer-ForceLocomotionAnimation.controller";
		public const string DEFAULT_ADDITIVE_LAYER = ADDITIVE_DIR_PATH + "/VRCAvatarBuilderAdditiveLayer.controller";
		public const string DEFAULT_GESTURE_LAYER = GESTURE_DIR_PATH + "/VRCAvatarBuilderGestureLayer.controller";
		public const string DEFAULT_ACTION_LAYER = ACTION_DIR_PATH + "/VRCAvatarBuilderActionLayer.controller";
		public const string DEFAULT_FX_EXPRESSION_LAYER = FX_DIR_PATH + "/VRCAvatarBuilderFxLayer-Expression.controller";

		public const string GENERATE_BASE_LAYER_FILENAME = "/GeneratedBaseLayer.controller";
		public const string GENERATE_ADDITIVE_LAYER_FILENAME = "/GeneratedAdditiveLayer.controller";
		public const string GENERATE_GESTURE_LAYER_FILENAME = "/GeneratedGestureLayer.controller";
		public const string GENERATE_ACTION_LAYER_FILENAME = "/GeneratedActionLayer.controller";
		public const string GENERATE_FX_LAYER_FILENAME = "/GeneratedFXLayer.controller";
		public const string GENERATE_SITTING_LAYER_FILENAME = "/GeneratedSittingLayer.controller";
		public const string GENERATE_T_POSE_LAYER_FILENAME = "/GeneratedTPoseLayer.controller";
		public const string GENERATE_IK_POSE_LAYER_FILENAME = "/GeneratedIKPoseLayer.controller";

		public const string SDK_HANDS_FIST_ANIMATION_FILE_PATH = "Assets/VRCSDK/Examples3/Animation/ProxyAnim/proxy_hands_fist.anim";
		public const string SDK_HANDS_OPEN_ANIMATION_FILE_PATH = "Assets/VRCSDK/Examples3/Animation/ProxyAnim/proxy_hands_open.anim";
		public const string SDK_HANDS_POINT_ANIMATION_FILE_PATH = "Assets/VRCSDK/Examples3/Animation/ProxyAnim/proxy_hands_point.anim";
		public const string SDK_HANDS_PEACE_ANIMATION_FILE_PATH = "Assets/VRCSDK/Examples3/Animation/ProxyAnim/proxy_hands_peace.anim";
		public const string SDK_HANDS_ROCK_N_ROLL_ANIMATION_FILE_PATH = "Assets/VRCSDK/Examples3/Animation/ProxyAnim/proxy_hands_rock.anim";
		public const string SDK_HANDS_GUN_ANIMATION_FILE_PATH = "Assets/VRCSDK/Examples3/Animation/ProxyAnim/proxy_hands_gun.anim";
		public const string SDK_HANDS_THUMBS_UP_ANIMATION_FILE_PATH = "Assets/VRCSDK/Examples3/Animation/ProxyAnim/proxy_hands_thumbs_up.anim";
		public const string SDK_HANDS_IDLE_ANIMATION_FILE_PATH = "Assets/VRCSDK/Examples3/Animation/ProxyAnim/proxy_hands_idle2.anim";

		public static readonly string[] SDK_HANDS_ANIMATION_FILE_PATHS = {
			SDK_HANDS_FIST_ANIMATION_FILE_PATH,
			SDK_HANDS_OPEN_ANIMATION_FILE_PATH,
			SDK_HANDS_POINT_ANIMATION_FILE_PATH,
			SDK_HANDS_PEACE_ANIMATION_FILE_PATH,
			SDK_HANDS_ROCK_N_ROLL_ANIMATION_FILE_PATH,
			SDK_HANDS_GUN_ANIMATION_FILE_PATH,
			SDK_HANDS_THUMBS_UP_ANIMATION_FILE_PATH,
			SDK_HANDS_IDLE_ANIMATION_FILE_PATH
		};

	}
}