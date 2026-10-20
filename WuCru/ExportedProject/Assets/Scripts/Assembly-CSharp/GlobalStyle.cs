using UnityEngine;

public class GlobalStyle : MonoBehaviour
{
	private static GlobalStyle _instance;

	public Color _KaiNameTextColorMain;

	public Color _JayNameTextColorMain;

	public Color _ColeNameTextColorMain;

	public Color _ZaneNameTextColorMain;

	public Color _NyaNameTextColorMain;

	public Color _LloydNameTextColorMain;

	public static GlobalStyle _pInstance
	{
		get
		{
			return _instance;
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		_instance = this;
	}

	public Color GetNameTextMainColor(MinifigType miniType)
	{
		switch (miniType)
		{
		case MinifigType.Cole:
			return _ColeNameTextColorMain;
		case MinifigType.Jay:
			return _JayNameTextColorMain;
		case MinifigType.Kai:
			return _KaiNameTextColorMain;
		case MinifigType.Lloyd:
			return _LloydNameTextColorMain;
		case MinifigType.Nya:
			return _NyaNameTextColorMain;
		case MinifigType.Zane:
			return _ZaneNameTextColorMain;
		default:
			return Color.white;
		}
	}
}
