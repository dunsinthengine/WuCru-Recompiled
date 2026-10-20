using UnityEngine;

public class DecorationForeground : MonoBehaviour
{
	public int decorationIndex { get; set; }

	public void ReplaceWith(GameObject rep)
	{
		GameObject gameObject = Object.Instantiate(rep, base.transform.position, base.transform.rotation);
		gameObject.transform.parent = base.transform.parent;
		gameObject.transform.localScale = base.transform.localScale;
		Object.Destroy(base.gameObject);
	}
}
