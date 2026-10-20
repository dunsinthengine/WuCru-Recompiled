using UnityEngine;

public class Facades<T> where T : class
{
	private static T _instance;

	public static T Instance
	{
		get
		{
			return _instance;
		}
	}

	public static void Register(T instance)
	{
		if (instance != null && _instance != null)
		{
			Debug.LogWarning("Overwritten pre-existing singleton reference for " + typeof(T).FullName);
		}
		_instance = instance;
	}
}
