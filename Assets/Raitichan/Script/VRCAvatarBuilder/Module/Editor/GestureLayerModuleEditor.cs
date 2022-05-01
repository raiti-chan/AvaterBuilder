using System.Collections.Generic;
using System.Linq;
using Raitichan.Script.Util.Enum;
using Raitichan.Script.Util.Extension;
using Raitichan.Script.VRCAvatarBuilder.Editor;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace Raitichan.Script.VRCAvatarBuilder.Module.Editor {
	[CustomEditor(typeof(GestureLayerModule))]
	public class GestureLayerModuleEditor : UnityEditor.Editor {
		private GestureLayerModule _target;
		private VRCAvatarBuilder _targetBuilder;

		private SerializedProperty _useDifferentAnimationProperty;
		private SerializedProperty _leftAnimationProperty;
		private SerializedProperty _rightAnimationProperty;

		private AnimatorController _controller;

		private RuntimeAnimatorController _referenceController;

		private void OnEnable() {
			this._target = this.target as GestureLayerModule;
			if (this._target == null) return;

			this._targetBuilder = this._target.GetTargetBuilder();
			this._useDifferentAnimationProperty =
				this.serializedObject.FindProperty(GestureLayerModule.UseDifferentAnimationPropertyName);
			this._leftAnimationProperty =
				this.serializedObject.FindProperty(GestureLayerModule.LeftAnimationPropertyName);
			this._rightAnimationProperty =
				this.serializedObject.FindProperty(GestureLayerModule.RightAnimationPropertyName);

			this._controller =
				AssetDatabase.LoadAssetAtPath<AnimatorController>(ConstantPath.DEFAULT_GESTURE_LAYER);
		}

		public override void OnInspectorGUI() {
			this.serializedObject.Update();
			EditorGUILayout.HelpBox(Strings.GestureLayerModuleEditor_Info, MessageType.Info);
			if (this._controller == null) {
				EditorGUILayout.HelpBox(Strings.GestureLayerModuleEditor_NotFoundDefaultController,
					MessageType.Error);
			} else {
				using (new EditorGUI.DisabledScope(true)) {
					EditorGUILayout.ObjectField(this._controller, typeof(AnimatorController), false);
				}
			}

			this._target.UtilityHoldout = RaitisEditorUtil.Foldout(this._target.UtilityHoldout, Strings.Utility);
			if (this._target.UtilityHoldout) {
				GUILayout.Space(5);
				EditorGUI.indentLevel++;
				this.DrawAvatarsDefaultGestureLayer();
				GUILayout.Space(5);
				this.DrawSettingWithController();
				EditorGUI.indentLevel--;
			}

			GUILayout.Space(5);
			EditorGUILayout.PropertyField(this._useDifferentAnimationProperty,
				new GUIContent(Strings.GestureLayerModuleEditor_UseDifferentAnimationProperty));

			InitArray(this._leftAnimationProperty);
			InitArray(this._rightAnimationProperty);

			GUILayout.Space(5);
			if (GUILayout.Button(Strings.GestureLayerModuleEditor_SetDefault)) {
				if (EditorUtility.DisplayDialog(Strings.Warning,
					    Strings.GestureLayerModuleEditor_SetDefault_WarningMessage,
					    Strings.OK, Strings.Cansel)) {
					this.SetDefaultParameter();
				}
			}


			GUILayout.Space(15);
			if (this._target.UseDifferentAnimation) {
				GUILayout.Label(Strings.LeftHand);
			}

			DrawGestureInventory(this._leftAnimationProperty);

			if (this._target.UseDifferentAnimation) {
				GUILayout.Space(5);

				GUILayout.Label(Strings.RightHand);
				DrawGestureInventory(this._rightAnimationProperty);
			}


			this.serializedObject.ApplyModifiedProperties();
		}

		private void DrawAvatarsDefaultGestureLayer() {
			VRCAvatarDescriptor descriptor = this._targetBuilder.AvatarDescriptor;
			if (descriptor == null) return;
			RuntimeAnimatorController gestureLayer = descriptor.GetLayer(VRCAvatarDescriptor.AnimLayerType.Gesture);
			if (gestureLayer == null) return;

			if (RaitisEditorUtil.HelpBoxWithButton("アバターに設定されているジェスチャーレイヤーが見つかりました。", MessageType.Info,
				    Strings.AutoSetting)) {
				this.SetupWithRuntimeAnimatorController(gestureLayer);
			}
		}

		private void DrawSettingWithController() {
			EditorGUILayout.LabelField("アニメーターコントローラーから設定する");
			GUILayout.BeginHorizontal();
			this._referenceController =
				EditorGUILayout.ObjectField(this._referenceController, typeof(RuntimeAnimatorController), true) as
					RuntimeAnimatorController;
			using (new EditorGUI.DisabledScope(this._referenceController == null)) {
				if (GUILayout.Button(Strings.Setting)) {
					this.SetupWithRuntimeAnimatorController(this._referenceController);
				}
			}

			GUILayout.EndHorizontal();
		}

		private void SetDefaultParameter() {
			for (int i = 0; i < 8; i++) {
				SerializedProperty left = this._leftAnimationProperty.GetArrayElementAtIndex(i);
				SerializedProperty right = this._rightAnimationProperty.GetArrayElementAtIndex(i);
				AnimationClip clip =
					AssetDatabase.LoadAssetAtPath<AnimationClip>(ConstantPath.SDK_HANDS_ANIMATION_FILE_PATHS[i]);
				left.objectReferenceValue = clip;
				right.objectReferenceValue = clip;
			}
		}


		private static void InitArray(SerializedProperty property) {
			int size = property.arraySize;
			if (size == 8) return;
			while (size < 8) {
				property.InsertArrayElementAtIndex(size);
				size++;
			}

			while (size > 8) {
				size--;
				property.DeleteArrayElementAtIndex(size);
			}
		}

		private static void DrawGestureInventory(SerializedProperty property) {
			using (new EditorGUI.IndentLevelScope()) {
				for (int i = 0; i < property.arraySize; i++) {
					SerializedProperty elementProperty = property.GetArrayElementAtIndex(i);
					EditorGUILayout.PropertyField(elementProperty, new GUIContent(((GestureTypes)i).ToString()));
				}
			}
		}

		private void SetupWithRuntimeAnimatorController(RuntimeAnimatorController rController) {
			switch (rController) {
				case AnimatorController animatorController:
					this.SetupWithAnimatorController(animatorController);
					break;
				case AnimatorOverrideController overrideController:
					this.SetupWithOverrideController(overrideController);
					break;
				default:
					EditorUtility.DisplayDialog(Strings.Error,
						"この形式のアニメーターはサポートしていません。AnimatorController か AnimatorOverrideControllerを使用してください。", Strings.OK);
					break;
			}
		}

		private void SetupWithOverrideController(AnimatorOverrideController controller) {
			this.SetupWithAnimatorController(controller.runtimeAnimatorController as AnimatorController);
			foreach (SerializedProperty elementProperty in this._leftAnimationProperty.GetEnumerable()) {
				AnimationClip originalClip = elementProperty.objectReferenceValue as AnimationClip;
				AnimationClip overrideClip = controller[originalClip];
				elementProperty.objectReferenceValue = overrideClip;
			}
			
			foreach (SerializedProperty elementProperty in this._rightAnimationProperty.GetEnumerable()) {
				AnimationClip originalClip = elementProperty.objectReferenceValue as AnimationClip;
				AnimationClip overrideClip = controller[originalClip];
				elementProperty.objectReferenceValue = overrideClip;
			}
		} 

		private void SetupWithAnimatorController(AnimatorController controller) {
			this._useDifferentAnimationProperty.boolValue = false;
			IEnumerable<AnimatorTransitionBase> targetTransitions = controller.GetAllTransitionBase()
				.Where(transition => transition.conditions.Count(condition =>
					condition.parameter == BuiltInAvatarParameter.GestureLeft.ToString() ||
					condition.parameter == BuiltInAvatarParameter.GestureRight.ToString()) != 0);

			Dictionary<GestureTypes, List<AnimationClip>> leftDictionary = new Dictionary<GestureTypes, List<AnimationClip>> {
				[GestureTypes.Fist] = new List<AnimationClip>(),
				[GestureTypes.Open] = new List<AnimationClip>(),
				[GestureTypes.Point] = new List<AnimationClip>(),
				[GestureTypes.Peace] = new List<AnimationClip>(),
				[GestureTypes.RockNRoll] = new List<AnimationClip>(),
				[GestureTypes.Gun] = new List<AnimationClip>(),
				[GestureTypes.ThumbsUp] = new List<AnimationClip>(),
				[GestureTypes.Idle] = new List<AnimationClip>(),
			};
			Dictionary<GestureTypes, List<AnimationClip>> rightDictionary = new Dictionary<GestureTypes, List<AnimationClip>> {
				[GestureTypes.Fist] = new List<AnimationClip>(),
				[GestureTypes.Open] = new List<AnimationClip>(),
				[GestureTypes.Point] = new List<AnimationClip>(),
				[GestureTypes.Peace] = new List<AnimationClip>(),
				[GestureTypes.RockNRoll] = new List<AnimationClip>(),
				[GestureTypes.Gun] = new List<AnimationClip>(),
				[GestureTypes.ThumbsUp] = new List<AnimationClip>(),
				[GestureTypes.Idle] = new List<AnimationClip>(),
			};
			foreach (AnimatorTransitionBase transition in targetTransitions) {
				if (transition.destinationState == null) continue;
				if (!(transition.destinationState.motion is AnimationClip clip)) continue;
				foreach (AnimatorCondition condition in transition.conditions) {
					if (condition.threshold > 7) continue;
					int value = (int)condition.threshold;
					GestureTypes type = value == 0 ? GestureTypes.Idle : (GestureTypes)(value - 1);
					if (condition.parameter == BuiltInAvatarParameter.GestureLeft.ToString()) {
						leftDictionary[type].Add(clip);
					}
					if (condition.parameter == BuiltInAvatarParameter.GestureRight.ToString()) {
						rightDictionary[type].Add(clip);
					}
				}
			}
			// TODO: 複数見つかった場合の選択ダイアログ
			foreach (KeyValuePair<GestureTypes,List<AnimationClip>> keyValuePair in leftDictionary) {
				if (keyValuePair.Value.Count <= 0) continue;
				SerializedProperty element = this._leftAnimationProperty.GetArrayElementAtIndex((int)keyValuePair.Key);
				element.objectReferenceValue = keyValuePair.Value[0];
			}
			
			foreach (KeyValuePair<GestureTypes,List<AnimationClip>> keyValuePair in rightDictionary) {
				if (keyValuePair.Value.Count <= 0) continue;
				SerializedProperty element = this._rightAnimationProperty.GetArrayElementAtIndex((int)keyValuePair.Key);
				SerializedProperty lElement = this._leftAnimationProperty.GetArrayElementAtIndex((int)keyValuePair.Key);
				element.objectReferenceValue = keyValuePair.Value[0];
				if (element.objectReferenceValue != lElement.objectReferenceValue) {
					this._useDifferentAnimationProperty.boolValue = true;
				}
			}
			
		}
	}
}