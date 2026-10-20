using System;
using System.Collections;

namespace UnityEditor.XCodeEditor
{
	public class PBXList : ArrayList
	{
		public PBXList()
		{
		}

		public PBXList(object firstValue)
		{
			Add(firstValue);
		}

		public static implicit operator bool(PBXList x)
		{
			return x != null && x.Count == 0;
		}

		public string ToCSV()
		{
			string text = string.Empty;
			IEnumerator enumerator = GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					string text2 = (string)enumerator.Current;
					text += "\"";
					text += text2;
					text += "\", ";
				}
				return text;
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}

		public override string ToString()
		{
			return "{" + ToCSV() + "} ";
		}
	}
}
