using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Raitichan.Script.VRCAvatarBuilder.DataBase {
	public class HumanoidBoneSubNameTable : ScriptableObject {
#if UNITY_EDITOR
		
		[Serializable]
		public struct HumanoidBoneSubNameElement {
			public string Name;
			public int OriginalNameID;
		}
		
		[SerializeField] private List<HumanoidBoneSubNameElement> _data;

		private List<HumanoidBoneSubNameElement> Data =>
			this._data ?? (this._data = new List<HumanoidBoneSubNameElement>());

		public void Add(HumanoidBoneSubNameElement element) {
			if (this.Data.Contains(element)) return;
			this.Data.Add(element);
			EditorUtility.SetDirty(this);
		}

		public IEnumerable<HumanoidBoneSubNameElement> FindByBoneIndex(int index) {
			return this._data.Where(element => element.OriginalNameID == index);
		}
#endif
	}
}