using System;
using System.Collections.Generic;

namespace AmuzoEngine
{
	public class ReadOnlyListWrapperBase<ElementT>
	{
		private IList<ElementT> _wrappedList;

		private Func<IList<ElementT>> _getWrappedList;

		protected IList<ElementT> _pWrappedList
		{
			get
			{
				return _wrappedList ?? ((_getWrappedList == null) ? null : _getWrappedList());
			}
		}

		protected ReadOnlyListWrapperBase(IList<ElementT> wrappedList)
		{
			_wrappedList = wrappedList;
		}

		protected ReadOnlyListWrapperBase(Func<IList<ElementT>> getWrappedList)
		{
			_getWrappedList = getWrappedList;
		}

		private ReadOnlyListWrapperBase()
		{
		}
	}
}
