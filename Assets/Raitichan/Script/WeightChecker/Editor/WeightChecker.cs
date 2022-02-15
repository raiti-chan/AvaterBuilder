using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Raitichan.Script.WeightChecker.Editor {
	public enum Mode {
		Tree,
		Not,
		Or,
	}

	public class WeightChecker : EditorWindow {
		[MenuItem("Raitichan/WeightChecker")]
		private static void ShowWindow() {
			WeightChecker window = GetWindow<WeightChecker>();
			window.titleContent = new GUIContent("WeightChecker");
			window.Show();
		}

		[SerializeField]
		private SkinnedMeshRenderer[] _skinnedMeshRenderers;

		private Mode _mode;

		private Transform _rootBone;

		private Vector2 scroll;

		private HashSet<Transform> _result;

		private void Awake() {
			SkinnedMeshRenderer[] skinnedMeshRenderers = Selection.GetFiltered<SkinnedMeshRenderer>(SelectionMode.Unfiltered);
			if (skinnedMeshRenderers.Length <= 0) {
				return;
			}
			this._skinnedMeshRenderers = skinnedMeshRenderers;
			this._rootBone = skinnedMeshRenderers[0].rootBone;
		}

		private void OnGUI() {

			SerializedObject so = new SerializedObject(this);
			EditorGUILayout.PropertyField(so.FindProperty(nameof(this._skinnedMeshRenderers)), true);
			so.ApplyModifiedProperties();

			Mode newMode = (Mode)EditorGUILayout.EnumPopup("Mode", this._mode);
			if (this._mode != newMode) {
				this._mode = newMode;
				this._result = null;
			}
			if (this._mode == Mode.Not || this._mode == Mode.Tree) {
				this._rootBone = EditorGUILayout.ObjectField("RootBone", this._rootBone, typeof(Transform), true) as Transform;
			}

			if (GUILayout.Button("Serche")) {
				switch (this._mode) {
					case Mode.Or:
						this.Or();
						break;
					case Mode.Not:
						this.Not();
						break;
					case Mode.Tree:
						this.Or();
						HashSet<Transform> boneList = this._result;
						this._result = new HashSet<Transform>();
						this.SearchChiled(this._rootBone, boneList);
						break;
				}
			}


			if (this._result != null) {
				EditorGUILayout.BeginVertical(GUI.skin.box);
				this.scroll = EditorGUILayout.BeginScrollView(this.scroll);
				foreach (Transform bone in this._result.Where(bone => bone != null)) {
					if (this._mode == Mode.Tree) {
						this.WriteObject(bone);
					} else {
						EditorGUILayout.ObjectField(bone.name, bone, typeof(Transform), true);
					}
				}
				EditorGUILayout.EndScrollView();
				EditorGUILayout.EndVertical();
			}
		}

		private void Or() {
			var bones = this._skinnedMeshRenderers
				.Where(mesh => mesh != null)
				.SelectMany(mesh => mesh.sharedMesh.boneWeights.Select(weight => (weight, mesh)))
				.SelectMany(t => new (Transform bone, float weight)[] { (t.mesh.bones[t.weight.boneIndex0], t.weight.weight0), (t.mesh.bones[t.weight.boneIndex1], t.weight.weight1), (t.mesh.bones[t.weight.boneIndex2], t.weight.weight2), (t.mesh.bones[t.weight.boneIndex3], t.weight.weight3) })
				.Where(t => t.weight != 0.0f)
				.Select(t => t.bone);

			this._result = new HashSet<Transform>(bones);
		}

		private void Not() {
			if (this._rootBone == null) return;
			this.Or();
			HashSet<Transform> weightedBoneSet = this._result;
			var bones = this._rootBone.gameObject.GetComponentsInChildren<Transform>();
			this._result = new HashSet<Transform>(bones);
			this._result.ExceptWith(weightedBoneSet);

		}

		private bool SearchChiled(Transform transform, HashSet<Transform> boneList) {
			bool hasWeight = boneList.Contains(transform);
			if (transform.childCount == 0) {
				return hasWeight;
			}

			if (hasWeight) {
				for (int i = 0; i < transform.childCount; i++) {
					Transform child = transform.GetChild(i);
					if (!this.SearchChiled(child, boneList)) {
						this._result.Add(child);
					};
				}
				return true;
			} else {
				bool hasChildWeight = false;
				for (int i = 0; i < transform.childCount; i++) {
					if (this.SearchChiled(transform.GetChild(i), boneList)) {
						hasChildWeight = true;
					}
				}
				return hasChildWeight;
			}
		}

		private void WriteObject(Transform transform) {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.ObjectField(transform, typeof(Transform), true);
			if (GUILayout.Button("Delete")) {
				Undo.DestroyObjectImmediate(transform.gameObject);
				return;
			}
			EditorGUILayout.EndHorizontal();
			EditorGUI.indentLevel++;
			for (int i = 0; i < transform.childCount; i++) {
				this.WriteObject(transform.GetChild(i));
			}
			EditorGUI.indentLevel--;
		}

	}
}