using LEGO.CoreSDK;
using UnityEngine;

namespace Cape.LegoSdk.Core
{
	public class LegoCoreSdkConfig : ScriptableObject
	{
		public string ApplicationConfigurationUrl;

		public string[] SupportedLocales;

		public string FallbackLocale;

		public MonoBehaviour AlertView;

		public MonoBehaviour PolicyView;

		public MonoBehaviour ParentalGateView;

		public LogLevel LogLevel;
	}
}
