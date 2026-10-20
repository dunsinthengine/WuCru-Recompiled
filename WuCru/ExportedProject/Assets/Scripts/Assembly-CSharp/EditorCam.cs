using UnityEngine;

public class EditorCam : MonoBehaviour
{
	public Camera cam;

	public float movementSpeed = 2.5f;

	public float sprintFactor = 5f;

	public float mouseSensitivityX = 3f;

	public float mouseSensitivityY = 3f;

	private float minY = -60f;

	private float maxY = 60f;

	private void Start()
	{
		cam = Camera.main;
	}

	private void Update()
	{
		float num = movementSpeed;
		if (Input.GetKey(KeyCode.LeftShift))
		{
			num *= sprintFactor;
		}
		if (Input.GetKey(KeyCode.W))
		{
			cam.transform.position += cam.transform.forward * Time.deltaTime * num;
		}
		if (Input.GetKey(KeyCode.S))
		{
			cam.transform.position -= cam.transform.forward * Time.deltaTime * num;
		}
		if (Input.GetKey(KeyCode.A))
		{
			cam.transform.position -= cam.transform.right * Time.deltaTime * num;
		}
		if (Input.GetKey(KeyCode.D))
		{
			cam.transform.position += cam.transform.right * Time.deltaTime * num;
		}
		if (!Input.GetMouseButton(1))
		{
			return;
		}
		float y = cam.transform.localEulerAngles.y + Input.GetAxis("Mouse X") * mouseSensitivityX;
		float num2 = cam.transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * mouseSensitivityY;
		if (num2 < 180f && num2 > maxY)
		{
			num2 = maxY;
		}
		if (num2 > 180f)
		{
			float num3 = num2 - 360f;
			if (num3 < minY)
			{
				num3 = minY;
			}
			num2 = num3 + 360f;
		}
		cam.transform.localEulerAngles = new Vector3(num2, y, 0f);
	}
}
