using UnityEngine;

public class CharacterPreviewRenderer : MonoBehaviour
{
	public Camera characterCamera;

	public Light characterLight;

	private static Color defaultAmbientLight = new Color(54f / 255f, 58f / 255f, 66f / 255f);

	public static Texture2D[] Generate(int[] ids, int textureWidth, int textureHeight)
	{
		RenderTexture[] array = new RenderTexture[ids.Length];
		GameObject gameObject = Object.Instantiate(Resources.Load<GameObject>("Prefabs/GUI/CharacterCreation/MenuCharacter"));
		MenuCharacter component = gameObject.GetComponent<MenuCharacter>();
		GameObject gameObject2 = Object.Instantiate(Resources.Load<GameObject>("Prefabs/GUI/CharacterCreation/CharacterPreviewRenderer"));
		CharacterPreviewRenderer component2 = gameObject2.GetComponent<CharacterPreviewRenderer>();
		Light[] array2 = Object.FindObjectsOfType<Light>();
		bool[] array3 = new bool[array2.Length];
		for (int i = 0; i < array2.Length; i++)
		{
			if (array2[i].type == LightType.Directional && array2[i] != component2.characterLight)
			{
				array3[i] = array2[i].enabled;
				array2[i].enabled = false;
			}
		}
		bool fog = RenderSettings.fog;
		RenderSettings.fog = false;
		Color ambientLight = RenderSettings.ambientLight;
		RenderSettings.ambientLight = defaultAmbientLight;
		for (int j = 0; j < ids.Length; j++)
		{
			if (ids[j] >= 0)
			{
				array[j] = new RenderTexture(textureWidth, textureHeight, 24, RenderTextureFormat.ARGB32);
				array[j].antiAliasing = 4;
				array[j].name = "Ninja_rendertex_" + component.ninjaID;
				if ((bool)component.hairGameObject)
				{
					component.hairGameObject.SetActive(false);
				}
				if ((bool)component.helmetGameObject)
				{
					component.helmetGameObject.SetActive(false);
				}
				MinifigDefinition def = null;
				if (ids[j] == 0 && GameController.Instance != null && GameController.Instance.PlayerProfile != null)
				{
					def = GameController.Instance.PlayerProfile.GetMinifigDefinition();
				}
				else if (CharacterBuilder.minifigDefinitions != null && CharacterBuilder.minifigDefinitions.ContainsKey(ids[j]))
				{
					def = CharacterBuilder.minifigDefinitions[ids[j]];
				}
				else if (CharacterBuilder.minifigDefinitions != null && CharacterBuilder.minifigDefinitions.Count > 0)
				{
					foreach (var d in CharacterBuilder.minifigDefinitions.Values) { def = d; break; }
				}
				if (def != null)
				{
					Texture2D characterTexture = CharacterBuilder.ConfigureTexture(component, def);
					CharacterBuilder.ConfigureCharacter(component, def, characterTexture);
				}
				component2.characterCamera.targetTexture = array[j];
				component2.characterCamera.Render();
				component2.characterCamera.targetTexture = null;
			}
		}
		RenderSettings.fog = fog;
		RenderSettings.ambientLight = ambientLight;
		for (int k = 0; k < array2.Length; k++)
		{
			if (array2[k].type == LightType.Directional && array2[k] != component2.characterLight)
			{
				array2[k].enabled = array3[k];
			}
		}
		Object.DestroyImmediate(gameObject);
		Object.DestroyImmediate(gameObject2);
		RenderTexture active = RenderTexture.active;
		Texture2D[] array4 = new Texture2D[array.Length];
		for (int l = 0; l < array4.Length; l++)
		{
			RenderTexture.active = array[l];
			Texture2D texture2D = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false);
			texture2D.filterMode = FilterMode.Bilinear;
			texture2D.name = "Ninja_" + textureWidth + "x" + textureHeight + "_" + component.ninjaID;
			texture2D.ReadPixels(new Rect(0f, 0f, textureWidth, textureHeight), 0, 0);
			texture2D.Apply();
			RenderTexture.active = active;
			array[l].Release();
			Object.Destroy(array[l]);
			array4[l] = texture2D;
		}
		return array4;
	}
}
