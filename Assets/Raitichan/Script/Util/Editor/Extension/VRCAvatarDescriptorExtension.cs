using System.Linq;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Security;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace Raitichan.Script.Util.Editor.Extension {
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
		/// <exception cref="InvalidParameterException">指定されたタイプが無効な場合</exception>
		// ReSharper disable once MemberCanBePrivate.Global
		public static LayerGroupType GetLayerGroupType(this VRCAvatarDescriptor.AnimLayerType type) {
			switch (type) {
				case VRCAvatarDescriptor.AnimLayerType.Base:
				case VRCAvatarDescriptor.AnimLayerType.Additive:
				case VRCAvatarDescriptor.AnimLayerType.Gesture:
				case VRCAvatarDescriptor.AnimLayerType.Action:
				case VRCAvatarDescriptor.AnimLayerType.FX:
					return LayerGroupType.Base;
				case VRCAvatarDescriptor.AnimLayerType.Sitting:
				case VRCAvatarDescriptor.AnimLayerType.TPose:
				case VRCAvatarDescriptor.AnimLayerType.IKPose:
					return LayerGroupType.Special;
				case VRCAvatarDescriptor.AnimLayerType.Deprecated0:
				default:
					throw new InvalidParameterException(
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
		/// <exception cref="InvalidParameterException">指定されたタイプが無効な場合</exception>
		public static RuntimeAnimatorController GetLayer(this VRCAvatarDescriptor descriptor,
			VRCAvatarDescriptor.AnimLayerType layerType) {
			VRCAvatarDescriptor.CustomAnimLayer[] layers = layerType.GetLayerGroupType() == LayerGroupType.Base
				? descriptor.baseAnimationLayers
				: descriptor.specialAnimationLayers;

			return layers.Where(layer => layer.type == layerType)
				.Select(layer => layer.animatorController)
				.FirstOrDefault();
		}


		public static void SetLayer(this VRCAvatarDescriptor descriptor, VRCAvatarDescriptor.AnimLayerType type,
			RuntimeAnimatorController controller) {
			SerializedObject serializedObject = new SerializedObject(descriptor);
			serializedObject.Update();

			SerializedProperty alProperty = serializedObject.FindProperty(
				type.GetLayerGroupType() == LayerGroupType.Base
					? nameof(VRCAvatarDescriptor.baseAnimationLayers)
					: nameof(VRCAvatarDescriptor.specialAnimationLayers));

			foreach (SerializedProperty alpElement in alProperty.GetEnumerable()) {
				SerializedProperty typeProperty =
					alpElement.FindPropertyRelative(nameof(VRCAvatarDescriptor.CustomAnimLayer.type));
				if (typeProperty.enumValueIndex != (int)type) continue;
				
				SerializedProperty AcProperty =
					alpElement.FindPropertyRelative(nameof(VRCAvatarDescriptor.CustomAnimLayer.animatorController));
				SerializedProperty isEnabledProperty =
					alpElement.FindPropertyRelative(nameof(VRCAvatarDescriptor.CustomAnimLayer.isEnabled));
				SerializedProperty isDefaultProperty =
					alpElement.FindPropertyRelative(nameof(VRCAvatarDescriptor.CustomAnimLayer.isDefault));
				
				AcProperty.objectReferenceValue = controller;
				isEnabledProperty.boolValue = true;
				isDefaultProperty.boolValue = false;
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}