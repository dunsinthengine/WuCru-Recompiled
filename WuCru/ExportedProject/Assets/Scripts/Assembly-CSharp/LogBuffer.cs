using System;
using UnityEngine;

public class LogBuffer
{
	public enum EBufferSize
	{
		ZERO = 0,
		TEST = 5,
		_1K = 10,
		_2K = 11,
		_4K = 12,
		_8K = 13,
		_16K = 14,
		_32K = 15,
		_64K = 16,
		_128K = 17,
		_256K = 18,
		_512K = 19,
		_1M = 20,
		_2M = 21,
		_4M = 22,
		_8M = 23,
		_16M = 24,
		_32M = 25,
		_64M = 26,
		_128M = 27,
		_256M = 28,
		_512M = 29
	}

	private const string LOG_TAG = "[LogBuffer] ";

	private const char LOG_TYPE_NULL = 'N';

	private const char LOG_TYPE_MESSAGE = 'M';

	private const char LOG_TYPE_WARNING = 'W';

	private const char LOG_TYPE_ERROR = 'E';

	private const string TIME_STAMP_BEGIN = "@";

	private const string TIME_STAMP_END = ": ";

	private char[] _buffer;

	private int _idxModMask;

	private bool _isShowTimeStamp;

	private int _firstEntryPos;

	private int _nextEntryPos;

	private int _dropCount;

	public LogBuffer(EBufferSize bufferSize, bool isShowTimeStamp = false)
	{
		int bufferSizeInChars = GetBufferSizeInChars(bufferSize);
		_buffer = new char[bufferSizeInChars];
		_idxModMask = bufferSizeInChars - 1;
		_isShowTimeStamp = isShowTimeStamp;
		Clear();
	}

	public static int GetBufferSizeInChars(EBufferSize bufferSize)
	{
		return 1 << (int)bufferSize;
	}

	public void Clear()
	{
		_firstEntryPos = (_nextEntryPos = 0);
		WriteChar(_nextEntryPos, 'N');
		_dropCount = 0;
	}

	public void Log(string message, UnityEngine.Object context = null)
	{
		LogInternal('M', message);
	}

	public void LogWarning(string message, UnityEngine.Object context = null)
	{
		LogInternal('W', message);
	}

	public void LogError(string message, UnityEngine.Object context = null)
	{
		LogInternal('E', message);
	}

	public void DumpToUnityLog(string logPrefix = null, bool isClearLog = true)
	{
		if (logPrefix == null)
		{
			logPrefix = string.Empty;
		}
		ForEachEntry((string m) =>
		{
			Debug.Log(logPrefix + m);
		}, (string w) =>
		{
			Debug.LogWarning(logPrefix + w);
		}, (string e) =>
		{
			Debug.LogError(logPrefix + e);
		});
		if (isClearLog)
		{
			Clear();
		}
	}

	public void ForEachEntry(Action<string> onMessage, Action<string> onWarning, Action<string> onError)
	{
		int pos = _firstEntryPos;
		bool flag = false;
		while (!flag)
		{
			char logType;
			string fullMessage;
			pos = ReadEntry(pos, out logType, out fullMessage);
			switch (logType)
			{
			case 'M':
				if (onMessage != null)
				{
					onMessage(fullMessage);
				}
				break;
			case 'W':
				if (onWarning != null)
				{
					onWarning(fullMessage);
				}
				break;
			case 'E':
				if (onError != null)
				{
					onError(fullMessage);
				}
				break;
			default:
				flag = true;
				break;
			}
		}
	}

	private void FixPointers()
	{
		_firstEntryPos &= _idxModMask;
		_nextEntryPos &= _idxModMask;
		if (_nextEntryPos < _firstEntryPos)
		{
			_nextEntryPos += _buffer.Length;
		}
	}

	private void LogInternal(char logType, string message)
	{
		string timeStamp = ((!_isShowTimeStamp) ? null : Time.realtimeSinceStartup.ToString());
		int totalLength = GetTotalLength(message, timeStamp);
		int num = totalLength + 4;
		if (num > _buffer.Length)
		{
			Debug.LogError("[LogBuffer] message too long");
			return;
		}
		EnsureSpace(_nextEntryPos, num);
		_nextEntryPos = WriteEntry(_nextEntryPos, logType, message, timeStamp, totalLength);
		WriteChar(_nextEntryPos, 'N');
		FixPointers();
	}

	private int GetTotalLength(string message, string timeStamp)
	{
		int num = message.Length;
		if (timeStamp != null)
		{
			num += "@".Length + timeStamp.Length + ": ".Length;
		}
		return num;
	}

	private void EnsureSpace(int pos, int len)
	{
		int num = pos + len;
		int num2 = num - _firstEntryPos;
		int num3 = _buffer.Length;
		while (num2 > num3)
		{
			_dropCount++;
			_firstEntryPos = ReadEntry(_firstEntryPos);
			num2 = num - _firstEntryPos;
		}
	}

	private int WriteEntry(int pos, char logType, string message, string timeStamp, int totalMessageLength)
	{
		pos = WriteChar(pos, logType);
		if (totalMessageLength < 0)
		{
			totalMessageLength = GetTotalLength(message, timeStamp);
		}
		pos = WriteInt(pos, totalMessageLength);
		if (timeStamp != null)
		{
			pos = WriteString(pos, "@");
			pos = WriteString(pos, timeStamp);
			pos = WriteString(pos, ": ");
		}
		pos = WriteString(pos, message);
		return pos;
	}

	private int WriteChar(int pos, char value)
	{
		_buffer[pos & _idxModMask] = value;
		return pos + 1;
	}

	private int WriteInt(int pos, int value)
	{
		_buffer[pos & _idxModMask] = (char)(value >> 16);
		_buffer[(pos + 1) & _idxModMask] = (char)(value & 0xFFFF);
		return pos + 2;
	}

	private int WriteString(int pos, string value)
	{
		int length = value.Length;
		int num = _buffer.Length;
		int num2 = num - (pos & _idxModMask);
		if (length > num2)
		{
			value.CopyTo(0, _buffer, pos & _idxModMask, num2);
			value.CopyTo(num2, _buffer, 0, length - num2);
		}
		else
		{
			value.CopyTo(0, _buffer, pos & _idxModMask, length);
		}
		return pos + length;
	}

	private int ReadEntry(int pos, out char logType, out string fullMessage)
	{
		pos = ReadChar(pos, out logType);
		if (logType == 'M' || logType == 'W' || logType == 'E')
		{
			int value;
			pos = ReadInt(pos, out value);
			pos = ReadString(pos, value, out fullMessage);
		}
		else
		{
			fullMessage = null;
		}
		return pos;
	}

	private int ReadEntry(int pos)
	{
		char value;
		pos = ReadChar(pos, out value);
		if (value == 'M' || value == 'W' || value == 'E')
		{
			int value2 = 0;
			pos = ReadInt(pos, out value2);
			pos += value2;
		}
		return pos;
	}

	private int ReadChar(int pos, out char value)
	{
		value = _buffer[pos & _idxModMask];
		return pos + 1;
	}

	private int ReadInt(int pos, out int value)
	{
		value = (int)(((uint)_buffer[pos & _idxModMask] << 16) | _buffer[(pos + 1) & _idxModMask]);
		return pos + 2;
	}

	private int ReadString(int pos, int length, out string value)
	{
		int num = _buffer.Length;
		int num2 = num - (pos & _idxModMask);
		if (length > num2)
		{
			value = new string(_buffer, pos & _idxModMask, num2);
			value += new string(_buffer, 0, length - num2);
		}
		else
		{
			value = new string(_buffer, pos & _idxModMask, length);
		}
		return pos + length;
	}
}
