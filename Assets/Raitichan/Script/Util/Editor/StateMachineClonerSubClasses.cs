using UnityEditor.Animations;
using UnityEngine;


namespace Raitichan.Script.Util.Editor {
	public partial class StateMachineCloner {
		private struct ChildStateMachineCloner {
			public readonly StateMachineCloner Cloner;
			public readonly Vector3 Position;

			public ChildStateMachineCloner(StateMachineCloner cloner, Vector3 position) {
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