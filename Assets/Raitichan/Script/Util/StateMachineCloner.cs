#define NO_HIDE_IN_HIERARCHY // これをつけるとhierarchyに表示されるようになる。

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Raitichan.Script.Util {
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
		private readonly List<StateMachineBehaviour> _clonedBehaviours;

		/// <summary>
		/// サブステートマシン内の物を含む全てのクローンされたブレンドツリーのリスト
		/// </summary>
		private readonly List<BlendTree> _clonedBlendTrees;

		/// <summary>
		/// 使われているパラメータ名のリスト。
		/// </summary>
		private readonly HashSet<string> _usedParameterNames;

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
			this._clonedBehaviours = new List<StateMachineBehaviour>();
			this._clonedBlendTrees = new List<BlendTree>();
			this._usedParameterNames = new HashSet<string>();
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
		/// すでにアセットに追加されている物は無視されます。
		/// </summary>
		/// <param name="dst">保存先</param>
		/// <returns>保存に失敗した場合false</returns>
		public bool SaveAsset(Object dst) {
			if (!this.IsRootStateMachine) return false;
			string path = AssetDatabase.GetAssetPath(dst);
			if (string.IsNullOrEmpty(path)) return false;
			foreach (ClonedStateMachineInfo animatorStateMachine in this._clonedStateMachines.Values
				         .Where(
					         animatorStateMachine =>
						         string.IsNullOrEmpty(AssetDatabase.GetAssetPath(animatorStateMachine.StateMachine)))) {
				AssetDatabase.AddObjectToAsset(animatorStateMachine.StateMachine, path);
			}

			foreach (AnimatorState animatorState in this._clonedStates.Values.Where(animatorState =>
				         string.IsNullOrEmpty(AssetDatabase.GetAssetPath(animatorState)))) {
				AssetDatabase.AddObjectToAsset(animatorState, path);
			}

			foreach (ClonedTransitionInfo animatorStateTransition in this._clonedTransitions.Where(
				         animatorStateTransition =>
					         string.IsNullOrEmpty(AssetDatabase.GetAssetPath(animatorStateTransition.Transition)))) {
				AssetDatabase.AddObjectToAsset(animatorStateTransition.Transition, path);
			}

			foreach (StateMachineBehaviour behaviour in this._clonedBehaviours.Where(behaviour =>
				         string.IsNullOrEmpty(AssetDatabase.GetAssetPath(behaviour)))) {
				AssetDatabase.AddObjectToAsset(behaviour, path);
			}

			foreach (BlendTree blendTree in this._clonedBlendTrees.Where(blendTree =>
				         string.IsNullOrEmpty(AssetDatabase.GetAssetPath(blendTree)))) {
				AssetDatabase.AddObjectToAsset(blendTree, path);
			}

			return true;
		}

		/// <summary>
		/// 複製したステートマシン内で指定した名前のプロパティが使われているかを返します。
		/// </summary>
		/// <param name="propertyName">プロパティ名</param>
		/// <returns>使われている場合true</returns>
		public bool IsUsedPropertyName(string propertyName) {
			return this._usedParameterNames.Contains(propertyName);
		}

		/// <summary>
		/// この複製機のソースが参照元のSyncedLayerを指定したレイヤーに複製します。
		/// </summary>
		/// <param name="srcController">参照元のコントローラ</param>
		/// <param name="srcIndex">参照レイヤーのインデックス</param>
		/// <param name="dst">複製先レイヤー</param>
		/// <returns>内部で複製されたアセットがある場合trueが返ります。trueの場合SaveAssetを実行してください。</returns>
		public bool CloneSyncedLayer(AnimatorController srcController, int srcIndex, AnimatorControllerLayer dst) {
			bool returnFlag = false;
			SerializedObject controller = new SerializedObject(srcController);
			controller.Update();

			SerializedProperty animatorLayersProperty = controller.FindProperty("m_AnimatorLayers");
			SerializedProperty animatorLayersPropertyElement = animatorLayersProperty.GetArrayElementAtIndex(srcIndex);
			SerializedProperty motionsProperty = animatorLayersPropertyElement.FindPropertyRelative("m_Motions");
			SerializedProperty behavioursProperty = animatorLayersPropertyElement.FindPropertyRelative("m_Behaviours");

			for (int i = 0; i < motionsProperty.arraySize; i++) {
				SerializedProperty motionsPropertyElement = motionsProperty.GetArrayElementAtIndex(i);
				SerializedProperty stateProperty = motionsPropertyElement.FindPropertyRelative("m_State");
				SerializedProperty motionProperty = motionsPropertyElement.FindPropertyRelative("m_Motion");

				int stateInstanceId = stateProperty.objectReferenceInstanceIDValue;
				Object motionObject = motionProperty.objectReferenceValue;
				if (!(motionObject is Motion motion)) continue; // もしobjectがモーションで無ければスキップ
				if (motion is BlendTree blendTree) {
					// ブレンドツリーの場合複製。
					motion = this.CloneBlendTree(blendTree);
					returnFlag = true;
				}

				dst.SetOverrideMotion(this._clonedStates[stateInstanceId], motion);
			}

			for (int i = 0; i < behavioursProperty.arraySize; i++) {
				SerializedProperty behavioursPropertyElement = behavioursProperty.GetArrayElementAtIndex(i);
				SerializedProperty stateProperty = behavioursPropertyElement.FindPropertyRelative("m_State");
				SerializedProperty behaviourProperty =
					behavioursPropertyElement.FindPropertyRelative("m_StateMachineBehaviours");

				int stateInstanceId = stateProperty.objectReferenceInstanceIDValue;
				if (behaviourProperty.arraySize <= 0) continue;
				StateMachineBehaviour[] behaviours = new StateMachineBehaviour[behaviourProperty.arraySize];
				for (int j = 0; j < behaviourProperty.arraySize; j++) {
					Object behaviourObject = behaviourProperty.GetArrayElementAtIndex(j).objectReferenceValue;
					if (!(behaviourObject is StateMachineBehaviour behaviour)) continue; // もしobjectがビヘイビアで無ければスキップ
					returnFlag = true;
					behaviours[j] = this.CloneBehaviour(behaviour);
				}

				dst.SetOverrideBehaviours(this._clonedStates[stateInstanceId], behaviours);
			}

			return returnFlag;
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
			foreach (ClonedStateMachineInfo animatorStateMachine in this._clonedStateMachines.Values
				         .Where(
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
			foreach (ClonedTransitionInfo clonedStateTransition in this._clonedTransitions.Where(
				         clonedStateTransition => !clonedStateTransition.IsExitTransition)) {
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
			Motion motion;
			if (src.motion is BlendTree tree) {
				// BlendTreeの場合BlendTreeも複製します。
				motion = this.CloneBlendTree(tree);
			} else {
				motion = src.motion;
			}

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
				motion = motion,
				tag = src.tag,
				speedParameter = src.speedParameter,
				mirrorParameter = src.mirrorParameter,
				timeParameter = src.timeParameter,
			};

			this.RegisterClonedState(src.GetInstanceID(), dst);
			return dst;
		}

		/// <summary>
		/// <see cref="BlendTree"/>を複製し、複製したリストに登録します。
		/// </summary>
		/// <param name="src">ソース</param>
		/// <returns>複製されたBlendTree</returns>
		private BlendTree CloneBlendTree(BlendTree src) {
			ChildMotion[] childMotions = this.CloneChildMotion(src.children);
			BlendTree cloned = new BlendTree {
#if !NO_HIDE_IN_HIERARCHY
				hideFlags = HideFlags.HideInHierarchy,
#endif
				name = src.name,
				children = childMotions,
				blendParameter = src.blendParameter,
				blendParameterY = src.blendParameterY,
				blendType = src.blendType,
				minThreshold = src.minThreshold,
				maxThreshold = src.maxThreshold,
				useAutomaticThresholds = src.useAutomaticThresholds
			};
			this.RegisterClonedBlendTree(cloned);
			this.RegisterUsedParameterName(cloned.blendParameter);
			this.RegisterUsedParameterName(cloned.blendParameterY);
			return cloned;
		}

		/// <summary>
		/// <see cref="ChildMotion"/>を複製します。サブBlendTreeも複製されます。
		/// </summary>
		/// <param name="src">ソース</param>
		/// <returns>複製<see cref="ChildMotion"/>配列</returns>
		// ReSharper disable once SuggestBaseTypeForParameter
		private ChildMotion[] CloneChildMotion(ChildMotion[] src) {
			if (src.Length <= 0) return null;
			ChildMotion[] childMotions = new ChildMotion[src.Length];
			for (int i = 0; i < childMotions.Length; i++) {
				Motion motion;
				if (src[i].motion is BlendTree tree) {
					motion = this.CloneBlendTree(tree);
				} else {
					motion = src[i].motion;
				}

				childMotions[i] = src[i];
				childMotions[i].motion = motion;
			}

			return childMotions;
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
					src.destinationStateMachine.GetInstanceID(), false));
			} else if (src.destinationState != null) {
				this.RegisterClonedStateTransition(new ClonedTransitionInfo(dst, false,
					src.destinationState.GetInstanceID(), false));
			} else {
				this.RegisterClonedStateTransition(new ClonedTransitionInfo(dst, false, 0, true));
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
				this.RegisterUsedParameterName(cloned[i].parameter);
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

			this._clonedBehaviours.Add(behaviour);
			Undo.RegisterCreatedObjectUndo(behaviour, "Clone Behaviour");
		}

		/// <summary>
		/// クローンしたブレンドツリーを登録します。
		/// </summary>
		/// <param name="tree">複製した<see cref="BlendTree"/></param>
		private void RegisterClonedBlendTree(BlendTree tree) {
			if (!this.IsRootStateMachine) {
				this._parent.RegisterClonedBlendTree(tree);
				return;
			}

			this._clonedBlendTrees.Add(tree);
			Undo.RegisterCreatedObjectUndo(tree, "Clone Tree");
		}

		/// <summary>
		/// 使われているパラメータ名を登録します。
		/// </summary>
		/// <param name="parameterName">パラメータ名</param>
		private void RegisterUsedParameterName(string parameterName) {
			if (!this.IsRootStateMachine) {
				this._parent.RegisterUsedParameterName(parameterName);
				return;
			}

			this._usedParameterNames.Add(parameterName);
		}

		#endregion

		#endregion
	}
}
#endif