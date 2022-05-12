using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Raitichan.Script.VRCAvatarBuilder.DataBase;
using Raitichan.Script.VRCAvatarBuilder.Editor.Dialog;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VRC.SDK3.Avatars;
using static Raitichan.Script.VRCAvatarBuilder.DataBase.HumanoidBoneSubNameTable;

namespace Raitichan.Script.VRCAvatarBuilder.Editor {
	[CustomEditor(typeof(ClothesAsset))]
	public class ClothesAssetEditor : UnityEditor.Editor {
		private ClothesAsset _target;

		private SerializedProperty _humanoidBonesProperty;

		private void OnEnable() {
			this._target = this.target as ClothesAsset;

			this._humanoidBonesProperty = this.serializedObject.FindProperty(ClothesAsset.HumanoidBonesPropertyName);

			this.InitHumanoidBonesProperty();
		}

		private void InitHumanoidBonesProperty() {
			int size = this._humanoidBonesProperty.arraySize;
			if (size == ClothesAsset.HUMAN_BODY_BONE_COUNT) return;
			while (size < ClothesAsset.HUMAN_BODY_BONE_COUNT) {
				this._humanoidBonesProperty.InsertArrayElementAtIndex(size);
				size++;
			}

			while (size > ClothesAsset.HUMAN_BODY_BONE_COUNT) {
				size--;
				this._humanoidBonesProperty.DeleteArrayElementAtIndex(size);
			}
		}

		public override VisualElement CreateInspectorGUI() {
			VisualElement visualElement = new VisualElement();
			IMGUIContainer defaultInspector = new IMGUIContainer(() => { this.DrawDefaultInspector(); });
			visualElement.Add(defaultInspector);

			visualElement.Add(new Button(this.OnClickAnalyze) { text = "解析" });
			visualElement.Add(new Button(() => {
				GameObject[] obj = new[] {
					this._target.gameObject
				};
				// TODO: なぜかアニメーターが無いとエラー
				AvatarDynamicsSetup.ConvertDynamicBonesToPhysBones(obj);
			}){text = "DynamicBoneをPhysBoneに変換"});

			return visualElement;
		}

		private void OnClickAnalyze() {
			this._target.SkinnedMeshes = this._target.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
			this._target.HumanoidBones[ClothesAsset.ARMATURE_BONE_INDEX] = this.FindArmature();
			this.PrefixAndSuffixSetting();
			EditorUtility.SetDirty(this._target);
		}

		private Transform FindArmature() {
			if (this._target.SkinnedMeshes.Length <= 0)
				// TODO: この方法だとヒエラルキーからD&DできないのでGUIで選択式に
				return SingleObjectDialog.DisplayDialog<Transform>("エラー", "Armatureが見つかりませんでした。\n手動で設定してください。", true);
			Transform baseBone = this._target.SkinnedMeshes[0].bones[0];
			while (baseBone.parent != this._target.transform) {
				baseBone = baseBone.parent;
			}

			return baseBone;
		}

		private void PrefixAndSuffixSetting() {
			Transform armature = (this._target.HumanoidBones[ClothesAsset.ARMATURE_BONE_INDEX]);
			if (armature == null) return;
			if (armature.childCount <= 0) return;
			string hipsName = armature.GetChild(0).name;

			foreach (HumanoidBoneSubNameElement element in GlobalDataBase.Instance.HumanoidBoneSubNameTable
				         .FindByBoneIndex((int)HumanBodyBones.Hips)) {
				Regex regex = new Regex($"(?<Prefix>.*){Regex.Escape(element.Name)}(?<Suffix>.*)");
				Match match = regex.Match(hipsName);
				if (!match.Success) continue;
				this._target.Prefix = match.Groups["Prefix"].Value;
				this._target.Suffix = match.Groups["Suffix"].Value;
			}

			
		}

		#region Gizmo

		[DrawGizmo(GizmoType.Selected)]
		private static void DrawGizmo(ClothesAsset clothesAsset, GizmoType gizmoType) {
			Gizmos.color = Color.green;
			DrawAllBone(clothesAsset.transform);
		}

		private const float _GIZMO_SPHERE_RADIUS = 0.003f;

		private static void DrawAllBone(Transform root) {
			foreach (Transform child in root.Cast<Transform>()) {
				DrawBoneGizmo(root, child);
				DrawAllBone(child);
			}

			if (root.childCount == 0) {
				Gizmos.DrawWireSphere(root.position, _GIZMO_SPHERE_RADIUS);
			}
		}

		private static void DrawBoneGizmo(Transform start, Transform end) {
			Vector3 startPosition = start.position;
			Vector3 endPosition = end.position;
			Matrix4x4 origin = Gizmos.matrix;
			Vector3 direction = (startPosition - endPosition).normalized;
			if (direction == Vector3.zero) return;
			Quaternion rotation = Quaternion.LookRotation(direction);
			float distance = Vector3.Distance(startPosition, endPosition);
			float frustumDistance = distance - _GIZMO_SPHERE_RADIUS;

			Gizmos.matrix = Matrix4x4.TRS(endPosition, rotation, Vector3.one);
			Gizmos.DrawWireSphere(new Vector3(0, 0, distance), _GIZMO_SPHERE_RADIUS);
			Gizmos.DrawFrustum(new Vector3(0, 0, _GIZMO_SPHERE_RADIUS), CalcFOV(frustumDistance, 0.05f),
				frustumDistance, 0f, 1.0f);
			Gizmos.matrix = origin;
		}

		private static float CalcFOV(float h, float l) {
			float length20 = h / Mathf.Tan(80f * Mathf.Deg2Rad) * 2f;
			if (length20 < l) {
				return 20f;
			}

			float c = Mathf.Atan((l / 2f) / h);
			return c * 2f * Mathf.Rad2Deg;
		}

		#endregion
	}
}