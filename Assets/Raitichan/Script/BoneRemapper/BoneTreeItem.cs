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

		public BoneTreeItem Find(string name) {

			foreach(BoneTreeItem child in this.Childs) {
				if (child.BaseName == name) {
					return child;
				}
				if (child.SubNames.Contains(name)) {
					return child;
				}
			}
			return null;
		}

		public bool Match(string name) {
			if (this.BaseName == name) {
				return true;
			}
			return this.SubNames.Contains(name);
		}

		/// <summary>
		/// リストデータからツリーを構築します。
		/// </summary>
		/// <param name="boneMapList">データ</param>
		/// <returns>構築されたツリー</returns>
		public static BoneTreeItem Import(BoneMapListItem[] boneMapList) {
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
			};
			list.Add(item);
			for (int i = 0; i < this.Childs.Count; i++) {
				this.Childs[i].Export(item.Path, list, i);
			}
		}

	}
}
