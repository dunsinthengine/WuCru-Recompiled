using UnityEngine;

public class DojoCharacterBehaviour : MonoBehaviour
{
	public enum STATE
	{
		SITTING = 0,
		EXITING_SIT = 1,
		WALKING = 2,
		ENTER_SIT = 3,
		SPOT_EFFECT = 4,
		WAITING = 5
	}

	private float _stateStartTime;

	private STATE _currentState;

	private Animator _animRef;

	private float _sitDuration;

	private float _initialWalkDelay;

	private int _currentPathPos;

	private bool _reversePathDir;

	private string _startState;

	private bool _exitSofaFromLeft;

	private int _numOnSofa;

	private DojoPath _path;

	public DojoPath _pPath
	{
		get
		{
			return _path;
		}
		set
		{
			_path = value;
		}
	}

	public string _pStartState
	{
		get
		{
			return _startState;
		}
		set
		{
			_startState = value;
		}
	}

	public float _pInitialWalkDelay
	{
		get
		{
			return _initialWalkDelay;
		}
		set
		{
			_initialWalkDelay = value;
		}
	}

	public bool _pExitSofaFromLeft
	{
		get
		{
			return _exitSofaFromLeft;
		}
		set
		{
			_exitSofaFromLeft = value;
		}
	}

	public int _pNumOnSofa
	{
		get
		{
			return _numOnSofa;
		}
		set
		{
			_numOnSofa = value;
		}
	}

	public STATE _pState
	{
		get
		{
			return _currentState;
		}
	}

	private void Awake()
	{
		_animRef = GetComponent<Animator>();
		_stateStartTime = Time.time;
	}

	private void Start()
	{
		_animRef.Play(_pStartState, 0, 0f);
		if (_currentState != STATE.WAITING)
		{
			ChangeState(STATE.SITTING);
		}
		_sitDuration += _initialWalkDelay;
	}

	public void SwapWith(DojoCharacterBehaviour other)
	{
		_pExitSofaFromLeft = other._pExitSofaFromLeft;
		_pPath = other._pPath;
		base.transform.position = other._pPath._offScreenPos.position;
		base.transform.rotation = other._pPath._offScreenPos.rotation;
		_currentPathPos = _pPath._path.Length - 1;
		_reversePathDir = true;
		Vector3 position = _pPath._path[_currentPathPos - 1].position;
		base.transform.forward = (position - base.transform.position).normalized;
		ChangeState(STATE.WALKING);
	}

	public void SetWaiting()
	{
		ChangeState(STATE.WAITING);
	}

	public void RequestSpotAnim()
	{
		if (_currentState == STATE.SITTING)
		{
			ChangeState(STATE.SPOT_EFFECT);
		}
	}

	private void ChangeState(STATE newState)
	{
		_currentState = newState;
		_stateStartTime = Time.time;
		switch (_currentState)
		{
		case STATE.SITTING:
			StartSitting();
			break;
		case STATE.EXITING_SIT:
			StartExitingSit();
			break;
		case STATE.WALKING:
			StartWalking();
			break;
		case STATE.ENTER_SIT:
			StartEnterSit();
			break;
		case STATE.SPOT_EFFECT:
			StartSpotEffect();
			break;
		case STATE.WAITING:
			StartWaiting();
			break;
		}
	}

	private void Update()
	{
		switch (_currentState)
		{
		case STATE.SITTING:
			UpdateSitting();
			break;
		case STATE.EXITING_SIT:
			UpdateExitingSit();
			break;
		case STATE.WALKING:
			UpdateWalking();
			break;
		case STATE.ENTER_SIT:
			UpdateEnterSit();
			break;
		case STATE.SPOT_EFFECT:
			UpdateSpotEffect();
			break;
		case STATE.WAITING:
			UpdateWaiting();
			break;
		}
	}

	private void StartSitting()
	{
		_reversePathDir = false;
		_sitDuration = Random.Range(10f, 15f);
	}

	private void UpdateSitting()
	{
		float num = Time.time - _stateStartTime;
		if (num > _sitDuration)
		{
			if (_pNumOnSofa <= 1)
			{
				_sitDuration += Random.Range(1f, 3f);
			}
			else
			{
				ChangeState(STATE.EXITING_SIT);
			}
		}
	}

	private void StartExitingSit()
	{
		_animRef.SetTrigger("ExitSit");
		_animRef.SetInteger("MountID", 0);
		_animRef.SetBool("SitDir", !_pExitSofaFromLeft && Random.Range(0, 100) < 50);
	}

	private void UpdateExitingSit()
	{
		AnimatorStateInfo currentAnimatorStateInfo = _animRef.GetCurrentAnimatorStateInfo(0);
		if (currentAnimatorStateInfo.normalizedTime > 0.25f && _pExitSofaFromLeft)
		{
			Vector3 normalized = (_pPath._path[1].position - base.transform.position).normalized;
			base.transform.forward = Vector3.MoveTowards(base.transform.forward, normalized, Time.deltaTime * 4f);
		}
		if (currentAnimatorStateInfo.IsName("walk"))
		{
			ChangeState(STATE.WALKING);
		}
	}

	private void StartWalking()
	{
	}

	private void UpdateWalking()
	{
		Vector3 position = _pPath._path[_currentPathPos].position;
		float sqrMagnitude = (base.transform.position - position).sqrMagnitude;
		if (sqrMagnitude < 0.5f)
		{
			if (_reversePathDir)
			{
				_currentPathPos--;
				if (_currentPathPos < 0)
				{
					_currentPathPos = 0;
					int num = Random.Range(0, 4);
					if (num > 3)
					{
						num = 3;
					}
					_animRef.SetInteger("SitID", num);
					ChangeState(STATE.ENTER_SIT);
				}
			}
			else
			{
				_currentPathPos++;
				if (_currentPathPos >= _pPath._path.Length)
				{
					_currentPathPos = _pPath._path.Length - 1;
					_reversePathDir = true;
					position = _pPath._path[_currentPathPos - 1].position;
					base.transform.forward = (position - base.transform.position).normalized;
					ChangeState(STATE.WAITING);
					DojoController._pInstance.RequestCharacterSwap(this);
				}
			}
		}
		Vector3 normalized = (position - base.transform.position).normalized;
		normalized.y = 0f;
		base.transform.position = Vector3.MoveTowards(base.transform.position, position, Time.deltaTime * 1f);
		base.transform.forward = Vector3.MoveTowards(base.transform.forward, normalized, Time.deltaTime * 4f);
	}

	private void StartEnterSit()
	{
		_animRef.SetTrigger("BeginSit");
		_animRef.SetBool("SitDir", true);
	}

	private void UpdateEnterSit()
	{
		base.transform.forward = Vector3.MoveTowards(base.transform.forward, _pPath._path[0].forward, Time.deltaTime * 4f);
		base.transform.position = Vector3.MoveTowards(base.transform.position, _pPath._path[0].position, Time.deltaTime * 1f);
		if (_animRef.GetCurrentAnimatorStateInfo(0).IsTag("Sit"))
		{
			ChangeState(STATE.SITTING);
		}
	}

	private void StartSpotEffect()
	{
		int num = Random.Range(1, 5);
		if (num > 4)
		{
			num = 4;
		}
		_animRef.SetTrigger("ExitSit");
		_animRef.SetInteger("MountID", num);
		num = Random.Range(0, 4);
		if (num > 3)
		{
			num = 3;
		}
		_animRef.SetInteger("SitID", num);
	}

	private void UpdateSpotEffect()
	{
		if (_animRef.GetCurrentAnimatorStateInfo(0).IsTag("Sit") && Time.time - _stateStartTime > 0.5f)
		{
			ChangeState(STATE.SITTING);
		}
	}

	private void StartWaiting()
	{
	}

	private void UpdateWaiting()
	{
	}
}
