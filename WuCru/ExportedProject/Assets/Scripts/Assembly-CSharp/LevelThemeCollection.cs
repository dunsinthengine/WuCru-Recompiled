using System;
using UnityEngine;

[Serializable]
public class LevelThemeCollection : ScriptableObject
{
	[SerializeField]
	public string collectionName = string.Empty;

	[SerializeField]
	public DictionaryStringLevelTheme themes = new DictionaryStringLevelTheme();
}
