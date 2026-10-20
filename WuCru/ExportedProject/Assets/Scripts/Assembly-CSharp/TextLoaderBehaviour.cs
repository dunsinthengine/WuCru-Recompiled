using System;
using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class TextLoaderBehaviour : MonoBehaviour
{
	private bool _loading;

	private string _location;

	private OnTextLoadComplete _callback;

	private object[] _args;

	public void LoadText(string location, OnTextLoadComplete callback, params object[] args)
	{
		if (location == null || !(location != string.Empty) || callback == null)
		{
			return;
		}
		if (_loading)
		{
			Debug.LogError("Cannot load '" + location + "' as still loading '" + _location + "'");
			return;
		}
		if (Application.isPlaying)
		{
			_location = location;
			_callback = callback;
			_args = args;
			_loading = true;
			StartCoroutine(LoadTextCoroutine());
			return;
		}
		WWW wWW = new WWW(location);
		DateTime now = DateTime.Now;
		while (!wWW.isDone)
		{
			if ((DateTime.Now - now).TotalSeconds > 45.0)
			{
				Debug.LogError("Synchronous Text Load Timeout! -> " + location);
				return;
			}
		}
		callback(wWW.text, args);
	}

	private IEnumerator LoadTextCoroutine()
	{
		WWW request = new WWW(_location);
		yield return request;
		_loading = false;
		_callback(request.text, _args);
	}
}
