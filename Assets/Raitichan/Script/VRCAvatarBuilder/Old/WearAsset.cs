using UnityEngine;

namespace Assets.Raitichan.Script.VRCAvatarBuilder {
	public class WearAsset : MonoBehaviour {

		/// <summary>
		/// ヒューマノイドボーンのボーン数(配列サイズ)
		/// </summary>
		public const int BodyBoneSize = (int)HumanBodyBones.UpperChest;

		/// <summary>
		/// 初期セットアップ済みか
		/// </summary>
		[SerializeField]
		private bool _isInit;
		public string IsInitPropertyName => nameof(this._isInit);
		public bool IsInit {
			get => this._isInit;
			set => this._isInit = value;
		}


		/// <summary>
		/// ヒューマノイドボーンに対応するボーン配列
		/// </summary>
		[SerializeField]
		private Transform[] _bodyBones;
		public string BodyBonesPropertyName => nameof(this._bodyBones);
		public Transform[] BodyBones {
			get => this._bodyBones;
			set => this._bodyBones = value;
		}

		/// <summary>
		/// 追加ボーンのボーン配列。
		/// </summary>
		[SerializeField]
		private Transform[] _additionalBones;
		public string AdditionalBonesPropertyName => nameof(this._additionalBones);
		public Transform[] AdditionalBones {
			get => this._bodyBones;
			set => this._bodyBones = value;
		}

		/// <summary>
		/// 追加ボーンのアバター上のボーン名
		/// </summary>
		[SerializeField]
		private string[] _additionalBoneOriginalNames;
		public string AdditionalBoneOriginalNamesPropertyName => nameof(this._additionalBoneOriginalNames);
		public string[] AdditionalBoneOriginalNames {
			get => this._additionalBoneOriginalNames;
			set => this._additionalBoneOriginalNames = value;
		}

		/// <summary>
		/// アーマチュア
		/// </summary>
		[SerializeField]
		private Transform _armature;
		public string ArmaturePropertyName => nameof(this._armature);
		public Transform Armature {
			get => this._armature;
			set => this._armature = value;
		}

	}
}
