using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NinjaTeamGUI : MonoBehaviour
{
	public NinjaPortrait[] portraits;

	public NinjaPortrait activeNinjaPortrait;

	public RawImage imageActiveNinja;

	public Dictionary<int, Texture2D> characterTextureDict = new Dictionary<int, Texture2D>();

	private List<Button> buttons;

	private Material hiddenMaterial;

	private GameObject fxGO;

	private float swapTime = 0.5f;

	private bool isSwapping;

	private void Awake()
	{
		if (GamePlayer.Instance != null)
		{
			Character character = GamePlayer.Instance.Player.Character;
			character.onPlayerDead = (Character.OnPlayerDeadDelegate)Delegate.Combine(character.onPlayerDead, new Character.OnPlayerDeadDelegate(OnPlayerDead));
		}
		hiddenMaterial = new Material(portraits[1].portrait.material);
		hiddenMaterial.color = new Color(1f, 1f, 1f, 0.3f);
		buttons = new List<Button>();
		CanvasScaler componentInParent = GetComponentInParent<CanvasScaler>();
		float num = portraits[1].portrait.rectTransform.sizeDelta.x / componentInParent.referenceResolution.x;
		float num2 = portraits[1].portrait.rectTransform.sizeDelta.y / portraits[1].portrait.rectTransform.sizeDelta.x;
		float num3 = (float)(2 * Screen.width) * num;
		float num4 = (float)(2 * Screen.width) * num * num2;
		Texture2D[] array = CharacterPreviewRenderer.Generate(GameController.Instance.PlayerProfile.NinjaTeam, (int)num3, (int)num4);
		portraits[0].Configure(GameController.Instance.PlayerProfile.GetNinjaInTeam(0), array[0], NinjaPortraitState.Selected);
		for (int i = 0; i < GameController.Instance.PlayerProfile.NinjaTeam.Length && i < portraits.Length - 1; i++)
		{
			int ninjaInTeam = GameController.Instance.PlayerProfile.GetNinjaInTeam(i);
			if (ninjaInTeam > -1)
			{
				buttons.Add(portraits[i + 1].button);
				portraits[i + 1].Configure(ninjaInTeam, array[i], (i == 0) ? NinjaPortraitState.Selected : NinjaPortraitState.Active);
				characterTextureDict[ninjaInTeam] = array[i];
			}
		}
		portraits[1].button.onClick.AddListener(() =>
		{
			if (!GamePlayer.Instance.Player.Character.IsInSpinjitzu())
			{
				AudioPlayer.PlaySound("ui/button/spinner");
				SwapNinja(0, portraits[1].button);
			}
		});
		portraits[2].button.onClick.AddListener(() =>
		{
			if (!GamePlayer.Instance.Player.Character.IsInSpinjitzu())
			{
				AudioPlayer.PlaySound("ui/button/spinner");
				int ninjaInTeam2 = GameController.Instance.PlayerProfile.GetNinjaInTeam(1);
				if (ninjaInTeam2 > 0)
				{
					SwapNinja(ninjaInTeam2, portraits[2].button);
				}
			}
		});
		portraits[3].button.onClick.AddListener(() =>
		{
			if (!GamePlayer.Instance.Player.Character.IsInSpinjitzu())
			{
				AudioPlayer.PlaySound("ui/button/spinner");
				int ninjaInTeam2 = GameController.Instance.PlayerProfile.GetNinjaInTeam(2);
				if (ninjaInTeam2 > 0)
				{
					SwapNinja(ninjaInTeam2, portraits[3].button);
				}
			}
		});
		portraits[4].button.onClick.AddListener(() =>
		{
			if (!GamePlayer.Instance.Player.Character.IsInSpinjitzu())
			{
				AudioPlayer.PlaySound("ui/button/spinner");
				int ninjaInTeam2 = GameController.Instance.PlayerProfile.GetNinjaInTeam(3);
				if (ninjaInTeam2 > 0)
				{
					SwapNinja(ninjaInTeam2, portraits[4].button);
				}
			}
		});
		portraits[5].button.onClick.AddListener(() =>
		{
			if (!GamePlayer.Instance.Player.Character.IsInSpinjitzu())
			{
				AudioPlayer.PlaySound("ui/button/spinner");
				int ninjaInTeam2 = GameController.Instance.PlayerProfile.GetNinjaInTeam(4);
				if (ninjaInTeam2 > 0)
				{
					SwapNinja(ninjaInTeam2, portraits[5].button);
				}
			}
		});
		int[] ninjaTeam = GameController.Instance.PlayerProfile.NinjaTeam;
		foreach (int num6 in ninjaTeam)
		{
			int num7 = 0;
			if (num6 == -1)
			{
				buttons[num7].GetComponentInChildren<RawImage>().material = hiddenMaterial;
			}
			num7++;
		}
		for (int num8 = 0; num8 < portraits.Length; num8++)
		{
			if (GameController.Instance.PlayerProfile.NinjaTeam.Length <= num8)
			{
				if (num8 == 1)
				{
					portraits[2].health.gameObject.SetActive(false);
				}
				if (num8 == 2)
				{
					portraits[3].health.gameObject.SetActive(false);
				}
				if (num8 == 3)
				{
					portraits[4].health.gameObject.SetActive(false);
				}
				if (num8 == 4)
				{
					portraits[5].health.gameObject.SetActive(false);
				}
			}
		}
		if (GameController.Instance.PlayerProfile.NinjaTeam.Length <= 1)
		{
			portraits[1].transform.parent.gameObject.SetActive(false);
		}
		if (GameController.Instance.PlayerProfile.NinjaTeam.Length > 1 && GameController.Instance.PlayerProfile.FirstTimeCruShow)
		{
			StartCoroutine(DoCruBlink());
			GameController.Instance.PlayerProfile.FirstTimeCruShow = false;
		}
	}

	public void OnDestroy()
	{
		for (int i = 0; i < portraits.Length; i++)
		{
			RenderTexture renderTexture = portraits[i].portrait.texture as RenderTexture;
			if ((bool)renderTexture)
			{
				renderTexture.Release();
			}
		}
	}

	private IEnumerator DoCruBlink()
	{
		float startTime = Time.time;
		float duration = 4f;
		float interval = 0.05f;
		while (Time.time - startTime <= duration)
		{
			float t = (Time.time - startTime) / duration;
			foreach (Button button in buttons)
			{
				float num = (1f + Mathf.Sin(t * (1f / interval))) / 2f;
				button.transform.localScale = new Vector3(0.5f + num / 2f, 0.5f + num / 2f, 0.5f + num / 2f);
			}
			yield return 0;
		}
		foreach (Button button2 in buttons)
		{
			button2.transform.localScale = Vector3.one;
		}
	}

	public void SwapNinja(int ninjaID, Button button)
	{
		if (GamePlayer.Instance.Player.Character.NinjaID == ninjaID)
		{
			Debug.LogWarning("Cannot swap ninja, as NinjaID '" + ninjaID + "' is already active");
		}
		else if (GameController.Instance.MissionStats.deadNinjaIDs.Contains(ninjaID))
		{
			Debug.LogWarning("Cannot swap ninja, as NinjaID '" + ninjaID + "' is dead!");
		}
		else if (GamePlayer.Instance.Player.Character.IsPreparingCombat() || GamePlayer.Instance.Player.Character.IsInCombat() || GamePlayer.Instance.Player.Character.IsPreparingSneakAttack() || GamePlayer.Instance.Player.Character.IsSneakAttacking() || GamePlayer.Instance.Player.Character.IsClimbing())
		{
			Debug.LogWarning("Cannot swap ninja, as Character is performing action preventing swap...design?");
		}
		else if (isSwapping)
		{
			Debug.LogWarning("Cannot swap ninja, as swapping is already in progress");
		}
		else
		{
			StartCoroutine(DoSwapNinja(ninjaID, button));
		}
	}

	private IEnumerator DoSwapNinja(int ninjaID, Button button)
	{
		isSwapping = true;
		int oldNinjaID = GamePlayer.Instance.Player.Character.NinjaID;
		for (int i = 0; i < GameController.Instance.PlayerProfile.NinjaTeam.Length; i++)
		{
			int ninjaInTeam = GameController.Instance.PlayerProfile.GetNinjaInTeam(i);
			if (ninjaInTeam == oldNinjaID)
			{
				if (!GameController.Instance.MissionStats.deadNinjaIDs.Contains(oldNinjaID))
				{
					portraits[i + 1].Configure(oldNinjaID, characterTextureDict[oldNinjaID]);
				}
				else
				{
					portraits[i + 1].Configure(oldNinjaID, characterTextureDict[oldNinjaID], NinjaPortraitState.Disabled);
				}
			}
			else if (ninjaInTeam == ninjaID)
			{
				portraits[i + 1].Configure(ninjaID, characterTextureDict[ninjaID], NinjaPortraitState.Selected);
			}
		}
		portraits[0].Configure(ninjaID, characterTextureDict[ninjaID], NinjaPortraitState.Selected);
		GamePlayer.Instance.Player.Character.Animator.SetTrigger(TriggerHash.switchNinja);
		ElementalPower toPower = CharacterBuilder.minifigDefinitions[ninjaID].elementalPower;
		if (fxGO != null)
		{
			UnityEngine.Object.Destroy(fxGO);
		}
		switch (toPower)
		{
		case ElementalPower.Earth:
			fxGO = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Characters/FX/CFX2_WWExplosion Brown"));
			break;
		case ElementalPower.Electricity:
			fxGO = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Characters/FX/CFX2_WWExplosion Blue"));
			break;
		case ElementalPower.Energy:
			fxGO = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Characters/FX/CFX2_WWExplosion Green"));
			break;
		case ElementalPower.Fire:
			fxGO = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Characters/FX/CFX2_WWExplosion Red"));
			break;
		case ElementalPower.Ice:
			fxGO = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Characters/FX/CFX2_WWExplosion White"));
			break;
		case ElementalPower.Water:
			fxGO = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Characters/FX/CFX2_WWExplosion Dark Blue"));
			break;
		default:
			fxGO = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Characters/FX/SwapNinjaFX"));
			break;
		}
		fxGO.transform.position = GamePlayer.Instance.Player.Character.transform.position;
		SwapNinjaFX fx = fxGO.GetComponent<SwapNinjaFX>();
		if (GameController.Instance.currentState != null && GameController.Instance.currentState.GetType() == typeof(GameControllerStatePlaying))
		{
			HealthBar healthBar = UnityEngine.Object.FindObjectOfType<HealthBar>();
			if (healthBar != null)
			{
				healthBar.UpdateHealth(Mathf.RoundToInt((GameController.Instance.currentState as GameControllerStatePlaying).GetNinjaHealthFactor(ninjaID) * (float)GamePlayer.Instance.Player.Character.MaxHealth));
			}
		}
		yield return new WaitForSeconds(0.6f);
		CharacterBuilder.ConfigureMinifig(GamePlayer.Instance.Player.Character, ninjaID);
		Team.Configure(level: GameController.Instance.PlayerProfile.GetNinjaLevel(Team.GetTypeForID(ninjaID)), character: GamePlayer.Instance.Player.Character, id: ninjaID);
		(GameController.Instance.currentState as GameControllerStatePlaying).CurrentNinjaID = ninjaID;
		ScreenHud._pInstance.SetSpinjitsuType(GamePlayer.Instance.Player.Character.elementalPower);
		if (GamePlayer.Instance.Player.Character.GetWeapon().type == WeaponType.Shuriken && GamePlayer.Instance.Player.Character.MissilesLeft <= 0)
		{
			(GamePlayer.Instance.Player.Character.Controller as PlayerController).ChangeWeapon(WeaponType.Katana);
		}
		yield return new WaitForSeconds(swapTime);
		GamePlayer.Instance.Player.Character.Animator.SetTrigger(TriggerHash.reset);
		GamePlayer.Instance.Player.Character.Health = Mathf.RoundToInt((GameController.Instance.currentState as GameControllerStatePlaying).GetNinjaHealthFactor(ninjaID) * (float)GamePlayer.Instance.Player.Character.MaxHealth);
		isSwapping = false;
	}

	public void HideSlotWithNinjaID(int ninjaID)
	{
		if (ninjaID == 0)
		{
			portraits[1].portrait.material = hiddenMaterial;
		}
		else if (ninjaID == GameController.Instance.PlayerProfile.GetNinjaInTeam(1))
		{
			portraits[2].portrait.material = hiddenMaterial;
		}
		else if (ninjaID == GameController.Instance.PlayerProfile.GetNinjaInTeam(2))
		{
			portraits[3].portrait.material = hiddenMaterial;
		}
		else if (ninjaID == GameController.Instance.PlayerProfile.GetNinjaInTeam(3))
		{
			portraits[4].portrait.material = hiddenMaterial;
		}
		else if (ninjaID == GameController.Instance.PlayerProfile.GetNinjaInTeam(4))
		{
			portraits[5].portrait.material = hiddenMaterial;
		}
	}

	public void OnPlayerDead()
	{
		for (int i = 0; i < GameController.Instance.PlayerProfile.NinjaTeam.Length; i++)
		{
			if (GameController.Instance.PlayerProfile.GetNinjaInTeam(i) == GamePlayer.Instance.Player.Character.NinjaID)
			{
				portraits[i + 1].Configure(GamePlayer.Instance.Player.Character.NinjaID, characterTextureDict[GamePlayer.Instance.Player.Character.NinjaID], NinjaPortraitState.Disabled);
			}
		}
	}

	public void HighlightPowers(ElementalPower power)
	{
		if (GamePlayer.Instance.Player.Character.elementalPower != power && power != ElementalPower.None)
		{
			activeNinjaPortrait.BlinkPowerIcon();
			AudioPlayer.PlaySound("props/generic/fail");
		}
		for (int i = 0; i < GameController.Instance.PlayerProfile.NinjaTeam.Length && i < portraits.Length - 1; i++)
		{
			int ninjaInTeam = GameController.Instance.PlayerProfile.GetNinjaInTeam(i);
			if (GameController.Instance.MissionStats.deadNinjaIDs.Contains(ninjaInTeam))
			{
				continue;
			}
			if (i == 0)
			{
				ElementalPower elementalPower = GameController.Instance.PlayerProfile.ElementalPower;
				if (elementalPower == power && GamePlayer.Instance.Player.Character.NinjaID != ninjaInTeam)
				{
					portraits[1].BlinkPowerIcon();
				}
			}
			else if (ninjaInTeam > -1)
			{
				ElementalPower elementalPower2 = CharacterBuilder.minifigDefinitions[ninjaInTeam].elementalPower;
				if (elementalPower2 == power && GamePlayer.Instance.Player.Character.NinjaID != ninjaInTeam)
				{
					portraits[i + 1].BlinkPowerIcon();
				}
			}
		}
	}
}
