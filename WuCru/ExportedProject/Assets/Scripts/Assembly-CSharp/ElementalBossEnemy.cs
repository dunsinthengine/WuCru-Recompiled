using UnityEngine;

public class ElementalBossEnemy : AIAgent
{
	public GameObject _earthFX;

	public GameObject _electricityFX;

	public GameObject _energyFX;

	public GameObject _fireFX;

	public GameObject _iceFX;

	public GameObject _waterFX;

	public GameObject _earthDeathFX;

	public GameObject _electricityDeathFX;

	public GameObject _energyDeathFX;

	public GameObject _fireDeathFX;

	public GameObject _iceDeathFX;

	public GameObject _waterDeathFX;

	private GameObject _earthDeathFXInst;

	private GameObject _electricityDeathFXInst;

	private GameObject _energyDeathFXInst;

	private GameObject _fireDeathFXInst;

	private GameObject _iceDeathFXInst;

	private GameObject _waterDeathFXInst;

	public float _earthDeathSelfDestroyDelay = 1f;

	public float _electricityDeathSelfDestroyDelay = 1f;

	public float _energyDeathSelfDestroyDelay = 1f;

	public float _fireDeathSelfDestroyDelay = 1f;

	public float _iceDeathSelfDestroyDelay = 1f;

	public float _waterDeathSelfDestroyDelay = 1f;

	public GameObject _cardFXPrefab;

	private bool _showingIcon;

	private GameObject _iconObject;

	protected override void Awake()
	{
		base.Awake();
		_earthFX.transform.parent = controller.Character.transform;
		_earthFX.transform.localPosition = Vector3.zero;
		_electricityFX.transform.parent = controller.Character.transform;
		_electricityFX.transform.localPosition = Vector3.zero;
		_energyFX.transform.parent = controller.Character.transform;
		_energyFX.transform.localPosition = Vector3.zero;
		_fireFX.transform.parent = controller.Character.transform;
		_fireFX.transform.localPosition = Vector3.zero;
		_iceFX.transform.parent = controller.Character.transform;
		_iceFX.transform.localPosition = Vector3.zero;
		_waterFX.transform.parent = controller.Character.transform;
		_waterFX.transform.localPosition = Vector3.zero;
		if (_earthDeathFX != null)
		{
			_earthDeathFXInst = Object.Instantiate(_earthDeathFX);
			_earthDeathFXInst.transform.parent = controller.Character.transform;
			_earthDeathFXInst.transform.localPosition = Vector3.zero;
			_earthDeathFXInst.gameObject.SetActive(false);
		}
		if (_electricityDeathFX != null)
		{
			_electricityDeathFXInst = Object.Instantiate(_electricityDeathFX);
			_electricityDeathFXInst.transform.parent = controller.Character.transform;
			_electricityDeathFXInst.transform.localPosition = Vector3.zero;
			_electricityDeathFXInst.gameObject.SetActive(false);
		}
		if (_energyDeathFX != null)
		{
			_energyDeathFXInst = Object.Instantiate(_energyDeathFX);
			_energyDeathFXInst.transform.parent = controller.Character.transform;
			_energyDeathFXInst.transform.localPosition = Vector3.zero;
			_energyDeathFXInst.gameObject.SetActive(false);
		}
		if ((bool)_fireDeathFX)
		{
			_fireDeathFXInst = Object.Instantiate(_fireDeathFX);
			_fireDeathFXInst.transform.parent = controller.Character.transform;
			_fireDeathFXInst.transform.localPosition = Vector3.zero;
			_fireDeathFXInst.gameObject.SetActive(false);
		}
		if (_iceDeathFX != null)
		{
			_iceDeathFXInst = Object.Instantiate(_iceDeathFX);
			_iceDeathFXInst.transform.parent = controller.Character.transform;
			_iceDeathFXInst.transform.localPosition = Vector3.zero;
			_iceDeathFXInst.gameObject.SetActive(false);
		}
		if (_waterDeathFX != null)
		{
			_waterDeathFXInst = Object.Instantiate(_waterDeathFX);
			_waterDeathFXInst.transform.parent = controller.Character.transform;
			_waterDeathFXInst.transform.localPosition = Vector3.zero;
			_waterDeathFXInst.gameObject.SetActive(false);
		}
		_iconObject = new GameObject();
		_iconObject.transform.parent = controller.Character.transform;
		_iconObject.transform.localPosition = Vector3.up * 0.9f;
		controller.Character.transform.localScale = Vector3.one * 1.5f;
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		float sqrMagnitude = (base.Character.transform.position - base.Player.Character.transform.position).sqrMagnitude;
		if (sqrMagnitude < 15f)
		{
			if (!_showingIcon && base.Character.elementalPower != base.Player.Character.elementalPower)
			{
				_showingIcon = true;
				ScreenHud._pInstance._hudElementDisplay.DisplayIcon(base.Character.elementalPower, _iconObject.transform);
			}
			else if (_showingIcon && base.Character.elementalPower == base.Player.Character.elementalPower)
			{
				_showingIcon = false;
				ScreenHud._pInstance._hudElementDisplay.DismissIcon(_iconObject.transform);
			}
		}
		else if (sqrMagnitude > 25f && _showingIcon)
		{
			_showingIcon = false;
			ScreenHud._pInstance._hudElementDisplay.DismissIcon(_iconObject.transform);
		}
	}

	public void OnChosenElement()
	{
		_earthFX.gameObject.SetActive(controller.Character.elementalPower == ElementalPower.Earth);
		_electricityFX.gameObject.SetActive(controller.Character.elementalPower == ElementalPower.Electricity);
		_energyFX.gameObject.SetActive(controller.Character.elementalPower == ElementalPower.Energy);
		_fireFX.gameObject.SetActive(controller.Character.elementalPower == ElementalPower.Fire);
		_iceFX.gameObject.SetActive(controller.Character.elementalPower == ElementalPower.Ice);
		_waterFX.gameObject.SetActive(controller.Character.elementalPower == ElementalPower.Water);
	}

	public override void OnStateChange()
	{
		switch (base.State)
		{
		case AIAgentState.Patrolling:
			controller.SetBehaviour(new FollowPath());
			break;
		case AIAgentState.SpottingPlayer:
			controller.SetBehaviour(new SoftSpottingPlayer());
			break;
		case AIAgentState.Suspicious:
			controller.SetBehaviour(new SearchForPlayer());
			break;
		case AIAgentState.Alert:
			controller.SetBehaviour(new HuntPlayer());
			break;
		case AIAgentState.Combat:
			controller.SetBehaviour(new CombatPlayerElementalBoss());
			break;
		case AIAgentState.BeaconCall:
			controller.SetBehaviour(new BeaconCall());
			break;
		case AIAgentState.Scared:
			controller.SetBehaviour(new FleePlayer());
			break;
		case AIAgentState.Dead:
			if (_cardFXPrefab != null)
			{
				GameObject gameObject = Object.Instantiate(_cardFXPrefab);
				gameObject.transform.position = base.Character.transform.position;
				InGameCardFX component = gameObject.GetComponent<InGameCardFX>();
				component.Begin();
			}
			if (_earthDeathFXInst != null && _electricityDeathFXInst != null && _energyDeathFXInst != null && _fireDeathFXInst != null && _iceDeathFXInst != null && _waterDeathFXInst != null)
			{
				_earthDeathFXInst.gameObject.SetActive(controller.Character.elementalPower == ElementalPower.Earth);
				_electricityDeathFXInst.gameObject.SetActive(controller.Character.elementalPower == ElementalPower.Electricity);
				_energyDeathFXInst.gameObject.SetActive(controller.Character.elementalPower == ElementalPower.Energy);
				_fireDeathFXInst.gameObject.SetActive(controller.Character.elementalPower == ElementalPower.Fire);
				_iceDeathFXInst.gameObject.SetActive(controller.Character.elementalPower == ElementalPower.Ice);
				_waterDeathFXInst.gameObject.SetActive(controller.Character.elementalPower == ElementalPower.Water);
				_earthDeathFXInst.transform.parent = null;
				_electricityDeathFXInst.transform.parent = null;
				_energyDeathFXInst.transform.parent = null;
				_fireDeathFXInst.transform.parent = null;
				_iceDeathFXInst.transform.parent = null;
				_waterDeathFXInst.transform.parent = null;
				DelayedSelfDestroy delayedSelfDestroy = null;
				delayedSelfDestroy = _earthDeathFXInst.AddComponent<DelayedSelfDestroy>();
				delayedSelfDestroy.delay = _earthDeathSelfDestroyDelay;
				delayedSelfDestroy.RefreshTimer();
				delayedSelfDestroy = _electricityDeathFXInst.AddComponent<DelayedSelfDestroy>();
				delayedSelfDestroy.delay = _electricityDeathSelfDestroyDelay;
				delayedSelfDestroy.RefreshTimer();
				delayedSelfDestroy = _energyDeathFXInst.AddComponent<DelayedSelfDestroy>();
				delayedSelfDestroy.delay = _energyDeathSelfDestroyDelay;
				delayedSelfDestroy.RefreshTimer();
				delayedSelfDestroy = _fireDeathFXInst.AddComponent<DelayedSelfDestroy>();
				delayedSelfDestroy.delay = _fireDeathSelfDestroyDelay;
				delayedSelfDestroy.RefreshTimer();
				delayedSelfDestroy = _iceDeathFXInst.AddComponent<DelayedSelfDestroy>();
				delayedSelfDestroy.delay = _iceDeathSelfDestroyDelay;
				delayedSelfDestroy.RefreshTimer();
				delayedSelfDestroy = _waterDeathFXInst.AddComponent<DelayedSelfDestroy>();
				delayedSelfDestroy.delay = _waterDeathSelfDestroyDelay;
				delayedSelfDestroy.RefreshTimer();
			}
			controller.SetBehaviour(new DeadStateElementalBoss());
			break;
		case AIAgentState.IdleAfterKnockdown:
			controller.SetBehaviour(new IdleAfterKnockdown());
			break;
		}
	}

	public override AIAgentType GetAgentType()
	{
		return AIAgentType.ElementalBoss;
	}
}
