using System.Collections;
using Cape.LegoSdk.Core;
using LEGO.CoreSDK.Legal.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LegoCoreSdkLegal : MonoBehaviour
{
	public static string previousScene = string.Empty;

	private PolicyBehaviour _policyBehaviour;

	private bool _policyBehaviourFound;

	private void Awake()
	{
		LegoCoreSdk.Instance.ShowAllPolicies();
	}

	private void Start()
	{
		StartCoroutine(GetPolicyBehaviour());
	}

	private void Update()
	{
		if (_policyBehaviourFound && _policyBehaviour == null && !string.IsNullOrEmpty(previousScene))
		{
			SceneManager.LoadScene(previousScene);
		}
	}

	private IEnumerator GetPolicyBehaviour()
	{
		_policyBehaviourFound = false;
		while (!_policyBehaviourFound)
		{
			_policyBehaviour = Object.FindObjectOfType<PolicyBehaviour>();
			if (_policyBehaviour == null)
			{
				yield return 0;
			}
			else
			{
				_policyBehaviourFound = true;
			}
		}
	}
}
