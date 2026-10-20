using System;
using System.Collections.Generic;
using UnityEngine;

namespace AmuzoEngine
{
	public sealed class ActionQueue
	{
		private enum EActionState
		{
			NULL = 0,
			WAITING = 1,
			ACTIVE = 2,
			COMPLETE = 3,
			CANCELLED = 4
		}

		public enum EActionStart
		{
			LAST = 0,
			NEXT = 1,
			NOW = 2
		}

		public enum EActionRemovePolicy
		{
			ON_ALL_ENDED = 0,
			ON_ONE_ENDED = 1,
			UNMANAGED = 2
		}

		private class Node
		{
			public int _actionId;

			public Action _action;

			public bool _isAsync;

			public string _name;

			public EActionState _state;

			public Node(int actionId, Action action, bool isAsync, string name)
			{
				_actionId = actionId;
				_action = action;
				_isAsync = isAsync;
				_name = name ?? "unnamed action";
				_state = EActionState.NULL;
			}
		}

		private const string LOG_TAG = "[ActionQueue] ";

		private const string QUEUE_NAME_DEFAULT = "unnamed queue";

		private const string ACTION_NAME_DEFAULT = "unnamed action";

		private static SimpleIntegerIdGenerator _sActionIdGenerator = new SimpleIntegerIdGenerator();

		private string _name;

		private EActionRemovePolicy _removePolicy;

		private List<Node> _nodes;

		private Dictionary<object, List<int>> _actionGroups;

		private int _nextNode;

		private int _numActive;

		private string _pLogTag
		{
			get
			{
				return "[ActionQueue:" + _name + "] ";
			}
		}

		private bool _pIsNextAction
		{
			get
			{
				return _nextNode < _nodes.Count;
			}
		}

		public bool _pIsEmpty
		{
			get
			{
				return _nodes == null || _nextNode == _nodes.Count;
			}
		}

		public bool _pIsAllActionsEnded
		{
			get
			{
				return _pIsEmpty && _numActive == 0;
			}
		}

		public int _pTotalCount
		{
			get
			{
				return (_nodes != null) ? _nodes.Count : 0;
			}
		}

		public int _pActiveCount
		{
			get
			{
				return _numActive;
			}
		}

		public event Action<int> _pOnActionStarted;

		public event Action<int> _pOnActionEnded;

		public event Action _pOnAllActionsEnded;

		public ActionQueue()
			: this("unnamed queue")
		{
		}

		public ActionQueue(string name)
		{
			_name = name;
			Reset();
		}

		public void SetActionRemovePolicy(EActionRemovePolicy removePolicy)
		{
			_removePolicy = removePolicy;
		}

		public int AddAction(Action action, Action<int> onActionAdded = null, bool isAsyncAction = false, string name = null, object groupId = null, EActionStart start = EActionStart.LAST)
		{
			if (_nodes == null)
			{
				return -1;
			}
			if (action == null)
			{
				return -1;
			}
			int pNew = _sActionIdGenerator._pNew;
			Node node = new Node(pNew, action, isAsyncAction, name);
			if (groupId != null)
			{
				SetActionGroup(node, groupId);
			}
			SetActionState(node, EActionState.WAITING);
			if (start == EActionStart.LAST)
			{
				_nodes.Add(node);
			}
			else
			{
				_nodes.Insert(_nextNode, node);
			}
			if (onActionAdded != null)
			{
				onActionAdded(pNew);
			}
			if (start == EActionStart.NOW)
			{
				DoNextAction();
			}
			return pNew;
		}

		public void OnActionComplete(int actionId)
		{
			Node node = FindNode(actionId);
			if (node == null)
			{
				Debug.LogWarning(_pLogTag + "Failed to find action " + actionId);
			}
			else if (node._state != EActionState.CANCELLED)
			{
				if (node._state != EActionState.ACTIVE)
				{
					Debug.LogError(_pLogTag + "Unexpected action state: " + node._state);
				}
				else
				{
					SetActionState(node, EActionState.COMPLETE);
				}
			}
		}

		public void DoActions(Func<bool> isNextActionAllowed = null)
		{
			if (_nodes == null)
			{
				return;
			}
			if (isNextActionAllowed == null)
			{
				isNextActionAllowed = () => true;
			}
			while (_pIsNextAction && isNextActionAllowed())
			{
				DoNextAction();
			}
		}

		public void DoActions(int maxActions)
		{
			int actionCounter = _numActive;
			DoActions(() => actionCounter++ < maxActions);
		}

		public void DoActions(float maxSeconds, Func<float> getSystemSeconds)
		{
			float actionMaxTime = 0.01f;
			float actionStartTime = getSystemSeconds();
			DoActions(() => getSystemSeconds() - actionStartTime < actionMaxTime);
		}

		public void Reset()
		{
			_nodes = new List<Node>();
			_actionGroups = new Dictionary<object, List<int>>();
			_nextNode = 0;
			_numActive = 0;
		}

		public void CancelAction(int actionId)
		{
			Node node = FindNode(actionId);
			if (node != null)
			{
				SetActionState(node, EActionState.CANCELLED);
			}
		}

		public void CancelActions(object groupId)
		{
			List<int> list = FindActionGroup(groupId);
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					CancelAction(list[i]);
				}
			}
		}

		public bool IsAllActionsEnded(object groupId)
		{
			if (groupId == null)
			{
				return _pIsAllActionsEnded;
			}
			List<int> list = FindActionGroup(groupId);
			if (list == null)
			{
				return true;
			}
			bool flag = true;
			for (int i = 0; i < list.Count; i++)
			{
				if (!flag)
				{
					break;
				}
				Node node = FindNode(list[i]);
				if (node != null)
				{
					flag = node._state == EActionState.COMPLETE || node._state == EActionState.CANCELLED;
				}
			}
			return flag;
		}

		private void DoNextAction()
		{
			Node node = _nodes[_nextNode++];
			if (node == null)
			{
				Debug.LogError(_pLogTag + "Null node!");
			}
			else
			{
				if (node._state == EActionState.CANCELLED)
				{
					return;
				}
				if (node._state != EActionState.WAITING)
				{
					Debug.LogError(_pLogTag + "Unexpected action state: " + node._state);
					return;
				}
				SetActionState(node, EActionState.ACTIVE);
				node._action();
				if (!node._isAsync)
				{
					SetActionState(node, EActionState.COMPLETE);
				}
			}
		}

		private void SetActionState(Node node, EActionState newState)
		{
			if (node._state != newState)
			{
				if (node._state == EActionState.ACTIVE)
				{
					_numActive--;
				}
				node._state = newState;
				switch (node._state)
				{
				case EActionState.ACTIVE:
					_numActive++;
					OnActionStarted(node);
					break;
				case EActionState.COMPLETE:
				case EActionState.CANCELLED:
					OnActionEnded(node);
					break;
				}
			}
		}

		private void OnActionStarted(Node node)
		{
			if (_pOnActionStarted != null)
			{
				_pOnActionStarted(node._actionId);
			}
		}

		private void OnActionEnded(Node node)
		{
			node._action = null;
			if (_pOnActionEnded != null)
			{
				_pOnActionEnded(node._actionId);
			}
			if (_pIsAllActionsEnded)
			{
				OnAllActionsEnded();
			}
			if (_removePolicy == EActionRemovePolicy.ON_ONE_ENDED)
			{
				RemoveAction(node);
			}
		}

		private void OnAllActionsEnded()
		{
			if (_pOnAllActionsEnded != null)
			{
				_pOnAllActionsEnded();
			}
			if (_removePolicy == EActionRemovePolicy.ON_ALL_ENDED)
			{
				Reset();
			}
		}

		private void RemoveAction(Node node)
		{
			int num = _nodes.IndexOf(node);
			if (num >= 0)
			{
				_nodes.RemoveAt(num);
				if (num < _nextNode)
				{
					_nextNode--;
				}
			}
		}

		private Node FindNode(int actionId)
		{
			return _nodes.Find((Node n) => n._actionId == actionId);
		}

		private void SetActionGroup(Node node, object groupId)
		{
			List<int> list;
			if (_actionGroups.ContainsKey(groupId))
			{
				list = _actionGroups[groupId];
			}
			else
			{
				list = new List<int>();
				_actionGroups.Add(groupId, list);
			}
			list.Add(node._actionId);
		}

		private List<int> FindActionGroup(object groupId)
		{
			if (!_actionGroups.ContainsKey(groupId))
			{
				return null;
			}
			return _actionGroups[groupId];
		}
	}
}
