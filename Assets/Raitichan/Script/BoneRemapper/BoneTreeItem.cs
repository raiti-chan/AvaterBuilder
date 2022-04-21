using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Raitichan.Script.BoneRemapper {
	/// <summary>
	/// ボーンのツリー構造を表すクラス
	/// </summary>
	public class BoneTreeItem {

		/// <summary>
		/// この要素自体の名前
		/// </summary>
		public string BaseName { get => this._baseName; set => this._baseName = value; }
		private string _baseName;

		/// <summary>
		/// 子要素
		/// </summary>
		public List<BoneTreeItem> Childs { get => this._childs; set => this._childs = value; }
		private List<BoneTreeItem> _childs = new List<BoneTreeItem>();

		/// <summary>
		/// この要素の別名
		/// </summary>
		public HashSet<string> SubNames { get => this._subNames; set => this._subNames = value; }
		private HashSet<string> _subNames = new HashSet<string>();


		/// <summary>
		/// ヒューマノイドボーンインデックス
		/// ヒューマノイドボーンでない場合-1
		/// </summary>
		public int HumanBoneIndex { get => this._humanBoneIndex; set => this._humanBoneIndex = value; }
		public int _humanBoneIndex;

		/// <summary>
		/// この要素が展開されるか、表示用
		/// </summary>
		public bool IsOpen { get => this._isOpen; set => this._isOpen = value; }
		private bool _isOpen;


		/// <summary>
		/// すべての階層から
		/// </summary>
		public void CleanAll() {
			this.Clean();
			foreach (BoneTreeItem item in this.Childs) {
				item.CleanAll();
			}
		}

		/// <summary>
		/// この階層からBaseNameが空の要素を削除する
		/// </summary>
		public void Clean() {
			this._childs = this._childs
				.Where(item => !string.IsNullOrEmpty(item._baseName))
				.ToList();
		}

		/// <summary>
		/// このツリー要素の子から一致する名前の要素を検索
		/// </summary>
		/// <param name="name">検索する名前</param>
		/// <returns>見つかった要素</returns>
		public BoneTreeItem Find(string name) {
			foreach (BoneTreeItem child in this.Childs) {
				if (child.Match(name)) {
					return child;
				}
			}
			return null;
		}

		/// <summary>
		/// このツリー要素の子から一致する名前の要素を再帰的に検索
		/// </summary>
		/// <param name="name">検索する名前</param>
		/// <returns>見つかった要素</returns>
		public BoneTreeItem FindChild(string name) {
			foreach (BoneTreeItem child in this.Childs) {
				if (child.Match(name)) {
					return child;
				}
				BoneTreeItem item = child.FindChild(name);
				if (item != null) {
					return item;
				}
			}
			return null;
		}

		/// <summary>
		/// 指定された要素の次の要素を取得します。
		/// 指定された要素が子に無い場合、又は次の要素が存在しない場合はnullを返します。
		/// </summary>
		/// <param name="before"></param>
		/// <returns></returns>
		public BoneTreeItem Next(BoneTreeItem before) {
			bool flag = false;
			foreach (BoneTreeItem child in this.Childs) {
				if (flag) return child;
				flag = child == before;
			}
			return null;
		}

		public bool Match(string name) {
			return this.BaseName == name || this.SubNames.Contains(name);
		}

		/// <summary>
		/// リストデータからツリーを構築します。
		/// </summary>
		/// <param name="boneMapList">データ</param>
		/// <returns>構築されたツリー</returns>
		public static BoneTreeItem Construct(BoneMapListItem[] boneMapList) {
			BoneTreeItem root = new BoneTreeItem();
			var list = boneMapList
				.OrderBy(element => element.Path.Length)
				.ThenBy(element => element.Path[element.Path.Length - 1]);

			foreach (BoneMapListItem item in list) {
				BoneTreeItem current = root;
				foreach (int index in item.Path) {
					while (index >= current.Childs.Count) {
						current.Childs.Add(new BoneTreeItem());
					}
					current = current.Childs[index];
				}
				current.BaseName = item.BaseName;
				current.SubNames = new HashSet<string>(item.SubNames);
				current.HumanBoneIndex = item.HumanBoneIndex;
			}

			return root.Childs[0];
		}

		/// <summary>
		/// ツリー構造をリストデータにエクスポートします。
		/// </summary>
		/// <returns>リストデータ</returns>
		public BoneMapListItem[] Export() {
			List<BoneMapListItem> list = new List<BoneMapListItem>();
			BoneMapListItem item = new BoneMapListItem() {
				Path = new int[] { 0 },
				BaseName = this.BaseName,
				SubNames = this.SubNames.ToArray(),
				HumanBoneIndex = this.HumanBoneIndex,
			};
			list.Add(item);

			for (int i = 0; i < this.Childs.Count; i++) {
				this.Childs[i].Export(item.Path, list, i);
			}

			return list
				.OrderBy(element => element.Path.Length)
				.ThenBy(element => element.Path[element.Path.Length - 1])
				.ToArray();
		}

		/// <summary>
		/// 子要素を再帰的にリストデータ化する。
		/// </summary>
		/// <param name="path">この要素の親までのパス情報</param>
		/// <param name="list">格納先リスト</param>
		/// <param name="index">親内のこの要素のインデックス</param>
		private void Export(int[] path, List<BoneMapListItem> list, int index) {
			BoneMapListItem item = new BoneMapListItem() {
				Path = path.Append(index).ToArray(),
				BaseName = this.BaseName,
				SubNames = this.SubNames.ToArray(),
				HumanBoneIndex = this.HumanBoneIndex,
			};
			list.Add(item);
			for (int i = 0; i < this.Childs.Count; i++) {
				this.Childs[i].Export(item.Path, list, i);
			}
		}

	}
}
