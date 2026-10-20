using UnityEngine;
using UnityEngine.UI;

public class TraversalTeaserGUI : MonoBehaviour
{
	public delegate void OnCloseDelegate();

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
}
