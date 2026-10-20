using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UnityEditor.XCodeEditor
{
	public class PBXObject
	{
		protected const string ISA_KEY = "isa";

		protected string _guid;

		protected PBXDictionary _data;

		public string guid
		{
			get
			{
				if (string.IsNullOrEmpty(_guid))
				{
					_guid = GenerateGuid();
				}
				return _guid;
			}
		}

		public PBXDictionary data
		{
			get
			{
				if (_data == null)
				{
					_data = new PBXDictionary();
				}
				return _data;
			}
		}

		public PBXObject()
		{
			_data = new PBXDictionary();
			_data["isa"] = GetType().Name;
			_guid = GenerateGuid();
		}

		public PBXObject(string guid)
			: this()
		{
			if (IsGuid(guid))
			{
				_guid = guid;
			}
		}

		public PBXObject(string guid, PBXDictionary dictionary)
			: this(guid)
		{
			if (!dictionary.ContainsKey("isa") || ((string)dictionary["isa"]).CompareTo(GetType().Name) != 0)
			{
				Debug.LogError("PBXDictionary is not a valid ISA object");
			}
			foreach (KeyValuePair<string, object> item in dictionary)
			{
				_data[item.Key] = item.Value;
			}
		}

		public static bool IsGuid(string aString)
		{
			return Regex.IsMatch(aString, "^[A-Fa-f0-9]{24}$");
		}

		public static string GenerateGuid()
		{
			return Guid.NewGuid().ToString("N").Substring(8)
				.ToUpper();
		}

		public void Add(string key, object obj)
		{
			_data.Add(key, obj);
		}

		public bool Remove(string key)
		{
			return _data.Remove(key);
		}

		public bool ContainsKey(string key)
		{
			return _data.ContainsKey(key);
		}

		public static implicit operator bool(PBXObject x)
		{
			return x != null && x.data.Count == 0;
		}

		public string ToCSV()
		{
			return string.Concat("\"", data, "\", ");
		}

		public override string ToString()
		{
			return "{" + ToCSV() + "} ";
		}
	}
}
