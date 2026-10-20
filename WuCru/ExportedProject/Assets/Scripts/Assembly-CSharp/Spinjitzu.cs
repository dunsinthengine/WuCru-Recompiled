using System.Collections;
using UnityEngine;

public class Spinjitzu : MonoBehaviour
{
	private GameObject spinjitzuGO;

	private float spinjitzuTiltAmount = 15f;

	private Vector3 spinjitzuTiltAxis = Vector3.up;

	private bool useInstability = true;

	private bool useDrift = true;

	private float instabilityFactor = 1f;

	private float driftFactor = 1f;

	private float driftSpeed = 1.6f;

	private bool _wantCancel;

	public bool _pWantCancel
	{
		get
		{
			return _wantCancel;
		}
		set
		{
			_wantCancel = value;
		}
	}

	private void Awake()
	{
		base.enabled = false;
		SwordTappingIcon componentInChildren = GamePlayer.Instance.Player.Character.GetComponentInChildren<SwordTappingIcon>();
		ScreenHud._pInstance._hudBattleMinigame.CloseMinigame();
		componentInChildren.Show(false);
		ScreenHud._pInstance._swordTapTelegraph.Hide();
		StartCoroutine(DoSpinjitzu(GamePlayer.Instance.Player.Character));
	}

	private IEnumerator DoSpinjitzu(Character character)
	{
		if (Time.timeScale != 1f)
		{
			GamePlayer.Instance.SetTimeScale(1f, 0.2f);
		}
		spinjitzuTiltAxis = Vector3.up;
		float startTime = Time.time;
		while (!character.IsInSpinjitzu())
		{
			yield return 0;
		}
		spinjitzuGO = (GameObject)Object.Instantiate(Resources.Load(SpinjitzuPrefabName(character)));
		spinjitzuGO.transform.parent = base.gameObject.transform;
		spinjitzuGO.transform.localPosition = Vector3.zero;
		spinjitzuGO.transform.localScale = Vector3.zero;
		startTime = Time.time;
		float duration = 0.625f;
		string spinjitzuAudioEvent = spinjitzuGO.GetComponent<SpinjitzuAudio>().SpinjitzuAudioEvent;
		AudioPlayer.PlaySound(spinjitzuAudioEvent, spinjitzuGO);
		while (startTime + duration > Time.time && !character.IsDead())
		{
			float t = (Time.time - startTime) / duration;
			spinjitzuGO.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
			yield return 0;
		}
		character.Visible(false);
		while (character.CurrentStateNameHash() != StateHash.spintitzu && !character.IsDead())
		{
			yield return 0;
		}
		startTime = Time.time;
		float hCurrent = 0f;
		float vCurrent = 0f;
		float hStep = -0.38f;
		float vStep = 0.45f;
		float rangeMin = 0.02f;
		float rangeMax = 0.16f;
		float vMin = Random.Range((0f - rangeMax) * instabilityFactor, (0f - rangeMin) * instabilityFactor);
		float vMax = Random.Range(rangeMin * instabilityFactor, rangeMax * instabilityFactor);
		float hMin = Random.Range((0f - rangeMax) * instabilityFactor, (0f - rangeMin) * instabilityFactor);
		float hMax = Random.Range(rangeMin * instabilityFactor, rangeMax * instabilityFactor);
		float driftAngle = Random.Range(0f, 360f);
		float driftAngleStep = 60f;
		while (startTime + character.SpinjitzuDuration > Time.time && !character.IsDead() && !_pWantCancel)
		{
			if (useDrift && !Input.GetMouseButton(0))
			{
				driftAngle += driftAngleStep * Time.deltaTime * Random.Range(0.5f, 4f);
				if (Random.value <= 0.02f)
				{
					driftAngleStep = 0f - driftAngleStep;
				}
				driftAngle %= 360f;
				Quaternion quaternion = Quaternion.AngleAxis(driftAngle, Vector3.up);
				Vector3 direction = quaternion * Vector3.forward;
				direction.y = 0f;
				character.MoveTowards(direction, driftSpeed * driftFactor, float.MaxValue);
			}
			Vector3 tiltAxis = Vector3.Cross(Vector3.up, character.Velocity);
			if (character.Velocity.magnitude == 0f)
			{
				tiltAxis = Vector3.up;
			}
			spinjitzuTiltAxis = Vector3.Slerp(spinjitzuTiltAxis, tiltAxis, 2.5f * Time.deltaTime);
			spinjitzuGO.transform.rotation = Quaternion.AngleAxis(0f - spinjitzuTiltAmount, spinjitzuTiltAxis);
			(character.Controller as PlayerController).HandlePicking();
			if (character.Controller.CurrentEnemy != null && !character.Controller.CurrentEnemy.IsDead())
			{
				Vector3 position = character.Controller.CurrentEnemy.transform.position;
				if ((position - character.transform.position).magnitude > 0.7f && character.CurrentDestination != character.Controller.CurrentEnemy.CurrentPositionNode)
				{
					character.ResetPath();
					character.WalkTo(character.Controller.CurrentEnemy.CurrentPositionNode);
				}
			}
			if (character.HasPath())
			{
				character.MoveStep();
			}
			character.SpinjitzuProgression = Mathf.Clamp01((Time.time - startTime) / character.SpinjitzuDuration);
			if (useInstability)
			{
				hCurrent += hStep * Time.deltaTime;
				if (hStep > 0f && hCurrent >= hMax)
				{
					hStep = 0f - hStep;
					hMin = Random.Range((0f - rangeMax) * instabilityFactor, (0f - rangeMin) * instabilityFactor);
				}
				if (hStep < 0f && hCurrent <= hMin)
				{
					hStep = 0f - hStep;
					hMax = Random.Range(rangeMin * instabilityFactor, rangeMax * instabilityFactor);
				}
				vCurrent += vStep * Time.deltaTime;
				if (vStep > 0f && vCurrent >= vMax)
				{
					vStep = 0f - vStep;
					vMin = Random.Range((0f - rangeMax) * instabilityFactor, (0f - rangeMin) * instabilityFactor);
				}
				if (vStep < 0f && vCurrent <= vMin)
				{
					vStep = 0f - vStep;
					vMax = Random.Range(rangeMin * instabilityFactor, rangeMax * instabilityFactor);
				}
				character.positionOffset = new Vector3(hCurrent, 0f, vCurrent);
			}
			yield return 0;
		}
		character.positionOffset = Vector3.zero;
		character.transform.position = new Vector3(character.transform.position.x, character.CurrentPositionNode.Elevation, character.transform.position.z);
		character.animator.SetTrigger(TriggerHash.stopSpinjitzu);
		AudioPlayer.StopSound(spinjitzuAudioEvent, spinjitzuGO);
		startTime = Time.time;
		duration = 0.5f;
		character.Visible(true);
		while (startTime + duration > Time.time && !character.IsDead())
		{
			float t2 = (Time.time - startTime) / duration;
			spinjitzuGO.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t2);
			yield return 0;
		}
		Object.Destroy(spinjitzuGO);
		spinjitzuGO = null;
		character.SpinjitzuProgression = 0f;
		character.SpinjitzuInProgress = false;
		(character.Controller as PlayerController).OnSpinjitzuDone();
		Object.Destroy(this);
	}

	private string SpinjitzuPrefabName(Character character)
	{
		switch (character.elementalPower)
		{
		case ElementalPower.Earth:
			return character.spinjitzuPrefabNameRoot + "Spinjitzu_Cole";
		case ElementalPower.Fire:
			return character.spinjitzuPrefabNameRoot + "Spinjitzu_Kai";
		case ElementalPower.Ice:
			return character.spinjitzuPrefabNameRoot + "Spinjitzu_Zane";
		case ElementalPower.Electricity:
			return character.spinjitzuPrefabNameRoot + "Spinjitzu_Jay";
		case ElementalPower.Energy:
			return character.spinjitzuPrefabNameRoot + "Spinjitzu_Lloyd";
		case ElementalPower.Water:
			return character.spinjitzuPrefabNameRoot + "Spinjitzu_Nya";
		default:
			Debug.LogError("Unknown elemental power in Spinjitzu!");
			return character.spinjitzuPrefabNameRoot + "Spinjitzu_Kai";
		}
	}
}
