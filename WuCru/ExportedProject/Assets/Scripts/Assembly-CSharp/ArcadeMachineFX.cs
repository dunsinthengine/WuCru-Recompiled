using UnityEngine;

public class ArcadeMachineFX : MonoBehaviour
{
	public float _targ;

	private Material _mat;

	private float _fadeOff = 1f;

	private bool _isFadeOff;

	private bool _hasDoneInit;

	private bool _areFXEnabled;

	private void Awake()
	{
		Init();
	}

	private void Init()
	{
		if (_hasDoneInit)
		{
			return;
		}
		MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
		int num = componentsInChildren.Length;
		for (int i = 0; i < num; i++)
		{
			if (i == 0)
			{
				_mat = new Material(componentsInChildren[i].sharedMaterial);
			}
			componentsInChildren[i].sharedMaterial = _mat;
		}
		_hasDoneInit = true;
	}

	private void Update()
	{
		if (_areFXEnabled)
		{
			Color color = _mat.GetColor("_TintColor");
			color.a = _targ * _fadeOff;
			_mat.SetColor("_TintColor", color);
			_fadeOff = Mathf.MoveTowards(_fadeOff, (!_isFadeOff) ? 1f : 0f, Time.deltaTime * 8f);
		}
	}

	public void FadeOff()
	{
		_isFadeOff = true;
	}

	public void FadeIn()
	{
		_isFadeOff = false;
	}

	public void ChangeEnabledState(bool doEnableFX)
	{
		Init();
		_areFXEnabled = doEnableFX;
		if (!_areFXEnabled)
		{
			Color color = _mat.GetColor("_TintColor");
			color.a = 0f;
			_mat.SetColor("_TintColor", color);
		}
	}
}
