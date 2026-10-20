using System;
using UnityEngine;

public class UpdateForwarder : MonoBehaviour
{
	public Action OnUpdateFunction;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Update()
	{
		if (OnUpdateFunction != null)
		{
			OnUpdateFunction();
		}
	}
}
