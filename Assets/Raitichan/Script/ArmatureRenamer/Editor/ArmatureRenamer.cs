using System;
using UnityEditor;
using UnityEngine;

namespace Raitichan.Script.ArmatureRenamer.Editor {
public class ArmatureRenamer : EditorWindow {
	[MenuItem("Raitichan/ArmatureRenamer")]
	private static void ShowWindow() {
		ArmatureRenamer window = GetWindow<ArmatureRenamer>();
		window.titleContent = new GUIContent("ArmatureRenamer");
		window.Show();
	}

	private GameObject _root;
	private string _addName = "";

	private void OnGUI() {
		GUILayout.Label("ArmatureRenamer");
		this._root = EditorGUILayout.ObjectField("Root", this._root, typeof(GameObject), true) as GameObject;
		this._addName = EditorGUILayout.TextField("Add Name", this._addName);
		
		EditorGUI.BeginDisabledGroup(_root == null);
		if (!GUILayout.Button("Execute")) return;
		if (_root == null) return;
		foreach (Transform transform in this._root.GetComponentsInChildren<Transform>()) {
			Undo.RecordObject(transform.gameObject, "rename");
			transform.gameObject.name += _addName;
		}

	}
}
}