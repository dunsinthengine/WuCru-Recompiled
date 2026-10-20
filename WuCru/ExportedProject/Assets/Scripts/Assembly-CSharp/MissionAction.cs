public class MissionAction
{
	public MissionActionType actionType;

	public MissionActionObject actionObject;

	private bool completed;

	public bool Completed
	{
		get
		{
			return completed;
		}
		set
		{
			completed = value;
		}
	}

	public MissionAction(MissionActionType type, MissionActionObject obj)
	{
		actionType = type;
		actionObject = obj;
	}
}
