using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Cape.Localization
{
	public class LocalizationFileReader
	{
		private byte[] mBuffer;

		private int mOffset;

		private static List<string> mTemp = new List<string>();

		public LocalizationFileReader(byte[] bytes)
		{
			mBuffer = bytes;
		}

		public LocalizationFileReader(TextAsset asset)
		{
			mBuffer = asset.bytes;
		}

		private static string ReadLine(byte[] buffer, int start, int count)
		{
			return Encoding.UTF8.GetString(buffer, start, count);
		}

		public string ReadLine(bool skipEmptyLines = true)
		{
			int num = mBuffer.Length;
			if (skipEmptyLines)
			{
				while (mOffset < num && mBuffer[mOffset] < 32)
				{
					mOffset++;
				}
			}
			int num2 = mOffset;
			if (num2 < num)
			{
				int num3;
				do
				{
					if (num2 < num)
					{
						num3 = mBuffer[num2++];
						continue;
					}
					num2++;
					break;
				}
				while (num3 != 10 && num3 != 13);
				string result = ReadLine(mBuffer, mOffset, num2 - mOffset - 1);
				mOffset = num2;
				return result;
			}
			mOffset = num;
			return null;
		}

		public List<string> ReadCSV()
		{
			mTemp.Clear();
			string text = string.Empty;
			bool flag = false;
			int num = 0;
			while (mBuffer != null && mOffset < mBuffer.Length)
			{
				if (flag)
				{
					string text2 = ReadLine(false);
					if (text2 == null)
					{
						return null;
					}
					text2 = text2.Replace("\\n", "\n");
					text = text + "\n" + text2;
				}
				else
				{
					text = ReadLine();
					if (text == null)
					{
						return null;
					}
					text = text.Replace("\\n", "\n");
					num = 0;
				}
				int i = num;
				for (int length = text.Length; i < length; i++)
				{
					switch (text[i])
					{
					case ',':
						if (!flag)
						{
							mTemp.Add(text.Substring(num, i - num));
							num = i + 1;
							if (i == length - 1)
							{
								mTemp.Add(string.Empty);
							}
						}
						break;
					case '"':
						if (flag)
						{
							if (i + 1 >= length)
							{
								mTemp.Add(text.Substring(num, i - num).Replace("\"\"", "\""));
								return mTemp;
							}
							if (text[i + 1] != '"')
							{
								mTemp.Add(text.Substring(num, i - num).Replace("\"\"", "\""));
								flag = false;
								if (text[i + 1] == ',')
								{
									i++;
									num = i + 1;
									if (i == length - 1)
									{
										mTemp.Add(string.Empty);
									}
								}
							}
							else
							{
								i++;
							}
						}
						else
						{
							num = i + 1;
							flag = true;
						}
						break;
					}
				}
				if (num < text.Length)
				{
					if (flag)
					{
						continue;
					}
					mTemp.Add(text.Substring(num, text.Length - num));
				}
				return mTemp;
			}
			return null;
		}
	}
}
