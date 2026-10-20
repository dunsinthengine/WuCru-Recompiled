using System;
using UnityEngine;

public class MovieTextureInfo
{
	public delegate void ReturnFunction();

	public float duration;

	public float currentMovieTime;

	public float stopTimeindex = -1f;

	public Action<AudioClip> _playMusicCallback;

	public int frames;

	private Vector2 offset;

	private Vector2 frameSize;

	private Texture2D staticMask;

	private ReturnFunction onMovieComplete;

	private GameObject cameraObject;

	private GameObject moviePlane;

	private bool isPlaying;

	public float progressDecimal
	{
		get
		{
			return currentMovieTime / duration;
		}
	}

	public bool IsPlaying
	{
		get
		{
			return isPlaying;
		}
	}

	public void playMovie()
	{
		playMovie(null, 0);
	}

	public void playMovie(ReturnFunction onMovieComplete)
	{
		playMovie(onMovieComplete, 0);
	}

	public void playMovie(int delayFrames)
	{
		playMovie(null, delayFrames);
	}

	public void playMovie(ReturnFunction onMovieComplete, int delayFrames)
	{
	}

	public void stopMovie()
	{
	}

	public void pauseMovie()
	{
	}

	public void goToAndStop(int frame)
	{
		goToAndStop((float)frame / (float)frames);
	}

	public void goToAndStop(float timeIndex)
	{
		stopTimeindex = timeIndex;
	}

	public void update()
	{
	}

	public void onGUI()
	{
	}
}
