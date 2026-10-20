using System;
using System.Collections.Generic;
using MiniJSON;
using UnityEngine;

public class MissionController : MonoBehaviour
{
	private Mission currentMission;

	private WuCruRequestType _currentRequest = WuCruRequestType.None;

	private bool editor;

	public static string GetAvailableMissionsUrl { get; private set; }

	public static string GetCompletedMissionsUrl { get; private set; }

	public static string CompleteMissionUrl { get; private set; }

	public Mission CurrentMission
	{
		get
		{
			return currentMission;
		}
	}

	public bool CurrentMissionAcknowledged { get; set; }

	public bool Online { get; set; }

	public event EventHandler<DataEventArgs<bool>> MissionCompletedEvent;

	public event EventHandler<DataEventArgs<Mission>> NewMissionReadyEvent;

	private void Awake()
	{
		Online = false;
	}

	private void Start()
	{
		GetAvailableMissionsUrl = "https://www.lego.com/en-us/ninjago/api/v1/wucrumissionservice/getmissions/available";
		GetCompletedMissionsUrl = "https://www.lego.com/en-us/ninjago/api/v1/wucrumissionservice/getmissions/completed";
		CompleteMissionUrl = "https://www.lego.com/en-us/ninjago/api/v1/wucrumissionservice/completemission/";
		CurrentMissionAcknowledged = false;
	}

	public void StartMission()
	{
		_currentRequest = WuCruRequestType.GetAvailableMissions;
	}

	public void ResetMission()
	{
		if (currentMission != null)
		{
			currentMission.Reset();
		}
		CurrentMissionAcknowledged = false;
	}

	public bool HasActiveMission()
	{
		return currentMission != null && !string.IsNullOrEmpty(currentMission.MissionID);
	}

	public void CompleteMission(string missionID)
	{
		string text = CompleteMissionUrl + missionID;
		text = text.Replace("{", "%7b").Replace("}", "%7d");
		Debug.LogFormat("Sending complete mission request\nURL : {0}", text);
	}

	private void HandleGetAvailableMissionsResponse(string json)
	{
		List<object> list = Json.Deserialize(json) as List<object>;
		List<string> list2 = new List<string>();
		foreach (Dictionary<string, object> item in list)
		{
			list2.Add((string)item["MissionId"]);
		}
		if (list2.Contains("{ED8A5593-039B-490B-A525-893FAECEB95A}"))
		{
			currentMission = Mission.GenerateMission("{ED8A5593-039B-490B-A525-893FAECEB95A}");
		}
		else if (list2.Contains("{D5CF288D-6B57-40B1-91BF-23F8AA8A6F83}"))
		{
			currentMission = Mission.GenerateMission("{D5CF288D-6B57-40B1-91BF-23F8AA8A6F83}");
		}
		else if (list2.Contains("{53ECB1A2-36E8-4BC5-8E72-771D35551DA1}"))
		{
			currentMission = Mission.GenerateMission("{53ECB1A2-36E8-4BC5-8E72-771D35551DA1}");
		}
		else if (list2.Contains("{81767DB1-3EFF-4F09-860F-69561B5E6FB5}"))
		{
			currentMission = Mission.GenerateMission("{81767DB1-3EFF-4F09-860F-69561B5E6FB5}");
		}
		if (currentMission == null)
		{
			Debug.Log("Could not find any available missions");
		}
		else
		{
			DataEventArgs<Mission> e = new DataEventArgs<Mission>();
			e.data = currentMission;
			DataEventArgs<Mission> e2 = e;
			if (NewMissionReadyEvent != null)
			{
				NewMissionReadyEvent(this, e2);
			}
		}
		_currentRequest = WuCruRequestType.None;
	}

	private void HandleCompleteMissionResponse(string json)
	{
		Dictionary<string, object> dictionary = Json.Deserialize(json) as Dictionary<string, object>;
		if (dictionary == null)
		{
			Debug.LogWarning("Could not parse JSON from complete mission response");
			if (MissionCompletedEvent != null)
			{
				DataEventArgs<bool> e = new DataEventArgs<bool>();
				e.data = false;
				DataEventArgs<bool> e2 = e;
				MissionCompletedEvent(this, e2);
			}
			ResetMission();
			return;
		}
		bool flag = (bool)dictionary["Success"];
		string text = (string)dictionary["Message"];
		Dictionary<string, object> dictionary2 = dictionary["Data"] as Dictionary<string, object>;
		if (flag)
		{
			string text2 = (string)dictionary2["MissionId"];
			Debug.LogWarningFormat("LEGO backend completed mission {0} successfully\n\nMessage : {1}", text2, text);
			GameController.Instance.PlayerProfile.CompleteMission(text2);
			if (MissionCompletedEvent != null)
			{
				DataEventArgs<bool> e = new DataEventArgs<bool>();
				e.data = true;
				DataEventArgs<bool> e3 = e;
				MissionCompletedEvent(this, e3);
			}
			currentMission = null;
		}
		else
		{
			Debug.LogWarningFormat("LEGO backend could not complete mission\n\nMessage : {0}", text);
			if (MissionCompletedEvent != null)
			{
				DataEventArgs<bool> e = new DataEventArgs<bool>();
				e.data = false;
				DataEventArgs<bool> e4 = e;
				MissionCompletedEvent(this, e4);
			}
			ResetMission();
		}
	}

	public void OnInteractableInteract(Level level, InteractableObject interactable)
	{
		if (currentMission == null || currentMission.IsCompleted() || currentMission.Action.actionType != MissionActionType.Collect)
		{
			return;
		}
		switch (currentMission.Action.actionObject)
		{
		case MissionActionObject.GoldChest:
			if (interactable.GetType() == typeof(Chest) && (interactable as Chest).chestType == ChestType.Gold)
			{
				currentMission.IncreaseCompletion();
			}
			break;
		case MissionActionObject.MetalChest:
			if (interactable.GetType() == typeof(Chest) && (interactable as Chest).chestType == ChestType.Metal)
			{
				currentMission.IncreaseCompletion();
			}
			break;
		}
	}

	public void OnCharacterDead(Level level, Character character, DamageType damageType)
	{
		if (currentMission == null || currentMission.IsCompleted())
		{
			return;
		}
		if (currentMission.Action.actionType == MissionActionType.Kill)
		{
			if (EvalDeadCharacter(currentMission.Action, character))
			{
				currentMission.IncreaseCompletion();
			}
		}
		else if (currentMission.Action.actionType == MissionActionType.RangeKill && damageType == DamageType.Explosion)
		{
			if (EvalDeadCharacter(currentMission.Action, character))
			{
				currentMission.IncreaseCompletion();
			}
		}
		else if (currentMission.Action.actionType == MissionActionType.StealthKill && damageType == DamageType.SneakAttack && EvalDeadCharacter(currentMission.Action, character))
		{
			currentMission.IncreaseCompletion();
		}
	}

	private bool EvalDeadCharacter(MissionAction a, Character character)
	{
		if (a.actionObject == MissionActionObject.Enemy)
		{
			return true;
		}
		AIAgentType agentType = (character.Controller as AIController).agent.GetAgentType();
		if (a.actionObject == MissionActionObject.EnemyWarrior && agentType == AIAgentType.WarriorEnemy)
		{
			return true;
		}
		return false;
	}
}
