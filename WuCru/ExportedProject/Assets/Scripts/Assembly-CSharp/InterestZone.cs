using UnityEngine;

public class InterestZone : MonoBehaviour
{
	public LevelPoint center;

	public LevelPoint extent;

	private InGameCam inGameCam;

	private BoxCollider boxCollider;

	private void Awake()
	{
		inGameCam = InGameCam._pInstance;
	}

	private void Start()
	{
		boxCollider = GetComponent<BoxCollider>();
		boxCollider.size = new Vector3((float)extent.col + 2f, 1f, (float)extent.row + 2f);
		base.gameObject.transform.position = new Vector3(center.col, (float)GameController.Instance.CurrentLevel.Nodes[center.row, center.col].Elevation + 0.5f, center.row);
	}

	private void Update()
	{
		if (inGameCam == null)
		{
			inGameCam = InGameCam._pInstance;
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		Character component = collider.GetComponent<Character>();
		if (component != null && component.isPlayer)
		{
			inGameCam.OnInterestZoneEnter(this);
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		Character component = collider.GetComponent<Character>();
		if (component != null && component.isPlayer)
		{
			inGameCam.OnInterestZoneExit(this);
		}
	}
}
