namespace UnityEditor.XCodeEditor
{
	public class XCModFile
	{
		public string filePath { get; private set; }

		public bool isWeak { get; private set; }

		public XCModFile(string inputString)
		{
			isWeak = false;
			if (inputString.Contains(":"))
			{
				string[] array = inputString.Split(':');
				filePath = array[0];
				isWeak = array[1].CompareTo("weak") == 0;
			}
			else
			{
				filePath = inputString;
			}
		}
	}
}
