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

		#region Modules Parameter

		[SerializeField] private VRCAvatarBuilderModuleBase[] _modules;

		public VRCAvatarBuilderModuleBase[] Modules {
			get => this._modules;
			set {
				if (this._modules == value) return;
				this.BeginUpdate();
				this._modules = value;
				this.Update();
			}
		}

		public static string ModulesPropertyName => nameof(_modules);


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

		[SerializeField] private Motion[] _leftGestureAnimations = new Motion[8];

		public Motion[] LeftGestureAnimations {
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

		[SerializeField] private Motion[] _rightGestureAnimations = new Motion[8];

		public Motion[] RightGestureAnimations {
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

		#region FxIdleAAnimation Parameter

		[SerializeField] private Motion _fxIdleAAnimation;

		public Motion FxIdleAAnimation {
			get => this._fxIdleAAnimation;
			set {
				if (this._fxIdleAAnimation == value) return;
				this.BeginUpdate();
				this._fxIdleAAnimation = value;
				this.Update();
			}
		}

		public static string FxIdleAAnimationPropertyName => nameof(_fxIdleAAnimation);

		#endregion

		#region LeftExpressionAnimations Parameter

		[SerializeField] private Motion[] _leftExpressionAnimations = new Motion[8];

		public Motion[] LeftExpressionAnimations {
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

		[SerializeField] private Motion[] _rightExpressionAnimations = new Motion[8];

		public Motion[] RightExpressionAnimations {
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