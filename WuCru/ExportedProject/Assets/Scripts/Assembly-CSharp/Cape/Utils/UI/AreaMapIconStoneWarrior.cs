using System;
using System.Collections;
using DentedPixel.LTEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Cape.Utils.UI
{
	public class AreaMapIconStoneWarrior : MonoBehaviour
	{
		[SerializeField]
		private Image _icon;

		[SerializeField]
		private float _iconBounceDuration;

		[SerializeField]
		private float _iconBounceFrequency;

		[SerializeField]
		private float _iconBounceAmount;

		[SerializeField]
		private float _animatorDelay;

		private float _counter;

		private float _tweenDuration;

		[SerializeField]
		private Animator _animator;

		private string _sound;

		private bool _initialized;

		private bool _playingSound;

		public int Seed { get; set; }

		private void Awake()
		{
			_counter = 0f;
			_playingSound = false;
			_initialized = false;
			if (GameController.Instance.PlayerProfile.HasSeenBriefing(PlayerBriefingType.StoneWarriorIcon))
			{
				_animator.enabled = true;
				return;
			}
			_icon.transform.localScale = Vector3.zero;
			LeanTweenVisual componentInChildren = GetComponentInChildren<LeanTweenVisual>();
			_tweenDuration = LeanTweenHelper.GetDuration(componentInChildren);
			componentInChildren.enabled = true;
		}

		private void Start()
		{
			if (GameController.Instance.PlayerProfile.HasSeenBriefing(PlayerBriefingType.StoneWarriorIcon))
			{
				AudioPlayer.PlaySound("ui/button/sword/shing");
			}
			else
			{
				_sound = GameController.Instance.LevelExtensionController.GetConfirmPlaySoundName(Seed);
			}
		}

		private void Update()
		{
			if (GameController.Instance.PlayerProfile.HasSeenBriefing(PlayerBriefingType.StoneWarriorIcon) || _initialized)
			{
				return;
			}
			if (_counter > _tweenDuration)
			{
				GameController.Instance.PlayerProfile.BriefingComplete(PlayerBriefingType.StoneWarriorIcon);
				GameController.Instance.PlayerProfile.Save();
				StartCoroutine(DoButtonBounce());
				_initialized = true;
				return;
			}
			if (_counter > _tweenDuration - 0.18f && !_playingSound)
			{
				_playingSound = true;
				AudioPlayer.PlaySound(_sound);
			}
			_counter += Time.deltaTime;
		}

		private IEnumerator DoButtonBounce()
		{
			float counter = 0f;
			Vector3 scale = Vector3.zero;
			while (counter < _iconBounceDuration)
			{
				counter += Time.deltaTime;
				float damping = 1f - counter / _iconBounceDuration;
				float bounce = _iconBounceAmount * Mathf.Sin((float)Math.PI * 2f * _iconBounceFrequency * counter) * damping;
				scale.x = 1f + bounce;
				scale.y = scale.x;
				scale.z = scale.x;
				_icon.transform.localScale = scale;
				yield return null;
			}
			yield return new WaitForSeconds(_animatorDelay);
			_animator.enabled = true;
		}
	}
}
