using System;
using UnityEngine;

public class GUIMonitor
{
	private static GUIMonitor _instance;

	private GUITypeFlag _registryFlags;

	private UnityEngine.Object[] _objectReferences;

	public static GUIMonitor Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new GUIMonitor();
			}
			return _instance;
		}
	}

	private GUIMonitor()
	{
		_registryFlags = GUITypeFlag.None;
		int upperBound = Enum.GetValues(typeof(GUITypeFlag)).GetUpperBound(0);
		_objectReferences = new GameObject[upperBound];
	}

	public void Register(GUITypeFlag flag, UnityEngine.Object reference)
	{
		_registryFlags |= flag;
		_objectReferences[(int)(flag - 1)] = reference;
	}

	public void Deregister(GUITypeFlag flag)
	{
		_registryFlags &= ~flag;
		_objectReferences[(int)(flag - 1)] = null;
	}

	public bool HasType(GUITypeFlag flag)
	{
		return (_registryFlags & flag) != 0;
	}

	public UnityEngine.Object GetReference(GUITypeFlag flag)
	{
		return _objectReferences[(int)(flag - 1)];
	}

	public GUITypeFlag TypeToFlag(Type type)
	{
		switch (type.ToString())
		{
		case "SettingsGUI":
			return GUITypeFlag.Settings;
		case "LoadScreenGUI":
			return GUITypeFlag.LoadScreen;
		default:
			Debug.LogErrorFormat("Could not map type ${0} to flag", type);
			return GUITypeFlag.None;
		}
	}
}
