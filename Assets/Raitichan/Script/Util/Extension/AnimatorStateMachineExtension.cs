#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;

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

		public static IEnumerable<AnimatorTransitionBase>
			GetAllTransitionBases(this AnimatorStateMachine stateMachine) {
			IEnumerable<AnimatorTransitionBase> transition = stateMachine.states
				.SelectMany(state => state.state.transitions)
				.Concat<AnimatorTransitionBase>(stateMachine.entryTransitions)
				.Concat(stateMachine.anyStateTransitions)
				.Concat(stateMachine.stateMachines.SelectMany(child =>
					stateMachine.GetStateMachineTransitions(child.stateMachine)));

			IEnumerable<AnimatorTransitionBase> childTransition =
				stateMachine.stateMachines.SelectMany(child => child.stateMachine.GetAllTransitionBases());
			return transition.Concat(childTransition);
		}
	}
}


#endif