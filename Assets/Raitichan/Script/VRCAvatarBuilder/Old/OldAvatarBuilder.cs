using Assets.Raitichan.Script.BoneRemapper;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

#if UNITY_EDITOR
namespace Assets.Raitichan.Script.VRCAvatarBuilder {
	public class OldAvatarBuilder : MonoBehaviour {

		/// <summary>
		/// 初期化されているか
		/// </summary>
		[SerializeField]
		private bool _isInitialized = false;
		public string IsInitializedPropertyName => nameof(this._isInitialized);
		public bool IsInitialized {
			get => this._isInitialized;
			set => this._isInitialized = value;
		}

		/// <summary>
		/// 作業ディレクトリ
		/// </summary>
		[SerializeField]
		private string _workingDirectry = "";
		public string WorkingDirectryPropertyName => nameof(this._workingDirectry);
		public string WorkingDirectry {
			get => this._workingDirectry;
			set => this._workingDirectry = value;
		}

		/// <summary>
		/// シーン
		/// </summary>
		[SerializeField]
		private SceneAsset _scene;
		public string ScenePropertyName => nameof(this._scene);
		public SceneAsset Scene {
			get => this._scene;
			set => this._scene = value; 
		}

		/// <summary>
		/// ボーン名プロファイル
		/// </summary>
		[SerializeField]
		private BoneNameProfile _boneNameProfile;
		public string BoneNameProfilePropertyName => nameof(this._boneNameProfile);
		public BoneNameProfile BoneNameProfile {
			get => this._boneNameProfile;
			set => this._boneNameProfile = value;
		}

		/// <summary>
		/// アバター
		/// </summary>
		[SerializeField]
		private VRCAvatarDescriptor _avatar;
		public string AvatarPropertyName => nameof(this._avatar);
		public VRCAvatarDescriptor Avatar {
			get => this._avatar;
			set => this._avatar = value;
		}

		/// <summary>
		/// スケール
		/// </summary>
		[SerializeField]
		private float _scale = 1;
		public string ScalePropertyName => nameof(this._scale);
		public float Scale {
			get => this._scale;
			set {
				if (value <= 0) return;
				this._scale = value;
			}
		}

		/// <summary>
		/// 次のビルド時のアバターバージョン
		/// </summary>
		[SerializeField]
		private uint _nextBuildVersion = 0;
		public string NextBuildVersionPropertyName => nameof(this._nextBuildVersion);
		public uint NextBuildVersion {
			get => this._nextBuildVersion;
			set => this._nextBuildVersion = value;
		}

		[SerializeField]
		private GameObject _hairsObject;
		public string HairsObjectPropertyName => nameof(this._hairsObject);
		public GameObject HairsObject {
			get => this._hairsObject;
			set => this._hairsObject = value;
		}

		[SerializeField]
		private GameObject _wearsObject;
		public string WearsObjectPropertyName => nameof(this._wearsObject);
		public GameObject WearsObject {
			get => this._wearsObject;
			set => this._wearsObject = value;
		}

		[SerializeField]
		private GameObject _accessorysObject;
		public string AccessorysObjectPropertyName => nameof(this._accessorysObject);
		public GameObject AccessorysObject {
			get => this._accessorysObject;
			set => this._accessorysObject = value;
		}

		[SerializeField]
		private GameObject _objectsObject;
		public string ObjectsObjectPropertyName => nameof(this._objectsObject);
		public GameObject ObjectsObject {
			get => this._objectsObject;
			set => this._objectsObject = value;
		}

		[SerializeField]
		private GameObject _armature;
		public string ArmaturePropertyName => nameof(this._armature);
		public GameObject Armature {
			get => this._armature;
			set => this._armature = value;
		}
	}
}

#endif