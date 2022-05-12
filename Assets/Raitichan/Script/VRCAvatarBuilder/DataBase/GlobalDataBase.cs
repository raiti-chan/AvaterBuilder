using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using static Raitichan.Script.VRCAvatarBuilder.DataBase.HumanoidBoneSubNameTable;
#endif

namespace Raitichan.Script.VRCAvatarBuilder.DataBase {
	public class GlobalDataBase : ScriptableObject {
#if UNITY_EDITOR

		/// <summary>
		/// インポート済みアバター
		/// </summary>
		public ImportedAvatarTable ImportedAvatarTable;

		/// <summary>
		/// ヒューマノイドボーン名
		/// </summary>
		public HumanoidBoneTable humanoidBoneTable;

		/// <summary>
		/// ヒューマノイドボーンのサブネーム
		/// </summary>
		public HumanoidBoneSubNameTable HumanoidBoneSubNameTable;


		public void ImportAvatar(Avatar avatar) {
			if (!avatar.isHuman) {
				EditorUtility.DisplayDialog("Error", "Non-humanoid avatars were imported.", "OK");
				return;
			}

			for (int i = 0; i < avatar.humanDescription.human.Length; i++) {
				this.HumanoidBoneSubNameTable.Add(new HumanoidBoneSubNameElement {
					OriginalNameID = i,
					Name = avatar.humanDescription.human[i].boneName
				});
			}
			
			this.ImportedAvatarTable.Add(avatar);
		}

		#region Singleton

		private static GlobalDataBase _instance;

		public static GlobalDataBase Instance {
			get {
				if (_instance != null) return _instance;
				_instance = AssetDatabase.LoadAssetAtPath<GlobalDataBase>(ConstantPath.GLOBAL_DB_PATH);

				if (_instance == null) {
					_instance = CreateNewDB();
				}

				return _instance;
			}
		}

		private static GlobalDataBase CreateNewDB() {
			GlobalDataBase db = CreateInstance<GlobalDataBase>();
			AssetDatabase.CreateAsset(db, ConstantPath.GLOBAL_DB_PATH);

			db.InitImportedAvatarTable();
			db.InitHumanoidBoneNameTable();
			db.InitHumanoidBoneSubNameTable();

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			return db;
		}

		private void InitImportedAvatarTable() {
			this.ImportedAvatarTable = CreateInstance<ImportedAvatarTable>();
			this.ImportedAvatarTable.name = "ImportedAvatarTable";
			AssetDatabase.AddObjectToAsset(this.ImportedAvatarTable, this);
		}

		private void InitHumanoidBoneNameTable() {
			this.humanoidBoneTable = CreateInstance<HumanoidBoneTable>();
			this.humanoidBoneTable.name = "HumanoidBoneTable";
			this.humanoidBoneTable.Init();
			AssetDatabase.AddObjectToAsset(this.humanoidBoneTable, this);
		}

		private void InitHumanoidBoneSubNameTable() {
			this.HumanoidBoneSubNameTable = CreateInstance<HumanoidBoneSubNameTable>();
			this.HumanoidBoneSubNameTable.name = "HumanoidBoneSubNameTable";
			AssetDatabase.AddObjectToAsset(this.HumanoidBoneSubNameTable, this);

			for (int i = 0; i < this.humanoidBoneTable.Length; i++) {
				HumanoidBoneSubNameElement element = new HumanoidBoneSubNameElement {
					OriginalNameID = i,
					Name = this.humanoidBoneTable[i].Name
				};
				this.HumanoidBoneSubNameTable.Add(element);
			}
		}

		#endregion


#endif
	}
}