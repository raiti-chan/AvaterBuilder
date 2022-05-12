using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Raitichan.Script.VRCAvatarBuilder.DataBase {
	public class ImportedAvatarTable : ScriptableObject {
#if UNITY_EDITOR
		[SerializeField] private List<Avatar> _data;

		private List<Avatar> Data => this._data ?? (this._data = new List<Avatar>());

		public bool Contains(Avatar avatar) {
			return this.Data.Contains(avatar);
		}

		public void Add(Avatar avatar) {
			if (this.Data.Contains(avatar)) return;
			this.Data.Add(avatar);
			EditorUtility.SetDirty(this);
		}
#endif
	}
}