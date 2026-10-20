using System;
using UnityEngine;
using UnityEngine.UI;

public class IngamePauseGUI : MonoBehaviour
{
	public Text studsCounterText;

	public Image studsCounterImage;

	public Button buttonOK;

	public Button buttonCancel;

	public CompletionChallengeContainer[] challenges;

	public Action onOKAction;

	public Action onCancelAction;

	public GameObject collectionContainerGrid;

	private void Awake()
	{
		buttonOK.onClick.AddListener(() =>
		{
			AudioPlayer.PlaySound("ui/button/sword/shing");
			GameObject gameObject11 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/ConfirmQuitGUI"));
			ConfirmQuitGUI component11 = gameObject11.GetComponent<ConfirmQuitGUI>();
			component11.onOKAction = (GameController.Instance.currentState as GameControllerStatePlaying).ConfirmQuit;
			component11.onCancelAction = (GameController.Instance.currentState as GameControllerStatePlaying).CancelQuit;
			UnityEngine.Object.Destroy(base.transform.parent.gameObject);
		});
		buttonCancel.onClick.AddListener(() =>
		{
			AudioPlayer.PlaySound("ui/button/back/click");
			onCancelAction();
			UnityEngine.Object.Destroy(base.transform.parent.gameObject);
		});
		for (int num = 0; num < challenges.Length; num++)
		{
			challenges[num].gameObject.SetActive(false);
		}
		GameController.Instance.MissionStats.CalcProgressionStats();
		int numChestsWoodToCollect = GameController.Instance.MissionStats.NumChestsWoodToCollect;
		numChestsWoodToCollect += GameController.Instance.MissionStats.NumChestsSilverToCollect;
		numChestsWoodToCollect += GameController.Instance.MissionStats.NumChestsGoldToCollect;
		if (numChestsWoodToCollect > 1)
		{
			numChestsWoodToCollect--;
		}
		int numCrystalsToCollect = GameController.Instance.MissionStats.NumCrystalsToCollect;
		numCrystalsToCollect += GameController.Instance.MissionStats.NumCrystalsEarthToCollect;
		numCrystalsToCollect += GameController.Instance.MissionStats.NumCrystalsFireToCollect;
		numCrystalsToCollect += GameController.Instance.MissionStats.NumCrystalsIceToCollect;
		numCrystalsToCollect += GameController.Instance.MissionStats.NumCrystalsElectricityToCollect;
		numCrystalsToCollect += GameController.Instance.MissionStats.NumCrystalsEnergyToCollect;
		numCrystalsToCollect += GameController.Instance.MissionStats.NumCrystalsWaterToCollect;
		if (numCrystalsToCollect > 1)
		{
			numCrystalsToCollect--;
		}
		int num2 = 0;
		for (int num3 = 0; num3 < GameController.Instance.MissionStats.NumCrystalsToCollect; num3++)
		{
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/CompletionObjectContainer"));
			CompletionObjectContainerGUI component = gameObject.GetComponent<CompletionObjectContainerGUI>();
			gameObject.transform.SetParent(collectionContainerGrid.transform);
			if (num3 + 1 <= GameController.Instance.MissionStats.NumCrystalsSoFar)
			{
				component.SetCompleted(CompletionObjectType.Crystal, true);
			}
			else
			{
				component.SetCompleted(CompletionObjectType.Crystal, false);
			}
		}
		num2 += GameController.Instance.MissionStats.NumCrystalsToCollect;
		for (int num4 = 0; num4 < GameController.Instance.MissionStats.NumCrystalsEarthToCollect; num4++)
		{
			GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/CompletionObjectContainer"));
			CompletionObjectContainerGUI component2 = gameObject2.GetComponent<CompletionObjectContainerGUI>();
			gameObject2.transform.SetParent(collectionContainerGrid.transform);
			if (num4 + 1 <= GameController.Instance.MissionStats.NumCrystalsEarthSoFar)
			{
				component2.SetCompleted(CompletionObjectType.CrystalEarth, true);
			}
			else
			{
				component2.SetCompleted(CompletionObjectType.CrystalEarth, false);
			}
		}
		num2 += GameController.Instance.MissionStats.NumCrystalsEarthToCollect;
		for (int num5 = 0; num5 < GameController.Instance.MissionStats.NumCrystalsFireToCollect; num5++)
		{
			GameObject gameObject3 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/CompletionObjectContainer"));
			CompletionObjectContainerGUI component3 = gameObject3.GetComponent<CompletionObjectContainerGUI>();
			gameObject3.transform.SetParent(collectionContainerGrid.transform);
			if (num5 + 1 <= GameController.Instance.MissionStats.NumCrystalsFireSoFar)
			{
				component3.SetCompleted(CompletionObjectType.CrystalFire, true);
			}
			else
			{
				component3.SetCompleted(CompletionObjectType.CrystalFire, false);
			}
		}
		num2 += GameController.Instance.MissionStats.NumCrystalsFireToCollect;
		for (int num6 = 0; num6 < GameController.Instance.MissionStats.NumCrystalsIceToCollect; num6++)
		{
			GameObject gameObject4 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/CompletionObjectContainer"));
			CompletionObjectContainerGUI component4 = gameObject4.GetComponent<CompletionObjectContainerGUI>();
			gameObject4.transform.SetParent(collectionContainerGrid.transform);
			if (num6 + 1 <= GameController.Instance.MissionStats.NumCrystalsIceSoFar)
			{
				component4.SetCompleted(CompletionObjectType.CrystalIce, true);
			}
			else
			{
				component4.SetCompleted(CompletionObjectType.CrystalIce, false);
			}
		}
		num2 += GameController.Instance.MissionStats.NumCrystalsIceToCollect;
		for (int num7 = 0; num7 < GameController.Instance.MissionStats.NumCrystalsElectricityToCollect; num7++)
		{
			GameObject gameObject5 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/CompletionObjectContainer"));
			CompletionObjectContainerGUI component5 = gameObject5.GetComponent<CompletionObjectContainerGUI>();
			gameObject5.transform.SetParent(collectionContainerGrid.transform);
			if (num7 + 1 <= GameController.Instance.MissionStats.NumCrystalsElectricitySoFar)
			{
				component5.SetCompleted(CompletionObjectType.CrystalElectricity, true);
			}
			else
			{
				component5.SetCompleted(CompletionObjectType.CrystalElectricity, false);
			}
		}
		num2 += GameController.Instance.MissionStats.NumCrystalsElectricityToCollect;
		for (int num8 = 0; num8 < GameController.Instance.MissionStats.NumCrystalsEnergyToCollect; num8++)
		{
			GameObject gameObject6 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/CompletionObjectContainer"));
			CompletionObjectContainerGUI component6 = gameObject6.GetComponent<CompletionObjectContainerGUI>();
			gameObject6.transform.SetParent(collectionContainerGrid.transform);
			if (num8 + 1 <= GameController.Instance.MissionStats.NumCrystalsEnergySoFar)
			{
				component6.SetCompleted(CompletionObjectType.CrystalEnergy, true);
			}
			else
			{
				component6.SetCompleted(CompletionObjectType.CrystalEnergy, false);
			}
		}
		num2 += GameController.Instance.MissionStats.NumCrystalsEnergyToCollect;
		for (int num9 = 0; num9 < GameController.Instance.MissionStats.NumCrystalsWaterToCollect; num9++)
		{
			GameObject gameObject7 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/CompletionObjectContainer"));
			CompletionObjectContainerGUI component7 = gameObject7.GetComponent<CompletionObjectContainerGUI>();
			gameObject7.transform.SetParent(collectionContainerGrid.transform);
			if (num9 + 1 <= GameController.Instance.MissionStats.NumCrystalsWaterSoFar)
			{
				component7.SetCompleted(CompletionObjectType.CrystalWater, true);
			}
			else
			{
				component7.SetCompleted(CompletionObjectType.CrystalWater, false);
			}
		}
		int numChestsWoodToCollect2 = GameController.Instance.MissionStats.NumChestsWoodToCollect;
		int numChestsSilverToCollect = GameController.Instance.MissionStats.NumChestsSilverToCollect;
		int numChestsGoldToCollect = GameController.Instance.MissionStats.NumChestsGoldToCollect;
		for (int num10 = 0; num10 < numChestsWoodToCollect2; num10++)
		{
			GameObject gameObject8 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/CompletionObjectContainer"));
			CompletionObjectContainerGUI component8 = gameObject8.GetComponent<CompletionObjectContainerGUI>();
			gameObject8.transform.SetParent(collectionContainerGrid.transform);
			if (num10 + 1 <= GameController.Instance.MissionStats.NumChestsWoodSoFar)
			{
				component8.SetCompleted(CompletionObjectType.ChestWood, true);
			}
			else
			{
				component8.SetCompleted(CompletionObjectType.ChestWood, false);
			}
		}
		for (int num11 = 0; num11 < numChestsSilverToCollect; num11++)
		{
			GameObject gameObject9 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/CompletionObjectContainer"));
			CompletionObjectContainerGUI component9 = gameObject9.GetComponent<CompletionObjectContainerGUI>();
			gameObject9.transform.SetParent(collectionContainerGrid.transform);
			if (num11 + 1 <= GameController.Instance.MissionStats.NumChestsSilverSoFar)
			{
				component9.SetCompleted(CompletionObjectType.ChestSilver, true);
			}
			else
			{
				component9.SetCompleted(CompletionObjectType.ChestSilver, false);
			}
		}
		for (int num12 = 0; num12 < numChestsGoldToCollect; num12++)
		{
			GameObject gameObject10 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/CompletionObjectContainer"));
			CompletionObjectContainerGUI component10 = gameObject10.GetComponent<CompletionObjectContainerGUI>();
			gameObject10.transform.SetParent(collectionContainerGrid.transform);
			if (num12 + 1 <= GameController.Instance.MissionStats.NumChestsGoldSoFar)
			{
				component10.SetCompleted(CompletionObjectType.ChestGold, true);
			}
			else
			{
				component10.SetCompleted(CompletionObjectType.ChestGold, false);
			}
		}
		int num13 = 0;
		foreach (LevelChallengeBase key in GameController.Instance.MissionStats.Challenges.Keys)
		{
			challenges[num13].gameObject.SetActive(true);
			challenges[num13].imageChallenge.sprite = Resources.Load<Sprite>(key.IconPath());
			if (GameController.Instance.MissionStats.Challenges[key])
			{
				challenges[num13].imageCheckmark.gameObject.SetActive(true);
				challenges[num13].imageFailed.gameObject.SetActive(false);
			}
			else
			{
				challenges[num13].imageCheckmark.gameObject.SetActive(false);
				challenges[num13].imageFailed.gameObject.SetActive(true);
			}
			num13++;
		}
		challenges[num13].gameObject.SetActive(true);
		challenges[num13].imageChallenge.sprite = Resources.Load<Sprite>(GameController.Instance.CurrentLevel.Challenge.IconPath());
		if (GameController.Instance.CurrentLevel.Challenge.IsCompleted)
		{
			challenges[num13].imageCheckmark.gameObject.SetActive(true);
			challenges[num13].imageFailed.gameObject.SetActive(false);
		}
		else if (GameController.Instance.CurrentLevel.Challenge.HasFailed)
		{
			challenges[num13].imageCheckmark.gameObject.SetActive(false);
			challenges[num13].imageFailed.gameObject.SetActive(true);
		}
		else
		{
			challenges[num13].imageCheckmark.gameObject.SetActive(false);
			challenges[num13].imageFailed.gameObject.SetActive(true);
		}
	}
}
