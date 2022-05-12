using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace Raitichan.Script.VRCAvatarBuilder.DataBase {
	[Serializable]
	public class HumanoidBoneTable : ScriptableObject {
#if UNITY_EDITOR

		[Serializable]
		public struct HumanoidBoneElement {
			public string Name;
			public int Id;
			public int ParentHumanoidBoneIndex;
			public bool IsOptionalBone;
		}

		[SerializeField] private HumanoidBoneElement[] _data;

		public HumanoidBoneElement this[int index] => this._data[index];

		public int Length => this._data.Length;

		public IEnumerable<HumanoidBoneElement> FindChildBones(int index) {
			return this._data.Where(element => {
				do {
					if (element.ParentHumanoidBoneIndex == index) return true;
					if (element.ParentHumanoidBoneIndex == -1) return false;
					element = this._data[element.ParentHumanoidBoneIndex];
				} while (element.IsOptionalBone);
				return false;
			});
		}

		public void Init() {
			this._data = new HumanoidBoneElement[Enum.GetNames(typeof(HumanBodyBones)).Length + 1];
			foreach (HumanBodyBones humanBodyBones in Enum.GetValues(typeof(HumanBodyBones)).Cast<HumanBodyBones>()) {
				int parentIndex;
				bool isOptionalBone = false;
				switch (humanBodyBones) {
					case HumanBodyBones.Hips:
						parentIndex = ((int)HumanBodyBones.LastBone) + 1;
						break;
					case HumanBodyBones.LeftUpperLeg:
					case HumanBodyBones.RightUpperLeg:
						parentIndex = (int)HumanBodyBones.Hips;
						break;
					case HumanBodyBones.LeftLowerLeg:
						parentIndex = (int)HumanBodyBones.LeftUpperLeg;
						break;
					case HumanBodyBones.RightLowerLeg:
						parentIndex = (int)HumanBodyBones.RightUpperLeg;
						break;
					case HumanBodyBones.LeftFoot:
						parentIndex = (int)HumanBodyBones.LeftLowerLeg;
						break;
					case HumanBodyBones.RightFoot:
						parentIndex = (int)HumanBodyBones.RightLowerLeg;
						break;
					case HumanBodyBones.Spine:
						parentIndex = (int)HumanBodyBones.Hips;
						break;
					case HumanBodyBones.Chest:
						parentIndex = (int)HumanBodyBones.Spine;
						isOptionalBone = true;
						break;
					case HumanBodyBones.UpperChest:
						parentIndex = (int)HumanBodyBones.Chest;
						isOptionalBone = true;
						break;
					case HumanBodyBones.Neck:
						parentIndex = (int)HumanBodyBones.UpperChest;
						isOptionalBone = true;
						break;
					case HumanBodyBones.Head:
						parentIndex = (int)HumanBodyBones.Neck;
						break;
					case HumanBodyBones.LeftShoulder:
					case HumanBodyBones.RightShoulder:
						parentIndex = (int)HumanBodyBones.UpperChest;
						isOptionalBone = true;
						break;
					case HumanBodyBones.LeftUpperArm:
						parentIndex = (int)HumanBodyBones.LeftShoulder;
						break;
					case HumanBodyBones.RightUpperArm:
						parentIndex = (int)HumanBodyBones.RightShoulder;
						break;
					case HumanBodyBones.LeftLowerArm:
						parentIndex = (int)HumanBodyBones.LeftUpperArm;
						break;
					case HumanBodyBones.RightLowerArm:
						parentIndex = (int)HumanBodyBones.RightUpperArm;
						break;
					case HumanBodyBones.LeftHand:
						parentIndex = (int)HumanBodyBones.LeftLowerArm;
						break;
					case HumanBodyBones.RightHand:
						parentIndex = (int)HumanBodyBones.RightLowerArm;
						break;
					case HumanBodyBones.LeftToes:
						parentIndex = (int)HumanBodyBones.LeftFoot;
						isOptionalBone = true;
						break;
					case HumanBodyBones.RightToes:
						parentIndex = (int)HumanBodyBones.RightFoot;
						isOptionalBone = true;
						break;
					case HumanBodyBones.LeftEye:
					case HumanBodyBones.RightEye:
					case HumanBodyBones.Jaw:
						parentIndex = (int)HumanBodyBones.Head;
						isOptionalBone = true;
						break;
					case HumanBodyBones.LeftThumbProximal:
						parentIndex = (int)HumanBodyBones.LeftHand;
						isOptionalBone = true;
						break;
					case HumanBodyBones.LeftThumbIntermediate:
						parentIndex = (int)HumanBodyBones.LeftThumbProximal;
						isOptionalBone = true;
						break;
					case HumanBodyBones.LeftThumbDistal:
						parentIndex = (int)HumanBodyBones.LeftThumbIntermediate;
						isOptionalBone = true;
						break;
					case HumanBodyBones.LeftIndexProximal:
						parentIndex = (int)HumanBodyBones.LeftHand;
						isOptionalBone = true;
						break;
					case HumanBodyBones.LeftIndexIntermediate:
						parentIndex = (int)HumanBodyBones.LeftIndexProximal;
						isOptionalBone = true;
						break;
					case HumanBodyBones.LeftIndexDistal:
						parentIndex = (int)HumanBodyBones.LeftIndexIntermediate;
						isOptionalBone = true;
						break;
					case HumanBodyBones.LeftMiddleProximal:
						parentIndex = (int)HumanBodyBones.LeftHand;
						isOptionalBone = true;
						break;
					case HumanBodyBones.LeftMiddleIntermediate:
						parentIndex = (int)HumanBodyBones.LeftMiddleProximal;
						isOptionalBone = true;
						break;
					case HumanBodyBones.LeftMiddleDistal:
						parentIndex = (int)HumanBodyBones.LeftMiddleIntermediate;
						isOptionalBone = true;
						break;
					case HumanBodyBones.LeftRingProximal:
						parentIndex = (int)HumanBodyBones.LeftHand;
						isOptionalBone = true;
						break;
					case HumanBodyBones.LeftRingIntermediate:
						parentIndex = (int)HumanBodyBones.LeftRingProximal;
						isOptionalBone = true;
						break;
					case HumanBodyBones.LeftRingDistal:
						parentIndex = (int)HumanBodyBones.LeftRingIntermediate;
						isOptionalBone = true;
						break;
					case HumanBodyBones.LeftLittleProximal:
						parentIndex = (int)HumanBodyBones.LeftHand;
						isOptionalBone = true;
						break;
					case HumanBodyBones.LeftLittleIntermediate:
						parentIndex = (int)HumanBodyBones.LeftLittleProximal;
						isOptionalBone = true;
						break;
					case HumanBodyBones.LeftLittleDistal:
						parentIndex = (int)HumanBodyBones.LeftLittleIntermediate;
						isOptionalBone = true;
						break;
					case HumanBodyBones.RightThumbProximal:
						parentIndex = (int)HumanBodyBones.RightHand;
						isOptionalBone = true;
						break;
					case HumanBodyBones.RightThumbIntermediate:
						parentIndex = (int)HumanBodyBones.RightThumbProximal;
						isOptionalBone = true;
						break;
					case HumanBodyBones.RightThumbDistal:
						parentIndex = (int)HumanBodyBones.RightThumbIntermediate;
						isOptionalBone = true;
						break;
					case HumanBodyBones.RightIndexProximal:
						parentIndex = (int)HumanBodyBones.RightHand;
						isOptionalBone = true;
						break;
					case HumanBodyBones.RightIndexIntermediate:
						parentIndex = (int)HumanBodyBones.RightIndexProximal;
						isOptionalBone = true;
						break;
					case HumanBodyBones.RightIndexDistal:
						parentIndex = (int)HumanBodyBones.RightIndexIntermediate;
						isOptionalBone = true;
						break;
					case HumanBodyBones.RightMiddleProximal:
						parentIndex = (int)HumanBodyBones.RightHand;
						isOptionalBone = true;
						break;
					case HumanBodyBones.RightMiddleIntermediate:
						parentIndex = (int)HumanBodyBones.RightMiddleProximal;
						isOptionalBone = true;
						break;
					case HumanBodyBones.RightMiddleDistal:
						parentIndex = (int)HumanBodyBones.RightMiddleIntermediate;
						isOptionalBone = true;
						break;
					case HumanBodyBones.RightRingProximal:
						parentIndex = (int)HumanBodyBones.RightHand;
						isOptionalBone = true;
						break;
					case HumanBodyBones.RightRingIntermediate:
						parentIndex = (int)HumanBodyBones.RightRingProximal;
						isOptionalBone = true;
						break;
					case HumanBodyBones.RightRingDistal:
						parentIndex = (int)HumanBodyBones.RightRingIntermediate;
						isOptionalBone = true;
						break;
					case HumanBodyBones.RightLittleProximal:
						parentIndex = (int)HumanBodyBones.RightHand;
						isOptionalBone = true;
						break;
					case HumanBodyBones.RightLittleIntermediate:
						parentIndex = (int)HumanBodyBones.RightLittleProximal;
						isOptionalBone = true;
						break;
					case HumanBodyBones.RightLittleDistal:
						parentIndex = (int)HumanBodyBones.RightLittleIntermediate;
						isOptionalBone = true;
						break;
					case HumanBodyBones.LastBone:
						parentIndex = -1;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				int index = (int)humanBodyBones;
				this._data[(int)humanBodyBones] = new HumanoidBoneElement {
					Name = humanBodyBones.ToString(),
					Id = index,
					ParentHumanoidBoneIndex = parentIndex,
					IsOptionalBone = isOptionalBone
				};
			}

			int lastIndex = this._data.Length - 1;
			this._data[lastIndex] = new HumanoidBoneElement {
				Name = "Armature",
				Id = lastIndex,
				ParentHumanoidBoneIndex = -1,
				IsOptionalBone = false
			};
		}
#endif
	}
}