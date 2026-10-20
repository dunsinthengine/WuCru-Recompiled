using System;
using UnityEngine;

public class BellTower : InteractableObject
{
	public Animator animator;

	private float distractionRangeSuccess = 10f;

	private float distractionRangeFailed = 3f;

	private bool clicked;

	private Character clickedByCharacter;

	private RangeSphere rangeSphere;

	public override int PickingPriority()
	{
		return 14;
	}

	protected override void OnInitialize()
	{
		animator = GetComponent<Animator>();
		onlyPlayer = true;
		rangeSphere = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/FXs/RangeSphere"))).GetComponent<RangeSphere>();
		rangeSphere.transform.parent = base.transform;
		rangeSphere.transform.localPosition = Vector3.zero;
		activateBlink = ActivateBlink.ON_PICK;
	}

	public void SwingMeterClick(bool success, float amount)
	{
		clicked = false;
		float num = distractionRangeSuccess * amount;
		if (!success)
		{
			num = distractionRangeFailed;
		}
		AIBeacon.Instance.ReportEvent(null, levelNode, base.transform.position, num, 1f);
		animator.SetBool("Triggered", true);
		AnalyticsEvents.LogDistrationTriggering(success);
		if (success)
		{
			SuccessHit(num);
		}
		else
		{
			FailHit(num);
		}
	}

	public void SuccessHit(float range)
	{
		GamePlayer.Instance.ScreenFlash.FlashAlpha(0.5f, 2.5f, Color.yellow);
		AudioPlayer.PlaySound("props/gong/hit", base.gameObject);
		rangeSphere.Show(range, 1f);
		if (OnInteract != null)
		{
			OnInteract(GameController.Instance.CurrentLevel, this);
		}
	}

	public void FailHit(float range)
	{
		rangeSphere.Show(range, 0.2f);
		AudioPlayer.PlaySound("props/gong/miss", base.gameObject);
	}

	protected override void OnEnter(Character character)
	{
		if (character.IsInSpinjitzu())
		{
			SuccessHit(distractionRangeSuccess);
		}
		else if (clicked)
		{
			ScreenHud._pInstance._zoneTapMinigame.BeginMinigame();
			HUDZoneTapMinigame zoneTapMinigame = ScreenHud._pInstance._zoneTapMinigame;
			zoneTapMinigame.OnClick = (HUDZoneTapMinigame.OnClickDelegate)Delegate.Combine(zoneTapMinigame.OnClick, new HUDZoneTapMinigame.OnClickDelegate(SwingMeterClick));
		}
	}

	protected override void OnExit(Character character)
	{
	}

	public override void OnPicked(Character character, bool tapDown)
	{
		clicked = true;
		clickedByCharacter = character;
		if (character.CurrentPositionNode == levelNode && !ScreenHud._pInstance._zoneTapMinigame._pIsActive)
		{
			OnEnter(character);
		}
		else
		{
			character.Controller.WalkTo(levelNode);
		}
	}
}
