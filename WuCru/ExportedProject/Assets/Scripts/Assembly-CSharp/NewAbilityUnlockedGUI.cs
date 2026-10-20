using Cape.Localization;
using UnityEngine;
using UnityEngine.UI;

public class NewAbilityUnlockedGUI : MonoBehaviour
{
	public delegate void OnCloseDelegate();

	public RawImage imageAbility;

	public Text textAbilityName;

	public Button ButtonOK;

	public OnCloseDelegate OnClose;

	private void Start()
	{
		ButtonOK.onClick.AddListener(() =>
		{
			Object.Destroy(base.gameObject);
			if (OnClose != null)
			{
				OnClose();
			}
		});
	}

	public void SetAbility(AbilityType type)
	{
		Localize component = textAbilityName.GetComponent<Localize>();
		if (component == null)
		{
			Debug.LogWarning("Localization key update failed: No Localize component attached to the Text object");
		}
		else
		{
			component.SetKey(type.ToString());
		}
	}
}
