namespace UnityEditor.XCodeEditor
{
	public class PBXGroup : PBXObject
	{
		protected const string NAME_KEY = "name";

		protected const string CHILDREN_KEY = "children";

		protected const string PATH_KEY = "path";

		protected const string SOURCETREE_KEY = "sourceTree";

		public PBXList children
		{
			get
			{
				if (!ContainsKey("children"))
				{
					Add("children", new PBXList());
				}
				return (PBXList)_data["children"];
			}
		}

		public string name
		{
			get
			{
				if (!ContainsKey("name"))
				{
					return null;
				}
				return (string)_data["name"];
			}
		}

		public string path
		{
			get
			{
				if (!ContainsKey("path"))
				{
					return null;
				}
				return (string)_data["path"];
			}
		}

		public string sourceTree
		{
			get
			{
				return (string)_data["sourceTree"];
			}
		}

		public PBXGroup(string name, string path = null, string tree = "SOURCE_ROOT")
		{
			Add("children", new PBXList());
			Add("name", name);
			if (path != null)
			{
				Add("path", path);
				Add("sourceTree", tree);
			}
			else
			{
				Add("sourceTree", "<group>");
			}
		}

		public PBXGroup(string guid, PBXDictionary dictionary)
			: base(guid, dictionary)
		{
		}

		public string AddChild(PBXObject child)
		{
			if (child is PBXFileReference || child is PBXGroup)
			{
				children.Add(child.guid);
				return child.guid;
			}
			return null;
		}

		public void RemoveChild(string id)
		{
			if (PBXObject.IsGuid(id))
			{
				children.Remove(id);
			}
		}

		public bool HasChild(string id)
		{
			if (!ContainsKey("children"))
			{
				Add("children", new PBXList());
				return false;
			}
			if (!PBXObject.IsGuid(id))
			{
				return false;
			}
			return ((PBXList)_data["children"]).Contains(id);
		}

		public string GetName()
		{
			return (string)_data["name"];
		}
	}
}
