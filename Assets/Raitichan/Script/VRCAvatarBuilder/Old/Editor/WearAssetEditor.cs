
using Assets.Raitichan.Script.BoneRemapper;
using System;
using System.Linq;
using System.Collections.Generic;
using Raitichan.Script.Util.Extension;
using UnityEditor;
using UnityEngine;

namespace Assets.Raitichan.Script.VRCAvatarBuilder.Editor {
	[CustomEditor(typeof(WearAsset))]
	public class WearAssetEditor : UnityEditor.Editor {

		private enum SetupStage {
			FindArmature,
			BindBodyBonesSetup,
			BindBodyBones,
			AddAdditionalBones,
			Finish,
		}

		private GUIStyle _richTextLabel;

		private WearAsset _target;
		private Transform[] _bodyBones;

		private bool _startSetup;
		private SetupStage _setupStage;

		private BoneNameProfile _boneNameProfile;
		private string _prefix;
		private string _suffix;
		private BoneMapper _mapper;

		public void OnEnable() {
			this._target = this.target as WearAsset;
			this._bodyBones = new Transform[WearAsset.BodyBoneSize];
			this._startSetup = false;
			this._setupStage = SetupStage.FindArmature;

			this._richTextLabel = new GUIStyle(EditorStyles.label) { richText = true };

		}

		public override void OnInspectorGUI() {
			this.serializedObject.Update();
			if (this._target.IsInit) {

			} else {
				if (this._startSetup) {
					this.Setup();
				} else if (GUILayout.Button("Setup")) {
					this._startSetup = true;
				}

			}

			EditorGUILayout.PropertyField(this.serializedObject.FindProperty(this._target.BodyBonesPropertyName));
			EditorGUILayout.PropertyField(this.serializedObject.FindProperty(this._target.AdditionalBonesPropertyName));
			EditorGUILayout.PropertyField(this.serializedObject.FindProperty(this._target.AdditionalBoneOriginalNamesPropertyName));


			this.serializedObject.ApplyModifiedProperties();

		}

		public void Setup() {
			switch (this._setupStage) {
				case SetupStage.FindArmature:
					this.FindArmature();
					break;
				case SetupStage.BindBodyBonesSetup:
					this.BindBodyBonesSetup();
					break;
				case SetupStage.BindBodyBones:
					this.BindBodyBones();
					break;
				case SetupStage.AddAdditionalBones:
					this.AddAdditionalBones();
					break;
			}
		}

		private void FindArmature() {
			GUILayout.Label("Find Armature");

			Transform armature = this._target.gameObject.transform.Find("Armature");
			if (armature == null) {
				armature = this._target.gameObject.transform.Find("armature");
			}

			if (armature == null) {
				// armatureが見つからない
				using (new GUILayout.HorizontalScope()) {
					GUILayout.Label(EditorGUIUtility.IconContent("d_console.warnicon.sml"), GUILayout.Width(20));
					GUILayout.Label("<color=red>Not found Armature.</color>", _richTextLabel);
				}

				EditorGUILayout.PropertyField(this.serializedObject.FindProperty(this._target.ArmaturePropertyName));
				using (new EditorGUI.DisabledGroupScope(this._target.Armature == null)) {
					if (!GUILayout.Button("Continue")) {
						return;
					}
				}
			} else {
				this.serializedObject.FindProperty(this._target.ArmaturePropertyName).objectReferenceValue = armature;
			}
			this._setupStage = SetupStage.BindBodyBonesSetup;
		}

		private void BindBodyBonesSetup() {
			GUILayout.Label("Bind Body Bones Setup");

			this._boneNameProfile = EditorGUILayout.ObjectField("Bone Name Profile", this._boneNameProfile, typeof(BoneNameProfile), false) as BoneNameProfile;
			this._prefix = EditorGUILayout.TextField("Prefix", this._prefix);
			this._suffix = EditorGUILayout.TextField("Prefix", this._suffix);

			using (new EditorGUI.DisabledGroupScope(this._boneNameProfile == null)) {
				if (GUILayout.Button("Continue")) {
					this._setupStage = SetupStage.BindBodyBones;
					this._mapper = new BoneMapper(this._boneNameProfile.BoneTree, this._target.Armature);
				}
			}
		}

		private void BindBodyBones() {
			GUILayout.Label("Bind Body Bones");


			GUILayout.Space(15);
			EditorGUILayout.LabelField("Target BoneName", this._mapper.CurrentBaseName);
			this._mapper.CurrentBone = EditorGUILayout.ObjectField("Bind Target", this._mapper.CurrentBone, typeof(Transform), true) as Transform;
			// TODO: 自動取得できない場合候補の表示
			using (new EditorGUI.DisabledGroupScope(this._mapper.CurrentBone == null)) {
				if (GUILayout.Button("Auto Mapping")) {
					// TODO: 自動設定が成功した場合そのチェックをスキップ
				}
				if (GUILayout.Button("Continue")) {
					int index = this._mapper.CurrentHumanBoneIndex;
					if (index != -1 && index < this._bodyBones.Length) {
						this._bodyBones[this._mapper.CurrentHumanBoneIndex] = this._mapper.CurrentBone;
					}
					do {
						this._mapper.Next();
					} while (this._mapper.CurrentItem.HumanBoneIndex == -1);
				}
			}
			using (new EditorGUI.DisabledGroupScope(this._mapper.CurrentBone != null)) {
				if (GUILayout.Button("Skip")) {
					do {
						this._mapper.Next();
					} while (this._mapper.CurrentItem.HumanBoneIndex == -1);
				}
				if (GUILayout.Button("Skip Childs")) {
					this._mapper.SkipChild();
					while (this._mapper.CurrentItem.HumanBoneIndex == -1) {
						this._mapper.Next();
					}
				}
			}

			if (GUILayout.Button("Completion")) {
				SerializedProperty bodyBoneArrayProperty = this.serializedObject.FindProperty(this._target.BodyBonesPropertyName);
				bodyBoneArrayProperty.ClearArray();
				for (int i = 0; i < this._bodyBones.Length; i++) {
					bodyBoneArrayProperty.InsertArrayElementAtIndex(i);
					SerializedProperty property = bodyBoneArrayProperty.GetArrayElementAtIndex(i);
					property.objectReferenceValue = this._bodyBones[i];
				}
				this._setupStage = SetupStage.AddAdditionalBones;
			}


			foreach ((string name, Transform bone) in Enum.GetNames(typeof(HumanBodyBones)).Zip(this._bodyBones, (name, bone) => (name, bone))) {
				EditorGUILayout.ObjectField(name, bone, typeof(Transform), true);
			}

		}

		private void AddAdditionalBones() {
			GUILayout.Label("Add Additional Bones");

			GUILayout.Space(15);
			EditorGUILayout.PropertyField(this.serializedObject.FindProperty(this._target.AdditionalBonesPropertyName));
			EditorGUILayout.PropertyField(this.serializedObject.FindProperty(this._target.AdditionalBoneOriginalNamesPropertyName));


		}



	}

	public class BoneMapper {

		private BoneTreeItem _currentItem;
		public BoneTreeItem CurrentItem {
			get => this._currentItem;
			set => this._currentItem = value;
		}
		private Transform _currentBone;
		public Transform CurrentBone {
			get => this._currentBone;
			set => this._currentBone = value;
		}

		public string CurrentBaseName => this._currentItem.BaseName;
		public int CurrentHumanBoneIndex => this._currentItem.HumanBoneIndex;

		private Stack<(BoneTreeItem, Transform)> _mappingStack;

		public BoneMapper(BoneTreeItem root, Transform armature) {
			this._mappingStack = new Stack<(BoneTreeItem, Transform)>();
			this._currentItem = root;
			this._currentBone = armature;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns>Is Finish</returns>
		public bool Next() {
			if (this.ToInside()) return false;
			return this.SkipChild();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns>Is Finish</returns>
		public bool SkipChild() {
			if (this.ToNext()) return false;

			while (this.ToOutside()) {
				if (this.ToNext()) return false;
			}
			return true;
		}

		private bool ToInside() {
			if (this._currentItem.Childs.Count <= 0) return false;
			if (this._currentBone != null &&  this._currentBone.childCount <= 0) return false;
			this._mappingStack.Push((this._currentItem, this._currentBone));
			this._currentItem = this._currentItem.Childs[0];
			if (this._currentBone == null) return true;
			this._currentBone = this._currentBone.Find(this._currentItem);
			return true;
		}

		private bool ToNext() {
			if (this._mappingStack.Count <= 0) return false;
			(BoneTreeItem parentItem, Transform parentBone) = this._mappingStack.Peek();
			BoneTreeItem nextItem = parentItem.Next(this._currentItem);
			if (nextItem == null) return false;
			this._currentItem = nextItem;
			if (parentBone == null) return true;
			this._currentBone = parentBone.Find(nextItem);
			return true;
		}

		private bool ToOutside() {
			if (this._mappingStack.Count <= 0) return false;
			(BoneTreeItem parentItem, Transform parentBone) = this._mappingStack.Pop();
			this._currentItem = parentItem;
			this._currentBone = parentBone;
			return true;
		}

	}

}
