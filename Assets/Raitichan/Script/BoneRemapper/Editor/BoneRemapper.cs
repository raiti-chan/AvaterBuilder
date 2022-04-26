using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Raitichan.Script.Util.Extension;
using UnityEditor;
using UnityEngine;

namespace Assets.Raitichan.Script.BoneRemapper.Editor {
	public class BoneRemapper : EditorWindow {

		[MenuItem("Raitichan/BoneRemapper")]
		private static void ShowWindow() {
			BoneRemapper window = GetWindow<BoneRemapper>();
			window.titleContent = new GUIContent("Bone Remapper");
			window.Show();
		}

		private Transform _targetArmature;

		private Transform _srcArmature;

		private BoneNameProfile _profile;

		private bool _isShowOption;

		private string _prifix;

		private string _suffix;

		private Regex _fixRegex;

		private bool _isAllRemap;

		private Dictionary<Transform, Transform> _boneMap = new Dictionary<Transform, Transform>();

		[SerializeField]
		private SkinnedMeshRenderer[] _srcMeshRenderers;

		private void Awake() {
			SkinnedMeshRenderer[] skinnedMeshRenderers = Selection.GetFiltered<SkinnedMeshRenderer>(SelectionMode.Unfiltered);
			if (skinnedMeshRenderers.Length <= 0) {
				return;
			}
			this._srcMeshRenderers = skinnedMeshRenderers;
		}

		private void OnGUI() {

			SerializedObject so = new SerializedObject(this);
			EditorGUILayout.PropertyField(so.FindProperty(nameof(this._srcMeshRenderers)), true);
			so.ApplyModifiedProperties();
			this._srcArmature = EditorGUILayout.ObjectField("転送元アーマチュア", this._srcArmature, typeof(Transform), true) as Transform;
			this._profile = EditorGUILayout.ObjectField("ボーン名プロファイル", this._profile, typeof(BoneNameProfile), false) as BoneNameProfile;
			this._targetArmature = EditorGUILayout.ObjectField("転送先アーマチュア", this._targetArmature, typeof(Transform), true) as Transform;

			this._isShowOption = EditorGUILayout.Foldout(this._isShowOption, "詳細オプション");
			if (this._isShowOption) {
				using (new EditorGUI.IndentLevelScope(1)) {
					using (EditorGUI.ChangeCheckScope fixChanged = new EditorGUI.ChangeCheckScope()) {
						this._prifix = EditorGUILayout.TextField("接頭辞", this._prifix);
						this._suffix = EditorGUILayout.TextField("接尾辞", this._suffix);

						if (fixChanged.changed) {
							this._fixRegex = new Regex($"{this._prifix}(?<name>(.*)){this._suffix}");
						}
					}
					this._isAllRemap = EditorGUILayout.Toggle(new GUIContent("位置が一致しないボーン", "位置が一致しないボーンも再ターゲットする"), this._isAllRemap);
				}
			}

			using (new EditorGUI.DisabledGroupScope(this._profile is null)) {
				if (GUILayout.Button("自動マッピング")) {
					this.DoMapping(this._targetArmature, this._srcArmature, this._profile.BoneTree);
				}

				using (new GUILayout.HorizontalScope()) {
					if (GUILayout.Button("すべての転送元を転送先の位置へ")) {
						this.DoTransferPositionAll();
					}
					if (GUILayout.Button("対象ボーンの子へ")) {
						this.DoTransferChild();
					}
					if (GUILayout.Button("対象ボーンをリターゲット")) {
						this.DoRetargetBone(this._isAllRemap);
						this._boneMap.Clear();
					}
					/*
					if (GUILayout.Button("オフセットを保持したままボーンをリターゲット")) {
						this.DoRetargetBoneKeepOffset();
						this._boneMap.Clear();
					}
					*/
				}

			}


			using (new GUILayout.HorizontalScope()) {
				this.DrawBoneTree();
				this.DrawBoneDrawer();
				this.DrawOther();
			}
		}

		private Vector2 _boneTreePos;

		private void DrawBoneTree() {
			using (new GUILayout.VerticalScope(GUI.skin.box, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true))) {
				EditorGUILayout.LabelField("転送元アーマチュア");
				using (GUILayout.ScrollViewScope scroll = new GUILayout.ScrollViewScope(this._boneTreePos)) {
					this._boneTreePos = scroll.scrollPosition;

					if (this._srcArmature == null) {
						return;
					}
					using (new EditorGUI.DisabledGroupScope(true)) {
						this.DrawBoneTreeItem(this._srcArmature);
					}
				}
			}
		}


		private void DrawBoneTreeItem(Transform bone) {
			EditorGUILayout.ObjectField(bone, typeof(Transform), true);

			using (new EditorGUI.IndentLevelScope(1)) {
				foreach (Transform child in bone) {
					if (this.CheckBone(child)) {
						this.DrawBoneTreeItem(child);
					}
				}
			}
		}

		private void DrawBoneDrawer() {
			using (new GUILayout.VerticalScope(GUI.skin.box, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true))) {
				EditorGUILayout.LabelField("転送先アーマチュア");
				using (GUILayout.ScrollViewScope scroll = new GUILayout.ScrollViewScope(this._boneTreePos)) {
					this._boneTreePos = scroll.scrollPosition;

					if (this._srcArmature == null) {
						return;
					}

					this.DrawBoneDrawerItem(this._srcArmature);

				}
			}
		}

		private void DrawBoneDrawerItem(Transform bone) {
			this._boneMap.TryGetValue(bone, out Transform target);
			this._boneMap[bone] = EditorGUILayout.ObjectField(target, typeof(Transform), true) as Transform;

			foreach (Transform child in bone) {
				if (this.CheckBone(child)) {
					this.DrawBoneDrawerItem(child);
				}
			}

		}

		private void DrawOther() {
			using (new GUILayout.VerticalScope(GUI.skin.box, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true))) {
				EditorGUILayout.LabelField("プロパティ");
				using (GUILayout.ScrollViewScope scroll = new GUILayout.ScrollViewScope(this._boneTreePos)) {
					this._boneTreePos = scroll.scrollPosition;

					if (this._srcArmature == null) {
						return;
					}

					this.DrawOtherItem(this._srcArmature);

				}
			}
		}

		private void DrawOtherItem(Transform bone) {
			using (new GUILayout.HorizontalScope()) {
				EditorGUILayout.LabelField("", GUILayout.MaxWidth(5));
				if (this._boneMap[bone] == null) {
					EditorGUILayout.LabelField("Not Mapping");
				} else {
					Transform target = this._boneMap[bone];
					if (bone.position == target.position && bone.rotation == target.rotation) {
						EditorGUILayout.LabelField("Position Equal");
					} else {
						if (GUILayout.Button("転送元を同じ位置へ", GUILayout.MaxWidth(130))) {
							Undo.RecordObject(bone, "転送元を同じ位置へ");
							bone.position = target.localPosition;
							bone.rotation = target.localRotation;
						}
					}
				}

			}

			foreach (Transform child in bone) {
				if (this.CheckBone(child)) {
					this.DrawOtherItem(child);
				}
			}

		}

		private void DoMapping(Transform target, Transform src, BoneTreeItem item) {
			if (src == null) {
				return;
			}
			this._boneMap[src] = target;
			if (item == null) {
				return;
			}
			foreach (Transform child in src) {
				BoneTreeItem childItem;
				if (this._fixRegex == null) {
					childItem = item.Find(child.name);
				} else {
					Match match = this._fixRegex.Match(child.name);
					if (match.Success) {
						childItem = item.Find(match.Groups["name"].Value);
					} else {
						childItem = item.Find(child.name);
					}
				}
				Transform targetChild = target.Find(childItem);
				this.DoMapping(targetChild, child, childItem);
			}
		}

		private void DoTransferPositionAll() {
			foreach (var (key, value) in this._boneMap.Select(element => (element.Key, element.Value))) {
				if (value != null) {
					Undo.RecordObject(key, "すべての転送元を同じ位置へ");
					key.position = value.position;
					key.rotation = value.rotation;
				}
			}
		}

		private void DoTransferChild() {
			foreach (var (key, value) in this._boneMap.Select(element => (element.Key, element.Value))) {
				if (value != null) {
					Undo.RecordObject(key, "対象ボーンの子へ");
					key.parent = value;
				}
			}
		}

		private void DoRetargetBone(bool all) {
			if (!EditorUtility.DisplayDialog("警告", "この操作は取り消せません。シーンを一時保存しておくことをおすすめします。取り消す際は保存せず再度シーンを開いてください。", "続行", "取り消し")) {
				return;
			}
			foreach (var (key, value) in this._boneMap.Select(element => (element.Key, element.Value))) {
				if (value != null) {
					if (all || (key.position == value.position && key.rotation == value.rotation)) {
						foreach (SkinnedMeshRenderer skinned in this._srcMeshRenderers) {
							int index = Array.IndexOf(skinned.bones, key);
							if (index >= 0) {
								Transform[] bones = skinned.bones;
								bones[index] = value;
								skinned.bones = bones;
							}
						}
						bool hasComponent = key.gameObject.GetComponents<Component>()
							.Any(component => !(component is Transform));
						if (!hasComponent) {
							while (key.childCount != 0) {
								Transform child = key.GetChild(0);
								child.parent = value;
							}

							Undo.DestroyObjectImmediate(key.gameObject);
						} else {
							key.parent = value;
						}
					} else {
						key.parent = value;
					}
				}
			}
		}

		private void DoRetargetBoneKeepOffset() {
			if (!EditorUtility.DisplayDialog("警告", "この操作は取り消せません。シーンを一時保存しておくことをおすすめします。取り消す際は保存せず再度シーンを開いてください。", "続行", "取り消し")) {
				return;
			}
			foreach (SkinnedMeshRenderer skinned in this._srcMeshRenderers) {
				Mesh copy = Instantiate(skinned.sharedMesh);
				string path = EditorUtility.SaveFilePanelInProject("コピーされたメッシュの保存先", skinned.sharedMesh.name + "_clone", "asset", "");
				if (string.IsNullOrEmpty(path)) {
					return;
				}
				AssetDatabase.CreateAsset(copy, path);
				AssetDatabase.SaveAssets();
				skinned.sharedMesh = copy;
			}
		}



		private bool CheckBone(Transform targetBone) {
			// このボーンがどれかしらのメッシュのボーンリストに乗っているか。
			bool isInBoneList = this._srcMeshRenderers
				.Where(renderer => renderer != null)
				.SelectMany(renderers => renderers.bones)
				.Distinct()
				.Any(bone => bone == targetBone);

			// このボーンの子ボーンがどれかしらのメッシュのボーンリストに乗っているか。
			if (!isInBoneList) {
				foreach (Transform child in targetBone) {
					if (this.CheckBone(child)) {
						return true;
					}
				}
			}


			return isInBoneList;
		}
	}
}