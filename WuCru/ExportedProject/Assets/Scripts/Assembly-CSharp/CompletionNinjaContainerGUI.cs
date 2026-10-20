using UnityEngine;
using UnityEngine.UI;

public class CompletionNinjaContainerGUI : MonoBehaviour
{
	public Image imageNinja;

	public Image imageFrameAlive;

	public Image imageFrameDead;

	private Material hiddenMaterial;

	private void Awake()
	{
		imageNinja.enabled = false;
		imageFrameAlive.enabled = false;
		imageFrameDead.enabled = false;
		hiddenMaterial = new Material(imageNinja.material);
		hiddenMaterial.color = new Color(1f, 1f, 1f, 0.3f);
	}

	public void ShowNinja(bool alive)
	{
		if (alive)
		{
			imageNinja.enabled = true;
			imageFrameAlive.enabled = true;
		}
		else
		{
			imageNinja.enabled = true;
			imageNinja.material = hiddenMaterial;
			imageFrameDead.enabled = true;
		}
	}
}
