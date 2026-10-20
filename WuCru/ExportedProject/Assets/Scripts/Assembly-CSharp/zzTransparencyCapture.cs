using System.IO;
using UnityEngine;

public class zzTransparencyCapture
{
	public static Texture2D capture(Rect pRect)
	{
		Camera main = Camera.main;
		CameraClearFlags clearFlags = main.clearFlags;
		Color backgroundColor = main.backgroundColor;
		main.clearFlags = CameraClearFlags.Color;
		main.backgroundColor = Color.black;
		main.Render();
		Texture2D texture2D = captureView(pRect);
		main.backgroundColor = Color.white;
		main.Render();
		Texture2D texture2D2 = captureView(pRect);
		for (int i = 0; i < texture2D2.width; i++)
		{
			for (int j = 0; j < texture2D2.height; j++)
			{
				Color pixel = texture2D.GetPixel(i, j);
				Color pixel2 = texture2D2.GetPixel(i, j);
				if (pixel != Color.clear)
				{
					texture2D2.SetPixel(i, j, getColor(pixel, pixel2));
				}
			}
		}
		texture2D2.Apply();
		Texture2D result = texture2D2;
		Object.DestroyImmediate(texture2D);
		main.backgroundColor = backgroundColor;
		main.clearFlags = clearFlags;
		return result;
	}

	public static Texture2D captureScreenshot()
	{
		return capture(new Rect(0f, 0f, Screen.width, Screen.height));
	}

	public static void captureScreenshot(string pFileName)
	{
		Texture2D texture2D = captureScreenshot();
		try
		{
			using (FileStream output = new FileStream(pFileName, FileMode.Create))
			{
				BinaryWriter binaryWriter = new BinaryWriter(output);
				binaryWriter.Write(texture2D.EncodeToPNG());
			}
		}
		finally
		{
			Object.DestroyImmediate(texture2D);
		}
	}

	private static Color getColor(Color pColorWhenBlack, Color pColorWhenWhite)
	{
		float alpha = getAlpha(pColorWhenBlack.r, pColorWhenWhite.r);
		return new Color(pColorWhenBlack.r / alpha, pColorWhenBlack.g / alpha, pColorWhenBlack.b / alpha, alpha);
	}

	private static float getAlpha(float pColorWhenZero, float pColorWhenOne)
	{
		return 1f + pColorWhenZero - pColorWhenOne;
	}

	private static Texture2D captureView(Rect pRect)
	{
		Texture2D texture2D = new Texture2D((int)pRect.width, (int)pRect.height, TextureFormat.ARGB32, false);
		texture2D.ReadPixels(pRect, 0, 0, false);
		return texture2D;
	}
}
