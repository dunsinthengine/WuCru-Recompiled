using UnityEngine;

public class StudsCounter : MonoBehaviour
{
	public static StudsCounter _instance;

	private int studs;

	public static StudsCounter _pInstance
	{
		get
		{
			return _instance;
		}
	}

	public int StudsCollected
	{
		get
		{
			return studs;
		}
	}

	private void Awake()
	{
		_instance = this;
		ScreenHud._pInstance.OnStudCountChanged(0, false);
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Pickup()
	{
		studs++;
		UpdateText();
	}

	public void Pickup(int amount)
	{
		studs += amount;
		UpdateText();
	}

	public void SetAmount(int amount)
	{
		studs = amount;
		UpdateText();
	}

	public void Remove()
	{
		studs--;
		UpdateText();
	}

	public Vector3 GetCenterPosition()
	{
		return base.transform.localPosition;
	}

	private void UpdateText()
	{
		ScreenHud._pInstance.OnStudCountChanged(studs, true);
	}
}
