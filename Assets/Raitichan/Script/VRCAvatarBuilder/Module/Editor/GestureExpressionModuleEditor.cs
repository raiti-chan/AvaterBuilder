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
	[CustomEditor(typeof(GestureExpressionModule))]
	public class GestureExpressionModuleEditor : UnityEditor.Editor {
		private GestureExpressionModule _target;
		private VRCAvatarBuilder _targetBuilder;
		private VRCAvatarBuilderModuleBase[] _allModules;

		private SerializedProperty _useDifferentAnimationProperty;
		private SerializedProperty _useUserDefinedAnimationProperty;
		private SerializedProperty _idleAnimationProperty;
		private SerializedProperty _leftAnimationProperty;
		private SerializedProperty _rightAnimationProperty;

		private AnimatorController _controller;
		private AnimatorController _idleController;
		private RuntimeAnimatorController _referenceController;

		private void OnEnable() {
			this._target = this.target as GestureExpressionModule;
			if (this._target == null) return;

			this._targetBuilder = this._target.GetTargetBuilder();
			this.ModuleListUpdate();

			this._useDifferentAnimationProperty =
				this.serializedObject.FindProperty(GestureExpressionModule.UseDifferentAnimationPropertyName);
			this._useUserDefinedAnimationProperty =
				this.serializedObject.FindProperty(GestureExpressionModule.UseUserDefinedIdleAnimationPropertyName);
			this._idleAnimationProperty =
				this.serializedObject.FindProperty(GestureExpressionModule.IdleAnimationPropertyName);
			this._leftAnimationProperty =
				this.serializedObject.FindProperty(GestureExpressionModule.LeftAnimationPropertyName);
			this._rightAnimationProperty =
				this.serializedObject.FindProperty(GestureExpressionModule.RightAnimationPropertyName);

			this._controller =
				AssetDatabase.LoadAssetAtPath<AnimatorController>(ConstantPath.DEFAULT_FX_EXPRESSION_LAYER);
			this._idleController =
				AssetDatabase.LoadAssetAtPath<AnimatorController>(ConstantPath.UTIL_IDLE_LAYER);

			Undo.undoRedoPerformed += this.ModuleListUpdate;
		}

		private void OnDisable() {
			Undo.undoRedoPerformed -= this.ModuleListUpdate;
		}

		private void ModuleListUpdate() {
			this._allModules = this._target.GetAllModule();
		}


		public override void OnInspectorGUI() {
			this.serializedObject.Update();
			EditorGUILayout.HelpBox(Strings.GestureExpressionModuleEditor_Info, MessageType.Info);
			if (this._controller == null) {
				EditorGUILayout.HelpBox(Strings.GestureExpressionModuleEditor_NotFoundDefaultController,
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
			// デフォルト表情の警告表示
			this.DrawDefaultExpressionWarning();

			// Idleアニメーションの設定
			// TODO: 削除予定
			// EditorGUILayout.PropertyField(this._useUserDefinedAnimationProperty,
			//	new GUIContent(Strings.GestureExpressionModuleEditor_UseUserDefinedIdleAnimation));
			if (this._target.UseUserDefinedIdleAnimation) {
				if (this._idleController == null) {
					EditorGUILayout.HelpBox(Strings.NotFoundIdleTemplateLayer,
						MessageType.Error);
				}

				using (new EditorGUI.IndentLevelScope()) {
					// EditorGUILayout.PropertyField(this._idleAnimationProperty, new GUIContent("Idle"));
				}
			}

			GUILayout.Space(5);
			EditorGUILayout.PropertyField(this._useDifferentAnimationProperty,
				new GUIContent(Strings.GestureLayerModuleEditor_UseDifferentAnimationProperty));

			InitArray(this._leftAnimationProperty);
			InitArray(this._rightAnimationProperty);

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

		// ReSharper disable Unity.PerformanceAnalysis
		private void DrawDefaultExpressionWarning() {

			if (this.FindIdleExpressionModule()) return;
			if (this._target.UseUserDefinedIdleAnimation) return;
			if (!RaitisEditorUtil.HelpBoxWithButton("デフォルトの表情用のレイヤーが存在しません。\nモジュールを追加しますか。", MessageType.Warning,
				    Strings.AddModule)) return;
			// TODO: このモジュールより上にIdleExpressionレイヤーがある場合の警告

			GameObject obj = new GameObject("デフォルト表情", typeof(IdleExpressionModule)) {
				transform = {
					parent = this._target.transform.parent
				}
			};
			obj.transform.SetSiblingIndex(this._target.transform.GetSiblingIndex());
			Undo.RegisterCreatedObjectUndo(obj, "AddModule");
			this._target.ModuleUpdateFlag = true;
			this.ModuleListUpdate();
		}

		private bool FindIdleExpressionModule() {
			return this._allModules.Select(module => module.GetType()).Contains(typeof(IdleExpressionModule));
		}

		private void DrawAvatarsDefaultGestureLayer() {
			VRCAvatarDescriptor descriptor = this._targetBuilder.AvatarDescriptor;
			if (descriptor == null) return;
			RuntimeAnimatorController gestureLayer = descriptor.GetLayer(VRCAvatarDescriptor.AnimLayerType.FX);
			if (gestureLayer == null) return;

			if (RaitisEditorUtil.HelpBoxWithButton(Strings.GestureExpressionModuleEditor_FoundFXLayerInAvatar,
				    MessageType.Info,
				    Strings.AutoSetting)) {
				this.SetupWithRuntimeAnimatorController(gestureLayer);
			}
		}

		private void DrawSettingWithController() {
			EditorGUILayout.LabelField(Strings.GestureLayerModuleEditor_SetupFromAnimatorController);
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

		private static void InitArray(SerializedProperty property) {
			int size = property.arraySize;
			if (size == 7) return;
			while (size < 7) {
				property.InsertArrayElementAtIndex(size);
				size++;
			}

			while (size > 7) {
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
						Strings.GestureLayerModuleEditor_NotSupportType, Strings.OK);
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

			Dictionary<GestureTypes, List<AnimationClip>> leftDictionary =
				new Dictionary<GestureTypes, List<AnimationClip>> {
					[GestureTypes.Fist] = new List<AnimationClip>(),
					[GestureTypes.Open] = new List<AnimationClip>(),
					[GestureTypes.Point] = new List<AnimationClip>(),
					[GestureTypes.Peace] = new List<AnimationClip>(),
					[GestureTypes.RockNRoll] = new List<AnimationClip>(),
					[GestureTypes.Gun] = new List<AnimationClip>(),
					[GestureTypes.ThumbsUp] = new List<AnimationClip>(),
					[GestureTypes.Idle] = new List<AnimationClip>(),
				};
			Dictionary<GestureTypes, List<AnimationClip>> rightDictionary =
				new Dictionary<GestureTypes, List<AnimationClip>> {
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
			// TODO: Idleアニメーションの探索
			foreach (KeyValuePair<GestureTypes, List<AnimationClip>> keyValuePair in leftDictionary) {
				if (keyValuePair.Key == GestureTypes.Idle) continue;
				if (keyValuePair.Value.Count <= 0) continue;
				SerializedProperty element = this._leftAnimationProperty.GetArrayElementAtIndex((int)keyValuePair.Key);
				element.objectReferenceValue = keyValuePair.Value[0];
			}

			foreach (KeyValuePair<GestureTypes, List<AnimationClip>> keyValuePair in rightDictionary) {
				if (keyValuePair.Key == GestureTypes.Idle) continue;
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