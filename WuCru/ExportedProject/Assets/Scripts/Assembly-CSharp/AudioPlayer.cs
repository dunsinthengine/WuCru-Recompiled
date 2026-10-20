using Fabric;
using UnityEngine;

public static class AudioPlayer
{
	public static void PlaySound(string fabricEventName, GameObject parentObject = null)
	{
		TriggerFabricEvent(fabricEventName, EventAction.PlaySound, null, parentObject);
	}

	public static void PlaySoundDebug(string fabricEventName, GameObject parentObject = null)
	{
		string text = string.Empty;
		if (parentObject != null)
		{
			text = " on GameObject: <b>" + parentObject.name + "</b>";
		}
		Debug.Log("Trigger event: <b>" + fabricEventName + "</b>" + text, parentObject);
		PlaySound(fabricEventName, parentObject);
	}

	public static void StopSound(string fabricEventName, GameObject parentObject = null)
	{
		TriggerFabricEvent(fabricEventName, EventAction.StopSound, false, parentObject);
	}

	public static void StopSoundDebug(string fabricEventName)
	{
		Debug.Log("Trigger <i>Stop Sound</i> on event: <b>" + fabricEventName + "</b>");
		TriggerFabricEvent(fabricEventName, EventAction.StopSound);
	}

	public static void SetVolume(string eventName, float volume, GameObject gameObject = null)
	{
		EventManager.Instance.PostEvent(eventName, EventAction.SetVolume, 0f, gameObject);
	}

	public static void SetSwitch(string eventName, string childComponentName, GameObject gameObject = null)
	{
		EventManager.Instance.PostEvent(eventName, EventAction.SetSwitch, childComponentName, gameObject);
	}

	public static bool IsPlaying(string componentName)
	{
		Fabric.Component componentByName = FabricManager.Instance.GetComponentByName(componentName);
		if (componentByName != null)
		{
			return componentByName.IsPlaying();
		}
		return false;
	}

	public static bool IsEventActive(string eventName, GameObject gameObject = null)
	{
		return EventManager.Instance.IsEventActive(eventName, gameObject);
	}

	public static void MoveFabricAudioListener(GameObject sourceGameObject, GameObject targetGameObject)
	{
		AudioListener componentInChildren = sourceGameObject.GetComponentInChildren<AudioListener>();
		if (componentInChildren != null)
		{
			componentInChildren.transform.parent = targetGameObject.transform;
			componentInChildren.transform.localPosition = Vector3.zero;
		}
		else
		{
			Debug.LogWarning("***** FABRIC AUDIO LISTENER WAS NULL! *****");
		}
	}

	public static FabricAudioListener GetFabricAudioListener(GameObject gameObject)
	{
		FabricAudioListener componentInChildren = gameObject.GetComponentInChildren<FabricAudioListener>();
		if (componentInChildren == null)
		{
			GameObject gameObject2 = new GameObject();
			gameObject2.name = "AudioListener";
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localPosition = Vector3.zero;
			return gameObject2.AddComponent<FabricAudioListener>();
		}
		return componentInChildren;
	}

	public static void MoveFabricAudioListener(FabricAudioListener sourceComponent, GameObject targetGameObject)
	{
		Object.Destroy(sourceComponent);
		targetGameObject.AddComponent<FabricAudioListener>();
	}

	private static void TriggerFabricEvent(string fabricEventName, EventAction fabricEventAction = EventAction.PlaySound, object parameter = null, GameObject parentGameObject = null)
	{
		if (fabricEventName != null)
		{
			if (parentGameObject != null)
			{
				EventManager.Instance.PostEvent(fabricEventName, fabricEventAction, parameter, parentGameObject);
			}
			else
			{
				EventManager.Instance.PostEvent(fabricEventName, fabricEventAction);
			}
		}
	}
}
