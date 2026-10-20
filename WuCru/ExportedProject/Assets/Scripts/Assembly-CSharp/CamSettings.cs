using System;
using UnityEngine;

[Serializable]
public class CamSettings
{
	[SerializeField]
	public Vector3 positionOffset;

	[SerializeField]
	public Vector3 lookAtOffset;

	[SerializeField]
	public float dist;

	[SerializeField]
	public float interpolationSpeed;

	[SerializeField]
	public float transitionOutTime;
}
