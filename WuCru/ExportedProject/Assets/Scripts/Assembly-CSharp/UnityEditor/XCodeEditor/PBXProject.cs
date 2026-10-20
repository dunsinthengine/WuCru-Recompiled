namespace UnityEditor.XCodeEditor
{
	public class PBXProject : PBXObject
	{
		protected string MAINGROUP_KEY = "mainGroup";

		protected string KNOWN_REGIONS_KEY = "knownRegions";

		protected bool _clearedLoc;

		public string mainGroupID
		{
			get
			{
				return (string)_data[MAINGROUP_KEY];
			}
		}

		public PBXList knownRegions
		{
			get
			{
				return (PBXList)_data[KNOWN_REGIONS_KEY];
			}
		}

		public PBXProject()
		{
		}

		public PBXProject(string guid, PBXDictionary dictionary)
			: base(guid, dictionary)
		{
		}

		public void AddRegion(string region)
		{
			if (!_clearedLoc)
			{
				knownRegions.Clear();
				_clearedLoc = true;
			}
			knownRegions.Add(region);
		}
	}
}
