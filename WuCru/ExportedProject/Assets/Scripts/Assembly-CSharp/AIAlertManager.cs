using UnityEngine;

public class AIAlertManager : MonoBehaviour
{
	private int alertCount;

	private int suspiciousCount;

	public int AlertCount
	{
		get
		{
			return alertCount;
		}
	}

	public int SuspiciousCount
	{
		get
		{
			return suspiciousCount;
		}
	}

	private void Update()
	{
		alertCount = 0;
		suspiciousCount = 0;
		if (GameController.Instance.CurrentLevel == null || GameController.Instance.CurrentLevel.Agents == null)
		{
			return;
		}
		AIAgent[] agents = GameController.Instance.CurrentLevel.Agents;
		foreach (AIAgent aIAgent in agents)
		{
			if (aIAgent.IsAlerted)
			{
				alertCount++;
			}
			if (aIAgent.IsSuspisious)
			{
				suspiciousCount++;
			}
		}
	}
}
