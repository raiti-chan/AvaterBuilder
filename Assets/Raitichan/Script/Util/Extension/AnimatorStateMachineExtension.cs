using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;

#if UNITY_EDITOR

namespace Raitichan.Script.Util.Extension {
	public static class AnimatorStateMachineExtension {
		/// <summary>
		/// ステートマシンの全てのステートを取得します。
		/// </summary>
		/// <param name="stateMachine"></param>
		/// <returns></returns>
		public static IEnumerable<AnimatorState> GetAllState(this AnimatorStateMachine stateMachine) {
			IEnumerable<AnimatorState> state = stateMachine.states.Select(child => child.state);
			IEnumerable<AnimatorState> childState =
				stateMachine.stateMachines.SelectMany(child => child.stateMachine.GetAllState());
			return state.Concat(childState);
		}
	}
}


#endif