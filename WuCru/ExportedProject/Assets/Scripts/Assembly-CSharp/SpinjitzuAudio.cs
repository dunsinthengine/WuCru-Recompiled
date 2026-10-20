using UnityEngine;

public class SpinjitzuAudio : MonoBehaviour
{
	[Header("Audio")]
	[SerializeField]
	private string spinjitzuAudioEvent;

	public string SpinjitzuAudioEvent
	{
		get
		{
			return spinjitzuAudioEvent;
		}
		set
		{
			spinjitzuAudioEvent = value;
		}
	}
}
