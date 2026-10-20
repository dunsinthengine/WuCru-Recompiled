using System;
using System.Collections.Generic;

namespace UnityEditor.XCodeEditor
{
	public class PBXSortedDictionary : SortedDictionary<string, object>
	{
		public void Append(PBXDictionary dictionary)
		{
			foreach (KeyValuePair<string, object> item in dictionary)
			{
				Add(item.Key, item.Value);
			}
		}

		public void Append<T>(PBXDictionary<T> dictionary) where T : PBXObject
		{
			foreach (KeyValuePair<string, T> item in dictionary)
			{
				Add(item.Key, item.Value);
			}
		}
	}
	public class PBXSortedDictionary<T> : SortedDictionary<string, T> where T : PBXObject
	{
		public PBXSortedDictionary()
		{
		}

		public PBXSortedDictionary(PBXDictionary genericDictionary)
		{
			foreach (KeyValuePair<string, object> item in genericDictionary)
			{
				if (((string)((PBXDictionary)item.Value)["isa"]).CompareTo(typeof(T).Name) == 0)
				{
					T value = (T)Activator.CreateInstance(typeof(T), item.Key, (PBXDictionary)item.Value);
					Add(item.Key, value);
				}
			}
		}

		public void Add(T newObject)
		{
			Add(newObject.guid, newObject);
		}

		public void Append(PBXDictionary<T> dictionary)
		{
			foreach (KeyValuePair<string, T> item in dictionary)
			{
				Add(item.Key, item.Value);
			}
		}
	}
}
