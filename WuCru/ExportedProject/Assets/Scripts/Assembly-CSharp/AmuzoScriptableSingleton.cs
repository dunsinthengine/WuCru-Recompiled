using UnityEngine;

public class AmuzoScriptableSingleton<T> : ScriptableObject where T : ScriptableObject
{
	private static T _instance;

	public static bool _pExists
	{
		get
		{
			return _instance != null;
		}
	}

	public static T _pInstance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Resources.Load<T>(typeof(T).Name);
				if (_instance == null)
				{
					_instance = AmuzoScriptableObjectReference.GetScriptableObject<T>();
					if (_instance == null)
					{
						Debug.LogWarning(string.Concat("AmuzoScriptableSingleton instance of type [", typeof(T), "] couldn't be found. Does it exist? Is it in a Resources folder, or has it been added to the AmuzoScriptableObjectReference?"));
						_instance = ScriptableObject.CreateInstance<T>();
					}
				}
				if (_instance is AmuzoScriptableSingleton<T>)
				{
					(_instance as AmuzoScriptableSingleton<T>).Initialise();
				}
			}
			return _instance;
		}
	}

	protected virtual void Initialise()
	{
	}
}
