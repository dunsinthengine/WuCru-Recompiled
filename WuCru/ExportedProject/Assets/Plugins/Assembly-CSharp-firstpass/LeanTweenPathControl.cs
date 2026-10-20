using UnityEngine;

[ExecuteInEditMode]
public class LeanTweenPathControl : MonoBehaviour
{
	private Vector3 lastPos;

	private Vector3[] lastPosControl;

	public LeanTweenPathControl[] controlRef;

	public int i;

	public bool isControl;

	public bool isBezier;

	private static Material matPoints;

	private static Material matControls;

	public void createMaterials()
	{
		if (!matPoints)
		{
			matPoints = new Material(Shader.Find("GUI/Text Shader"));
			matPoints.color = Color.cyan;
			matPoints.hideFlags = HideFlags.HideAndDontSave;
			matControls = new Material(Shader.Find("GUI/Text Shader"));
			matControls.color = Color.white;
			matControls.hideFlags = HideFlags.HideAndDontSave;
		}
	}

	private void OnEnable()
	{
	}

	private void OnApplicationFocus()
	{
		OnDisable();
	}

	private void OnBecameInvisible()
	{
		OnDisable();
	}

	private void OnDisable()
	{
		matPoints = null;
		matControls = null;
	}

	public void init(LeanTweenPathControl[] controlRef, int i, bool isControl)
	{
		this.isControl = isControl;
		this.i = i;
		createMaterials();
		base.gameObject.GetComponent<Renderer>().material = ((!isControl) ? matControls : matPoints);
		lastPos = base.transform.position;
		this.controlRef = controlRef;
		lastPosControl = new Vector3[2];
		for (int j = 0; j < controlRef.Length; j++)
		{
			if ((bool)controlRef[j])
			{
				lastPosControl[j] = controlRef[j].transform.position;
			}
		}
		isBezier = true;
	}

	public void init(int i)
	{
		this.i = i;
		createMaterials();
		base.gameObject.GetComponent<Renderer>().material = matPoints;
		isBezier = false;
	}

	private void OnDrawGizmos()
	{
		if (controlRef == null || controlRef.Length <= 0 || !controlRef[0])
		{
			return;
		}
		if (lastPosControl != null && lastPos != base.transform.position && lastPosControl[0] == controlRef[0].transform.position)
		{
			Vector3 vector = base.transform.position - lastPos;
			lastPos = base.transform.position;
			controlRef[0].transform.position += vector;
			lastPosControl[0] = controlRef[0].transform.position;
			if ((bool)controlRef[1])
			{
				controlRef[1].transform.position += vector;
				lastPosControl[1] = controlRef[1].transform.position;
			}
		}
		controlRef[0].transform.LookAt(base.transform.position);
		if ((bool)controlRef[1])
		{
			controlRef[1].transform.LookAt(base.transform.position);
		}
	}
}
