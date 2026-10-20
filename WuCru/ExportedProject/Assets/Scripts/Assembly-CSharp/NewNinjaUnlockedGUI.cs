using Cape.Localization;
using UnityEngine;
using UnityEngine.UI;

public class NewNinjaUnlockedGUI : MonoBehaviour
{
	public delegate void OnCloseDelegate();

	public RawImage imageNinja;

	public Text textNinjaName;

	public Button ButtonOK;

	public Camera ninjaCamera;

	public OnCloseDelegate OnClose;

	private void Start()
	{
		ButtonOK.onClick.AddListener(() =>
		{
			Object.Destroy(base.gameObject);
			if (OnClose != null)
			{
				OnClose();
			}
		});
	}

	public void SetNinja(int ninjaID)
	{
		Localize component = textNinjaName.GetComponent<Localize>();
		if (component == null)
		{
			Debug.LogWarning("Localization key update failed: No Localize component attached to the Text object");
		}
		else
		{
			component.SetKey(CharacterBuilder.minifigDefinitions[ninjaID].TypeLocalizationKey);
		}
		CharacterBuilder.RenderToImage(imageNinja, GetComponent<CanvasScaler>(), ninjaCamera, base.transform, ninjaID);
		RenderTexture texture = CharacterFullbodyRenderer.Generate(ninjaID, (int)imageNinja.rectTransform.sizeDelta.x, (int)imageNinja.rectTransform.sizeDelta.y);
		imageNinja.texture = texture;
	}

	public void OnDestroy()
	{
		RenderTexture renderTexture = imageNinja.texture as RenderTexture;
		if ((bool)renderTexture)
		{
			renderTexture.Release();
		}
	}
}
