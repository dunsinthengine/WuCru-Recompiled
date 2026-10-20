using System;
using LEGO;
using LEGO.CoreSDK;
using LEGO.CoreSDK.Legal;
using LEGO.CoreSDK.VPC;
using UnityEngine;

namespace Cape.LegoSdk.Core
{
	public class LegoCoreSdk
	{
		private static LegoCoreSdk _instance;

		private const string PLATFORM_IOS = "ios";

		private const string PLATFORM_ANDROID = "android";

		private const string PLATFORM_WEB = "web";

		private const string PLATFORM_KEY = "{type}";

		private const string CONFIG_URL_TEMPLATE = "https://lego.com/go/21/1/{type}";

		private const string ARCADE_URL_DEFAULT = "https://mi-od-live-s.legocdn.com/FranchiseSites/Ninjago/NinjagoMovieGame/v8/index.html";

		private const string KEY_CUSTOMVALUE_ARCADEURL = "gameurl";

		public static LegoCoreSdk Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new LegoCoreSdk();
				}
				if (_instance.State == CoreSdkState.None)
				{
					_instance.Initialize();
				}
				return _instance;
			}
		}

		public string _pArcadeMachineUrl
		{
			get
			{
				if (State == CoreSdkState.Initialized && LEGOSDK.ApplicationConfigurationService.ApplicationConfiguration.CustomValues != null && LEGOSDK.ApplicationConfigurationService.ApplicationConfiguration.CustomValues.ContainsKey("gameurl"))
				{
					return LEGOSDK.ApplicationConfigurationService.ApplicationConfiguration.CustomValues["gameurl"];
				}
				return "https://mi-od-live-s.legocdn.com/FranchiseSites/Ninjago/NinjagoMovieGame/v8/index.html";
			}
		}

		public CoreSdkState State { get; private set; }

		public LegoCoreSdkConfig Config { get; private set; }

		public Version AppVersion
		{
			get
			{
				return LEGOSDK.ApplicationConfigurationService.ApplicationConfiguration.Version;
			}
		}

		public string LegoIdConfigurationUrl
		{
			get
			{
				JSONObject rawJson = LEGOSDK.ApplicationConfigurationService.ApplicationConfiguration.RawJson;
				JSONObject jSONObject = rawJson["configuration"]["commonconfigurations"]["legoid"]["endpoints"]["legoidconfiguration"];
				return jSONObject.str;
			}
		}

		public string Experience
		{
			get
			{
				JSONObject rawJson = LEGOSDK.ApplicationConfigurationService.ApplicationConfiguration.RawJson;
				JSONObject jSONObject = rawJson["configuration"]["experience"];
				return jSONObject.str;
			}
		}

		public static event EventHandler<EventArgs> ConfigurationChangedEvent;

		private LegoCoreSdk()
		{
			State = CoreSdkState.None;
		}

		~LegoCoreSdk()
		{
			LEGOSDK.ApplicationConfigurationService.ApplicationConfigurationChanged -= OnCoreAppConfigChanged;
		}

		public void Initialize()
		{
			if (State <= CoreSdkState.None)
			{
				State = CoreSdkState.Initializing;
				LogState();
				string _configUrl = "https://lego.com/go/21/1/{type}";
				Config = (LegoCoreSdkConfig)Resources.Load("LegoSdk/LegoCoreSdkConfig");
				SanityCheckCoreConfig(Config);
				SetLogLevel(Config.LogLevel);
				_configUrl = _configUrl.Replace("{type}", GetPlatform());
				LEGOSDK.Initialize((CoreConfiguration config) =>
				{
					config.ApplicationConfigurationURL = _configUrl;
					config.SupportedLocales = Config.SupportedLocales;
					config.FallbackLocale = Config.FallbackLocale;
				});
				LEGOSDK.ApplicationConfigurationService.ApplicationConfigurationChanged += OnCoreAppConfigChanged;
				State = CoreSdkState.Initialized;
				if (IsKillSwitchActive())
				{
					State = CoreSdkState.KillSwitch;
				}
				if (IsAppVersionOutDated())
				{
					State = CoreSdkState.Version;
				}
				LogState();
			}
		}

		private string GetPlatform()
		{
			return "android";
		}

		public void SetLogLevel(LogLevel level)
		{
			LEGOSDK.Log.LogLevel = level;
		}

		public void ShowAllPolicies()
		{
			LEGOSDK.Policies.Show(() =>
			{
				Debug.Log("[CapeLegoSdk] All policies popup closed");
			});
		}

		public void ShowCookiePolicy()
		{
			LEGOSDK.Policies.Show(() =>
			{
				Debug.LogError("[CapeLegoSdk] Cookie policy popup closed");
			}, PolicyType.Cookie);
		}

		public void ShowPrivacyPolicy()
		{
			LEGOSDK.Policies.Show(() =>
			{
				Debug.Log("[CapeLegoSdk] Privacy policy popup closed");
			}, PolicyType.Privacy);
		}

		public void ShowTermsPolicy()
		{
			LEGOSDK.Policies.Show(() =>
			{
				Debug.Log("[CapeLegoSdk] Terms of service popup closed\t");
			}, PolicyType.Terms);
		}

		private void SanityCheckCoreConfig(LegoCoreSdkConfig configAsset)
		{
			if (configAsset == null)
			{
				HandleError("[CapeLegoSdk] Configuration asset is null");
			}
			if (string.IsNullOrEmpty(configAsset.ApplicationConfigurationUrl))
			{
				HandleError("[CapeLegoSdk] No application configuration URL specified");
			}
			if (configAsset.SupportedLocales == null || configAsset.SupportedLocales.Length == 0)
			{
				HandleError("[CapeLegoSdk] No supported locales specified");
			}
			if (string.IsNullOrEmpty(configAsset.FallbackLocale))
			{
				HandleError("[CapeLegoSdk] No fallback locale specified");
			}
			if (configAsset.AlertView != null && !(configAsset.AlertView is IAlert))
			{
				HandleError("[CapeLegoSdk] Alert view does not implement IAlert");
			}
			if (configAsset.PolicyView != null && !(configAsset.PolicyView is IPolicyView))
			{
				HandleError("[CapeLegoSdk] Policy view does not implement IPolicyView");
			}
			if (configAsset.ParentalGateView != null && !(configAsset.ParentalGateView is IParentalGate))
			{
				HandleError("[CapeLegoSdk] Parental gate view does not implement IParentalGate");
			}
		}

		private void HandleError(string message)
		{
			throw new Exception(message);
		}

		public void LogState()
		{
			switch (State)
			{
			case CoreSdkState.None:
				Debug.Log("[CapeLegoSdk] LEGO Core SDK has not started initializing");
				break;
			case CoreSdkState.Initializing:
				Debug.Log("[CapeLegoSdk] Initializing LEGO Core SDK");
				break;
			case CoreSdkState.Initialized:
				Debug.Log("[CapeLegoSdk] LEGO Core SDK initialized");
				break;
			case CoreSdkState.KillSwitch:
				Debug.Log("[CapeLegoSdk] Kill switch is on");
				break;
			case CoreSdkState.Version:
				Debug.Log("[CapeLegoSdk] App version is outdated");
				break;
			default:
				Debug.Log("[CapeLegoSdk] Unknown SDK state : " + State);
				break;
			}
		}

		public bool IsKillSwitchActive()
		{
			return false;
		}

		public bool IsAppVersionOutDated(Version requiredVersion = null)
		{
			return false;
		}

		public void OnCoreAppConfigChanged(IApplicationConfiguration config)
		{
			State = CoreSdkState.Initialized;
			Debug.Log("[CapeLegoSdk] LEGO Core SDK config changed");
			LogState();
			if (ConfigurationChangedEvent != null)
			{
				ConfigurationChangedEvent(this, null);
			}
		}
	}
}
