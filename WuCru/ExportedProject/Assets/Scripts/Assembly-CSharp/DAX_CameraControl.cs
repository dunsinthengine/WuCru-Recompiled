using UnityEngine;

public class DAX_CameraControl : MonoBehaviour
{
	public float sensitivityX = 8f;

	public float sensitivityY = 8f;

	private float mHdg;

	private float mPitch;

	private void Start()
	{
	}

	private void Update()
	{
		float num = 0f;
		float axisRaw = Input.GetAxisRaw("Horizontal");
		num = Input.GetAxisRaw("Vertical");
		base.transform.position += base.transform.right * (sensitivityX * axisRaw * 0.33f);
		base.transform.position += base.transform.forward * (sensitivityY * num * 0.33f);
		if (Input.GetMouseButton(2) || Input.GetMouseButton(0))
		{
			float aVal = Input.GetAxis("Mouse X") * sensitivityX;
			float num2 = Input.GetAxis("Mouse Y") * sensitivityY;
			if (Input.GetMouseButton(2) && Input.GetMouseButton(0))
			{
				Strafe(aVal);
				ChangeHeight(num2);
			}
			else if (Input.GetMouseButton(2))
			{
				MoveForwards(num2);
				ChangeHeading(aVal);
			}
			else if (Input.GetMouseButton(0))
			{
				ChangeHeading(aVal);
				ChangePitch(0f - num2);
			}
		}
	}

	private void MoveForwards(float aVal)
	{
		Vector3 forward = base.transform.forward;
		forward.y = 0f;
		forward.Normalize();
		base.transform.position += aVal * forward;
	}

	private void Strafe(float aVal)
	{
		base.transform.position += aVal * base.transform.right;
	}

	private void ChangeHeight(float aVal)
	{
		base.transform.position += aVal * Vector3.up;
	}

	private void ChangeHeading(float aVal)
	{
		mHdg += aVal;
		WrapAngle(ref mHdg);
		base.transform.localEulerAngles = new Vector3(mPitch, mHdg, 0f);
	}

	private void ChangePitch(float aVal)
	{
		mPitch += aVal;
		WrapAngle(ref mPitch);
		base.transform.localEulerAngles = new Vector3(mPitch, mHdg, 0f);
	}

	public static void WrapAngle(ref float angle)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
	}
}
