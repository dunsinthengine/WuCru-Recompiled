using UnityEngine;
using UnityEngine.UI;

public class DAX_MainCamera : MonoBehaviour
{
	public GameObject[] Items;

	public string[] Descr;

	public Text OutText;

	public Text OutDescr;

	public int curIndex;

	private GameObject curPrefab;

	private void Start()
	{
		showPrefab(curIndex);
	}

	private void Update()
	{
	}

	private void FixedUpdate()
	{
		OutText.text = string.Format("{0}/{1}", curIndex + 1, Items.Length);
	}

	public void Next()
	{
		curIndex++;
		if (curIndex >= Items.Length)
		{
			curIndex = 0;
		}
		showPrefab(curIndex);
	}

	public void Prev()
	{
		curIndex--;
		if (curIndex < 0)
		{
			curIndex = Items.Length - 1;
		}
		showPrefab(curIndex);
	}

	public void showPrefab(int index)
	{
		if (curPrefab != null)
		{
			Object.Destroy(curPrefab);
		}
		curPrefab = Object.Instantiate(Items[curIndex]);
		curPrefab.transform.position.Set(0f, 0f, 0f);
		OutDescr.text = Descr[curIndex];
	}
}
