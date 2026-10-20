using System;
using System.Collections.Generic;
using AmuzoEngine;
using UnityEngine;

public class ScreenBase : MonoBehaviour, ScreenInputElements.IScreen
{
	public enum ScreenTweenType
	{
		Idle = 0,
		TweenIn = 1,
		TweenOut = 2
	}

	private const float UNFOCUSED_PANEL_ALPHA = 0.5f;

	public bool isOverlay;

	public bool _isFocusFade;

	public bool _isDisableInputWhileExiting;

	public bool _isDisableInputWhileUnfocused;

	private bool _isManualShow;

	private Action _onNextShowComplete;

	private Action _onNextExitComplete;

	private ScreenObjectTransitionWidget[] _transitionWidgets;

	private int _numTransitionWidgets;

	private UITextureResourceLoader[] _textureResourceLoaders;

	protected bool _isVisible;

	private bool _initialised;

	private float _screenExitTime;

	private float _screenStartTime;

	private Dictionary<GameObject, ScreenObjectTransitionWidget[]> _objectWidgetLookup = new Dictionary<GameObject, ScreenObjectTransitionWidget[]>();

	private ScreenInputElements _inputElements;

	protected UIPanel _panel;

	private bool? _isFocused;

	private object _focusedContext;

	private object _exitingContext;

	private ScreenFlow _screenFlow;

	private ScreenFlow _screenFlowForNavigateEndFlow;

	private List<ScreenBase> _layers;

	private ScreenTweenType _tweenType;

	protected bool _isReadyToExitTransition = true;

	GameObject ScreenInputElements.IScreen._pGameObject
	{
		get
		{
			return base.gameObject;
		}
	}

	public bool _pIsShowing
	{
		get
		{
			return _isVisible;
		}
	}

	public bool _pIsManualShow
	{
		get
		{
			return _isManualShow;
		}
	}

	public ScreenTweenType _pCurrentTweenType
	{
		get
		{
			return _tweenType;
		}
	}

	public bool _pIsTweening
	{
		get
		{
			return _tweenType != ScreenTweenType.Idle;
		}
	}

	protected float _pAlpha
	{
		get
		{
			return (!(_panel != null)) ? 1f : _panel.alpha;
		}
		set
		{
			if (_panel != null)
			{
				_panel.alpha = value;
			}
		}
	}

	public int _pDepth
	{
		get
		{
			return (!(_panel != null)) ? (-1) : _panel.depth;
		}
		protected set
		{
			if (_panel != null)
			{
				_panel.depth = value;
			}
		}
	}

	public string _pScreenName
	{
		get
		{
			return base.gameObject.name;
		}
	}

	public ScreenFlow _pScreenFlow
	{
		get
		{
			return _screenFlow;
		}
	}

	public IList<ScreenBase> _pLayers
	{
		get
		{
			return _layers;
		}
	}

	public bool IsReadyToExitTransition
	{
		get
		{
			return _isReadyToExitTransition;
		}
	}

	public event Action _pOnShowScreen;

	public event Action _pOnShowComplete;

	public event Action _pOnExitScreen;

	public event Action _pOnExitComplete;

	public event Action _pOnLayoutChanged;

	protected virtual void Awake()
	{
		_panel = GetComponent<UIPanel>();
		_focusedContext = new BooleanStateRequests.Context(base.name + ":IsFocused");
		_exitingContext = new BooleanStateRequests.Context(base.name + ":IsExiting");
	}

	public void RegisterScreen()
	{
		Facades<ScreenFacade>.Instance.RegisterScreen(this);
		OnRegisterScreen();
	}

	public virtual void OnAllScreensLoaded()
	{
	}

	public void AddLayer(ScreenBase layer)
	{
		if (!(layer == null))
		{
			if (_layers == null)
			{
				_layers = new List<ScreenBase>();
			}
			if (!_layers.Contains(layer))
			{
				_layers.Add(layer);
			}
			if (_isVisible && !layer._isVisible)
			{
				layer.ShowScreen(_screenFlow);
			}
		}
	}

	public void RemoveLayer(ScreenBase layer)
	{
		if (!(layer == null) && _layers != null && _layers.Contains(layer))
		{
			if (_isVisible && layer._isVisible)
			{
				layer.ExitScreen();
			}
			_layers.Remove(layer);
		}
	}

	public void RemoveAllLayers()
	{
		if (_layers == null)
		{
			return;
		}
		if (_isVisible)
		{
			for (int i = 0; i < _layers.Count; i++)
			{
				if (_layers[i] != null && _layers[i]._isVisible)
				{
					_layers[i].ExitScreen();
				}
			}
		}
		_layers.Clear();
	}

	public void SetLayers(params ScreenBase[] layers)
	{
		RemoveAllLayers();
		if (layers != null)
		{
			for (int i = 0; i < layers.Length; i++)
			{
				AddLayer(layers[i]);
			}
		}
	}

	public void ForEachLayer(bool includeSelf, Action<ScreenBase> action, Predicate<ScreenBase> layerMatch = null)
	{
		if (action == null)
		{
			return;
		}
		if (includeSelf)
		{
			action(this);
		}
		IList<ScreenBase> pLayers = _pLayers;
		if (pLayers == null)
		{
			return;
		}
		for (int i = 0; i < pLayers.Count; i++)
		{
			if (!(pLayers[i] == null) && (layerMatch == null || layerMatch(pLayers[i])))
			{
				action(pLayers[i]);
			}
		}
	}

	public JobCounter ForEachLayerAsync(bool includeSelf, Action<ScreenBase, Action> asyncAction, Action onComplete = null, Predicate<ScreenBase> layerMatch = null)
	{
		if (asyncAction == null)
		{
			if (onComplete != null)
			{
				onComplete();
			}
			return null;
		}
		JobCounter actionJobs = new JobCounter(onComplete);
		Action<ScreenBase> action = (ScreenBase s) =>
		{
			actionJobs.BeginJob();
			asyncAction(s, () =>
			{
				actionJobs.EndJob();
			});
		};
		actionJobs.BeginJob();
		if (includeSelf)
		{
			action(this);
		}
		IList<ScreenBase> pLayers = _pLayers;
		if (pLayers != null)
		{
			for (int num = 0; num < pLayers.Count; num++)
			{
				if (!(pLayers[num] == null) && (layerMatch == null || layerMatch(pLayers[num])))
				{
					action(pLayers[num]);
				}
			}
		}
		actionJobs.EndJob();
		return (!actionJobs._pHasJobs) ? null : actionJobs;
	}

	public ScreenBase FindLayer(bool includeSelf, Predicate<ScreenBase> pred)
	{
		IList<ScreenBase> pLayers = _pLayers;
		if (pred == null)
		{
			return includeSelf ? this : ((pLayers == null || pLayers.Count <= 0) ? null : pLayers[0]);
		}
		if (includeSelf && pred(this))
		{
			return this;
		}
		if (pLayers != null)
		{
			for (int i = 0; i < pLayers.Count; i++)
			{
				if (!(pLayers[i] == null) && pred(pLayers[i]))
				{
					return pLayers[i];
				}
			}
		}
		return null;
	}

	private void Initialise()
	{
		_transitionWidgets = base.gameObject.GetComponentsInChildren<ScreenObjectTransitionWidget>(true);
		_numTransitionWidgets = _transitionWidgets.Length;
		_initialised = true;
		AmuzoNGUILocalize[] componentsInChildren = base.gameObject.GetComponentsInChildren<AmuzoNGUILocalize>(true);
		int num = componentsInChildren.Length;
		for (int i = 0; i < num; i++)
		{
			componentsInChildren[i].AssignTextFromKey();
		}
		OnInitialise();
	}

	protected virtual void OnRegisterScreen()
	{
	}

	protected virtual void OnInitialise()
	{
	}

	protected virtual void OnShowScreen()
	{
		if (_pOnShowScreen != null)
		{
			_pOnShowScreen();
		}
		LoadTextureResources();
	}

	protected virtual void OnExitScreen()
	{
		if (_pOnExitScreen != null)
		{
			_pOnExitScreen();
		}
	}

	protected virtual void OnScreenShowComplete()
	{
		ForceEndTransitionWidgetsAutoTweenIn();
		if (_onNextShowComplete != null)
		{
			_onNextShowComplete();
			_onNextShowComplete = null;
		}
		if (_pOnShowComplete != null)
		{
			_pOnShowComplete();
		}
	}

	protected virtual void OnScreenExitComplete()
	{
		ForceEndTransitionWidgetsAutoTweenOut();
		_isVisible = false;
		_isManualShow = false;
		_screenFlow = null;
		_tweenType = ScreenTweenType.Idle;
		base.gameObject.SetActive(false);
		if (_isDisableInputWhileExiting)
		{
			RequestDisableInput(false, _exitingContext);
		}
		if (_pOnExitComplete != null)
		{
			_pOnExitComplete();
		}
		UnloadTextureResources();
		if (_onNextExitComplete != null)
		{
			Action onNextExitComplete = _onNextExitComplete;
			_onNextExitComplete = null;
			onNextExitComplete();
		}
	}

	public static string formatCamelCaseForDebug(string camelCase)
	{
		string text = camelCase.Substring(0, 1).ToUpper();
		for (int i = 1; i < camelCase.Length; i++)
		{
			string text2 = camelCase.Substring(i, 1);
			text = ((!(text2 == text2.ToUpper())) ? (text + text2) : (text + " " + text2));
		}
		return text;
	}

	public void Navigate(ScreenBase nextScreen, bool doClearHistory = false, bool doAddCurrentScreenToHistory = true, Action onComplete = null)
	{
		if (nextScreen == null)
		{
			Debug.LogError("Null Next Screen");
		}
		else if (_screenFlowForNavigateEndFlow == null)
		{
			Debug.LogError(base.name + ": Navigate: Null _screenFlowForNavigateEndFlow");
		}
		else
		{
			_screenFlowForNavigateEndFlow.ShowScreen(nextScreen, doClearHistory, doAddCurrentScreenToHistory, onComplete);
		}
	}

	public void NavigateBack(Action onComplete = null)
	{
		if (_screenFlowForNavigateEndFlow == null)
		{
			Debug.LogError(base.name + ": NavigateBack: Null _screenFlowForNavigateEndFlow");
		}
		else
		{
			_screenFlowForNavigateEndFlow.ShowPreviousScreen(onComplete);
		}
	}

	public void EndFlow(Action onComplete = null)
	{
		if (_screenFlowForNavigateEndFlow == null)
		{
			Debug.LogError(base.name + ": EndFlow: Null _screenFlowForNavigateEndFlow");
		}
		else
		{
			_screenFlowForNavigateEndFlow.End(onComplete);
		}
	}

	public void ShowPopupOverlay(ScreenBase popupOverlay, Action onShowComplete = null, Action onPopupOverlayEnd = null)
	{
		Facades<ScreenFacade>.Instance.ShowPopupOverlay(popupOverlay, onShowComplete, onPopupOverlayEnd);
	}

	public virtual void ShowScreen(ScreenFlow screenFlow, Action onComplete = null)
	{
		if (!_initialised)
		{
			Initialise();
		}
		if (_screenFlow != null && _screenFlow != screenFlow)
		{
			Debug.LogError("ShowScreen: '" + base.name + "' already has a different screen flow set, ie already shown in a different flow!");
			return;
		}
		_screenFlow = screenFlow;
		_screenFlowForNavigateEndFlow = screenFlow;
		bool isVisible = _isVisible;
		base.gameObject.SetActive(true);
		_isVisible = true;
		OnShowScreen();
		_onNextShowComplete = (Action)Delegate.Combine(_onNextShowComplete, onComplete);
		if (isVisible)
		{
			OnScreenShowComplete();
			return;
		}
		_screenStartTime = 0f;
		int num = 0;
		for (int i = 0; i < _numTransitionWidgets; i++)
		{
			float num2 = 0f;
			if (_transitionWidgets[i] == null)
			{
				continue;
			}
			if (_transitionWidgets[i]._pCurrentPropertySet == null)
			{
				Debug.LogWarning("Widget has no property sets: " + _transitionWidgets[i].name, _transitionWidgets[i].gameObject);
			}
			else if (_transitionWidgets[i]._pCurrentPropertySet.autoTweenInOnScreenTransition)
			{
				num++;
				if (_transitionWidgets[i]._pCurrentPropertySet.adjustsColorIn)
				{
					num2 = _transitionWidgets[i]._pCurrentPropertySet.colorInDelay + _transitionWidgets[i]._pCurrentPropertySet._pAdditionalDelayColorIn + _transitionWidgets[i]._pCurrentPropertySet.colorInDuration;
				}
				if (_transitionWidgets[i]._pCurrentPropertySet.adjustsPositionIn)
				{
					float b = _transitionWidgets[i]._pCurrentPropertySet.positionInDelay + _transitionWidgets[i]._pCurrentPropertySet._pAdditionalDelayPosIn + _transitionWidgets[i]._pCurrentPropertySet.positionInDuration;
					num2 = Mathf.Max(num2, b);
				}
				if (_transitionWidgets[i]._pCurrentPropertySet.adjustsScaleIn)
				{
					float b2 = _transitionWidgets[i]._pCurrentPropertySet.scaleInDelay + _transitionWidgets[i]._pCurrentPropertySet._pAdditionalDelayScaleIn + _transitionWidgets[i]._pCurrentPropertySet.scaleInDuration;
					num2 = Mathf.Max(num2, b2);
				}
				_transitionWidgets[i].TweenIn(true);
				_screenStartTime = Mathf.Max(_screenStartTime, num2);
			}
		}
		_screenStartTime += RealTime.time;
		if (num > 0)
		{
			_tweenType = ScreenTweenType.TweenIn;
			UIPanel component = GetComponent<UIPanel>();
			if (component != null)
			{
				component.SetDirty();
			}
		}
		else
		{
			OnScreenShowComplete();
		}
	}

	public void CancelShowScreen()
	{
		if (_isVisible)
		{
			_onNextShowComplete = null;
			ExitScreen(null, true);
		}
	}

	public virtual float ExitScreen(Action onComplete = null, bool isForceExitImmediate = false)
	{
		if (_isDisableInputWhileExiting)
		{
			RequestDisableInput(true, _exitingContext);
		}
		OnExitScreen();
		_onNextExitComplete = (Action)Delegate.Combine(_onNextExitComplete, onComplete);
		_screenExitTime = 0f;
		int num = 0;
		for (int i = 0; i < _numTransitionWidgets; i++)
		{
			if (isForceExitImmediate)
			{
				continue;
			}
			float num2 = 0f;
			if (_transitionWidgets[i] == null)
			{
				continue;
			}
			if (_transitionWidgets[i]._pCurrentPropertySet == null)
			{
				Debug.LogWarning("Widget has no property sets: " + _transitionWidgets[i].name, _transitionWidgets[i].gameObject);
			}
			else if (_transitionWidgets[i]._pCurrentPropertySet.autoTweenOutOnScreenTransition)
			{
				num++;
				if (_transitionWidgets[i]._pCurrentPropertySet.adjustsColorOut)
				{
					num2 = _transitionWidgets[i]._pCurrentPropertySet.colorOutDelay + _transitionWidgets[i]._pCurrentPropertySet._pAdditionalDelayColorOut + _transitionWidgets[i]._pCurrentPropertySet.colorOutDuration;
				}
				if (_transitionWidgets[i]._pCurrentPropertySet.adjustsPositionOut)
				{
					float b = _transitionWidgets[i]._pCurrentPropertySet.positionOutDelay + _transitionWidgets[i]._pCurrentPropertySet._pAdditionalDelayPosOut + _transitionWidgets[i]._pCurrentPropertySet.positionOutDuration;
					num2 = Mathf.Max(num2, b);
				}
				if (_transitionWidgets[i]._pCurrentPropertySet.adjustsScaleOut)
				{
					float b2 = _transitionWidgets[i]._pCurrentPropertySet.scaleOutDelay + _transitionWidgets[i]._pCurrentPropertySet._pAdditionalDelayScaleOut + _transitionWidgets[i]._pCurrentPropertySet.scaleOutDuration;
					num2 = Mathf.Max(num2, b2);
				}
				_transitionWidgets[i].TweenOut(true);
				_screenExitTime = Mathf.Max(_screenExitTime, num2);
			}
		}
		float screenExitTime = _screenExitTime;
		_screenExitTime += RealTime.time;
		if (num > 0)
		{
			_tweenType = ScreenTweenType.TweenOut;
		}
		else
		{
			OnScreenExitComplete();
		}
		return screenExitTime;
	}

	public virtual float getExitTime()
	{
		if (_numTransitionWidgets == 0)
		{
			return 0f;
		}
		_screenExitTime = 0f;
		for (int i = 0; i < _numTransitionWidgets; i++)
		{
			float num = 0f;
			if (!(_transitionWidgets[i] == null) && _transitionWidgets[i]._pCurrentPropertySet != null && _transitionWidgets[i]._pCurrentPropertySet.autoTweenOutOnScreenTransition)
			{
				if (_transitionWidgets[i]._pCurrentPropertySet.adjustsColorOut)
				{
					num = _transitionWidgets[i]._pCurrentPropertySet.colorOutDelay + _transitionWidgets[i]._pCurrentPropertySet._pAdditionalDelayColorOut + _transitionWidgets[i]._pCurrentPropertySet.colorOutDuration;
				}
				if (_transitionWidgets[i]._pCurrentPropertySet.adjustsPositionOut)
				{
					float b = _transitionWidgets[i]._pCurrentPropertySet.positionOutDelay + _transitionWidgets[i]._pCurrentPropertySet._pAdditionalDelayPosOut + _transitionWidgets[i]._pCurrentPropertySet.positionOutDuration;
					num = Mathf.Max(num, b);
				}
				if (_transitionWidgets[i]._pCurrentPropertySet.adjustsScaleOut)
				{
					float b2 = _transitionWidgets[i]._pCurrentPropertySet.scaleOutDelay + _transitionWidgets[i]._pCurrentPropertySet._pAdditionalDelayScaleOut + _transitionWidgets[i]._pCurrentPropertySet.scaleOutDuration;
					num = Mathf.Max(num, b2);
				}
				_screenExitTime = Mathf.Max(_screenExitTime, num);
			}
		}
		return _screenExitTime;
	}

	protected void TryChangeWidgetSets(GameObject rootWidgetObject, string setName)
	{
		if (!_objectWidgetLookup.ContainsKey(rootWidgetObject))
		{
			_objectWidgetLookup[rootWidgetObject] = rootWidgetObject.GetComponentsInChildren<ScreenObjectTransitionWidget>();
		}
		int num = _objectWidgetLookup[rootWidgetObject].Length;
		for (int i = 0; i < num; i++)
		{
			_objectWidgetLookup[rootWidgetObject][i].TryChangeSet(setName);
		}
	}

	protected virtual void Update()
	{
		if (_tweenType == ScreenTweenType.Idle)
		{
			return;
		}
		if (_tweenType == ScreenTweenType.TweenOut)
		{
			if (RealTime.time >= _screenExitTime)
			{
				_tweenType = ScreenTweenType.Idle;
				OnScreenExitComplete();
			}
		}
		else if (_tweenType == ScreenTweenType.TweenIn && RealTime.time >= _screenStartTime)
		{
			_tweenType = ScreenTweenType.Idle;
			if (_panel != null)
			{
				_panel.SetDirty();
			}
			OnScreenShowComplete();
		}
	}

	public static Color getColourFromArray(Color[] colours, float decimalProgress)
	{
		if (colours.Length < 2)
		{
			Debug.Log("ScreenBase: Must have at least 2 colours in order to use 'getColourFromArray'!");
			return Color.white;
		}
		float num = 1f / (float)colours.Length;
		float t = decimalProgress % num / num;
		float num2 = 0f;
		Color white = Color.white;
		Color white2 = Color.white;
		for (int i = 0; i < colours.Length; i++)
		{
			num2 += num;
			if (num2 > decimalProgress)
			{
				white = colours[i];
				white2 = colours[(i + 1) % colours.Length];
				return Color.Lerp(white, white2, t);
			}
		}
		return Color.white;
	}

	protected void LoadTextureResources()
	{
		CheckInitTextureResources();
		for (int i = 0; i < _textureResourceLoaders.Length; i++)
		{
			_textureResourceLoaders[i].AssignTexture();
		}
	}

	protected void UnloadTextureResources()
	{
		CheckInitTextureResources();
		for (int i = 0; i < _textureResourceLoaders.Length; i++)
		{
			_textureResourceLoaders[i].UnAssignTexture();
		}
	}

	private void CheckInitTextureResources()
	{
		if (_textureResourceLoaders == null)
		{
			_textureResourceLoaders = base.gameObject.GetComponentsInChildren<UITextureResourceLoader>(true);
		}
	}

	public void OnScreensChanged()
	{
		CheckFocused();
	}

	protected void CheckFocused()
	{
		bool focused = OnCheckFocused();
		SetFocused(focused);
	}

	protected virtual bool OnCheckFocused()
	{
		return OnCheckFocusedDefault();
	}

	protected bool OnCheckFocusedDefault()
	{
		if (!_isVisible)
		{
			return false;
		}
		if (Facades<ScreenFacade>.Instance == null)
		{
			return true;
		}
		return Facades<ScreenFacade>.Instance.IsScreenOnTop(this);
	}

	private void SetFocused(bool newState)
	{
		if (!_isFocused.HasValue || _isFocused.Value != newState)
		{
			_isFocused = newState;
			OnSetFocused();
		}
	}

	protected virtual void OnSetFocused()
	{
		OnSetFocusedDefault();
	}

	protected void OnSetFocusedDefault()
	{
		if (_isFocused.HasValue)
		{
			if (_isDisableInputWhileUnfocused)
			{
				RequestDisableInputElements(!_isFocused.Value, _focusedContext);
			}
			if (_isFocusFade)
			{
				_pAlpha = ((!_isFocused.Value) ? 0.5f : 1f);
			}
		}
	}

	private void CheckInitInputElements()
	{
		if (_inputElements == null)
		{
			_inputElements = new ScreenInputElements(this);
		}
	}

	private void RequestDisableInputElements(bool wantDisabled, object context)
	{
		CheckInitInputElements();
		_inputElements.RequestDisableElements(wantDisabled, context);
	}

	public void RequestDisableInput(bool wantDisabled, object context)
	{
		RequestDisableInputElements(wantDisabled, context);
	}

	public void ForEachButton(Action<ScreenButton> action)
	{
		CheckInitInputElements();
		_inputElements.ForEachElement(action);
	}

	public ScreenButton FindButton(Predicate<ScreenButton> pred)
	{
		CheckInitInputElements();
		return _inputElements.FindElement(pred);
	}

	protected string Localise(string key)
	{
		string text = LocalisationFacade.Instance.GetString(key);
		if (text == string.Empty || text == null)
		{
			return key;
		}
		return text;
	}

	public virtual void OnDeviceBackButtonPressed(ref bool isHandled)
	{
	}

	protected void NotifyLayoutChanged()
	{
		if (_pOnLayoutChanged != null)
		{
			_pOnLayoutChanged();
		}
	}

	private void ForceEndTransitionWidgetsAutoTweenIn()
	{
		for (int i = 0; i < _numTransitionWidgets; i++)
		{
			if (!(_transitionWidgets[i] == null) && _transitionWidgets[i]._pCurrentPropertySet != null && _transitionWidgets[i]._pCurrentPropertySet.autoTweenInOnScreenTransition && _transitionWidgets[i]._pTweenType == ScreenObjectTransitionWidget.TweenType.TWEEN_IN)
			{
				_transitionWidgets[i].ForceEndTween();
			}
		}
	}

	private void ForceEndTransitionWidgetsAutoTweenOut()
	{
		for (int i = 0; i < _numTransitionWidgets; i++)
		{
			if (!(_transitionWidgets[i] == null) && _transitionWidgets[i]._pCurrentPropertySet != null && _transitionWidgets[i]._pCurrentPropertySet.autoTweenOutOnScreenTransition && _transitionWidgets[i]._pTweenType == ScreenObjectTransitionWidget.TweenType.TWEEN_OUT)
			{
				_transitionWidgets[i].ForceEndTween();
			}
		}
	}
}
