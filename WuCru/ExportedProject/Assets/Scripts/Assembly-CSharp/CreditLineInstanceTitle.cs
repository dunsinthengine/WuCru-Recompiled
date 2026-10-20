using UnityEngine.UI;

public class CreditLineInstanceTitle : CreditLineInstance
{
	public override void setVisual(string data, string data2)
	{
		GetComponent<Text>().text = data;
	}

	public override void fadeIn(float time)
	{
		GetComponent<Text>().CrossFadeAlpha(1f, time, false);
	}

	public override void fadeOut(float time)
	{
		GetComponent<Text>().CrossFadeAlpha(0f, time, false);
	}
}
