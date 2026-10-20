using UnityEngine;
using UnityEngine.UI;

public class VehicleTest : MonoBehaviour
{
	public GameObject[] vehicles;

	public Text fps;

	private void Update()
	{
		GameObject[] array = vehicles;
		foreach (GameObject gameObject in array)
		{
			gameObject.transform.Rotate(Vector3.up);
		}
		fps.text = (1f / Time.deltaTime).ToString("0.00");
	}
}
