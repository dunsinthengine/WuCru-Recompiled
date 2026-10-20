using UnityEngine;

public class CollectableCards
{
	public static ECOLLECTABLE_CARD GetCardID(Texture tex)
	{
		switch (tex.name)
		{
		case "Lloyd_Card":
			return ECOLLECTABLE_CARD.LLOYD;
		case "Kai_Card":
			return ECOLLECTABLE_CARD.KAI;
		case "Nya_Card":
			return ECOLLECTABLE_CARD.NYA;
		case "Zane_Card":
			return ECOLLECTABLE_CARD.ZANE;
		case "Cole_Card":
			return ECOLLECTABLE_CARD.COLE;
		case "Jay_Card":
			return ECOLLECTABLE_CARD.JAY;
		case "Wu_Card":
			return ECOLLECTABLE_CARD.WU;
		case "Garmadon_Card":
			return ECOLLECTABLE_CARD.GARMADON;
		case "General1_Card":
			return ECOLLECTABLE_CARD.GENERAL_1;
		case "General2_Card":
			return ECOLLECTABLE_CARD.GENERAL_2;
		case "FishSquad_Card":
			return ECOLLECTABLE_CARD.FISH_SQUAD;
		case "ITGuys_Card":
			return ECOLLECTABLE_CARD.IT_GUYS;
		default:
			return ECOLLECTABLE_CARD.INVALID;
		}
	}

	public static string GetCardPath(ECOLLECTABLE_CARD index)
	{
		bool flag = GameController.Instance.PlayerProfile.HasCardBeenUnlocked((int)index);
		switch (index)
		{
		case ECOLLECTABLE_CARD.LLOYD:
			return (!flag) ? "Missing_Card_PL" : "Lloyd_Card";
		case ECOLLECTABLE_CARD.KAI:
			return (!flag) ? "Missing_Card_PL" : "Kai_Card";
		case ECOLLECTABLE_CARD.NYA:
			return (!flag) ? "Missing_Card_PL" : "Nya_Card";
		case ECOLLECTABLE_CARD.ZANE:
			return (!flag) ? "Missing_Card_PL" : "Zane_Card";
		case ECOLLECTABLE_CARD.COLE:
			return (!flag) ? "Missing_Card_PL" : "Cole_Card";
		case ECOLLECTABLE_CARD.JAY:
			return (!flag) ? "Missing_Card_PL" : "Jay_Card";
		case ECOLLECTABLE_CARD.WU:
			return (!flag) ? "Missing_Card_PL" : "Wu_Card";
		case ECOLLECTABLE_CARD.GARMADON:
			return (!flag) ? "Missing_Card_PL" : "Garmadon_Card";
		case ECOLLECTABLE_CARD.GENERAL_1:
			return (!flag) ? "Missing_Card_PL" : "General1_Card";
		case ECOLLECTABLE_CARD.GENERAL_2:
			return (!flag) ? "Missing_Card_PL" : "General2_Card";
		case ECOLLECTABLE_CARD.FISH_SQUAD:
			return (!flag) ? "Missing_Card_PL" : "FishSquad_Card";
		case ECOLLECTABLE_CARD.IT_GUYS:
			return (!flag) ? "Missing_Card_PL" : "ITGuys_Card";
		case ECOLLECTABLE_CARD.INVALID:
			return "Missing_Card_PL";
		default:
			return null;
		}
	}
}
