using System;
using Raitichan.Script.VRCAvatarBuilder.DataBase;
using UnityEditor;
using UnityEngine;
using static Raitichan.Script.VRCAvatarBuilder.DataBase.HumanoidBoneTable;

namespace Raitichan.Script.Utility.Editor {
	public class HumanoidBoneTableView : EditorWindow {
		
		[MenuItem("Raitichan/Test/HumanoidBoneTableView")]
		private static void ShowWindow() {
			HumanoidBoneTableView window = GetWindow<HumanoidBoneTableView>();
			window.Show();
		}

		private Vector2 _scroll;
		
		private void OnGUI() {
			GlobalDataBase dataBase = GlobalDataBase.Instance;
			this._scroll = GUILayout.BeginScrollView(this._scroll);
			DrawTree(dataBase.humanoidBoneTable, -1);
			GUILayout.EndScrollView();
		}

		private static void DrawTree(HumanoidBoneTable table, int index) {
			foreach (HumanoidBoneElement humanoidBoneElement in table.FindChildBones(index)) {
				EditorGUILayout.LabelField(humanoidBoneElement.Name);
				using (new EditorGUI.IndentLevelScope()) {
					DrawTree(table, humanoidBoneElement.Id);
				}
			}
		}
	}
}