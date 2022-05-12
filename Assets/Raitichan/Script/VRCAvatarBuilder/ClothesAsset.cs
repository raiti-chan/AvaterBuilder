using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Raitichan.Script.VRCAvatarBuilder {
	public class ClothesAsset : MonoBehaviour {

		#region HumanoidBones Parameter

		public const int HUMAN_BODY_BONE_COUNT = (int)HumanBodyBones.LastBone + 2;
		public const int ARMATURE_BONE_INDEX = HUMAN_BODY_BONE_COUNT - 1;

		[SerializeField] private Transform[] _humanoidBones = new Transform[HUMAN_BODY_BONE_COUNT];

		public Transform[] HumanoidBones {
			get => this._humanoidBones;
			set {
				if (this._humanoidBones == value) return;
				this.BeginUpdate();
				this._humanoidBones = value;
				this.Update();
			}
		}

		public static string HumanoidBonesPropertyName => nameof(_humanoidBones);


		#endregion

		#region SkinnedMeshes Parameter

		[SerializeField] private SkinnedMeshRenderer[] _skinnedMeshes;

		public SkinnedMeshRenderer[] SkinnedMeshes {
			get => this._skinnedMeshes;
			set {
				if (this._skinnedMeshes == value) return;
				this.BeginUpdate();
				this._skinnedMeshes = value;
				this.Update();
			}
		}

		public static string SkinnedMeshesPropertyName => nameof(_skinnedMeshes);


		#endregion

		#region Prefix Parameter

		[SerializeField] private string _prefix;

		public string Prefix {
			get => this._prefix;
			set {
				if (this._prefix == value) return;
				this.BeginUpdate();
				this._prefix = value;
				this.Update();
			}
		}

		public static string PrefixPropertyName => nameof(_prefix);


		#endregion

		#region Suffix Parameter

		[SerializeField] private string _suffix;

		public string Suffix {
			get => this._suffix;
			set {
				if (this._suffix == value) return;
				this.BeginUpdate();
				this._suffix = value;
				this.Update();
			}
		}

		public static string SuffixPropertyName => nameof(_suffix);


		#endregion
		
		#region Private Method

		private void BeginUpdate() {
			Undo.RecordObject(this, "Change Property");
		}

		private void Update() {
			EditorUtility.SetDirty(this);
		}

		#endregion
		
	}
}