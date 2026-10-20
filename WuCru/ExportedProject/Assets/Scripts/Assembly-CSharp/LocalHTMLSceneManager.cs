using UnityEngine;

public class LocalHTMLSceneManager : MonoBehaviour
{
	public string fileName;

	public string htmlText;

	public void LoadFromFile()
	{
		UniWebView uniWebView = CreateWebView();
		uniWebView.url = UniWebViewHelper.streamingAssetURLForPath(fileName);
		uniWebView.Load();
		uniWebView.Show();
	}

	public void LoadFromText()
	{
		UniWebView uniWebView = CreateWebView();
		uniWebView.LoadHTMLString(htmlText, null);
		uniWebView.Show();
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
