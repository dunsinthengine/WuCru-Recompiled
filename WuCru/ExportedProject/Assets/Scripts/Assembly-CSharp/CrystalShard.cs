using System;
using System.Collections;
using UnityEngine;

public class CrystalShard : InteractableObject
{
	public ParticleSystem lightParticles;

	public ElementalPower elementalPower;

	public SpriteRenderer icon;

	public float iconFadeDuration = 0.4f;

	private Character activeCharacter;

	private RangeSphere rangeSphere;

	[Header("Audio")]
	[SerializeField]
	private string explosionAudioEvent = "props/dark/crystal/explode";

	[SerializeField]
	private string humLoopAudioEvent = "props/crystal/hum";

	[SerializeField]
	private ParticleSystem parryParticleSystem;

	private float explosionDelay = 0.2f;

	private bool hasExploded;

	private Color materialStartColor;

	private bool materialStartColorSet;

	public override int PickingPriority()
	{
		return 1;
	}

	protected override void OnInitialize()
	{
		if (icon != null)
		{
			icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 0f);
		}
		AudioPlayer.PlaySound(humLoopAudioEvent, base.gameObject);
		activateBlink = ActivateBlink.ON_PICK;
		materialStartColor = renderers[1].material.GetColor("_EmissionColor");
		materialStartColorSet = true;
	}

	protected override void OnEnter(Character character)
	{
		if (character.isPlayer && !hasExploded)
		{
			Debug.Log("TODO");
		}
		if (hasExploded || !(GamePlayer.Instance.SelectedObject == base.gameObject))
		{
			return;
		}
		if (elementalPower == ElementalPower.None || elementalPower == character.elementalPower)
		{
			activeCharacter = character;
			if (character.IsInSpinjitzu())
			{
				StartCoroutine(DoExplode(10f));
				base.HasTriggered = true;
			}
			else if (!ScreenHud._pInstance._zoneTapMinigame._pIsActive)
			{
				ScreenHud._pInstance._zoneTapMinigame.BeginMinigame();
				HUDZoneTapMinigame zoneTapMinigame = ScreenHud._pInstance._zoneTapMinigame;
				zoneTapMinigame.OnClick = (HUDZoneTapMinigame.OnClickDelegate)Delegate.Combine(zoneTapMinigame.OnClick, new HUDZoneTapMinigame.OnClickDelegate(SwingMeterClick));
			}
		}
		else
		{
			ScreenHud._pInstance._hudElementDisplay.DisplayIcon(elementalPower, base.transform, true);
			if (character.isPlayer && character.Animator.GetBool(VarHash.isMoving))
			{
				character.Animator.SetBool(VarHash.isMoving, false);
				character.ResetPath();
			}
		}
	}

	private IEnumerator FadeIcon()
	{
		Debug.Log("Animating icon");
		Color color = icon.color;
		float startTime = Time.time;
		while (Time.time - startTime < iconFadeDuration)
		{
			float a = Mathf.Clamp01((Time.time - startTime) / iconFadeDuration);
			icon.color = new Color(color.r, color.g, color.b, a);
			yield return 0;
		}
		yield return new WaitForSeconds(2f);
		startTime = Time.time;
		while (Time.time - startTime < iconFadeDuration)
		{
			float a2 = Mathf.Clamp01((Time.time - startTime) / iconFadeDuration);
			icon.color = new Color(color.r, color.g, color.b, 1f - a2);
			yield return 0;
		}
		icon.color = new Color(color.r, color.g, color.b, 0f);
		base.HasTriggered = false;
	}

	public void SwingMeterClick(bool success, float amount)
	{
		AnalyticsEvents.LogCrystalSmash(success);
		if (success)
		{
			base.HasTriggered = true;
			hasExploded = true;
			StartCoroutine(DoExplode(amount));
		}
		else
		{
			parryParticleSystem.Play();
			AudioPlayer.PlaySound("player/sword/parry", base.gameObject);
			base.HasTriggered = false;
		}
	}

	protected override void OnExit(Character character)
	{
	}

	public override void OnPicked(Character character, bool tapDown)
	{
		if (playerIsInTriggerZone && !hasExploded && !ScreenHud._pInstance._zoneTapMinigame._pIsActive)
		{
			if (elementalPower == ElementalPower.None || elementalPower == character.elementalPower)
			{
				activeCharacter = GamePlayer.Instance.Player.Character;
				ScreenHud._pInstance._zoneTapMinigame.BeginMinigame();
				HUDZoneTapMinigame zoneTapMinigame = ScreenHud._pInstance._zoneTapMinigame;
				zoneTapMinigame.OnClick = (HUDZoneTapMinigame.OnClickDelegate)Delegate.Combine(zoneTapMinigame.OnClick, new HUDZoneTapMinigame.OnClickDelegate(SwingMeterClick));
			}
		}
		else
		{
			character.Controller.WalkTo(levelNode);
		}
	}

	protected override void SetOutlineSwitchValue(float value)
	{
		if (!materialStartColorSet)
		{
			return;
		}
		Color color = Color.Lerp(materialStartColor, Color.white, value);
		if (renderers == null)
		{
			return;
		}
		MeshRenderer[] array = renderers;
		foreach (MeshRenderer meshRenderer in array)
		{
			if (meshRenderer != null && meshRenderer.enabled)
			{
				meshRenderer.material.SetColor("_EmissionColor", color);
			}
		}
	}

	public void Explode(float amount)
	{
		activeCharacter = GamePlayer.Instance.Player.Character;
		StartCoroutine(DoExplode(amount));
	}

	private IEnumerator DoExplode(float amount)
	{
		Debug.Log("DoExplode");
		Vector3 originalPos = base.gameObject.transform.position;
		if (!GamePlayer.Instance.Player.Character.IsInSpinjitzu())
		{
			yield return new WaitForSeconds(explosionDelay);
		}
		MeshRenderer[] componentsInChildren = base.transform.parent.GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer meshRenderer in componentsInChildren)
		{
			meshRenderer.gameObject.AddComponent<Rigidbody>();
			MeshCollider meshCollider = meshRenderer.gameObject.AddComponent<MeshCollider>();
			meshCollider.convex = true;
		}
		GamePlayer.Instance.ScreenFlash.FlashAlpha(0.5f, 0.5f, Color.magenta);
		base.transform.parent.gameObject.AddComponent<CollapseEFX>();
		ParticleSystem[] componentsInChildren2 = base.transform.parent.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem particleSystem in componentsInChildren2)
		{
			if (particleSystem != lightParticles)
			{
				particleSystem.Play();
			}
			else
			{
				particleSystem.Stop();
			}
		}
		bool canShake = true;
		if (elementalPower == activeCharacter.elementalPower)
		{
			MinifigType minifigType = MinifigType.None;
			if (activeCharacter.NinjaID == 0)
			{
				switch (activeCharacter.elementalPower)
				{
				case ElementalPower.Earth:
					minifigType = MinifigType.Cole;
					break;
				case ElementalPower.Electricity:
					minifigType = MinifigType.Jay;
					break;
				case ElementalPower.Energy:
					minifigType = MinifigType.Lloyd;
					break;
				case ElementalPower.Fire:
					minifigType = MinifigType.Kai;
					break;
				case ElementalPower.Ice:
					minifigType = MinifigType.Zane;
					break;
				case ElementalPower.Water:
					minifigType = MinifigType.Nya;
					break;
				}
			}
			else
			{
				minifigType = Team.GetTypeForID(activeCharacter.NinjaID);
			}
			switch (minifigType)
			{
			case MinifigType.Cole:
				spawnTypePrefabName = "Prefabs/NinjaOrbs/ColeOrb";
				GameController.Instance.PlayerProfile.ColeTokens++;
				break;
			case MinifigType.Jay:
				spawnTypePrefabName = "Prefabs/NinjaOrbs/JayOrb";
				GameController.Instance.PlayerProfile.JayTokens++;
				break;
			case MinifigType.Kai:
				spawnTypePrefabName = "Prefabs/NinjaOrbs/KaiOrb";
				GameController.Instance.PlayerProfile.KaiTokens++;
				break;
			case MinifigType.Zane:
				spawnTypePrefabName = "Prefabs/NinjaOrbs/ZaneOrb";
				GameController.Instance.PlayerProfile.ZaneTokens++;
				break;
			case MinifigType.Nya:
				spawnTypePrefabName = "Prefabs/NinjaOrbs/NyaOrb";
				GameController.Instance.PlayerProfile.NyaTokens++;
				break;
			case MinifigType.Lloyd:
				spawnTypePrefabName = "Prefabs/NinjaOrbs/Lloydrb";
				GameController.Instance.PlayerProfile.LloydTokens++;
				break;
			}
			int num = GameController.Instance.PlayerProfile.ColeTokens + GameController.Instance.PlayerProfile.JayTokens + GameController.Instance.PlayerProfile.KaiTokens + GameController.Instance.PlayerProfile.ZaneTokens + GameController.Instance.PlayerProfile.NyaTokens + GameController.Instance.PlayerProfile.LloydTokens;
			if (num == 1)
			{
				canShake = false;
				ScreenHud._pInstance.TriggerLevellingCruFTUE();
			}
		}
		if (canShake)
		{
			InGameCam._pInstance.Shake(0.16f, 0.5f);
			GamePlayer.Instance.TimeBlink(0.05f);
			if (GameController.Instance.currentState.GetType() == typeof(GameControllerStatePlaying))
			{
				int num2 = GameController.Instance.CurrentLevel.ChunkDB.GetNumObjectsOfType(LevelNodeObjectType.CrystalShard);
				for (LevelNodeObjectType levelNodeObjectType = LevelNodeObjectType.CrystalShard_earth; levelNodeObjectType <= LevelNodeObjectType.CrystalShard_water; levelNodeObjectType++)
				{
					num2 += GameController.Instance.CurrentLevel.ChunkDB.GetNumObjectsOfType(levelNodeObjectType);
				}
				int num3 = GameController.Instance.CurrentLevel.ChunkDB.GetNumObjectOfTypePicked(LevelNodeObjectType.CrystalShard);
				for (LevelNodeObjectType levelNodeObjectType2 = LevelNodeObjectType.CrystalShard_earth; levelNodeObjectType2 <= LevelNodeObjectType.CrystalShard_water; levelNodeObjectType2++)
				{
					num3 += GameController.Instance.CurrentLevel.ChunkDB.GetNumObjectOfTypePicked(levelNodeObjectType2);
				}
				num3++;
				ScreenHud._pInstance._poolMessagesGreenNumbers.TriggerMessage(base.gameObject.transform.position, num3 + "/" + num2);
			}
		}
		if (OnInteract != null)
		{
			OnInteract(GameController.Instance.CurrentLevel, this);
		}
		if (spawnTypePrefabName.Length > 0)
		{
			StartCoroutine(SpawnObject());
		}
		AudioPlayer.PlaySound(explosionAudioEvent, base.gameObject);
		AudioPlayer.StopSound(humLoopAudioEvent, base.gameObject);
		rangeSphere = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/FXs/RangeSphere"))).GetComponent<RangeSphere>();
		rangeSphere.transform.parent = base.transform;
		rangeSphere.transform.localPosition = Vector3.zero;
		float baseExplosionRange = 10f;
		float amountExplosion = baseExplosionRange * amount;
		rangeSphere.Show(amountExplosion, 1f);
		int baseDamage = 9001;
		int damFalloffFactor = 1;
		yield return new WaitForSeconds(0.5f);
		if (GameController.Instance.currentState.GetType() == typeof(GameControllerStatePlaying))
		{
			AIAgent[] agents = GameController.Instance.CurrentLevel.Agents;
			foreach (AIAgent aIAgent in agents)
			{
				if (aIAgent is ElementalBossEnemy || aIAgent.isFriend)
				{
					continue;
				}
				float num4 = Vector3.Distance(originalPos, aIAgent.Character.transform.position);
				if (num4 <= amountExplosion)
				{
					int num5 = Mathf.RoundToInt(num4);
					if (num5 < 1)
					{
						num5 = 1;
					}
					int amount2 = baseDamage / num5 / damFalloffFactor;
					aIAgent.Character.TakeDamage(amount2, aIAgent.Character.transform.position, DamageType.Explosion);
				}
			}
		}
		else
		{
			AIAgent[] agents2 = EditorStateMachine.Instance.level.Agents;
			foreach (AIAgent aIAgent2 in agents2)
			{
				if (aIAgent2.isFriend)
				{
					continue;
				}
				float num6 = Vector3.Distance(originalPos, aIAgent2.Character.transform.position);
				if (num6 <= amountExplosion)
				{
					int num7 = Mathf.RoundToInt(num6);
					if (num7 < 1)
					{
						num7 = 1;
					}
					int amount3 = baseDamage / num7 / damFalloffFactor;
					aIAgent2.Character.TakeDamage(amount3, aIAgent2.Character.transform.position, DamageType.Explosion);
				}
			}
		}
		GamePlayer.Instance.Level.ChunkDB.PickupLoot(levelNode.ObjectTypes[0].objectType);
		yield return new WaitForSeconds(2f);
		MeshRenderer[] componentsInChildren3 = base.transform.parent.GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer meshRenderer2 in componentsInChildren3)
		{
			meshRenderer2.gameObject.SetActive(false);
		}
		UnityEngine.Object.Destroy(base.transform.parent.gameObject);
	}

	public static LevelNodeObjectType GetElementalPowerType(Level level)
	{
		ElementalPower elementalPower = level.PrimaryPower;
		if (UnityEngine.Random.value < 0.34f)
		{
			elementalPower = level.SecondaryPower;
		}
		switch (elementalPower)
		{
		case ElementalPower.Earth:
			return LevelNodeObjectType.CrystalShard_earth;
		case ElementalPower.Electricity:
			return LevelNodeObjectType.CrystalShard_electricity;
		case ElementalPower.Energy:
			return LevelNodeObjectType.CrystalShard_energy;
		case ElementalPower.Fire:
			return LevelNodeObjectType.CrystalShard_fire;
		case ElementalPower.Ice:
			return LevelNodeObjectType.CrystalShard_ice;
		case ElementalPower.Water:
			return LevelNodeObjectType.CrystalShard_water;
		case ElementalPower.None:
			return LevelNodeObjectType.CrystalShard;
		default:
			return LevelNodeObjectType.CrystalShard;
		}
	}
}
