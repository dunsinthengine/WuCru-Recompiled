using System;
using System.Collections.Generic;
using LEGO.CoreSDK;
using LEGO.CoreSDK.Legal;
using UnityEngine;

namespace Cape.LegoSdk.Core.View
{
	public class LegoSdkPolicyViewProxy<T> : IPolicyView where T : new()
	{
		public IPolicyView Implementation { private get; set; }

		public LegoSdkPolicyViewProxy()
		{
			Implementation = (IPolicyView)(object)new T();
		}

		public void Show(Func<Locale, LegalPolicies> getPolicies, Locale currentLocale, IEnumerable<Locale> supportedLocales, PolicyType? policyType, Action completionHandler)
		{
			Implementation.Show(getPolicies, currentLocale, supportedLocales, policyType, () =>
			{
				Debug.Log("All policies popup closed");
			});
		}

		public void Close()
		{
			Implementation.Close();
		}
	}
}
