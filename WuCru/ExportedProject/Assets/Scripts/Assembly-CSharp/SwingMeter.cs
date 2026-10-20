using System.Collections;
using Fabric;
using UnityEngine;
using UnityEngine.UI;

public class SwingMeter : MonoBehaviour
{
	public delegate void OnClickDelegate(bool success, float amount);

	public delegate void OnSuccessHitStartDelegate();

	public Canvas canvas;

	public Image imageBG;

	public Image imagePointer;

	public Image imageMarkerRed;

	public Image imageMarkerGreen;

	private float windowFactor = 0.45f;

	private float width;

	private bool movingDown;

	private float startTime;

	private float minSpeed = 100f;

	private float maxSpeed = 800f;

	private float speed;

	private float leftX;

	private float rightX;

	private float swingMeterLowPassMin = 400f;

	private float swingMeterLowPassMax = 1500f;

	public OnClickDelegate OnClick;

	public OnSuccessHitStartDelegate OnSuccessHitStart;

	public bool Visible
	{
		get
		{
			return canvas.gameObject.activeSelf;
		}
	}

	private void Awake()
	{
		width = imageMarkerRed.rectTransform.sizeDelta.x * 0.88f;
		leftX = imageMarkerRed.transform.localPosition.x - width / 2f;
		rightX = imageMarkerRed.transform.localPosition.x + width / 2f;
		imageMarkerGreen.fillAmount = windowFactor;
		imagePointer.transform.localPosition = new Vector3(leftX, imagePointer.transform.localPosition.y, imagePointer.transform.localPosition.z);
		Show(false);
	}

	public void Show(bool visible)
	{
		if (GamePlayer.Instance.State != GamePlayerState.Exploring)
		{
			canvas.gameObject.SetActive(false);
			if (OnClick != null)
			{
				OnClick(false, 0f);
			}
			return;
		}
		canvas.gameObject.SetActive(visible);
		if (visible)
		{
			movingDown = false;
			startTime = Time.time;
			speed = minSpeed;
			imageMarkerGreen.fillAmount = windowFactor;
			imagePointer.transform.localPosition = new Vector3(leftX, imagePointer.transform.localPosition.y, imagePointer.transform.localPosition.z);
			GamePlayer.Instance.Player.Character.EquipWeapon(true);
			GamePlayer.Instance.Player.Character.ResetPath();
			GamePlayer.Instance.Player.Character.Animator.SetTrigger(TriggerHash.swingHitPrepare);
			AudioPlayer.PlaySound("ui/swingmeter");
		}
		else
		{
			AudioPlayer.StopSound("ui/swingmeter");
		}
	}

	private void Update()
	{
		if (!canvas.gameObject.activeSelf || ((!(GameController.Instance.currentState != null) || !(GameController.Instance.currentState is GameControllerStatePlaying)) && (!(EditorStateMachine.Instance != null) || EditorStateMachine.Instance.CurrentState.GetType() != typeof(Playing))) || ((!(GameController.Instance.currentState != null) || GameController.Instance.currentState.GetType() != typeof(GameControllerStatePlaying) || (GameController.Instance.currentState as GameControllerStatePlaying).IsConfirmingQuit) && (!(EditorStateMachine.Instance != null) || EditorStateMachine.Instance.CurrentState.GetType() != typeof(Playing))))
		{
			return;
		}
		if (GamePlayer.Instance.State == GamePlayerState.Combat)
		{
			Debug.Log("Hide, due to combat");
			Show(false);
			if (OnClick != null)
			{
				OnClick(false, 0f);
			}
			return;
		}
		float x = imagePointer.transform.localPosition.x;
		float num = (x - leftX) / (rightX - leftX);
		speed = minSpeed + num * (maxSpeed - minSpeed);
		if (movingDown)
		{
			if (x <= leftX)
			{
				movingDown = false;
				x = leftX;
			}
			imagePointer.transform.localPosition = new Vector3(Mathf.Clamp(x - speed * Time.deltaTime, leftX, rightX), imagePointer.transform.localPosition.y, imagePointer.transform.localPosition.z);
		}
		else
		{
			if (x >= rightX)
			{
				movingDown = true;
				x = rightX;
			}
			imagePointer.transform.localPosition = new Vector3(Mathf.Clamp(x + speed * Time.deltaTime, leftX, rightX), imagePointer.transform.localPosition.y, imagePointer.transform.localPosition.z);
		}
		EventManager.Instance.SetDSPParameter("ui/swingmeter", DSPType.LowPass, "CutoffFrequency", swingMeterLowPassMin + Mathf.Clamp01(num) * (swingMeterLowPassMax - swingMeterLowPassMin), 0f, 0.5f, base.gameObject);
		if (!Input.GetMouseButtonDown(0))
		{
			return;
		}
		StartCoroutine(DelayedUnequipWeapon());
		Show(false);
		if (num >= 1f - windowFactor)
		{
			if (OnClick != null)
			{
				OnClick(true, num);
			}
			if (OnSuccessHitStart != null)
			{
				OnSuccessHitStart();
			}
			GamePlayer.Instance.Player.Character.Animator.SetTrigger(TriggerHash.swingHitSucceed);
		}
		else
		{
			if (OnClick != null)
			{
				OnClick(false, 0f);
			}
			GamePlayer.Instance.Player.Character.Animator.SetTrigger(TriggerHash.swingHitFail);
		}
	}

	private IEnumerator DelayedUnequipWeapon()
	{
		yield return new WaitForSeconds(0.2f);
		GamePlayer.Instance.Player.Character.EquipWeapon(false);
	}
}
