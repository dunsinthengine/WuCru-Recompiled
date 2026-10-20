using System;
using Fabric;
using UnityEngine;

public class GameAudioManager : MonoBehaviour
{
	private static GameAudioManager _instance;

	[SerializeField]
	private GroupComponent[] FxGroups;

	[SerializeField]
	private GroupComponent[] MusicGroups;

	[SerializeField]
	private GroupComponent[] AmbienceGroups;

	private bool _musicMuted;

	private bool _fxMuted;

	public static GameAudioManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindObjectOfType<GameAudioManager>();
				if (_instance == null)
				{
					throw new Exception("There is no GameAudioManager in the scene.");
				}
			}
			return _instance;
		}
	}

	public bool MusicMuted
	{
		get
		{
			return _musicMuted;
		}
		set
		{
			_musicMuted = value;
			GroupComponent[] musicGroups = MusicGroups;
			foreach (GroupComponent groupComponent in musicGroups)
			{
				groupComponent.Mute = _musicMuted;
			}
		}
	}

	public bool FxMuted
	{
		get
		{
			return _fxMuted;
		}
		set
		{
			_fxMuted = value;
			GroupComponent[] fxGroups = FxGroups;
			foreach (GroupComponent groupComponent in fxGroups)
			{
				groupComponent.Mute = _fxMuted;
			}
		}
	}

	public void Awake()
	{
	}

	public void StopAllMusic(bool ignoreFade)
	{
		GroupComponent[] musicGroups = MusicGroups;
		foreach (GroupComponent groupComponent in musicGroups)
		{
			groupComponent.Stop(true, false, ignoreFade);
		}
	}

	public void StopAllMusicExcept(string componentName, bool ignoreFade, float fadeTime = 0f)
	{
		GroupComponent[] musicGroups = MusicGroups;
		foreach (GroupComponent groupComponent in musicGroups)
		{
			Fabric.Component[] childComponents = groupComponent.GetChildComponents();
			foreach (Fabric.Component component in childComponents)
			{
				if (component.name != componentName)
				{
					component.Stop(true, false, ignoreFade, fadeTime);
				}
			}
		}
	}

	public void StopAmbience(bool ignoreFade, float newFadeTime = 0f)
	{
		GroupComponent[] ambienceGroups = AmbienceGroups;
		foreach (GroupComponent component in ambienceGroups)
		{
			StopChildrenInGroup(component, ignoreFade, newFadeTime);
		}
	}

	public void StopChildrenInGroup(GroupComponent component, bool ignoreFade, float newFadeTime = 0f)
	{
		Fabric.Component[] childComponents = component.GetChildComponents();
		foreach (Fabric.Component component2 in childComponents)
		{
			component2.Stop(true, false, ignoreFade, newFadeTime);
		}
	}
}
