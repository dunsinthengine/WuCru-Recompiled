using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Tools
{
	internal class UIElementCleanup : MonoBehaviour
	{
		private void OnDestroy()
		{
			RawImage component = GetComponent<RawImage>();
			if (component != null)
			{
				component.texture = null;
			}
			Image component2 = GetComponent<Image>();
			if (component2 != null)
			{
				component2.sprite = null;
			}
		}
	}
}
