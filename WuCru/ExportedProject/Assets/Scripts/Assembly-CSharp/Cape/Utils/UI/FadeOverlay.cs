using System;
using UnityEngine;
using UnityEngine.UI;

namespace Cape.Utils.UI
{
	public class FadeOverlay : MonoBehaviour
	{
		[SerializeField]
		private Image _overlay;

		[SerializeField]
		private Easing.Function _fadeInEasing;

		[SerializeField]
		private float _fadeInDuration = 1f;

		[SerializeField]
		private float _fadeInDelay;

		[SerializeField]
		private Easing.Function _fadeOutEasing;

		[SerializeField]
		private float _fadeOutDuration = 1f;

		[SerializeField]
		private float _fadeOutDelay;

		private bool _fadeIn;

		private bool _fadeOut;

		private float _counter;

		public event EventHandler<EventArgs> FadeInDone;

		public event EventHandler<EventArgs> FadeOutDone;

		private void Awake()
		{
			_fadeIn = false;
			_fadeOut = false;
			_counter = 0f;
			SetAlpha(1f);
			_overlay.gameObject.SetActive(true);
		}

		private void Update()
		{
			if (_fadeIn)
			{
				if (_counter <= _fadeInDuration + _fadeInDelay)
				{
					_counter += Time.deltaTime;
					if (_counter >= _fadeInDelay)
					{
						float num = (_counter - _fadeInDelay) / _fadeInDuration;
						num = 1f - num;
						float alpha = Easing.Interpolate(num, _fadeInEasing);
						SetAlpha(alpha);
					}
				}
				else
				{
					_fadeIn = false;
					_counter = 0f;
					if (FadeInDone != null)
					{
						FadeInDone(this, null);
					}
				}
			}
			else
			{
				if (!_fadeOut)
				{
					return;
				}
				if (_counter <= _fadeOutDuration + _fadeOutDelay)
				{
					_counter += Time.deltaTime;
					if (_counter >= _fadeOutDelay)
					{
						float p = (_counter - _fadeOutDelay) / _fadeOutDuration;
						float alpha2 = Easing.Interpolate(p, _fadeOutEasing);
						SetAlpha(alpha2);
					}
				}
				else
				{
					_fadeOut = false;
					_counter = 0f;
					if (FadeOutDone != null)
					{
						FadeOutDone(this, null);
					}
				}
			}
		}

		public void StartFadeIn()
		{
			_fadeOut = false;
			_fadeIn = true;
		}

		public void StartFadeOut()
		{
			_fadeIn = false;
			_fadeOut = true;
		}

		public bool IsFadingIn()
		{
			return _fadeIn;
		}

		public bool IsFadingOut()
		{
			return _fadeOut;
		}

		private void SetAlpha(float value)
		{
			Color color = _overlay.color;
			color.a = value;
			_overlay.color = color;
		}
	}
}
