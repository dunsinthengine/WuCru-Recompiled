using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cape.Localization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoryViewer : MonoBehaviour
{
	public delegate void OnChapterEndDelegate();

	public StorySpeechBubble speechBubble;

	public StoryBackgroundController storyBackground;

	public StoryOverlayController storyOverlay;

	public Button buttonExit;

	public MenuCharacter character;

	private bool isShowing;

	public static OnChapterEndDelegate OnChapterEnd;

	private StoryBackground oldBackground;

	private StoryOverlay oldOverlay;

	private StoryActor oldActor = StoryActor.Jay;

	private StoryFaceMood oldMood;

	private int oldAnimIndex = -1;

	private Animator characterAnimator;

	private GameObject falconGO;

	public bool IsShowing
	{
		get
		{
			return isShowing;
		}
	}

	private void Awake()
	{
		AnalyticsEvents.LogLoadScene("StoryViewer");
		buttonExit.onClick.AddListener(() =>
		{
			AudioPlayer.PlaySound("ui/button/back/click");
			Done(true);
		});
	}

	private void Start()
	{
		int lastPlayedSeason = GameController.Instance.PlayerProfile.LastPlayedSeason;
		int chapterCursor = GameController.Instance.PlayerProfile.GetChapterCursor(lastPlayedSeason);
		AnalyticsEvents.LogStoryViewerStart(lastPlayedSeason, chapterCursor);
		StartCoroutine(DoShowStory(lastPlayedSeason, chapterCursor));
	}

	private IEnumerator DoShowStory(int season, int chapter)
	{
		isShowing = true;
		List<string> lines = GetSeasonChapterFromFile(season, chapter);
		for (int i = 0; i < lines.Count; i++)
		{
			string[] lineItems = lines[i].Split('\t');
			StoryBackground currentBackGround = BackgroundFromText(lineItems[3]);
			StoryOverlay currentOverlay = OverlayFromText(lineItems[4]);
			if (currentBackGround != oldBackground)
			{
				storyBackground.SetBackground(currentBackGround);
				oldBackground = currentBackGround;
			}
			if (currentOverlay != oldOverlay)
			{
				storyOverlay.SetOverlay(currentOverlay);
				oldOverlay = currentOverlay;
			}
			StoryActor myActor = ActorFromText(lineItems[5]);
			StoryFaceMood myMood = MoodFromText(lineItems[6]);
			StoryPose myPose = PoseFromText(lineItems[7]);
			if (myActor != StoryActor.Falcon)
			{
				if (falconGO != null)
				{
					Object.Destroy(falconGO);
					character.gameObject.SetActive(true);
				}
				if (myActor != oldActor || oldMood != myMood)
				{
					int characterIdFromActorAndMood = GetCharacterIdFromActorAndMood(myActor, myMood);
					BuildCharacter(characterIdFromActorAndMood);
					if (myActor == StoryActor.Pythor)
					{
						Quaternion rotation = character.gameObject.transform.rotation;
						rotation.y = 150f;
						character.gameObject.transform.rotation = rotation;
					}
					else
					{
						Quaternion rotation2 = character.gameObject.transform.rotation;
						rotation2.y = 170f;
						character.gameObject.transform.rotation = rotation2;
					}
					oldActor = myActor;
					oldMood = myMood;
				}
				yield return StartCoroutine(SetAnimation(myActor, myMood, myPose));
			}
			else
			{
				falconGO = (GameObject)Object.Instantiate(Resources.Load("Prefabs/Falcon"), character.transform.position - new Vector3(0f, 0.15f, 0f), character.transform.rotation);
				falconGO.gameObject.SetLayerRecursivly(LayerMask.NameToLayer("Player"));
				character.gameObject.SetActive(false);
			}
			string localizationKey = string.Format("STORY_S{0}C{1}P{2}", season, chapter, i + 1);
			string localizedText = Localization.Instance.Get(localizationKey);
			speechBubble.DisplayText(localizedText);
			while (speechBubble.IsDisplayingText)
			{
				yield return 0;
			}
			while (!Input.GetMouseButtonDown(0))
			{
				yield return 0;
			}
		}
		speechBubble.Hide();
		isShowing = false;
		Done(false);
	}

	private IEnumerator SetAnimation(StoryActor actor, StoryFaceMood mood, StoryPose pose)
	{
		characterAnimator.SetTrigger("Set mood");
		while (!characterAnimator.GetCurrentAnimatorStateInfo(0).IsName("SettingMood"))
		{
			yield return 0;
		}
		switch (pose)
		{
		case StoryPose.Default:
			characterAnimator.SetFloat("mood", (float)mood);
			characterAnimator.SetTrigger("Show mood");
			break;
		case StoryPose.Armsraised:
			characterAnimator.SetFloat("mood", 6f);
			characterAnimator.SetTrigger("Show mood");
			break;
		case StoryPose.Holdinghands:
			characterAnimator.SetFloat("mood", 7f);
			characterAnimator.SetTrigger("Show mood");
			break;
		case StoryPose.Openarms:
			characterAnimator.SetFloat("mood", 8f);
			characterAnimator.SetTrigger("Show mood");
			break;
		case StoryPose.Leanback:
			characterAnimator.SetFloat("mood", 9f);
			characterAnimator.SetTrigger("Show mood");
			break;
		case StoryPose.Pointing:
			characterAnimator.SetFloat("mood", 10f);
			characterAnimator.SetTrigger("Show mood");
			break;
		case StoryPose.Tilted:
			characterAnimator.SetFloat("mood", 11f);
			characterAnimator.SetTrigger("Show mood");
			break;
		case StoryPose.Lookup:
			characterAnimator.SetFloat("mood", 12f);
			characterAnimator.SetTrigger("Show mood");
			break;
		case StoryPose.Straight:
			characterAnimator.SetFloat("mood", 13f);
			characterAnimator.SetTrigger("Show mood");
			break;
		}
	}

	private static List<string> GetSeasonChapterFromFile(int season, int chapter)
	{
		TextAsset storyFile = GameController.Instance.storyDB.GetStoryFile(season);
		List<string> list = new List<string>();
		using (StringReader stringReader = new StringReader(storyFile.text))
		{
			for (string text = stringReader.ReadLine(); text != null; text = stringReader.ReadLine())
			{
				string[] array = text.Split('\t');
				if (array[0] == season.ToString() && array[1] == chapter.ToString())
				{
					list.Add(text);
				}
			}
			return list;
		}
	}

	private static void Done(bool skipped, bool silent = false)
	{
		int lastPlayedSeason = GameController.Instance.PlayerProfile.LastPlayedSeason;
		int chapterCursor = GameController.Instance.PlayerProfile.GetChapterCursor(lastPlayedSeason);
		List<string> seasonChapterFromFile = GetSeasonChapterFromFile(lastPlayedSeason, chapterCursor);
		if (!silent)
		{
			AnalyticsEvents.LogStoryViewerEnd(lastPlayedSeason, chapterCursor, skipped);
		}
		if (seasonChapterFromFile.Count > 0)
		{
			string[] array = seasonChapterFromFile[seasonChapterFromFile.Count - 1].Split('\t');
			string text = "TeaFarm";
			if (array[8].Length > 0)
			{
				text = array[8];
			}
			if (OnChapterEnd != null)
			{
				OnChapterEnd();
			}
			GameController.Instance.ObjectiveController.InitiateObjective(lastPlayedSeason, chapterCursor);
			if (text == "AreaMap")
			{
				Object.Instantiate(Resources.Load("Prefabs/Dialogs/InGameLoadScreen"));
			}
			SceneManager.LoadScene(text);
		}
		else
		{
			SceneManager.LoadScene("TeaFarm");
		}
	}

	private StoryActor ActorFromText(string text)
	{
		switch (text.ToLower())
		{
		case "wu":
			return StoryActor.Wu;
		case "kai":
			return StoryActor.Kai;
		case "lloyd":
			return StoryActor.Lloyd;
		case "jay":
			return StoryActor.Jay;
		case "zane":
			return StoryActor.Zane;
		case "nya":
			return StoryActor.Nya;
		case "cole":
			return StoryActor.Cole;
		case "falcon":
			return StoryActor.Falcon;
		case "pythor":
			return StoryActor.Pythor;
		case "fangtom":
			return StoryActor.Fangtom;
		default:
			Debug.LogError("Unknown actor type...");
			return StoryActor.Wu;
		}
	}

	private StoryFaceMood MoodFromText(string text)
	{
		switch (text.ToLower())
		{
		case "neutral":
			return StoryFaceMood.Neutral;
		case "surprised":
			return StoryFaceMood.Surprised;
		case "happy":
			return StoryFaceMood.Happy;
		case "sad":
			return StoryFaceMood.Sad;
		case "angry":
			return StoryFaceMood.Angry;
		case "corrupted":
			return StoryFaceMood.Corrupted;
		default:
			Debug.LogError("Unknown face mood");
			return StoryFaceMood.Neutral;
		}
	}

	private StoryPose PoseFromText(string text)
	{
		switch (text.ToLower())
		{
		case "":
			Debug.Log("No PoseFromText - Using default pose for state");
			return StoryPose.Default;
		case "default":
			return StoryPose.Default;
		case "arms raised":
			return StoryPose.Armsraised;
		case "holding hands":
			return StoryPose.Holdinghands;
		case "open arms":
			return StoryPose.Openarms;
		case "leanback":
			return StoryPose.Leanback;
		case "pointing":
			return StoryPose.Pointing;
		case "tilted":
			return StoryPose.Tilted;
		case "look up":
			return StoryPose.Lookup;
		case "straight":
			return StoryPose.Straight;
		default:
			Debug.LogError("Unknown pose");
			return StoryPose.Default;
		}
	}

	private StoryBackground BackgroundFromText(string text)
	{
		switch (text.ToLower())
		{
		case "unknown location":
			return StoryBackground.UnknownLocation;
		case "teafarm":
			return StoryBackground.TeaFarm;
		case "forest":
			return StoryBackground.Forest;
		case "desert":
			return StoryBackground.Desert;
		case "mountain":
			return StoryBackground.Mountain;
		case "swamp":
			return StoryBackground.Swamp;
		case "dungeon":
			return StoryBackground.Dungeon;
		case "samuraixcave":
			return StoryBackground.SamuraiXCave;
		case "holoplayer":
			return StoryBackground.HoloPlayer;
		case "hypnobraitomb":
			return StoryBackground.HypnobrainTomb;
		case "fangpyretomb":
			return StoryBackground.FangpyreTomb;
		case "constrictaitomb":
			return StoryBackground.ConstrictaiTomb;
		default:
			Debug.LogError("Unkown background type");
			return StoryBackground.TeaFarm;
		}
	}

	private StoryOverlay OverlayFromText(string text)
	{
		switch (text.ToLower())
		{
		case "holoplayer":
			return StoryOverlay.HoloPlayer;
		case "corrupted":
			return StoryOverlay.Corrupted;
		default:
			return StoryOverlay.None;
		}
	}

	public int GetCharacterIdFromActorAndMood(StoryActor actor, StoryFaceMood mood)
	{
		switch (actor)
		{
		case StoryActor.Wu:
			switch (mood)
			{
			case StoryFaceMood.Neutral:
				return 1060;
			case StoryFaceMood.Surprised:
				return 1061;
			case StoryFaceMood.Happy:
				return 1062;
			case StoryFaceMood.Sad:
				return 1063;
			case StoryFaceMood.Angry:
				return 1064;
			case StoryFaceMood.Corrupted:
				return 1065;
			default:
				return 1060;
			}
		case StoryActor.Kai:
			switch (mood)
			{
			case StoryFaceMood.Neutral:
				return 1000;
			case StoryFaceMood.Surprised:
				return 1001;
			case StoryFaceMood.Happy:
				return 1002;
			case StoryFaceMood.Sad:
				return 1003;
			case StoryFaceMood.Angry:
				return 1004;
			case StoryFaceMood.Corrupted:
				return 1005;
			default:
				return 1000;
			}
		case StoryActor.Jay:
			switch (mood)
			{
			case StoryFaceMood.Neutral:
				return 1010;
			case StoryFaceMood.Surprised:
				return 1011;
			case StoryFaceMood.Happy:
				return 1012;
			case StoryFaceMood.Sad:
				return 1013;
			case StoryFaceMood.Angry:
				return 1014;
			case StoryFaceMood.Corrupted:
				return 1015;
			default:
				return 1010;
			}
		case StoryActor.Cole:
			switch (mood)
			{
			case StoryFaceMood.Neutral:
				return 1020;
			case StoryFaceMood.Surprised:
				return 1021;
			case StoryFaceMood.Happy:
				return 1022;
			case StoryFaceMood.Sad:
				return 1023;
			case StoryFaceMood.Angry:
				return 1024;
			case StoryFaceMood.Corrupted:
				return 1025;
			default:
				return 1020;
			}
		case StoryActor.Zane:
			switch (mood)
			{
			case StoryFaceMood.Neutral:
				return 1030;
			case StoryFaceMood.Surprised:
				return 1031;
			case StoryFaceMood.Happy:
				return 1032;
			case StoryFaceMood.Sad:
				return 1033;
			case StoryFaceMood.Angry:
				return 1034;
			case StoryFaceMood.Corrupted:
				return 1035;
			default:
				return 1030;
			}
		case StoryActor.Nya:
			switch (mood)
			{
			case StoryFaceMood.Neutral:
				return 1040;
			case StoryFaceMood.Surprised:
				return 1041;
			case StoryFaceMood.Happy:
				return 1042;
			case StoryFaceMood.Sad:
				return 1043;
			case StoryFaceMood.Angry:
				return 1044;
			case StoryFaceMood.Corrupted:
				return 1045;
			default:
				return 1040;
			}
		case StoryActor.Lloyd:
			switch (mood)
			{
			case StoryFaceMood.Neutral:
				return 1050;
			case StoryFaceMood.Surprised:
				return 1051;
			case StoryFaceMood.Happy:
				return 1052;
			case StoryFaceMood.Sad:
				return 1053;
			case StoryFaceMood.Angry:
				return 1054;
			case StoryFaceMood.Corrupted:
				return 1055;
			default:
				return 1050;
			}
		case StoryActor.Falcon:
			switch (mood)
			{
			case StoryFaceMood.Neutral:
				return 1070;
			case StoryFaceMood.Surprised:
				return 1071;
			case StoryFaceMood.Happy:
				return 1072;
			case StoryFaceMood.Sad:
				return 1073;
			case StoryFaceMood.Angry:
				return 1074;
			case StoryFaceMood.Corrupted:
				return 1075;
			default:
				return 1070;
			}
		case StoryActor.Fangtom:
			switch (mood)
			{
			case StoryFaceMood.Neutral:
				return 1090;
			case StoryFaceMood.Surprised:
				return 1091;
			case StoryFaceMood.Happy:
				return 1092;
			case StoryFaceMood.Sad:
				return 1093;
			case StoryFaceMood.Angry:
				return 1094;
			case StoryFaceMood.Corrupted:
				return 1095;
			default:
				return 1090;
			}
		case StoryActor.Pythor:
			switch (mood)
			{
			case StoryFaceMood.Neutral:
				return 1080;
			case StoryFaceMood.Surprised:
				return 1081;
			case StoryFaceMood.Happy:
				return 1082;
			case StoryFaceMood.Sad:
				return 1083;
			case StoryFaceMood.Angry:
				return 1084;
			case StoryFaceMood.Corrupted:
				return 1085;
			default:
				return 1080;
			}
		default:
			return 0;
		}
	}

	public void BuildCharacter(int i)
	{
		MinifigDefinition minifigDefinition = CharacterBuilder.minifigDefinitions[i];
		Texture2D characterTexture = CharacterBuilder.ConfigureTexture(character, minifigDefinition);
		CharacterBuilder.ConfigureCharacter(character, minifigDefinition, characterTexture);
		characterAnimator = character.GetComponent<Animator>();
		characterAnimator.SetFloat(TeahouseVariables.weaponType, (float)minifigDefinition.weapon);
		characterAnimator.SetFloat("randomStart", Random.value);
		if ((double)Random.value > 0.5)
		{
			characterAnimator.SetFloat("idles", 1f);
		}
		else
		{
			characterAnimator.SetFloat("idles", 0f);
		}
	}

	public static void Load()
	{
		Done(true, true);
	}
}
