using System.Collections.Generic;
using UnityEngine;

public class MovieTextureManager
{
	public static GameObject cameraHolder;

	public static int numCameras;

	private static Dictionary<string, MovieTextureInfo> movieTextures;

	public static MovieTextureInfo getMovie(string movieId, Vector2 frameSize, int frames, bool loop)
	{
		if (movieTextures == null)
		{
			movieTextures = new Dictionary<string, MovieTextureInfo>();
		}
		if (cameraHolder == null)
		{
			cameraHolder = new GameObject();
			cameraHolder.transform.position = new Vector3(0f, 1000f, 0f);
			cameraHolder.name = "MovieTexture Camera Holder";
			Object.DontDestroyOnLoad(cameraHolder);
		}
		if (movieTextures.ContainsKey(movieId))
		{
			return movieTextures[movieId];
		}
		return null;
	}
}
