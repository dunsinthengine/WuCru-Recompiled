using System;
using UnityEngine;

public class MathHelper
{
	public class SimEqSolver
	{
		private float[,] _lm;

		private int _nr;

		private int _nc;

		private int[] _rm;

		private int[] _cm;

		private int[] _cmi;

		private int _pr;

		private int _pc;

		private float _pv;

		private float[] _sv;

		private void Initialise(float[,] m)
		{
			_nr = m.GetLength(0);
			_nc = m.GetLength(1);
			_lm = new float[_nr, _nc];
			for (int i = 0; i < _nr; i++)
			{
				for (int j = 0; j < _nc; j++)
				{
					_lm[i, j] = m[i, j];
				}
			}
			_rm = new int[_nr];
			_cm = new int[_nc];
			_cmi = new int[_nc];
			for (int k = 0; k < _nr; k++)
			{
				_rm[k] = k;
			}
			for (int l = 0; l < _nc; l++)
			{
				_cm[l] = l;
			}
			_pr = -1;
			_pc = -1;
			_pv = 0f;
			_sv = new float[_nr];
		}

		private void FindPivot(int sr)
		{
			_pr = -1;
			_pc = -1;
			_pv = 0f;
			for (int i = sr; i < _nr; i++)
			{
				for (int j = sr; j < _nr; j++)
				{
					float num = _lm[_rm[i], _cm[j]];
					if (Mathf.Abs(num) > Mathf.Abs(_pv))
					{
						_pr = i;
						_pc = j;
						_pv = num;
					}
				}
			}
		}

		public float[] Solve(float[,] m)
		{
			Initialise(m);
			for (int i = 0; i < _nr; i++)
			{
				FindPivot(i);
				if (_pv == 0f)
				{
					return null;
				}
				if (_pr != i)
				{
					int num = _rm[i];
					_rm[i] = _rm[_pr];
					_rm[_pr] = num;
				}
				if (_pc != i)
				{
					int num2 = _cm[i];
					_cm[i] = _cm[_pc];
					_cm[_pc] = num2;
				}
				if (_pv != 0f)
				{
					_lm[_rm[i], _cm[i]] = 1f;
					for (int j = i + 1; j < _nc; j++)
					{
						_lm[_rm[i], _cm[j]] = _lm[_rm[i], _cm[j]] / _pv;
					}
				}
				for (int k = i + 1; k < _nr; k++)
				{
					float num3 = _lm[_rm[k], _cm[i]];
					if (num3 != 0f)
					{
						_lm[_rm[k], _cm[i]] = 0f;
						for (int j = i + 1; j < _nc; j++)
						{
							_lm[_rm[k], _cm[j]] = _lm[_rm[k], _cm[j]] - num3 * _lm[_rm[i], _cm[j]];
						}
					}
				}
			}
			for (int i = _nr - 1; i >= 0; i--)
			{
				for (int k = 0; k < i; k++)
				{
					float num3 = _lm[_rm[k], _cm[i]];
					if (num3 != 0f)
					{
						_lm[_rm[k], _cm[i]] = 0f;
						_lm[_rm[k], _cm[_nc - 1]] = _lm[_rm[k], _cm[_nc - 1]] - num3 * _lm[_rm[i], _cm[_nc - 1]];
					}
				}
			}
			for (int j = 0; j < _nc; j++)
			{
				_cmi[_cm[j]] = j;
			}
			for (int i = 0; i < _nr; i++)
			{
				_sv[i] = _lm[_rm[_cmi[i]], _cm[_nc - 1]];
			}
			return _sv;
		}
	}

	private static int[] _exponents = new int[11]
	{
		(int)Mathf.Pow(10f, 0f),
		(int)Mathf.Pow(10f, 1f),
		(int)Mathf.Pow(10f, 2f),
		(int)Mathf.Pow(10f, 3f),
		(int)Mathf.Pow(10f, 4f),
		(int)Mathf.Pow(10f, 5f),
		(int)Mathf.Pow(10f, 6f),
		(int)Mathf.Pow(10f, 7f),
		(int)Mathf.Pow(10f, 8f),
		(int)Mathf.Pow(10f, 9f),
		(int)Mathf.Pow(10f, 10f)
	};

	public static Vector3 RoundVector3ToPrecision(Vector3 v, int decimalPlaces)
	{
		return Vector3.right * RoundToPrecision(v.x, decimalPlaces) + Vector3.up * RoundToPrecision(v.y, decimalPlaces) + Vector3.forward * RoundToPrecision(v.z, decimalPlaces);
	}

	public static float RoundToPrecision(float n, int decimalPlaces)
	{
		float num = _exponents[decimalPlaces];
		return (float)(int)(n * num) / num;
	}

	public static double RoundToPrecision(double n, int decimalPlaces)
	{
		double num = _exponents[decimalPlaces];
		return (double)(int)(n * num) / num;
	}

	public static bool ApproxEquals(float a, float b, float tolerance)
	{
		return Mathf.Abs(a - b) <= tolerance;
	}

	public static bool ApproxEquals(Vector2 a, Vector2 b, float tolerance)
	{
		return (a - b).magnitude <= tolerance;
	}

	public static bool ApproxEquals(Vector3 a, Vector3 b, float tolerance)
	{
		return (a - b).magnitude <= tolerance;
	}

	public static float GetAngle(Vector3 v1, Vector3 v2, Vector3 normal)
	{
		return Mathf.Atan2(Vector3.Dot(normal, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * 57.29578f;
	}

	public static float GetArcDistance(Vector3 originPosition, Quaternion originRotation, Vector3 destinationPosition)
	{
		float circleRadius = GetCircleRadius(originPosition, originRotation, destinationPosition);
		if (float.IsInfinity(circleRadius) || float.IsNaN(circleRadius))
		{
			return float.PositiveInfinity;
		}
		Vector3 circleCenter = GetCircleCenter(originPosition, originRotation, destinationPosition);
		float num = Vector3.Angle(originPosition - circleCenter, destinationPosition - circleCenter);
		return (float)Math.PI * 2f * circleRadius * num / 360f;
	}

	public static Vector3 GetCircleCenter(Vector3 originPosition, Quaternion originRotation, Vector3 destinationPosition)
	{
		float circleRadius = GetCircleRadius(originPosition, originRotation, destinationPosition);
		float num = Mathf.Sign(GetAngle(originRotation * Vector3.forward, destinationPosition - originPosition, originRotation * Vector3.up));
		return originPosition + originRotation * Vector3.left * num * circleRadius;
	}

	public static float GetCircleRadius(Vector3 originPosition, Quaternion originRotation, Vector3 destinationPosition)
	{
		float f = (float)Math.PI / 180f * Vector3.Angle(originRotation * Vector3.forward, destinationPosition - originPosition);
		return Mathf.Abs(Vector3.Distance(originPosition, destinationPosition) * Mathf.Cos(f) / Mathf.Sin(f));
	}

	public static float GetCircleRadius(float maxSpeed, float grip, float gravity)
	{
		return Mathf.Abs(maxSpeed * maxSpeed / (grip * gravity));
	}

	public static float GetMaxSpeed(float radius, float grip, float gravity)
	{
		if (float.IsInfinity(radius) || float.IsNaN(radius))
		{
			radius = 10000f;
		}
		return Mathf.Sqrt(Mathf.Abs(radius * grip * gravity));
	}

	public static float GetMaxSpeed(Vector3 originPosition, Quaternion originRotation, Vector3 destinationPosition, float destinationSpeed, float grip, float deceleration, float gravity)
	{
		float circleRadius = GetCircleRadius(originPosition, originRotation, destinationPosition);
		float arcDistance = GetArcDistance(originPosition, originRotation, destinationPosition);
		return Mathf.Max(1f, Mathf.Min(GetMaxSpeed(circleRadius, grip, gravity), destinationSpeed + arcDistance * deceleration));
	}

	public static float Lerp(float x1, float y1, float x2, float y2, float x)
	{
		return y1 + (y2 - y1) / (x2 - x1) * (x - x1);
	}

	public static Vector2 LerpVector2(float x1, Vector2 y1, float x2, Vector2 y2, float x)
	{
		return y1 + (y2 - y1) / (x2 - x1) * (x - x1);
	}

	public static Vector3 LerpVector3(float x1, Vector3 y1, float x2, Vector3 y2, float x)
	{
		return y1 + (y2 - y1) / (x2 - x1) * (x - x1);
	}

	public static float StepSpring(ref float pos, ref float vel, float spring, float damping, float extForce, float mass, float dt)
	{
		float num = 0.5f * (mass * vel * vel + spring * pos * pos);
		float num2 = (extForce - spring * pos - damping * vel) / mass;
		pos += vel * dt + 0.5f * num2 * dt * dt;
		vel += num2 * dt;
		return 0.5f * (mass * vel * vel + spring * pos * pos) - num;
	}

	public static float LambdaDecay(float value, float lambda, float dt)
	{
		if (lambda <= 0f)
		{
			return value;
		}
		return value / Mathf.Pow(2f, dt / lambda);
	}

	public static void CombineTransforms(Vector3 posA, Quaternion rotA, Vector3 posB, Quaternion rotB, out Vector3 posR, out Quaternion rotR)
	{
		posR = posA + rotA * posB;
		rotR = rotA * rotB;
	}

	public static void InvertTransform(Vector3 pos, Quaternion rot, out Vector3 invPos, out Quaternion invRot)
	{
		invRot = Quaternion.Inverse(rot);
		invPos = invRot * -pos;
	}

	public static Vector3 ProjectVector3(Vector3 v, Vector3 axis)
	{
		return Vector3.Dot(v, axis) * axis;
	}

	public static Vector2 ProjectVector2(Vector2 v, Vector2 axis)
	{
		return Vector2.Dot(v, axis) * axis;
	}

	public static Vector3 ClipVector3(Vector3 v, Vector3 axis)
	{
		return v - ProjectVector3(v, axis);
	}

	public static Vector2 ClipVector2(Vector2 v, Vector2 axis)
	{
		return v - ProjectVector2(v, axis);
	}

	public static Vector3 RotateVector3(Vector3 v, Vector3 axis, float angle)
	{
		float num = Mathf.Cos(angle);
		float num2 = Mathf.Sin(angle);
		return num * v + (1f - num) * Vector3.Dot(v, axis) * axis - num2 * Vector3.Cross(v, axis);
	}

	public static Vector2 RotateVector2(Vector2 v, float angle)
	{
		float num = Mathf.Cos(angle);
		float num2 = Mathf.Sin(angle);
		return new Vector2(num * v.x - num2 * v.y, num2 * v.x + num * v.y);
	}

	public static Vector2 RotateExtents2(Vector2 e, float angle)
	{
		float num = Mathf.Abs(Mathf.Cos(angle));
		float num2 = Mathf.Abs(Mathf.Sin(angle));
		return new Vector2(num * e.x + num2 * e.y, num2 * e.x + num * e.y);
	}

	public static Vector3 TransformExtents(Transform tm, Vector3 extents)
	{
		Matrix4x4 localToWorldMatrix = tm.localToWorldMatrix;
		Vector3 result = default(Vector3);
		result.x = Mathf.Abs(extents.x * localToWorldMatrix[0, 0]) + Mathf.Abs(extents.y * localToWorldMatrix[1, 0]) + Mathf.Abs(extents.z * localToWorldMatrix[2, 0]);
		result.y = Mathf.Abs(extents.x * localToWorldMatrix[0, 1]) + Mathf.Abs(extents.y * localToWorldMatrix[1, 1]) + Mathf.Abs(extents.z * localToWorldMatrix[2, 1]);
		result.z = Mathf.Abs(extents.x * localToWorldMatrix[0, 2]) + Mathf.Abs(extents.y * localToWorldMatrix[1, 2]) + Mathf.Abs(extents.z * localToWorldMatrix[2, 2]);
		return result;
	}

	public static Bounds TransformBounds(Transform tm, Bounds bounds)
	{
		Bounds result = bounds;
		result.extents = TransformExtents(tm, bounds.extents);
		result.center = tm.TransformPoint(bounds.center);
		return result;
	}

	public static Vector3 TransformExtents(SimpleTransform tm, Vector3 extents)
	{
		Vector3 result = default(Vector3);
		result.x = Mathf.Abs(extents.x * tm[0, 0]) + Mathf.Abs(extents.y * tm[1, 0]) + Mathf.Abs(extents.z * tm[2, 0]);
		result.y = Mathf.Abs(extents.x * tm[0, 1]) + Mathf.Abs(extents.y * tm[1, 1]) + Mathf.Abs(extents.z * tm[2, 1]);
		result.z = Mathf.Abs(extents.x * tm[0, 2]) + Mathf.Abs(extents.y * tm[1, 2]) + Mathf.Abs(extents.z * tm[2, 2]);
		return result;
	}

	public static Bounds TransformBounds(SimpleTransform tm, Bounds bounds)
	{
		Bounds result = bounds;
		result.extents = TransformExtents(tm, bounds.extents);
		result.center = tm.TransformPoint(bounds.center);
		return result;
	}

	public static Vector3 InverseTransformExtents(Transform tm, Vector3 extents)
	{
		Matrix4x4 localToWorldMatrix = tm.localToWorldMatrix;
		Vector3 result = default(Vector3);
		result.x = Mathf.Abs(extents.x * localToWorldMatrix[0, 0]) + Mathf.Abs(extents.y * localToWorldMatrix[0, 1]) + Mathf.Abs(extents.z * localToWorldMatrix[0, 2]);
		result.y = Mathf.Abs(extents.x * localToWorldMatrix[1, 0]) + Mathf.Abs(extents.y * localToWorldMatrix[1, 1]) + Mathf.Abs(extents.z * localToWorldMatrix[1, 2]);
		result.z = Mathf.Abs(extents.x * localToWorldMatrix[2, 0]) + Mathf.Abs(extents.y * localToWorldMatrix[2, 1]) + Mathf.Abs(extents.z * localToWorldMatrix[2, 2]);
		return result;
	}

	public static Bounds InverseTransformBounds(Transform tm, Bounds bounds)
	{
		Bounds result = bounds;
		result.center = tm.InverseTransformPoint(bounds.center);
		result.extents = InverseTransformExtents(tm, bounds.extents);
		return result;
	}

	public static Vector3 InverseTransformExtents(SimpleTransform tm, Vector3 extents)
	{
		Vector3 result = default(Vector3);
		result.x = Mathf.Abs(extents.x * tm[0, 0]) + Mathf.Abs(extents.y * tm[0, 1]) + Mathf.Abs(extents.z * tm[0, 2]);
		result.y = Mathf.Abs(extents.x * tm[1, 0]) + Mathf.Abs(extents.y * tm[1, 1]) + Mathf.Abs(extents.z * tm[1, 2]);
		result.z = Mathf.Abs(extents.x * tm[2, 0]) + Mathf.Abs(extents.y * tm[2, 1]) + Mathf.Abs(extents.z * tm[2, 2]);
		return result;
	}

	public static Bounds InverseTransformBounds(SimpleTransform tm, Bounds bounds)
	{
		Bounds result = bounds;
		result.center = tm.InverseTransformPoint(bounds.center);
		result.extents = InverseTransformExtents(tm, bounds.extents);
		return result;
	}

	public static float ExtentInDirection(Vector2 extents, Vector2 direction)
	{
		return Mathf.Abs(extents.x * direction.x) + Mathf.Abs(extents.y * direction.y);
	}

	public static float ExtentInDirection(Vector3 extents, Vector3 direction)
	{
		return Mathf.Abs(extents.x * direction.x) + Mathf.Abs(extents.y * direction.y) + Mathf.Abs(extents.z * direction.z);
	}

	public static Plane TransformPlane(Transform tm, Plane plane)
	{
		Plane result = plane;
		if (tm != null)
		{
			Vector3 position = plane.normal * plane.distance;
			result.SetNormalAndPosition(tm.TransformDirection(plane.normal), tm.TransformPoint(position));
		}
		return result;
	}

	public static Plane InverseTransformPlane(Transform tm, Plane plane)
	{
		Plane result = plane;
		if (tm != null)
		{
			Vector3 direction = plane.normal * plane.distance;
			result.SetNormalAndPosition(tm.InverseTransformDirection(plane.normal), tm.InverseTransformDirection(direction));
		}
		return result;
	}

	public static float Vector2Cross(Vector2 a, Vector2 b)
	{
		return a.x * b.y - a.y * b.x;
	}

	public static Vector2 Vector2Perp(Vector2 a)
	{
		return new Vector2(0f - a.y, a.x);
	}

	public static Vector3 GeneratePerpVector3(Vector3 a)
	{
		Vector3 zero = Vector3.zero;
		float num = Mathf.Abs(a.x);
		float num2 = Mathf.Abs(a.y);
		float num3 = Mathf.Abs(a.z);
		return ((num > num2) ? ((!(num > num3)) ? Vector3.Cross(Vector3.up, a) : Vector3.Cross(Vector3.forward, a)) : ((!(num2 > num3)) ? Vector3.Cross(Vector3.up, a) : Vector3.Cross(Vector3.right, a))).normalized;
	}

	public static bool SolveQuadratic(float a, float b, float c, out float r1, out float r2)
	{
		if (a != 0f)
		{
			float num = b * b - 4f * a * c;
			if (num >= 0f)
			{
				r1 = (0f - b - Mathf.Sqrt(num)) / (2f * a);
				r2 = (0f - b + Mathf.Sqrt(num)) / (2f * a);
				return true;
			}
		}
		r1 = (r2 = 0f);
		return false;
	}

	public static float Wrap(float val, float closedMin, float openMax)
	{
		float num = openMax - closedMin;
		float num2 = (val - closedMin) % num + closedMin;
		if (num2 < closedMin)
		{
			num2 += num;
		}
		return num2;
	}

	public static int Wrap(int val, int closedMin, int openMax)
	{
		int num = openMax - closedMin;
		int num2 = (val - closedMin) % num + closedMin;
		if (num2 < closedMin)
		{
			num2 += num;
		}
		return num2;
	}

	public static float Wrap01(float val)
	{
		return Wrap(val, 0f, 1f);
	}

	public static bool IsInOpenRange(float val, float min, float max)
	{
		return val > min && val < max;
	}

	public static bool IsInClosedRange(float val, float min, float max)
	{
		return val >= min && val <= max;
	}

	public static bool IsBetween01(float val)
	{
		return IsInOpenRange(val, 0f, 1f);
	}
}
