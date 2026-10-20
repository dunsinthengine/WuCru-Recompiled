using System.Collections;
using Fabric;
using UnityEngine;

public class Key : Collectible
{
	private MeshRenderer[] renderers;

	private float rotationSpeed = 42f;

	public ParticleSystem idleFX;

	public ParticleSystem pickupFX;

	public MeshRenderer studRenderer;

	public int keyID;

	[Header("Audio")]
	public string KeyPickupAudioEvent;

	private void Start()
	{
		renderers = base.gameObject.GetComponentsInChildren<MeshRenderer>();
	}

	private void Update()
	{
		base.gameObject.transform.Rotate(Vector3.up, Time.deltaTime * rotationSpeed);
	}

	protected override void OnPickup(Character character)
	{
		character.AddDoorKey(keyID);
		StartCoroutine(DoPickup());
	}

	private IEnumerator DoPickup()
	{
		studRenderer.enabled = false;
		idleFX.Stop();
		pickupFX.gameObject.SetActive(true);
		pickupFX.Play();
		GamePlayer.Instance.ScreenFlash.FlashAlpha(0.3f, 2f, Color.white);
		EventManager.Instance.PostEvent(KeyPickupAudioEvent);
		while (!pickupFX.isStopped)
		{
			yield return null;
		}
		Object.Destroy(base.gameObject);
	}
}
