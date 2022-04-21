using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Raitichan.Script.BoneRemapper.Editor {
	[CustomEditor(typeof(BoneNameProfile))]
	public class BoneNameProfileEditor : UnityEditor.Editor {

		private BoneNameProfile _target;
		private Transform _armature;
		private Avatar _avatar;
		private Vector2 _scrollPos;

		public void OnEnable() {
			this._armature = null;
			this._target = this.target as BoneNameProfile;
		}

		public override void OnInspectorGUI() {
			this.serializedObject.Update();
			SerializedProperty boneMapList = this.serializedObject.FindProperty(BoneNameProfile.BoneMapListPropertyName);
			SerializedProperty isOnlyHumanBone = this.serializedObject.FindProperty(BoneNameProfile.IsOnlyHumanBonePropertyName);

			if (boneMapList.arraySize == 0) {
				this._armature = EditorGUILayout.ObjectField("インポート元 Armature", this._armature, typeof(Transform), true) as Transform;
				this._avatar = EditorGUILayout.ObjectField("インポート元 Avatar", this._avatar, typeof(Avatar), false) as Avatar;
				isOnlyHumanBone.boolValue = GUILayout.Toggle(isOnlyHumanBone.boolValue, "ヒューマノイドボーンのみをインポート");

				using (new EditorGUI.DisabledGroupScope(this._armature == null || this._avatar == null)) {
					if (GUILayout.Button("インポート")) {
						this._target.ImportArmature(this._armature, this._avatar, this._target.IsOnlyHumanBone);
						this.FlashBoneTree();
					}
				}
			} else {
				if (GUILayout.Button("ボーンプロファイルデータ削除")) {
					if (EditorUtility.DisplayDialog("警告", "ボーンプロファイルデータをすべて削除します。", "はい", "キャンセル")) {
						this._target.BoneTree = null;
						this._target.BoneMapList = new BoneMapListItem[0];
						EditorUtility.SetDirty(this._target);
						this.serializedObject.ApplyModifiedProperties();
						return;
					}
				}

				if (GUILayout.Button("別 Armature のバインド")) {
					ArmatureBindingWindow.Open(this._target);
				}
				using (var scroll = new GUILayout.ScrollViewScope(this._scrollPos)) {
					EditorGUI.indentLevel++;
					this._scrollPos = scroll.scrollPosition;
					if (!this.DrawBoneTree(this._target.BoneTree, null, 0)) {
						this._target.BoneTree = null;
						this._target.BoneMapList = new BoneMapListItem[0];
						EditorUtility.SetDirty(this._target);
					}
					EditorGUI.indentLevel--;
				}
			}

			this.serializedObject.ApplyModifiedProperties();
		}

		private bool DrawBoneTree(BoneTreeItem item, BoneTreeItem parent, int index) {
			string name = item.SubNames
				.DefaultIfEmpty(" ")
				.Aggregate((a, b) => a + ", " + b);
			using (new GUILayout.HorizontalScope()) {
				item.IsOpen = EditorGUILayout.Foldout(item.IsOpen, $"{item.BaseName} : [{name}]");
				if (GUILayout.Button(new GUIContent("Edit", "ボーン情報の編集"), GUILayout.MaxWidth(40))) {
					BoneSettingWindow.Open(item, this._target);
				}

				using (new EditorGUI.DisabledGroupScope(index <= 0)) {
					if (GUILayout.Button(new GUIContent("▲", "上へ"), GUILayout.MaxWidth(20))) {
						parent.Childs.RemoveAt(index);
						parent.Childs.Insert(index - 1, item);
						this.FlashBoneTree();
					}
				}

				using (new EditorGUI.DisabledGroupScope(parent == null || parent.Childs.Count - 1 <= index)) {
					if (GUILayout.Button(new GUIContent("▼", "下へ"), GUILayout.MaxWidth(20))) {
						parent.Childs.RemoveAt(index);
						parent.Childs.Insert(index + 1, item);
						this.FlashBoneTree();
					}
				}

				if (GUILayout.Button(new GUIContent("✚", "子ボーンの追加"), GUILayout.MaxWidth(20))) {
					BoneTreeItem create = new BoneTreeItem();
					item.Childs.Add(create);
					BoneSettingWindow.Open(create, this._target);
				}
				if (GUILayout.Button(new GUIContent("✕", "ボーン情報の削除"), GUILayout.MaxWidth(20))) {
					return false;
				}
			}
			if (!item.IsOpen) {
				return true;
			}

			EditorGUI.indentLevel++;
			for (int i = 0; i < item.Childs.Count; i++) {
				if (!this.DrawBoneTree(item.Childs[i], item, i)) {
					item.Childs.RemoveAt(i);
					i--;
					this.FlashBoneTree();
				}
			}
			EditorGUI.indentLevel--;
			return true;
		}

		private void FlashBoneTree() {
			this._target.BoneMapList = this._target.BoneTree.Export();
			EditorUtility.SetDirty(this._target);
		}
	}

	

	
}
