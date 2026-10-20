using UnityEngine;

public class FullScreenCollider : MonoBehaviour
{
	private const string LOG_TAG = "[FullScreenCollider] ";

	private bool _isInitialized;

	private Collider _collider;

	private UIWidget _widget;

	public bool _pIsActive
	{
		get
		{
			return _isInitialized && _collider != null && _collider.enabled && _collider.gameObject.activeInHierarchy;
		}
	}

	public int _pDepth
	{
		get
		{
			return (!_isInitialized || !(_widget != null)) ? int.MinValue : _widget.depth;
		}
	}

	private void Awake()
	{
		Initialize();
	}

	public void Initialize()
	{
		if (!_isInitialized)
		{
			_collider = base.gameObject.GetComponent<Collider>();
			_widget = base.gameObject.GetComponent<UIWidget>();
			_isInitialized = true;
		}
	}
}
