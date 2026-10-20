using UnityEngine.SceneManagement;

public class GameControllerStateTraversalGame : GameControllerState
{
	public override void BeginState()
	{
		SceneManager.LoadScene("Traversal");
	}

	public override void EndState()
	{
	}

	public override void UpdateState()
	{
	}
}
