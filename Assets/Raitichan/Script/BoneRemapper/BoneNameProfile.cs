using System;
using UnityEngine;

namespace Assets.Raitichan.Script.BoneRemapper {

	[CreateAssetMenu(menuName = "Raitichan/BoneNameProfile", fileName = "BoneNameProfile")]
	public class BoneNameProfile : ScriptableObject {

		private BoneTreeItem _boneTree;
		public BoneTreeItem BoneTree {
			get {
				if (this._boneTree == null) {
					this._boneTree = BoneTreeItem.Import(this.BoneMapList);
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

	}


}