using System.Collections;
using System.Linq;
using DentedPixel.LTEditor;
using Fabric;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeBox : MonoBehaviour
{
	[SerializeField]
	private GameObject studImage;

	[SerializeField]
	private GameObject challengeCompleteEFX;

	[SerializeField]
	private Image imageBoxObject;

	[SerializeField]
	private LeanTweenVisual animIn;

	[SerializeField]
	private LeanTweenVisual animOut;

	[SerializeField]
	private ImageBlinker imageBoxBlinker;

	[SerializeField]
	private Image imageChallengeObject;

	[SerializeField]
	private ImageBlinker imageChallengeBlinker;

	[SerializeField]
	private Text textCounter;

	[SerializeField]
	private LeanTweenVisual onCounterUpdateAnim;

	[SerializeField]
	private Image imageCheckmark;

	[SerializeField]
	private ImageBlinker imageCheckmarkBlinker;

	[SerializeField]
	private LeanTweenVisual onCheckmarkAnimIn;

	[SerializeField]
	private Image imageFailed;

	[SerializeField]
	private GameObject dummyPosHolder;

	private bool isInit;

	private StudsCounter studsCounter;

	private bool flyToPos;

	private int studsTotal;

	private int studsCompleted;

	public void Start()
	{
		Initialize();
	}

	public void Initialize()
	{
		if (!isInit)
		{
			isInit = true;
			studsCounter = Object.FindObjectOfType<StudsCounter>();
			animIn = Object.Instantiate(animIn.gameObject).GetComponent<LeanTweenVisual>();
			animIn.transform.parent = base.transform;
			animIn.playOnStart = false;
			animOut = Object.Instantiate(animOut.gameObject).GetComponent<LeanTweenVisual>();
			animOut.transform.parent = base.transform;
			animOut.playOnStart = false;
			onCounterUpdateAnim.playOnStart = false;
			imageCheckmark.gameObject.SetActive(false);
			imageFailed.gameObject.SetActive(false);
			textCounter.gameObject.SetActive(false);
		}
	}

	public void AnimIn(float delay = 0f)
	{
		StartCoroutine(DoAnimIn(delay));
	}

	private IEnumerator DoAnimIn(float delay)
	{
		base.gameObject.transform.localScale = Vector3.zero;
		yield return new WaitForSeconds(delay);
		LeanTweenGroup toPlace = animIn.groupList.Where((LeanTweenGroup g) => g.name == "ToPlace").First();
		toPlace.itemList[0].to = dummyPosHolder.transform.position;
		animIn.start(base.gameObject);
		LTDescr[] descr = LeanTween.descriptions(base.gameObject);
		float longest = 0f;
		LTDescr lastDescr = null;
		for (int num = 0; num < descr.Length; num++)
		{
			float num2 = descr[num].delay + descr[num].time;
			if (num2 > longest)
			{
				lastDescr = descr[num];
			}
		}
		if (lastDescr != null)
		{
			lastDescr.setOnComplete(HandleOnAnimIn);
		}
		yield return new WaitForSeconds(2.15f);
		AudioPlayer.PlaySound("fx/sub/goal/countup");
		imageBoxBlinker.Blink(Color.white, 0.3f);
	}

	private void HandleOnAnimIn()
	{
		flyToPos = true;
	}

	public void Update()
	{
		if (flyToPos)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, dummyPosHolder.transform.position, 0.2f);
		}
	}

	public void SetChallengeSprite(Sprite sprite)
	{
		imageChallengeObject.sprite = sprite;
	}

	public void SetChallengeCounter(int counter, int max, bool animate = true)
	{
		if (!textCounter.gameObject.activeSelf)
		{
			textCounter.gameObject.SetActive(true);
		}
		textCounter.text = counter + "/" + max;
		if (animate)
		{
			imageBoxBlinker.Blink(Color.white, 0.3f);
			imageChallengeBlinker.Blink(Color.black, 0.3f);
			onCounterUpdateAnim.start(textCounter.gameObject);
		}
		if (counter > 0)
		{
			AudioPlayer.PlaySound("fx/sub/goal/countup");
		}
	}

	public void SetChallengeComplete()
	{
		StartCoroutine(ChallengeCompleteSequence());
	}

	private IEnumerator ChallengeCompleteSequence()
	{
		AudioPlayer.PlaySound("fx/sub/goal/reached");
		imageCheckmark.gameObject.SetActive(false);
		textCounter.gameObject.SetActive(false);
		GameObject particleEffect = Object.Instantiate(challengeCompleteEFX);
		particleEffect.transform.parent = base.transform;
		particleEffect.transform.localPosition = Vector3.zero;
		particleEffect.transform.localScale = Vector3.one;
		imageBoxBlinker.Blink(Color.white, 0.6f);
		imageChallengeBlinker.Blink(Color.black, 0.6f);
		animOut.start();
		yield return new WaitForSeconds(0.2f);
		SpawnStuds(10);
		yield return new WaitForSeconds(0.2f);
		SpawnStuds(10);
		yield return new WaitForSeconds(0.6f);
		imageCheckmark.gameObject.SetActive(true);
		imageCheckmarkBlinker.Blink(Color.white, 0.2f);
		onCheckmarkAnimIn.start();
		yield return new WaitForSeconds(0.5f);
		yield return new WaitForSeconds(1.5f);
		Object.Destroy(base.gameObject);
	}

	private void SpawnStuds(int amount)
	{
		float num = 80f;
		float num2 = 20f;
		studImage.SetActive(true);
		studsTotal = amount;
		studsCompleted = 0;
		Vector3 localPosition = base.transform.localPosition;
		Vector3 localPosition2 = studsCounter.transform.localPosition;
		for (int i = 0; i < amount; i++)
		{
			GameObject gameObject = Object.Instantiate(studImage);
			gameObject.transform.parent = studImage.transform.parent;
			gameObject.transform.localScale = Vector3.zero;
			gameObject.transform.localPosition = localPosition;
			float num3 = Random.Range(0f, 0.3f);
			LeanTween.scale(gameObject, Vector3.one, 0.3f).setDelay(num3);
			LeanTween.moveLocal(gameObject, localPosition + new Vector3(Random.Range(0f - num, num), Random.Range(0f - num, num), 0f), 0.5f).setDelay(num3).setEase(LeanTweenType.easeOutQuart);
			num3 += 1f + Random.Range(0f, 0.3f);
			LeanTween.scale(gameObject, Vector3.one * 1.5f, 0.3f).setDelay(num3);
			LeanTween.scale(gameObject, Vector3.one, 0.3f).setDelay(num3 + 0.7f);
			LeanTween.moveLocalX(gameObject, localPosition2.x + Random.Range(0f - num2, num2), 1f).setDelay(num3).setEase(LeanTweenType.easeInOutSine);
			LeanTween.moveLocalY(gameObject, Random.Range(-100f, -150f), 0.5f).setDelay(num3).setEase(LeanTweenType.easeOutSine);
			LeanTween.moveLocalY(gameObject, localPosition2.y + Random.Range(0f - num2, num2), 0.5f).setDelay(num3 + 0.5f).setEase(LeanTweenType.easeInSine)
				.setOnComplete(OnStudReachTarget, gameObject);
		}
		studImage.SetActive(false);
	}

	public void OnStudReachTarget(object stud)
	{
		Object.Destroy(stud as GameObject);
		studsCounter.Pickup();
		studsCompleted++;
		float num = (float)studsTotal / (float)studsCompleted;
		EventManager.Instance.PostEvent("props/stud/pickup", EventAction.SetPitchProperty, 1f + num);
		AudioPlayer.PlaySound("props/stud/pickup");
	}

	public void SetChallengeFailed()
	{
		imageFailed.gameObject.SetActive(true);
		textCounter.gameObject.SetActive(false);
	}

	private void OnDestroy()
	{
		Object.Destroy(dummyPosHolder);
	}
}
