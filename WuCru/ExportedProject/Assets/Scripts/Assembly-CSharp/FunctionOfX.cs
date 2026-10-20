using System;
using System.Text;
using UnityEngine;

[Serializable]
public class FunctionOfX
{
	[Serializable]
	public class FunctionSegment
	{
		public float _multiplier;

		public int _powerInt;

		public bool _doUsePowerFloat;

		public float _powerFloat;
	}

	public class FunctionOfXDescriptionAttribute : PropertyAttribute
	{
	}

	public FunctionSegment[] _segments;

	[FunctionOfXDescription]
	public string _functionDescriptionString;

	private float returnValue;

	private float tempPow;

	private int x;

	private int y;

	public float Run(float forX)
	{
		returnValue = 0f;
		for (x = 0; x < _segments.Length; x++)
		{
			if (_segments[x]._doUsePowerFloat)
			{
				if (_segments[x]._powerFloat == 0f)
				{
					tempPow = 1f;
				}
				else if (_segments[x]._powerFloat == 1f)
				{
					tempPow = forX;
				}
				else
				{
					tempPow = Mathf.Pow(forX, _segments[x]._powerFloat);
				}
			}
			else
			{
				tempPow = 1f;
				for (y = 0; y < _segments[x]._powerInt; y++)
				{
					tempPow *= forX;
				}
			}
			returnValue += _segments[x]._multiplier * tempPow;
		}
		return returnValue;
	}

	public static string MakeFunctionDescriptionString(FunctionSegment[] segments)
	{
		if (segments == null || segments.Length == 0)
		{
			return "0";
		}
		Array.Sort(segments, sortSegmentByPower);
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < segments.Length; i++)
		{
			if (segments[i] == null)
			{
				continue;
			}
			if (segments[i]._multiplier >= 0f && i != 0)
			{
				stringBuilder.Append("+ ");
			}
			if (segments[i]._multiplier != 1f || (segments[i]._doUsePowerFloat && segments[i]._powerFloat == 0f) || (!segments[i]._doUsePowerFloat && segments[i]._powerInt == 0))
			{
				stringBuilder.Append(segments[i]._multiplier.ToString());
			}
			if ((segments[i]._doUsePowerFloat && segments[i]._powerFloat != 0f) || (!segments[i]._doUsePowerFloat && segments[i]._powerInt != 0))
			{
				stringBuilder.Append("x");
				if ((segments[i]._doUsePowerFloat && segments[i]._powerFloat != 1f) || (!segments[i]._doUsePowerFloat && segments[i]._powerInt != 1))
				{
					stringBuilder.Append("^");
					if (segments[i]._doUsePowerFloat)
					{
						stringBuilder.Append(segments[i]._powerFloat);
					}
					else
					{
						stringBuilder.Append(segments[i]._powerInt);
					}
				}
			}
			if (i != segments.Length - 1)
			{
				stringBuilder.Append(" ");
			}
		}
		return stringBuilder.ToString();
	}

	private static int sortSegmentByPower(FunctionSegment segA, FunctionSegment segB)
	{
		if (segA == null && segB == null)
		{
			return 0;
		}
		if (segA == null)
		{
			return 1;
		}
		if (segB == null)
		{
			return -1;
		}
		float num = ((!segA._doUsePowerFloat) ? ((float)segA._powerInt) : segA._powerFloat);
		float num2 = ((!segB._doUsePowerFloat) ? ((float)segB._powerInt) : segB._powerFloat);
		if (num == num2)
		{
			return 0;
		}
		return (num < num2) ? 1 : (-1);
	}
}
