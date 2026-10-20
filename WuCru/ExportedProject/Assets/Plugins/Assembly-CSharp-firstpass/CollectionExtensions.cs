using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CollectionExtensions
{
	public static void SafeClear<T>(this List<T> list)
	{
		if (list != null && list.Count > 0)
		{
			list.Clear();
		}
	}

	public static T First<T>(this IList<T> targ)
	{
		return (targ.Count != 0) ? targ[0] : default(T);
	}

	public static T Last<T>(this IList<T> targ)
	{
		return (targ.Count != 0) ? targ[targ.Count - 1] : default(T);
	}

	public static T[] Push<T>(this T[] targ, T item)
	{
		T[] array = new T[targ.Length + 1];
		targ.CopyTo(array, 0);
		array[array.Length - 1] = item;
		return array;
	}

	public static T GetRandom<T>(this IList<T> list)
	{
		if (list.Count == 0)
		{
			return default(T);
		}
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	public static T GetRandom<T>(this IList<T> list, bool doRemove)
	{
		if (list.Count == 0)
		{
			return default(T);
		}
		int index = UnityEngine.Random.Range(0, list.Count);
		T result = list[index];
		if (doRemove)
		{
			list.RemoveAt(index);
		}
		return result;
	}

	public static T[] Concat<T>(this IList<T> targ, IList<T> other)
	{
		T[] array = new T[targ.Count + other.Count];
		targ.CopyTo(array, 0);
		other.CopyTo(array, targ.Count);
		return array;
	}

	public static T[] Add<T>(this IList<T> targ, params T[] others)
	{
		T[] array = new T[targ.Count + others.Length];
		targ.CopyTo(array, 0);
		others.CopyTo(array, targ.Count);
		return array;
	}

	public static bool Contains<T>(this IList<T> targ, T item)
	{
		return targ.IndexOf(item) != -1;
	}

	public static void Remove<T>(this IList<T> targ, IList<T> items)
	{
		foreach (T item in items)
		{
			targ.Remove(item);
		}
	}

	public static void CloneTo(this IList thisList, IList toList)
	{
		if (toList != null)
		{
			int num = Math.Min(thisList.Count, toList.Count);
			for (int i = 0; i < num; i++)
			{
				toList[i] = thisList[i];
			}
			for (int j = num; j < thisList.Count; j++)
			{
				toList.Add(thisList[j]);
			}
			while (toList.Count > thisList.Count)
			{
				toList.RemoveAt(toList.Count - 1);
			}
		}
	}

	public static void ForEach<T>(this IList<T> thisList, Action<T> elementAction)
	{
		if (elementAction != null)
		{
			for (int i = 0; i < thisList.Count; i++)
			{
				elementAction(thisList[i]);
			}
		}
	}

	public static List<T> FindAll<T>(this IList<T> thisList, Predicate<T> elementMatch)
	{
		if (elementMatch == null)
		{
			new List<T>(thisList);
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

	public static void SetCountAtLeast(this IList list, int countAtLeast, object value = null, Action<int> onPreAdd = null, Action<int> onAdded = null)
	{
		if (list == null)
		{
			throw new NullReferenceException("list is null");
		}
		while (list.Count < countAtLeast)
		{
			if (onPreAdd != null)
			{
				onPreAdd(list.Count);
			}
			list.Add(value);
			if (onAdded != null)
			{
				onAdded(list.Count - 1);
			}
		}
	}

	public static void SetCountAtMost(this IList list, int countAtMost, Action<int> onPreRemove = null, Action<int> onRemoved = null)
	{
		if (list == null)
		{
			throw new NullReferenceException("list is null");
		}
		if (countAtMost < 0)
		{
			throw new ArgumentOutOfRangeException("countAtMost", countAtMost, "expected non-negative value");
		}
		while (list.Count > countAtMost)
		{
			if (onPreRemove != null)
			{
				onPreRemove(list.Count - 1);
			}
			list.RemoveAt(list.Count - 1);
			if (onRemoved != null)
			{
				onRemoved(list.Count);
			}
		}
	}

	public static void SetCount(this IList list, int count, object defaultValue = null, Action<int> onPreAdd = null, Action<int> onAdded = null, Action<int> onPreRemove = null, Action<int> onRemoved = null)
	{
		if (list == null)
		{
			throw new NullReferenceException("list is null");
		}
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("count", count, "expected non-negative value");
		}
		list.SetCountAtLeast(count, defaultValue, onPreAdd, onAdded);
		list.SetCountAtMost(count, onPreRemove, onRemoved);
	}
}
