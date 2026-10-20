using System;
using UnityEngine;

public static class Easing
{
	public enum Function
	{
		Linear = 0,
		QuadraticEaseIn = 1,
		QuadraticEaseOut = 2,
		QuadraticEaseInOut = 3,
		CubicEaseIn = 4,
		CubicEaseOut = 5,
		CubicEaseInOut = 6,
		QuarticEaseIn = 7,
		QuarticEaseOut = 8,
		QuarticEaseInOut = 9,
		QuinticEaseIn = 10,
		QuinticEaseOut = 11,
		QuinticEaseInOut = 12,
		SineEaseIn = 13,
		SineEaseOut = 14,
		SineEaseInOut = 15,
		CircularEaseIn = 16,
		CircularEaseOut = 17,
		CircularEaseInOut = 18,
		ExponentialEaseIn = 19,
		ExponentialEaseOut = 20,
		ExponentialEaseInOut = 21,
		ElasticEaseIn = 22,
		ElasticEaseOut = 23,
		ElasticEaseInOut = 24,
		BackEaseIn = 25,
		BackEaseOut = 26,
		BackEaseInOut = 27,
		BounceEaseIn = 28,
		BounceEaseOut = 29,
		BounceEaseInOut = 30
	}

	private const float PI = (float)Math.PI;

	private const float HALFPI = (float)Math.PI / 2f;

	public static float Interpolate(float p, Function function)
	{
		switch (function)
		{
		default:
			return Linear(p);
		case Function.QuadraticEaseOut:
			return QuadraticEaseOut(p);
		case Function.QuadraticEaseIn:
			return QuadraticEaseIn(p);
		case Function.QuadraticEaseInOut:
			return QuadraticEaseInOut(p);
		case Function.CubicEaseIn:
			return CubicEaseIn(p);
		case Function.CubicEaseOut:
			return CubicEaseOut(p);
		case Function.CubicEaseInOut:
			return CubicEaseInOut(p);
		case Function.QuarticEaseIn:
			return QuarticEaseIn(p);
		case Function.QuarticEaseOut:
			return QuarticEaseOut(p);
		case Function.QuarticEaseInOut:
			return QuarticEaseInOut(p);
		case Function.QuinticEaseIn:
			return QuinticEaseIn(p);
		case Function.QuinticEaseOut:
			return QuinticEaseOut(p);
		case Function.QuinticEaseInOut:
			return QuinticEaseInOut(p);
		case Function.SineEaseIn:
			return SineEaseIn(p);
		case Function.SineEaseOut:
			return SineEaseOut(p);
		case Function.SineEaseInOut:
			return SineEaseInOut(p);
		case Function.CircularEaseIn:
			return CircularEaseIn(p);
		case Function.CircularEaseOut:
			return CircularEaseOut(p);
		case Function.CircularEaseInOut:
			return CircularEaseInOut(p);
		case Function.ExponentialEaseIn:
			return ExponentialEaseIn(p);
		case Function.ExponentialEaseOut:
			return ExponentialEaseOut(p);
		case Function.ExponentialEaseInOut:
			return ExponentialEaseInOut(p);
		case Function.ElasticEaseIn:
			return ElasticEaseIn(p);
		case Function.ElasticEaseOut:
			return ElasticEaseOut(p);
		case Function.ElasticEaseInOut:
			return ElasticEaseInOut(p);
		case Function.BackEaseIn:
			return BackEaseIn(p);
		case Function.BackEaseOut:
			return BackEaseOut(p);
		case Function.BackEaseInOut:
			return BackEaseInOut(p);
		case Function.BounceEaseIn:
			return BounceEaseIn(p);
		case Function.BounceEaseOut:
			return BounceEaseOut(p);
		case Function.BounceEaseInOut:
			return BounceEaseInOut(p);
		}
	}

	public static float Linear(float p)
	{
		return p;
	}

	public static float QuadraticEaseIn(float p)
	{
		return p * p;
	}

	public static float QuadraticEaseOut(float p)
	{
		return 0f - p * (p - 2f);
	}

	public static float QuadraticEaseInOut(float p)
	{
		if (p < 0.5f)
		{
			return 2f * p * p;
		}
		return -2f * p * p + 4f * p - 1f;
	}

	public static float CubicEaseIn(float p)
	{
		return p * p * p;
	}

	public static float CubicEaseOut(float p)
	{
		float num = p - 1f;
		return num * num * num + 1f;
	}

	public static float CubicEaseInOut(float p)
	{
		if (p < 0.5f)
		{
			return 4f * p * p * p;
		}
		float num = 2f * p - 2f;
		return 0.5f * num * num * num + 1f;
	}

	public static float QuarticEaseIn(float p)
	{
		return p * p * p * p;
	}

	public static float QuarticEaseOut(float p)
	{
		float num = p - 1f;
		return num * num * num * (1f - p) + 1f;
	}

	public static float QuarticEaseInOut(float p)
	{
		if (p < 0.5f)
		{
			return 8f * p * p * p * p;
		}
		float num = p - 1f;
		return -8f * num * num * num * num + 1f;
	}

	public static float QuinticEaseIn(float p)
	{
		return p * p * p * p * p;
	}

	public static float QuinticEaseOut(float p)
	{
		float num = p - 1f;
		return num * num * num * num * num + 1f;
	}

	public static float QuinticEaseInOut(float p)
	{
		if (p < 0.5f)
		{
			return 16f * p * p * p * p * p;
		}
		float num = 2f * p - 2f;
		return 0.5f * num * num * num * num * num + 1f;
	}

	public static float SineEaseIn(float p)
	{
		return Mathf.Sin((p - 1f) * ((float)Math.PI / 2f)) + 1f;
	}

	public static float SineEaseOut(float p)
	{
		return Mathf.Sin(p * ((float)Math.PI / 2f));
	}

	public static float SineEaseInOut(float p)
	{
		return 0.5f * (1f - Mathf.Cos(p * (float)Math.PI));
	}

	public static float CircularEaseIn(float p)
	{
		return 1f - Mathf.Sqrt(1f - p * p);
	}

	public static float CircularEaseOut(float p)
	{
		return Mathf.Sqrt((2f - p) * p);
	}

	public static float CircularEaseInOut(float p)
	{
		if (p < 0.5f)
		{
			return 0.5f * (1f - Mathf.Sqrt(1f - 4f * (p * p)));
		}
		return 0.5f * (Mathf.Sqrt((0f - (2f * p - 3f)) * (2f * p - 1f)) + 1f);
	}

	public static float ExponentialEaseIn(float p)
	{
		return (p != 0f) ? Mathf.Pow(2f, 10f * (p - 1f)) : p;
	}

	public static float ExponentialEaseOut(float p)
	{
		return (p != 1f) ? (1f - Mathf.Pow(2f, -10f * p)) : p;
	}

	public static float ExponentialEaseInOut(float p)
	{
		if ((double)p == 0.0 || (double)p == 1.0)
		{
			return p;
		}
		if (p < 0.5f)
		{
			return 0.5f * Mathf.Pow(2f, 20f * p - 10f);
		}
		return -0.5f * Mathf.Pow(2f, -20f * p + 10f) + 1f;
	}

	public static float ElasticEaseIn(float p)
	{
		return Mathf.Sin(20.420353f * p) * Mathf.Pow(2f, 10f * (p - 1f));
	}

	public static float ElasticEaseOut(float p)
	{
		return Mathf.Sin(-20.420353f * (p + 1f)) * Mathf.Pow(2f, -10f * p) + 1f;
	}

	public static float ElasticEaseInOut(float p)
	{
		if (p < 0.5f)
		{
			return 0.5f * Mathf.Sin(20.420353f * (2f * p)) * Mathf.Pow(2f, 10f * (2f * p - 1f));
		}
		return 0.5f * (Mathf.Sin(-20.420353f * (2f * p - 1f + 1f)) * Mathf.Pow(2f, -10f * (2f * p - 1f)) + 2f);
	}

	public static float BackEaseIn(float p)
	{
		return p * p * p - p * Mathf.Sin(p * (float)Math.PI);
	}

	public static float BackEaseOut(float p)
	{
		float num = 1f - p;
		return 1f - (num * num * num - num * Mathf.Sin(num * (float)Math.PI));
	}

	public static float BackEaseInOut(float p)
	{
		if (p < 0.5f)
		{
			float num = 2f * p;
			return 0.5f * (num * num * num - num * Mathf.Sin(num * (float)Math.PI));
		}
		float num2 = 1f - (2f * p - 1f);
		return 0.5f * (1f - (num2 * num2 * num2 - num2 * Mathf.Sin(num2 * (float)Math.PI))) + 0.5f;
	}

	public static float BounceEaseIn(float p)
	{
		return 1f - BounceEaseOut(1f - p);
	}

	public static float BounceEaseOut(float p)
	{
		if (p < 372f / 1023f)
		{
			return 121f * p * p / 16f;
		}
		if (p < 744f / 1023f)
		{
			return 9.075f * p * p - 9.9f * p + 3.4f;
		}
		if (p < 0.9f)
		{
			return 12.066482f * p * p - 19.635458f * p + 8.898061f;
		}
		return 10.8f * p * p - 20.52f * p + 10.72f;
	}

	public static float BounceEaseInOut(float p)
	{
		if (p < 0.5f)
		{
			return 0.5f * BounceEaseIn(p * 2f);
		}
		return 0.5f * BounceEaseOut(p * 2f - 1f) + 0.5f;
	}
}
