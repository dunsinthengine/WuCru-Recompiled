using UnityEngine;

namespace Cape.Utils.IO
{
	public static class DeviceRotation
	{
		private static bool gyroInitialized;

		public static bool HasGyroscope
		{
			get
			{
				return SystemInfo.supportsGyroscope;
			}
		}

		private static void InitGyro()
		{
			if (HasGyroscope)
			{
				Input.gyro.enabled = true;
				Input.gyro.updateInterval = 0.0167f;
			}
			gyroInitialized = true;
		}

		private static Quaternion ReadGyroscopeRotation()
		{
			return new Quaternion(0.5f, 0.5f, -0.5f, 0.5f) * Input.gyro.attitude * new Quaternion(0f, 0f, 1f, 0f);
		}

		public static Quaternion GetRotation()
		{
			if (!gyroInitialized)
			{
				InitGyro();
			}
			return (!HasGyroscope) ? Quaternion.identity : ReadGyroscopeRotation();
		}

		public static float GetAngleByAxis(Vector3 axis)
		{
			Quaternion rotation = GetRotation();
			Quaternion quaternion = Quaternion.Inverse(Quaternion.FromToRotation(axis, rotation * axis));
			Vector3 eulerAngles = (quaternion * rotation).eulerAngles;
			float result = eulerAngles.z;
			if (axis == Vector3.up)
			{
				result = eulerAngles.y;
			}
			if (axis == Vector3.right)
			{
				result = ((!(eulerAngles.y > 90f) || !(eulerAngles.y < 270f)) ? eulerAngles.x : (180f - eulerAngles.x));
			}
			return result;
		}
	}
}
