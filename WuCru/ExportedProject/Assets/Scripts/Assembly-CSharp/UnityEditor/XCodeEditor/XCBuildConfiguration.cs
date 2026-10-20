using UnityEngine;

namespace UnityEditor.XCodeEditor
{
	public class XCBuildConfiguration : PBXObject
	{
		protected const string BUILDSETTINGS_KEY = "buildSettings";

		protected const string HEADER_SEARCH_PATHS_KEY = "HEADER_SEARCH_PATHS";

		protected const string LIBRARY_SEARCH_PATHS_KEY = "LIBRARY_SEARCH_PATHS";

		protected const string FRAMEWORK_SEARCH_PATHS_KEY = "FRAMEWORK_SEARCH_PATHS";

		protected const string OTHER_C_FLAGS_KEY = "OTHER_CFLAGS";

		protected const string OTHER_LDFLAGS_KEY = "OTHER_LDFLAGS";

		public PBXSortedDictionary buildSettings
		{
			get
			{
				if (ContainsKey("buildSettings"))
				{
					if (_data["buildSettings"].GetType() == typeof(PBXDictionary))
					{
						PBXSortedDictionary pBXSortedDictionary = new PBXSortedDictionary();
						pBXSortedDictionary.Append((PBXDictionary)_data["buildSettings"]);
						return pBXSortedDictionary;
					}
					return (PBXSortedDictionary)_data["buildSettings"];
				}
				return null;
			}
		}

		public XCBuildConfiguration(string guid, PBXDictionary dictionary)
			: base(guid, dictionary)
		{
		}

		protected bool AddSearchPaths(string path, string key, bool recursive = true)
		{
			PBXList pBXList = new PBXList();
			pBXList.Add(path);
			return AddSearchPaths(pBXList, key, recursive);
		}

		protected bool AddSearchPaths(PBXList paths, string key, bool recursive = true, bool quoted = false)
		{
			bool result = false;
			if (!ContainsKey("buildSettings"))
			{
				Add("buildSettings", new PBXSortedDictionary());
			}
			foreach (string path in paths)
			{
				string text2 = path;
				if (!((PBXDictionary)_data["buildSettings"]).ContainsKey(key))
				{
					((PBXDictionary)_data["buildSettings"]).Add(key, new PBXList());
				}
				else if (((PBXDictionary)_data["buildSettings"])[key] is string)
				{
					PBXList pBXList = new PBXList();
					pBXList.Add(((PBXDictionary)_data["buildSettings"])[key]);
					((PBXDictionary)_data["buildSettings"])[key] = pBXList;
				}
				if (text2.Contains(" "))
				{
					quoted = true;
				}
				if (quoted)
				{
					text2 = ((!text2.EndsWith("/**")) ? ("\\\"" + text2 + "\\\"") : ("\\\"" + text2.Replace("/**", "\\\"/**")));
				}
				if (!((PBXList)((PBXDictionary)_data["buildSettings"])[key]).Contains(text2))
				{
					((PBXList)((PBXDictionary)_data["buildSettings"])[key]).Add(text2);
					result = true;
				}
			}
			return result;
		}

		public bool AddHeaderSearchPaths(PBXList paths, bool recursive = true)
		{
			return AddSearchPaths(paths, "HEADER_SEARCH_PATHS", recursive);
		}

		public bool AddLibrarySearchPaths(PBXList paths, bool recursive = true)
		{
			Debug.Log("AddLibrarySearchPaths " + paths);
			return AddSearchPaths(paths, "LIBRARY_SEARCH_PATHS", recursive);
		}

		public bool AddFrameworkSearchPaths(PBXList paths, bool recursive = true)
		{
			return AddSearchPaths(paths, "FRAMEWORK_SEARCH_PATHS", recursive);
		}

		public bool AddOtherCFlags(string flag)
		{
			PBXList pBXList = new PBXList();
			pBXList.Add(flag);
			return AddOtherCFlags(pBXList);
		}

		public bool AddOtherCFlags(PBXList flags)
		{
			bool result = false;
			if (!ContainsKey("buildSettings"))
			{
				Add("buildSettings", new PBXSortedDictionary());
			}
			foreach (string flag in flags)
			{
				if (!((PBXDictionary)_data["buildSettings"]).ContainsKey("OTHER_CFLAGS"))
				{
					((PBXDictionary)_data["buildSettings"]).Add("OTHER_CFLAGS", new PBXList());
				}
				else if (((PBXDictionary)_data["buildSettings"])["OTHER_CFLAGS"] is string)
				{
					string value = (string)((PBXDictionary)_data["buildSettings"])["OTHER_CFLAGS"];
					((PBXDictionary)_data["buildSettings"])["OTHER_CFLAGS"] = new PBXList();
					((PBXList)((PBXDictionary)_data["buildSettings"])["OTHER_CFLAGS"]).Add(value);
				}
				if (!((PBXList)((PBXDictionary)_data["buildSettings"])["OTHER_CFLAGS"]).Contains(flag))
				{
					((PBXList)((PBXDictionary)_data["buildSettings"])["OTHER_CFLAGS"]).Add(flag);
					result = true;
				}
			}
			return result;
		}

		public bool AddOtherLinkerFlags(string flag)
		{
			PBXList pBXList = new PBXList();
			pBXList.Add(flag);
			return AddOtherLinkerFlags(pBXList);
		}

		public bool AddOtherLinkerFlags(PBXList flags)
		{
			bool result = false;
			if (!ContainsKey("buildSettings"))
			{
				Add("buildSettings", new PBXSortedDictionary());
			}
			foreach (string flag in flags)
			{
				if (!((PBXDictionary)_data["buildSettings"]).ContainsKey("OTHER_LDFLAGS"))
				{
					((PBXDictionary)_data["buildSettings"]).Add("OTHER_LDFLAGS", new PBXList());
				}
				else if (((PBXDictionary)_data["buildSettings"])["OTHER_LDFLAGS"] is string)
				{
					string value = (string)((PBXDictionary)_data["buildSettings"])["OTHER_LDFLAGS"];
					((PBXDictionary)_data["buildSettings"])["OTHER_LDFLAGS"] = new PBXList();
					if (!string.IsNullOrEmpty(value))
					{
						((PBXList)((PBXDictionary)_data["buildSettings"])["OTHER_LDFLAGS"]).Add(value);
					}
				}
				if (!((PBXList)((PBXDictionary)_data["buildSettings"])["OTHER_LDFLAGS"]).Contains(flag))
				{
					((PBXList)((PBXDictionary)_data["buildSettings"])["OTHER_LDFLAGS"]).Add(flag);
					result = true;
				}
			}
			return result;
		}

		public bool overwriteBuildSetting(string settingName, string settingValue)
		{
			Debug.Log("overwriteBuildSetting " + settingName + " " + settingValue);
			bool result = false;
			if (!ContainsKey("buildSettings"))
			{
				Debug.Log("creating key buildSettings");
				Add("buildSettings", new PBXSortedDictionary());
			}
			if (!((PBXDictionary)_data["buildSettings"]).ContainsKey(settingName))
			{
				Debug.Log("adding key " + settingName);
				((PBXDictionary)_data["buildSettings"]).Add(settingName, new PBXList());
			}
			else if (((PBXDictionary)_data["buildSettings"])[settingName] is string)
			{
				((PBXDictionary)_data["buildSettings"])[settingName] = new PBXList();
			}
			if (!((PBXList)((PBXDictionary)_data["buildSettings"])[settingName]).Contains(settingValue))
			{
				Debug.Log("setting " + settingName + " to " + settingValue);
				((PBXList)((PBXDictionary)_data["buildSettings"])[settingName]).Add(settingValue);
				result = true;
			}
			return result;
		}
	}
}
