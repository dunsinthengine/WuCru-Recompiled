using UnityEngine;

public class InterestPoint : MonoBehaviour
{
	private InGameCam inGameCam;

	private void Awake()
	{
		inGameCam = InGameCam._pInstance;
	}

	private void Update()
	{
		if (inGameCam == null)
		{
			inGameCam = InGameCam._pInstance;
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		Character component = collider.GetComponent<Character>();
		if (component != null && component.isPlayer)
		{
			inGameCam.OnInterestPointEnter(this);
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		Character component = collider.GetComponent<Character>();
		if (component != null && component.isPlayer)
		{
			inGameCam.OnInterestPointExit(this);
		}
	}
}
