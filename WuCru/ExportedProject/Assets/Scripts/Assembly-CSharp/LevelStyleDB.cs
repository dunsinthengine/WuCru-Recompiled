using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelStyleDB : ScriptableObject
{
	[SerializeField]
	public List<LevelStyle> _styles = new List<LevelStyle>();
}
