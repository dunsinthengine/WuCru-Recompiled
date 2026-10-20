using System.Collections;
using UnityEngine;

public class FidelityFacade : InitialisationObject
{
	public static FidelityFacade Instance;

	private Fidelity _fidelity = Fidelity.Normal;

	public FidelityQualitySettings lowQualitySettings;

	public FidelityQualitySettings normalQualitySettings;

	public FidelityQualitySettings highQualitySettings;

	private FidelityQualitySettings[] _qualitySettings;

	public Fidelity forceMode = Fidelity.None;

	private EShadowQuality _shadows;

	private string _expPath;

	public Fidelity fidelity
	{
		get
		{
			return _fidelity;
		}
	}

	public EShadowQuality shadows
	{
		get
		{
			return _shadows;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		Instance = this;
		Facades<FidelityFacade>.Register(this);
		Object.DontDestroyOnLoad(base.gameObject);
		_qualitySettings = new FidelityQualitySettings[3];
		_qualitySettings[0] = highQualitySettings;
		_qualitySettings[1] = normalQualitySettings;
		_qualitySettings[2] = lowQualitySettings;
	}

	public override void startInitialising()
	{
		_currentState = InitialisationState.INITIALISING;
		init();
	}

	private void init()
	{
		_fidelity = Fidelity.Normal;
		if (SystemInfo.systemMemorySize < 1024)
		{
			_fidelity = Fidelity.Low;
		}
		else if (SystemInfo.systemMemorySize < 1536)
		{
			_fidelity = Fidelity.Normal;
		}
		else
		{
			_fidelity = Fidelity.High;
		}
		_qualitySettings[(int)_fidelity].Select();
		_shadows = _qualitySettings[(int)_fidelity]._shadows;
		Debug.LogWarning("!!! --- FIDELITY SET TO [" + _fidelity.ToString() + "] --- !!!");
		CheckObb();
	}

	private void initComplete()
	{
		_currentState = InitialisationState.FINISHED;
	}

	private void CheckObb()
	{
		if (!GooglePlayDownloader.RunningOnAndroid())
		{
			initComplete();
			return;
		}
		_expPath = GooglePlayDownloader.GetExpansionFilePath();
		if (_expPath != null)
		{
			Debug.Log("Google Play Downloader: expansion path = " + _expPath);
			string mainOBBPath = GooglePlayDownloader.GetMainOBBPath(_expPath);
			Debug.Log((mainOBBPath != null) ? mainOBBPath.Substring(_expPath.Length) : " NOT AVAILABLE");
			if (mainOBBPath == null)
			{
				Debug.Log("Fetching OBB ...");
				GooglePlayDownloader.FetchOBB();
				StartCoroutine(PostFetchOBB());
			}
			else
			{
				initComplete();
			}
		}
		else
		{
			Debug.LogWarning("Google Play Downloader: no expansion file path found");
			initComplete();
		}
	}

	protected IEnumerator PostFetchOBB()
	{
		string mainPath;
		do
		{
			yield return new WaitForSeconds(1f);
			mainPath = GooglePlayDownloader.GetMainOBBPath(_expPath);
			Debug.Log("Waiting for OBB ... " + mainPath);
		}
		while (mainPath == null);
		Debug.Log("Fetching OBB done, sending launch signals.");
		initComplete();
	}
}
