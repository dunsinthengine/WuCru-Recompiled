using UnityEngine;
using UnityEngine.UI;

public class CreditLineInstanceTwoColumns : CreditLineInstance
{
	public GameObject header;

	public GameObject text;

	public override void setVisual(string data, string data2)
	{
		header.GetComponent<Text>().text = data;
		text.GetComponent<Text>().text = data2;
	}

	public override void fadeIn(float time)
	{
		header.GetComponent<Text>().CrossFadeAlpha(1f, time, false);
		text.GetComponent<Text>().CrossFadeAlpha(1f, time, false);
	}

	public override void fadeOut(float time)
	{
		header.GetComponent<Text>().CrossFadeAlpha(0f, time, false);
		text.GetComponent<Text>().CrossFadeAlpha(0f, time, false);
	}
}
