using System;
using UnityEngine;

[Serializable]
public class LevelUnlockData
{
	[SerializeField]
	public int seed;

	[SerializeField]
	public int season;

	[SerializeField]
	public bool starCompletion;

	[SerializeField]
	public bool starObjects;

	[SerializeField]
	public bool starChallenges;
}
