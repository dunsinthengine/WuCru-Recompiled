using System.Collections.Generic;

public interface IObjectPoolFFItem<T>
{
	LinkedListNode<T> _pObjectPoolListNode { get; }
}
