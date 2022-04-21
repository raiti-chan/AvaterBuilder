using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Raitichan.Script.Util.Editor {
// #define NO_HIDE_IN_HIERARCHY
	/// <summary>
	/// ステートマシンを複製するためのクラス。
	/// Create by Raitichan
	/// </summary>
	public partial class StateMachineCloner {
		#region Private Field

		/// <summary>
		/// 親のCloner サブステートマシンじゃない場合null
		/// </summary>
		private readonly StateMachineCloner _parent;

		/// <summary>
		/// クローン元ステートマシン
		/// </summary>
		private readonly AnimatorStateMachine _src;

		/// <summary>
		/// サブステートマシンのClonerと座標のペア
		/// </summary>
		private readonly ChildStateMachineCloner[] _child;

		/// <summary>
		/// サブステートマシン内の物を含む全てのクローン元のインスタンスidとクローンされたステートマシンのディクショナリ
		/// </summary>
		private readonly Dictionary<int, ClonedStateMachineInfo> _clonedStateMachines;

		/// <summary>
		/// サブステートマシン内の物を含む全てのクローンされたステートのリスト
		/// </summary>
		private readonly Dictionary<int, AnimatorState> _clonedStates;

		/// <summary>
		/// サブステートマシン内の物を含む全てのクローン元のインスタンスidとクローンされたトランシジョンのリスト
		/// </summary>
		private readonly List<ClonedTransitionInfo> _clonedTransitions;

		/// <summary>
		/// サブステートマシン内の物を含む全てのクローンされたビヘイビア
		/// </summary>
		private readonly List<StateMachineBehaviour> _clonedBehaviour;

		#endregion

		#region Private Property

		/// <summary>
		/// この<see cref="StateMachineCloner"/>がRootか否か
		/// </summary>
		private bool IsRootStateMachine => this._parent == null;

		#endregion


		#region Constructer

		/// <summary>
		/// <see cref="StateMachineCloner"/>を初期化します。
		/// parentがnullの場合Rootのステートマシンとして動作します。
		/// </summary>
		/// <param name="src">ソース</param>
		/// <param name="parent">親の<see cref="StateMachineCloner"/></param>
		private StateMachineCloner(AnimatorStateMachine src, StateMachineCloner parent) {
			this._src = src;
			this._parent = parent;
			this._child = new ChildStateMachineCloner[src.stateMachines.Length];
			for (int i = 0; i < _child.Length; i++) {
				this._child[i] = new ChildStateMachineCloner(
					new StateMachineCloner(src.stateMachines[i].stateMachine, this),
					src.stateMachines[i].position);
			}

			if (parent != null) return;
			// サブステートの場合
			this._clonedStateMachines = new Dictionary<int, ClonedStateMachineInfo>();
			this._clonedStates = new Dictionary<int, AnimatorState>();
			this._clonedTransitions = new List<ClonedTransitionInfo>();
			this._clonedBehaviour = new List<StateMachineBehaviour>();
		}

		/// <summary>
		/// ソースを指定して、<see cref="StateMachineCloner"/>を初期化します。
		/// </summary>
		/// <param name="src">ソース</param>
		public StateMachineCloner(AnimatorStateMachine src) : this(src, null) { }

		#endregion

		#region Public Method

		/// <summary>
		/// ステートマシンを複製します。
		/// </summary>
		/// <returns>複製されたステートマシン</returns>
		public AnimatorStateMachine CloneStateMachine() {
			ChildAnimatorStateMachine[] childStateMachines = this.CloneChildStateMachines();
			ChildAnimatorState[] childStates = this.CloneChildStates();
			AnimatorStateTransition[] anyStateTransitions = this.CloneStateTransitions(this._src.anyStateTransitions);
			AnimatorTransition[] entryTransitions = this.CloneTransitions(this._src.entryTransitions);
			StateMachineBehaviour[] behaviours = this.CloneBehaviours(this._src.behaviours);

			AnimatorStateMachine stateMachine = new AnimatorStateMachine {
#if !NO_HIDE_IN_HIERARCHY
				hideFlags = HideFlags.HideInHierarchy,
#endif
				name = this._src.name,
				states = childStates,
				stateMachines = childStateMachines,
				anyStateTransitions = anyStateTransitions,
				entryTransitions = entryTransitions,
				behaviours = behaviours,
				anyStatePosition = this._src.anyStatePosition,
				entryPosition = this._src.entryPosition,
				exitPosition = this._src.exitPosition,
				parentStateMachinePosition = this._src.parentStateMachinePosition,
			};

			bool defaultStateIsNull = this._src.defaultState == null;
			int defaultStateId = defaultStateIsNull ? -1 : this._src.defaultState.GetInstanceID();
			this.RegisterClonedStateMachine(this._src.GetInstanceID(),
				new ClonedStateMachineInfo(stateMachine, defaultStateId, defaultStateIsNull));

			this.CloneStateMachineTransitions(stateMachine);
			this.ReplaceDefaultState();
			this.ReplaceTransitionDestination();

			return stateMachine;
		}

		/// <summary>
		/// 複製されたステートマシン内の全要素を指定された<see cref="UnityEngine.Object"/>のサブアセットとして追加します。
		/// ここで保存しない場合Unityを閉じた際に全て失われます。
		/// </summary>
		/// <param name="dst">保存先</param>
		/// <returns>保存に失敗した場合false</returns>
		public bool SaveAsset(Object dst) {
			if (!this.IsRootStateMachine) return false;
			string path = AssetDatabase.GetAssetPath(dst);
			if (string.IsNullOrEmpty(path)) return false;
			foreach (ClonedStateMachineInfo animatorStateMachine in this._clonedStateMachines.Values) {
				AssetDatabase.AddObjectToAsset(animatorStateMachine.StateMachine, path);
			}

			foreach (AnimatorState animatorState in this._clonedStates.Values) {
				AssetDatabase.AddObjectToAsset(animatorState, path);
			}

			foreach (ClonedTransitionInfo animatorStateTransition in this._clonedTransitions) {
				AssetDatabase.AddObjectToAsset(animatorStateTransition.Transition, path);
			}

			foreach (StateMachineBehaviour behaviour in this._clonedBehaviour) {
				AssetDatabase.AddObjectToAsset(behaviour, path);
			}

			return true;
		}

		#endregion

		#region Private Method

		/// <summary>
		/// この複製機のソースのサブステートマシンを複製します。
		/// </summary>
		/// <returns>複製されたステートマシンの配列</returns>
		private ChildAnimatorStateMachine[] CloneChildStateMachines() {
			ChildAnimatorStateMachine[] childStateMachines = new ChildAnimatorStateMachine[this._child.Length];
			for (int i = 0; i < childStateMachines.Length; i++) {
				childStateMachines[i].position = this._child[i].Position;
				childStateMachines[i].stateMachine = this._child[i].Cloner.CloneStateMachine();
			}

			return childStateMachines;
		}

		/// <summary>
		/// この複製機のソースのステートを複製します。
		/// </summary>
		/// <returns>複製されたステートの配列</returns>
		private ChildAnimatorState[] CloneChildStates() {
			ChildAnimatorState[] childStates = new ChildAnimatorState[this._src.states.Length];
			for (int i = 0; i < childStates.Length; i++) {
				childStates[i].position = this._src.states[i].position;
				childStates[i].state = this.CloneState(this._src.states[i].state);
			}

			return childStates;
		}

		/// <summary>
		/// この複製機のソースのステートマシントランシジョンを複製します。
		/// </summary>
		/// <param name="dst">複製されたトランシジョンの追加先</param>
		private void CloneStateMachineTransitions(AnimatorStateMachine dst) {
			foreach (AnimatorStateMachine originAsm in this._src.stateMachines.Select(o => o.stateMachine)) {
				AnimatorTransition[] srcTransition = this._src.GetStateMachineTransitions(originAsm);
				if (srcTransition.Length <= 0) continue; // トランシジョンが無ければスキップ
				AnimatorTransition[] clonedTransition = this.CloneTransitions(srcTransition);
				AnimatorStateMachine clonedOrigin = this._clonedStateMachines[originAsm.GetInstanceID()].StateMachine;
				dst.SetStateMachineTransitions(clonedOrigin, clonedTransition);
			}
		}

		/// <summary>
		/// 複製されたステートマシンのデフォルトステートを複製されたステートに置き換えます。
		/// </summary>
		private void ReplaceDefaultState() {
			if (!this.IsRootStateMachine) return;
			foreach (ClonedStateMachineInfo animatorStateMachine in this._clonedStateMachines.Values.Where(
				         animatorStateMachine => !animatorStateMachine.DefaultStateIsNull)) {
				animatorStateMachine.StateMachine.defaultState =
					this._clonedStates[animatorStateMachine.DefaultStateInstanceId];
			}
		}

		/// <summary>
		/// 複製されたトランシジョンの宛先を複製されたステートか、ステートマシンに置き換えます。
		/// </summary>
		private void ReplaceTransitionDestination() {
			if (!this.IsRootStateMachine) return;
			foreach (ClonedTransitionInfo clonedStateTransition in this._clonedTransitions) {
				if (clonedStateTransition.HasStateMachine) {
					clonedStateTransition.Transition.destinationStateMachine =
						this._clonedStateMachines[clonedStateTransition.DestinationInstanceId].StateMachine;
				} else {
					clonedStateTransition.Transition.destinationState =
						this._clonedStates[clonedStateTransition.DestinationInstanceId];
				}
			}
		}


		#region Clone Method

		/// <summary>
		/// <see cref="AnimatorState"/>を複製し、複製したリストに登録します。
		/// </summary>
		/// <param name="src">ソース</param>
		/// <returns>複製された<see cref="AnimatorState"/></returns>
		private AnimatorState CloneState(AnimatorState src) {
			AnimatorStateTransition[] transitions = this.CloneStateTransitions(src.transitions);
			StateMachineBehaviour[] behaviours = this.CloneBehaviours(src.behaviours);

			AnimatorState dst = new AnimatorState {
#if !NO_HIDE_IN_HIERARCHY
				hideFlags = HideFlags.HideInHierarchy,
#endif
				name = src.name,
				speed = src.speed,
				cycleOffset = src.cycleOffset,
				transitions = transitions,
				behaviours = behaviours,
				iKOnFeet = src.iKOnFeet,
				writeDefaultValues = src.writeDefaultValues,
				mirror = src.mirror,
				speedParameterActive = src.speedParameterActive,
				mirrorParameterActive = src.mirrorParameterActive,
				cycleOffsetParameterActive = src.cycleOffsetParameterActive,
				timeParameterActive = src.timeParameterActive,
				motion = src.motion,
				tag = src.tag,
				speedParameter = src.speedParameter,
				mirrorParameter = src.mirrorParameter,
				timeParameter = src.timeParameter,
			};

			this.RegisterClonedState(src.GetInstanceID(), dst);
			return dst;
		}


		#region Behaviours

		/// <summary>
		/// <see cref="StateMachineBehaviour"/>のリストを複製し、複製したリストに登録します。
		/// </summary>
		/// <param name="src">ソース</param>
		/// <returns>複製された<see cref="StateMachineBehaviour"/>配列</returns>
		// ReSharper disable once SuggestBaseTypeForParameter
		private StateMachineBehaviour[] CloneBehaviours(StateMachineBehaviour[] src) {
			StateMachineBehaviour[] dst = new StateMachineBehaviour[src.Length];
			for (int i = 0; i < dst.Length; i++) {
				dst[i] = this.CloneBehaviour(src[i]);
			}

			return dst;
		}

		/// <summary>
		/// <see cref="StateMachineBehaviour"/>を複製し、複製したリストに登録します。
		/// </summary>
		/// <param name="src">ソース</param>
		/// <returns>複製されたStateMachineBehaviour</returns>
		private StateMachineBehaviour CloneBehaviour(StateMachineBehaviour src) {
			StateMachineBehaviour dst = Object.Instantiate(src);
			this.RegisterClonedBehaviour(dst);
#if !NO_HIDE_IN_HIERARCHY
			dst.hideFlags = HideFlags.HideInHierarchy;
#endif
			dst.name = src.name;
			return dst;
		}

		#endregion

		#region Transition

		/// <summary>
		/// <see cref="AnimatorStateTransition"/>のリストを複製し、複製したリストに登録します。
		/// </summary>
		/// <param name="src">ソース</param>
		/// <returns>複製された<see cref="AnimatorStateTransition"/>配列</returns>
		// ReSharper disable once SuggestBaseTypeForParameter
		private AnimatorStateTransition[] CloneStateTransitions(AnimatorStateTransition[] src) {
			AnimatorStateTransition[] cloned = new AnimatorStateTransition[src.Length];
			for (int i = 0; i < cloned.Length; i++) {
				cloned[i] = this.CloneStateTransition(src[i]);
			}

			return cloned;
		}

		/// <summary>
		/// <see cref="AnimatorStateTransition"/>を複製し、複製したリストに登録します。
		/// </summary>
		/// <param name="src">ソース</param>
		/// <returns>複製された<see cref="AnimatorStateTransition"/></returns>
		private AnimatorStateTransition CloneStateTransition(AnimatorStateTransition src) {
			AnimatorStateTransition dst = new AnimatorStateTransition {
				duration = src.duration,
				offset = src.offset,
				exitTime = src.exitTime,
				hasExitTime = src.hasExitTime,
				hasFixedDuration = src.hasExitTime,
				interruptionSource = src.interruptionSource,
				orderedInterruption = src.orderedInterruption,
				canTransitionToSelf = src.canTransitionToSelf
			};
			this.CloneTransitionBase(src, dst);
			return dst;
		}

		/// <summary>
		/// <see cref="AnimatorTransition"/>のリストを複製し、複製したリストに登録します。
		/// </summary>
		/// <param name="src">ソース</param>
		/// <returns>複製された<see cref="AnimatorTransition"/>配列</returns>
		// ReSharper disable once SuggestBaseTypeForParameter
		private AnimatorTransition[] CloneTransitions(AnimatorTransition[] src) {
			AnimatorTransition[] cloned = new AnimatorTransition[src.Length];
			for (int i = 0; i < cloned.Length; i++) {
				cloned[i] = this.CloneTransition(src[i]);
			}

			return cloned;
		}

		/// <summary>
		/// <see cref="AnimatorTransition"/>を複製し、複製したリストに登録します。
		/// </summary>
		/// <param name="src">ソース</param>
		/// <returns>複製された<see cref="AnimatorTransition"/></returns>
		private AnimatorTransition CloneTransition(AnimatorTransition src) {
			AnimatorTransition cloned = new AnimatorTransition();
			this.CloneTransitionBase(src, cloned);
			return cloned;
		}

		/// <summary>
		/// <see cref="AnimatorTransitionBase"/>をdstに複製し、複製したリストに登録します。
		/// </summary>
		/// <param name="src">複製元</param>
		/// <param name="dst">複製先</param>
		/// <typeparam name="T"></typeparam>
		private void CloneTransitionBase<T>(T src, T dst) where T : AnimatorTransitionBase {
			AnimatorCondition[] conditions = this.CloneAnimatorConditions(src.conditions);
#if !NO_HIDE_IN_HIERARCHY
			dst.hideFlags = HideFlags.HideInHierarchy;
#endif
			dst.name = src.name;
			dst.conditions = conditions;
			// dst.destinationState =  ,
			// dst.destinationStateMachine =  ,
			dst.solo = src.solo;
			dst.mute = src.mute;
			dst.isExit = src.isExit;

			if (src.destinationStateMachine != null) {
				this.RegisterClonedStateTransition(new ClonedTransitionInfo(dst, true,
					src.destinationStateMachine.GetInstanceID()));
			} else if (src.destinationState != null) {
				this.RegisterClonedStateTransition(new ClonedTransitionInfo(dst, false,
					src.destinationState.GetInstanceID()));
			}
		}

		/// <summary>
		/// <see cref="AnimatorCondition"/>のリストを複製します。
		/// </summary>
		/// <param name="src">ソース</param>
		/// <returns>複製された<see cref="AnimatorCondition"/>のリスト</returns>
		// ReSharper disable once MemberCanBeMadeStatic.Local
		private AnimatorCondition[] CloneAnimatorConditions(AnimatorCondition[] src) {
			if (src == null) throw new ArgumentNullException(nameof(src));
			if (src.Length <= 0) return null;
			AnimatorCondition[] cloned = new AnimatorCondition[src.Length];
			for (int i = 0; i < cloned.Length; i++) {
				cloned[i] = src[i];
			}

			return cloned;
		}

		#endregion

		#endregion

		#region Register Method

		/// <summary>
		/// クローンしたステートマシンを登録する。
		/// </summary>
		/// <param name="id">クローン元のインスタンスid</param>
		/// <param name="stateMachineInfo">複製されたステートマシンの情報</param>
		private void RegisterClonedStateMachine(int id, ClonedStateMachineInfo stateMachineInfo) {
			if (!this.IsRootStateMachine) {
				this._parent.RegisterClonedStateMachine(id, stateMachineInfo);
				return;
			}

			this._clonedStateMachines.Add(id, stateMachineInfo);
			Undo.RegisterCreatedObjectUndo(stateMachineInfo.StateMachine, "Clone StateMachine");
		}

		/// <summary>
		/// クローンしたステートを登録する。
		/// </summary>
		/// <param name="id">クローン元のインスタンスid</param>
		/// <param name="state"><see cref="AnimatorState"/></param>
		private void RegisterClonedState(int id, AnimatorState state) {
			if (!this.IsRootStateMachine) {
				this._parent.RegisterClonedState(id, state);
				return;
			}

			this._clonedStates.Add(id, state);
			Undo.RegisterCreatedObjectUndo(state, "Clone State");
		}

		/// <summary>
		/// クローンしたトランシジョンを登録する。
		/// </summary>
		/// <param name="clonedTransition">複製されたトランシジョン情報</param>
		private void RegisterClonedStateTransition(ClonedTransitionInfo clonedTransition) {
			if (!this.IsRootStateMachine) {
				this._parent.RegisterClonedStateTransition(clonedTransition);
				return;
			}

			this._clonedTransitions.Add(clonedTransition);
			Undo.RegisterCreatedObjectUndo(clonedTransition.Transition, "Clone Transition");
		}

		/// <summary>
		/// クローンしたビヘイビアを登録する。
		/// </summary>
		/// <param name="behaviour">複製した<see cref="StateMachineBehaviour"/></param>
		private void RegisterClonedBehaviour(StateMachineBehaviour behaviour) {
			if (!this.IsRootStateMachine) {
				this._parent.RegisterClonedBehaviour(behaviour);
				return;
			}

			this._clonedBehaviour.Add(behaviour);
			Undo.RegisterCreatedObjectUndo(behaviour, "Clone Behaviour");
		}

		#endregion

		#endregion
	}
}