using UnityEngine;

public class SwordTappingSettings
{
	public float graceWindow = 0.3f;

	public float beginWindow = 0.6f;

	public float endWindow = 0.95f;

	public float duration = 0.95f;

	public float perfectBeginWindow = 0.7f;

	public float perfectEndWindows = 0.8f;

	public static SwordTappingSettings GetRandomSettings()
	{
		if (GameController.Instance.PlayerProfile.Level >= 4)
		{
			SwordTappingSettings swordTappingSettings = new SwordTappingSettings();
			swordTappingSettings.beginWindow = Random.Range(0.2f, 0.5f);
			swordTappingSettings.endWindow = swordTappingSettings.beginWindow + Random.Range(0.3f, 0.5f);
			swordTappingSettings.duration = swordTappingSettings.endWindow;
			swordTappingSettings.perfectBeginWindow = swordTappingSettings.beginWindow + 0.2f;
			swordTappingSettings.perfectEndWindows = swordTappingSettings.perfectBeginWindow + 0.1f;
			return swordTappingSettings;
		}
		if (GameController.Instance.PlayerProfile.Level >= 2)
		{
			SwordTappingSettings swordTappingSettings2 = new SwordTappingSettings();
			swordTappingSettings2.beginWindow = Random.Range(0.2f, 0.3f);
			swordTappingSettings2.endWindow = swordTappingSettings2.beginWindow + Random.Range(0.4f, 0.6f);
			swordTappingSettings2.duration = swordTappingSettings2.endWindow;
			swordTappingSettings2.perfectBeginWindow = swordTappingSettings2.beginWindow + 0.2f;
			swordTappingSettings2.perfectEndWindows = swordTappingSettings2.perfectBeginWindow + 0.1f;
			return swordTappingSettings2;
		}
		SwordTappingSettings swordTappingSettings3 = new SwordTappingSettings();
		swordTappingSettings3.beginWindow = Random.Range(0.2f, 0.3f);
		swordTappingSettings3.endWindow = swordTappingSettings3.beginWindow + Random.Range(0.5f, 0.7f);
		swordTappingSettings3.duration = swordTappingSettings3.endWindow;
		swordTappingSettings3.perfectBeginWindow = swordTappingSettings3.beginWindow + 0.2f;
		swordTappingSettings3.perfectEndWindows = swordTappingSettings3.perfectBeginWindow + 0.1f;
		return swordTappingSettings3;
	}
}
