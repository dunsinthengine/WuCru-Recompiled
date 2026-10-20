using System.Collections;
using System.Collections.Generic;

namespace AmuzoEngine
{
	public interface IReadOnlyList<ElementT> : IEnumerable<ElementT>, IEnumerable
	{
		ElementT this[int index] { get; }

		int Count { get; }

		bool Contains(ElementT item);

		void CopyTo(ElementT[] array, int arrayIndex);

		int IndexOf(ElementT item);
	}
}
