using System;
using System.Collections.Generic;
using Cape.Localization;
using UnityEngine;
using UnityEngine.UI;

public class CruStatsGUI : MonoBehaviour
{
	[SerializeField]
	private CruContainerGUI[] _teamNinjaGUI;

	[SerializeField]
	private CruButtonContainerGUI[] _unlockedNinjaGUI;

	[SerializeField]
	private NinjaPortrait[] _teamNinjaPortraits;

	[SerializeField]
	private NinjaPortrait[] _unlockedNinjaPortraits;

	public Sprite[] shardPowerSprites;

	public static Dictionary<int, Texture2D> characterTextureMap = new Dictionary<int, Texture2D>();

	private bool wasVisible;

	private int[] _teamIds;

	private NinjaScreenController _ninjaScreen;

	private int _ninjaPortraitWidth;

	private int _ninjaPortraitHeight;

	private void Awake()
	{
		_ninjaScreen = GetComponentInParent<NinjaScreenController>();
		_ninjaScreen.HelmetToggled += NinjaScreen_HelmetToggled;
		PlayerProfile playerProfile = GameController.Instance.PlayerProfile;
		_teamIds = new int[6];
		_teamIds[0] = Team.GetID(MinifigType.Zane, playerProfile.ZaneLevel);
		_teamIds[1] = Team.GetID(MinifigType.Cole, playerProfile.ColeLevel);
		_teamIds[2] = Team.GetID(MinifigType.Jay, playerProfile.JayLevel);
		_teamIds[3] = Team.GetID(MinifigType.Kai, playerProfile.KaiLevel);
		_teamIds[4] = Team.GetID(MinifigType.Nya, playerProfile.NyaLevel);
		_teamIds[5] = Team.GetID(MinifigType.Lloyd, playerProfile.LloydLevel);
		SetNinjaPortraitSize();
		UpdateCharacterTextureMap();
		for (int i = 0; i < _unlockedNinjaGUI.Length && i < _teamIds.Length; i++)
		{
			_unlockedNinjaGUI[i].ninjaID = _teamIds[i];
			_unlockedNinjaGUI[i].OnButtonClick = OnButtonClick;
			bool flag = playerProfile.IsNinjaInTeam(_teamIds[i]);
			bool flag2 = playerProfile.UnlockedNinjaSKUs.Contains(_teamIds[i]);
			Localize component = _unlockedNinjaGUI[i].label.gameObject.GetComponent<Localize>();
			component.SetKey(CharacterBuilder.minifigDefinitions[_teamIds[i]].TypeLocalizationKey);
			if (flag2)
			{
				MinifigDefinition minifigDefinition = CharacterBuilder.minifigDefinitions[_teamIds[i]];
				_unlockedNinjaGUI[i].shardImage.sprite = shardPowerSprites[(int)minifigDefinition.elementalPower];
				MinifigType typeForID = Team.GetTypeForID(_teamIds[i]);
				if (typeForID == MinifigType.None)
				{
					continue;
				}
				int num = Team.ShardsForLevel(playerProfile.GetNinjaLevel(typeForID) + 1);
				if (num == -1)
				{
					_unlockedNinjaGUI[i].shardLabel.text = " " + Localization.Instance.Get("LABEL_MAX");
				}
				else
				{
					_unlockedNinjaGUI[i].shardLabel.text = " " + playerProfile.GetNinjaTokens(Team.GetTypeForID(_teamIds[i])) + "/" + num;
				}
				_unlockedNinjaGUI[i].levelLabel.text = playerProfile.GetNinjaLevel(Team.GetTypeForID(_teamIds[i])).ToString();
			}
			_unlockedNinjaGUI[i].missingLabel.gameObject.SetActive(!flag2);
			_unlockedNinjaGUI[i].shardImage.gameObject.SetActive(flag2);
			_unlockedNinjaGUI[i].shardLabel.gameObject.SetActive(flag2);
			_unlockedNinjaGUI[i].levelImage.gameObject.SetActive(flag2);
			_unlockedNinjaGUI[i].levelLabel.gameObject.SetActive(flag2);
			NinjaPortrait componentInChildren = _unlockedNinjaGUI[i].GetComponentInChildren<NinjaPortrait>();
			componentInChildren.Configure(_teamIds[i], characterTextureMap[_teamIds[i]], flag ? NinjaPortraitState.Selected : ((!flag2) ? NinjaPortraitState.Missing : NinjaPortraitState.Active));
		}
		int num2 = GameController.Instance.PlayerProfile.NinjaTeamSlotCount - 1;
		RefreshContainers();
	}

	private void Start()
	{
		for (int i = 0; i < _unlockedNinjaGUI.Length; i++)
		{
			Localize component = _unlockedNinjaGUI[i].label.gameObject.GetComponent<Localize>();
			component.SetKey(CharacterBuilder.minifigDefinitions[_unlockedNinjaGUI[i].ninjaID].TypeLocalizationKey);
		}
	}

	private void SetNinjaPortraitSize()
	{
		CanvasScaler componentInParent = GetComponentInParent<CanvasScaler>();
		float num = _unlockedNinjaPortraits[0].portrait.rectTransform.sizeDelta.x / componentInParent.referenceResolution.x;
		float num2 = _unlockedNinjaPortraits[0].portrait.rectTransform.sizeDelta.y / _unlockedNinjaPortraits[0].portrait.rectTransform.sizeDelta.x;
		_ninjaPortraitWidth = Mathf.FloorToInt((float)Screen.width * num);
		_ninjaPortraitHeight = Mathf.FloorToInt((float)Screen.width * num * num2);
	}

	public void UpdateCharacterTextureMap(int ninjaId = -1)
	{
		if (characterTextureMap == null)
		{
			characterTextureMap = new Dictionary<int, Texture2D>();
		}
		if (ninjaId == -1)
		{
			Texture2D[] array = CharacterPreviewRenderer.Generate(_teamIds, _ninjaPortraitWidth, _ninjaPortraitWidth);
			for (int i = 0; i < _teamIds.Length; i++)
			{
				characterTextureMap[_teamIds[i]] = array[i];
			}
		}
		else
		{
			Texture2D[] array2 = CharacterPreviewRenderer.Generate(new int[1] { ninjaId }, _ninjaPortraitWidth, _ninjaPortraitWidth);
			characterTextureMap[ninjaId] = array2[0];
		}
	}

	private void UpdateNinjaPortraits(int ninjaId = -1)
	{
		for (int i = 0; i < _teamNinjaGUI.Length; i++)
		{
			if (ninjaId == -1 || _teamNinjaGUI[i].GetNinjaId() == ninjaId)
			{
				_teamNinjaGUI[i].Refresh();
			}
		}
		for (int j = 0; j < _unlockedNinjaPortraits.Length && j < _teamIds.Length; j++)
		{
			if (ninjaId == -1 || _teamIds[j] == ninjaId)
			{
				_unlockedNinjaPortraits[j].UpdatePortraitTexture(characterTextureMap[_teamIds[j]]);
			}
		}
	}

	public void RefreshContainers()
	{
		for (int i = 0; i < _teamNinjaGUI.Length; i++)
		{
			CruContainerGUI obj = _teamNinjaGUI[i];
			obj.OnButtonClick = (CruContainerGUI.OnButtonClickDelegate)Delegate.Combine(obj.OnButtonClick, new CruContainerGUI.OnButtonClickDelegate(OnButtonClick));
			_teamNinjaGUI[i].Refresh();
		}
		for (int j = 0; j < _unlockedNinjaGUI.Length; j++)
		{
			CruButtonContainerGUI obj2 = _unlockedNinjaGUI[j];
			obj2.OnButtonClick = (CruButtonContainerGUI.OnButtonClickDelegtate)Delegate.Combine(obj2.OnButtonClick, new CruButtonContainerGUI.OnButtonClickDelegtate(OnButtonClick));
			_unlockedNinjaGUI[j].Refresh();
		}
	}

	public void NinjaScreen_HelmetToggled(object sender, DataEventArgs<int> e)
	{
		UpdateCharacterTextureMap(e.data);
		UpdateNinjaPortraits(e.data);
	}

	public void OnButtonClick(int ninjaID)
	{
		OnButtonClickInner();
	}

	public void OnButtonClick()
	{
		OnButtonClickInner();
	}

	private void OnButtonClickInner()
	{
		AudioPlayer.PlaySound("ui/button/click");
		for (int i = 0; i < _teamNinjaGUI.Length; i++)
		{
			_teamNinjaGUI[i].Refresh();
		}
		for (int j = 0; j < _unlockedNinjaGUI.Length; j++)
		{
			_unlockedNinjaGUI[j].Refresh();
		}
		_ninjaScreen.BuildCharacters();
		GameController.Instance.PlayerProfile.Save();
	}

	public void OnDestroy()
	{
		characterTextureMap = null;
		_ninjaScreen.HelmetToggled -= NinjaScreen_HelmetToggled;
	}
}
