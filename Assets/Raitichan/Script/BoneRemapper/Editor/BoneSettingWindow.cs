using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Raitichan.Script.BoneRemapper.Editor {
	public class BoneSettingWindow : EditorWindow {

		private BoneTreeItem _target;

		private BoneNameProfile _profile;

		[SerializeField]
		private string[] _subNames;

		public static void Open(BoneTreeItem item, BoneNameProfile profile) {
			BoneSettingWindow window = GetWindow<BoneSettingWindow>();
			window.titleContent = new GUIContent($"BoneSetting {item.BaseName}");
			window._target = item;
			window._profile = profile;
			window.Show();
		}

		private void OnGUI() {
			this._target.BaseName = EditorGUILayout.TextField("基本名", this._target.BaseName);
			this._subNames = this._target.SubNames.ToArray();

			SerializedObject serializedObject = new SerializedObject(this);
			serializedObject.Update();
			SerializedProperty subNames = serializedObject.FindProperty(nameof(this._subNames));

			EditorGUILayout.PropertyField(subNames, new GUIContent("別名リスト"));
			serializedObject.ApplyModifiedProperties();

			this._target.SubNames = new HashSet<string>(this._subNames);

			this._profile.BoneMapList = this._profile.BoneTree.Export();
			EditorUtility.SetDirty(this._profile);
		}
	}
}
