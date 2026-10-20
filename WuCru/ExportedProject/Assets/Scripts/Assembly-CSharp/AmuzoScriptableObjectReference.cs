using UnityEngine;

public class AmuzoScriptableObjectReference : AmuzoMonoSingleton<AmuzoScriptableObjectReference>
{
	private static ScriptableObject[] _scriptableObjectsArray;

	public ScriptableObject[] _scriptableObjects;

	public static T GetScriptableObject<T>() where T : ScriptableObject
	{
		if (_scriptableObjectsArray == null)
		{
			_scriptableObjectsArray = GetAllScriptableObjects();
		}
		if (_scriptableObjectsArray == null)
		{
			return (T)null;
		}
		int i = 0;
		for (int num = _scriptableObjectsArray.Length; i < num; i++)
		{
			if (_scriptableObjectsArray[i] is T)
			{
				return _scriptableObjectsArray[i] as T;
			}
		}
		return (T)null;
	}

	public static ScriptableObject[] GetAllScriptableObjects()
	{
		ScriptableObject[] array = new ScriptableObject[0];
		if (array.Length == 0 && AmuzoMonoSingleton<AmuzoScriptableObjectReference>._pInstance != null && AmuzoMonoSingleton<AmuzoScriptableObjectReference>._pInstance._scriptableObjects != null)
		{
			array = AmuzoMonoSingleton<AmuzoScriptableObjectReference>._pInstance._scriptableObjects;
			AmuzoMonoSingleton<AmuzoScriptableObjectReference>._pInstance._scriptableObjects = null;
		}
		return array;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		_scriptableObjectsArray = null;
	}
}
