using System;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Serialization;
using VRC.SDK3.Avatars.Components;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Raitichan.Script.VRCAvatarBuilder {
	/// <summary>
	/// アバタービルダーコンポーネント
	/// </summary>
	public class VRCAvatarBuilder : MonoBehaviour {
		public const string OUTPUT_DIRECTORY = "/out";
		public const string GENERATED_SUFFIX = "_Generated";

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

		#region EmptyAnimatorController Parameter

		[SerializeField] private AnimatorController _emptyAnimatorController;

		public AnimatorController EmptyAnimatorController {
			get => this._emptyAnimatorController;
			set {
				if (this._emptyAnimatorController == value) return;
				this.BeginUpdate();
				this._emptyAnimatorController = value;
				this.Update();
			}
		}

		public static string EmptyAnimatorControllerPropertyName => nameof(_emptyAnimatorController);


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

#if UNITY_EDITOR
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

#endif

		#endregion

		#region BaseLayers Parameter

		[SerializeField] private RuntimeAnimatorController[] _baseLayers;

		public RuntimeAnimatorController[] BaseLayers {
			get => this._baseLayers;
			set {
				if (this._baseLayers == value) return;
				this.BeginUpdate();
				this._baseLayers = value;
				this.Update();
			}
		}

		public static string BaseLayersPropertyName => nameof(_baseLayers);

		#endregion

		#region AdditiveLayers Parameter

		[SerializeField] private RuntimeAnimatorController[] _additiveLayers;

		public RuntimeAnimatorController[] AdditiveLayers {
			get => this._additiveLayers;
			set {
				if (this._additiveLayers == value) return;
				this.BeginUpdate();
				this._additiveLayers = value;
				this.Update();
			}
		}

		public static string AdditiveLayersPropertyName => nameof(_additiveLayers);

		#endregion

		#region GestureLayers Parameter

		[SerializeField] private RuntimeAnimatorController[] _gestureLayers;

		public RuntimeAnimatorController[] GestureLayers {
			get => this._gestureLayers;
			set {
				if (this._gestureLayers == value) return;
				this.BeginUpdate();
				this._gestureLayers = value;
				this.Update();
			}
		}

		public static string GestureLayersPropertyName => nameof(_gestureLayers);

		#endregion

		#region ActionLayers Parameter

		[SerializeField] private RuntimeAnimatorController[] _actionLayers;

		public RuntimeAnimatorController[] ActionLayers {
			get => this._actionLayers;
			set {
				if (this._actionLayers == value) return;
				this.BeginUpdate();
				this._actionLayers = value;
				this.Update();
			}
		}

		public static string ActionLayersPropertyName => nameof(_actionLayers);

		#endregion

		#region FxLayers Parameter

		[SerializeField] private RuntimeAnimatorController[] _fxLayers;

		public RuntimeAnimatorController[] FxLayers {
			get => this._fxLayers;
			set {
				if (this._fxLayers == value) return;
				this.BeginUpdate();
				this._fxLayers = value;
				this.Update();
			}
		}

		public static string FxLayersPropertyName => nameof(_fxLayers);

		#endregion

		#region LeftGestureAnimations Parameter

		[SerializeField] private AnimationClip[] _leftGestureAnimations = new AnimationClip[7];

		public AnimationClip[] LeftGestureAnimations {
			get => this._leftGestureAnimations;
			set {
				if (this._leftGestureAnimations == value) return;
				this.BeginUpdate();
				this._leftGestureAnimations = value;
				this.Update();
			}
		}

		public static string LeftGestureAnimationsPropertyName => nameof(_leftGestureAnimations);


		#endregion

		#region RightGestureAnimations Parameter

		[SerializeField] private AnimationClip[] _rightGestureAnimations = new AnimationClip[7];

		public AnimationClip[] RightGestureAnimations {
			get => this._rightGestureAnimations;
			set {
				if (this._rightGestureAnimations == value) return;
				this.BeginUpdate();
				this._rightGestureAnimations = value;
				this.Update();
			}
		}

		public static string RightGestureAnimationsPropertyName => nameof(_rightGestureAnimations);


		#endregion
		
		#region LeftExpressionAnimations Parameter

		[SerializeField] private AnimationClip[] _leftExpressionAnimations = new AnimationClip[7];

		public AnimationClip[] LeftExpressionAnimations {
			get => this._leftExpressionAnimations;
			set {
				if (this._leftExpressionAnimations == value) return;
				this.BeginUpdate();
				this._leftExpressionAnimations = value;
				this.Update();
			}
		}

		public static string LeftExpressionAnimationsPropertyName => nameof(_leftExpressionAnimations);


		#endregion

		#region RightExpressionAnimations Parameter

		[SerializeField] private AnimationClip[] _rightExpressionAnimations = new AnimationClip[7];

		public AnimationClip[] RightExpressionAnimations {
			get => this._rightExpressionAnimations;
			set {
				if (this._rightExpressionAnimations == value) return;
				this.BeginUpdate();
				this._rightExpressionAnimations = value;
				this.Update();
			}
		}

		public static string RightExpressionAnimationsPropertyName => nameof(_rightExpressionAnimations);


		#endregion
		
		// TODO: 複合アニメーションの対応
		
		#region Private Method

		private void BeginUpdate() {
#if UNITY_EDITOR
			Undo.RecordObject(this, "Change Property");
#endif
		}

		private void Update() {
#if UNITY_EDITOR
			Undo.RecordObject(this, "Change Property");
			EditorUtility.SetDirty(this);
#endif
		}

		#endregion
	}



	public enum GestureTypes {
		Fist,
		Open,
		Point,
		Peace,
		RockNRoll,
		Gun,
		ThumbsUp,
		Idle,
	}
}