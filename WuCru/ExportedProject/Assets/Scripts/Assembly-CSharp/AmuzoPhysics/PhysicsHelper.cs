using UnityEngine;

namespace AmuzoPhysics
{
	public static class PhysicsHelper
	{
		public static float ComputeRequiredImpulse(float massA, float massB, float velA, float velB, float wantSpeedDelta)
		{
			return (wantSpeedDelta - (velA - velB)) / (1f / massA + 1f / massB);
		}

		public static float ComputeCollisionImpulse(float massA, float massB, float velA, float velB, float bounce)
		{
			return ComputeRequiredImpulse(massA, massB, velA, velB, (0f - bounce) * (velA - velB));
		}

		public static float ComputeFrictionImpulse(float massA, float massB, float velA, float velB, float normalImpulse, float frictionCoeff)
		{
			float value = ComputeRequiredImpulse(massA, massB, velA, velB, 0f);
			float num = Mathf.Abs(frictionCoeff * normalImpulse);
			return Mathf.Clamp(value, 0f - num, num);
		}

		public static bool DoBoundsCollisionTest(Bounds boundsA, Bounds boundsB, out Vector3 normal, out float penetration)
		{
			Vector3 min = boundsA.min;
			Vector3 max = boundsA.max;
			Vector3 min2 = boundsB.min;
			Vector3 max2 = boundsB.max;
			float num = max2.y - min.y;
			float num2 = max.y - min2.y;
			float num3 = max.x - min2.x;
			float num4 = max2.x - min.x;
			float num5 = max2.z - min.z;
			float num6 = max.z - min2.z;
			normal = Vector3.zero;
			penetration = Mathf.Min(num, num2, num3, num4, num5, num6);
			if (penetration == num)
			{
				normal = Vector3.up;
			}
			else if (penetration == num2)
			{
				normal = -Vector3.up;
			}
			else if (penetration == num3)
			{
				normal = -Vector3.right;
			}
			else if (penetration == num4)
			{
				normal = Vector3.right;
			}
			else if (penetration == num5)
			{
				normal = Vector3.forward;
			}
			else if (penetration == num6)
			{
				normal = -Vector3.forward;
			}
			return penetration >= 0f;
		}
	}
}
