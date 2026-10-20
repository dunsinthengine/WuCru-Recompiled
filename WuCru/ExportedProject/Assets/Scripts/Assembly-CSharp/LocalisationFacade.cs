using System;
using LitJson;
using UnityEngine;

public sealed class LocalisationFacade : LocalisationNode
{
	private static bool DO_DEBUG = true;

	public static bool DO_DOUBLE_TEXT = false;

	public static bool DO_OVERRIDE_TEXT_COLOUR = false;

	public static string OVERRIDE_COLOUR = "C90020";

	public static string OVERRIDE_TEMPLATE = "[{0}]{1}[-]";

	public static readonly string INVALID_STRING = "INVALID";

	public static bool SHOW_LOCALISATION_NODES = false;

	private static bool _sIsDebugMenuAdded = false;

	private static JsonData _localisationTable = null;

	private static string[] _sDefaultLanguages = new string[2] { "EN", "EN_UK" };

	private static string _defaultLanguage = null;

	private static LocalisationFacade _instance = null;

	private string _language;

	private string[] _availableLanguages;

	public static string _pDefaultLanguage
	{
		get
		{
			return (!string.IsNullOrEmpty(_defaultLanguage)) ? _defaultLanguage : _sDefaultLanguages[0];
		}
	}

	public static LocalisationFacade Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new LocalisationFacade();
				Facades<LocalisationFacade>.Register(_instance);
				TextAsset textAsset = Resources.Load("localisation") as TextAsset;
				JsonData jsonData = null;
				if (textAsset != null)
				{
					jsonData = Extensions.LoadJson(textAsset.text);
				}
				else
				{
					Debug.LogWarning("Localisation text not loaded from resources");
					jsonData = JsonLoader.data("localisation");
				}
				if (jsonData == null)
				{
					Debug.LogError("ERROR - Localisation text not found, game will not localise.");
				}
				_instance.Initialise(jsonData);
			}
			return _instance;
		}
	}

	public string language
	{
		get
		{
			return _language;
		}
		set
		{
			for (int i = 0; i < availableLanguages.Length; i++)
			{
				if (_availableLanguages[i] == value)
				{
					_language = value;
					return;
				}
			}
			Log("Cannot set language to '" + value + "' because it is not in available list of languages");
		}
	}

	public string[] availableLanguages
	{
		get
		{
			if (_localisation == null)
			{
				_availableLanguages = null;
				return null;
			}
			if (_availableLanguages != null)
			{
				return _availableLanguages;
			}
			JsonData jsonData = _localisation.JsonAtPath("Languages/list");
			if (jsonData == null)
			{
				return null;
			}
			_availableLanguages = ((string)jsonData).Split('|');
			return _availableLanguages;
		}
	}

	private LocalisationFacade()
	{
	}

	public void Initialise(JsonData localisationTable)
	{
		_localisationTable = localisationTable;
		if (_localisationTable != null)
		{
			_localisation = _localisationTable.JsonAtPath("localisation");
		}
		else
		{
			_localisation = null;
		}
		if (string.IsNullOrEmpty(_defaultLanguage))
		{
			SetDefaultLanguage();
		}
		Log("Default language code [" + _defaultLanguage + "].");
		Log("looking for language on device: \n" + string.Format("LANGUAGE ID: {0} \nLANGUAGE: {1} \n REGION: {2} \n CURRENCY CODE: {3} \n CURRENCY SYMBOL: {4}", PreciseLocale.GetLanguageID(), PreciseLocale.GetLanguage(), PreciseLocale.GetRegion(), PreciseLocale.GetCurrencyCode(), PreciseLocale.GetCurrencySymbol()));
		_language = GetLanguageCode(PreciseLocale.GetLanguageID());
		if (_language == null)
		{
			_language = _defaultLanguage;
		}
	}

	private void Reinitialize()
	{
		Initialise(_localisationTable);
	}

	private void SetDefaultLanguage()
	{
		string[] array = availableLanguages;
		int num = 0;
		for (num = 0; num < _sDefaultLanguages.Length && Array.IndexOf(array, _sDefaultLanguages[num]) < 0; num++)
		{
		}
		if (num < _sDefaultLanguages.Length)
		{
			_defaultLanguage = _sDefaultLanguages[num];
			Log("[LocalisationFacade] Setting default language to '" + _defaultLanguage + "'");
		}
		else
		{
			Debug.LogWarning("[LocalisationFacade] Failed to set a valid default language");
		}
	}

	public static string GetLanguageCode(string languageString)
	{
		if (string.IsNullOrEmpty(languageString))
		{
			return null;
		}
		Log("Language code received [" + languageString + "]");
		string text = _pDefaultLanguage;
		string[] array = languageString.ToLower().Split('_');
		if (array.Length <= 0)
		{
			Log("language code format unrecognised, returning default language");
			return text;
		}
		switch (array[0])
		{
		case "cs":
			text = "CS";
			break;
		case "dan":
		case "da":
			text = "DA";
			break;
		case "gsw":
		case "ger":
		case "deu":
		case "de":
		case "nds":
		case "gmh":
		case "goh":
		case "gem":
			text = "DE";
			break;
		case "spa":
		case "es":
		case "lad":
			text = ((array.Length <= 1 || !(array[1] == "ar")) ? "ES" : "AR");
			break;
		case "fi":
			text = "FI";
			break;
		case "fre":
		case "fra":
		case "fr":
			text = "FR";
			break;
		case "hu":
			text = "HU";
			break;
		case "ita":
		case "it":
			text = "IT";
			break;
		case "ja":
		case "jp":
			text = "JA";
			break;
		case "ko":
			text = "KO";
			break;
		case "dut":
		case "nld":
		case "nl":
		case "dum":
			text = "NL";
			break;
		case "no":
		case "nn":
		case "nb":
			text = "NB";
			break;
		case "pl":
			text = "PL";
			break;
		case "ru":
			text = "RU";
			break;
		case "sv":
			text = "SV";
			break;
		case "zh":
		case "zhsi":
		case "zh-hans":
		case "zh-hant":
			text = "ZH";
			break;
		case "gre":
		case "ell":
		case "el":
			text = "EL";
			break;
		case "ae":
		case "ar":
			text = "AE";
			break;
		case "hr":
			text = "HR";
			break;
		case "ro":
			text = "RO";
			break;
		case "tr":
			text = "TR";
			break;
		case "ua":
		case "uk":
			text = "UA";
			break;
		}
		Log("Language code returned [" + text + "]");
		return text;
	}

	public static string GetLanguageCode(SystemLanguage language)
	{
		string empty = string.Empty;
		Log("Language code received [" + language.ToString() + "]");
		switch (language)
		{
		case SystemLanguage.Czech:
			empty = "CS";
			break;
		case SystemLanguage.Danish:
			empty = "DA";
			break;
		case SystemLanguage.German:
			empty = "DE";
			break;
		case SystemLanguage.Spanish:
			empty = "ES";
			break;
		case SystemLanguage.Finnish:
			empty = "FI";
			break;
		case SystemLanguage.French:
			empty = "FR";
			break;
		case SystemLanguage.Hungarian:
			empty = "HU";
			break;
		case SystemLanguage.Italian:
			empty = "IT";
			break;
		case SystemLanguage.Japanese:
			empty = "JA";
			break;
		case SystemLanguage.Korean:
			empty = "KO";
			break;
		case SystemLanguage.Dutch:
			empty = "NL";
			break;
		case SystemLanguage.Norwegian:
			empty = "NB";
			break;
		case SystemLanguage.Polish:
			empty = "PL";
			break;
		case SystemLanguage.Russian:
			empty = "RU";
			break;
		case SystemLanguage.Swedish:
			empty = "SV";
			break;
		case SystemLanguage.Chinese:
			empty = "ZH";
			break;
		case SystemLanguage.ChineseSimplified:
			empty = "ZH";
			break;
		case SystemLanguage.ChineseTraditional:
			empty = "ZH";
			break;
		case SystemLanguage.Greek:
			empty = "EL";
			break;
		case SystemLanguage.Arabic:
			empty = "AE";
			break;
		case SystemLanguage.SerboCroatian:
			empty = "HR";
			break;
		case SystemLanguage.Romanian:
			empty = "RO";
			break;
		case SystemLanguage.Turkish:
			empty = "TR";
			break;
		case SystemLanguage.Ukrainian:
			empty = "UA";
			break;
		default:
			empty = _pDefaultLanguage;
			break;
		}
		Log("Language code returned [" + empty + "]");
		return empty;
	}

	public static void Log(string message, UnityEngine.Object o = null)
	{
		if (DO_DEBUG)
		{
			Debug.Log("LocalisationFacade: " + message, o);
		}
	}

	private void AddDebugMenu()
	{
		if (!_sIsDebugMenuAdded)
		{
			_sIsDebugMenuAdded = true;
			AmuzoDebugMenu amuzoDebugMenu = new AmuzoDebugMenu("LOCALISATION FACADE");
			amuzoDebugMenu.AddButton(new AmuzoDebugMenuButton(amuzoDebugMenu, () => (!SHOW_LOCALISATION_NODES) ? "SHOW LOCALISATION NODES" : "SHOW LOCALISED TEXT", () =>
			{
				ToggleLocalisationNodes();
			}));
			amuzoDebugMenu.AddButton(new AmuzoDebugMenuButton(amuzoDebugMenu, () => (!DO_OVERRIDE_TEXT_COLOUR) ? "OVERRIDE TEXT COLOUR" : "NORMAL TEXT COLOUR", () =>
			{
				ToggleLocalisationColour();
			}));
			AmuzoDebugMenuManager.RegisterRootDebugMenu(amuzoDebugMenu);
		}
	}

	private void ToggleLocalisationNodes()
	{
		SHOW_LOCALISATION_NODES = !SHOW_LOCALISATION_NODES;
	}

	private void ToggleLocalisationColour()
	{
		DO_OVERRIDE_TEXT_COLOUR = !DO_OVERRIDE_TEXT_COLOUR;
	}
}
