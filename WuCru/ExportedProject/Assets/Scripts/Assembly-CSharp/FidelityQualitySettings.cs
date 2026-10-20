using System;
using UnityEngine;

[Serializable]
public class FidelityQualitySettings
{
	public AnisotropicFiltering _anisotropicFiltering;

	public int _antiAliasing;

	public int _vSyncCount;

	public int _maxQueuedFrames;

	public EShadowQuality _shadows;

	public int _qualityLevel;

	public float _fixedDeltaTime = -1f;

	public void Select()
	{
		QualitySettings.anisotropicFiltering = _anisotropicFiltering;
		QualitySettings.antiAliasing = _antiAliasing;
		QualitySettings.vSyncCount = _vSyncCount;
		QualitySettings.maxQueuedFrames = _maxQueuedFrames;
		QualitySettings.SetQualityLevel(_qualityLevel);
		if (_fixedDeltaTime > 0f)
		{
			Time.fixedDeltaTime = _fixedDeltaTime;
		}
	}
}
