using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class Credits : MonoBehaviour
{
	public delegate void CreditsEndListener(Credits c);

	public TextAsset creditsFile;

	public bool playOnAwake = true;

	public int speed = 100;

	public float fadeTime = 1f;

	public GameObject prefabSingleColumn;

	public GameObject prefabTwoColumns;

	public GameObject prefabImage;

	public GameObject prefabTitle;

	public GameObject prefabEmptySpace;

	private bool started;

	private List<CreditLine> lines = new List<CreditLine>();

	private int height;

	private int count;

	private static Credits _instance;

	public event CreditsEndListener endListeners;

	public static Credits getInstance()
	{
		return _instance;
	}

	private void Awake()
	{
		_instance = this;
	}

	private void Start()
	{
		if (playOnAwake)
		{
			beginCredits();
		}
		AnalyticsEvents.LogLoadScene("Credits");
	}

	public void beginCredits()
	{
		if (started)
		{
			return;
		}
		speed = Mathf.Clamp(speed, 50, 1000);
		height = (int)GetComponent<RectTransform>().rect.height;
		XmlReader xmlReader = XmlReader.Create(new StringReader(creditsFile.text));
		while (xmlReader.ReadToFollowing("credit"))
		{
			CreditType creditType = CreditType.EmptySpace;
			string text = string.Empty;
			string data = string.Empty;
			while (xmlReader.MoveToNextAttribute())
			{
				if (xmlReader.Name == "type")
				{
					creditType = CreditLine.textToType(xmlReader.Value);
				}
				else if (xmlReader.Name == "data")
				{
					text = xmlReader.Value;
				}
				else if (xmlReader.Name == "data2")
				{
					data = xmlReader.Value;
				}
			}
			if (creditType == CreditType.EmptySpace)
			{
				int num = Mathf.Clamp(int.Parse(text), 1, 100);
				for (int i = 0; i < num; i++)
				{
					lines.Add(new CreditLine(CreditType.EmptySpace, string.Empty, string.Empty));
				}
			}
			else
			{
				lines.Add(new CreditLine(creditType, text, data));
			}
		}
		count = lines.Count;
		started = true;
		spawnNext();
	}

	public void spawnNext()
	{
		if (lines.Count > 0)
		{
			CreditLine creditLine = lines[0];
			lines.RemoveAt(0);
			if (creditLine.type == CreditType.EmptySpace)
			{
				GameObject gameObject = Object.Instantiate(prefabEmptySpace);
				gameObject.GetComponent<RectTransform>().SetParent(base.transform, false);
				gameObject.GetComponent<CreditLineInstanceSpace>().go(creditLine.data, creditLine.data2, height, speed, fadeTime);
			}
			else if (creditLine.type == CreditType.Title)
			{
				GameObject gameObject2 = Object.Instantiate(prefabTitle);
				gameObject2.GetComponent<RectTransform>().SetParent(base.transform, false);
				gameObject2.GetComponent<CreditLineInstanceTitle>().go(creditLine.data, creditLine.data2, height, speed, fadeTime);
			}
			else if (creditLine.type == CreditType.SingleColumn)
			{
				GameObject gameObject3 = Object.Instantiate(prefabSingleColumn);
				gameObject3.GetComponent<RectTransform>().SetParent(base.transform, false);
				gameObject3.GetComponent<CreditLineInstanceSingleColumn>().go(creditLine.data, creditLine.data2, height, speed, fadeTime);
			}
			else if (creditLine.type == CreditType.TwoColumns)
			{
				GameObject gameObject4 = Object.Instantiate(prefabTwoColumns);
				gameObject4.GetComponent<RectTransform>().SetParent(base.transform, false);
				gameObject4.GetComponent<CreditLineInstanceTwoColumns>().go(creditLine.data, creditLine.data2, height, speed, fadeTime);
			}
			else if (creditLine.type == CreditType.Image)
			{
				GameObject gameObject5 = Object.Instantiate(prefabImage);
				gameObject5.GetComponent<RectTransform>().SetParent(base.transform, false);
				gameObject5.GetComponent<CreditLineInstanceImage>().go(creditLine.data, creditLine.data2, height, speed, fadeTime);
			}
		}
	}

	public void callbackDeleted()
	{
		count--;
		if (count <= 0 && endListeners != null)
		{
			endListeners(this);
		}
	}
}
