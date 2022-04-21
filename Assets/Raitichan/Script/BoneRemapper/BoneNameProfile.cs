using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Raitichan.Script.BoneRemapper {

	[CreateAssetMenu(menuName = "Raitichan/BoneNameProfile", fileName = "BoneNameProfile")]
	public class BoneNameProfile : ScriptableObject {

		private BoneTreeItem _boneTree;
		public BoneTreeItem BoneTree {
			get {
				if (this._boneTree == null) {
					this._boneTree = BoneTreeItem.Construct(this.BoneMapList);
				}
				return this._boneTree;
			}
			set {
				this._boneTree = value;
			}
		}

		[SerializeField]
		private BoneMapListItem[] _boneMapList;
		public BoneMapListItem[] BoneMapList { get => this._boneMapList; set => this._boneMapList = value; }
		public static string BoneMapListPropertyName { get => nameof(_boneMapList); }

		[SerializeField]
		private bool _isOnlyHumanBone = true;
		public bool IsOnlyHumanBone { get => this._isOnlyHumanBone; set => this._isOnlyHumanBone = value; }
		public static string IsOnlyHumanBonePropertyName { get => nameof(_isOnlyHumanBone); }

		/// <summary>
		/// アーマチュアのインポート
		/// </summary>
		/// <param name="armature"></param>
		/// <param name="avatar"></param>
		/// <param name="isOnlyHumanBone"></param>
		public void ImportArmature(Transform armature, Avatar avatar, bool isOnlyHumanBone = true) {
			this.IsOnlyHumanBone = isOnlyHumanBone;

			BoneTreeItem tree = new BoneTreeItem {
				BaseName = "Armature",
				Childs = new List<BoneTreeItem>(armature.childCount),
				SubNames = new HashSet<string>(),
				HumanBoneIndex = -1,
			};

			tree.SubNames.Add("armature");

			for (int i = 0; i < armature.childCount; i++) {
				tree.Childs.Add(new BoneTreeItem());
				this.ImportBone(armature.GetChild(i), tree.Childs[i], avatar);
			}
			tree.CleanAll();
			this.BoneTree = tree;
			this.BoneMapList = tree.Export();
		}

		private void ImportBone(Transform bone, BoneTreeItem tree, Avatar avatar) {
			tree.BaseName = avatar.humanDescription.human
				.Where(element => element.boneName == bone.name)
				.Select(element => element.humanName)
				.FirstOrDefault();
			if (string.IsNullOrEmpty(tree.BaseName)) {
				tree.HumanBoneIndex = -1;
				tree.BaseName = this.IsOnlyHumanBone ? "" : bone.name;
			} else {
				string enumName = tree.BaseName.Replace(" ", string.Empty);
				tree.HumanBoneIndex = (int)Enum.Parse(typeof(HumanBodyBones), enumName);
				if (enumName != tree.BaseName) {
					tree.SubNames.Add(enumName);
				}
			}

			if (tree.BaseName != bone.name) {
				tree.SubNames.Add(bone.name);
			}

			string lowerName = bone.name.ToLower();
			if (lowerName != bone.name) {
				tree.SubNames.Add(lowerName);
			}

			tree.Childs = new List<BoneTreeItem>(bone.childCount);

			for (int i = 0; i < bone.childCount; i++) {
				tree.Childs.Add(new BoneTreeItem());
				this.ImportBone(bone.GetChild(i), tree.Childs[i], avatar);
			}

		}
	}


	[Serializable]
	public class BoneMapListItem {
		[SerializeField]
		private string _baseName;
		public string BaseName { get => this._baseName; set => this._baseName = value; }

		[SerializeField]
		private string[] _subNames = new string[0];
		public string[] SubNames { get => this._subNames; set => this._subNames = value; }

		[SerializeField]
		private int[] _path = new int[0];
		public int[] Path { get => this._path; set => this._path = value; }

		[SerializeField]
		private int _humanBoneIndex;
		public int HumanBoneIndex { get => this._humanBoneIndex; set => this._humanBoneIndex = value; }

	}
}