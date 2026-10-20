using UnityEngine;

public class PlayCam : MonoBehaviour
{
	private Player player;

	private Vector3 aimPosition;

	private Vector3 aimPosition_target;

	private Vector3 aimPosition_velocity;

	private Vector3 lookAt;

	private float smoothTime;

	private float maxSmoothTime = 0.4f;

	private float minSmoothTime = 0.2f;

	private float smoothTimeDecay = 0.15f;

	public Vector3 aimPositionOffset;

	public Vector3 aimPositionOffsetCombat;

	public float zoomDistance = 18f;

	public float zoomDistanceCombat = 12f;

	public float height = 9.1f;

	public float heightCombat = 11f;

	private bool wasMoving;

	private void Start()
	{
		aimPositionOffset = new Vector3(3.5f, 2.2f, 0f);
		aimPositionOffsetCombat = new Vector3(3f, 6f, 0f);
		player = Object.FindObjectOfType<Player>();
		wasMoving = false;
		ForceReset();
	}

	public void ForceReset()
	{
		if (player != null)
		{
			Vector3 position = player.Character.transform.position;
			position.y = 0f;
			aimPosition_target = position;
			aimPosition = aimPosition_target;
			aimPosition_velocity = Vector3.zero;
			smoothTime = maxSmoothTime;
		}
	}

	private void Update()
	{
		if (player != null)
		{
			bool flag = player.Character.IsRunning();
			if (flag != wasMoving)
			{
				if (!flag)
				{
					smoothTime = maxSmoothTime;
				}
				wasMoving = flag;
			}
			smoothTime = Mathf.Max(minSmoothTime, smoothTime - smoothTimeDecay * Time.deltaTime);
			if (GamePlayer.Instance.State == GamePlayerState.Exploring)
			{
				Vector3 target = player.Character.transform.position - aimPositionOffset + new Vector3(1f, 0f, 0f);
				aimPosition = Vector3.SmoothDamp(aimPosition, target, ref aimPosition_velocity, smoothTime);
				MoveCamera();
			}
			else if (GamePlayer.Instance.State == GamePlayerState.Combat && GamePlayer.Instance.CombatManager.CurrentAttacker != null && GamePlayer.Instance.Player != null)
			{
				Vector3 target2 = player.Character.transform.position - aimPositionOffsetCombat;
				aimPosition = Vector3.SmoothDamp(aimPosition, target2, ref aimPosition_velocity, smoothTime);
				MoveCamera();
			}
		}
		else
		{
			player = Object.FindObjectOfType<Player>();
		}
	}

	private void MoveCamera()
	{
		float num = ((GamePlayer.Instance.State != GamePlayerState.Combat) ? zoomDistance : zoomDistanceCombat);
		Vector3 vector = ((GamePlayer.Instance.State != GamePlayerState.Combat) ? aimPositionOffset : aimPositionOffsetCombat);
		float num2 = ((GamePlayer.Instance.State != GamePlayerState.Combat) ? height : heightCombat);
		base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, aimPosition + new Vector3(0f, num2, 0f - Mathf.Sqrt(Mathf.Max(num * num - num2 * num2, 0f))), 8f * Time.deltaTime);
		lookAt = Vector3.Lerp(lookAt, aimPosition + vector, 8f * Time.deltaTime);
		base.transform.LookAt(lookAt);
	}
}
