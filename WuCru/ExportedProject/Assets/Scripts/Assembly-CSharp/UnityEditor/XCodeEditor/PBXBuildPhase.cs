namespace UnityEditor.XCodeEditor
{
	public class PBXBuildPhase : PBXObject
	{
		protected const string FILES_KEY = "files";

		public PBXList files
		{
			get
			{
				if (!ContainsKey("files"))
				{
					Add("files", new PBXList());
				}
				return (PBXList)_data["files"];
			}
		}

		public PBXBuildPhase()
		{
		}

		public PBXBuildPhase(string guid, PBXDictionary dictionary)
			: base(guid, dictionary)
		{
		}

		public bool AddBuildFile(PBXBuildFile file)
		{
			if (!ContainsKey("files"))
			{
				Add("files", new PBXList());
			}
			((PBXList)_data["files"]).Add(file.guid);
			return true;
		}

		public void RemoveBuildFile(string id)
		{
			if (!ContainsKey("files"))
			{
				Add("files", new PBXList());
			}
			else
			{
				((PBXList)_data["files"]).Remove(id);
			}
		}

		public bool HasBuildFile(string id)
		{
			if (!ContainsKey("files"))
			{
				Add("files", new PBXList());
				return false;
			}
			if (!PBXObject.IsGuid(id))
			{
				return false;
			}
			return ((PBXList)_data["files"]).Contains(id);
		}
	}
}
