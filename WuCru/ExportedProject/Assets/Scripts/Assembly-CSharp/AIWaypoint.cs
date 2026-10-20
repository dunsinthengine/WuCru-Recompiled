public class AIWaypoint
{
	public LevelNode levelNode;

	public float restTime = 3.8f;

	public AIWaypoint(LevelNode node, float restTime)
	{
		levelNode = node;
		this.restTime = restTime;
	}
}
