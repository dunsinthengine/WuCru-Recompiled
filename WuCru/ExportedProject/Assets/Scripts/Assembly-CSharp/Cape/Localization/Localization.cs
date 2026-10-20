using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using UnityEngine;

namespace Cape.Localization
{
	public class Localization
	{
		private static Localization _instance;

		private int _fileCount;

		private int _currentLanguageIndex = -1;

		private int _textLocalizationFileCount;

		private LocalizationConfig _localizationConfig;

		private readonly Dictionary<string, Font> _fontResources = new Dictionary<string, Font>();

		private readonly Dictionary<string, string> _fontMappings = new Dictionary<string, string>();

		private readonly Dictionary<string, string[]> _textMappings = new Dictionary<string, string[]>();

		private readonly Font _defaultFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");

		private readonly string _testLanguageString = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

		public readonly string TestLanguage = "test";

		private int _defaultLanguageIndex = -1;

		public static Localization Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new Localization();
				}
				return _instance;
			}
		}

		public List<string> AvailableLanguages { get; private set; }

		public string DefaultLanguage { get; set; }

		public int DefaultLanguageIndex
		{
			get
			{
				for (int i = 0; i < AvailableLanguages.Count; i++)
				{
					if (AvailableLanguages[i] == DefaultLanguage)
					{
						_defaultLanguageIndex = i;
						break;
					}
				}
				return _defaultLanguageIndex;
			}
		}

		public string CurrentLanguage
		{
			get
			{
				return AvailableLanguages[_currentLanguageIndex];
			}
		}

		public List<string> Keys
		{
			get
			{
				return new List<string>(_textMappings.Keys);
			}
		}

		public Font Font
		{
			get
			{
				string text = ((!(CurrentLanguage == TestLanguage)) ? CurrentLanguage : DefaultLanguage);
				if (_fontResources.ContainsKey(_fontMappings[text]))
				{
					return _fontResources[_fontMappings[text]];
				}
				Debug.Log("No font found for language '" + text + "', using default font (Arial)");
				return _defaultFont;
			}
		}

		public Directionality Direction
		{
			get
			{
				if (CurrentLanguage == "ar-AE")
				{
					return Directionality.RightToLeft;
				}
				return Directionality.LeftToRight;
			}
		}

		public bool UpperCaseText { get; private set; }

		public event LocalizeDelegate LocalizationChanged;

		private Localization()
		{
			_localizationConfig = Resources.Load<LocalizationConfig>("Localization/LocalizationConfig");
			LoadFontMappings(_localizationConfig.fontLocalizationFile);
			TextAsset[] textLocalizationFiles = _localizationConfig.textLocalizationFiles;
			foreach (TextAsset txt in textLocalizationFiles)
			{
				LoadTextMappings(txt);
			}
			AvailableLanguages.Add(TestLanguage);
			DefaultLanguage = _localizationConfig.defaultLanguage;
			SelectLanguage("en-US");
			UpperCaseText = _localizationConfig.upperCaseText;
		}

		public bool LoadFontMappings(TextAsset txt)
		{
			if (txt != null && LoadFontCSV(txt))
			{
				return true;
			}
			return false;
		}

		public bool LoadTextMappings(TextAsset txt)
		{
			if (txt != null && LoadTextCSV(txt))
			{
				return true;
			}
			return false;
		}

		private bool LoadFontCSV(TextAsset asset)
		{
			LocalizationFileReader localizationFileReader = new LocalizationFileReader(asset);
			List<string> list = localizationFileReader.ReadCSV();
			int count = list.Count;
			if (count < 2)
			{
				return false;
			}
			string[] array = new string[count];
			list.CopyTo(array);
			List<string> list2 = localizationFileReader.ReadCSV();
			for (int i = 0; i < count; i++)
			{
				try
				{
					_fontMappings.Add(array[i], list2[i]);
				}
				catch (Exception)
				{
					Debug.LogError(string.Format("Unable to add '{0}' to the text localization map. Found the double in {1}", list[i], asset.name));
				}
				if (!_fontResources.ContainsKey(list2[i]) && !string.IsNullOrEmpty(list2[i]))
				{
					Font value = Resources.Load<Font>("Fonts/" + list2[i]);
					_fontResources.Add(list2[i], value);
				}
			}
			return true;
		}

		private bool LoadTextCSV(TextAsset asset)
		{
			LocalizationFileReader localizationFileReader = new LocalizationFileReader(asset);
			List<string> list = localizationFileReader.ReadCSV();
			if (list.Count < 2)
			{
				return false;
			}
			if (!string.Equals(list[0].ToLower(), "key"))
			{
				Debug.LogWarning("Invalid localization CSV file. The first value is expected to be 'Key', followed by language columns.\nInstead found '" + list[0] + "'", asset);
				return false;
			}
			if (AvailableLanguages == null)
			{
				AvailableLanguages = new List<string>();
				for (int i = 1; i < list.Count; i++)
				{
					AvailableLanguages.Add(list[i]);
				}
			}
			else if (AvailableLanguages.Count != list.Count - 1)
			{
				Debug.LogError(string.Format("Localization file {0} does not have the correct amount of languages, expected {1} but got {2}", asset.name, AvailableLanguages.Count, list.Count - 1));
				return false;
			}
			for (list = localizationFileReader.ReadCSV(); list != null; list = localizationFileReader.ReadCSV())
			{
				string[] array = new string[list.Count - 1];
				for (int j = 0; j < array.Length; j++)
				{
					array[j] = list[j + 1];
				}
				try
				{
					_textMappings.Add(list[0], array);
				}
				catch (Exception)
				{
					Debug.LogError(string.Format("Unable to add '{0}' to the Localization _textMapping. Found the double in {1}", list[0], asset.name));
				}
			}
			return true;
		}

		public bool SelectLanguage(string language)
		{
			_currentLanguageIndex = -1;
			if (AvailableLanguages == null)
			{
				Debug.LogError("[Localization.Instance.SelectLanguage] No available languages. Has data been loaded?");
				return false;
			}
			for (int i = 0; i < AvailableLanguages.Count; i++)
			{
				if (AvailableLanguages[i] == language)
				{
					_currentLanguageIndex = i;
					break;
				}
			}
			try
			{
				Thread.CurrentThread.CurrentCulture = new CultureInfo(CurrentLanguage);
			}
			catch
			{
				Debug.LogWarningFormat("Can not set culture from language '{0}'", CurrentLanguage);
			}
			if (Application.isPlaying && LocalizationChanged != null)
			{
				LocalizationChanged();
			}
			Debug.Log("Language set to '" + language + "'");
			return _currentLanguageIndex >= 0;
		}

		public void SelectPreviousLanguage()
		{
			_currentLanguageIndex = ((_currentLanguageIndex != 0) ? (--_currentLanguageIndex) : (AvailableLanguages.Count - 1));
			SelectLanguage(AvailableLanguages[_currentLanguageIndex]);
		}

		public void SelectNextLanguage()
		{
			_currentLanguageIndex++;
			_currentLanguageIndex %= AvailableLanguages.Count;
			SelectLanguage(AvailableLanguages[_currentLanguageIndex]);
		}

		public string Get(string key)
		{
			if (CurrentLanguage == TestLanguage)
			{
				return _testLanguageString;
			}
			string[] value;
			_textMappings.TryGetValue(key, out value);
			if (_currentLanguageIndex != -1 && value != null && value.Length > 0 && _currentLanguageIndex < value.Length && !string.IsNullOrEmpty(value[_currentLanguageIndex]))
			{
				return value[_currentLanguageIndex];
			}
			Debug.LogWarning("Missing translation for key-language pair (" + key + ", " + CurrentLanguage + ")");
			if (value != null && value.Length > 0 && DefaultLanguageIndex < value.Length && !string.IsNullOrEmpty(value[DefaultLanguageIndex]))
			{
				Debug.LogWarning("Using default language fallback for key-language pair (" + key + ", " + CurrentLanguage + ")");
				return value[DefaultLanguageIndex];
			}
			Debug.LogWarning("Missing default translation for key-language pair (" + key + ", " + AvailableLanguages[DefaultLanguageIndex] + "), returning key as result");
			return key;
		}

		public string[] GetAll(string key)
		{
			string[] value;
			if (_textMappings.TryGetValue(key, out value))
			{
				return value;
			}
			Debug.LogWarning("Localization key not found: '" + key + "'");
			return null;
		}

		public bool Exists(string key)
		{
			return _textMappings.ContainsKey(key);
		}
	}
}
