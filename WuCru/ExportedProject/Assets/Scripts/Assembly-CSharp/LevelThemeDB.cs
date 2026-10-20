using System;
using UnityEngine;

[Serializable]
public class LevelThemeDB : ScriptableObject
{
	[SerializeField]
	public DictionaryStringLevelThemeCollection themeCollections = new DictionaryStringLevelThemeCollection();
}
