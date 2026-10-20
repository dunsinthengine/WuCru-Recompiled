using UnityEngine;

public class AIBeaconReport
{
	public AIAgent reporter;

	public LevelNode playerLocation;

	public Vector3 reporterPosition;

	public float radius;

	public float severity;

	public AIBeaconReport(AIAgent reporter, LevelNode playerLocation, Vector3 reporterPosition, float radius, float severity)
	{
		this.reporter = reporter;
		this.playerLocation = playerLocation;
		this.reporterPosition = reporterPosition;
		this.radius = radius;
		this.severity = severity;
	}
}
