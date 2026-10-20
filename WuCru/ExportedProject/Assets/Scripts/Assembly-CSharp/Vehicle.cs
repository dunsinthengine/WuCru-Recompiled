using System;
using System.Collections;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
	public int id;

	public int health;

	[NonSerialized]
	public bool isEnemy;

	private float animationSpeed;

	public Transform[] smokeEmitters;

	public Transform[] muzzleFlashEmitters;

	public GameObject smokePartileSystem;

	public GameObject muzzleFlashParticleSystem;

	private Coroutine animateCoroutine;

	private Animation animation;

	public Transform[] rotators;

	public Vector3[] rotationAxis;

	public float[] rotationSpeed;

	public float AnimationSpeed
	{
		get
		{
			return animationSpeed;
		}
		set
		{
			animationSpeed = value;
			if (animation != null)
			{
				animation[animation.clip.name].speed = animationSpeed;
			}
		}
	}

	public Vector3 missileSpawnPoint
	{
		get
		{
			Vector3 zero = Vector3.zero;
			for (int i = 0; i < muzzleFlashEmitters.Length; i++)
			{
				zero += muzzleFlashEmitters[i].position;
			}
			return zero / muzzleFlashEmitters.Length;
		}
	}

	private void Awake()
	{
		animation = GetComponentInChildren<Animation>();
		AnimationSpeed = 0f;
	}

	private void Update()
	{
		int num = 0;
		while (rotators != null && num < rotators.Length && num < rotationAxis.Length && num < rotationSpeed.Length)
		{
			rotators[num].Rotate(rotationAxis[num], rotationSpeed[num] * Time.deltaTime * animationSpeed, Space.Self);
			num++;
		}
	}

	public void Animate(float duration)
	{
		if (animateCoroutine == null)
		{
			animateCoroutine = StartCoroutine(DoAnimate(duration));
		}
	}

	private IEnumerator DoAnimate(float duration)
	{
		string sound = "player/traversal/driving/loop";
		AudioPlayer.PlaySound(sound, base.gameObject);
		AnimationSpeed = 1f;
		float startTime = Time.time;
		while (Time.time - startTime <= duration)
		{
			yield return 0;
		}
		AnimationSpeed = 0f;
		AudioPlayer.StopSound(sound, base.gameObject);
		animateCoroutine = null;
	}
}
