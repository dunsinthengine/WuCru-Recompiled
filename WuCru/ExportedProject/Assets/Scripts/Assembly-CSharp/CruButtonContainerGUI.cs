using UnityEngine;
using UnityEngine.UI;

public class CruButtonContainerGUI : MonoBehaviour
{
	public delegate void OnButtonClickDelegtate(int ninjaID);

	public int ninjaID;

	public Image imageStatus;

	public Text label;

	public Image shardImage;

	public Text shardLabel;

	public Image levelImage;

	public Text levelLabel;

	public Text missingLabel;

	private bool wasVisible;

	public OnButtonClickDelegtate OnButtonClick;

	private void Awake()
	{
		Button componentInChildren = GetComponentInChildren<Button>();
		componentInChildren.onClick.AddListener(() =>
		{
			if (GameController.Instance.PlayerProfile.IsNinjaInTeam(ninjaID))
			{
				AudioPlayer.PlaySound("ui/button/click");
				imageStatus.enabled = true;
				GameController.Instance.PlayerProfile.RemoveNinjaFromTeam(ninjaID);
				if (OnButtonClick != null)
				{
					OnButtonClick(ninjaID);
				}
			}
			else if (GameController.Instance.PlayerProfile.SetNinjaInTeam(ninjaID))
			{
				AudioPlayer.PlaySound("ui/button/click");
				imageStatus.enabled = true;
				if (OnButtonClick != null)
				{
					OnButtonClick(ninjaID);
				}
				if (!GameController.Instance.PlayerProfile.HasSeenBriefing(PlayerBriefingType.EquipCruMember))
				{
					NinjaScreenController ninjaScreenController = Object.FindObjectOfType<NinjaScreenController>();
					ninjaScreenController.CompleteEquipCru();
				}
			}
		});
	}

	private void Initialize()
	{
		Button componentInChildren = GetComponentInChildren<Button>();
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		bool flag = playerProfile.IsNinjaInTeam(ninjaID);
		bool flag2 = playerProfile.UnlockedNinjaSKUs.Contains(ninjaID);
		if (flag)
		{
			imageStatus.enabled = true;
		}
		else
		{
			imageStatus.enabled = false;
		}
		NinjaPortrait componentInChildren2 = GetComponentInChildren<NinjaPortrait>();
		Animator componentInChildren3 = componentInChildren2.GetComponentInChildren<Animator>();
		if (flag)
		{
			componentInChildren2.Configure(ninjaID, CruStatsGUI.characterTextureMap[ninjaID], NinjaPortraitState.Selected);
			componentInChildren3.enabled = false;
		}
		else if (flag2)
		{
			componentInChildren2.Configure(ninjaID, CruStatsGUI.characterTextureMap[ninjaID], NinjaPortraitState.UnequippedMenu);
			componentInChildren3.Play("ButtonActive");
		}
		else
		{
			componentInChildren2.Configure(ninjaID, CruStatsGUI.characterTextureMap[ninjaID], NinjaPortraitState.LockedMenu);
			componentInChildren3.enabled = false;
		}
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

	public void Refresh()
	{
		Initialize();
	}
}
