using UnityEngine;
using UnityEngine.UI;

public class UseWithCodeSceneManager : MonoBehaviour
{
	public InputField urlInput;

	private void Start()
	{
		urlInput.text = "https://google.com";
	}

	public void OpenButtonClicked()
	{
		GameObject gameObject = GameObject.Find("WebView");
		if (gameObject == null)
		{
			gameObject = new GameObject("WebView");
		}
		UniWebView uniWebView = gameObject.AddComponent<UniWebView>();
		uniWebView.OnLoadComplete += OnLoadComplete;
		uniWebView.InsetsForScreenOreitation += InsetsForScreenOreitation;
		uniWebView.toolBarShow = true;
		uniWebView.url = urlInput.text;
		uniWebView.Load();
	}

	private void OnLoadComplete(UniWebView webView, bool success, string errorMessage)
	{
		if (success)
		{
			webView.Show();
		}
		else
		{
			Debug.Log("Something wrong in webview loading: " + errorMessage);
		}
	}

	private UniWebViewEdgeInsets InsetsForScreenOreitation(UniWebView webView, UniWebViewOrientation orientation)
	{
		if (orientation == UniWebViewOrientation.Portrait)
		{
			return new UniWebViewEdgeInsets(5, 5, 5, 5);
		}
		return new UniWebViewEdgeInsets(5, 5, 5, 5);
	}
}
