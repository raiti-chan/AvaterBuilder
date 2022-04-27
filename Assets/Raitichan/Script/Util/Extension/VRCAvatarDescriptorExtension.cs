#if UNITY_EDITOR
using System.ComponentModel;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using AnimLayerType = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor.AnimLayerType;
using CustomAnimLayer = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor.CustomAnimLayer;

namespace Raitichan.Script.Util.Extension {
	public static class VRCAvatarDescriptorExtension {
		/// <summary>
		/// レイヤーの所属するグループのタイプ
		/// </summary>
		public enum LayerGroupType {
			Base,
			Special,
		}

		/// <summary>
		/// レイヤータイプからレイヤグループを取得します。
		/// </summary>
		/// <param name="type">レイヤタイプ</param>
		/// <returns>レイヤグループ</returns>
		/// <exception cref="InvalidEnumArgumentException">指定されたタイプが無効な場合</exception>
		// ReSharper disable once MemberCanBePrivate.Global
		public static LayerGroupType GetLayerGroupType(this AnimLayerType type) {
			switch (type) {
				case AnimLayerType.Base:
				case AnimLayerType.Additive:
				case AnimLayerType.Gesture:
				case AnimLayerType.Action:
				case AnimLayerType.FX:
					return LayerGroupType.Base;
				case AnimLayerType.Sitting:
				case AnimLayerType.TPose:
				case AnimLayerType.IKPose:
					return LayerGroupType.Special;
				case AnimLayerType.Deprecated0:
				default:
					throw new InvalidEnumArgumentException(
						$"Invalid Animation LayerType : {nameof(type)} = {type}");
			}
		}


		/// <summary>
		/// 指定されたタイプのレイヤーを取得します。
		/// 設定されていない場合nullです。
		/// </summary>
		/// <param name="descriptor"></param>
		/// <param name="layerType"></param>
		/// <returns></returns>
		/// <exception cref="InvalidEnumArgumentException">指定されたタイプが無効な場合</exception>
		public static RuntimeAnimatorController GetLayer(this VRCAvatarDescriptor descriptor,
			AnimLayerType layerType) {
			CustomAnimLayer[] layers = layerType.GetLayerGroupType() == LayerGroupType.Base
				? descriptor.baseAnimationLayers
				: descriptor.specialAnimationLayers;

			return layers.Where(layer => layer.type == layerType)
				.Select(layer => layer.animatorController)
				.FirstOrDefault();
		}

		/// <summary>
		/// 指定されたタイプのレイヤーを設定します。
		/// nullを設定した場合デフォルトに設定します。
		/// </summary>
		/// <param name="descriptor"></param>
		/// <param name="type"></param>
		/// <param name="controller"></param>
		/// <exception cref="InvalidEnumArgumentException">指定されたタイプが無効な場合</exception>
		public static void SetLayer(this VRCAvatarDescriptor descriptor, AnimLayerType type,
			RuntimeAnimatorController controller) {
			SerializedObject serializedObject = new SerializedObject(descriptor);
			serializedObject.Update();

			SerializedProperty alProperty = serializedObject.FindProperty(
				type.GetLayerGroupType() == LayerGroupType.Base
					? nameof(VRCAvatarDescriptor.baseAnimationLayers)
					: nameof(VRCAvatarDescriptor.specialAnimationLayers));

			foreach (SerializedProperty alpElement in alProperty.GetEnumerable()) {
				SerializedProperty typeProperty =
					alpElement.FindPropertyRelative(nameof(CustomAnimLayer.type));
				if (typeProperty.enumValueIndex != (int)type) continue;

				SerializedProperty AcProperty =
					alpElement.FindPropertyRelative(nameof(CustomAnimLayer.animatorController));
				SerializedProperty isDefaultProperty =
					alpElement.FindPropertyRelative(nameof(CustomAnimLayer.isDefault));

				AcProperty.objectReferenceValue = controller;
				isDefaultProperty.boolValue = controller == null;
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}

#endif