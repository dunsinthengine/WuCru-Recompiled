using System;
using System.Collections;
using UnityEngine;

public class ControllerVideoPlayer : MonoBehaviour
{
	private const string DEBUG_LOG_PREFIX = "VIDEO PLAYER CONTROLLER - ";

	private const float TIME_SHOW_AD_WARNING = 3f;

	private const float TIME_POLL_INTERNET = 2f;

	private const float TIME_SHOW_CTA = 3f;

	private AudioSource _audioSource;

	private static ControllerVideoPlayer _instance;

	public GameObject _resizableScreen;

	public GameObject _loadingScreen;

	public GameObject _advertWarningScreen;

	public GameObject _ctaScreen;

	public UITexture _ctaImage;

	public GameObject _notReadyScreen;

	public MediaPlayerCtrl _videoManager;

	public GameObject _spinner;

	public float _spinSpeed = 20f;

	private float _minLoadingScreenTime = 2f;

	private float _loadingScreenTimer;

	private bool _isLoadingScreenActive;

	private bool _isErrorScreenActive;

	private int _videoDuration;

	private bool _isVideoLoaded;

	private bool _isVideoLoading;

	private bool _isPlaying;

	private bool _shouldBePaused;

	private bool _isVideoVisible = true;

	private bool _isNextVideoAdvert;

	private bool _isNextVideoCta;

	private bool _hasCtaImageSet;

	private float _initialLocalScaleX;

	private float _initialLocalScaleY;

	private float _timerAdWarning;

	private float _timerPoll;

	private float _timerCta;

	private string _log = string.Empty;

	private MediaPlayerCtrl.MEDIAPLAYER_STATE _prevVideoState;

	private AudioSource _pAudioSource
	{
		get
		{
			if (_audioSource == null)
			{
				_audioSource = GetComponentInChildren<AudioSource>();
			}
			return _audioSource;
		}
	}

	public static ControllerVideoPlayer _pInstance
	{
		get
		{
			return _instance;
		}
	}

	public bool _pIsVideoLoaded
	{
		get
		{
			return _isVideoLoaded;
		}
		private set
		{
			if (value != _isVideoLoaded)
			{
				_isVideoLoaded = value;
				if (_onVideoLoadedChanged != null)
				{
					_onVideoLoadedChanged(_isVideoLoaded);
				}
			}
		}
	}

	public bool _pIsPlaying
	{
		get
		{
			return _isPlaying;
		}
		private set
		{
			if (value != _isPlaying)
			{
				_isPlaying = value;
				if (_onVideoPlayingChanged != null)
				{
					_onVideoPlayingChanged(_isPlaying);
				}
			}
		}
	}

	public bool _pIsVideoVisible
	{
		get
		{
			return _isVideoVisible;
		}
		private set
		{
			if (value != _isVideoVisible)
			{
				_isVideoVisible = value;
			}
		}
	}

	public bool _pIsCtaVisible
	{
		get
		{
			return _timerCta > 0f;
		}
	}

	public float _pNormVideoProgress
	{
		get
		{
			if (_videoManager == null)
			{
				return 0f;
			}
			return (_videoDuration != 0) ? ((float)_videoManager.GetSeekPosition() / (float)_videoDuration) : 0f;
		}
	}

	public event Action<bool> _onVideoLoadedChanged;

	public event Action<bool> _onVideoPlayingChanged;

	public event Action _onVideoLoaded;

	public event Action _onVideoPlaying;

	private void Awake()
	{
		if (_instance != null)
		{
			Debug.Log("VIDEO PLAYER CONTROLLER - Awake - instance already exists! there should never be more then 1");
		}
		_instance = this;
		if (_resizableScreen != null)
		{
			_initialLocalScaleX = _resizableScreen.transform.localScale.x;
			_initialLocalScaleY = _resizableScreen.transform.localScale.y;
		}
		if (_videoManager != null)
		{
			MediaPlayerCtrl videoManager = _videoManager;
			videoManager.OnReady = (MediaPlayerCtrl.VideoReady)Delegate.Combine(videoManager.OnReady, new MediaPlayerCtrl.VideoReady(OnVideoReady));
			MediaPlayerCtrl videoManager2 = _videoManager;
			videoManager2.OnVideoFirstFrameReady = (MediaPlayerCtrl.VideoFirstFrameReady)Delegate.Combine(videoManager2.OnVideoFirstFrameReady, new MediaPlayerCtrl.VideoFirstFrameReady(OnVideoFirstFrameReady));
			MediaPlayerCtrl videoManager3 = _videoManager;
			videoManager3.OnEnd = (MediaPlayerCtrl.VideoEnd)Delegate.Combine(videoManager3.OnEnd, new MediaPlayerCtrl.VideoEnd(OnVideoEnd));
			MediaPlayerCtrl videoManager4 = _videoManager;
			videoManager4.OnVideoError = (MediaPlayerCtrl.VideoError)Delegate.Combine(videoManager4.OnVideoError, new MediaPlayerCtrl.VideoError(OnVideoError));
		}
		if (ManagerCrossPromo._pInstance._pHasCtaImageToLookUp)
		{
			LookUpCtaImage();
		}
		_pIsVideoLoaded = false;
		_isVideoLoading = false;
		_pIsVideoVisible = false;
		_shouldBePaused = false;
		_videoDuration = 0;
		DisplayErrorOrNotReadyScreen();
	}

	private void OnDisable()
	{
		_instance = null;
	}

	private void OnDestroy()
	{
		_instance = null;
	}

	private void Update()
	{
		if (_videoManager != null && _videoManager.GetCurrentState() != _prevVideoState)
		{
			OnVideoStateChanged(_prevVideoState, _videoManager.GetCurrentState());
			_prevVideoState = _videoManager.GetCurrentState();
		}
		UpdateLoadingScreen();
		UpdateErrorOrNotReadyScreen();
		if (_timerAdWarning > 0f)
		{
			UpdateAdvertWarning();
		}
		else if (_timerCta > 0f)
		{
			UpdateCtaScreen();
		}
		if (_pIsPlaying && _videoManager != null)
		{
			float num = ((!GameAudioManager.Instance.FxMuted) ? 1f : 0f);
			if (_videoManager.GetVolume() != num)
			{
				_videoManager.SetVolume(num);
			}
		}
	}

	public void Next()
	{
		if (ManagerCrossPromo._pInstance._pCanPlayVideos && !_shouldBePaused && (!_isVideoLoading || !_isLoadingScreenActive))
		{
			if (!_isLoadingScreenActive)
			{
				DisplayLoadingScreen();
			}
			string videoUrl = string.Empty;
			if (ManagerCrossPromo._pInstance._pCanPlayVideos)
			{
				ManagerCrossPromo._pInstance.GetVideoUrl(out videoUrl, out _isNextVideoAdvert, out _isNextVideoCta);
				_videoManager.Load(videoUrl);
				Log("Loading: " + videoUrl);
			}
		}
	}

	public void Pause(bool pause)
	{
		_shouldBePaused = pause;
		ManagerCrossPromo._pInstance.PauseSetup(pause);
		if (_videoManager == null)
		{
			return;
		}
		switch (_videoManager.GetCurrentState())
		{
		case MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING:
			if (_shouldBePaused)
			{
				_videoManager.Pause();
			}
			break;
		case MediaPlayerCtrl.MEDIAPLAYER_STATE.PAUSED:
			if (!_shouldBePaused)
			{
				_videoManager.Play();
			}
			break;
		default:
			if (!_shouldBePaused)
			{
				Next();
			}
			break;
		}
	}

	public void LookUpCtaImage()
	{
		if (ManagerCrossPromo._pInstance._pHasCtaImageToLookUp && !(_ctaImage == null))
		{
			_hasCtaImageSet = false;
			DownloadRequest downloadRequest = new DownloadRequest();
			downloadRequest.requestName = "CtaLookup";
			downloadRequest.requestUrl = ManagerCrossPromo._pInstance._pUrlCtaImage;
			downloadRequest._requestTimeout = 6f;
			downloadRequest.doLocalDownload = false;
			downloadRequest.SetAssetType(EDownloadAssetType.TEXTURE);
			downloadRequest.onSuccessCallBack += (DownloadRequest request) =>
			{
				StartCoroutine(OnCtaImageDownloadSuccess(request));
			};
			downloadRequest.onFailCallback += (DownloadRequest request) =>
			{
				Debug.LogWarning("VIDEO PLAYER CONTROLLER - CTA IMAGE DOWNLOAD FAILED: " + request.requestUrl);
			};
			downloadRequest.download();
		}
	}

	public IEnumerator OnCtaImageDownloadSuccess(DownloadRequest request)
	{
		Debug.Log("VIDEO PLAYER CONTROLLER - CTA IMAGE DOWNLOAD RESPONCE RECIEVED");
		if (string.IsNullOrEmpty(request.localFilePath))
		{
			yield break;
		}
		WWW www = new WWW(FileHelper.AddLocalFileUrlScheme(request.localFilePath));
		yield return www;
		if (string.IsNullOrEmpty(www.error))
		{
			if (_ctaImage != null)
			{
				_ctaImage.mainTexture = www.texture;
			}
			_hasCtaImageSet = true;
		}
		www.Dispose();
		www = null;
	}

	private void DisplayErrorOrNotReadyScreen()
	{
		_isErrorScreenActive = true;
		if (_notReadyScreen != null)
		{
			_notReadyScreen.SetActive(true);
			if (_pAudioSource != null)
			{
				_pAudioSource.enabled = false;
			}
		}
	}

	private void HideErrorOrNotReadyScreen()
	{
		_isErrorScreenActive = false;
		if (_notReadyScreen != null)
		{
			_notReadyScreen.SetActive(false);
			if (_pAudioSource != null)
			{
				_pAudioSource.enabled = true;
			}
		}
	}

	private void UpdateErrorOrNotReadyScreen()
	{
		_timerPoll += Time.unscaledDeltaTime;
		if (_timerPoll > 2f)
		{
			_timerPoll = 0f;
			if (ManagerCrossPromo._pInstance._pCanPlayVideos && _isErrorScreenActive)
			{
				Next();
			}
			else if (!ManagerCrossPromo._pInstance._pCanPlayVideos && !_isErrorScreenActive)
			{
				DisplayErrorOrNotReadyScreen();
			}
		}
	}

	private void DisplayLoadingScreen()
	{
		_isLoadingScreenActive = true;
		_loadingScreenTimer = _minLoadingScreenTime;
		HideErrorOrNotReadyScreen();
		if (_loadingScreen != null)
		{
			_loadingScreen.SetActive(true);
		}
		HideAdvertWarning();
	}

	private void HideLoadingScreen()
	{
		_isLoadingScreenActive = false;
		if (_loadingScreen != null)
		{
			_loadingScreen.SetActive(false);
		}
		if (_isNextVideoAdvert && _advertWarningScreen != null)
		{
			DisplayAdvertWarning();
		}
		else
		{
			PlayLoadedVideo();
		}
	}

	private void UpdateLoadingScreen()
	{
		if (_isLoadingScreenActive)
		{
			_loadingScreenTimer -= Time.deltaTime;
			if (_spinner != null)
			{
				_spinner.transform.eulerAngles -= new Vector3(0f, 0f, Time.unscaledDeltaTime * _spinSpeed);
			}
			if (_loadingScreenTimer <= 0f && _pIsVideoLoaded)
			{
				_pIsVideoLoaded = false;
				HideLoadingScreen();
			}
		}
	}

	private void ShowCtaScreen()
	{
		if (_ctaScreen != null)
		{
			_timerCta = 3f;
			_ctaScreen.SetActive(true);
		}
	}

	private void HideCtaScreen()
	{
		_timerCta = 0f;
		if (_ctaScreen != null)
		{
			_ctaScreen.SetActive(false);
		}
	}

	private void UpdateCtaScreen()
	{
		_timerCta -= Time.deltaTime;
		if (_timerCta < 0f)
		{
			HideCtaScreen();
			Next();
		}
	}

	private void DisplayAdvertWarning()
	{
		HideErrorOrNotReadyScreen();
		if (_advertWarningScreen != null)
		{
			_timerAdWarning = 3f;
			_advertWarningScreen.SetActive(true);
		}
	}

	private void HideAdvertWarning()
	{
		_timerAdWarning = 0f;
		if (_advertWarningScreen != null)
		{
			_advertWarningScreen.SetActive(false);
		}
	}

	private void UpdateAdvertWarning()
	{
		_timerAdWarning -= Time.deltaTime;
		if (_timerAdWarning < 0f)
		{
			HideAdvertWarning();
			PlayLoadedVideo();
		}
	}

	private void UpdateVideoLayout()
	{
		float num = ((!(_videoManager == null)) ? ((float)_videoManager.GetVideoWidth()) : 0f);
		float num2 = ((!(_videoManager == null)) ? ((float)_videoManager.GetVideoHeight()) : 0f);
		if (num != 0f && num2 != 0f)
		{
			float num3 = num2 / num;
			if (_resizableScreen != null)
			{
				_resizableScreen.transform.localScale = new Vector3(_initialLocalScaleY / num3, _initialLocalScaleY, 1f);
			}
		}
	}

	private void Log(string message)
	{
		_log = _log + "   " + message + "\n";
	}

	private void OnGUI()
	{
	}

	private void OnVideoStateChanged(MediaPlayerCtrl.MEDIAPLAYER_STATE prevState, MediaPlayerCtrl.MEDIAPLAYER_STATE newState)
	{
		if (prevState == MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING != (newState == MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING))
		{
			OnVideoPlayingChanged(newState == MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING);
		}
		Log("State Changed: " + newState);
	}

	private void OnVideoPlayingChanged(bool isPlaying)
	{
		_pIsPlaying = isPlaying;
	}

	private void OnVideoLoaded()
	{
		_pIsVideoLoaded = true;
		_isVideoLoading = false;
		if (_onVideoLoaded != null)
		{
			_onVideoLoaded();
		}
	}

	private void PlayLoadedVideo()
	{
		HideAdvertWarning();
		HideErrorOrNotReadyScreen();
		_pIsVideoVisible = true;
		UpdateVideoLayout();
		_videoDuration = ((!(_videoManager == null)) ? _videoManager.GetDuration() : 0);
		if (_videoManager != null)
		{
			_videoManager.SetVolume((!GameAudioManager.Instance.FxMuted) ? 1f : 0f);
			_videoManager.Play();
			if (_onVideoPlaying != null)
			{
				_onVideoPlaying();
			}
		}
	}

	private void OnVideoReady()
	{
		Log("OnReady");
		OnVideoLoaded();
	}

	private void OnVideoFirstFrameReady()
	{
		Log("OnFirstFrameReady");
	}

	private void OnVideoEnd()
	{
		Log("OnEnd");
		if (_isNextVideoCta && _hasCtaImageSet)
		{
			ShowCtaScreen();
		}
		else
		{
			Next();
		}
	}

	private void OnVideoError(MediaPlayerCtrl.MEDIAPLAYER_ERROR errorCode, MediaPlayerCtrl.MEDIAPLAYER_ERROR errorCodeExtra)
	{
		Log(string.Concat("OnError: ", errorCode, ", ", errorCodeExtra));
	}
}
