using UnityEngine;
using UnityEngine.UI;

public class MenuCharacterMouseInteraction : MonoBehaviour
{
	public Button.ButtonClickedEvent onClick { get; set; }

	private void OnMouseUpAsButton()
	{
		onClick.Invoke();
	}
}
