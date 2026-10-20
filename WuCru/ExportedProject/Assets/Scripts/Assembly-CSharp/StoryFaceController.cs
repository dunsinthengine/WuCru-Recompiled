using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StoryFaceController : MonoBehaviour
{
	public Image imageFace;

	public Sprite wuNeutral;

	public Sprite wuSurprised;

	public Sprite wuHappy;

	public Sprite wuSad;

	public Sprite wuAngry;

	public Sprite wuCorrupted;

	public Sprite kaiNeutral;

	public Sprite kaiSurprised;

	public Sprite kaiHappy;

	public Sprite kaiSad;

	public Sprite kaiAngry;

	public Sprite kaiCorrupted;

	public Sprite jayNeutral;

	public Sprite jaySurprised;

	public Sprite jayHappy;

	public Sprite jaySad;

	public Sprite jayAngry;

	public Sprite jayCorrupted;

	public Sprite zaneNeutral;

	public Sprite zaneSurprised;

	public Sprite zaneHappy;

	public Sprite zaneSad;

	public Sprite zaneAngry;

	public Sprite zaneCorrupted;

	public Sprite coleNeutral;

	public Sprite coleSurprised;

	public Sprite coleHappy;

	public Sprite coleSad;

	public Sprite coleAngry;

	public Sprite coleCorrupted;

	public Sprite falconNeutral;

	public Sprite falconSurprised;

	public Sprite falconHappy;

	public Sprite falconSad;

	public Sprite falconAngry;

	public Sprite falconCorrupted;

	public Sprite lloydNeutral;

	public Sprite lloydSurprised;

	public Sprite lloydHappy;

	public Sprite lloydSad;

	public Sprite lloydAngry;

	public Sprite lloydCorrupted;

	public Sprite nyaNeutral;

	public Sprite nyaSurprised;

	public Sprite nyaHappy;

	public Sprite nyaSad;

	public Sprite nyaAngry;

	public Sprite nyaCorrupted;

	public Sprite pythorNeutral;

	public Sprite pythorSurprised;

	public Sprite pythorHappy;

	public Sprite pythorSad;

	public Sprite pythorAngry;

	public Sprite pythorCorrupted;

	public Sprite fangtomNeutral;

	public Sprite fangtomSurprised;

	public Sprite fangtomHappy;

	public Sprite fangtomSad;

	public Sprite fangtomAngry;

	public Sprite fangtomCorrupted;

	private Sprite[,] faces;

	private int numActors = 10;

	private int numExpressions = 6;

	private float scaleDuration = 0.15f;

	private StoryActor currentActor = StoryActor.Unknown;

	private StoryFaceMood currentMood;

	private bool isSettingFace;

	public bool IsSettingFace
	{
		get
		{
			return isSettingFace;
		}
	}

	private void Awake()
	{
		currentActor = StoryActor.Unknown;
		imageFace.transform.localScale = Vector3.zero;
		faces = new Sprite[numActors, numExpressions];
		faces[0, 0] = wuNeutral;
		faces[0, 1] = wuSurprised;
		faces[0, 2] = wuHappy;
		faces[0, 3] = wuSad;
		faces[0, 4] = wuAngry;
		faces[0, 5] = wuCorrupted;
		faces[1, 0] = kaiNeutral;
		faces[1, 1] = kaiSurprised;
		faces[1, 2] = kaiHappy;
		faces[1, 3] = kaiSad;
		faces[1, 4] = kaiAngry;
		faces[1, 5] = kaiCorrupted;
		faces[2, 0] = jayNeutral;
		faces[2, 1] = jaySurprised;
		faces[2, 2] = jayHappy;
		faces[2, 3] = jaySad;
		faces[2, 4] = jayAngry;
		faces[2, 5] = jayCorrupted;
		faces[3, 0] = zaneNeutral;
		faces[3, 1] = zaneSurprised;
		faces[3, 2] = zaneHappy;
		faces[3, 3] = zaneSad;
		faces[3, 4] = zaneAngry;
		faces[3, 5] = zaneCorrupted;
		faces[4, 0] = coleNeutral;
		faces[4, 1] = coleSurprised;
		faces[4, 2] = coleHappy;
		faces[4, 3] = coleSad;
		faces[4, 4] = coleAngry;
		faces[4, 5] = coleCorrupted;
		faces[5, 0] = falconNeutral;
		faces[5, 1] = falconSurprised;
		faces[5, 2] = falconHappy;
		faces[5, 3] = falconSad;
		faces[5, 4] = falconAngry;
		faces[5, 5] = falconCorrupted;
		faces[6, 0] = lloydNeutral;
		faces[6, 1] = lloydSurprised;
		faces[6, 2] = lloydHappy;
		faces[6, 3] = lloydSad;
		faces[6, 4] = lloydAngry;
		faces[6, 5] = lloydCorrupted;
		faces[7, 0] = nyaNeutral;
		faces[7, 1] = nyaSurprised;
		faces[7, 2] = nyaHappy;
		faces[7, 3] = nyaSad;
		faces[7, 4] = nyaAngry;
		faces[7, 5] = nyaCorrupted;
		faces[8, 0] = pythorNeutral;
		faces[8, 1] = pythorSurprised;
		faces[8, 2] = pythorHappy;
		faces[8, 3] = pythorSad;
		faces[8, 4] = pythorAngry;
		faces[8, 5] = pythorCorrupted;
		faces[9, 0] = fangtomNeutral;
		faces[9, 1] = fangtomSurprised;
		faces[9, 2] = fangtomHappy;
		faces[9, 3] = fangtomSad;
		faces[9, 4] = fangtomAngry;
		faces[9, 5] = fangtomCorrupted;
	}

	public void Hide()
	{
		imageFace.transform.localScale = Vector3.zero;
		currentActor = StoryActor.Unknown;
	}

	public void SetFace(StoryActor actor, StoryFaceMood mood)
	{
		StartCoroutine(DoSetFace(actor, mood));
	}

	private IEnumerator DoSetFace(StoryActor actor, StoryFaceMood mood)
	{
		if (currentActor != actor)
		{
			isSettingFace = true;
			float startTime = Time.time;
			if (currentActor != StoryActor.Unknown)
			{
				while (startTime + scaleDuration >= Time.time)
				{
					float t = (Time.time - startTime) / scaleDuration;
					imageFace.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
					yield return 0;
				}
			}
			imageFace.transform.localScale = Vector3.zero;
			imageFace.sprite = faces[(int)actor, (int)mood];
			currentActor = actor;
			currentMood = mood;
			startTime = Time.time;
			while (startTime + scaleDuration >= Time.time)
			{
				float t2 = (Time.time - startTime) / scaleDuration;
				imageFace.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t2);
				yield return 0;
			}
			imageFace.transform.localScale = Vector3.one;
			isSettingFace = false;
		}
		else if (currentActor == actor && currentMood != mood)
		{
			imageFace.sprite = faces[(int)actor, (int)mood];
			currentMood = mood;
			yield return 0;
		}
		else
		{
			yield return 0;
		}
	}
}
