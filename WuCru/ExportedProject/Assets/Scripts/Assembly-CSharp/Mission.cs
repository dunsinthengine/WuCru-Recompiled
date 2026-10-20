public class Mission
{
	public delegate void OnIncreaseCompletionDelegate(int completionCount, int actionCount);

	private string missionID;

	private MissionAction action;

	private int actionCount;

	private int completionCount;

	private string missionImagePath;

	private string missionCompletedImagePath;

	private string missionIngameImagePath;

	public const string mission1ID = "{ED8A5593-039B-490B-A525-893FAECEB95A}";

	public const string mission2ID = "{D5CF288D-6B57-40B1-91BF-23F8AA8A6F83}";

	public const string mission3ID = "{53ECB1A2-36E8-4BC5-8E72-771D35551DA1}";

	public const string mission4ID = "{81767DB1-3EFF-4F09-860F-69561B5E6FB5}";

	public OnIncreaseCompletionDelegate OnIncreaseCompletion;

	public string MissionID
	{
		get
		{
			return missionID;
		}
	}

	public MissionAction Action
	{
		get
		{
			return action;
		}
	}

	public int ActionCount
	{
		get
		{
			return actionCount;
		}
	}

	public int CompletionCount
	{
		get
		{
			return completionCount;
		}
	}

	public string MissionImagePath
	{
		get
		{
			return missionImagePath;
		}
	}

	public string MissionCompletedImagePath
	{
		get
		{
			return missionCompletedImagePath;
		}
	}

	public string MissionIngameImagePath
	{
		get
		{
			return missionIngameImagePath;
		}
	}

	public bool IsCompleted()
	{
		if (completionCount >= actionCount)
		{
			return true;
		}
		return false;
	}

	public void IncreaseCompletion()
	{
		completionCount++;
		if (OnIncreaseCompletion != null)
		{
			OnIncreaseCompletion(completionCount, actionCount);
		}
	}

	public void Reset()
	{
		completionCount = 0;
	}

	public static Mission GenerateMission(string missionID)
	{
		switch (missionID)
		{
		case "{ED8A5593-039B-490B-A525-893FAECEB95A}":
			return GenerateMission1();
		case "{D5CF288D-6B57-40B1-91BF-23F8AA8A6F83}":
			return GenerateMission2();
		case "{53ECB1A2-36E8-4BC5-8E72-771D35551DA1}":
			return GenerateMission3();
		case "{81767DB1-3EFF-4F09-860F-69561B5E6FB5}":
			return GenerateMission4();
		default:
			return null;
		}
	}

	private static Mission GenerateMission1()
	{
		Mission mission = new Mission();
		mission.missionID = "{ED8A5593-039B-490B-A525-893FAECEB95A}";
		mission.action = new MissionAction(MissionActionType.Collect, MissionActionObject.MetalChest);
		mission.actionCount = 3;
		mission.completionCount = 0;
		mission.missionImagePath = "wu-cru/wu_cru_missions_3silverchests_challenge";
		mission.missionCompletedImagePath = "wu-cru/wu_cru_missions_3silverchests_success";
		mission.missionIngameImagePath = Paths.challengeMetalChest;
		return mission;
	}

	private static Mission GenerateMission2()
	{
		Mission mission = new Mission();
		mission.missionID = "{D5CF288D-6B57-40B1-91BF-23F8AA8A6F83}";
		mission.action = new MissionAction(MissionActionType.StealthKill, MissionActionObject.Enemy);
		mission.actionCount = 5;
		mission.completionCount = 0;
		mission.missionImagePath = "wu-cru/wu_cru_missions_5sneakattack_challenge";
		mission.missionCompletedImagePath = "wu-cru/wu_cru_missions_5sneakattack_success";
		mission.missionIngameImagePath = Paths.challengeSneakAttack;
		return mission;
	}

	private static Mission GenerateMission3()
	{
		Mission mission = new Mission();
		mission.missionID = "{53ECB1A2-36E8-4BC5-8E72-771D35551DA1}";
		mission.action = new MissionAction(MissionActionType.Kill, MissionActionObject.EnemyWarrior);
		mission.actionCount = 3;
		mission.completionCount = 0;
		mission.missionImagePath = "wu-cru/wu_cru_missions_3warriors_challenge";
		mission.missionCompletedImagePath = "wu-cru/wu_cru_missions_3warriors_success";
		mission.missionIngameImagePath = Paths.challengeKillWarrior;
		return mission;
	}

	private static Mission GenerateMission4()
	{
		Mission mission = new Mission();
		mission.missionID = "{81767DB1-3EFF-4F09-860F-69561B5E6FB5}";
		mission.action = new MissionAction(MissionActionType.RangeKill, MissionActionObject.Enemy);
		mission.actionCount = 5;
		mission.completionCount = 0;
		mission.missionImagePath = "wu-cru/wu_cru_missions_5crystalexplosions_challenge";
		mission.missionCompletedImagePath = "wu-cru/wu_cru_missions_5crystalexplosions_success";
		mission.missionIngameImagePath = Paths.challengeExplodeEnemies;
		return mission;
	}
}
