using UnityEngine;

public class Weapon : MonoBehaviour
{
	public float scytheDamageBonus = 1.15f;

	public float katanaDuelBonus = 1.15f;

	public float shurikenHitWindowBonus = 0.15f;

	public float nunchucksBlockWindowBonus = 0.15f;

	public WeaponType type;

	private void Awake()
	{
		base.enabled = false;
	}

	public float GetHitPointFactor()
	{
		if (type == WeaponType.Scythe)
		{
			return scytheDamageBonus;
		}
		if (type == WeaponType.VM_Scythe)
		{
			return scytheDamageBonus;
		}
		return 1f;
	}

	public float GetDuellingStrengthFactor()
	{
		if (type == WeaponType.Katana)
		{
			return katanaDuelBonus;
		}
		if (type == WeaponType.VM_Sword)
		{
			return katanaDuelBonus;
		}
		return 1f;
	}

	public SwordTappingSettings GetAttackTappingSettings()
	{
		SwordTappingSettings randomSettings = SwordTappingSettings.GetRandomSettings();
		if (type == WeaponType.Shuriken)
		{
			float num = (randomSettings.endWindow - randomSettings.beginWindow) * shurikenHitWindowBonus;
			randomSettings.endWindow += num;
			randomSettings.duration += num;
		}
		return randomSettings;
	}

	public SwordTappingSettings GetParryTappingSettings()
	{
		SwordTappingSettings randomSettings = SwordTappingSettings.GetRandomSettings();
		if (type == WeaponType.Nunchuck)
		{
			float num = (randomSettings.endWindow - randomSettings.beginWindow) * nunchucksBlockWindowBonus;
			randomSettings.endWindow += num;
			randomSettings.duration += num;
		}
		return randomSettings;
	}
}
