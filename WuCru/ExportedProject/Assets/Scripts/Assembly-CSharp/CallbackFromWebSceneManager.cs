using UnityEngine;
using UnityEngine.UI;

public class CallbackFromWebSceneManager : MonoBehaviour
{
	public Text result;

	private UniWebView _webView;

	private string _fileName = "UniWebViewDemo/callback.html";

	public void LoadFromFile()
	{
		if (!(_webView != null))
		{
			_webView = CreateWebView();
			_webView.url = UniWebViewHelper.streamingAssetURLForPath(_fileName);
			int screenHeight = UniWebViewHelper.screenHeight;
			_webView.insets = new UniWebViewEdgeInsets(0, 0, screenHeight / 2, 0);
			_webView.OnReceivedMessage += OnReceivedMessage;
			_webView.Load();
			_webView.Show();
		}
	}

	private void OnReceivedMessage(UniWebView webView, UniWebViewMessage message)
	{
		if (message.path == "close")
		{
			result.text = string.Empty;
			Object.Destroy(webView);
			_webView = null;
		}
		if (message.path == "add")
		{
			int num = 0;
			int num2 = 0;
			if (int.TryParse(message.args["num1"], out num) && int.TryParse(message.args["num2"], out num2))
			{
				int num3 = num + num2;
				result.text = num + " + " + num2 + " = " + num3;
			}
			else
			{
				result.text = "Invalid Input";
			}
		}
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
