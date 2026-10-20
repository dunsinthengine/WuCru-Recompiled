using UnityEngine;

namespace Cape.Localization
{
	public class LocalizationConfig : ScriptableObject
	{
		public TextAsset fontLocalizationFile;

		public TextAsset[] textLocalizationFiles;

		public string defaultLanguage = "en-US";

		public bool upperCaseText;
	}
}
