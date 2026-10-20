using System.Collections.Generic;

public class ObjectPoolFF<T> : ObjectPool<T> where T : class, IObjectPoolFFItem<T>
{
	public ObjectPoolFF(int size, DInternalAlloc internalAllocFn, DInternalFree internalFreeFn)
		: base(size, internalAllocFn, internalFreeFn)
	{
	}

	protected override LinkedListNode<T> GetActiveListNode(T obj)
	{
		return obj._pObjectPoolListNode;
	}
}
