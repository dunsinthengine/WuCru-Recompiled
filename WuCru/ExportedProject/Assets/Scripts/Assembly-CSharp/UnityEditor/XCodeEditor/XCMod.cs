using System.Collections;
using System.IO;
using UnityEngine;
using XUPorterJSON;

namespace UnityEditor.XCodeEditor
{
	public class XCMod
	{
		private Hashtable _datastore = new Hashtable();

		private ArrayList _libs;

		public string name { get; private set; }

		public string path { get; private set; }

		public string group
		{
			get
			{
				if (_datastore != null && _datastore.Contains("group"))
				{
					return (string)_datastore["group"];
				}
				return string.Empty;
			}
		}

		public ArrayList patches
		{
			get
			{
				return (ArrayList)_datastore["patches"];
			}
		}

		public ArrayList libs
		{
			get
			{
				if (_libs == null)
				{
					_libs = new ArrayList(((ArrayList)_datastore["libs"]).Count);
					foreach (string item in (ArrayList)_datastore["libs"])
					{
						Debug.Log("Adding to Libs: " + item);
						_libs.Add(new XCModFile(item));
					}
				}
				return _libs;
			}
		}

		public ArrayList frameworks
		{
			get
			{
				return (ArrayList)_datastore["frameworks"];
			}
		}

		public ArrayList headerpaths
		{
			get
			{
				return (ArrayList)_datastore["headerpaths"];
			}
		}

		public ArrayList files
		{
			get
			{
				return (ArrayList)_datastore["files"];
			}
		}

		public ArrayList folders
		{
			get
			{
				return (ArrayList)_datastore["folders"];
			}
		}

		public ArrayList excludes
		{
			get
			{
				return (ArrayList)_datastore["excludes"];
			}
		}

		public ArrayList compiler_flags
		{
			get
			{
				return (ArrayList)_datastore["compiler_flags"];
			}
		}

		public ArrayList linker_flags
		{
			get
			{
				return (ArrayList)_datastore["linker_flags"];
			}
		}

		public ArrayList embed_binaries
		{
			get
			{
				return (ArrayList)_datastore["embed_binaries"];
			}
		}

		public Hashtable plist
		{
			get
			{
				return (Hashtable)_datastore["plist"];
			}
		}

		public XCMod(string filename)
		{
			FileInfo fileInfo = new FileInfo(filename);
			if (!fileInfo.Exists)
			{
				Debug.LogWarning("File does not exist.");
			}
			name = Path.GetFileNameWithoutExtension(filename);
			path = Path.GetDirectoryName(filename);
			string text = fileInfo.OpenText().ReadToEnd();
			Debug.Log(text);
			_datastore = (Hashtable)XUPorterJSON.MiniJSON.jsonDecode(text);
			if (_datastore == null || _datastore.Count == 0)
			{
				Debug.Log(text);
				throw new UnityException("Parse error in file " + Path.GetFileName(filename) + "! Check for typos such as unbalanced quotation marks, etc.");
			}
		}
	}
}
