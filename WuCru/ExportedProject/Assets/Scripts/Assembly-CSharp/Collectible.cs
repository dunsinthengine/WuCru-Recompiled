using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Collectible : MonoBehaviour
{
	public delegate void OnPickupDelegate(Level level, Collectible instance);

	public LevelNode levelNode;

	public float attractionDuration = 1.2f;

	private List<string> objectTags = new List<string>();

	public OnPickupDelegate OnPickupTrigger;

	private void Awake()
	{
		OnPickupTrigger = (OnPickupDelegate)Delegate.Combine(OnPickupTrigger, new OnPickupDelegate(GameController.Instance.ObjectiveController.OnCollectiblePickup));
		if (GameController.Instance.CurrentLevel != null && GameController.Instance.CurrentLevel.Challenge != null)
		{
			OnPickupTrigger = (OnPickupDelegate)Delegate.Combine(OnPickupTrigger, new OnPickupDelegate(GameController.Instance.CurrentLevel.Challenge.OnCollectiblePickup));
		}
	}

	public void AddObjectTag(string tag)
	{
		objectTags.Add(tag);
	}

	public bool HasObjectTag(string tag)
	{
		foreach (string objectTag in objectTags)
		{
			if (objectTag == tag)
			{
				return true;
			}
		}
		return false;
	}

	private void OnTriggerEnter(Collider other)
	{
		Character component = other.gameObject.GetComponent<Character>();
		if (component != null && component.isPlayer)
		{
			OnBasePickup(component);
		}
	}

	private void OnBasePickup(Character character)
	{
		if (OnPickupTrigger != null)
		{
			OnPickupTrigger(GameController.Instance.CurrentLevel, this);
		}
		OnPickup(character);
	}

	protected abstract void OnPickup(Character character);

	public virtual void StartAttract()
	{
		StartCoroutine(DoAttractBase());
	}

	private IEnumerator DoAttractBase()
	{
		float startTime = Time.time;
		float t = 0f;
		while (t <= 1f)
		{
			yield return null;
			t = (Time.time - startTime) / attractionDuration;
			if (t > 1f)
			{
				t = 1f;
			}
			base.transform.position = Vector3.Lerp(base.transform.position, GamePlayer.Instance.Player.Character.transform.position, t);
		}
	}
}
