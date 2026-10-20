using System.Collections.Generic;
using UnityEngine;

public class MinifigTest : MonoBehaviour
{
	public MenuCharacter menuCharacterPrefab;

	public bool useHelmet = true;

	private void Start()
	{
		int num = 0;
		foreach (Dictionary<MinifigClass, int> value in CharacterBuilder.enemies.Values)
		{
			foreach (int value2 in value.Values)
			{
				MenuCharacter component = Object.Instantiate(menuCharacterPrefab.gameObject).GetComponent<MenuCharacter>();
				component.transform.localPosition = num++ * Vector3.right;
				MinifigDefinition def = CharacterBuilder.minifigDefinitions[value2];
				Texture2D characterTexture = CharacterBuilder.ConfigureTexture(component, def);
				CharacterBuilder.ConfigureCharacter(component, def, characterTexture);
			}
		}
		num = 0;
		foreach (int ninja in CharacterBuilder.ninjas)
		{
			MenuCharacter component2 = Object.Instantiate(menuCharacterPrefab.gameObject).GetComponent<MenuCharacter>();
			component2.transform.localPosition = num++ * Vector3.right + Vector3.forward * -3f;
			MinifigDefinition def2 = CharacterBuilder.minifigDefinitions[ninja];
			Texture2D characterTexture2 = CharacterBuilder.ConfigureTexture(component2, def2);
			CharacterBuilder.ConfigureCharacter(component2, def2, characterTexture2);
		}
		PlayerProfile playerProfile = new PlayerProfile();
		playerProfile.LoadLocal();
		MenuCharacter component3 = Object.Instantiate(menuCharacterPrefab.gameObject).GetComponent<MenuCharacter>();
		component3.transform.localPosition = Vector3.forward * -6f;
		MinifigDefinition minifigDefinition = playerProfile.GetMinifigDefinition();
		Texture2D characterTexture3 = CharacterBuilder.ConfigureTexture(component3, minifigDefinition);
		CharacterBuilder.ConfigureCharacter(component3, minifigDefinition, characterTexture3);
	}

	private void Update()
	{
	}
}
