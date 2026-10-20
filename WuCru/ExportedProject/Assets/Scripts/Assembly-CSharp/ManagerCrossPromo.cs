using System.Collections.Generic;
using LEGO;
using LitJson;
using UnityEngine;

public class ManagerCrossPromo
{
	private enum EState_CrossPromo
	{
		DECLARED = 0,
		FETCHING_VIDEOS = 1,
		PAUSED = 2,
		COMPLETE = 3,
		BROKENDOWN = 4
	}

	internal class VideoData
	{
		internal bool _isAd;

		internal bool _isCta;

		internal string _id;

		internal string _url;
	}

	private const bool DEBUG_DOWNLOAD_REQUESTS = false;

	private const bool SHUFFLE_LIST = false;

	private const float TIMEOUT_REQUEST = 3f;

	private const string URL_CTA = "https://c00.adobe.com/v3/1850813cf55379df22851ea66b55ef8f407d1fa9b813bc65d3ae0dc48ea2c607/start?a_dl=58aafef46058ecbcad08a392";

	private const string URL_VIDEO_LIBRARY = "https://wwwsecure.lego.com/{lang}/mediaplayer/api/video.json/";

	private const string KEY_LANG = "{lang}";

	private const string NODENAME_LIST_PROMOTIONS = "PromotionLists";

	private const string NODENAME_HEADERTEXT = "HeaderText";

	private const string NODENAME_ORIENTATION = "Orientation";

	private const string NODENAME_URI = "Uri";

	private const string NODENAME_URL = "Url";

	private const string NODENAME_NEXTPAGE = "NextPage";

	private const string NODENAME_CONTENT = "Content";

	private const string NODENAME_PAGENUMBER = "PageNumber";

	private const string NODENAME_PAGENUMBERTOTAL = "TotalNumberOfPages";

	private const string NODENAME_ID = "Id";

	private const string NODENAME_VIDEOFORMATS = "VideoFormats";

	private const string NODENAME_FORMAT = "Format";

	private const string NODENAME_QUALITY = "Quality";

	private const string NODENAME_HITS = "hits";

	private const string NODENAME_SOURCE = "_source";

	private const string NODENAME_VIDEOTYPE = "videoType";

	private const string NODENAME_TITLE = "title";

	private const string NODENAME_NAME = "name";

	private const string NODENAME_CLASSIFICATION = "classification";

	private const string NODEVALUE_HEADERTEXT = "All Videos";

	private const string NODEVALUE_ORIENTATION = "Horizontal";

	private const string NODEVALUE_MP4 = "Mp4";

	private const string NODEVALUE_LOW = "Low";

	private const string NODEVALUE_TVC = "TVC";

	private const string NODEVALUE_APP = "App";

	private const string HEADER_KEY_API = "x-api-key";

	private const string HEADER_KEY_TYPE = "Content-Type";

	private const string HEADER_VALUE_API = "sA0DCIrasm30BtPNfst248WNfUH9StqNVrlCsu30";

	private const string HEADER_VALUE_TYPE = "application/json";

	private const string POST_DATA_PREFIX = "{\"filter\":{\"term\":{\"id\":\"";

	private const string POST_DATA_SUFFIX = "\"}}}";

	private const string DICT_KEY_APPIMAGE = "appimage";

	private const string DEBUG_LOG_PREFIX = "MANAGER CROSS PROMO - ";

	private const string EMPTY_STRING = "";

	private const string INVALID_STRING = "INVALID";

	private static ManagerCrossPromo _instance;

	public bool _debugLogging;

	public string _debugLang = string.Empty;

	public string _urlVideoListAndroid = "https://wwwsecure.lego.com/services/contentpromotionapi/api/v1/contentpromotion/Video/lego-ninjago-wu-cru/androidphone/8/?languageVersion=";

	public string _urlVideoListIos = "https://wwwsecure.lego.com/services/contentpromotionapi/api/v1/contentpromotion/Video/lego-ninjago-wu-cru/iphone/8/?languageVersion=";

	private int _nextVideoIdIndex;

	private int _nextVideoUrlIndex;

	private int _numUrlsFetched;

	private int _pageCurrent;

	private int _pageTotal;

	private int _numVideosInPage;

	private string _dataGetRequest = string.Empty;

	private string _urlVideoList = string.Empty;

	private string _urlCtaImage = string.Empty;

	private JsonData _pageData;

	private List<VideoData> _videoList;

	private DownloadRequest _getDataRequest;

	private EState_CrossPromo _stateCurrent;

	public static ManagerCrossPromo _pInstance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new ManagerCrossPromo();
			}
			return _instance;
		}
	}

	public bool _pCanPlayVideos
	{
		get
		{
			if (!_pPlatformIsValid)
			{
				return false;
			}
			if (_pIsBroken)
			{
				return false;
			}
			if (!_pReadyForUse)
			{
				return false;
			}
			if (!InternetCheckFacade._pIsOnline)
			{
				return false;
			}
			return true;
		}
	}

	public bool _pReadyForUse
	{
		get
		{
			if (!_pPlatformIsValid)
			{
				return false;
			}
			if (_pHasUrlsToPlay)
			{
				return true;
			}
			return false;
		}
	}

	public bool _pLoadedAllVideos
	{
		get
		{
			if (!_pPlatformIsValid)
			{
				return false;
			}
			return _stateCurrent == EState_CrossPromo.COMPLETE;
		}
	}

	public bool _pIsBroken
	{
		get
		{
			return _stateCurrent == EState_CrossPromo.BROKENDOWN;
		}
	}

	public bool _pIsSettingUp
	{
		get
		{
			return _stateCurrent == EState_CrossPromo.FETCHING_VIDEOS || _stateCurrent == EState_CrossPromo.PAUSED;
		}
	}

	public bool _pHasStarted
	{
		get
		{
			if (_stateCurrent == EState_CrossPromo.DECLARED)
			{
				return false;
			}
			return true;
		}
	}

	public bool _pHasCtaImageToLookUp
	{
		get
		{
			return _urlCtaImage != string.Empty;
		}
	}

	public string _pUrlCtaImage
	{
		get
		{
			return _urlCtaImage;
		}
	}

	private bool _pPlatformIsValid
	{
		get
		{
			return true;
		}
	}

	private bool _pHasUrlsToPlay
	{
		get
		{
			return _numUrlsFetched > 0;
		}
	}

	private ManagerCrossPromo()
	{
		_stateCurrent = EState_CrossPromo.DECLARED;
		Log("MANAGER CROSS PROMO - Constructor, set state to" + _stateCurrent);
		_videoList = new List<VideoData>();
		AssembleDataRequest();
	}

	public bool GetVideoUrl(out string videoUrl, out bool isAdvert, out bool isCta)
	{
		videoUrl = string.Empty;
		isAdvert = false;
		isCta = false;
		if (!_pReadyForUse)
		{
			return false;
		}
		if (_videoList == null || _videoList.Count <= 0)
		{
			return false;
		}
		for (int i = 0; i < _numUrlsFetched; i++)
		{
			if (_nextVideoUrlIndex >= _numUrlsFetched)
			{
				_nextVideoUrlIndex = 0;
			}
			if (_videoList[_nextVideoUrlIndex] != null && !string.IsNullOrEmpty(_videoList[_nextVideoUrlIndex]._url))
			{
				videoUrl = _videoList[_nextVideoUrlIndex]._url;
				isAdvert = _videoList[_nextVideoUrlIndex]._isAd;
				isCta = _videoList[_nextVideoUrlIndex]._isCta;
			}
			_nextVideoUrlIndex++;
			if (videoUrl != string.Empty)
			{
				return true;
			}
		}
		return false;
	}

	public void SetUp()
	{
		if (_pPlatformIsValid)
		{
			Debug.Log("Setting Up Cross Promo Manager, from init or with new app config change");
			LookUpCtaImageUrl();
			if (_stateCurrent == EState_CrossPromo.DECLARED)
			{
				_numUrlsFetched = 0;
				_stateCurrent = EState_CrossPromo.FETCHING_VIDEOS;
				_getDataRequest = new DownloadRequest();
				_getDataRequest.requestUrl = _dataGetRequest;
				_getDataRequest.onSuccessCallBack += OnGetApiRequestSuccess;
				_getDataRequest.onFailCallback += OnApiRequestFail;
				_getDataRequest.doCacheDownload = false;
				_getDataRequest.doCacheHeaderInfo = false;
				_getDataRequest.doLocalDownload = false;
				_getDataRequest.doLocalHeaderInfo = false;
				_getDataRequest._requestTimeout = 3f;
				_getDataRequest.doDebug = false;
				Log("MANAGER CROSS PROMO - SetUp - Requesting promotion data for this title");
				_getDataRequest.download();
			}
		}
	}

	public void PauseSetup(bool pause)
	{
		if (pause)
		{
			if (_stateCurrent == EState_CrossPromo.FETCHING_VIDEOS)
			{
				_stateCurrent = EState_CrossPromo.PAUSED;
			}
		}
		else if (_stateCurrent == EState_CrossPromo.PAUSED)
		{
			_stateCurrent = EState_CrossPromo.FETCHING_VIDEOS;
			FindVideoMetaData();
		}
	}

	public void OpenCtaUrl()
	{
		Application.OpenURL("https://c00.adobe.com/v3/1850813cf55379df22851ea66b55ef8f407d1fa9b813bc65d3ae0dc48ea2c607/start?a_dl=58aafef46058ecbcad08a392");
	}

	private void AssembleDataRequest()
	{
		Log("MANAGER CROSS PROMO - AssembleDataRequest - creating data request string.");
		_dataGetRequest = _urlVideoListAndroid;
		_dataGetRequest += ((!string.IsNullOrEmpty(_debugLang)) ? _debugLang : LEGOID.GetCorrectLanguageCode());
		Log("MANAGER CROSS PROMO - AssembleDataRequest - Video data url is: " + _dataGetRequest);
	}

	private void LookUpCtaImageUrl()
	{
		if (_pPlatformIsValid)
		{
			Log("MANAGER CROSS PROMO - LookUpCtaImageUrl - looking up CTA image URL");
			_urlCtaImage = string.Empty;
			if (LEGOSDK.ApplicationConfigurationService != null &&
			    LEGOSDK.ApplicationConfigurationService.ApplicationConfiguration != null &&
			    LEGOSDK.ApplicationConfigurationService.ApplicationConfiguration.Assets != null &&
			    LEGOSDK.ApplicationConfigurationService.ApplicationConfiguration.Assets.ContainsKey("appimage"))
			{
				_urlCtaImage = LEGOSDK.ApplicationConfigurationService.ApplicationConfiguration.Assets["appimage"];
				Log("MANAGER CROSS PROMO - LookUpCtaImageUrl - CTA image url is: " + _urlCtaImage);
			}
			else
			{
				Log("MANAGER CROSS PROMO - LookUpCtaImageUrl - No CTA image found in config.");
			}
		}
	}

	private void OnGetApiRequestSuccess(DownloadRequest request)
	{
		Log("MANAGER CROSS PROMO - OnGetDataSuccess - data Recieved");
		if (request == null || string.IsNullOrEmpty(request._pResponseString))
		{
			Log("MANAGER CROSS PROMO - OnGetApiRequestSuccess - data INVALID, shutting down service.");
			_stateCurrent = EState_CrossPromo.BROKENDOWN;
			return;
		}
		Log("MANAGER CROSS PROMO - OnGetApiRequestSuccess - data VALID: " + request._pResponseString);
		if (!FindVideoListUrl(request._pResponseString))
		{
			Log("MANAGER CROSS PROMO - OnGetApiRequestSuccess - couldnt find video list url, shutting down.");
			_stateCurrent = EState_CrossPromo.BROKENDOWN;
			return;
		}
		Log("MANAGER CROSS PROMO - OnGetApiRequestSuccess - now requesting video list form url: " + _urlVideoList);
		_getDataRequest = new DownloadRequest();
		_getDataRequest.requestUrl = _urlVideoList;
		_getDataRequest.onSuccessCallBack += OnGetVideoIdListSuccess;
		_getDataRequest.onFailCallback += OnGetVideoIdListFail;
		_getDataRequest.doCacheDownload = false;
		_getDataRequest.doCacheHeaderInfo = false;
		_getDataRequest.doLocalDownload = false;
		_getDataRequest.doLocalHeaderInfo = false;
		_getDataRequest._requestTimeout = 3f;
		_getDataRequest.doDebug = false;
		_getDataRequest.download();
	}

	private void OnApiRequestFail(DownloadRequest request)
	{
		Log("MANAGER CROSS PROMO - OnApiRequestFail - failed to get responce, shutting down service.");
		_stateCurrent = EState_CrossPromo.BROKENDOWN;
	}

	private bool FindVideoListUrl(string jsonDataAsString)
	{
		if (string.IsNullOrEmpty(jsonDataAsString))
		{
			Log("MANAGER CROSS PROMO - FindVideoListUrl - invalid data passed as argument - FAIL");
			_stateCurrent = EState_CrossPromo.BROKENDOWN;
			return false;
		}
		JsonData jsonData = JsonMapperLite.ToObject(new JsonReader(jsonDataAsString));
		if (jsonData == null)
		{
			Log("MANAGER CROSS PROMO - FindVideoListUrl - json creation unsuccessful - FAIL");
			_stateCurrent = EState_CrossPromo.BROKENDOWN;
			return false;
		}
		if (!jsonData.Contains("PromotionLists") || !jsonData["PromotionLists"].IsArray)
		{
			Log("MANAGER CROSS PROMO - FindVideoListUrl - promotion list is not found or is not an array - FAIL");
			_stateCurrent = EState_CrossPromo.BROKENDOWN;
			return false;
		}
		int num = 0;
		if (jsonData["PromotionLists"].Count > 0)
		{
			for (num = 0; num < jsonData["PromotionLists"].Count && (jsonData["PromotionLists"][num] == null || !jsonData["PromotionLists"][num].Contains("HeaderText") || !((string)jsonData["PromotionLists"][num]["HeaderText"] == "All Videos") || !jsonData["PromotionLists"][num].Contains("Orientation") || !((string)jsonData["PromotionLists"][num]["Orientation"] == "Horizontal")); num++)
			{
			}
			if (num >= jsonData["PromotionLists"].Count)
			{
				num = 0;
			}
			if (jsonData["PromotionLists"][num].Contains("Uri"))
			{
				_urlVideoList = (string)jsonData["PromotionLists"][num]["Uri"];
				return true;
			}
		}
		return false;
	}

	private void OnGetVideoIdListSuccess(DownloadRequest request)
	{
		Log("MANAGER CROSS PROMO - OnGetVideoIdListSuccess - data Recieved");
		if (request == null || string.IsNullOrEmpty(request._pResponseString))
		{
			Log("MANAGER CROSS PROMO - OnGetVideoIdListSuccess - data INVALID, shutting down service.");
			_stateCurrent = EState_CrossPromo.BROKENDOWN;
			return;
		}
		Log("MANAGER CROSS PROMO - OnGetVideoIdListSuccess - data VALID: " + request._pResponseString);
		_pageData = JsonMapperLite.ToObject(new JsonReader(request._pResponseString));
		if (_pageData == null)
		{
			Log("MANAGER CROSS PROMO - OnGetVideoIdListSuccess - json creation unsuccessful - FAIL");
			_stateCurrent = EState_CrossPromo.BROKENDOWN;
		}
		else
		{
			if (!_pageData.Contains("Content"))
			{
				return;
			}
			if (_pageData.Contains("PageNumber"))
			{
				_pageCurrent = (int)_pageData["PageNumber"];
				if (_pageData.Contains("TotalNumberOfPages"))
				{
					_pageTotal = (int)_pageData["TotalNumberOfPages"];
					Log("MANAGER CROSS PROMO - OnGetVideoIdListSuccess - Getting video ids from list page " + _pageCurrent + " of " + _pageTotal);
				}
			}
			CollectVideoIdsFromList(_pageData["Content"]);
		}
	}

	private void OnGetVideoIdListFail(DownloadRequest request)
	{
		_stateCurrent = EState_CrossPromo.BROKENDOWN;
		if (_videoList != null && _videoList.Count > 0)
		{
			Log("MANAGER CROSS PROMO - OnGetVideoIdListFail - failed to get responce, but we do have previous ids, so continuing with those.");
		}
		else
		{
			Log("MANAGER CROSS PROMO - OnGetVideoIdListFail - failed to get responce, shutting down service.");
		}
	}

	private void CollectVideoIdsFromList(JsonData videoList)
	{
		if (videoList == null || !videoList.IsArray)
		{
			return;
		}
		_numVideosInPage = 0;
		_nextVideoIdIndex = _videoList.Count - 1;
		for (int i = 0; i < videoList.Count; i++)
		{
			if (videoList[i] != null && videoList[i].Contains("Id"))
			{
				VideoData videoData = new VideoData();
				videoData._id = (string)videoList[i]["Id"];
				_videoList.Add(videoData);
				_numVideosInPage++;
			}
		}
		if (_numVideosInPage > 0)
		{
			FindVideoMetaData();
		}
	}

	private void FindVideoMetaData()
	{
		if (_stateCurrent == EState_CrossPromo.PAUSED)
		{
			return;
		}
		_nextVideoIdIndex++;
		if (_videoList == null)
		{
			Log("MANAGER CROSS PROMO - FindVideoMetaData - no video list - FAIL");
			_stateCurrent = EState_CrossPromo.BROKENDOWN;
			return;
		}
		if (_nextVideoIdIndex < _videoList.Count)
		{
			Log("MANAGER CROSS PROMO - Fetching meta for video id: " + (_nextVideoIdIndex + 1) + "/" + _videoList.Count + " Page " + _pageCurrent + "/" + _pageTotal);
			string text = "https://wwwsecure.lego.com/{lang}/mediaplayer/api/video.json/";
			text = text.Replace("{lang}", LEGOID.GetCorrectLanguageCodeCapitolRegion());
			text += _videoList[_nextVideoIdIndex]._id;
			_getDataRequest = new DownloadRequest();
			_getDataRequest.requestUrl = text;
			_getDataRequest.onSuccessCallBack += OnGetVideoMetaDataSuccess;
			_getDataRequest.onFailCallback += OnGetVideoMetaDataFail;
			_getDataRequest.doCacheDownload = false;
			_getDataRequest.doCacheHeaderInfo = false;
			_getDataRequest.doLocalDownload = false;
			_getDataRequest.doLocalHeaderInfo = false;
			_getDataRequest._requestTimeout = 3f;
			_getDataRequest.doDebug = false;
			_getDataRequest.download();
			return;
		}
		Log("MANAGER CROSS PROMO - FindVideoMetaData - Fetching video urls and flags from meta data finished for page: " + _pageCurrent + "/" + _pageTotal);
		int num = 0;
		int num2 = _videoList.Count - 1;
		while (num2 >= _videoList.Count - 1 - _numVideosInPage && num2 > 0)
		{
			if (string.IsNullOrEmpty(_videoList[num2]._url))
			{
				_videoList.RemoveAt(num2);
				num++;
			}
			num2--;
		}
		Log("MANAGER CROSS PROMO - FindVideoMetaData - Video Urls found: " + _numVideosInPage + ", and number invalid removed: " + num.ToString());
		int num3 = _numVideosInPage - num;
		int num4 = 0;
		int num5 = _videoList.Count - 1;
		while (num5 >= _videoList.Count - 1 - num3 && num5 > 0)
		{
			if (_videoList[num5] != null && _videoList[num5]._isAd)
			{
				num4++;
			}
			num5--;
		}
		Log("MANAGER CROSS PROMO - FindVideoMetaData - Ads found on page: " + num4.ToString() + " Out of: " + num3);
		ContinueToNextPage();
	}

	private void ContinueToNextPage()
	{
		if (_pageData != null && _pageData.Contains("NextPage"))
		{
			string text = (string)_pageData["NextPage"];
			if (!string.IsNullOrEmpty(text))
			{
				Log("MANAGER CROSS PROMO - OnGetVideoIdListSuccess - Another page url found, requesting now");
				_getDataRequest = new DownloadRequest();
				_getDataRequest.requestUrl = text;
				_getDataRequest.onSuccessCallBack += OnGetVideoIdListSuccess;
				_getDataRequest.onFailCallback += OnGetVideoIdListFail;
				_getDataRequest.doCacheDownload = false;
				_getDataRequest.doCacheHeaderInfo = false;
				_getDataRequest.doLocalDownload = false;
				_getDataRequest.doLocalHeaderInfo = false;
				_getDataRequest._requestTimeout = 3f;
				_getDataRequest.doDebug = false;
				_getDataRequest.download();
				return;
			}
		}
		FinishSetUp();
	}

	private void OnGetVideoMetaDataSuccess(DownloadRequest request)
	{
		Log("MANAGER CROSS PROMO - OnGetVideoMetaDataSuccess - data Recieved");
		if (request == null || string.IsNullOrEmpty(request._pResponseString))
		{
			Log("MANAGER CROSS PROMO - OnGetVideoMetaDataSuccess - data INVALID, shutting down service.");
			_stateCurrent = EState_CrossPromo.BROKENDOWN;
			return;
		}
		Log("MANAGER CROSS PROMO - OnGetVideoMetaDataSuccess - data VALID: " + request._pResponseString);
		JsonData jsonData = JsonMapperLite.ToObject(new JsonReader(request._pResponseString));
		if (jsonData == null)
		{
			Log("MANAGER CROSS PROMO - OnGetVideoMetaDataSuccess - json creation unsuccessful - FAIL");
			_stateCurrent = EState_CrossPromo.BROKENDOWN;
			return;
		}
		if (jsonData.Contains("VideoFormats"))
		{
			ExtractCorrectVideoUrlFromMetaData(jsonData["VideoFormats"]);
		}
		FindIfVideoIsAnAdvertisement();
	}

	private void OnGetVideoMetaDataFail(DownloadRequest request)
	{
		Log("MANAGER CROSS PROMO - OnGetVideoMetaDataFail - failed to get responce, shutting down service.");
		FindVideoMetaData();
	}

	private void ExtractCorrectVideoUrlFromMetaData(JsonData videoMetaData)
	{
		if (videoMetaData == null || !videoMetaData.IsArray)
		{
			return;
		}
		for (int i = 0; i < videoMetaData.Count; i++)
		{
			if (videoMetaData[i] != null && videoMetaData[i].Contains("Format") && (string)videoMetaData[i]["Format"] == "Mp4" && videoMetaData[i].Contains("Quality") && (string)videoMetaData[i]["Quality"] == "Low" && videoMetaData[i].Contains("Url") && _nextVideoIdIndex < _videoList.Count)
			{
				_videoList[_nextVideoIdIndex]._url = (string)videoMetaData[i]["Url"];
				_numUrlsFetched++;
			}
		}
	}

	private void FindIfVideoIsAnAdvertisement()
	{
		if (_videoList == null)
		{
			Log("MANAGER CROSS PROMO - FindIfVideoIsAnAdvertisement - no video list - FAIL");
			_stateCurrent = EState_CrossPromo.BROKENDOWN;
		}
		else if (_nextVideoIdIndex < _videoList.Count)
		{
			_getDataRequest = new DownloadRequest();
			_getDataRequest.doRemoteDownload = true;
			_getDataRequest.doLocalDownload = false;
			_getDataRequest.doLocalHeaderInfo = false;
			_getDataRequest.doCacheDownload = false;
			_getDataRequest.doCacheHeaderInfo = false;
			_getDataRequest.formHeaders.Add("x-api-key", "sA0DCIrasm30BtPNfst248WNfUH9StqNVrlCsu30");
			_getDataRequest.formHeaders.Add("Content-Type", "application/json");
			_getDataRequest.requestUrl = "https://services.slingshot.lego.com/api/v2/video/_search";
			_getDataRequest.onSuccessCallBack += OnGetDataByPostSuccess;
			_getDataRequest.onFailCallback += OnGetDataByPostFail;
			string postData = "{\"filter\":{\"term\":{\"id\":\"" + _videoList[_nextVideoIdIndex]._id + "_" + LEGOID.GetCorrectLanguageCodeCapitolRegion() + "\"}}}";
			_getDataRequest._postData = postData;
			_getDataRequest._requestTimeout = 3f;
			_getDataRequest.doDebug = false;
			_getDataRequest.download();
		}
		else
		{
			Log("MANAGER CROSS PROMO - FindIfVideoIsAnAdvertisement - ERROR, we should not get here. ", true);
		}
	}

	private void OnGetDataByPostSuccess(DownloadRequest request)
	{
		Log("MANAGER CROSS PROMO - OnGetDataByPostSuccess - data Recieved");
		if (request == null || string.IsNullOrEmpty(request._pResponseString))
		{
			Log("MANAGER CROSS PROMO - OnGetDataByPostSuccess - data INVALID, shutting down service.");
			_stateCurrent = EState_CrossPromo.BROKENDOWN;
			return;
		}
		Log("MANAGER CROSS PROMO - OnGetDataByPostSuccess - data VALID: " + request._pResponseString);
		JsonData jsonData = JsonMapperLite.ToObject(new JsonReader(request._pResponseString));
		if (jsonData == null)
		{
			Log("MANAGER CROSS PROMO - OnGetDataByPostSuccess - json creation unsuccessful - FAIL");
			_stateCurrent = EState_CrossPromo.BROKENDOWN;
			return;
		}
		if (jsonData.Contains("hits") && jsonData["hits"].Contains("hits"))
		{
			ExtractAdvertisementFlagFromMetaData(jsonData["hits"]["hits"]);
			ExtractCtaFlagFromMetaData(jsonData["hits"]["hits"]);
		}
		FindVideoMetaData();
	}

	private void OnGetDataByPostFail(DownloadRequest request)
	{
		Log("MANAGER CROSS PROMO - OnGetDataByPostFail - failed to get responce, shutting down service.");
		FindVideoMetaData();
	}

	private void ExtractAdvertisementFlagFromMetaData(JsonData videoMetaData)
	{
		if (videoMetaData == null || !videoMetaData.IsArray || videoMetaData.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < videoMetaData.Count; i++)
		{
			if (videoMetaData[i] == null)
			{
				continue;
			}
			JsonData jsonData = videoMetaData[i];
			if (!jsonData.Contains("_source"))
			{
				continue;
			}
			jsonData = jsonData["_source"];
			if (!jsonData.Contains("videoType"))
			{
				continue;
			}
			jsonData = jsonData["videoType"];
			if (!jsonData.IsArray || jsonData.Count <= 0)
			{
				continue;
			}
			for (int j = 0; j < jsonData.Count; j++)
			{
				if (jsonData[j] == null)
				{
					continue;
				}
				JsonData jsonData2 = jsonData[j];
				if (jsonData2.Contains("name"))
				{
					jsonData2 = jsonData2["name"];
					_videoList[_nextVideoIdIndex]._isAd = "TVC" == (string)jsonData2;
					Log("MANAGER CROSS PROMO - ExtractAdvertisementFlagFromMetaData - Video with index " + _nextVideoIdIndex + " Has video Type of: " + (string)jsonData2 + " So isAd = " + _videoList[_nextVideoIdIndex]._isAd);
					if (_videoList[_nextVideoIdIndex]._isAd)
					{
						return;
					}
				}
			}
		}
	}

	private void ExtractCtaFlagFromMetaData(JsonData videoMetaData)
	{
		if (videoMetaData == null || !videoMetaData.IsArray || videoMetaData.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < videoMetaData.Count; i++)
		{
			if (videoMetaData[i] == null)
			{
				continue;
			}
			JsonData jsonData = videoMetaData[i];
			if (!jsonData.Contains("_source"))
			{
				continue;
			}
			jsonData = jsonData["_source"];
			if (!jsonData.Contains("classification"))
			{
				continue;
			}
			jsonData = jsonData["classification"];
			if (!jsonData.IsArray || jsonData.Count <= 0)
			{
				continue;
			}
			for (int j = 0; j < jsonData.Count; j++)
			{
				if (jsonData[j] == null)
				{
					continue;
				}
				JsonData jsonData2 = jsonData[j];
				if (jsonData2.Contains("name"))
				{
					jsonData2 = jsonData2["name"];
					_videoList[_nextVideoIdIndex]._isCta = "App" == (string)jsonData2;
					Log("MANAGER CROSS PROMO - ExtractCtaFlagFromMetaData - Video with index " + _nextVideoIdIndex + " Has classification of: " + (string)jsonData2 + " So isCta = " + _videoList[_nextVideoIdIndex]._isCta);
					if (_videoList[_nextVideoIdIndex]._isCta)
					{
						return;
					}
				}
			}
		}
	}

	private void FinishSetUp()
	{
		if (_debugLogging)
		{
			Log("MANAGER CROSS PROMO - FINISHED AND READY FOR USE");
			Log("MANAGER CROSS PROMO - Video Id List complete, number of videos: " + _videoList.Count);
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < _videoList.Count; i++)
			{
				if (_videoList[i] != null)
				{
					if (_videoList[i]._isAd)
					{
						num++;
					}
					if (_videoList[i]._isCta)
					{
						num2++;
					}
				}
			}
			Log("MANAGER CROSS PROMO - Ads found: " + num.ToString() + " Out of: " + _videoList.Count);
			Log("MANAGER CROSS PROMO - Ctas found: " + num2.ToString() + " Out of: " + _videoList.Count);
		}
		_stateCurrent = EState_CrossPromo.COMPLETE;
	}

	private void Log(string message, bool error = false)
	{
		if (_debugLogging)
		{
			Debug.Log(message);
		}
	}
}
