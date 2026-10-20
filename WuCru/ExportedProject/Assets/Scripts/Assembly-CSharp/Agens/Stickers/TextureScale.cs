using UnityEngine;

namespace Agens.Stickers
{
	public static class TextureScale
	{
		public static Texture2D ScaledResized(Texture2D src, int width, int height, Color backgroundColor, float fillPercentage, FilterMode mode = FilterMode.Trilinear, ScaleMode anchor = ScaleMode.ScaleToFit)
		{
			Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, false);
			if (src == null)
			{
				Debug.LogWarning("Source texture is null");
				return texture2D;
			}
			RenderTexture renderTexture = CreateScaledTexture(src, width, height, backgroundColor, fillPercentage, mode, anchor);
			Rect source = new Rect(0f, 0f, width, height);
			texture2D.ReadPixels(source, 0, 0, true);
			texture2D.Apply(false);
			RenderTexture.active = null;
			renderTexture.Release();
			return texture2D;
		}

		private static RenderTexture CreateScaledTexture(Texture2D src, int width, int height, Color backgroundColor, float fillPercentage, FilterMode fmode = FilterMode.Trilinear, ScaleMode scaleMode = ScaleMode.ScaleToFit)
		{
			src.filterMode = fmode;
			src.Apply(true);
			RenderTexture result = (RenderTexture.active = new RenderTexture(width, height, 32));
			GL.LoadPixelMatrix(0f, width, height, 0f);
			GL.Clear(true, true, backgroundColor);
			fillPercentage = ((scaleMode != ScaleMode.ScaleToFit) ? 1f : Mathf.Clamp01(fillPercentage));
			float num = (float)width * fillPercentage;
			float num2 = (float)height * fillPercentage;
			float x = ((float)width - num) / 2f;
			float y = ((float)height - num2) / 2f;
			DrawTexture(new Rect(x, y, num, num2), src, scaleMode);
			return result;
		}

		private static void DrawTexture(Rect position, Texture image, ScaleMode scaleMode, float imageAspect = 0f)
		{
			if ((double)imageAspect == 0.0)
			{
				imageAspect = (float)image.width / (float)image.height;
			}
			Rect outScreenRect = default(Rect);
			Rect outSourceRect = default(Rect);
			CalculateScaledTextureRects(position, scaleMode, imageAspect, ref outScreenRect, ref outSourceRect);
			Graphics.DrawTexture(outScreenRect, image, outSourceRect, 0, 0, 0, 0);
		}

		private static bool CalculateScaledTextureRects(Rect position, ScaleMode scaleMode, float imageAspect, ref Rect outScreenRect, ref Rect outSourceRect)
		{
			float num = position.width / position.height;
			bool result = false;
			switch (scaleMode)
			{
			case ScaleMode.ScaleToFit:
				if (num > imageAspect)
				{
					float num4 = imageAspect / num;
					outScreenRect = new Rect(position.xMin + (float)((double)position.width * (1.0 - (double)num4) * 0.5), position.yMin, num4 * position.width, position.height);
					outSourceRect = new Rect(0f, 0f, 1f, 1f);
					result = true;
				}
				else
				{
					float num5 = num / imageAspect;
					outScreenRect = new Rect(position.xMin, position.yMin + (float)((double)position.height * (1.0 - (double)num5) * 0.5), position.width, num5 * position.height);
					outSourceRect = new Rect(0f, 0f, 1f, 1f);
					result = true;
				}
				break;
			case ScaleMode.ScaleAndCrop:
				if (num > imageAspect)
				{
					float num2 = imageAspect / num;
					outScreenRect = position;
					outSourceRect = new Rect(0f, (float)((1.0 - (double)num2) * 0.5), 1f, num2);
					result = true;
				}
				else
				{
					float num3 = num / imageAspect;
					outScreenRect = position;
					outSourceRect = new Rect((float)(0.5 - (double)num3 * 0.5), 0f, num3, 1f);
					result = true;
				}
				break;
			case ScaleMode.StretchToFill:
				outScreenRect = position;
				outSourceRect = new Rect(0f, 0f, 1f, 1f);
				result = true;
				break;
			}
			return result;
		}
	}
}
