using System;
using UnityEngine;

[Serializable]
public class AreaMapNode
{
	[SerializeField]
	public string levelName = string.Empty;

	[SerializeField]
	public int row;

	[SerializeField]
	public int col;

	[SerializeField]
	public int styleIndex;

	[SerializeField]
	public int themeCollectionIndex;

	[SerializeField]
	public bool isPlayed;

	[SerializeField]
	public bool isUnlocked;

	[SerializeField]
	public bool isBlocked;

	[SerializeField]
	public int lockUntilChapter;

	[SerializeField]
	public ElementalPower primaryPower;

	[SerializeField]
	public ElementalPower secondaryPower;

	[SerializeField]
	public LevelStyle.e_LevelGroundType groundTypeOverwrite;

	[SerializeField]
	public int environmentSettingOverwrite = -1;
}
