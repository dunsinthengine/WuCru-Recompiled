using System;
using System.Collections.Generic;
using UnityEngine;

public class BooleanStateRequests
{
	[Flags]
	public enum ERequestField
	{
		NULL = 0,
		SOURCE = 1,
		PRIORITY = 2,
		STATE = 4,
		DEFAULT_ID_FIELDS = SOURCE
	}

	public delegate void DSetState(bool value);

	public delegate bool DGetState();

	public class Context
	{
		private string _name;

		public Context()
		{
			_name = "Anon";
		}

		public Context(string name)
		{
			_name = name;
		}

		public override string ToString()
		{
			return _name;
		}
	}

	private class Request
	{
		public object _source;

		public int _priority;

		public bool _state;

		public Request(object source, int priority, bool state)
		{
			_source = source;
			_priority = priority;
			_state = state;
		}
	}

	private const string LOG_TAG = "[BooleanStateRequests] ";

	public const int DEFAULT_PRIORITY = 0;

	private DSetState _setState;

	private DGetState _getState;

	private bool _defaultState;

	private ERequestField _requestIdFields;

	private LinkedList<Request> _requests;

	public BooleanStateRequests(DSetState setState, DGetState getState, bool defaultState, ERequestField requestIdFields = ERequestField.SOURCE)
	{
		_setState = setState;
		_getState = getState;
		_defaultState = defaultState;
		_requestIdFields = requestIdFields;
		ResetState();
	}

	private BooleanStateRequests()
	{
	}

	public void SetDefaultState(bool defaultState)
	{
		_defaultState = defaultState;
		RefreshState();
	}

	public void RequestState(bool wantRequest, object source, int priority, bool wantState)
	{
		if (wantRequest)
		{
			AddStateRequest(source, priority, wantState);
		}
		else
		{
			RemoveStateRequest(source, priority, wantState);
		}
	}

	public void Clear()
	{
		ResetState();
	}

	public void ForEachStateRequest(Action<object, int, bool> action)
	{
		if (action != null)
		{
			LinkedListNode<Request> linkedListNode = _requests.First;
			while (linkedListNode != null)
			{
				LinkedListNode<Request> next = linkedListNode.Next;
				action(linkedListNode.Value._source, linkedListNode.Value._priority, linkedListNode.Value._state);
				linkedListNode = next;
			}
		}
	}

	protected virtual bool ResolveConflict(int priority)
	{
		return _defaultState;
	}

	private void ResetState()
	{
		_requests = new LinkedList<Request>();
		RefreshState();
	}

	private void AddStateRequest(object source, int priority, bool state)
	{
		if (source == null)
		{
			Debug.LogError("[BooleanStateRequests] Can not add request with null source!");
			return;
		}
		bool flag = false;
		LinkedListNode<Request> linkedListNode = FindRequest(source, priority, state);
		if (linkedListNode != null)
		{
			Request value = linkedListNode.Value;
			if (value._source != source)
			{
				value._source = source;
				flag = true;
			}
			if (value._state != state)
			{
				value._state = state;
				flag = true;
			}
			if (value._priority != priority)
			{
				value._priority = priority;
				flag = true;
			}
		}
		else
		{
			_requests.AddLast(new Request(source, priority, state));
			flag = true;
		}
		if (flag)
		{
			RefreshState();
		}
	}

	private void RemoveStateRequest(object source, int priority, bool state)
	{
		if (source == null)
		{
			Debug.LogWarning("[BooleanStateRequests] Removing request with null source!");
		}
		LinkedListNode<Request> linkedListNode = FindRequest(source, priority, state);
		if (linkedListNode != null)
		{
			_requests.Remove(linkedListNode);
			RefreshState();
		}
	}

	private LinkedListNode<Request> FindRequest(object source, int priority, bool state)
	{
		for (LinkedListNode<Request> linkedListNode = _requests.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			if (((_requestIdFields & ERequestField.SOURCE) == 0 || linkedListNode.Value._source == source) && ((_requestIdFields & ERequestField.PRIORITY) == 0 || linkedListNode.Value._priority == priority) && ((_requestIdFields & ERequestField.STATE) == 0 || linkedListNode.Value._state == state))
			{
				return linkedListNode;
			}
		}
		return null;
	}

	private bool GetDesiredState(bool isRefreshing)
	{
		if (_requests == null || _requests.Count == 0)
		{
			return _defaultState;
		}
		int num = int.MinValue;
		int num2 = 0;
		bool flag = _defaultState;
		bool flag2 = false;
		bool flag3 = false;
		LinkedListNode<Request> linkedListNode = _requests.First;
		while (linkedListNode != null)
		{
			LinkedListNode<Request> next = linkedListNode.Next;
			if (linkedListNode.Value == null || linkedListNode.Value._source == null)
			{
				_requests.Remove(linkedListNode);
				flag3 = true;
			}
			else
			{
				int priority = linkedListNode.Value._priority;
				if (priority > num)
				{
					flag = linkedListNode.Value._state;
					num = priority;
					num2 = 1;
					flag2 = false;
				}
				else if (priority == num)
				{
					num2++;
					if (linkedListNode.Value._state != flag)
					{
						flag2 = true;
					}
				}
			}
			linkedListNode = next;
		}
		if (flag2)
		{
			flag = ResolveConflict(num);
		}
		if (flag3 && !isRefreshing)
		{
			RefreshState();
		}
		return flag;
	}

	private void CheckState(Action<bool> onWrongState, bool isRefreshing)
	{
		bool desiredState = GetDesiredState(isRefreshing);
		if (desiredState != _getState())
		{
			onWrongState(desiredState);
		}
	}

	private void RefreshState()
	{
		CheckState((bool wantState) =>
		{
			_setState(wantState);
		}, true);
	}
}
