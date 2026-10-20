public class LEGOID : InitialisationObject
{
	private string GetPlatform()
	{
		return "android";
	}

	public static string GetCorrectLanguageCode()
	{
		switch ("EN")
		{
		case "CS":
			return "cs-cz";
		case "DA":
			return "da-dk";
		case "DE":
			return "de-de";
		case "ES":
			return "es-es";
		case "ES_MX":
			return "es-ar";
		case "FI":
			return "fi-fi";
		case "FR":
			return "fr-fr";
		case "HU":
			return "hu-hu";
		case "IT":
			return "it-it";
		case "JA":
		case "JP":
			return "ja-jp";
		case "KO":
			return "ko-kr";
		case "NL":
			return "nl-be";
		case "NO":
		case "NB":
			return "nb-no";
		case "PL":
			return "pl-pl";
		case "RU":
			return "ru-ru";
		case "SV":
			return "sv-se";
		case "ZH":
		case "ZHSI":
			return "zh-cn";
		case "EN_UK":
			return "en-gb";
		case "RO":
			return "ro-ro";
		default:
			return "en-us";
		}
	}

	public static string GetCorrectLanguageCodeCapitolRegion()
	{
		string text = GetCorrectLanguageCode();
		string[] array = text.Split('-');
		if (array.Length == 2)
		{
			array[1] = array[1].ToUpper();
			text = array[0] + "-" + array[1];
		}
		return text;
	}
}
