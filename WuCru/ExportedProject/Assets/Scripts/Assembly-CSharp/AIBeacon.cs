using System.Collections.Generic;
using UnityEngine;

public class AIBeacon
{
	private static AIBeacon instance;

	private static List<AIBeaconReport> reports = new List<AIBeaconReport>();

	private static Dictionary<int, LevelNode> lastBeaconCallLocation = new Dictionary<int, LevelNode>();

	public static AIBeacon Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new AIBeacon();
			}
			return instance;
		}
	}

	public void Reset()
	{
		reports.Clear();
	}

	public LevelNode GetLastBeaconCallLocation(int agentId)
	{
		if (!lastBeaconCallLocation.ContainsKey(agentId))
		{
			return null;
		}
		return lastBeaconCallLocation[agentId];
	}

	public void ReportEvent(AIAgent reporter, LevelNode playerLocationNode, Vector3 reporterPosition, float radius, float severity)
	{
		AIBeaconReport item = new AIBeaconReport(reporter, playerLocationNode, reporterPosition, radius, severity);
		reports.Add(item);
	}

	public void Blink(AIAgent[] agents)
	{
		foreach (AIAgent aIAgent in agents)
		{
			LevelNode levelNode = DoEvaluate(aIAgent, aIAgent.Character.transform.position, aIAgent.AIBeaconResponseThreshold);
			if (levelNode != null)
			{
				if (lastBeaconCallLocation.ContainsKey(aIAgent.AgentId))
				{
					lastBeaconCallLocation[aIAgent.AgentId] = levelNode;
				}
				else
				{
					lastBeaconCallLocation.Add(aIAgent.AgentId, levelNode);
				}
				aIAgent.OnAIBeaconCall();
			}
		}
		reports.Clear();
	}

	private LevelNode DoEvaluate(AIAgent agent, Vector3 agentPosition, float responseThreshold)
	{
		float num = responseThreshold;
		LevelNode result = null;
		foreach (AIBeaconReport report in reports)
		{
			if (!(report.reporter != agent))
			{
				continue;
			}
			float num2 = Vector3.Distance(agentPosition, report.reporterPosition);
			if (report.radius > num2)
			{
				float num3 = (report.radius - num2) / report.radius;
				float num4 = report.severity * num3;
				if (num4 > num)
				{
					num = num4;
					result = report.playerLocation;
				}
			}
		}
		return result;
	}
}
