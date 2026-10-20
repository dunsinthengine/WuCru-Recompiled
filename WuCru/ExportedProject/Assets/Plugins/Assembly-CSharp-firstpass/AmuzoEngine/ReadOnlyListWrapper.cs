using System;
using System.Collections;
using System.Collections.Generic;

namespace AmuzoEngine
{
	public class ReadOnlyListWrapper<ElementT> : ReadOnlyListWrapperBase<ElementT>, IReadOnlyList<ElementT>, IList<ElementT>, IEnumerable<ElementT>, IEnumerable, ICollection<ElementT>
	{
		public ElementT this[int index]
		{
			get
			{
				return base._pWrappedList[index];
			}
			set
			{
				throw new NotImplementedException("Cannot set item in readonly list");
			}
		}

		public int Count
		{
			get
			{
				return base._pWrappedList.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public ReadOnlyListWrapper(IList<ElementT> wrappedList)
			: base(wrappedList)
		{
		}

		public ReadOnlyListWrapper(Func<IList<ElementT>> getWrappedList)
			: base(getWrappedList)
		{
		}

		public void Add(ElementT item)
		{
			throw new NotImplementedException("Cannot add item in readonly list");
		}

		public void Clear()
		{
			throw new NotImplementedException("Cannot clear items in readonly list");
		}

		public bool Contains(ElementT item)
		{
			return base._pWrappedList.Contains(item);
		}

		public void CopyTo(ElementT[] array, int arrayIndex)
		{
			base._pWrappedList.CopyTo(array, arrayIndex);
		}

		public IEnumerator<ElementT> GetEnumerator()
		{
			return base._pWrappedList.GetEnumerator();
		}

		public int IndexOf(ElementT item)
		{
			return base._pWrappedList.IndexOf(item);
		}

		public void Insert(int index, ElementT item)
		{
			throw new NotImplementedException("Cannot insert item in readonly list");
		}

		public bool Remove(ElementT item)
		{
			throw new NotImplementedException("Cannot remove item in readonly list");
		}

		public void RemoveAt(int index)
		{
			throw new NotImplementedException("Cannot remove item in readonly list");
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return base._pWrappedList.GetEnumerator();
		}
	}
}
