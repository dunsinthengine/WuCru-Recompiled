using UnityEngine;

public class PreciseLocale
{
	public static string GetRegion()
	{
		try { return System.Globalization.RegionInfo.CurrentRegion.TwoLetterISORegionName; } catch { return "US"; }
	}

	public static string GetLanguageID()
	{
		try { return System.Globalization.CultureInfo.CurrentCulture.Name; } catch { return "en-US"; }
	}

	public static string GetLanguage()
	{
		try { return System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName; } catch { return "en"; }
	}

	public static string GetCurrencyCode()
	{
		try { return System.Globalization.RegionInfo.CurrentRegion.ISOCurrencySymbol; } catch { return "USD"; }
	}

	public static string GetCurrencySymbol()
	{
		try { return System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol; } catch { return "$"; }
	}
}
