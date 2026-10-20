using System;
using System.Collections.Generic;

namespace AmuzoEngine
{
	public static class ReadOnlyListExtensions
	{
		public static void ForEach<T>(this IReadOnlyList<T> thisList, Action<T> elementAction)
		{
			if (elementAction != null)
			{
				for (int i = 0; i < thisList.Count; i++)
				{
					elementAction(thisList[i]);
				}
			}
		}

		public static List<T> FindAll<T>(this IReadOnlyList<T> thisList, Predicate<T> elementMatch)
		{
			if (elementMatch == null)
			{
				return new List<T>(thisList);
			}
			List<T> list = new List<T>();
			for (int i = 0; i < thisList.Count; i++)
			{
				if (elementMatch(thisList[i]))
				{
					list.Add(thisList[i]);
				}
			}
			return list;
		}
	}
}
