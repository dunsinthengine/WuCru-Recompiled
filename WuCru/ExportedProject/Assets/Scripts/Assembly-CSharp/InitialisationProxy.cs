using UnityEngine;

public class InitialisationProxy : InitialisationObject
{
	public interface ITarget
	{
		InitialisationState _pInitialisationState { get; set; }

		void OnStartInitialising();

		InitialisationState OnUpdateInitialising();
	}

	private const string LOG_TAG = "[InitialisationProxy] ";

	[SerializeField]
	private MonoBehaviour _targetScript;

	private ITarget _target;

	protected override void Awake()
	{
		if (_targetScript != null)
		{
			_target = _targetScript as ITarget;
			if (_target == null)
			{
				Debug.LogError("[InitialisationProxy] Target script does not implement ITarget interface");
				return;
			}
		}
		if (_target != null)
		{
			_target._pInitialisationState = InitialisationState.WAITING_TO_START;
		}
		base.Awake();
	}

	protected override void OnDestroy()
	{
		if (_target != null)
		{
			_target._pInitialisationState = InitialisationState.FINISHED;
		}
		base.OnDestroy();
	}

	public override void startInitialising()
	{
		if (_target != null)
		{
			_target.OnStartInitialising();
			_currentState = _target._pInitialisationState;
		}
	}

	public override InitialisationState updateInitialisation()
	{
		if (_target != null)
		{
			_currentState = _target.OnUpdateInitialising();
		}
		return _currentState;
	}
}
