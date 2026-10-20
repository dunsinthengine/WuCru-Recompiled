using System.IO;
using UnityEngine;

public static class Paths
{
	public static string characterPrefabBasePath = "Prefabs/Characters/";

	public static string characterTextureBasePath = "Characters/Textures/Minifig_decals-assets/";

	public static string enemiesTextureBasePath = "Characters/Textures/Minifig_decals-assets/";

	public static string emoticonAlertedIcon = "EmoticonSprites/alert icon";

	public static string emoticonSuspicious = "EmoticonSprites/enemy suspicious icon";

	public static string emoticonAlertedOffscreen = "EmoticonSprites/OffScreen/enemy alerted";

	public static string emoticonSuspiciousOffscreen = "EmoticonSprites/OffScreen/enemy suspicious";

	public static string emoticonDoorIcon = "EmoticonSprites/DoorIcon";

	public static string emoticonDoorOffscreen = "EmoticonSprites/OffScreen/DoorIconOffscreen";

	public static string emoticonAttackingIcon = "EmoticonSprites/attackingIcon";

	public static string emoticonAttackingOffscreen = "EmoticonSprites/Offscreen/enemy attacking";

	public static string ninjaCruStatusIconEmpty = "NinjaCruStatusSprites/icon_unfilled";

	public static string ninjaCruStatusIconFilled = "NinjaCruStatusSprites/icon green checkmark";

	public static string areaMapStarIconEmpty = "MapIcons/reward star empty";

	public static string areaMapStarIconFull = "MapIcons/reward star full";

	public static string weaponSpriteKatana = "NinjaSprites/weapon katana";

	public static string weaponSpriteShuriken = "NinjaSprites/weapon shuriken";

	public static string weaponSpriteScythe = "NinjaSprites/weapon scythe";

	public static string weaponSpriteNunchuk = "NinjaSprites/weapon nunchuck";

	public static string elementSpriteEarth = "NinjaSprites/element earth";

	public static string elementSpriteEnergy = "NinjaSprites/element energy";

	public static string elementSpriteElectricicy = "NinjaSprites/element electricity";

	public static string elementSpriteFire = "NinjaSprites/element fire";

	public static string elementSpriteIce = "NinjaSprites/element ice";

	public static string elementSpriteWater = "NinjaSprites/element water";

	public static string skillSpritePerfectAttack = "SkillIcons/icon_weapon";

	public static string skillSpritePerfectBlock = "SkillIcons/icon_weapon";

	public static string skillSpritePerfectSneak = "SkillIcons/icon_weapon";

	public static string skillSpritePerfectSpin = "SkillIcons/icon_spinjitzu";

	public static string skillSpriteSpinjitzuElevation = "SkillIcons/icon_spinjitzu";

	public static string skillSpriteTime = "SkillIcons/icon_spinjitzu";

	public static string skillSpriteSpeed = "SkillIcons/icon_agility";

	public static string skillSpriteDamage = "SkillIcons/icon_strength";

	public static string skillSpriteSpinjitzu = "SkillIcons/icon_spinjitzu";

	public static string skillSpriteHeart = "SkillIcons/icon_stamina";

	public static string skillSpriteChestHeart = "SkillIcons/icon_stamina";

	public static string skillSpriteLvlBase = "SkillIcons/";

	public static string skillSpriteCoinsBase = "SkillIcons/skillcoins";

	public static string skillImageSpinjitzu = "SkillIcons/levelupimage spinjitzu";

	public static string skillImageWeapon = "SkillIcons/levelupimage weapon";

	public static string skillImageStrength = "SkillIcons/levelupimage strength";

	public static string skillImageSpeed = "SkillIcons/levelupimage speed";

	public static string skillImageHealth = "SkillIcons/levelupimage health";

	public static string vehicleIcon = "VehicleIcons/vehicle_";

	public static string ninjaUpgradeFrame = "NinjaCruStatusSprites/user framing white";

	public static string ninjaUpgradeFrameSimple = "NinjaCruStatusSprites/upgrade user framing simple";

	public static string ninjaBackgroundSlots = "NinjaCruStatusSprites/user framing bg";

	public static string ninjaBackgroundSimple = "NinjaCruStatusSprites/user framing circle bg";

	public static string sceneNameNinjaScreen = "NinjaScreen";

	public static string sceneNameWorldMap = "WorldMap";

	public static string sceneNameAreaMap = "AreaMap";

	public static string sceneNameTeaFarm = "TeaFarm";

	public static string sceneNameGarage = "Garage";

	public static string challengeDefault = "Challenges/challenge_none";

	public static string challengeAnyChest = "Challenges/challenge_any_chest";

	public static string challengeAnyCrystal = "Challenges/challenge_any_crystal";

	public static string challengeStuds = "Challenges/challenge_studs";

	public static string challengeExplodeEnemies = "Challenges/challenge_explode_enemies";

	public static string challengeGoldChest = "Challenges/challenge_gold_chest";

	public static string challengeGuardedChest = "Challenges/challenge_guarded_chest";

	public static string challengeKillAllEnemies = "Challenges/challenge_kill_all_enemies";

	public static string challengeKillWarrior = "Challenges/challenge_kill_warrior";

	public static string challengeKillHeavyPatrol = "Challenges/challenge_kill_heavy_patrol";

	public static string challengeMetalChest = "Challenges/challenge_metal_chest";

	public static string challengeSneakAttack = "Challenges/challenge_sneakattack";

	public static string challengeSpinjitzuEnemies = "Challenges/challenge_spinjitzu";

	public static string challengeStealth = "Challenges/challenge_stealth";

	public static string challengeTrapKill = "Challenges/challenge_trap_kill";

	public static string challengeTriggerGong = "Challenges/challenge_trigger_gong";

	public static string challengeWoodChest = "Challenges/challenge_wood_chest";

	public static string challengeDismantleTraps = "Challenges/challenge_dismantle_trap";

	public static string saveGameFilename
	{
		get
		{
			return Path.Combine(Application.persistentDataPath, "savegame.txt");
		}
	}
}
