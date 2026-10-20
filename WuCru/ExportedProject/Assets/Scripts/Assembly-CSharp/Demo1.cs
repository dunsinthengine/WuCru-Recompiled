using UnityEngine;
using UnityEngine.UI;

public class Demo1 : MonoBehaviour
{
	public Text header;

	public TypefaceAnimator[] anims;

	private int m_currentNum;

	public int currentNum
	{
		get
		{
			return m_currentNum;
		}
		set
		{
			m_currentNum = value;
			if (m_currentNum >= anims.Length)
			{
				m_currentNum = 0;
			}
			else if (m_currentNum < 0)
			{
				m_currentNum = anims.Length - 1;
			}
		}
	}

	private string headerText
	{
		get
		{
			return currentNum + 1 + " / " + anims.Length;
		}
	}

	private void Start()
	{
		SwitchAnimation(m_currentNum);
		header.text = headerText;
	}

	private void Update()
	{
		if (Input.GetKeyDown("right"))
		{
			OnChangeAnimation(1);
		}
		if (Input.GetKeyDown("left"))
		{
			OnChangeAnimation(-1);
		}
	}

	private void SwitchAnimation(int num)
	{
		for (int i = 0; i < anims.Length; i++)
		{
			anims[i].gameObject.SetActive(false);
		}
		anims[num].gameObject.SetActive(true);
	}

	public void OnChangeAnimation(int num)
	{
		currentNum += num;
		SwitchAnimation(currentNum);
		header.text = headerText;
	}
}
