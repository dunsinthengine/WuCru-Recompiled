using System.Collections;
using System.Collections.Generic;
using PlistCS;
using UnityEngine;

namespace UnityEditor.XCodeEditor
{
	public class XCPlist
	{
		private string plistPath;

		private bool plistModified;

		private const string BundleUrlTypes = "CFBundleURLTypes";

		private const string BundleTypeRole = "CFBundleTypeRole";

		private const string BundleUrlName = "CFBundleURLName";

		private const string BundleUrlSchemes = "CFBundleURLSchemes";

		private const string PlistUrlType = "urltype";

		private const string PlistRole = "role";

		private const string PlistEditor = "Editor";

		private const string PlistName = "name";

		private const string PlistSchemes = "schemes";

		public XCPlist(string plistPath)
		{
			this.plistPath = plistPath;
		}

		public void Process(Hashtable plist)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)Plist.readPlist(plistPath);
			foreach (DictionaryEntry item in plist)
			{
				AddPlistItems((string)item.Key, item.Value, dictionary);
			}
			if (plistModified)
			{
				Plist.writeXml(dictionary, plistPath);
			}
		}

		public void AddPlistItems(string key, object value, Dictionary<string, object> dict)
		{
			Debug.Log("AddPlistItems: key=" + key);
			if (key.CompareTo("urltype") == 0)
			{
				processUrlTypes((ArrayList)value, dict);
			}
			else
			{
				Debug.LogWarning("unknown plist key : " + key);
			}
		}

		private void processUrlTypes(ArrayList urltypes, Dictionary<string, object> dict)
		{
			List<object> list = ((!dict.ContainsKey("CFBundleURLTypes")) ? new List<object>() : ((List<object>)dict["CFBundleURLTypes"]));
			foreach (Hashtable urltype in urltypes)
			{
				string value = (string)urltype["role"];
				if (string.IsNullOrEmpty(value))
				{
					value = "Editor";
				}
				string text = (string)urltype["name"];
				ArrayList arrayList = (ArrayList)urltype["schemes"];
				List<object> list2 = new List<object>();
				foreach (string item in arrayList)
				{
					list2.Add(item);
				}
				Dictionary<string, object> dictionary = findUrlTypeByName(list, text);
				if (dictionary == null)
				{
					dictionary = new Dictionary<string, object>();
					dictionary["CFBundleTypeRole"] = value;
					dictionary["CFBundleURLName"] = text;
					dictionary["CFBundleURLSchemes"] = list2;
					list.Add(dictionary);
				}
				else
				{
					dictionary["CFBundleTypeRole"] = value;
					dictionary["CFBundleURLSchemes"] = list2;
				}
				plistModified = true;
			}
			dict["CFBundleURLTypes"] = list;
		}

		private Dictionary<string, object> findUrlTypeByName(List<object> bundleUrlTypes, string name)
		{
			if (bundleUrlTypes == null || bundleUrlTypes.Count == 0)
			{
				return null;
			}
			foreach (Dictionary<string, object> bundleUrlType in bundleUrlTypes)
			{
				string strA = (string)bundleUrlType["CFBundleURLName"];
				if (string.Compare(strA, name) == 0)
				{
					return bundleUrlType;
				}
			}
			return null;
		}
	}
}
