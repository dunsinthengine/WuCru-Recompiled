using System;
using UnityEngine;

[Serializable]
public class LevelExtensionData
{
	[SerializeField]
	public DictionaryMapExtensionIndex mapSeedIndex = new DictionaryMapExtensionIndex();

	[SerializeField]
	public int progress;
}
