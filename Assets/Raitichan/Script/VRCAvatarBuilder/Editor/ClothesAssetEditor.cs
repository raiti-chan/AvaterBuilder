using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Raitichan.Script.VRCAvatarBuilder.DataBase;
using Raitichan.Script.VRCAvatarBuilder.Editor.Dialog;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VRC.Dynamics;
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
			visualElement.Add(new Button(this.ConvertDynamicBonesToPhysBones) { text = "DynamicBoneをPhysBoneに変換" });
			visualElement.Add(new Button(this.ExtractPB) { text = "PhysBoneをArmature以下から抽出" });

			return visualElement;
		}

		private void ConvertDynamicBonesToPhysBones() {
			GameObject[] obj = {
				this._target.gameObject
			};
			Animator animator = this._target.gameObject.GetComponent<Animator>();
			if (animator == null) {
				animator = this._target.gameObject.AddComponent<Animator>();
			}

			AvatarDynamicsSetup.ConvertDynamicBonesToPhysBones(obj);
			DestroyImmediate(animator);
		}

		private void OnClickAnalyze() {
			this._target.SkinnedMeshes = this._target.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
			this._target.HumanoidBones[ClothesAsset.ARMATURE_BONE_INDEX] = this.FindArmature();
			this._target.PhysBones = this._target.gameObject.GetComponentsInChildren<VRCPhysBoneBase>();
			this._target.PhysBoneColliders = this._target.gameObject.GetComponentsInChildren<VRCPhysBoneColliderBase>();
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

		private void ExtractPB() {
			GameObject pbRoot = new GameObject("PhysBones") {
				transform = {
					parent = this._target.transform
				}
			};

			Undo.RecordObject(this._target, "ExtractPB");
			Undo.RegisterCreatedObjectUndo(pbRoot, "ExtractPB");
			for (int index = 0; index < this._target.PhysBones.Length; index++) {
				VRCPhysBoneBase srcPhysBone = this._target.PhysBones[index];

				VRCPhysBoneBase newPhysBone = Instantiate(srcPhysBone, pbRoot.transform, true);
				newPhysBone.rootTransform = srcPhysBone.rootTransform == null
					? srcPhysBone.transform
					: srcPhysBone.rootTransform;
				newPhysBone.name =
					GameObjectUtility.GetUniqueNameForSibling(pbRoot.transform, newPhysBone.rootTransform.name + ".PB");

				foreach (VRCPhysBoneBase subComponent in newPhysBone.GetComponents<VRCPhysBoneBase>()) {
					// 一緒に複製された同一GameObject内のコンポーネントを削除
					if (subComponent.rootTransform == newPhysBone.rootTransform) continue;
					DestroyImmediate(subComponent);
				}

				this._target.PhysBones[index] = newPhysBone;
				Undo.DestroyObjectImmediate(srcPhysBone);
			}


			GameObject pbcRoot = new GameObject("PhysBoneColliders") {
				transform = {
					parent = this._target.transform
				}
			};
			Undo.RegisterCreatedObjectUndo(pbcRoot, "ExtractPB");
			for (int index = 0; index < this._target.PhysBoneColliders.Length; index++) {
				VRCPhysBoneColliderBase srcCollider = this._target.PhysBoneColliders[index];

				VRCPhysBoneColliderBase newCollider = Instantiate(srcCollider, pbcRoot.transform, true);
				newCollider.rootTransform = srcCollider.rootTransform == null
					? srcCollider.transform
					: srcCollider.rootTransform;
				newCollider.name =
					GameObjectUtility.GetUniqueNameForSibling(pbcRoot.transform,
						newCollider.rootTransform.name + ".PBC");
				
				foreach (Transform transform in newCollider.transform.Cast<Transform>()) {
					// 一緒に複製された子を削除
					DestroyImmediate(transform.gameObject);
				}

				foreach (VRCPhysBoneBase physBone in this._target.PhysBones) {
					// PhysBoneColliderの参照を新しいコライダーへ差し替え
					for (int i = 0; i < physBone.colliders.Count; i++) {
						if (physBone.colliders[i] != srcCollider) continue;
						Undo.RecordObject(physBone, "Replace PhysBoneCollider");
						physBone.colliders[i] = newCollider;
					}
				}

				this._target.PhysBoneColliders[index] = newCollider;
				Undo.DestroyObjectImmediate(srcCollider);
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