using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NinjaPortrait : MonoBehaviour
{
	public Image[] frame;

	public RawImage portrait;

	public Button button;

	public Image power;

	public Image weapon;

	public Image background;

	public HealthBarNinjaTeam health;

	private Coroutine blinkCoroutine;

	public void Hide(NinjaPortraitState state)
	{
		NinjaPortraitData component = GameController.Instance.GetComponent<NinjaPortraitData>();
		if ((bool)portrait)
		{
			portrait.gameObject.SetActive(false);
		}
		if ((bool)power)
		{
			power.gameObject.SetActive(false);
		}
		if ((bool)weapon)
		{
			weapon.gameObject.SetActive(false);
		}
		if ((bool)background)
		{
			background.color = component.FrameColor(state);
		}
		for (int i = 0; i < frame.Length; i++)
		{
			frame[i].color = component.FrameColor(state);
		}
	}

	public void Configure(int ninjaID, Texture portraitTexture, NinjaPortraitState state = NinjaPortraitState.Active)
	{
		NinjaPortraitData component = GameController.Instance.GetComponent<NinjaPortraitData>();
		MinifigDefinition minifigDefinition = ((ninjaID != 0) ? CharacterBuilder.minifigDefinitions[ninjaID] : GameController.Instance.PlayerProfile.GetMinifigDefinition());
		UpdatePortraitTexture(portraitTexture);
		if ((bool)power)
		{
			power.gameObject.SetActive(true);
			power.sprite = component.powerSprites[(int)minifigDefinition.elementalPower];
		}
		if ((bool)weapon)
		{
			weapon.gameObject.SetActive(true);
			if (GameController.Instance.currentState is GameControllerStatePlaying)
			{
				UpdateWeaponSprite(minifigDefinition.weapon, (GameController.Instance.currentState as GameControllerStatePlaying).GetNinjaMissilesLeft(ninjaID));
			}
			else
			{
				weapon.sprite = component.weaponSprites[(int)minifigDefinition.weapon];
			}
		}
		if ((bool)background)
		{
			background.color = component.powerColors[(int)minifigDefinition.elementalPower];
		}
		SetFrameState(state);
		SetPortraitAndBackgroundState(state);
	}

	public void UpdatePortraitTexture(Texture portraitTexture)
	{
		if ((bool)portrait)
		{
			portrait.gameObject.SetActive(true);
			portrait.texture = portraitTexture;
		}
	}

	public void UpdateWeaponSprite(WeaponType weaponType, int missileCount = 0)
	{
		NinjaPortraitData component = GameController.Instance.GetComponent<NinjaPortraitData>();
		switch (missileCount)
		{
		default:
			weapon.sprite = component.weaponSprites[(int)weaponType];
			break;
		case 1:
			weapon.sprite = component.missile1Sprites[(int)weaponType];
			break;
		case 2:
			weapon.sprite = component.missile2prites[(int)weaponType];
			break;
		}
	}

	public void SetPortraitAndBackgroundState(NinjaPortraitState state)
	{
		Color color = background.color;
		Color color2 = portrait.color;
		if (state == NinjaPortraitState.Disabled || state == NinjaPortraitState.LockedMenu)
		{
			color.a = 0.5f;
			background.color = color;
			color2.a = 0.5f;
			portrait.color = color2;
		}
		else
		{
			color.a = 1f;
			background.color = color;
			color2.a = 1f;
			portrait.color = color2;
		}
	}

	public void SetFrameState(NinjaPortraitState state)
	{
		NinjaPortraitData component = GameController.Instance.GetComponent<NinjaPortraitData>();
		for (int i = 0; i < frame.Length; i++)
		{
			frame[i].color = component.FrameColor(state);
		}
	}

	public void BlinkPowerIcon()
	{
		if (blinkCoroutine != null)
		{
			StopCoroutine(blinkCoroutine);
			blinkCoroutine = null;
		}
		blinkCoroutine = StartCoroutine(DoBlinkElement(power.gameObject, 1.5f));
	}

	private IEnumerator DoBlinkElement(GameObject go, float duration, float interval = 0.15f)
	{
		float startTime = Time.time;
		float lastBlinkTime = Time.time;
		go.SetActive(true);
		while (Time.time - startTime <= duration)
		{
			if (Time.time > lastBlinkTime + interval)
			{
				go.SetActive(!go.activeSelf);
				lastBlinkTime = Time.time;
			}
			yield return 0;
		}
		go.SetActive(true);
	}
}
