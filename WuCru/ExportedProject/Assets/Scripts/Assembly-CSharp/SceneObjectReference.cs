using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class SceneObjectReference : MonoBehaviour
{
	private const string LOG_TAG = "[SceneObjectReference] ";

	private static Dictionary<string, SceneObjectReference> _sReferencesDict = new Dictionary<string, SceneObjectReference>();

	[SerializeField]
	private string _nameOverride;

	[SerializeField]
	private Component _component;

	private Type _type;

	private string _pName
	{
		get
		{
			return (!string.IsNullOrEmpty(_nameOverride)) ? _nameOverride : base.gameObject.name;
		}
	}

	public static GameObject FindGameObject(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}
		if (!_sReferencesDict.ContainsKey(name))
		{
			return null;
		}
		SceneObjectReference sceneObjectReference = _sReferencesDict[name];
		return (!(sceneObjectReference != null)) ? null : sceneObjectReference.gameObject;
	}

	public static T FindComponent<T>(string name) where T : Component
	{
		if (string.IsNullOrEmpty(name))
		{
			return (T)null;
		}
		if (!_sReferencesDict.ContainsKey(name))
		{
			return (T)null;
		}
		SceneObjectReference sceneObjectReference = _sReferencesDict[name];
		if (sceneObjectReference == null)
		{
			return (T)null;
		}
		if (sceneObjectReference._component == null)
		{
			return (T)null;
		}
		Type typeFromHandle = typeof(T);
		if (!DoesReferenceMatchType(sceneObjectReference, typeFromHandle))
		{
			return (T)null;
		}
		return sceneObjectReference._component as T;
	}

	private static void RegisterReference(SceneObjectReference @ref)
	{
		string pName = @ref._pName;
		if (_sReferencesDict.ContainsKey(pName))
		{
			Debug.LogError("[SceneObjectReference] " + string.Format("Already registered reference named '{0}'", pName));
		}
		else
		{
			_sReferencesDict.Add(pName, @ref);
		}
	}

	private static void UnregisterReference(SceneObjectReference @ref)
	{
		string pName = @ref._pName;
		if (!_sReferencesDict.ContainsKey(pName))
		{
			Debug.LogWarning("[SceneObjectReference] " + string.Format("Reference named '{0}' not registered", pName));
		}
		else
		{
			_sReferencesDict.Remove(pName);
		}
	}

	private static bool DoesReferenceMatchType(SceneObjectReference @ref, Type type)
	{
		return type.IsAssignableFrom(@ref._type);
	}

	private static bool DoesReferenceExactlyMatchType(SceneObjectReference @ref, Type type)
	{
		return @ref._type == type;
	}

	private void Awake()
	{
		Initialize();
		RegisterReference(this);
	}

	private void OnDestroy()
	{
		UnregisterReference(this);
	}

	private void Initialize()
	{
		SetType();
	}

	private void SetType()
	{
		if (_component != null)
		{
			_type = _component.GetType();
		}
		else
		{
			_type = typeof(GameObject);
		}
	}
}
