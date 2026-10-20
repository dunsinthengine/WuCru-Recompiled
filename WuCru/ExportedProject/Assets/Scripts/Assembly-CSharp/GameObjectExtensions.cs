using UnityEngine;

public static class GameObjectExtensions
{
	public static void SetLayerRecursivly(this GameObject gameObject, string layerName)
	{
		int layer = LayerMask.NameToLayer(layerName);
		gameObject.SetLayerRecursivly(layer);
	}

	public static void SetLayerRecursivly(this GameObject gameObject, int layer)
	{
		Transform[] componentsInChildren = gameObject.GetComponentsInChildren<Transform>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.layer = layer;
		}
	}
}
