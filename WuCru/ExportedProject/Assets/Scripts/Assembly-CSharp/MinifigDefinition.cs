using UnityEngine;

public class MinifigDefinition
{
	public int id;

	public int sku;

	public MinifigType type;

	public MinifigClass minifigClass;

	public string hairModel;

	public string helmetModel;

	public string shoulderModel;

	public bool helmetReplacesHead;

	public bool helmetHidesHair;

	public bool showHelmet;

	public string colorSlots;

	public string faceDecal;

	public string legDecal;

	public string pelvisDecal;

	public string torsoDecal;

	public WeaponType weapon;

	public Color32[] colors;

	public ElementalPower elementalPower;

	public string icon;

	public string TypeLocalizationKey
	{
		get
		{
			return "CHAR_" + type.ToString().ToUpper();
		}
	}
}
