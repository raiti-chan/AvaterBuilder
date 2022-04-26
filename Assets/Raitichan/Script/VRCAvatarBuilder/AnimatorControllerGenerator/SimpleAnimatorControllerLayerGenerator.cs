#if UNITY_EDITOR
using Raitichan.Script.Util.Extension;
using UnityEditor.Animations;

namespace Raitichan.Script.VRCAvatarBuilder.AnimatorControllerGenerator {
	/// <summary>
	/// 単一コントローラーを返すジェネレーター
	/// </summary>
	public class SimpleAnimatorControllerLayerGenerator : IAnimatorControllerLayerGenerator {
		
		/// <summary>
		/// 生成対象のコントローラー
		/// </summary>
		public AnimatorController Controller { get; set; }
		
		/// <summary>
		/// 生成
		/// </summary>
		/// <returns></returns>
		public void Generate(AnimatorController controller) {
			controller.AppendLayerAll(controller);
		}
	}
}
#endif