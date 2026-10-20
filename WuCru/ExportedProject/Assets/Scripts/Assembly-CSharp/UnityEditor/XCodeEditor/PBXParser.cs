using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UnityEditor.XCodeEditor
{
	public class PBXParser
	{
		public const string PBX_HEADER_TOKEN = "// !$*UTF8*$!\n";

		public const char WHITESPACE_SPACE = ' ';

		public const char WHITESPACE_TAB = '\t';

		public const char WHITESPACE_NEWLINE = '\n';

		public const char WHITESPACE_CARRIAGE_RETURN = '\r';

		public const char ARRAY_BEGIN_TOKEN = '(';

		public const char ARRAY_END_TOKEN = ')';

		public const char ARRAY_ITEM_DELIMITER_TOKEN = ',';

		public const char DICTIONARY_BEGIN_TOKEN = '{';

		public const char DICTIONARY_END_TOKEN = '}';

		public const char DICTIONARY_ASSIGN_TOKEN = '=';

		public const char DICTIONARY_ITEM_DELIMITER_TOKEN = ';';

		public const char QUOTEDSTRING_BEGIN_TOKEN = '"';

		public const char QUOTEDSTRING_END_TOKEN = '"';

		public const char QUOTEDSTRING_ESCAPE_TOKEN = '\\';

		public const char END_OF_FILE = '\u001a';

		public const string COMMENT_BEGIN_TOKEN = "/*";

		public const string COMMENT_END_TOKEN = "*/";

		public const string COMMENT_LINE_TOKEN = "//";

		private const int BUILDER_CAPACITY = 20000;

		private char[] data;

		private int index;

		private PBXResolver resolver;

		private string marker;

		public PBXDictionary Decode(string data)
		{
			if (!data.StartsWith("// !$*UTF8*$!\n"))
			{
				Debug.Log("Wrong file format.");
				return null;
			}
			data = data.Substring(13);
			this.data = data.ToCharArray();
			index = 0;
			return (PBXDictionary)ParseValue();
		}

		public string Encode(PBXDictionary pbxData, bool readable = false)
		{
			resolver = new PBXResolver(pbxData);
			StringBuilder stringBuilder = new StringBuilder("// !$*UTF8*$!\n", 20000);
			bool flag = SerializeValue(pbxData, stringBuilder, readable);
			resolver = null;
			stringBuilder.Append("\n");
			return (!flag) ? null : stringBuilder.ToString();
		}

		private void Indent(StringBuilder builder, int deep)
		{
			builder.Append(string.Empty.PadLeft(deep, '\t'));
		}

		private void Endline(StringBuilder builder, bool useSpace = false)
		{
			builder.Append((!useSpace) ? "\n" : " ");
		}

		private void MarkSection(StringBuilder builder, string name)
		{
			if (marker != null || name != null)
			{
				if (marker != null && name != marker)
				{
					builder.Append(string.Format("/* End {0} section */\n", marker));
				}
				if (name != null && name != marker)
				{
					builder.Append(string.Format("\n/* Begin {0} section */\n", name));
				}
				marker = name;
			}
		}

		private bool GUIDComment(string guid, StringBuilder builder)
		{
			string text = resolver.ResolveName(guid);
			string text2 = resolver.ResolveBuildPhaseNameForFile(guid);
			if (text != null)
			{
				if (text2 != null)
				{
					builder.Append(string.Format(" /* {0} in {1} */", text, text2));
				}
				else
				{
					builder.Append(string.Format(" /* {0} */", text));
				}
				return true;
			}
			Debug.Log("GUIDComment " + guid + " [no filename]");
			return false;
		}

		private char NextToken()
		{
			SkipWhitespaces();
			return StepForeward();
		}

		private string Peek(int step = 1)
		{
			string text = string.Empty;
			for (int i = 1; i <= step && data.Length - 1 >= index + i; i++)
			{
				text += data[index + i];
			}
			return text;
		}

		private bool SkipWhitespaces()
		{
			bool result = false;
			while (Regex.IsMatch(StepForeward().ToString(), "\\s"))
			{
				result = true;
			}
			StepBackward();
			if (SkipComments())
			{
				result = true;
				SkipWhitespaces();
			}
			return result;
		}

		private bool SkipComments()
		{
			string text = string.Empty;
			switch (Peek(2))
			{
			case "/*":
				while (Peek(2).CompareTo("*/") != 0)
				{
					text += StepForeward();
				}
				text += StepForeward(2);
				break;
			case "//":
				while (!Regex.IsMatch(StepForeward().ToString(), "\\n"))
				{
				}
				break;
			default:
				return false;
			}
			return true;
		}

		private char StepForeward(int step = 1)
		{
			index = Math.Min(data.Length, index + step);
			return data[index];
		}

		private char StepBackward(int step = 1)
		{
			index = Math.Max(0, index - step);
			return data[index];
		}

		private object ParseValue()
		{
			switch (NextToken())
			{
			case '\u001a':
				Debug.Log("End of file");
				return null;
			case '{':
				return ParseDictionary();
			case '(':
				return ParseArray();
			case '"':
				return ParseString();
			default:
				StepBackward();
				return ParseEntity();
			}
		}

		private PBXDictionary ParseDictionary()
		{
			SkipWhitespaces();
			PBXDictionary pBXDictionary = new PBXDictionary();
			string key = string.Empty;
			object obj = null;
			bool flag = false;
			while (!flag)
			{
				switch (NextToken())
				{
				case '\u001a':
					Debug.Log("Error: reached end of file inside a dictionary: " + index);
					flag = true;
					break;
				case ';':
					key = string.Empty;
					obj = null;
					break;
				case '}':
					key = string.Empty;
					obj = null;
					flag = true;
					break;
				case '=':
					obj = ParseValue();
					if (!pBXDictionary.ContainsKey(key))
					{
						pBXDictionary.Add(key, obj);
					}
					break;
				default:
					StepBackward();
					key = ParseValue() as string;
					break;
				}
			}
			return pBXDictionary;
		}

		private PBXList ParseArray()
		{
			PBXList pBXList = new PBXList();
			bool flag = false;
			while (!flag)
			{
				switch (NextToken())
				{
				case '\u001a':
					Debug.Log("Error: Reached end of file inside a list: " + pBXList);
					flag = true;
					break;
				case ')':
					flag = true;
					break;
				default:
					StepBackward();
					pBXList.Add(ParseValue());
					break;
				case ',':
					break;
				}
			}
			return pBXList;
		}

		private object ParseString()
		{
			string text = string.Empty;
			for (char c = StepForeward(); c != '"'; c = StepForeward())
			{
				text += c;
				if (c == '\\')
				{
					text += StepForeward();
				}
			}
			return text;
		}

		private object ParseEntity()
		{
			string text = string.Empty;
			while (!Regex.IsMatch(Peek(), "[;,\\s=]"))
			{
				text += StepForeward();
			}
			if (text.Length != 24 && Regex.IsMatch(text, "^\\d+$"))
			{
				return int.Parse(text);
			}
			return text;
		}

		private bool SerializeValue(object value, StringBuilder builder, bool readable = false, int indent = 0)
		{
			if (value == null)
			{
				builder.Append("null");
			}
			else if (value is PBXObject)
			{
				SerializeDictionary(((PBXObject)value).data, builder, readable, indent);
			}
			else if (value is Dictionary<string, object>)
			{
				SerializeDictionary((Dictionary<string, object>)value, builder, readable, indent);
			}
			else if (value.GetType().IsArray)
			{
				SerializeArray(new ArrayList((ICollection)value), builder, readable, indent);
			}
			else if (value is ArrayList)
			{
				SerializeArray((ArrayList)value, builder, readable, indent);
			}
			else if (value is string)
			{
				SerializeString((string)value, builder, false, readable);
			}
			else if (value is char)
			{
				SerializeString(Convert.ToString((char)value), builder, false, readable);
			}
			else if (value is bool)
			{
				builder.Append(Convert.ToInt32(value).ToString());
			}
			else
			{
				if (!value.GetType().IsPrimitive)
				{
					Debug.LogWarning("Error: unknown object of type " + value.GetType().Name);
					return false;
				}
				builder.Append(Convert.ToString(value));
			}
			return true;
		}

		private bool SerializeDictionary(Dictionary<string, object> dictionary, StringBuilder builder, bool readable = false, int indent = 0)
		{
			builder.Append('{');
			if (readable)
			{
				Endline(builder);
			}
			foreach (KeyValuePair<string, object> item in dictionary)
			{
				if (readable && indent == 1)
				{
					MarkSection(builder, item.Value.GetType().Name);
				}
				if (readable)
				{
					Indent(builder, indent + 1);
				}
				SerializeString(item.Key, builder, false, readable);
				builder.Append(string.Format(" {0} ", '='));
				SerializeValue(item.Value, builder, readable && item.Value.GetType() != typeof(PBXBuildFile) && item.Value.GetType() != typeof(PBXFileReference), indent + 1);
				builder.Append(';');
				Endline(builder, !readable);
			}
			if (readable && indent == 1)
			{
				MarkSection(builder, null);
			}
			if (readable)
			{
				Indent(builder, indent);
			}
			builder.Append('}');
			return true;
		}

		private bool SerializeArray(ArrayList anArray, StringBuilder builder, bool readable = false, int indent = 0)
		{
			builder.Append('(');
			if (readable)
			{
				Endline(builder);
			}
			for (int i = 0; i < anArray.Count; i++)
			{
				object value = anArray[i];
				if (readable)
				{
					Indent(builder, indent + 1);
				}
				if (!SerializeValue(value, builder, readable, indent + 1))
				{
					return false;
				}
				builder.Append(',');
				Endline(builder, !readable);
			}
			if (readable)
			{
				Indent(builder, indent);
			}
			builder.Append(')');
			return true;
		}

		private bool SerializeString(string aString, StringBuilder builder, bool useQuotes = false, bool readable = false)
		{
			if (Regex.IsMatch(aString, "^[A-Fa-f0-9]{24}$"))
			{
				builder.Append(aString);
				GUIDComment(aString, builder);
				return true;
			}
			if (string.IsNullOrEmpty(aString))
			{
				builder.Append('"');
				builder.Append('"');
				return true;
			}
			if (!Regex.IsMatch(aString, "^[A-Za-z0-9_./-]+$"))
			{
				useQuotes = true;
			}
			if (useQuotes)
			{
				builder.Append('"');
			}
			builder.Append(aString);
			if (useQuotes)
			{
				builder.Append('"');
			}
			return true;
		}
	}
}
