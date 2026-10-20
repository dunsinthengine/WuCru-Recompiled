using UnityEngine;

public class AmuzoNGUILocalize : MonoBehaviour
{
	public string key;

	private UILabel _uiLabel;

	public UILabel _pUILabel
	{
		get
		{
			if (_uiLabel == null)
			{
				_uiLabel = GetComponent<UILabel>();
			}
			return _uiLabel;
		}
	}

	private void OnEnable()
	{
		if (Application.isPlaying)
		{
			AssignTextFromKey();
		}
	}

	public void AssignTextFromKey()
	{
		if (LocalisationFacade.Instance != null)
		{
			string text = LocalisationFacade.Instance.GetString(key);
			if (text == string.Empty || text == null)
			{
				_pUILabel.text = key;
			}
			else
			{
				_pUILabel.text = text;
			}
		}
	}
}
