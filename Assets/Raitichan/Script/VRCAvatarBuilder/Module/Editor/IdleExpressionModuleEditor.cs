using System;
using System.Collections.Generic;
using System.Linq;
using Raitichan.Script.Util.Extension;
using Raitichan.Script.VRCAvatarBuilder.Editor;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDKBase;
using static Raitichan.Script.VRCAvatarBuilder.Module.IdleExpressionModule;
using static Raitichan.Script.VRCAvatarBuilder.Module.IdleExpressionModule.SkinnedMeshRendererAdditionalData;

namespace Raitichan.Script.VRCAvatarBuilder.Module.Editor {
	[CustomEditor(typeof(IdleExpressionModule))]
	public class IdleExpressionModuleEditor : UnityEditor.Editor {
		private IdleExpressionModule _target;
		private VRCAvatarBuilder _targetBuilder;
		private SkinnedMeshRenderer[] _meshRenderers;
		private Dictionary<SkinnedMeshRenderer, float[]> _defaultBlendShapeWeightDictionary;

		private AnimatorController _controller;
		private AnimationClip _blinkClip;

		private bool _previewEnable;

		private SerializedProperty _skinnedMeshRendererAdditionalProperty;
		private SerializedProperty _useSimpleBlinkProperty;
		private SerializedProperty _simpleBlinkSkinnedMeshRendererProperty;
		private SerializedProperty _simpleBlinkBlendShapeIndexProperty;
		private SerializedProperty _useAdditionalAnimationProperty;
		private SerializedProperty _additionalAnimationProperty;

		private void OnEnable() {
			this._target = this.target as IdleExpressionModule;
			if (this._target == null) return;

			this._targetBuilder = this._target.GetTargetBuilder();

			if (this._targetBuilder.AvatarDescriptor != null) {
				this._meshRenderers = this._targetBuilder.AvatarDescriptor.gameObject
					.GetComponentsInChildren<SkinnedMeshRenderer>()
					.Where(renderer => {
						Mesh sharedMesh = renderer.sharedMesh;
						return sharedMesh != null && sharedMesh.blendShapeCount != 0;
					})
					.ToArray();
			} else {
				this._meshRenderers = Array.Empty<SkinnedMeshRenderer>();
			}

			this._skinnedMeshRendererAdditionalProperty =
				this.serializedObject.FindProperty(SkinnedMeshRendererAdditionalPropertyName);

			this._useSimpleBlinkProperty = this.serializedObject.FindProperty(UseSimpleBlinkPropertyName);
			this._simpleBlinkSkinnedMeshRendererProperty =
				this.serializedObject.FindProperty(SimpleBlinkSkinnedMeshRendererPropertyName);
			this._simpleBlinkBlendShapeIndexProperty =
				this.serializedObject.FindProperty(SimpleBlinkBlendShapeIndexPropertyName);

			this._useAdditionalAnimationProperty =
				this.serializedObject.FindProperty(UseAdditionalAnimationPropertyName);
			this._additionalAnimationProperty =
				this.serializedObject.FindProperty(AdditionalAnimationPropertyName);

			this._controller =
				AssetDatabase.LoadAssetAtPath<AnimatorController>(ConstantPath.UTIL_IDLE_LAYER);
			this._blinkClip =
				AssetDatabase.LoadAssetAtPath<AnimationClip>(ConstantPath.BLINK_ANIM_FILE_PATH);

			this._defaultBlendShapeWeightDictionary = new Dictionary<SkinnedMeshRenderer, float[]>();
			this.SkinnedMeshRendererAdditionalUpdate();
			this._previewEnable = false;
		}

		private void OnDisable() {
			if (this._previewEnable) {
				this.SetDefaultData();
			}
		}

		private void SkinnedMeshRendererAdditionalUpdate() {
			// TODO: 要最適化
			List<SkinnedMeshRendererAdditionalData> dataList = this._target.SkinnedMeshRendererAdditional;
			foreach (SkinnedMeshRenderer renderer in this._meshRenderers) {
				bool isContains = dataList.Count(data => data.Renderer == renderer) != 0;
				BlendShapeWeightData[] blendShapeData = new BlendShapeWeightData[renderer.sharedMesh.blendShapeCount];
				float[] weights = new float[blendShapeData.Length];
				for (int i = 0; i < blendShapeData.Length; i++) {
					blendShapeData[i].Name = renderer.sharedMesh.GetBlendShapeName(i);
					blendShapeData[i].Index = i;
					weights[i] = renderer.GetBlendShapeWeight(i);
				}

				this._defaultBlendShapeWeightDictionary[renderer] = weights;
				if (isContains) continue;

				dataList.Add(new SkinnedMeshRendererAdditionalData {
					Renderer = renderer,
					IsOpen = false,
					BlendShapeData = blendShapeData
				});
			}

			for (int i = 0; i < dataList.Count; i++) {
				SkinnedMeshRendererAdditionalData data = dataList[i];
				bool isContains = this._meshRenderers.Contains(data.Renderer);
				if (isContains) continue;
				dataList.RemoveAt(i);
				i--;
			}
		}

		private void SetPreviewData() {
			foreach (SkinnedMeshRendererAdditionalData additionalData in this._target.SkinnedMeshRendererAdditional) {
				SkinnedMeshRenderer meshRenderer = additionalData.Renderer;
				float[] defaultWeights = this._defaultBlendShapeWeightDictionary[meshRenderer];
				foreach (BlendShapeWeightData weightData in additionalData.BlendShapeData) {
					meshRenderer.SetBlendShapeWeight(weightData.Index,
						weightData.Use ? weightData.Weight : defaultWeights[weightData.Index]);
				}
			}
		}

		private void SetDefaultData() {
			foreach (SkinnedMeshRenderer meshRenderer in this._meshRenderers) {
				float[] weights = this._defaultBlendShapeWeightDictionary[meshRenderer];
				for (int i = 0; i < weights.Length; i++) {
					meshRenderer.SetBlendShapeWeight(i, weights[i]);
				}
			}
		}

		public override void OnInspectorGUI() {
			this.serializedObject.Update();
			EditorGUILayout.HelpBox("このデフォルトの表情アニメーションレイヤーを追加します。", MessageType.Info);
			if (this._controller == null) {
				EditorGUILayout.HelpBox(Strings.GestureExpressionModuleEditor_NotFoundDefaultController,
					MessageType.Error);
			} else {
				using (new EditorGUI.DisabledScope(true)) {
					EditorGUILayout.ObjectField(this._controller, typeof(AnimatorController), false);
				}
			}

			GUILayout.Space(5);
			EditorGUILayout.PropertyField(this._useAdditionalAnimationProperty, new GUIContent("追加アニメーションを使用する"));
			if (this._target.UseAdditionalAnimation) {
				EditorGUILayout.PropertyField(this._additionalAnimationProperty, new GUIContent("追加アニメーションファイル"));
			}

			GUILayout.Space(5);
			this._target.SimpleBlinkHoldout = 
				RaitisEditorUtil.FoldoutWithToggle(this._target.SimpleBlinkHoldout, this._useSimpleBlinkProperty, "シンプルなまばたきアニメーションを追加");
			if (this._target.SimpleBlinkHoldout) {
				if (this._blinkClip == null) {
					EditorGUILayout.HelpBox("まばたき用アニメーションファイルが見つかりません。\nアセットを再度インポートしてください。",
						MessageType.Error);
				} else {
					using (new EditorGUI.DisabledScope(true)) {
						EditorGUILayout.ObjectField(this._controller, typeof(AnimatorController), false);
					}
				}
				using (new EditorGUI.DisabledScope(!this._target.UseSimpleBlink)) {
					EditorGUILayout.PropertyField(this._simpleBlinkSkinnedMeshRendererProperty,
						new GUIContent("顔のメッシュ"));
					GUILayout.BeginVertical(GUI.skin.box);
					GUILayout.Label("対象シェイプキー");
					string[] blendShapeNames = this._target.SimpleBlinkSkinnedMeshRenderer == null
						? Array.Empty<string>()
						: this._target.SimpleBlinkSkinnedMeshRenderer.sharedMesh.GetAllBlendShapeName();

					string[] popupValues = Enumerable.Empty<string>()
						.Append("--無し--")
						.Concat(blendShapeNames)
						.ToArray();
					foreach (SerializedProperty element in this._simpleBlinkBlendShapeIndexProperty.GetEnumerable()) {
						element.intValue =
							EditorGUILayout.Popup(element.intValue + 1, popupValues) - 1;
					}

					GUILayout.BeginHorizontal();
					{
						int arraySize = this._simpleBlinkBlendShapeIndexProperty.arraySize;
						if (GUILayout.Button("+")) {
							this._simpleBlinkBlendShapeIndexProperty.InsertArrayElementAtIndex(arraySize);
							this._simpleBlinkBlendShapeIndexProperty.GetArrayElementAtIndex(arraySize).intValue = -1;
						}

						using (new EditorGUI.DisabledScope(arraySize <= 1))
							if (GUILayout.Button("-")) {
								this._simpleBlinkBlendShapeIndexProperty.DeleteArrayElementAtIndex(arraySize - 1);
							}
					}

					GUILayout.EndHorizontal();
					GUILayout.EndVertical();
				}
			}

			this.DrawShowMode();
			this.DrawPreviewButton();
			this.DrawSkinnedMeshRendererHoldouts();
			this.serializedObject.ApplyModifiedProperties();
			if (this._previewEnable) {
				this.SetPreviewData();
			}
		}

		private void DrawShowMode() {
			GUILayout.Space(10);
			float labelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 250;
			this._target.SkinnedMeshRendererDisplayMode =
				(DisplayMode)EditorGUILayout.Popup("SkinnedMeshRenderer表示モード",
					(int)this._target.SkinnedMeshRendererDisplayMode,
					new[] { "全て", "使用されているオブジェクトのみ", "使用されていないオブジェクトのみ" });
			EditorGUIUtility.labelWidth = labelWidth;
		}


		private void DrawPreviewButton() {
			bool old = this._previewEnable;
			this._previewEnable = RaitisEditorUtil.ToggleButton("プレビュー", _previewEnable, "プレビュー解除");
			if (old != this._previewEnable && !this._previewEnable) {
				this.SetDefaultData();
			}
		}

		private void DrawSkinnedMeshRendererHoldouts() {
			foreach (SkinnedMeshRenderer renderer in this._meshRenderers) {
				int additionalDataIndex = this._target.FindAdditionalDataIndex(renderer);
				if (additionalDataIndex == -1) continue;
				SkinnedMeshRendererAdditionalData
					data = this._target.SkinnedMeshRendererAdditional[additionalDataIndex];

				// ReSharper disable once ConvertIfStatementToSwitchStatement
				if (this._target.SkinnedMeshRendererDisplayMode == DisplayMode.Used) {
					if (!data.BlendShapeData.Any(weightData => weightData.Use)) continue;
				} else if (this._target.SkinnedMeshRendererDisplayMode == DisplayMode.NonUsed) {
					if (data.BlendShapeData.Any(weightData => weightData.Use)) continue;
				}

				bool oldState = data.IsOpen;
				data.IsOpen = RaitisEditorUtil.Foldout(oldState, renderer.name,
					OnSkinnedMeshRendererFoldoutMenuClick, renderer, Strings.GoToObject);
				if (oldState != data.IsOpen) {
					this._target.SkinnedMeshRendererAdditional[additionalDataIndex] = data;
					// Holdoutが変更されたら更新をマーク
					EditorUtility.SetDirty(this._target);
				}

				if (!data.IsOpen) continue;
				this.DrawSkinnedMeshRendererBlendShapeWarning(data);
				SerializedProperty property =
					this._skinnedMeshRendererAdditionalProperty.GetArrayElementAtIndex(additionalDataIndex);
				this.DrawSkinnedMeshRendererContent(data, property);
			}
		}


		private void DrawSkinnedMeshRendererBlendShapeWarning(SkinnedMeshRendererAdditionalData data) {
			// TODO: 事前に確認して表示だけするように軽量化したい。
			// BUG: 現在の仕様では、プレビュー時に表示されてしまう。
			if (this._previewEnable) return;
			int shapeCount = data.Renderer.sharedMesh.blendShapeCount;
			bool flag = false;
			float[] weights = new float[shapeCount];
			for (int i = 0; i < shapeCount; i++) {
				float weight = data.Renderer.GetBlendShapeWeight(i);
				if (weight != 0) {
					flag = true;
				}

				weights[i] = weight;
			}

			if (!flag) return;
			if (!RaitisEditorUtil.HelpBoxWithButton("BlendShapeが直接変更されています。\nデフォルト表情として設定し、全て0に置き換えますか?",
				    MessageType.Info,
				    Strings.AutoSetting)) return;
			Undo.RecordObject(data.Renderer, "Set Blend Shape");
			Undo.RecordObject(this._target, "Ser Blend Shape");
			float[] defaultWeights = this._defaultBlendShapeWeightDictionary[data.Renderer];
			for (int i = 0; i < shapeCount; i++) {
				float weight = weights[i];
				if (weight == 0) continue;
				data.Renderer.SetBlendShapeWeight(i, 0);
				defaultWeights[i] = 0;
				BlendShapeWeightData shapeData = data.BlendShapeData[i];
				shapeData.Use = true;
				shapeData.Weight = weight;
				data.BlendShapeData[i] = shapeData;
			}
		}

		private void DrawSkinnedMeshRendererContent(SkinnedMeshRendererAdditionalData data,
			SerializedProperty property) {
			SerializedProperty blendShapeWeightDataProperty =
				property.FindPropertyRelative(nameof(SkinnedMeshRendererAdditionalData.BlendShapeData));

			GUILayout.Space(5);
			this.DrawAllShapeProperty(data.BlendShapeData, data.Renderer);
			for (int i = 0; i < data.BlendShapeData.Length; i++) {
				// TODO: 全てを表示させると重たいのでカリング機能の追加 難しそう
				// https://tm8r.hateblo.jp/entry/2016/07/13/193836
				SerializedProperty blendShapeWeightDataPropertyElement =
					blendShapeWeightDataProperty.GetArrayElementAtIndex(i);

				this.DrawBlendShapeProperty(blendShapeWeightDataPropertyElement, data.Renderer);
			}
		}

		private void DrawAllShapeProperty(BlendShapeWeightData[] data, SkinnedMeshRenderer renderer) {
			bool enable = data
				.Where(dataElement => this.FilteredBlendShapeName(dataElement.Name, dataElement.Index, renderer))
				.All(dataElement => dataElement.Use);
			Rect currentRect = GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight);
			currentRect = EditorGUI.IndentedRect(currentRect);
			Rect toggleRect = currentRect;
			toggleRect.x += 2;
			toggleRect.width -= 170;
			bool toggleFlag = EditorGUI.Toggle(toggleRect, enable);

			Rect labelRect = currentRect;
			labelRect.x += 22;
			labelRect.width -= 200;
			EditorGUI.LabelField(labelRect, "全てチェック");

			if (toggleFlag == enable) return;
			Undo.RecordObject(this._target, "All Check");
			for (int i = 0; i < data.Length; i++) {
				data[i].Use = toggleFlag;
			}
		}

		private void DrawBlendShapeProperty(SerializedProperty property, SkinnedMeshRenderer renderer) {
			// TODO: SerializedPropertyを事前キャッシュしたい
			SerializedProperty nameProperty = property.FindPropertyRelative(nameof(BlendShapeWeightData.Name));
			SerializedProperty indexProperty = property.FindPropertyRelative(nameof(BlendShapeWeightData.Index));
			SerializedProperty useProperty = property.FindPropertyRelative(nameof(BlendShapeWeightData.Use));
			SerializedProperty weightProperty = property.FindPropertyRelative(nameof(BlendShapeWeightData.Weight));

			if (!this.FilteredBlendShapeName(nameProperty.stringValue, indexProperty.intValue, renderer)) {
				useProperty.boolValue = false;
				return;
			}

			Rect currentRect = GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight);
			currentRect = EditorGUI.IndentedRect(currentRect);
			Rect toggleRect = currentRect;
			toggleRect.x += 2;
			toggleRect.width -= 170;
			EditorGUI.PropertyField(toggleRect, useProperty, GUIContent.none);

			Rect labelRect = currentRect;
			labelRect.x += 22;
			labelRect.width -= 200;
			EditorGUI.LabelField(labelRect, nameProperty.stringValue);

			using (new EditorGUI.DisabledScope(!useProperty.boolValue)) {
				Rect sliderRect = currentRect;
				sliderRect.x += 22 + labelRect.width;
				sliderRect.width = 170;
				EditorGUI.Slider(sliderRect, weightProperty, 0, 100, GUIContent.none);
			}
		}

		// ReSharper disable once SuggestBaseTypeForParameter
		private bool FilteredBlendShapeName(string shapeName, int index, SkinnedMeshRenderer mesh) {
			VRCAvatarDescriptor avatar = this._targetBuilder.AvatarDescriptor;
			if (avatar.VisemeSkinnedMesh == mesh) {
				switch (avatar.lipSync) {
					case VRC_AvatarDescriptor.LipSyncStyle.JawFlapBlendShape:
						if (avatar.MouthOpenBlendShapeName == shapeName) return false;
						break;
					case VRC_AvatarDescriptor.LipSyncStyle.VisemeBlendShape:
						if (avatar.VisemeBlendShapes.Contains(shapeName)) return false;
						break;
					case VRC_AvatarDescriptor.LipSyncStyle.Default:
					case VRC_AvatarDescriptor.LipSyncStyle.JawFlapBone:
					case VRC_AvatarDescriptor.LipSyncStyle.VisemeParameterOnly:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			if (!avatar.enableEyeLook) return true;
			VRCAvatarDescriptor.CustomEyeLookSettings eyeLookSettings = avatar.customEyeLookSettings;
			if (eyeLookSettings.eyelidType != VRCAvatarDescriptor.EyelidType.Blendshapes) return true;
			if (eyeLookSettings.eyelidsSkinnedMesh != mesh) return true;
			return !eyeLookSettings.eyelidsBlendshapes.Contains(index);
		}


		private static void OnSkinnedMeshRendererFoldoutMenuClick(object data, int index) {
			if (!(data is SkinnedMeshRenderer meshRenderer)) return;
			switch (index) {
				case 0:
					Selection.activeObject = meshRenderer;
					break;
			}
		}
	}
}