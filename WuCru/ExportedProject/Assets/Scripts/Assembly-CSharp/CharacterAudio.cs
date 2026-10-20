using UnityEngine;

public class CharacterAudio : MonoBehaviour
{
	[Header("Audio")]
	public string footstepGrassAudioEvent;

	public string footstepSandAudioEvent;

	public string footstepSnowAudioEvent;

	public string footstepGravelAudioEvent;

	public string footstepLadderAudioEvent;

	public string dashAudioEvent;

	public string rollGrassAudioEvent;

	public string rollSandAudioEvent;

	public string rollSnowAudioEvent;

	public string rollGravelAudioEvent;

	public string playerSwordAttackAudioEvent = "player/combat/sword/hit/enemy";

	public string playerShurikenAttackAudioEvent = "player/combat/shuriken/attack/success";

	public string playerNunchucksAttackAudioEvent = "player/combat/nunchucks/attack/success";

	public string playerScytheAttackAudioEvent = "player/combat/scythe/attack/success";

	public string playerSwordParryAudioEvent = "player/sword/parry";

	public string playerShurikenParryAudioEvent = "player/combat/shuriken/attack/success";

	public string playerNunchucksParryAudioEvent = "player/combat/nunchucks/attack/success";

	public string playerScytheParryAudioEvent = "player/combat/scythe/attack/success";

	public void FootstepRunAudioEvent()
	{
		AudioPlayer.PlaySound(footstepGrassAudioEvent, base.gameObject);
	}

	public void FootstepLadderAudioEvent()
	{
		AudioPlayer.PlaySound(footstepLadderAudioEvent, base.gameObject);
	}

	public void RollAudioEvent()
	{
		AudioPlayer.PlaySound(rollGrassAudioEvent, base.gameObject);
	}

	public void DashAudioEvent()
	{
		AudioPlayer.PlaySound(dashAudioEvent, base.gameObject);
	}

	public void SwordAttackAudioEvent()
	{
		AudioPlayer.PlaySound(playerSwordAttackAudioEvent, base.gameObject);
	}

	public void ShurikenAttackAudioEvent()
	{
		AudioPlayer.PlaySound(playerShurikenAttackAudioEvent, base.gameObject);
	}

	public void NunchucksAttackAudioEvent()
	{
		AudioPlayer.PlaySound(playerNunchucksAttackAudioEvent, base.gameObject);
	}

	public void ScytheAttackAudioEvent()
	{
		AudioPlayer.PlaySound(playerScytheAttackAudioEvent, base.gameObject);
	}

	public void SwordParryAudioEvent()
	{
		AudioPlayer.PlaySound(playerSwordParryAudioEvent, base.gameObject);
	}

	public void ShurikenParryAudioEvent()
	{
		AudioPlayer.PlaySound(playerShurikenParryAudioEvent, base.gameObject);
	}

	public void NunchucksParryAudioEvent()
	{
		AudioPlayer.PlaySound(playerNunchucksParryAudioEvent, base.gameObject);
	}

	public void ScytheParryAudioEvent()
	{
		AudioPlayer.PlaySound(playerScytheParryAudioEvent, base.gameObject);
	}
}
