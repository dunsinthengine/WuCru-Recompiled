using System;
using UnityEngine;

public class CharacterFullbodyRenderer : MonoBehaviour
{
	public Camera characterCamera;

	public Light characterLight;

	private static Color defaultAmbientLight = new Color(54f / 255f, 58f / 255f, 66f / 255f);

	public static RenderTexture Generate(int id, int textureWidth, int textureHeight)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/GUI/CharacterCreation/MenuCharacter"));
		MenuCharacter component = gameObject.GetComponent<MenuCharacter>();
		component.GetComponent<Animator>().SetTrigger(TeahouseTriggers.stillPose);
		UnityEngine.Random.seed = DateTime.Now.Millisecond;
		float value = UnityEngine.Random.Range(0, 7);
		component.GetComponent<Animator>().SetFloat(TeahouseVariables.stillPoses, value);
		GameObject gameObject2 = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/GUI/CharacterCreation/CharacterFullbodyRenderer"));
		CharacterFullbodyRenderer component2 = gameObject2.GetComponent<CharacterFullbodyRenderer>();
		Light[] array = UnityEngine.Object.FindObjectsOfType<Light>();
		bool[] array2 = new bool[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].type == LightType.Directional && array[i] != component2.characterLight)
			{
				array2[i] = array[i].enabled;
				array[i].enabled = false;
			}
		}
		bool fog = RenderSettings.fog;
		RenderSettings.fog = false;
		Color ambientLight = RenderSettings.ambientLight;
		RenderSettings.ambientLight = defaultAmbientLight;
		RenderTexture renderTexture = new RenderTexture(textureWidth * 2, textureHeight * 2, 24, RenderTextureFormat.ARGB32);
		renderTexture.antiAliasing = 4;
		MinifigDefinition def = ((id != 0) ? CharacterBuilder.minifigDefinitions[id] : GameController.Instance.PlayerProfile.GetMinifigDefinition());
		Texture2D characterTexture = CharacterBuilder.ConfigureTexture(component, def);
		CharacterBuilder.ConfigureCharacter(component, def, characterTexture);
		Weapon weapon = component.GetWeapon();
		if ((bool)weapon && weapon.type == WeaponType.Nunchuck)
		{
			Animation componentInChildren = weapon.GetComponentInChildren<Animation>();
			componentInChildren["folded"].time = 0.001f;
			componentInChildren["folded"].enabled = true;
			componentInChildren["folded"].weight = 1f;
			componentInChildren.Sample();
		}
		component2.characterCamera.targetTexture = renderTexture;
		component2.characterCamera.Render();
		component2.characterCamera.targetTexture = null;
		RenderSettings.fog = fog;
		RenderSettings.ambientLight = ambientLight;
		for (int j = 0; j < array.Length; j++)
		{
			if (array[j].type == LightType.Directional && array[j] != component2.characterLight)
			{
				array[j].enabled = array2[j];
			}
		}
		UnityEngine.Object.DestroyImmediate(gameObject);
		UnityEngine.Object.DestroyImmediate(gameObject2);
		return renderTexture;
	}
}
