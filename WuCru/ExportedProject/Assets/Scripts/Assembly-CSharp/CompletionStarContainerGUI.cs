using UnityEngine;
using UnityEngine.UI;

public class CompletionStarContainerGUI : MonoBehaviour
{
	public GameObject root;

	public Image imageStar;

	private void Awake()
	{
		imageStar.enabled = false;
	}

	public void GiveStar(bool completed)
	{
		if (!completed)
		{
			imageStar.enabled = false;
		}
		else
		{
			imageStar.enabled = true;
		}
	}
}
