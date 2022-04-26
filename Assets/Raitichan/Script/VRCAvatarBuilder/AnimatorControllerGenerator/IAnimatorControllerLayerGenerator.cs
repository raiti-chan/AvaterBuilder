#if UNITY_EDITOR
using UnityEditor.Animations;

namespace Raitichan.Script.VRCAvatarBuilder.AnimatorControllerGenerator {
	/// <summary>
	/// レイヤーを生成するクラスのインターフェイス
	/// </summary>
	public interface IAnimatorControllerLayerGenerator {
		/// <summary>
		/// 対象のコントローラーにレイヤーを生成します。
		/// </summary>
		/// <param name="controller"></param>
		void Generate(AnimatorController controller);
	}
}
#endif