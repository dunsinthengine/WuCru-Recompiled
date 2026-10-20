using UnityEngine;
using UnityEngine.UI;

public class DebugGUI : MonoBehaviour
{
	public Canvas debugCanvas;

	public Text levelInfo;

	private void Awake()
	{
		base.enabled = false;
	}

	public void SetLevelInfo(int seed, string styleName, string themeCollectionName, string themeSequenceName)
	{
		levelInfo.text = "Seed: " + seed + ", Style: " + styleName + ", ThemeCollection: " + themeCollectionName + ", Theme Sequence: " + themeSequenceName;
	}

	private void Update()
	{
	}
}
