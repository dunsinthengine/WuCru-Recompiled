using System;
using Fabric;
using UnityEngine;

public class EndFlag : InteractableObject
{
	public bool isDarkCrystal;

	public ParticleSystem lightParticles;

	[Header("Audio")]
	[SerializeField]
	private string explosionAudioEvent = "props/dark/crystal/explode";

	[SerializeField]
	private string humLoopAudioEvent = "props/crystal/hum";

	private bool hasExploded;

	[SerializeField]
	private ParticleSystem parryParticleSystem;

	private Color materialStartColor;

	public override int PickingPriority()
	{
		return 1;
	}

	protected override void OnInitialize()
	{
		if (isDarkCrystal)
		{
			materialStartColor = renderers[0].material.GetColor("_EmissionColor");
			EventManager.Instance.PostEvent(humLoopAudioEvent, EventAction.SetVolumeProperty, 1f, base.gameObject.transform.parent.gameObject);
			EventManager.Instance.PostEvent(humLoopAudioEvent, EventAction.SetPitchProperty, 0.8f, base.gameObject.transform.parent.gameObject);
			AudioPlayer.PlaySound(humLoopAudioEvent, base.gameObject.transform.parent.gameObject);
		}
		activateBlink = ActivateBlink.ON_GOTO_NOTE;
	}

	protected override void OnEnter(Character character)
	{
		if (isDarkCrystal && !hasExploded)
		{
			ScreenHud._pInstance._zoneTapMinigame.BeginMinigame();
			HUDZoneTapMinigame zoneTapMinigame = ScreenHud._pInstance._zoneTapMinigame;
			zoneTapMinigame.OnClick = (HUDZoneTapMinigame.OnClickDelegate)Delegate.Combine(zoneTapMinigame.OnClick, new HUDZoneTapMinigame.OnClickDelegate(SwingMeterClick));
		}
		else
		{
			character.OnCharacterReachEndFlag();
		}
	}

	protected override void OnExit(Character character)
	{
	}

	public override void OnPicked(Character character, bool tapDown)
	{
		if (isDarkCrystal && playerIsInTriggerZone && !hasExploded)
		{
			ScreenHud._pInstance._zoneTapMinigame.BeginMinigame();
			HUDZoneTapMinigame zoneTapMinigame = ScreenHud._pInstance._zoneTapMinigame;
			zoneTapMinigame.OnClick = (HUDZoneTapMinigame.OnClickDelegate)Delegate.Combine(zoneTapMinigame.OnClick, new HUDZoneTapMinigame.OnClickDelegate(SwingMeterClick));
		}
		else
		{
			character.Controller.WalkTo(levelNode);
		}
	}

	protected override void SetOutlineSwitchValue(float value)
	{
		if (isDarkCrystal)
		{
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
		else
		{
			base.SetOutlineSwitchValue(value);
		}
	}

	public void SwingMeterClick(bool success, float amount)
	{
		if (success)
		{
			base.HasTriggered = true;
			hasExploded = true;
			Explode();
			SwingMeterSuccessHitStart();
		}
		else
		{
			parryParticleSystem.Play();
			AudioPlayer.PlaySound("player/sword/parry", base.gameObject);
			base.HasTriggered = false;
		}
	}

	public void SwingMeterSuccessHitStart()
	{
		InGameCam pInstance = InGameCam._pInstance;
		pInstance.Zoom(8f, 2f, 1f);
		pInstance.Shake(0.16f, 2f);
	}

	private void Explode()
	{
		InGameCam._pInstance.Shake(0.2f, 0.5f);
		AudioPlayer.PlaySound(explosionAudioEvent, FabricManager.Instance.gameObject);
		ExplodingObject componentInParent = base.gameObject.GetComponentInParent<ExplodingObject>();
		if (componentInParent != null)
		{
			MeshRenderer[] componentsInChildren = componentInParent.explodingObjectRoot.GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer meshRenderer in componentsInChildren)
			{
				Rigidbody rigidbody = meshRenderer.gameObject.AddComponent<Rigidbody>();
				MeshCollider meshCollider = meshRenderer.gameObject.AddComponent<MeshCollider>();
				meshCollider.convex = true;
			}
			componentInParent.explodingObjectRoot.AddComponent<CollapseEFX>();
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
		}
		GamePlayer.Instance.Player.Character.OnCharacterReachEndFlag();
	}
}
