using UnityEngine;
using UnityEngine.UI;

public class RunJavaScriptInWebSceneManager : MonoBehaviour
{
	public Text result;

	private UniWebView _webView;

	private string _fileName = "UniWebViewDemo/demo.html";

	public void LoadFromFile()
	{
		if (!(_webView != null))
		{
			_webView = CreateWebView();
			_webView.url = UniWebViewHelper.streamingAssetURLForPath(_fileName);
			int screenHeight = UniWebViewHelper.screenHeight;
			_webView.insets = new UniWebViewEdgeInsets(0, 0, screenHeight / 2, 0);
			_webView.OnEvalJavaScriptFinished += OnEvalJavaScriptFinished;
			_webView.OnWebViewShouldClose += (UniWebView webView) =>
			{
				_webView = null;
				return true;
			};
			_webView.Load();
			_webView.Show();
		}
	}

	public void AddScript(InputField input)
	{
		if (_webView == null)
		{
			result.text = "Please open the web view first.";
		}
		else
		{
			_webView.AddJavaScript(input.text);
		}
	}

	public void RunScript(InputField input)
	{
		if (_webView == null)
		{
			result.text = "Please open the web view first.";
		}
		else
		{
			_webView.EvaluatingJavaScript(input.text);
		}
	}

	private void OnEvalJavaScriptFinished(UniWebView webView, string r)
	{
		result.text = r;
	}

	private UniWebView CreateWebView()
	{
		GameObject gameObject = GameObject.Find("WebView");
		if (gameObject == null)
		{
			gameObject = new GameObject("WebView");
		}
		UniWebView uniWebView = gameObject.AddComponent<UniWebView>();
		uniWebView.toolBarShow = true;
		return uniWebView;
	}
}
