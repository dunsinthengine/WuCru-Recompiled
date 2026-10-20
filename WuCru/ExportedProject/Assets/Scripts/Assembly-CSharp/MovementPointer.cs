using System.Collections;
using UnityEngine;

public class MovementPointer : MonoBehaviour
{
	public enum MovementPointerStyle
	{
		NONE = 0,
		POINTER = 1,
		JOYSTICK = 2,
		ERROR = 3
	}

	[SerializeField]
	private GameObject Pointer;

	[SerializeField]
	private GameObject Joystick;

	[SerializeField]
	private GameObject Error;

	public MovementPointerStyle currentStyle;

	private ParticleSystem[] particleSystems;

	private Vector3 pointerCenter;

	private bool isPlaying;

	public static MovementPointer CreateNew()
	{
		GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/FXs/MovementPointer"));
		MovementPointer component = gameObject.GetComponent<MovementPointer>();
		component.Initialize();
		return component;
	}

	public void Initialize()
	{
		particleSystems = GetComponentsInChildren<ParticleSystem>();
		Pointer.SetActive(false);
		Joystick.SetActive(false);
		Error.SetActive(false);
	}

	private void SetStyle(MovementPointerStyle style)
	{
		currentStyle = style;
		Pointer.SetActive(currentStyle == MovementPointerStyle.POINTER);
		Joystick.SetActive(currentStyle == MovementPointerStyle.JOYSTICK);
		Error.SetActive(currentStyle == MovementPointerStyle.ERROR);
		if (style != MovementPointerStyle.ERROR)
		{
			return;
		}
		for (int i = 0; i < particleSystems.Length; i++)
		{
			ParticleSystem.Particle[] array = new ParticleSystem.Particle[particleSystems[i].maxParticles];
			particleSystems[i].GetParticles(array);
			for (int j = 0; j < array.Length; j++)
			{
				array[j].startColor = Color.red;
			}
			particleSystems[i].SetParticles(array, array.Length);
		}
	}

	public MovementPointerStyle GetStyle()
	{
		return currentStyle;
	}

	public bool BlinkInteractablesAt(LevelNode levelNode)
	{
		Debug.Log("BlinkInteractables");
		if (levelNode == null)
		{
			return false;
		}
		foreach (GameObject @object in levelNode.Objects)
		{
			if (@object == null)
			{
				continue;
			}
			InteractableObject[] componentsInChildren = @object.GetComponentsInChildren<InteractableObject>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i] != null)
				{
					componentsInChildren[i].DoActivateBlink(InteractableObject.ActivateBlink.ON_GOTO_NOTE);
					return true;
				}
			}
		}
		return false;
	}

	public void BlinkInteractable(InteractableObject interact)
	{
		if (GameController.Instance.currentState is GameControllerStatePlaying && (GameController.Instance.currentState as GameControllerStatePlaying).IsConfirmingQuit)
		{
			Debug.Log("__Bail__");
			return;
		}
		AudioPlayer.PlaySound("fx/move/pointer/prop");
		interact.DoActivateBlink(InteractableObject.ActivateBlink.ON_PICK);
	}

	public void BlinkCharacter(Character character)
	{
		if (!(GameController.Instance.currentState is GameControllerStatePlaying) || !(GameController.Instance.currentState as GameControllerStatePlaying).IsConfirmingQuit)
		{
			AudioPlayer.PlaySound("fx/move/pointer/sneak/attack");
			character.DoActivateBlink();
		}
	}

	public void PositionTarget(Vector3 pos, MovementPointerStyle style)
	{
		base.transform.position = pos;
		if (GameController.Instance.currentState is GameControllerStatePlaying && (GameController.Instance.currentState as GameControllerStatePlaying).IsConfirmingQuit)
		{
			SetStyle(MovementPointerStyle.NONE);
		}
		else if (style != currentStyle)
		{
			SetStyle(style);
			for (int i = 0; i < particleSystems.Length; i++)
			{
				particleSystems[i].Play();
			}
			if (currentStyle == MovementPointerStyle.POINTER)
			{
				AudioPlayer.PlaySound("fx/move/pointer/regular");
			}
			else if (currentStyle == MovementPointerStyle.ERROR)
			{
				AudioPlayer.PlaySound("fx/move/pointer/not/allowed");
				StartCoroutine(DelayedHide(0.3f));
			}
		}
	}

	private IEnumerator DelayedHide(float delay)
	{
		yield return new WaitForSeconds(delay);
		Hide(true);
	}

	public void Hide(bool force = false)
	{
		if (!force && currentStyle == MovementPointerStyle.NONE)
		{
			return;
		}
		currentStyle = MovementPointerStyle.NONE;
		for (int i = 0; i < particleSystems.Length; i++)
		{
			ParticleSystem.Particle[] array = new ParticleSystem.Particle[particleSystems[i].maxParticles];
			particleSystems[i].GetParticles(array);
			for (int j = 0; j < array.Length; j++)
			{
				array[j].remainingLifetime /= 2f;
			}
			particleSystems[i].SetParticles(array, array.Length);
			particleSystems[i].Stop();
		}
	}
}
