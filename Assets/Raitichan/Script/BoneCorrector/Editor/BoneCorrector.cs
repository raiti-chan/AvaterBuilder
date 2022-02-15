using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Raitichan.Script.BoneCorrector.Editor {
	public class BoneCorrector : EditorWindow {

		[MenuItem("Raitichan/BoneCorrector")]
		private static void ShowWindow() {
			BoneCorrector window = GetWindow<BoneCorrector>();
			window.titleContent = new GUIContent("Bone Corrector");
			window.Show();
		}

		private SkinnedMeshRenderer _srcMeshRenderer;
		private SkinnedMeshRenderer _dstMeshRenderer;
		private SkinnedMeshRenderer _targetMeshRenderer;

		private Vector2 _scrollPos;


		private void OnGUI() {

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.ExpandHeight(true));
				this._srcMeshRenderer = EditorGUILayout.ObjectField("Src", this._srcMeshRenderer, typeof(SkinnedMeshRenderer), false) as SkinnedMeshRenderer;
				if (this._srcMeshRenderer != null) {
					GUILayout.Label("Bone List");
					this._scrollPos = EditorGUILayout.BeginScrollView(this._scrollPos);
					foreach (var (bone, i) in this._srcMeshRenderer.bones.Select((bone, i) => (bone, i))) {
						EditorGUILayout.ObjectField($"{i} : {bone.name}", bone, typeof(Transform), true);
					}
					EditorGUILayout.EndScrollView();
				}
				EditorGUILayout.EndVertical();
			}
			{
				EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.ExpandHeight(true));
				this._dstMeshRenderer = EditorGUILayout.ObjectField("Dst", this._dstMeshRenderer, typeof(SkinnedMeshRenderer), false) as SkinnedMeshRenderer;
				if (this._dstMeshRenderer != null) {
					GUILayout.Label("Bone List");
					this._scrollPos = EditorGUILayout.BeginScrollView(this._scrollPos);
					foreach (var (bone, i) in this._dstMeshRenderer.bones.Select((bone, i) => (bone, i))) {
						EditorGUILayout.ObjectField($"{i} : {bone.name}", bone, typeof(Transform), true);
					}
					EditorGUILayout.EndScrollView();
				}
				EditorGUILayout.EndVertical();
			}
			{
				EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.ExpandHeight(true), GUILayout.MinWidth(300.0f));
				this._targetMeshRenderer = EditorGUILayout.ObjectField("Target", this._targetMeshRenderer, typeof(SkinnedMeshRenderer), true) as SkinnedMeshRenderer;
				if (this._targetMeshRenderer == null) {
					GUILayout.Label("ターゲットとなるSkinnedMeshRendererを上の項目に設定してください。");
				} else if (this._srcMeshRenderer == null) {
					GUILayout.Label("アバターのもともとのメッシュをSrcに指定してください。");
				} else if (this._dstMeshRenderer == null) {
					GUILayout.Label("変更後のメッシュをDstに指定してください。");
				} else {
					string[] srcBoneNames = this._srcMeshRenderer.bones
						.Select(bone => bone.name)
						.ToArray();
					string[] dstBoneNames = this._dstMeshRenderer.bones
						.Select(bone => bone.name)
						.ToArray();

					bool boneNameMatching = dstBoneNames.All(name => Array.IndexOf(srcBoneNames, name) != -1);
					GUILayout.Label($"ボーン名の完全一致 : {boneNameMatching}");

					bool boneIndexMatching = dstBoneNames.Zip(srcBoneNames, (a, b) => a == b).All(a => a);
					GUILayout.Label($"ボーンインデックスの一致 : {boneIndexMatching}");
					GUILayout.Label("状況に応じて、下のどちらかのボタンを1度押してください。\n基本的に下のボタンで大丈夫です。");
					EditorGUI.BeginDisabledGroup(boneIndexMatching);
					if (GUILayout.Button("メッシュ側ボーンインデックスの修正")) {
						bool result = EditorUtility.DisplayDialog("警告", "この操作により、FBX内のメッシュデータが直接変更されます。\nよろしいですか。", "はい", "いいえ");
						if (result) {
							int[] transferBoneIndexMap = this.GenerateTransferBoneIndexMap(srcBoneNames, dstBoneNames);
							this.FixedMesh(transferBoneIndexMap);
						}
					}

					if (GUILayout.Button("SkinnedMeshRenderer側ボーンインデックスの修正")) {
						int[] transferBoneIndexMap = this.GenerateTransferBoneIndexMap(srcBoneNames, dstBoneNames);
						this.FixedSkinnedMeshRenderer(transferBoneIndexMap);
					}
					EditorGUI.EndDisabledGroup();
				}
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndHorizontal();

		}

		private int[] GenerateTransferBoneIndexMap(string[] srcBoneNames, string[] dstBoneNames) {
			return dstBoneNames
				.Select(name => Array.IndexOf(srcBoneNames, name))
				.ToArray();
		}

		private void FixedMesh(int[] transferBoneIndexMap) {
			BoneWeight[] boneWeights = this._dstMeshRenderer.sharedMesh.boneWeights
				.Select(boneWeight => this.FixedBoneWeight(boneWeight, transferBoneIndexMap))
				.ToArray();
			this._targetMeshRenderer.sharedMesh.boneWeights = boneWeights;

			Matrix4x4[] bindposes = this._dstMeshRenderer.sharedMesh.bindposes
				.Select((m, i) => this._srcMeshRenderer.sharedMesh.bindposes[i])
				.ToArray();
			this._targetMeshRenderer.sharedMesh.bindposes = bindposes;
		}

		private BoneWeight FixedBoneWeight(BoneWeight boneWeight, int[] transferBoneIndexMap) {

			boneWeight.boneIndex0 = transferBoneIndexMap[boneWeight.boneIndex0];
			boneWeight.boneIndex1 = transferBoneIndexMap[boneWeight.boneIndex1];
			boneWeight.boneIndex2 = transferBoneIndexMap[boneWeight.boneIndex2];
			boneWeight.boneIndex3 = transferBoneIndexMap[boneWeight.boneIndex3];

			return boneWeight;
		}

		private void FixedSkinnedMeshRenderer(int[] transferBoneIndexMap) {
			Transform[] bones = this._targetMeshRenderer.bones
				.Select((bone, i) => this._targetMeshRenderer.bones[transferBoneIndexMap[i]])
				.ToArray();
			this._targetMeshRenderer.bones = bones;
		}
	}
}
