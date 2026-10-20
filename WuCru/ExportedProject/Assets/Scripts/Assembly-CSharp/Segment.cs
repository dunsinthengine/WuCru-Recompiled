using UnityEngine;

public class Segment
{
	public EndPoint left;

	public EndPoint right;

	public Segment()
	{
		left = new EndPoint();
		right = new EndPoint();
		left.segment = this;
		right.segment = this;
		left.begin = true;
		right.begin = false;
	}

	public bool InFrontOf(Segment other, Vector3 point)
	{
		bool flag = IsPointLeftOf(Vector3.Lerp(other.left.position, other.right.position, 0.01f));
		bool flag2 = IsPointLeftOf(Vector3.Lerp(other.right.position, other.left.position, 0.01f));
		bool flag3 = IsPointLeftOf(point);
		bool flag4 = other.IsPointLeftOf(Vector3.Lerp(left.position, right.position, 0.01f));
		bool flag5 = other.IsPointLeftOf(Vector3.Lerp(right.position, left.position, 0.01f));
		bool flag6 = other.IsPointLeftOf(point);
		if (flag == flag2 && flag2 != flag3)
		{
			return true;
		}
		return false;
	}

	public bool IsPointLeftOf(Vector3 point)
	{
		float num = (right.position.x - left.position.x) * (point.z - left.position.z) - (right.position.z - left.position.z) * (point.x - left.position.x);
		return num > 0f;
	}
}
