#if UNITY_EDITOR

using UnityEditor.Animations;
using UnityEngine;

namespace Raitichan.Script.Util {
	public partial class StateMachineCloner {
		private struct ChildStateMachineCloner {
			public readonly Util.StateMachineCloner Cloner;
			public readonly Vector3 Position;

			public ChildStateMachineCloner(Util.StateMachineCloner cloner, Vector3 position) {
				Cloner = cloner;
				Position = position;
			}
		}

		private struct ClonedTransitionInfo {
			public readonly AnimatorTransitionBase Transition;
			public readonly bool HasStateMachine;
			public readonly int DestinationInstanceId;

			public ClonedTransitionInfo(AnimatorTransitionBase transition, bool hasStateMachine,
				int destinationInstanceId) {
				Transition = transition;
				HasStateMachine = hasStateMachine;
				DestinationInstanceId = destinationInstanceId;
			}
		}

		private struct ClonedStateMachineInfo {
			public readonly AnimatorStateMachine StateMachine;
			public readonly int DefaultStateInstanceId;
			public readonly bool DefaultStateIsNull;

			public ClonedStateMachineInfo(AnimatorStateMachine stateMachine, int defaultStateInstanceId,
				bool defaultStateIsNull) {
				StateMachine = stateMachine;
				DefaultStateInstanceId = defaultStateInstanceId;
				DefaultStateIsNull = defaultStateIsNull;
			}
		}
	}
}
#endif
