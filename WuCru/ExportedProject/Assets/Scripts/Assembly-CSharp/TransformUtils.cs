using UnityEngine;

public static class TransformUtils
{
	public static TransformData Clone(this Transform transform)
	{
		return new TransformData
		{
			position = transform.position,
			localPosition = transform.localPosition,
			rotation = transform.rotation,
			localRotation = transform.localRotation,
			localScale = transform.localScale,
			parent = transform.parent
		};
	}
}
