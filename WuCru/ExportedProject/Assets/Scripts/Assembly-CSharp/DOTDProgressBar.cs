using System.Collections;
using DentedPixel.LTEditor;
using UnityEngine;
using UnityEngine.UI;

public class DOTDProgressBar : MonoBehaviour
{
	[SerializeField]
	private GameObject challengeCompleteEFX;

	[SerializeField]
	private LeanTweenVisual mainAnimIn;

	[SerializeField]
	private LeanTweenVisual mainAnimOut;

	[SerializeField]
	private Image fill;

	[SerializeField]
	private Image fillIncrease;

	[SerializeField]
	private Image frame;

	[SerializeField]
	private LeanTweenVisual lloydAnimOut;

	private ImageBlinker fillBlinker;

	private ImageBlinker fillIncreaseBlinker;

	private ImageBlinker frameBlinker;

	private ImageBlinker lloydBlinker;

	private ScreenFlash screenFlash;

	private int[] que;

	private bool isPlaying;

	private int progress;

	private void Awake()
	{
		fillBlinker = fill.GetComponent<ImageBlinker>();
		fillIncreaseBlinker = fillIncrease.GetComponent<ImageBlinker>();
		frameBlinker = frame.GetComponent<ImageBlinker>();
		lloydBlinker = lloydAnimOut.gameObject.GetComponent<ImageBlinker>();
		fill.fillAmount = 0f;
		fillIncrease.fillAmount = 0f;
		Show(false);
	}

	private void Start()
	{
		screenFlash = Object.FindObjectOfType<ScreenFlash>();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.I))
		{
			IncreaseCounter(progress, ++progress, 30);
		}
		if (Input.GetKeyDown(KeyCode.O))
		{
			IncreaseCounter(29, 30, 30);
		}
	}

	public void IncreaseCounter(int from, int to, int total, float delay = 0f)
	{
		if (isPlaying)
		{
			que = new int[3];
			que[0] = from;
			que[1] = to;
			que[2] = total;
		}
		else
		{
			StartCoroutine(DoIncreaseCounterEffect((float)from / (float)total, (float)to / (float)total, delay));
		}
	}

	private IEnumerator DoIncreaseCounterEffect(float from, float to, float delay)
	{
		yield return new WaitForSeconds(delay);
		isPlaying = true;
		fill.fillAmount = from;
		fillIncrease.fillAmount = 0f;
		SetFromValues(mainAnimIn, base.gameObject);
		Show(true);
		frameBlinker.Blink(Color.black, 0.3f);
		mainAnimIn.start(base.gameObject);
		yield return new WaitForSeconds(1f);
		float time = from;
		float startTime = Time.realtimeSinceStartup;
		float progress = 0f;
		bool blinkActivated = false;
		yield return new WaitForSeconds(0.2f);
		fillIncrease.fillAmount = from;
		fillIncreaseBlinker.SetBlinkValue(Color.white, 1f);
		AudioPlayer.PlaySound("fx/sub/goal/countup");
		time = 0.6f;
		startTime = Time.realtimeSinceStartup;
		progress = 0f;
		blinkActivated = false;
		while (progress < 1f)
		{
			progress = Mathf.Clamp01((Time.realtimeSinceStartup - startTime) / time);
			fillIncrease.fillAmount = Mathf.Lerp(from, to, Easing.CubicEaseOut(progress));
			if (progress > 0.5f && !blinkActivated)
			{
				blinkActivated = true;
				fillIncreaseBlinker.Blink(Color.white, 0.3f);
			}
			yield return new WaitForEndOfFrame();
		}
		fillIncreaseBlinker.Blink(Color.white, 0.1f);
		if (to >= 1f)
		{
			lloydAnimOut.start();
			yield return new WaitForSeconds(0.6f);
			AudioPlayer.PlaySound("fx/sub/goal/reached");
			GameObject particleEffect = Object.Instantiate(challengeCompleteEFX);
			particleEffect.transform.parent = lloydAnimOut.transform;
			particleEffect.transform.localPosition = Vector3.zero;
			particleEffect.transform.localScale = Vector3.one;
			yield return new WaitForSeconds(0.6f);
		}
		yield return new WaitForSeconds(2f);
		mainAnimOut.start(base.gameObject);
		frameBlinker.BlinkReverse(Color.black, 0.2f);
		yield return new WaitForSeconds(0.2f);
		Show(false);
		isPlaying = false;
		if (que != null)
		{
			IncreaseCounter(que[0], que[1], que[2]);
			que = null;
		}
	}

	public void SetFromValues(LeanTweenVisual tween, GameObject gameobject)
	{
		for (int i = 0; i < tween.groupList.Count; i++)
		{
			LeanTweenGroup leanTweenGroup = tween.groupList[i];
			if (leanTweenGroup.delay != 0f)
			{
				continue;
			}
			for (int j = 0; j < leanTweenGroup.itemList.Count; j++)
			{
				LeanTweenItem leanTweenItem = leanTweenGroup.itemList[j];
				if (leanTweenItem.delay == 0f)
				{
					switch (leanTweenItem.action)
					{
					case TweenAction.MOVE_LOCAL:
						gameobject.transform.localPosition = leanTweenItem.from;
						break;
					case TweenAction.SCALE:
						gameobject.transform.localScale = leanTweenItem.from;
						break;
					}
				}
			}
		}
	}

	public void Show(bool value)
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			base.transform.GetChild(i).gameObject.SetActive(value);
		}
	}
}
