using Cape.LegoSdk.Core;
using UnityEngine;

public class LegalPoliciesGUI : MonoBehaviour
{
	public static string returnToSceneName = string.Empty;

	private void Awake()
	{
		LegoCoreSdk.Instance.ShowAllPolicies();
	}
}
