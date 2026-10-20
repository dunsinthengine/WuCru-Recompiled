using System;
using System.Collections.Generic;

public class ObjectPool<T> where T : class
{
	public delegate T DInternalAlloc();

	public delegate void DInternalFree(T obj);

	private LinkedList<T> _freeList;

	private LinkedList<T> _activeList;

	private DInternalAlloc _internalAlloc;

	private DInternalFree _internalFree;

	private int _size
	{
		get
		{
			return _numFree + _numActive;
		}
	}

	public int Size
	{
		get
		{
			return _size;
		}
		set
		{
			SetSize(value);
		}
	}

	public LinkedList<T> ActiveList
	{
		get
		{
			return _activeList;
		}
	}

	private int _numFree
	{
		get
		{
			return _freeList.Count;
		}
	}

	private int _numActive
	{
		get
		{
			return _activeList.Count;
		}
	}

	public IEnumerable<T> ActiveObjects
	{
		get
		{
			LinkedListNode<T> objNode = _activeList.First;
			while (objNode != null)
			{
				LinkedListNode<T> nextNode = objNode.Next;
				yield return objNode.Value;
				objNode = nextNode;
			}
		}
	}

	public ObjectPool(int size, DInternalAlloc internalAllocFn, DInternalFree internalFreeFn)
	{
		_freeList = new LinkedList<T>();
		_activeList = new LinkedList<T>();
		_internalAlloc = internalAllocFn;
		_internalFree = internalFreeFn;
		EnsureInternalAllocFree();
		SetSize(size);
	}

	public T Allocate()
	{
		LinkedListNode<T> first = _freeList.First;
		if (first != null)
		{
			_freeList.Remove(first);
			_activeList.AddLast(first);
			return first.Value;
		}
		return (T)null;
	}

	public void Free(T obj)
	{
		if (obj != null)
		{
			LinkedListNode<T> activeListNode = GetActiveListNode(obj);
			if (activeListNode != null)
			{
				FreeNode(activeListNode);
			}
		}
	}

	public void FreeOldest(int count = 1)
	{
		while (_activeList.First != null && count > 0)
		{
			FreeNode(_activeList.First);
			count--;
		}
	}

	public void FreeSome(Predicate<T> wantFreePred)
	{
		if (wantFreePred == null)
		{
			return;
		}
		LinkedListNode<T> linkedListNode = _activeList.First;
		while (linkedListNode != null)
		{
			LinkedListNode<T> next = linkedListNode.Next;
			if (wantFreePred(linkedListNode.Value))
			{
				FreeNode(linkedListNode);
			}
			linkedListNode = next;
		}
	}

	public void FreeAll()
	{
		while (_activeList.First != null)
		{
			FreeNode(_activeList.First);
		}
	}

	protected virtual LinkedListNode<T> GetActiveListNode(T obj)
	{
		return _activeList.Find(obj);
	}

	private void FreeNode(LinkedListNode<T> objNode)
	{
		_activeList.Remove(objNode);
		_freeList.AddLast(objNode);
	}

	private void SetSize(int newSize)
	{
		int size = _size;
		if (newSize > size)
		{
			InternalAllocateObjects(newSize - size);
		}
		else if (newSize < size)
		{
			InternalFreeObjects(size - newSize);
		}
	}

	private void InternalAllocateObjects(int numObjects)
	{
		while (numObjects > 0)
		{
			_freeList.AddLast(_internalAlloc());
			numObjects--;
		}
	}

	private void InternalFreeObjects(int numObjects)
	{
		numObjects = InternalFreeObjects(ref _freeList, numObjects);
		if (numObjects > 0)
		{
			InternalFreeObjects(ref _activeList, numObjects);
		}
	}

	private int InternalFreeObjects(ref LinkedList<T> list, int numObjects)
	{
		while (numObjects > 0)
		{
			_internalFree(list.Last.Value);
			list.RemoveLast();
			numObjects--;
		}
		return numObjects;
	}

	private void EnsureInternalAllocFree()
	{
		if (_internalAlloc == null)
		{
			_internalAlloc = () => (T)null;
		}
		if (_internalFree == null)
		{
			_internalFree = (T obj) =>
			{
			};
		}
	}
}
