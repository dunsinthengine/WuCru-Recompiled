using UnityEngine;

public class AmuzoMonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T _instance;

	[SerializeField]
	private bool _dontDestroyOnLoad = true;

	[SerializeField]
	private bool _isAutoInitialise = true;

	[SerializeField]
	private bool _allowBeingDestroyed;

	private static bool _hasBeenInitialised;

	private static bool _hasBeenFinalDestroyed;

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
			if (_hasBeenFinalDestroyed)
			{
				return (T)null;
			}
			if (_instance == null)
			{
				_instance = (T)Object.FindObjectOfType(typeof(T));
				if (_instance == null)
				{
					GameObject gameObject = new GameObject(typeof(T).ToString());
					_instance = gameObject.AddComponent<T>();
				}
				AmuzoMonoSingleton<T> amuzoMonoSingleton = _instance as AmuzoMonoSingleton<T>;
				if (amuzoMonoSingleton != null)
				{
					if (amuzoMonoSingleton._isAutoInitialise)
					{
						amuzoMonoSingleton.Initialise();
					}
					else if (!_hasBeenInitialised)
					{
						Debug.LogError(typeof(T).ToString() + " was not initialised before it's instance was accessed!");
					}
				}
			}
			return _instance;
		}
	}

	protected void Initialise()
	{
		OnInitialise();
		_hasBeenInitialised = true;
	}

	protected virtual void OnInitialise()
	{
	}

	protected virtual bool OnResolveSingletonConflict(T newInstance)
	{
		return false;
	}

	protected virtual void Awake()
	{
		if (_dontDestroyOnLoad)
		{
			Object.DontDestroyOnLoad(base.gameObject);
		}
		if (_instance == null && !_hasBeenFinalDestroyed)
		{
			_instance = this as T;
			if (_isAutoInitialise && !_hasBeenInitialised)
			{
				Initialise();
			}
		}
		else if (_instance != this && !(_instance as AmuzoMonoSingleton<T>).OnResolveSingletonConflict(this as T))
		{
			Object.Destroy(base.gameObject);
		}
	}

	protected virtual void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = (T)null;
			_hasBeenFinalDestroyed = !_allowBeingDestroyed;
		}
	}
}
