using System;
using UnityEngine;

namespace AmuzoEngine
{
	public class Easing
	{
		public enum EaseType
		{
			Linear = 0,
			EaseIn = 1,
			EaseOut = 2,
			EaseInOut = 3,
			EaseInSine = 4,
			EaseOutSine = 5,
			EaseInOutSine = 6,
			EaseInExpo = 7,
			EaseOutExpo = 8,
			EaseInOutExpo = 9,
			EaseInCircle = 10,
			EaseOutCircle = 11,
			EaseInOutCircle = 12,
			EaseInElastic = 13,
			EaseOutElastic = 14,
			EaseInOutElastic = 15,
			EaseInBack = 16,
			EaseOutBack = 17,
			EaseInOutBack = 18,
			EaseInBounce = 19,
			EaseOutBounce = 20,
			EaseInOutBounce = 21
		}

		public enum FadeType
		{
			None = 0,
			In = 1,
			Out = 2,
			InOut = 3
		}

		public static float Ease(EaseType easing, float time, float duration, float start, float end)
		{
			float num = CalculateTime(easing, time, duration);
			float num2 = end - start;
			return start + num2 * num;
		}

		public static Vector2 Ease(EaseType easing, float time, float duration, Vector2 start, Vector2 end)
		{
			float num = CalculateTime(easing, time, duration);
			Vector2 vector = end - start;
			return start + vector * num;
		}

		public static Vector3 Ease(EaseType easing, float time, float duration, Vector3 start, Vector3 end)
		{
			float num = CalculateTime(easing, time, duration);
			Vector3 vector = end - start;
			return start + vector * num;
		}

		public static Vector4 Ease(EaseType easing, float time, float duration, Vector4 start, Vector4 end)
		{
			float num = CalculateTime(easing, time, duration);
			Vector4 vector = end - start;
			return start + vector * num;
		}

		public static Color Ease(EaseType easing, float time, float duration, Color start, Color end)
		{
			return Color.Lerp(start, end, CalculateTime(easing, time, duration));
		}

		public static float Wave(EaseType easing, float time, float duration, float start, float max, int cycles = 1, FadeType fade = FadeType.None)
		{
			WaveTransformTime(ref time, ref duration, cycles, fade);
			return Ease(easing, time, duration, start, max);
		}

		public static Vector2 Wave(EaseType easing, float time, float duration, Vector2 start, Vector2 max, int cycles = 1, FadeType fade = FadeType.None)
		{
			WaveTransformTime(ref time, ref duration, cycles, fade);
			return Ease(easing, time, duration, start, max);
		}

		public static Vector3 Wave(EaseType easing, float time, float duration, Vector3 start, Vector3 max, int cycles = 1, FadeType fade = FadeType.None)
		{
			WaveTransformTime(ref time, ref duration, cycles, fade);
			return Ease(easing, time, duration, start, max);
		}

		public static Vector4 Wave(EaseType easing, float time, float duration, Vector4 start, Vector4 max, int cycles = 1, FadeType fade = FadeType.None)
		{
			WaveTransformTime(ref time, ref duration, cycles, fade);
			return Ease(easing, time, duration, start, max);
		}

		public static Color Wave(EaseType easing, float time, float duration, Color start, Color max, int cycles = 1, FadeType fade = FadeType.None)
		{
			WaveTransformTime(ref time, ref duration, cycles, fade);
			return Ease(easing, time, duration, start, max);
		}

		public static float FadeMultiplier(FadeType fade, float time, float duration)
		{
			switch (fade)
			{
			default:
				return 1f;
			case FadeType.In:
				return time / duration;
			case FadeType.Out:
				return 1f - time / duration;
			case FadeType.InOut:
				time *= 2f;
				if (time < duration)
				{
					return time / duration;
				}
				return 2f - time / duration;
			}
		}

		private static void WaveTransformTime(ref float time, ref float duration, int cycles, FadeType fade)
		{
			float num = FadeMultiplier(fade, time, duration);
			duration /= cycles;
			float num2 = duration / 2f;
			while (time >= duration)
			{
				time -= duration;
			}
			if (time > num2)
			{
				time = duration - time;
			}
			duration = num2;
			time *= num;
		}

		private static float CalculateTime(EaseType easing, float time, float duration)
		{
			switch (easing)
			{
			default:
				return time / duration;
			case EaseType.EaseIn:
				time /= duration;
				return time * time;
			case EaseType.EaseOut:
				time /= duration;
				return -1f * time * (time - 2f);
			case EaseType.EaseInOut:
				time /= duration / 2f;
				if (time < 1f)
				{
					return 0.5f * time * time;
				}
				return -0.5f * ((time - 1f) * (time - 3f) - 1f);
			case EaseType.EaseInSine:
				return 1f - Mathf.Cos(time / duration * ((float)Math.PI / 2f));
			case EaseType.EaseOutSine:
				return Mathf.Sin(time / duration * ((float)Math.PI / 2f));
			case EaseType.EaseInOutSine:
				return -0.5f * (Mathf.Cos((float)Math.PI * (time / duration)) - 1f);
			case EaseType.EaseInExpo:
				if (Mathf.Approximately(0f, time))
				{
					return 0f;
				}
				return Mathf.Pow(2f, 10f * (time / duration - 1f));
			case EaseType.EaseOutExpo:
				if (Mathf.Approximately(duration, time))
				{
					return 1f;
				}
				return 1f - Mathf.Pow(2f, -10f * (time / duration));
			case EaseType.EaseInOutExpo:
				if (Mathf.Approximately(0f, time))
				{
					return 0f;
				}
				if (Mathf.Approximately(duration, time))
				{
					return 1f;
				}
				time /= duration / 2f;
				if (time < 1f)
				{
					return 0.5f * Mathf.Pow(2f, 10f * (time - 1f));
				}
				return 0.5f * (2f - Mathf.Pow(2f, -10f * (time - 1f)));
			case EaseType.EaseInCircle:
				time /= duration;
				return 0f - (Mathf.Sqrt(1f - time * time) - 1f);
			case EaseType.EaseOutCircle:
				time /= duration;
				time--;
				return Mathf.Sqrt(1f - time * time);
			case EaseType.EaseInOutCircle:
				time /= duration / 2f;
				if (time < 1f)
				{
					return -0.5f * (Mathf.Sqrt(1f - time * time) - 1f);
				}
				time -= 2f;
				return 0.5f * (Mathf.Sqrt(1f - time * time) + 1f);
			case EaseType.EaseInElastic:
			{
				if (Mathf.Approximately(time, 0f))
				{
					return 0f;
				}
				time /= duration;
				if (Mathf.Approximately(time, 1f))
				{
					return 1f;
				}
				float num = duration * 0.3f;
				float num2 = num / 4f;
				time--;
				return 0f - Mathf.Pow(2f, 10f * time) * Mathf.Sin((time * duration - num2) * ((float)Math.PI * 2f) / num);
			}
			case EaseType.EaseOutElastic:
			{
				if (Mathf.Approximately(time, 0f))
				{
					return 0f;
				}
				time /= duration;
				if (Mathf.Approximately(time, 1f))
				{
					return 1f;
				}
				float num = duration * 0.3f;
				float num2 = num / 4f;
				return Mathf.Pow(2f, -10f * time) * Mathf.Sin((time * duration - num2) * ((float)Math.PI * 2f) / num) + 1f;
			}
			case EaseType.EaseInOutElastic:
			{
				if (Mathf.Approximately(time, 0f))
				{
					return 0f;
				}
				time /= duration / 2f;
				if (Mathf.Approximately(time, 2f))
				{
					return 1f;
				}
				float num = duration * 0.45000002f;
				float num2 = num / 4f;
				time--;
				if (time < 0f)
				{
					return -0.5f * (Mathf.Pow(2f, 10f * time) * Mathf.Sin((time * duration - num2) * ((float)Math.PI * 2f) / num));
				}
				return Mathf.Pow(2f, -10f * time) * Mathf.Sin((time * duration - num2) * ((float)Math.PI * 2f) / num) * 0.5f + 1f;
			}
			case EaseType.EaseInBack:
			{
				float num2 = 1.70158f;
				time /= duration;
				return time * time * ((num2 + 1f) * time - num2);
			}
			case EaseType.EaseOutBack:
			{
				float num2 = 1.70158f;
				time /= duration;
				time--;
				return time * time * ((num2 + 1f) * time + num2) + 1f;
			}
			case EaseType.EaseInOutBack:
			{
				float num2 = 2.5949094f;
				time /= duration / 2f;
				if (time < 1f)
				{
					return 0.5f * (time * time * ((num2 + 1f) * time - num2));
				}
				time -= 2f;
				return 0.5f * (time * time * ((num2 + 1f) * time + num2) + 2f);
			}
			case EaseType.EaseInBounce:
				return EaseInBounce(time, duration);
			case EaseType.EaseOutBounce:
				return EaseOutBounce(time, duration);
			case EaseType.EaseInOutBounce:
				if (time < duration / 2f)
				{
					return EaseInBounce(time * 2f, duration) * 0.5f;
				}
				return EaseOutBounce(time * 2f - duration, duration) * 0.5f + 0.5f;
			}
		}

		private static float EaseInBounce(float time, float duration)
		{
			return 1f - EaseOutBounce(duration - time, duration);
		}

		private static float EaseOutBounce(float time, float duration)
		{
			time /= duration;
			if (time < 372f / 1023f)
			{
				return 7.5625f * time * time;
			}
			if (time < 744f / 1023f)
			{
				time -= 558f / 1023f;
				return 7.5625f * time * time + 0.75f;
			}
			if (time < 930f / 1023f)
			{
				time -= 837f / 1023f;
				return 7.5625f * time * time + 0.9375f;
			}
			time -= 21f / 22f;
			return 7.5625f * time * time + 63f / 64f;
		}
	}
}
