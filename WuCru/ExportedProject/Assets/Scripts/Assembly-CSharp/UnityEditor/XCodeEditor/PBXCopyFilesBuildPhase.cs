namespace UnityEditor.XCodeEditor
{
	public class PBXCopyFilesBuildPhase : PBXBuildPhase
	{
		public PBXCopyFilesBuildPhase(int buildActionMask)
		{
			Add("buildActionMask", buildActionMask);
			Add("dstPath", string.Empty);
			Add("dstSubfolderSpec", 10);
			Add("name", "Embed Frameworks");
			Add("runOnlyForDeploymentPostprocessing", 0);
		}

		public PBXCopyFilesBuildPhase(string guid, PBXDictionary dictionary)
			: base(guid, dictionary)
		{
		}
	}
}
