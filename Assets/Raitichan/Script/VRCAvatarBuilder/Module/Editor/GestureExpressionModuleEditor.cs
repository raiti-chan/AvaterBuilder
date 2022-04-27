using System;
using Raitichan.Script.Util.Enum;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Raitichan.Script.VRCAvatarBuilder.Module.Editor {
	[CustomEditor(typeof(GestureExpressionModule))]
	public class GestureExpressionModuleEditor : UnityEditor.Editor {
		private GestureExpressionModule _target;

		private SerializedProperty _useDifferentAnimationProperty;
		private SerializedProperty _leftAnimationProperty;
		private SerializedProperty _rightAnimationProperty;

		private AnimatorController _controller;

		private void OnEnable() {
			this._target = this.target as GestureExpressionModule;

			this._useDifferentAnimationProperty =
				this.serializedObject.FindProperty(GestureExpressionModule.UseDifferentAnimationPropertyName);
			this._leftAnimationProperty =
				this.serializedObject.FindProperty(GestureExpressionModule.LeftAnimationPropertyName);
			this._rightAnimationProperty =
				this.serializedObject.FindProperty(GestureExpressionModule.RightAnimationPropertyName);

			this._controller =
				AssetDatabase.LoadAssetAtPath<AnimatorController>(ConstantPath.DEFAULT_FX_EXPRESSION_LAYER);
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

			GUILayout.Space(5);
			EditorGUILayout.PropertyField(this._useDifferentAnimationProperty,
				new GUIContent(Strings.GestureLayerModuleEditor_UseDifferentAnimationProperty));

			InitArray(this._leftAnimationProperty);
			InitArray(this._rightAnimationProperty);

			// TODO: アバターについてくるアニメーターから自動設定する機能を作りたい
			// TODO: Idle レイヤーの設定

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
	}
}