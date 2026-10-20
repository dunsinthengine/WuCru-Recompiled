public class GameSpringboard : InitialisationObject
{
	public override void startInitialising()
	{
		base.startInitialising();
		_currentState = InitialisationState.FINISHED;
	}
}
