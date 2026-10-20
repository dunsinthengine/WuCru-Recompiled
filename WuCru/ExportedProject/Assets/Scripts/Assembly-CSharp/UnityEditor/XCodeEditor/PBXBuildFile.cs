namespace UnityEditor.XCodeEditor
{
	public class PBXBuildFile : PBXObject
	{
		private const string FILE_REF_KEY = "fileRef";

		private const string SETTINGS_KEY = "settings";

		private const string ATTRIBUTES_KEY = "ATTRIBUTES";

		private const string WEAK_VALUE = "Weak";

		private const string COMPILER_FLAGS_KEY = "COMPILER_FLAGS";

		public string fileRef
		{
			get
			{
				return (string)_data["fileRef"];
			}
		}

		public PBXBuildFile(PBXFileReference fileRef, bool weak = false)
		{
			Add("fileRef", fileRef.guid);
			SetWeakLink(weak);
			if (!string.IsNullOrEmpty(fileRef.compilerFlags))
			{
				string[] array = fileRef.compilerFlags.Split(',');
				foreach (string flag in array)
				{
					AddCompilerFlag(flag);
				}
			}
		}

		public PBXBuildFile(string guid, PBXDictionary dictionary)
			: base(guid, dictionary)
		{
		}

		public bool SetWeakLink(bool weak = false)
		{
			PBXDictionary pBXDictionary = null;
			PBXList pBXList = null;
			if (!_data.ContainsKey("settings"))
			{
				if (weak)
				{
					pBXList = new PBXList();
					pBXList.Add("Weak");
					pBXDictionary = new PBXDictionary();
					pBXDictionary.Add("ATTRIBUTES", pBXList);
					_data.Add("settings", pBXDictionary);
				}
				return true;
			}
			pBXDictionary = _data["settings"] as PBXDictionary;
			if (!pBXDictionary.ContainsKey("ATTRIBUTES"))
			{
				if (weak)
				{
					pBXList = new PBXList();
					pBXList.Add("Weak");
					pBXDictionary.Add("ATTRIBUTES", pBXList);
					return true;
				}
				return false;
			}
			pBXList = pBXDictionary["ATTRIBUTES"] as PBXList;
			if (weak)
			{
				pBXList.Add("Weak");
			}
			else
			{
				pBXList.Remove("Weak");
			}
			pBXDictionary.Add("ATTRIBUTES", pBXList);
			Add("settings", pBXDictionary);
			return true;
		}

		public bool AddCodeSignOnCopy()
		{
			if (!_data.ContainsKey("settings"))
			{
				_data["settings"] = new PBXDictionary();
			}
			PBXDictionary pBXDictionary = _data["settings"] as PBXDictionary;
			if (!pBXDictionary.ContainsKey("ATTRIBUTES"))
			{
				PBXList pBXList = new PBXList();
				pBXList.Add("CodeSignOnCopy");
				pBXList.Add("RemoveHeadersOnCopy");
				pBXDictionary.Add("ATTRIBUTES", pBXList);
			}
			else
			{
				PBXList pBXList2 = pBXDictionary["ATTRIBUTES"] as PBXList;
				pBXList2.Add("CodeSignOnCopy");
				pBXList2.Add("RemoveHeadersOnCopy");
			}
			return true;
		}

		public bool AddCompilerFlag(string flag)
		{
			if (!_data.ContainsKey("settings"))
			{
				_data["settings"] = new PBXDictionary();
			}
			if (!((PBXDictionary)_data["settings"]).ContainsKey("COMPILER_FLAGS"))
			{
				((PBXDictionary)_data["settings"]).Add("COMPILER_FLAGS", flag);
				return true;
			}
			string[] array = ((string)((PBXDictionary)_data["settings"])["COMPILER_FLAGS"]).Split(' ');
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (text.CompareTo(flag) == 0)
				{
					return false;
				}
			}
			((PBXDictionary)_data["settings"])["COMPILER_FLAGS"] = string.Join(" ", array) + " " + flag;
			return true;
		}
	}
}
