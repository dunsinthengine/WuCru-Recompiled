using UnityEngine;

public class ExamplePlayControl : MonoBehaviour
{
	public bool _skipVideo;

	private bool _started;

	private void Start()
	{
		if (!ManagerCrossPromo._pInstance._pHasStarted)
		{
			ManagerCrossPromo._pInstance.SetUp();
		}
	}

	private void Update()
	{
	}
}
