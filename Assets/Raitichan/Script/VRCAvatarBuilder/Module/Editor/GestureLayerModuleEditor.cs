using Raitichan.Script.Util.Enum;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Raitichan.Script.VRCAvatarBuilder.Module.Editor {
	[CustomEditor(typeof(GestureLayerModule))]
	public class GestureLayerModuleEditor : UnityEditor.Editor {
		private GestureLayerModule _target;

		private SerializedProperty _useDifferentAnimationProperty;
		private SerializedProperty _leftGestureProperty;
		private SerializedProperty _rightGestureProperty;

		private AnimatorController _controller;

		private void OnEnable() {
			this._target = this.target as GestureLayerModule;

			this._useDifferentAnimationProperty =
				this.serializedObject.FindProperty(GestureLayerModule.UseDifferentAnimationPropertyName);
			this._leftGestureProperty =
				this.serializedObject.FindProperty(GestureLayerModule.LeftAnimationPropertyName);
			this._rightGestureProperty =
				this.serializedObject.FindProperty(GestureLayerModule.RightGesturePropertyName);

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

			GUILayout.Space(5);
			EditorGUILayout.PropertyField(this._useDifferentAnimationProperty,
				new GUIContent(Strings.GestureLayerModuleEditor_UseDifferentAnimationProperty));

			InitArray(this._leftGestureProperty);
			InitArray(this._rightGestureProperty);

			GUILayout.Space(5);
			if (GUILayout.Button(Strings.GestureLayerModuleEditor_SetDefault)) {
				if (EditorUtility.DisplayDialog(Strings.Warning,
					    Strings.GestureLayerModuleEditor_SetDefault_WarningMessage,
					    Strings.OK, Strings.Cansel)) {
					this.SetDefaultParameter();
				}
			}

			// TODO: アバターについてくるアニメーターから自動設定する機能を作りたい

			GUILayout.Space(15);
			if (this._target.UseDifferentAnimation) {
				GUILayout.Label(Strings.LeftHand);
			}

			DrawGestureInventory(this._leftGestureProperty);

			if (this._target.UseDifferentAnimation) {
				GUILayout.Space(5);

				GUILayout.Label(Strings.RightHand);
				DrawGestureInventory(this._rightGestureProperty);
			}


			this.serializedObject.ApplyModifiedProperties();
		}

		private void SetDefaultParameter() {
			for (int i = 0; i < 8; i++) {
				SerializedProperty left = this._leftGestureProperty.GetArrayElementAtIndex(i);
				SerializedProperty right = this._rightGestureProperty.GetArrayElementAtIndex(i);
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
	}
}