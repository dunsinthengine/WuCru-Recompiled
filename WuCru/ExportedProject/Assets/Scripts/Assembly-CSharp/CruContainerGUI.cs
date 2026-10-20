using UnityEngine;
using UnityEngine.UI;

public class CruContainerGUI : MonoBehaviour
{
	public delegate void OnButtonClickDelegate();

	public int index;

	public Image closeImage;

	public Image lockImage;

	public Image emptyImage;

	public Image levelImage;

	public NinjaPortrait portrait;

	private bool wasVisible;

	public OnButtonClickDelegate OnButtonClick;

	private void Start()
	{
		portrait.button.onClick.AddListener(() =>
		{
			int ninjaId = GetNinjaId();
			if (ninjaId > 0)
			{
				AudioPlayer.PlaySound("ui/button/shing");
				GameController.Instance.PlayerProfile.RemoveNinjaFromTeam(ninjaId);
				if (OnButtonClick != null)
				{
					OnButtonClick();
				}
			}
			else
			{
				AudioPlayer.PlaySound("ui/button/back/click");
			}
		});
		Initialize();
	}

	private void Initialize()
	{
		int num = GameController.Instance.PlayerProfile.NinjaTeamSlotCount;
		if (num > GameController.Instance.PlayerProfile.UnlockedNinjaSKUs.Count)
		{
			num = GameController.Instance.PlayerProfile.UnlockedNinjaSKUs.Count;
		}
		int ninjaId = GetNinjaId();
		portrait.button.enabled = num > index;
		if (ninjaId != -1)
		{
			portrait.background.sprite = Resources.Load<Sprite>(Paths.ninjaBackgroundSlots);
			portrait.background.enabled = true;
			portrait.Configure(ninjaId, CruStatsGUI.characterTextureMap[ninjaId], NinjaPortraitState.Selected);
			portrait.frame[0].sprite = Resources.Load<Sprite>(Paths.ninjaUpgradeFrame);
			portrait.frame[1].sprite = Resources.Load<Sprite>(Paths.ninjaUpgradeFrame);
			Animator component = portrait.GetComponent<Animator>();
			component.enabled = false;
		}
		else if (index < num)
		{
			portrait.background.sprite = Resources.Load<Sprite>(Paths.ninjaBackgroundSimple);
			portrait.background.enabled = true;
			portrait.Hide(NinjaPortraitState.UnequippedMenu);
			portrait.frame[0].sprite = Resources.Load<Sprite>(Paths.ninjaUpgradeFrameSimple);
			portrait.frame[1].sprite = Resources.Load<Sprite>(Paths.ninjaUpgradeFrameSimple);
			Animator component2 = portrait.GetComponent<Animator>();
			component2.Play("ButtonActive");
		}
		else
		{
			portrait.background.sprite = Resources.Load<Sprite>(Paths.ninjaBackgroundSimple);
			portrait.background.enabled = true;
			portrait.Hide(NinjaPortraitState.LockedMenu);
			portrait.frame[0].sprite = Resources.Load<Sprite>(Paths.ninjaUpgradeFrameSimple);
			portrait.frame[1].sprite = Resources.Load<Sprite>(Paths.ninjaUpgradeFrameSimple);
			Animator component3 = portrait.GetComponent<Animator>();
			component3.enabled = false;
		}
		emptyImage.gameObject.SetActive(ninjaId == -1 && index < num);
		closeImage.gameObject.SetActive(false);
		lockImage.gameObject.SetActive(index >= num);
		levelImage.gameObject.SetActive(index >= num);
	}

	public int GetNinjaId()
	{
		return GameController.Instance.PlayerProfile.GetNinjaInTeam(index);
	}

	public void Refresh()
	{
		Initialize();
	}

	private void Update()
	{
		if (base.gameObject.activeSelf && !wasVisible)
		{
			wasVisible = true;
			Initialize();
		}
		if (!base.gameObject.activeSelf && wasVisible)
		{
			wasVisible = false;
		}
	}
}
