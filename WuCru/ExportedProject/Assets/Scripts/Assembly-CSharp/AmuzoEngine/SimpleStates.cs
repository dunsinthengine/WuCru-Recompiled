using System;
using System.Collections.Generic;

namespace AmuzoEngine
{
	public class SimpleStates<EStateType> where EStateType : struct, IConvertible
	{
		public delegate void DOnBeginState();

		public delegate void DOnUpdateState(float dt);

		public delegate void DOnEndState();

		public delegate void DOnStateChanged(EStateType oldState, EStateType newState);

		public interface IStateEvents<DStateEventType>
		{
			DStateEventType this[EStateType state] { set; }
		}

		private class StateEvents<DStateEventType> : IStateEvents<DStateEventType>
		{
			public delegate int DGetStateIndex(EStateType state);

			private DStateEventType[] _events;

			private DGetStateIndex _getStateIndex;

			DStateEventType IStateEvents<DStateEventType>.this[EStateType state]
			{
				set
				{
					AmuzoEngine_002ESimpleStates_003CEStateType_003E_002EIStateEvents_003CDStateEventType_003E_002Eset_Item(state, value);
				}
			}

			public DStateEventType this[int stateIndex]
			{
				get
				{
					return _events[stateIndex];
				}
			}

			public StateEvents(int stateCount, DGetStateIndex getStateIndex)
			{
				_events = new DStateEventType[stateCount];
				_getStateIndex = getStateIndex;
			}

			private void AmuzoEngine_002ESimpleStates_003CEStateType_003E_002EIStateEvents_003CDStateEventType_003E_002Eset_Item(EStateType state, DStateEventType value)
			{
				_events[_getStateIndex(state)] = value;
			}
		}

		private class OnBeginStateEvents : StateEvents<DOnBeginState>
		{
			public OnBeginStateEvents(int stateCount, DGetStateIndex getStateIndex)
				: base(stateCount, getStateIndex)
			{
			}
		}

		private class OnUpdateStateEvents : StateEvents<DOnUpdateState>
		{
			public OnUpdateStateEvents(int stateCount, DGetStateIndex getStateIndex)
				: base(stateCount, getStateIndex)
			{
			}
		}

		private class OnEndStateEvents : StateEvents<DOnEndState>
		{
			public OnEndStateEvents(int stateCount, DGetStateIndex getStateIndex)
				: base(stateCount, getStateIndex)
			{
			}
		}

		private const string LOG_TAG = "[SimpleStates] ";

		private EStateType[] _stateValues;

		private int _stateCount;

		private Dictionary<EStateType, int> _stateIndices;

		private int _defaultStateIndex;

		private int _currStateIndex;

		private int _prevStateIndex = -1;

		private OnBeginStateEvents _onBeginStateEvents;

		private OnUpdateStateEvents _onUpdateStateEvents;

		private OnEndStateEvents _onEndStateEvents;

		public EStateType _pState
		{
			get
			{
				return _stateValues[_currStateIndex];
			}
			set
			{
				OnSetState(_stateIndices[value], false);
			}
		}

		public bool _pHasPreviousState
		{
			get
			{
				return _prevStateIndex >= 0;
			}
		}

		public EStateType _pPreviousState
		{
			get
			{
				return _stateValues[_prevStateIndex];
			}
		}

		public IStateEvents<DOnBeginState> _pOnBeginState
		{
			get
			{
				return _onBeginStateEvents;
			}
		}

		public IStateEvents<DOnUpdateState> _pOnUpdateState
		{
			get
			{
				return _onUpdateStateEvents;
			}
		}

		public IStateEvents<DOnEndState> _pOnEndState
		{
			get
			{
				return _onEndStateEvents;
			}
		}

		private event DOnStateChanged _onStateChanged;

		public event DOnStateChanged _pOnStateChanged
		{
			add
			{
				_onStateChanged += value;
			}
			remove
			{
				_onStateChanged -= value;
			}
		}

		public SimpleStates(EStateType defaultState)
		{
			Initialize();
			_defaultStateIndex = _stateIndices[defaultState];
			ResetState();
		}

		private SimpleStates()
		{
		}

		public void ResetState()
		{
			OnSetState(_defaultStateIndex, true);
		}

		public void OnUpdate(float dt)
		{
			OnUpdateState(_currStateIndex, dt);
		}

		private void Initialize()
		{
			Array values = Enum.GetValues(typeof(EStateType));
			_stateCount = values.Length;
			_stateValues = new EStateType[_stateCount];
			_stateIndices = new Dictionary<EStateType, int>(_stateCount);
			for (int i = 0; i < _stateCount; i++)
			{
				_stateValues[i] = (EStateType)values.GetValue(i);
				_stateIndices.Add(_stateValues[i], i);
			}
			_onBeginStateEvents = new OnBeginStateEvents(_stateCount, GetStateIndex);
			_onUpdateStateEvents = new OnUpdateStateEvents(_stateCount, GetStateIndex);
			_onEndStateEvents = new OnEndStateEvents(_stateCount, GetStateIndex);
		}

		private int GetStateIndex(EStateType state)
		{
			return _stateIndices[state];
		}

		private void OnSetState(int newStateIndex, bool isForce)
		{
			if (_currStateIndex != newStateIndex || isForce)
			{
				OnEndState(_currStateIndex);
				_prevStateIndex = _currStateIndex;
				_currStateIndex = newStateIndex;
				OnBeginState(_currStateIndex);
				if (_onStateChanged != null)
				{
					_onStateChanged(_stateValues[_prevStateIndex], _stateValues[newStateIndex]);
				}
			}
		}

		private void OnBeginState(int stateIndex)
		{
			DOnBeginState dOnBeginState = _onBeginStateEvents[stateIndex];
			if (dOnBeginState != null)
			{
				dOnBeginState();
			}
		}

		private void OnUpdateState(int stateIndex, float dt)
		{
			DOnUpdateState dOnUpdateState = _onUpdateStateEvents[stateIndex];
			if (dOnUpdateState != null)
			{
				dOnUpdateState(dt);
			}
		}

		private void OnEndState(int stateIndex)
		{
			DOnEndState dOnEndState = _onEndStateEvents[stateIndex];
			if (dOnEndState != null)
			{
				dOnEndState();
			}
		}
	}
}
