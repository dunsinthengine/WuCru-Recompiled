using UnityEngine;

public class Player : MonoBehaviour
{
	public PlayerController controller;

	public Character Character
	{
		get
		{
			return controller.Character;
		}
	}

	private void Awake()
	{
		GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/Player/PlayerController"));
		controller = gameObject.GetComponent<PlayerController>();
	}

	private void OnDestroy()
	{
		if (controller != null && controller.gameObject != null)
		{
			Object.Destroy(controller.gameObject);
		}
	}
}
