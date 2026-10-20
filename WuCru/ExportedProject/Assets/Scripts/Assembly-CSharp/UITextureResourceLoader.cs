using UnityEngine;

public class UITextureResourceLoader : MonoBehaviour
{
	public UITexture _texture;

	public string _absoluteTextureLocation;

	public string _fullResTextureLocation;

	public string _lowResTextureLocation;

	private bool _isFirstTimeRefresh = true;

	public static bool _pIsUseLowFidelityTextures { get; set; }

	public void AssignTexture()
	{
		if (!(_texture == null) && !(_texture.mainTexture != null) && (!string.IsNullOrEmpty(_fullResTextureLocation) || !string.IsNullOrEmpty(_lowResTextureLocation)))
		{
			if (_pIsUseLowFidelityTextures && !string.IsNullOrEmpty(_lowResTextureLocation))
			{
				_texture.mainTexture = Resources.Load<Texture>(_lowResTextureLocation);
			}
			if (_texture.mainTexture == null && !string.IsNullOrEmpty(_fullResTextureLocation))
			{
				_texture.mainTexture = Resources.Load<Texture>(_fullResTextureLocation);
			}
			if (_isFirstTimeRefresh)
			{
				_isFirstTimeRefresh = false;
				return;
			}
			_texture.Update();
			_texture.UpdateAnchors();
		}
	}

	public void UnAssignTexture()
	{
		if (!(_texture == null) && !(_texture.mainTexture == null))
		{
			_texture.mainTexture = null;
		}
	}
}
