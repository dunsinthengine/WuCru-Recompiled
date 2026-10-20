using System;
using UnityEngine;

public class AmuzoLEGOFacade : AmuzoScriptableSingleton<AmuzoLEGOFacade>
{
	private const string LOG_TAG = "[AmuzoLEGOFacade] ";

	private static string _urlPrivacy;

	private static string _urlCookies;

	private static string _exitAppMessage;

	[SerializeField]
	private bool _useCrossPromoDefault;

	[SerializeField]
	private string _urlPrivacyKey;

	[SerializeField]
	private string _urlCookiesKey;

	[SerializeField]
	private string _exitAppMessageKillSwitchKey;

	[SerializeField]
	private string _exitAppMessageMinVersionKey;

	public static bool _pIsKillSwitchRaised { get; private set; }

	public static bool _pIsMinVersionWrong { get; private set; }

	public static bool _pUseCrossPromo { get; set; }

	public static string _pUrlPrivacy
	{
		get
		{
			if (_urlPrivacy == null && !string.IsNullOrEmpty(AmuzoScriptableSingleton<AmuzoLEGOFacade>._pInstance._urlPrivacyKey))
			{
				return Facades<LocalisationFacade>.Instance.GetString(AmuzoScriptableSingleton<AmuzoLEGOFacade>._pInstance._urlPrivacyKey);
			}
			return _urlPrivacy;
		}
		set
		{
			_urlPrivacy = value;
		}
	}

	public static string _pUrlCookies
	{
		get
		{
			if (_urlCookies == null && !string.IsNullOrEmpty(AmuzoScriptableSingleton<AmuzoLEGOFacade>._pInstance._urlCookiesKey))
			{
				return Facades<LocalisationFacade>.Instance.GetString(AmuzoScriptableSingleton<AmuzoLEGOFacade>._pInstance._urlCookiesKey);
			}
			return _urlCookies;
		}
		set
		{
			_urlCookies = value;
		}
	}

	public static string _pExitAppMessage
	{
		get
		{
			if (_exitAppMessage == null)
			{
				if (_pIsKillSwitchRaised)
				{
					if (!string.IsNullOrEmpty(AmuzoScriptableSingleton<AmuzoLEGOFacade>._pInstance._exitAppMessageKillSwitchKey))
					{
						return Facades<LocalisationFacade>.Instance.GetString(AmuzoScriptableSingleton<AmuzoLEGOFacade>._pInstance._exitAppMessageKillSwitchKey);
					}
				}
				else if (_pIsMinVersionWrong && !string.IsNullOrEmpty(AmuzoScriptableSingleton<AmuzoLEGOFacade>._pInstance._exitAppMessageMinVersionKey))
				{
					return Facades<LocalisationFacade>.Instance.GetString(AmuzoScriptableSingleton<AmuzoLEGOFacade>._pInstance._exitAppMessageMinVersionKey);
				}
			}
			return _exitAppMessage;
		}
		set
		{
			_exitAppMessage = value;
		}
	}

	public static bool CheckAndReportKillSwitch(bool killswitch, Action<string> onLogMessage, string killSwitchExitMessage)
	{
		_pIsKillSwitchRaised = killswitch;
		if (_pIsKillSwitchRaised)
		{
			_pExitAppMessage = killSwitchExitMessage;
		}
		onLogMessage("KillSwitch raised: " + _pIsKillSwitchRaised);
		return !_pIsKillSwitchRaised;
	}

	public static bool CheckAndReportMinVersion(string minVersion, Action<string> onLogMessage, string oldVersionExitMessage)
	{
		string[] array = minVersion.Split('.');
		string pBundleVersion = AnvilBuildInfo._pBundleVersion;
		string[] array2 = pBundleVersion.Split('.');
		int num = 0;
		int num2 = 0;
		bool flag = false;
		onLogMessage("Comparing version in build to version in config data - Build: " + pBundleVersion + " - Config: " + minVersion.ToString());
		for (int i = 0; i < array.Length && i < array2.Length; i++)
		{
			num = int.Parse(array[i]);
			num2 = int.Parse(array2[i]);
			if (num2 < num)
			{
				flag = true;
			}
			else if (num2 > num)
			{
				break;
			}
		}
		if (flag)
		{
			_pIsMinVersionWrong = true;
			_pExitAppMessage = oldVersionExitMessage;
			onLogMessage("Verson is OLD and so INVALID");
		}
		else
		{
			_pIsMinVersionWrong = false;
			onLogMessage("Version VALID");
		}
		return !flag;
	}

	protected override void Initialise()
	{
		base.Initialise();
		_pUseCrossPromo = _useCrossPromoDefault;
	}
}
