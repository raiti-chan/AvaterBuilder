using System;
using Raitichan.Script.VRCAvatarBuilder.Module;
using UnityEngine;
using UnityEngine.Serialization;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif

namespace Raitichan.Script.VRCAvatarBuilder {
	/// <summary>
	/// アバタービルダーコンポーネント
	/// </summary>
	[AddComponentMenu("Raitichan/VRCAvatarBuilder/VRCAvatarBuilder")]
	public class VRCAvatarBuilder : MonoBehaviour {
#if UNITY_EDITOR
		#region Language Parameter

		[FormerlySerializedAs("_language")] [SerializeField]
		private Strings.Languages languages;

		public Strings.Languages Languages {
			get => this.languages;
			set {
				if (this.languages == value) return;
				this.BeginUpdate();
				this.languages = value;
				this.Update();
			}
		}

		public static string LanguagePropertyName => nameof(languages);

		#endregion

		#region AvaterDescriptor Parameter

		[SerializeField] private VRCAvatarDescriptor _avatarDescriptor;

		public VRCAvatarDescriptor AvatarDescriptor {
			get => this._avatarDescriptor;
			set {
				if (this._avatarDescriptor == value) return;
				this.BeginUpdate();
				this._avatarDescriptor = value;
				this.Update();
			}
		}

		public static string AvatarDescriptorPropertyName => nameof(_avatarDescriptor);

		#endregion

		#region WorkingDirectoryPath Parameter

		[SerializeField] private string _workingDirectoryPath;

		public string WorkingDirectoryPath {
			get => this._workingDirectoryPath;
			set {
				if (this._workingDirectoryPath == value) return;
				this.BeginUpdate();
				this._workingDirectoryPath = value;
				this.Update();
			}
		}

		public static string WorkingDirectoryPathPropertyName => nameof(_workingDirectoryPath);

		#endregion

		#region Scale Parameter

		[SerializeField] private float _scale = 1.0f;

		public float Scale {
			get => this._scale;
			set {
				if (Math.Abs(this._scale - value) < 0.0000001) return;
				this.BeginUpdate();
				this._scale = value;
				this.Update();
			}
		}

		public static string ScalePropertyName => nameof(_scale);

		#endregion

		#region UploadScene Parameter

		[SerializeField] private SceneAsset _uploadScene;

		public SceneAsset UploadScene {
			get => this._uploadScene;
			set {
				if (this._uploadScene == value) return;
				this.BeginUpdate();
				this._uploadScene = value;
				this.Update();
			}
		}

		public static string UploadScenePropertyName => nameof(_uploadScene);

		#endregion

		#region ExpressionsMenu Parameter

		[SerializeField] private VRCExpressionsMenu _expressionsMenu;

		public VRCExpressionsMenu ExpressionsMenu {
			get => this._expressionsMenu;
			set {
				if (this._expressionsMenu == value) return;
				this.BeginUpdate();
				this._expressionsMenu = value;
				this.Update();
			}
		}

		public static string ExpressionsMenuPropertyName => nameof(_expressionsMenu);

		#endregion

		// TODO: 複合アニメーションの対応

		#region Private Method

		private void BeginUpdate() {
			Undo.RecordObject(this, "Change Property");
		}

		private void Update() {
			EditorUtility.SetDirty(this);
		}

		#endregion

#endif
	}
}