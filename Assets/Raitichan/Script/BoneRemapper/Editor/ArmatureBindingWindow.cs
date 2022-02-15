using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Raitichan.Script.BoneRemapper.Editor {
	public class ArmatureBindingWindow : EditorWindow {

		private BoneNameProfile _profile;

		private Vector2 _scrollPos;

		private Transform _target;

		private Avatar _targetAvatar;

		private Dictionary<BoneTreeItem, Transform> _boneDictionaly = new Dictionary<BoneTreeItem, Transform>();

		public static void Open(BoneNameProfile profile) {
			ArmatureBindingWindow window = GetWindow<ArmatureBindingWindow>();
			window.titleContent = new GUIContent("Armature Binding");
			window._profile = profile;
			window.Show();
		}


		private void OnGUI() {
			EditorGUILayout.LabelField("Armature Mapper");
			this._target = EditorGUILayout.ObjectField("自動マップ対象 Armature", this._target, typeof(Transform), true) as Transform;
			bool enableAutoMappingFromArmature = false;
			bool enableAutoMappingFromAvatar = false;

			using (new EditorGUI.DisabledScope(this._target is null)) {
				enableAutoMappingFromArmature = GUILayout.Button("Armatureから自動マッピング");
				if (enableAutoMappingFromArmature) {
					this._boneDictionaly.Clear();
					this.BoneMappingWithArmature(this._profile.BoneTree, this._target);
				}
			}

			this._targetAvatar = EditorGUILayout.ObjectField("自動マップ対象 Avatar", this._targetAvatar, typeof(Avatar), false) as Avatar;
			using (new EditorGUI.DisabledGroupScope(this._targetAvatar is null || !this._targetAvatar.isHuman || this._target is null)) {
				enableAutoMappingFromAvatar = GUILayout.Button("Avatarから自動マッピング(推奨)");
				if (enableAutoMappingFromAvatar) {
					this._boneDictionaly.Clear();
					this.BoneMappingWithAvatar(this._profile.BoneTree, this._target, this._targetAvatar);
				}
			}


			using (new GUILayout.HorizontalScope()) {

				using (new GUILayout.VerticalScope())
				using (var scroll = new GUILayout.ScrollViewScope(this._scrollPos, false, false, GUIStyle.none, GUIStyle.none)) {
					this._scrollPos = scroll.scrollPosition;
					EditorGUI.indentLevel++;
					this.DrawBone(this._profile.BoneTree);
					EditorGUI.indentLevel--;
				}

				using (new GUILayout.VerticalScope())
				using (var scroll = new GUILayout.ScrollViewScope(this._scrollPos)) {
					this._scrollPos = scroll.scrollPosition;
					EditorGUI.indentLevel++;
					this.DrawBoneDrawer(this._profile.BoneTree);
					EditorGUI.indentLevel--;
				}
			}

			if (GUILayout.Button("登録")) {
				foreach (var (key, value) in this._boneDictionaly.Select(element => (element.Key, element.Value))) {
					if (value == null) {
						continue;
					}
					if (key.BaseName == value.name) {
						continue;
					}
					key.SubNames.Add(value.name);

				}
				this._profile.BoneMapList = this._profile.BoneTree.Export();
				EditorUtility.SetDirty(this._profile);
			}
		}

		private void DrawBone(BoneTreeItem item) {
			EditorGUILayout.LabelField(item.BaseName);
			EditorGUI.indentLevel++;
			foreach (BoneTreeItem child in item.Childs) {
				this.DrawBone(child);
			}
			EditorGUI.indentLevel--;

		}

		private void BoneMappingWithArmature(BoneTreeItem item, Transform targetBone) {
			this._boneDictionaly[item] = targetBone;
			if (targetBone == null) return;
			for (int i = 0; i < item.Childs.Count; i++) {
				BoneTreeItem child = item.Childs[i];
				Transform childBone = targetBone.Find(child.BaseName);
				if (childBone == null) {

					childBone = child.SubNames
						.Select(name => targetBone.Find(name))
						.Where(bone => bone != null)
						.FirstOrDefault();
				}

				if (childBone == null & i < targetBone.childCount) {
					childBone = targetBone.GetChild(i);
				}
				this.BoneMappingWithArmature(child, childBone);
			}
		}

		private void BoneMappingWithAvatar(BoneTreeItem item, Transform targetBone, Avatar avatar) {
			this._boneDictionaly[item] = targetBone;
			if (targetBone == null) return;
			for (int i = 0; i < item.Childs.Count; i++) {
				BoneTreeItem child = item.Childs[i];

				string boneName = GetBoneNameWithBoneTreeItem(child, avatar);
				Transform childBone = null;
				if (!string.IsNullOrEmpty(boneName)) {
					childBone = targetBone.Find(boneName);
				}
				if (childBone == null) {
					childBone = targetBone.Find(child.BaseName);
				}
				if (childBone == null) {

					childBone = child.SubNames
						.Select(name => targetBone.Find(name))
						.Where(bone => bone != null)
						.FirstOrDefault();
				}

				this.BoneMappingWithAvatar(child, childBone, avatar);
			}
		}

		private static string GetBoneNameWithBoneTreeItem(BoneTreeItem item, Avatar avatar) {
			IEnumerable<string> names = item.SubNames.Prepend(item.BaseName);
			return avatar.humanDescription.human
				.Where(bone => names.Contains(bone.humanName))
				.Select(bone => bone.boneName)
				.FirstOrDefault();
		}

		private void DrawBoneDrawer(BoneTreeItem item) {
			this._boneDictionaly.TryGetValue(item, out Transform bone);
			this._boneDictionaly[item] = EditorGUILayout.ObjectField(bone, typeof(Transform), true) as Transform;
			foreach (BoneTreeItem child in item.Childs) {
				this.DrawBoneDrawer(child);
			}
		}

	}
}
