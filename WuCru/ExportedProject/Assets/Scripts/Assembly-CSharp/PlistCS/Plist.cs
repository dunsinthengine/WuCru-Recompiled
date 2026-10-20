using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace PlistCS
{
	public static class Plist
	{
		private static List<int> offsetTable = new List<int>();

		private static List<byte> objectTable = new List<byte>();

		private static int refCount;

		private static int objRefSize;

		private static int offsetByteSize;

		private static long offsetTableOffset;

		public static object readPlist(string path)
		{
			using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				return readPlist(stream, plistType.Auto);
			}
		}

		public static object readPlistSource(string source)
		{
			return readPlist(Encoding.UTF8.GetBytes(source));
		}

		public static object readPlist(byte[] data)
		{
			return readPlist(new MemoryStream(data), plistType.Auto);
		}

		public static plistType getPlistType(Stream stream)
		{
			byte[] array = new byte[8];
			stream.Read(array, 0, 8);
			if (BitConverter.ToInt64(array, 0) == 3472403351741427810L)
			{
				return plistType.Binary;
			}
			return plistType.Xml;
		}

		public static object readPlist(Stream stream, plistType type)
		{
			if (type == plistType.Auto)
			{
				type = getPlistType(stream);
				stream.Seek(0L, SeekOrigin.Begin);
			}
			if (type == plistType.Binary)
			{
				using (BinaryReader binaryReader = new BinaryReader(stream))
				{
					byte[] data = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
					return readBinary(data);
				}
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.XmlResolver = null;
			xmlDocument.Load(stream);
			return readXml(xmlDocument);
		}

		public static void writeXml(object value, string path)
		{
			using (StreamWriter streamWriter = new StreamWriter(path))
			{
				streamWriter.Write(writeXml(value));
			}
		}

		public static void writeXml(object value, Stream stream)
		{
			using (StreamWriter streamWriter = new StreamWriter(stream))
			{
				streamWriter.Write(writeXml(value));
			}
		}

		public static string writeXml(object value)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
				xmlWriterSettings.Encoding = new UTF8Encoding(false);
				xmlWriterSettings.ConformanceLevel = ConformanceLevel.Document;
				xmlWriterSettings.Indent = true;
				using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings))
				{
					xmlWriter.WriteStartDocument();
					xmlWriter.WriteStartElement("plist");
					xmlWriter.WriteAttributeString("version", "1.0");
					compose(value, xmlWriter);
					xmlWriter.WriteEndElement();
					xmlWriter.WriteEndDocument();
					xmlWriter.Flush();
					xmlWriter.Close();
					return Encoding.UTF8.GetString(memoryStream.ToArray());
				}
			}
		}

		public static void writeBinary(object value, string path)
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(new FileStream(path, FileMode.Create)))
			{
				binaryWriter.Write(writeBinary(value));
			}
		}

		public static void writeBinary(object value, Stream stream)
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(stream))
			{
				binaryWriter.Write(writeBinary(value));
			}
		}

		public static byte[] writeBinary(object value)
		{
			offsetTable.Clear();
			objectTable.Clear();
			refCount = 0;
			objRefSize = 0;
			offsetByteSize = 0;
			offsetTableOffset = 0L;
			int num = (refCount = countObject(value) - 1);
			objRefSize = RegulateNullBytes(BitConverter.GetBytes(refCount)).Length;
			composeBinary(value);
			writeBinaryString("bplist00", false);
			offsetTableOffset = objectTable.Count;
			offsetTable.Add(objectTable.Count - 8);
			offsetByteSize = RegulateNullBytes(BitConverter.GetBytes(offsetTable[offsetTable.Count - 1])).Length;
			List<byte> list = new List<byte>();
			offsetTable.Reverse();
			for (int i = 0; i < offsetTable.Count; i++)
			{
				offsetTable[i] = objectTable.Count - offsetTable[i];
				byte[] array = RegulateNullBytes(BitConverter.GetBytes(offsetTable[i]), offsetByteSize);
				Array.Reverse(array);
				list.AddRange(array);
			}
			objectTable.AddRange(list);
			objectTable.AddRange(new byte[6]);
			objectTable.Add(Convert.ToByte(offsetByteSize));
			objectTable.Add(Convert.ToByte(objRefSize));
			byte[] bytes = BitConverter.GetBytes((long)num + 1L);
			Array.Reverse(bytes);
			objectTable.AddRange(bytes);
			objectTable.AddRange(BitConverter.GetBytes(0L));
			bytes = BitConverter.GetBytes(offsetTableOffset);
			Array.Reverse(bytes);
			objectTable.AddRange(bytes);
			return objectTable.ToArray();
		}

		private static object readXml(XmlDocument xml)
		{
			XmlNode node = xml.DocumentElement.ChildNodes[0];
			return parse(node);
		}

		private static object readBinary(byte[] data)
		{
			offsetTable.Clear();
			List<byte> list = new List<byte>();
			objectTable.Clear();
			refCount = 0;
			objRefSize = 0;
			offsetByteSize = 0;
			offsetTableOffset = 0L;
			List<byte> list2 = new List<byte>(data);
			List<byte> range = list2.GetRange(list2.Count - 32, 32);
			parseTrailer(range);
			objectTable = list2.GetRange(0, (int)offsetTableOffset);
			list = list2.GetRange((int)offsetTableOffset, list2.Count - (int)offsetTableOffset - 32);
			parseOffsetTable(list);
			return parseBinary(0);
		}

		private static Dictionary<string, object> parseDictionary(XmlNode node)
		{
			XmlNodeList childNodes = node.ChildNodes;
			if (childNodes.Count % 2 != 0)
			{
				throw new DataMisalignedException("Dictionary elements must have an even number of child nodes");
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			for (int i = 0; i < childNodes.Count; i += 2)
			{
				XmlNode xmlNode = childNodes[i];
				XmlNode node2 = childNodes[i + 1];
				if (xmlNode.Name != "key")
				{
					throw new ApplicationException("expected a key node");
				}
				object obj = parse(node2);
				if (obj != null)
				{
					dictionary.Add(xmlNode.InnerText, obj);
				}
			}
			return dictionary;
		}

		private static List<object> parseArray(XmlNode node)
		{
			List<object> list = new List<object>();
			foreach (XmlNode childNode in node.ChildNodes)
			{
				object obj = parse(childNode);
				if (obj != null)
				{
					list.Add(obj);
				}
			}
			return list;
		}

		private static void composeArray(List<object> value, XmlWriter writer)
		{
			writer.WriteStartElement("array");
			foreach (object item in value)
			{
				compose(item, writer);
			}
			writer.WriteEndElement();
		}

		private static object parse(XmlNode node)
		{
			switch (node.Name)
			{
			case "dict":
				return parseDictionary(node);
			case "array":
				return parseArray(node);
			case "string":
				return node.InnerText;
			case "integer":
				return Convert.ToInt32(node.InnerText, NumberFormatInfo.InvariantInfo);
			case "real":
				return Convert.ToDouble(node.InnerText, NumberFormatInfo.InvariantInfo);
			case "false":
				return false;
			case "true":
				return true;
			case "null":
				return null;
			case "date":
				return XmlConvert.ToDateTime(node.InnerText, XmlDateTimeSerializationMode.Utc);
			case "data":
				return Convert.FromBase64String(node.InnerText);
			default:
				throw new ApplicationException(string.Format("Plist Node `{0}' is not supported", node.Name));
			}
		}

		private static void compose(object value, XmlWriter writer)
		{
			if (value == null || value is string)
			{
				writer.WriteElementString("string", value as string);
			}
			else if (value is int || value is long)
			{
				writer.WriteElementString("integer", ((int)value).ToString(NumberFormatInfo.InvariantInfo));
			}
			else if (value is Dictionary<string, object> || value.GetType().ToString().StartsWith("System.Collections.Generic.Dictionary`2[System.String"))
			{
				Dictionary<string, object> dictionary = value as Dictionary<string, object>;
				if (dictionary == null)
				{
					dictionary = new Dictionary<string, object>();
					IDictionary dictionary2 = (IDictionary)value;
					foreach (object key in dictionary2.Keys)
					{
						dictionary.Add(key.ToString(), dictionary2[key]);
					}
				}
				writeDictionaryValues(dictionary, writer);
			}
			else if (value is List<object>)
			{
				composeArray((List<object>)value, writer);
			}
			else if (value is byte[])
			{
				writer.WriteElementString("data", Convert.ToBase64String((byte[])value));
			}
			else if (value is float || value is double)
			{
				writer.WriteElementString("real", ((double)value).ToString(NumberFormatInfo.InvariantInfo));
			}
			else if (value is DateTime)
			{
				DateTime dateTime = (DateTime)value;
				string value2 = XmlConvert.ToString(dateTime, XmlDateTimeSerializationMode.Utc);
				writer.WriteElementString("date", value2);
			}
			else
			{
				if (!(value is bool))
				{
					throw new Exception(string.Format("Value type '{0}' is unhandled", value.GetType().ToString()));
				}
				writer.WriteElementString(value.ToString().ToLower(), string.Empty);
			}
		}

		private static void writeDictionaryValues(Dictionary<string, object> dictionary, XmlWriter writer)
		{
			writer.WriteStartElement("dict");
			foreach (string key in dictionary.Keys)
			{
				object value = dictionary[key];
				writer.WriteElementString("key", key);
				compose(value, writer);
			}
			writer.WriteEndElement();
		}

		private static int countObject(object value)
		{
			int num = 0;
			switch (value.GetType().ToString())
			{
			case "System.Collections.Generic.Dictionary`2[System.String,System.Object]":
			{
				Dictionary<string, object> dictionary = (Dictionary<string, object>)value;
				foreach (string key in dictionary.Keys)
				{
					num += countObject(dictionary[key]);
				}
				num += dictionary.Keys.Count;
				return num + 1;
			}
			case "System.Collections.Generic.List`1[System.Object]":
			{
				List<object> list = (List<object>)value;
				foreach (object item in list)
				{
					num += countObject(item);
				}
				return num + 1;
			}
			default:
				return num + 1;
			}
		}

		private static byte[] writeBinaryDictionary(Dictionary<string, object> dictionary)
		{
			List<byte> list = new List<byte>();
			List<byte> list2 = new List<byte>();
			List<int> list3 = new List<int>();
			for (int num = dictionary.Count - 1; num >= 0; num--)
			{
				object[] array = new object[dictionary.Count];
				dictionary.Values.CopyTo(array, 0);
				composeBinary(array[num]);
				offsetTable.Add(objectTable.Count);
				list3.Add(refCount);
				refCount--;
			}
			for (int num2 = dictionary.Count - 1; num2 >= 0; num2--)
			{
				string[] array2 = new string[dictionary.Count];
				dictionary.Keys.CopyTo(array2, 0);
				composeBinary(array2[num2]);
				offsetTable.Add(objectTable.Count);
				list3.Add(refCount);
				refCount--;
			}
			if (dictionary.Count < 15)
			{
				list2.Add(Convert.ToByte(0xD0 | Convert.ToByte(dictionary.Count)));
			}
			else
			{
				list2.Add(223);
				list2.AddRange(writeBinaryInteger(dictionary.Count, false));
			}
			foreach (int item in list3)
			{
				byte[] array3 = RegulateNullBytes(BitConverter.GetBytes(item), objRefSize);
				Array.Reverse(array3);
				list.InsertRange(0, array3);
			}
			list.InsertRange(0, list2);
			objectTable.InsertRange(0, list);
			return list.ToArray();
		}

		private static byte[] composeBinaryArray(List<object> objects)
		{
			List<byte> list = new List<byte>();
			List<byte> list2 = new List<byte>();
			List<int> list3 = new List<int>();
			for (int num = objects.Count - 1; num >= 0; num--)
			{
				composeBinary(objects[num]);
				offsetTable.Add(objectTable.Count);
				list3.Add(refCount);
				refCount--;
			}
			if (objects.Count < 15)
			{
				list2.Add(Convert.ToByte(0xA0 | Convert.ToByte(objects.Count)));
			}
			else
			{
				list2.Add(175);
				list2.AddRange(writeBinaryInteger(objects.Count, false));
			}
			foreach (int item in list3)
			{
				byte[] array = RegulateNullBytes(BitConverter.GetBytes(item), objRefSize);
				Array.Reverse(array);
				list.InsertRange(0, array);
			}
			list.InsertRange(0, list2);
			objectTable.InsertRange(0, list);
			return list.ToArray();
		}

		private static byte[] composeBinary(object obj)
		{
			switch (obj.GetType().ToString())
			{
			case "System.Collections.Generic.Dictionary`2[System.String,System.Object]":
				return writeBinaryDictionary((Dictionary<string, object>)obj);
			case "System.Collections.Generic.List`1[System.Object]":
				return composeBinaryArray((List<object>)obj);
			case "System.Byte[]":
				return writeBinaryByteArray((byte[])obj);
			case "System.Double":
				return writeBinaryDouble((double)obj);
			case "System.Int32":
				return writeBinaryInteger((int)obj, true);
			case "System.String":
				return writeBinaryString((string)obj, true);
			case "System.DateTime":
				return writeBinaryDate((DateTime)obj);
			case "System.Boolean":
				return writeBinaryBool((bool)obj);
			default:
				return new byte[0];
			}
		}

		public static byte[] writeBinaryDate(DateTime obj)
		{
			List<byte> list = new List<byte>(RegulateNullBytes(BitConverter.GetBytes(PlistDateConverter.ConvertToAppleTimeStamp(obj)), 8));
			list.Reverse();
			list.Insert(0, 51);
			objectTable.InsertRange(0, list);
			return list.ToArray();
		}

		public static byte[] writeBinaryBool(bool obj)
		{
			List<byte> list = new List<byte>(new byte[1] { (byte)((!obj) ? 8 : 9) });
			objectTable.InsertRange(0, list);
			return list.ToArray();
		}

		private static byte[] writeBinaryInteger(int value, bool write)
		{
			List<byte> list = new List<byte>(BitConverter.GetBytes((long)value));
			list = new List<byte>(RegulateNullBytes(list.ToArray()));
			while ((double)list.Count != Math.Pow(2.0, Math.Log(list.Count) / Math.Log(2.0)))
			{
				list.Add(0);
			}
			int value2 = 0x10 | (int)(Math.Log(list.Count) / Math.Log(2.0));
			list.Reverse();
			list.Insert(0, Convert.ToByte(value2));
			if (write)
			{
				objectTable.InsertRange(0, list);
			}
			return list.ToArray();
		}

		private static byte[] writeBinaryDouble(double value)
		{
			List<byte> list = new List<byte>(RegulateNullBytes(BitConverter.GetBytes(value), 4));
			while ((double)list.Count != Math.Pow(2.0, Math.Log(list.Count) / Math.Log(2.0)))
			{
				list.Add(0);
			}
			int value2 = 0x20 | (int)(Math.Log(list.Count) / Math.Log(2.0));
			list.Reverse();
			list.Insert(0, Convert.ToByte(value2));
			objectTable.InsertRange(0, list);
			return list.ToArray();
		}

		private static byte[] writeBinaryByteArray(byte[] value)
		{
			List<byte> list = new List<byte>(value);
			List<byte> list2 = new List<byte>();
			if (value.Length < 15)
			{
				list2.Add(Convert.ToByte(0x40 | Convert.ToByte(value.Length)));
			}
			else
			{
				list2.Add(79);
				list2.AddRange(writeBinaryInteger(list.Count, false));
			}
			list.InsertRange(0, list2);
			objectTable.InsertRange(0, list);
			return list.ToArray();
		}

		private static byte[] writeBinaryString(string value, bool head)
		{
			List<byte> list = new List<byte>();
			List<byte> list2 = new List<byte>();
			char[] array = value.ToCharArray();
			foreach (char value2 in array)
			{
				list.Add(Convert.ToByte(value2));
			}
			if (head)
			{
				if (value.Length < 15)
				{
					list2.Add(Convert.ToByte(0x50 | Convert.ToByte(value.Length)));
				}
				else
				{
					list2.Add(95);
					list2.AddRange(writeBinaryInteger(list.Count, false));
				}
			}
			list.InsertRange(0, list2);
			objectTable.InsertRange(0, list);
			return list.ToArray();
		}

		private static byte[] RegulateNullBytes(byte[] value)
		{
			return RegulateNullBytes(value, 1);
		}

		private static byte[] RegulateNullBytes(byte[] value, int minBytes)
		{
			Array.Reverse(value);
			List<byte> list = new List<byte>(value);
			int num = 0;
			while (num < list.Count && list[num] == 0 && list.Count > minBytes)
			{
				list.Remove(list[num]);
				num--;
				num++;
			}
			if (list.Count < minBytes)
			{
				int num2 = minBytes - list.Count;
				for (int i = 0; i < num2; i++)
				{
					list.Insert(0, 0);
				}
			}
			value = list.ToArray();
			Array.Reverse(value);
			return value;
		}

		private static void parseTrailer(List<byte> trailer)
		{
			offsetByteSize = BitConverter.ToInt32(RegulateNullBytes(trailer.GetRange(6, 1).ToArray(), 4), 0);
			objRefSize = BitConverter.ToInt32(RegulateNullBytes(trailer.GetRange(7, 1).ToArray(), 4), 0);
			byte[] array = trailer.GetRange(12, 4).ToArray();
			Array.Reverse(array);
			refCount = BitConverter.ToInt32(array, 0);
			byte[] array2 = trailer.GetRange(24, 8).ToArray();
			Array.Reverse(array2);
			offsetTableOffset = BitConverter.ToInt64(array2, 0);
		}

		private static void parseOffsetTable(List<byte> offsetTableBytes)
		{
			for (int i = 0; i < offsetTableBytes.Count; i += offsetByteSize)
			{
				byte[] array = offsetTableBytes.GetRange(i, offsetByteSize).ToArray();
				Array.Reverse(array);
				offsetTable.Add(BitConverter.ToInt32(RegulateNullBytes(array, 4), 0));
			}
		}

		private static object parseBinaryDictionary(int objRef)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			List<int> list = new List<int>();
			int num = 0;
			int newBytePosition;
			num = getCount(offsetTable[objRef], out newBytePosition);
			newBytePosition = ((num >= 15) ? (offsetTable[objRef] + 2 + RegulateNullBytes(BitConverter.GetBytes(num), 1).Length) : (offsetTable[objRef] + 1));
			for (int i = newBytePosition; i < newBytePosition + num * 2 * objRefSize; i += objRefSize)
			{
				byte[] array = objectTable.GetRange(i, objRefSize).ToArray();
				Array.Reverse(array);
				list.Add(BitConverter.ToInt32(RegulateNullBytes(array, 4), 0));
			}
			for (int j = 0; j < num; j++)
			{
				dictionary.Add((string)parseBinary(list[j]), parseBinary(list[j + num]));
			}
			return dictionary;
		}

		private static object parseBinaryArray(int objRef)
		{
			List<object> list = new List<object>();
			List<int> list2 = new List<int>();
			int num = 0;
			int newBytePosition;
			num = getCount(offsetTable[objRef], out newBytePosition);
			newBytePosition = ((num >= 15) ? (offsetTable[objRef] + 2 + RegulateNullBytes(BitConverter.GetBytes(num), 1).Length) : (offsetTable[objRef] + 1));
			for (int i = newBytePosition; i < newBytePosition + num * objRefSize; i += objRefSize)
			{
				byte[] array = objectTable.GetRange(i, objRefSize).ToArray();
				Array.Reverse(array);
				list2.Add(BitConverter.ToInt32(RegulateNullBytes(array, 4), 0));
			}
			for (int j = 0; j < num; j++)
			{
				list.Add(parseBinary(list2[j]));
			}
			return list;
		}

		private static int getCount(int bytePosition, out int newBytePosition)
		{
			byte b = objectTable[bytePosition];
			byte b2 = Convert.ToByte(b & 0xF);
			int result;
			if (b2 < 15)
			{
				result = b2;
				newBytePosition = bytePosition + 1;
			}
			else
			{
				result = (int)parseBinaryInt(bytePosition + 1, out newBytePosition);
			}
			return result;
		}

		private static object parseBinary(int objRef)
		{
			byte b = objectTable[offsetTable[objRef]];
			switch (b & 0xF0)
			{
			case 0:
				return (objectTable[offsetTable[objRef]] != 0) ? ((object)(objectTable[offsetTable[objRef]] == 9)) : null;
			case 16:
				return parseBinaryInt(offsetTable[objRef]);
			case 32:
				return parseBinaryReal(offsetTable[objRef]);
			case 48:
				return parseBinaryDate(offsetTable[objRef]);
			case 64:
				return parseBinaryByteArray(offsetTable[objRef]);
			case 80:
				return parseBinaryAsciiString(offsetTable[objRef]);
			case 96:
				return parseBinaryUnicodeString(offsetTable[objRef]);
			case 208:
				return parseBinaryDictionary(objRef);
			case 160:
				return parseBinaryArray(objRef);
			default:
				throw new Exception("This type is not supported");
			}
		}

		public static object parseBinaryDate(int headerPosition)
		{
			byte[] array = objectTable.GetRange(headerPosition + 1, 8).ToArray();
			Array.Reverse(array);
			double timestamp = BitConverter.ToDouble(array, 0);
			DateTime dateTime = PlistDateConverter.ConvertFromAppleTimeStamp(timestamp);
			return dateTime;
		}

		private static object parseBinaryInt(int headerPosition)
		{
			int newHeaderPosition;
			return parseBinaryInt(headerPosition, out newHeaderPosition);
		}

		private static object parseBinaryInt(int headerPosition, out int newHeaderPosition)
		{
			byte b = objectTable[headerPosition];
			int num = (int)Math.Pow(2.0, b & 0xF);
			byte[] array = objectTable.GetRange(headerPosition + 1, num).ToArray();
			Array.Reverse(array);
			newHeaderPosition = headerPosition + num + 1;
			return BitConverter.ToInt32(RegulateNullBytes(array, 4), 0);
		}

		private static object parseBinaryReal(int headerPosition)
		{
			byte b = objectTable[headerPosition];
			int count = (int)Math.Pow(2.0, b & 0xF);
			byte[] array = objectTable.GetRange(headerPosition + 1, count).ToArray();
			Array.Reverse(array);
			return BitConverter.ToDouble(RegulateNullBytes(array, 8), 0);
		}

		private static object parseBinaryAsciiString(int headerPosition)
		{
			int newBytePosition;
			int count = getCount(headerPosition, out newBytePosition);
			List<byte> range = objectTable.GetRange(newBytePosition, count);
			return (range.Count <= 0) ? string.Empty : Encoding.ASCII.GetString(range.ToArray());
		}

		private static object parseBinaryUnicodeString(int headerPosition)
		{
			int newBytePosition;
			int count = getCount(headerPosition, out newBytePosition);
			count *= 2;
			byte[] array = new byte[count];
			for (int i = 0; i < count; i += 2)
			{
				byte b = objectTable.GetRange(newBytePosition + i, 1)[0];
				byte b2 = objectTable.GetRange(newBytePosition + i + 1, 1)[0];
				if (BitConverter.IsLittleEndian)
				{
					array[i] = b2;
					array[i + 1] = b;
				}
				else
				{
					array[i] = b;
					array[i + 1] = b2;
				}
			}
			return Encoding.Unicode.GetString(array);
		}

		private static object parseBinaryByteArray(int headerPosition)
		{
			int newBytePosition;
			int count = getCount(headerPosition, out newBytePosition);
			return objectTable.GetRange(newBytePosition, count).ToArray();
		}
	}
}
