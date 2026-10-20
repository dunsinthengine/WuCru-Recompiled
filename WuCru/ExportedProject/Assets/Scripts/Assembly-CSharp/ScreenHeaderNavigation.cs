using System;
using UnityEngine;

public class ScreenHeaderNavigation : ScreenBase
{
	private static ScreenHeaderNavigation _instance;

	public UIButton _skills;

	public UIButton _settings;

	public UIButton _garage;

	public UIButton _map;

	public UIButton _dojo;

	public ScreenObjectTransitionWidget _manualTween;

	public ParticleSystem[] _shinyGarage;

	public ParticleSystem[] _shinyWardrobe;

	public GoldenTrailFX _minifigPartGoldenTrail;

	public GoldenTrailFX _garageGoldenTrail;

	public GoldenTrailFX _dojoGoldenTrail;

	public UITexture _ftueMapArrow;

	private Action _skillsAction;

	private Action _settingsAction;

	private Action _garageAction;

	private Action _mapAction;

	private Action _dojoAction;

	public static ScreenHeaderNavigation _pInstance
	{
		get
		{
			return _instance;
		}
	}

	protected override void Awake()
	{
		_instance = this;
		base.Awake();
	}

	protected override void OnShowScreen()
	{
		_ftueMapArrow.enabled = false;
		base.OnShowScreen();
	}

	protected override void OnScreenExitComplete()
	{
		base.OnScreenExitComplete();
	}

	protected override void Update()
	{
		base.Update();
		if (_ftueMapArrow.enabled)
		{
			float num = Mathf.Sin(Time.time * 6f);
			_ftueMapArrow.transform.localPosition = Vector3.up * (1f - num) * 10f;
		}
	}

	private void initFX()
	{
	}

	public void playGarageFX()
	{
		_shinyGarage[0].Play();
		_shinyGarage[1].Play();
		AudioPlayer.PlaySound("ui/panel/impact");
	}

	public void playWardrobeFX()
	{
		_shinyWardrobe[0].Play();
		_shinyWardrobe[1].Play();
		AudioPlayer.PlaySound("ui/panel/impact");
	}

	public void ManualTweenBar(bool on)
	{
		if (on)
		{
			if (!_manualTween._pConsideredTweenedIn)
			{
				_manualTween.TweenIn();
			}
		}
		else if (_manualTween._pConsideredTweenedIn)
		{
			_manualTween.TweenOut();
		}
	}

	public void OnDojo()
	{
		if (_dojoAction != null)
		{
			AudioPlayer.PlaySound("ui/button/sword/shing");
			_dojoAction();
		}
	}

	public void OnSkills()
	{
		if (_skillsAction != null)
		{
			AudioPlayer.PlaySound("ui/button/sword/shing");
			_skillsAction();
		}
	}

	public void OnMap()
	{
		if (_mapAction != null)
		{
			_ftueMapArrow.enabled = false;
			AudioPlayer.PlaySound("ui/button/sword/shing");
			_mapAction();
		}
	}

	public void OnGarage()
	{
		if (_garageAction != null)
		{
			AudioPlayer.PlaySound("ui/button/sword/shing");
			_garageAction();
		}
	}

	public void OnSettings()
	{
		if (_settingsAction != null)
		{
			AudioPlayer.PlaySound("ui/button/sword/shing");
			_settingsAction();
		}
	}

	public void CloseHeader()
	{
		if (base.gameObject.activeInHierarchy)
		{
			EndFlow();
		}
	}

	public void OpenHeader()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			ScreenFacade instance = Facades<ScreenFacade>.Instance;
			instance.ShowPopupOverlay(this);
			_manualTween.SetStateConsideredTweenedIn(true);
		}
	}

	public void DisableButtons()
	{
		_skills.isEnabled = false;
		_map.isEnabled = false;
		_garage.isEnabled = false;
		_settings.isEnabled = false;
		_dojo.isEnabled = false;
	}

	public void EnableButtons()
	{
		_skills.isEnabled = _skillsAction != null;
		_map.isEnabled = _mapAction != null;
		_garage.isEnabled = _garageAction != null;
		_settings.isEnabled = _settingsAction != null;
		_dojo.isEnabled = _dojoAction != null;
	}

	public void DisableButtonsSettings()
	{
		_skills.isEnabled = false;
		_map.isEnabled = false;
		_garage.isEnabled = false;
		_dojo.isEnabled = false;
	}

	public void EnableButtonsSettings()
	{
		_skills.isEnabled = _skillsAction != null;
		_map.isEnabled = _mapAction != null;
		_garage.isEnabled = _garageAction != null;
		_dojo.isEnabled = _dojoAction != null;
	}

	public void ConfigureHeader(Action skillsAction, Action mapAction, Action garageAction, Action dojoAction, Action settingsAction, bool showMinifigGoldenTrail, bool showGarageGoldenTrail, bool showDojoGoldenTrail)
	{
		if (GameController.Instance.PlayerProfile.GetUsedVehicleParts().Count == 0)
		{
			garageAction = null;
			_garage.gameObject.SetActive(false);
		}
		else
		{
			_garage.gameObject.SetActive(true);
		}
		_skills.isEnabled = skillsAction != null;
		_map.isEnabled = mapAction != null;
		_garage.isEnabled = garageAction != null;
		_settings.isEnabled = settingsAction != null;
		_dojo.isEnabled = dojoAction != null;
		_skillsAction = skillsAction;
		_mapAction = mapAction;
		_garageAction = garageAction;
		_settingsAction = settingsAction;
		_dojoAction = dojoAction;
		_minifigPartGoldenTrail.SetVisible(showMinifigGoldenTrail && _skills.isEnabled);
		_garageGoldenTrail.SetVisible(showGarageGoldenTrail && _garage.isEnabled);
		_dojoGoldenTrail.SetVisible(showDojoGoldenTrail && _dojo.isEnabled);
	}
}
