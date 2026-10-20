using UnityEngine;

public class EditorController : MonoBehaviour
{
	private void Start()
	{
		EditorStateMachine.Instance.SetState(GetComponent<Editing>());
	}
}
