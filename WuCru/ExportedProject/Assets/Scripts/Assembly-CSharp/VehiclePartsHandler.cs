using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehiclePartsHandler : MonoBehaviour
{
	public class MeshRendererData
	{
		public MeshRenderer _partRenderer;

		private Material[] _cachedMaterials;

		public void CacheMaterials()
		{
			if (_partRenderer.material != null)
			{
				_cachedMaterials = new Material[1];
				_cachedMaterials[0] = _partRenderer.material;
			}
			else if (_partRenderer.sharedMaterial != null)
			{
				_cachedMaterials = new Material[1];
				_cachedMaterials[0] = _partRenderer.sharedMaterial;
			}
			else if (_partRenderer.materials != null && _partRenderer.materials.Length > 0)
			{
				_cachedMaterials = new Material[_partRenderer.materials.Length];
				for (int i = 0; i < _partRenderer.materials.Length; i++)
				{
					_cachedMaterials[i] = _partRenderer.materials[i];
				}
			}
			else if (_partRenderer.sharedMaterials != null && _partRenderer.sharedMaterials.Length > 0)
			{
				_cachedMaterials = new Material[_partRenderer.sharedMaterials.Length];
				for (int j = 0; j < _partRenderer.sharedMaterials.Length; j++)
				{
					_cachedMaterials[j] = _partRenderer.sharedMaterials[j];
				}
			}
		}

		public void LoadMaterialsFromCache()
		{
			if (_cachedMaterials == null || _cachedMaterials.Length == 0)
			{
				return;
			}
			if (_partRenderer.material != null)
			{
				_partRenderer.material = _cachedMaterials[0];
			}
			else if (_partRenderer.sharedMaterial != null)
			{
				_partRenderer.sharedMaterial = _cachedMaterials[0];
			}
			else if (_partRenderer.materials != null && _partRenderer.materials.Length == _cachedMaterials.Length)
			{
				for (int i = 0; i < _partRenderer.materials.Length; i++)
				{
					_partRenderer.materials[i] = _cachedMaterials[i];
				}
			}
			else if (_partRenderer.sharedMaterials != null && _partRenderer.sharedMaterials.Length == _cachedMaterials.Length)
			{
				for (int j = 0; j < _partRenderer.sharedMaterials.Length; j++)
				{
					_partRenderer.sharedMaterials[j] = _cachedMaterials[j];
				}
			}
		}
	}

	private const float FLASH_DURATION = 0.5f;

	public MeshRenderer[] p1;

	public MeshRenderer[] p2;

	public MeshRenderer[] p3;

	private MeshRendererData[] cacheP1;

	private MeshRendererData[] cacheP2;

	private MeshRendererData[] cacheP3;

	private bool _flashInProgress;

	private List<int> _flashQueue = new List<int>();

	public MeshRenderer[] GetParts(int pindex)
	{
		switch (pindex)
		{
		case 0:
			return p1;
		case 1:
			return p2;
		case 2:
			return p3;
		default:
			return null;
		}
	}

	public void OverrideMaterial(int pIndex, Material newMat)
	{
		MeshRenderer[] parts = GetParts(pIndex);
		for (int i = 0; i < parts.Length; i++)
		{
			if (parts[i].material != null)
			{
				parts[i].material = newMat;
			}
			if (parts[i].sharedMaterial != null)
			{
				parts[i].sharedMaterial = newMat;
			}
			if (parts[i].materials != null && parts[i].materials.Length > 0)
			{
				for (int j = 0; j < parts[i].materials.Length; j++)
				{
					parts[i].materials[j] = newMat;
				}
			}
			if (parts[i].sharedMaterials != null && parts[i].sharedMaterials.Length > 0)
			{
				for (int k = 0; k < parts[i].sharedMaterials.Length; k++)
				{
					parts[i].sharedMaterials[k] = newMat;
				}
			}
		}
	}

	public void Flash(int pIndex, Material sourceMat)
	{
		CacheMaterials(pIndex);
		if (_flashInProgress)
		{
			OverrideMaterial(pIndex, sourceMat);
			_flashQueue.Add(pIndex);
		}
		else
		{
			_flashInProgress = true;
			StartCoroutine(FlashParts(pIndex, sourceMat));
		}
	}

	private void SetMaterialWhiteColour(int pIndex, float decimalProgress)
	{
		Color color = Color.Lerp(Color.black, Color.white, decimalProgress);
		Color color2 = Color.Lerp(new Color(0.5f, 0.5f, 0.5f), Color.white, decimalProgress);
		MeshRenderer[] parts = GetParts(pIndex);
		for (int i = 0; i < parts.Length; i++)
		{
			if (parts[i].material != null)
			{
				parts[i].material.color = color;
				parts[i].material.SetColor("_SpecularColor", color2);
				parts[i].material.SetFloat("_Shininess", 1f - decimalProgress);
			}
			if (parts[i].sharedMaterial != null)
			{
				parts[i].sharedMaterial.color = color;
				parts[i].sharedMaterial.SetColor("_SpecularColor", color2);
				parts[i].sharedMaterial.SetFloat("_Shininess", 1f - decimalProgress);
			}
			if (parts[i].materials != null && parts[i].materials.Length > 0)
			{
				for (int j = 0; j < parts[i].materials.Length; j++)
				{
					parts[i].materials[j].color = color;
					parts[i].materials[j].SetColor("_SpecularColor", color2);
					parts[i].materials[j].SetFloat("_Shininess", 1f - decimalProgress);
				}
			}
			if (parts[i].sharedMaterials != null && parts[i].sharedMaterials.Length > 0)
			{
				for (int k = 0; k < parts[i].sharedMaterials.Length; k++)
				{
					parts[i].sharedMaterials[k].color = color;
					parts[i].sharedMaterials[k].SetColor("_SpecularColor", color2);
					parts[i].sharedMaterials[k].SetFloat("_Shininess", 1f - decimalProgress);
				}
			}
		}
	}

	private void CacheMaterials(int pIndex)
	{
		MeshRendererData[] array = null;
		switch (pIndex)
		{
		case 0:
			array = cacheP1;
			break;
		case 1:
			array = cacheP2;
			break;
		case 2:
			array = cacheP3;
			break;
		}
		if (array == null)
		{
			MeshRenderer[] parts = GetParts(pIndex);
			array = new MeshRendererData[parts.Length];
			for (int i = 0; i < parts.Length; i++)
			{
				array[i] = new MeshRendererData();
				array[i]._partRenderer = parts[i];
				array[i].CacheMaterials();
			}
			switch (pIndex)
			{
			case 0:
				cacheP1 = array;
				break;
			case 1:
				cacheP2 = array;
				break;
			case 2:
				cacheP3 = array;
				break;
			}
		}
	}

	private void LoadMaterialsFromCache(int pIndex)
	{
		MeshRendererData[] array = null;
		switch (pIndex)
		{
		case 0:
			array = cacheP1;
			break;
		case 1:
			array = cacheP2;
			break;
		case 2:
			array = cacheP3;
			break;
		}
		if (array != null)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i].LoadMaterialsFromCache();
			}
		}
	}

	private IEnumerator FlashParts(int pIndex, Material sourceMat)
	{
		Material tempMat = new Material(sourceMat);
		OverrideMaterial(pIndex, tempMat);
		float startTime = RealTime.time;
		float flashDecimal = 0f;
		while (flashDecimal < 1f)
		{
			flashDecimal = Mathf.Clamp01((RealTime.time - startTime) / 0.5f);
			SetMaterialWhiteColour(pIndex, flashDecimal);
			yield return new WaitForEndOfFrame();
		}
		LoadMaterialsFromCache(pIndex);
		_flashInProgress = false;
		if (_flashQueue.Count > 0)
		{
			int pIndex2 = _flashQueue[0];
			_flashQueue.RemoveAt(0);
			Flash(pIndex2, sourceMat);
		}
	}
}
