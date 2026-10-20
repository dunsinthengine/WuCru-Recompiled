using UnityEngine;

public class NinjaPortraitData : MonoBehaviour
{
	public Sprite[] powerSprites;

	public Sprite[] weaponSprites;

	public Sprite[] missile1Sprites;

	public Sprite[] missile2prites;

	public Color[] powerColors;

	public Sprite activeFrameSprite;

	public Sprite activeBackgroundSprite;

	public Sprite inactiveFrameSprite;

	public Sprite inactiveBackgroundSprite;

	public Color selectedColor;

	public Color activeColor;

	public Color missingColor;

	public Color disabledColor;

	public Color lockedMenuColor;

	public Color unequippedMenuColor;

	public Color FrameColor(NinjaPortraitState state)
	{
		switch (state)
		{
		default:
			return activeColor;
		case NinjaPortraitState.Selected:
			return selectedColor;
		case NinjaPortraitState.Missing:
			return missingColor;
		case NinjaPortraitState.Disabled:
			return disabledColor;
		case NinjaPortraitState.LockedMenu:
			return lockedMenuColor;
		case NinjaPortraitState.UnequippedMenu:
			return unequippedMenuColor;
		}
	}
}
