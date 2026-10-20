using UnityEngine;
using UnityEngine.UI;

public class GameOverGUI : MonoBehaviour
{
	public Image imageLevel;

	public NinjaPortrait[] portraits;

	public RawImage[] imageNinjas;

	private void Awake()
	{
		imageLevel.sprite = GameController.Instance.CurrentLevel.LevelStyle.previewImage;
		if (imageNinjas.Length <= 0)
		{
			return;
		}
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		Texture2D[] array = CharacterPreviewRenderer.Generate(playerProfile.NinjaTeam, (int)imageNinjas[0].rectTransform.sizeDelta.x, (int)imageNinjas[0].rectTransform.sizeDelta.y);
		for (int i = 0; i < imageNinjas.Length; i++)
		{
			int ninjaInTeam = playerProfile.GetNinjaInTeam(i);
			if (ninjaInTeam >= 0)
			{
				portraits[i].Configure(ninjaInTeam, array[i], NinjaPortraitState.Disabled);
			}
			else
			{
				portraits[i].gameObject.SetActive(false);
			}
		}
	}
}
