using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using AnimatorController = UnityEditor.Animations.AnimatorController;

namespace Raitichan.Script.VRCAvatarBuilder.Module.Editor {
	[CustomEditor(typeof(AnimatorControllerMergerModule))]
	public class AnimatorControllerMergerModuleEditor : UnityEditor.Editor {
		private AnimatorControllerMergerModule _target;
		private ReorderableList _controllerList;

		private SerializedProperty _targetLayerTypeProperty;
		private SerializedProperty _animatorControllersProperty;


		public void OnEnable() {
			this._target = this.target as AnimatorControllerMergerModule;

			this._targetLayerTypeProperty =
				this.serializedObject.FindProperty(AnimatorControllerMergerModule.TargetLayerTypePropertyName);
			this._animatorControllersProperty =
				this.serializedObject.FindProperty(AnimatorControllerMergerModule.AnimatorControllersPropertyName);

			this._controllerList = new ReorderableList(this.serializedObject, this._animatorControllersProperty);
			this._controllerList.onAddCallback += Add;
			this._controllerList.onRemoveCallback += Remove;
			this._controllerList.drawHeaderCallback += DrawHeader;
			this._controllerList.elementHeightCallback += this.GetElementHeight;
			this._controllerList.drawElementCallback += this.DrawElement;
		}

		public override void OnInspectorGUI() {
			EditorGUILayout.HelpBox(Strings.AnimatorControllerMergerModuleEditor_Info, MessageType.Info);
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this._targetLayerTypeProperty, new GUIContent(Strings.TargetLayer));
			this._controllerList.DoLayoutList();

			this.serializedObject.ApplyModifiedProperties();
		}

		private static void Add(ReorderableList list) {
			int size = list.serializedProperty.arraySize;
			list.serializedProperty.InsertArrayElementAtIndex(size);
			list.serializedProperty.GetArrayElementAtIndex(size).objectReferenceValue = null;
		}

		private static void Remove(ReorderableList list) {
			SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(list.index);
			element.objectReferenceValue = null;
			list.serializedProperty.DeleteArrayElementAtIndex(list.index);
		}

		private void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
			SerializedProperty element = this._animatorControllersProperty.GetArrayElementAtIndex(index);
			Rect currentRect = rect;
			currentRect.y += 2;
			currentRect.height = EditorGUI.GetPropertyHeight(element);
			EditorGUI.PropertyField(currentRect, element, GUIContent.none);

			if (!(element.objectReferenceValue is AnimatorController controller)) return;
			if (!CheckFirstLayersWeightIsZero(controller)) return;

			currentRect.y += currentRect.height + 5;
			currentRect.height = 25;
			EditorGUI.HelpBox(currentRect, Strings.AnimatorControllerMergerModuleEditor_FirstLayersWeightIsZero,
				MessageType.Warning);

			currentRect.y += 3;
			currentRect.height -= 6;
			currentRect.x += 300;
			currentRect.width -= 303;
			if (!GUI.Button(currentRect, Strings.AutoFix)) return;
			SerializedObject controllerObject = new SerializedObject(controller);
			controllerObject.Update();
			controllerObject.FindProperty("m_AnimatorLayers")
				.GetArrayElementAtIndex(0)
				.FindPropertyRelative("m_DefaultWeight")
				.floatValue = 1;
			controllerObject.ApplyModifiedProperties();
		}

		private static void DrawHeader(Rect rect) {
			GUI.Label(rect, Strings.AnimatorController);
		}

		private float GetElementHeight(int index) {
			SerializedProperty element = this._animatorControllersProperty.GetArrayElementAtIndex(index);
			float height = EditorGUI.GetPropertyHeight(element) + 5;

			if (!(element.objectReferenceValue is AnimatorController controller)) return height;
			if (!CheckFirstLayersWeightIsZero(controller)) return height;
			return height + 30;
		}

		private static bool CheckFirstLayersWeightIsZero(AnimatorController controller) {
			if (controller.layers.Length <= 0) return false;
			return controller.layers[0].defaultWeight == 0;
		}
	}
}