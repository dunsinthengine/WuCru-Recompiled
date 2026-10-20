using UnityEngine;
using UnityEngine.UI;

namespace Cape.Localization
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Text))]
	[AddComponentMenu("Cape/Localization/Localize")]
	public class Localize : MonoBehaviour
	{
		private Text label;

		private bool _started;

		[SerializeField]
		private string _key;

		[SerializeField]
		private bool _useCustomFont = true;

		public string Key
		{
			get
			{
				return _key;
			}
		}

		public string value
		{
			set
			{
				if (Localization.Instance.UpperCaseText)
				{
					value = value.ToUpper();
				}
				if (label != null)
				{
					label.text = value;
				}
			}
		}

		public bool HasValidKey()
		{
			return string.IsNullOrEmpty(_key);
		}

		public void SetKey(string key)
		{
			if (!string.IsNullOrEmpty(key))
			{
				_key = key;
				value = Localization.Instance.Get(_key);
			}
			else
			{
				Debug.LogWarning("Ignoring attempt to replace existing key '" + _key + "' with null or empty string");
			}
		}

		public void SetValue(string text)
		{
			value = text;
			_key = null;
		}

		private void Awake()
		{
			Localization.Instance.LocalizationChanged += OnLocalize;
			label = GetComponent<Text>();
		}

		private void Start()
		{
			_started = true;
			OnLocalize();
		}

		private void OnDestroy()
		{
			Localization.Instance.LocalizationChanged -= OnLocalize;
			label = null;
		}

		private void OnEnable()
		{
			if (_started)
			{
				OnLocalize();
			}
		}

		public void OnLocalize()
		{
			if (_useCustomFont)
			{
				label.font = Localization.Instance.Font;
			}
			if (string.IsNullOrEmpty(_key))
			{
				_key = label.text;
			}
			else
			{
				value = Localization.Instance.Get(_key);
			}
		}
	}
}
