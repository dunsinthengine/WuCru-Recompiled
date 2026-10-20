using System;
using System.Text;

public class SimpleDataSerializer
{
	private const string LOG_TAG = "[SimpleDataSerializer] ";

	private const char DEFAULT_RECORD_SEPARATOR = '#';

	private const char DEFAULT_PAIR_SEPARATOR = ';';

	private const char DEFAULT_KEY_VALUE_SEPARATOR = '=';

	private const char INVALID_SEPARATOR = '\0';

	private char _recordSeparator = '#';

	private char _pairSeparator = ';';

	private char _keyValueSeparator = '=';

	private string _nullValue;

	private StringBuilder _sb = new StringBuilder();

	public SimpleDataSerializer()
	{
		_recordSeparator = '#';
		_pairSeparator = ';';
		_keyValueSeparator = '=';
		_nullValue = null;
	}

	public SimpleDataSerializer(string nullValue)
	{
		_recordSeparator = '#';
		_pairSeparator = ';';
		_keyValueSeparator = '=';
		_nullValue = nullValue;
	}

	public SimpleDataSerializer(char pairSeparator, char keyValueSeparator, string nullValue)
	{
		_recordSeparator = '\0';
		_pairSeparator = pairSeparator;
		_keyValueSeparator = keyValueSeparator;
		_nullValue = nullValue;
	}

	public SimpleDataSerializer(char recordSeparator, char pairSeparator, char keyValueSeparator, string nullValue)
	{
		_recordSeparator = recordSeparator;
		_pairSeparator = pairSeparator;
		_keyValueSeparator = keyValueSeparator;
		_nullValue = nullValue;
	}

	private static void SerializeRecords(int recordCount, Func<int, int> getPairCount, Func<int, int, string> getKey, Func<int, int, string> getValue, char recordSeparator, char pairSeparator, char keyValueSeparator, string nullValue, StringBuilder sb)
	{
		sb.Length = 0;
		if (getPairCount == null || getKey == null || getValue == null)
		{
			return;
		}
		for (int i = 0; i < recordCount; i++)
		{
			int iCap = i;
			if (i > 0)
			{
				sb.Append(recordSeparator);
			}
			int pairCount = getPairCount(i);
			SerializePairs(pairCount, (int pairIndex) => getKey(iCap, pairIndex), (int pairIndex) => getValue(iCap, pairIndex), pairSeparator, keyValueSeparator, nullValue, sb);
		}
	}

	private static void SerializePairs(int pairCount, Func<int, string> getKey, Func<int, string> getValue, char pairSeparator, char keyValueSeparator, string nullValue, StringBuilder sb)
	{
		if (getKey == null || getValue == null)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < pairCount; i++)
		{
			string text = getKey(i);
			if (text == null)
			{
				continue;
			}
			string text2 = getValue(i);
			if (text2 != null || nullValue != null)
			{
				if (flag)
				{
					sb.Append(pairSeparator);
				}
				SerializeKeyValue(text, text2, keyValueSeparator, nullValue, sb);
				flag = true;
			}
		}
	}

	private static void SerializeKeyValue(string key, string value, char keyValueSeparator, string nullValue, StringBuilder sb)
	{
		sb.Append(key);
		sb.Append(keyValueSeparator);
		if (value == null)
		{
			sb.Append(nullValue);
		}
		else
		{
			sb.Append(value);
		}
	}

	private static void DeserializeRecords(string data, Action<int> setRecordCount, Action<int, int> setPairCount, Action<int, int, string, string> setPair, char recordSeparator, char pairSeparator, char keyValueSeparator, string nullValue)
	{
		string[] array = data.Split(recordSeparator);
		if (setRecordCount != null)
		{
			setRecordCount(array.Length);
		}
		for (int i = 0; i < array.Length; i++)
		{
			int iCap = i;
			DeserializePairs(array[i], (int pairCount) =>
			{
				if (setPairCount != null)
				{
					setPairCount(iCap, pairCount);
				}
			}, (int pairIndex, string key, string value) =>
			{
				setPair(iCap, pairIndex, key, value);
			}, pairSeparator, keyValueSeparator, nullValue);
		}
	}

	private static void DeserializePairs(string data, Action<int> setPairCount, Action<int, string, string> setPair, char pairSeparator, char keyValueSeparator, string nullValue)
	{
		string[] array = data.Split(pairSeparator);
		if (setPairCount != null)
		{
			setPairCount(array.Length);
		}
		for (int i = 0; i < array.Length; i++)
		{
			int iCap = i;
			DeserializeKeyValue(array[i], (string key, string value) =>
			{
				setPair(iCap, key, value);
			}, keyValueSeparator, nullValue);
		}
	}

	private static void DeserializeKeyValue(string data, Action<string, string> setPair, char keyValueSeparator, string nullValue)
	{
		string[] array = data.Split(keyValueSeparator);
		if (nullValue != null && array[1] == nullValue)
		{
			setPair(array[0], null);
		}
		else
		{
			setPair(array[0], array[1]);
		}
	}

	public string SerializeRecords(int recordCount, Func<int, int> getPairCount, Func<int, int, string> getKey, Func<int, int, string> getValue)
	{
		_sb.Length = 0;
		SerializeRecords(recordCount, getPairCount, getKey, getValue, _recordSeparator, _pairSeparator, _keyValueSeparator, _nullValue, _sb);
		return _sb.ToString();
	}

	public string SerializePairs(int pairCount, Func<int, string> getKey, Func<int, string> getValue)
	{
		_sb.Length = 0;
		SerializePairs(pairCount, getKey, getValue, _pairSeparator, _keyValueSeparator, _nullValue, _sb);
		return _sb.ToString();
	}

	public void DeserializeRecords(string data, Action<int> setRecordCount, Action<int, int> setPairCount, Action<int, int, string, string> setPair)
	{
		DeserializeRecords(data, setRecordCount, setPairCount, setPair, _recordSeparator, _pairSeparator, _keyValueSeparator, _nullValue);
	}

	public void DeserializePairs(string data, Action<int> setPairCount, Action<int, string, string> setPair)
	{
		DeserializePairs(data, setPairCount, setPair, _pairSeparator, _keyValueSeparator, _nullValue);
	}
}
