using UnityEngine;

public class StickerbookElement : MonoBehaviour
{
	public ScreenStickerBook _stickerbookScreen;

	private ECOLLECTABLE_CARD _cardID;

	private UITexture _texture;

	public ECOLLECTABLE_CARD _pCardID
	{
		get
		{
			return _cardID;
		}
		set
		{
			_cardID = value;
		}
	}

	public UITexture _pTexture
	{
		get
		{
			if (_texture == null)
			{
				_texture = GetComponent<UITexture>();
			}
			return _texture;
		}
	}

	private void OnClick()
	{
		if (!(_pTexture.mainTexture == null))
		{
			_stickerbookScreen.PerformZoom(_pTexture);
		}
	}

	private void OnDrag(Vector2 delta)
	{
		_stickerbookScreen.OnDrag(delta);
	}
}
