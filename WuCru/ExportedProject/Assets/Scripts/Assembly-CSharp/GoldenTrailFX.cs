using UnityEngine;

public class GoldenTrailFX : MonoBehaviour
{
	public UISprite[] _sprites;

	public UITexture[] _textures;

	public Vector3 minScaleSize;

	public Vector3 scaleSize;

	public float speed = 1f;

	public float spacing = 0.1f;

	private bool _enabled;

	private void Awake()
	{
		SetVisible(false);
	}

	public void SetVisible(bool visible)
	{
		if (_sprites != null)
		{
			int num = _sprites.Length;
			for (int i = 0; i < num; i++)
			{
				_sprites[i].enabled = visible;
			}
		}
		if (_textures != null)
		{
			int num2 = _textures.Length;
			for (int j = 0; j < num2; j++)
			{
				_textures[j].enabled = visible;
			}
		}
		_enabled = visible;
	}

	private void Update()
	{
		if (!_enabled)
		{
			return;
		}
		int num = ((_sprites != null) ? _sprites.Length : 0);
		for (int i = 0; i < num; i++)
		{
			float num2 = (Time.time + (float)i * spacing) * speed;
			float num3 = Frac(num2);
			if (num2 % 2f < 1f)
			{
				num3 = 0f;
			}
			_sprites[i].transform.localScale = Vector3.Lerp(minScaleSize, scaleSize, num3);
			_sprites[i].alpha = 1f - num3;
		}
		int num4 = ((_textures != null) ? _textures.Length : 0);
		for (int j = 0; j < num4; j++)
		{
			float num5 = (Time.time + (float)j * spacing) * speed;
			float num6 = Frac(num5);
			if (num5 % 2f < 1f)
			{
				num6 = 0f;
			}
			_textures[j].transform.localScale = Vector3.Lerp(minScaleSize, scaleSize, num6);
			_textures[j].alpha = 1f - num6;
		}
	}

	private float Frac(float v)
	{
		return v - Mathf.Floor(v);
	}
}
