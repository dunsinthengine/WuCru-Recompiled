using System;
using System.Collections.Generic;

namespace UnityEditor.XCodeEditor
{
	public class PBXDictionary : Dictionary<string, object>
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

		public void Append(PBXSortedDictionary dictionary)
		{
			foreach (KeyValuePair<string, object> item in dictionary)
			{
				Add(item.Key, item.Value);
			}
		}

		public void Append<T>(PBXSortedDictionary<T> dictionary) where T : PBXObject
		{
			foreach (KeyValuePair<string, T> item in dictionary)
			{
				Add(item.Key, item.Value);
			}
		}

		public static implicit operator bool(PBXDictionary x)
		{
			return x != null && x.Count == 0;
		}

		public string ToCSV()
		{
			string text = string.Empty;
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, object> current = enumerator.Current;
					text += "<";
					text += current.Key;
					text += ", ";
					text += current.Value;
					text += ">, ";
				}
				return text;
			}
		}

		public override string ToString()
		{
			return "{" + ToCSV() + "}";
		}
	}
	public class PBXDictionary<T> : Dictionary<string, T> where T : PBXObject
	{
		public PBXDictionary()
		{
		}

		public PBXDictionary(PBXDictionary genericDictionary)
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

		public PBXDictionary(PBXSortedDictionary genericDictionary)
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
