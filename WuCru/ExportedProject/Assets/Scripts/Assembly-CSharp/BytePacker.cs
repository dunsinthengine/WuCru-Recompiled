using System;
using System.Collections.Generic;
using System.Text;

public class BytePacker
{
	private const int MaxCapacity = int.MaxValue;

	private const int DefaultCapacity = 32;

	private List<byte> _buffer;

	private int _position;

	public int Length
	{
		get
		{
			return _buffer.Count;
		}
	}

	public int Position
	{
		get
		{
			return _position;
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException("value", "The Position has to be a non negative number.");
			}
			if (value > _buffer.Count)
			{
				throw new ArgumentOutOfRangeException("value", "The Position cannot exceed the length of the stream.");
			}
			_position = value;
		}
	}

	public BytePacker()
	{
		_buffer = new List<byte>(32);
	}

	public BytePacker(byte[] buffer)
	{
		if (buffer == null)
		{
			throw new ArgumentNullException("buffer", "The buffer cannot be null.");
		}
		if (buffer.Length > int.MaxValue)
		{
			throw new ArgumentException("The stream is larger than its max capacity: " + int.MaxValue);
		}
		_buffer = new List<byte>(buffer);
	}

	public BytePacker(byte[] buffer, int index, int count)
	{
		if (buffer == null)
		{
			throw new ArgumentNullException("buffer", "The buffer cannot be null.");
		}
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException("index", "The index has to be a non negative number.");
		}
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("count", "The count has to be a non negative number.");
		}
		if (index + count > buffer.Length)
		{
			throw new ArgumentException("The number of bytes to copy exceeds the buffers length.");
		}
		if (_position + count > int.MaxValue)
		{
			throw new ArgumentException("The stream is larger than its max capacity: " + int.MaxValue);
		}
		_buffer = new List<byte>(count);
		for (int i = index; i < count; i++)
		{
			_buffer.Add(buffer[i - index]);
		}
	}

	internal void Write7BitEncodedInt(int value)
	{
		uint num;
		for (num = (uint)value; num >= 128; num >>= 7)
		{
			Write((byte)(num | 0x80));
		}
		Write((byte)num);
	}

	internal int Read7BitEncodedInt()
	{
		int num = 0;
		int num2 = 0;
		byte b;
		do
		{
			if (num2 == 35)
			{
				throw new FormatException("Error in the byte stream, too many bytes to fit into an integer.");
			}
			b = ReadByte();
			num |= (b & 0x7F) << num2;
			num2 += 7;
		}
		while ((b & 0x80) != 0);
		return num;
	}

	public byte[] ToArray()
	{
		return _buffer.ToArray();
	}

	public void Write(byte value)
	{
		if (_position + 1 > int.MaxValue)
		{
			throw new ArgumentException("The stream is larger than its max capacity: " + int.MaxValue);
		}
		if (_position == _buffer.Count)
		{
			_buffer.Add(value);
		}
		else
		{
			_buffer[_position] = value;
		}
		_position++;
	}

	public void Write(byte[] buffer)
	{
		Write(buffer, 0, buffer.Length);
	}

	public void Write(byte[] buffer, int index, int count)
	{
		if (buffer == null)
		{
			throw new ArgumentNullException("buffer", "The buffer cannot be null.");
		}
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException("index", "The index has to be a non negative number.");
		}
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("count", "The count has to be a non negative number.");
		}
		if (index + count > buffer.Length)
		{
			throw new ArgumentException("The number of bytes to copy exceeds the buffers length.");
		}
		if (_position + count > int.MaxValue)
		{
			throw new ArgumentException("The stream is larger than its max capacity: " + int.MaxValue);
		}
		int count2 = _buffer.Count;
		int position = _position;
		int i = 0;
		while (_position < count2 && i < count)
		{
			_buffer[_position++] = buffer[index + i];
			i++;
		}
		for (; i < count; i++)
		{
			_buffer.Add(buffer[index + i]);
		}
		_position = position + i;
	}

	public void Write(bool value)
	{
		byte value2 = 0;
		if (value)
		{
			value2 = 1;
		}
		Write(value2);
	}

	public void Write(ushort value)
	{
		Write(new byte[2]
		{
			(byte)(value >> 8),
			(byte)value
		});
	}

	public void Write(short value)
	{
		Write((ushort)value);
	}

	public void Write(uint value)
	{
		Write(new byte[4]
		{
			(byte)(value >> 24),
			(byte)(value >> 16),
			(byte)(value >> 8),
			(byte)value
		});
	}

	public void Write(int value)
	{
		Write((uint)value);
	}

	public void Write(ulong value)
	{
		Write(new byte[8]
		{
			(byte)(value >> 56),
			(byte)(value >> 48),
			(byte)(value >> 40),
			(byte)(value >> 32),
			(byte)(value >> 24),
			(byte)(value >> 16),
			(byte)(value >> 8),
			(byte)value
		});
	}

	public void Write(long value)
	{
		Write((ulong)value);
	}

	public void Write(float value)
	{
		byte[] bytes = BitConverter.GetBytes(value);
		Array.Reverse(bytes);
		Write(bytes);
	}

	public void Write(string value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value", "The string cannot be null.");
		}
		byte[] bytes = Encoding.UTF8.GetBytes(value);
		Write7BitEncodedInt(bytes.Length);
		Write(bytes);
	}

	public byte ReadByte()
	{
		if (_position == _buffer.Count)
		{
			throw new EndOfStreamException("The end of the stream is reached.");
		}
		return _buffer[_position++];
	}

	public byte[] ReadBytes(int count)
	{
		if (_position + count > _buffer.Count)
		{
			throw new EndOfStreamException("The number of bytes to read exceeds the stream's length.");
		}
		byte[] array = new byte[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = _buffer[_position++];
		}
		return array;
	}

	public bool ReadBoolean()
	{
		byte b = ReadByte();
		return b > 0;
	}

	public ushort ReadUInt16()
	{
		if (_position + 2 > _buffer.Count)
		{
			throw new EndOfStreamException("The end of the stream is reached.");
		}
		int num = _buffer[_position++] << 8;
		num |= _buffer[_position++];
		return (ushort)num;
	}

	public short ReadInt16()
	{
		return (short)ReadUInt16();
	}

	public uint ReadUInt32()
	{
		return (uint)ReadInt32();
	}

	public int ReadInt32()
	{
		if (_position + 4 > _buffer.Count)
		{
			throw new EndOfStreamException("The end of the stream is reached.");
		}
		int num = _buffer[_position++] << 24;
		num |= _buffer[_position++] << 16;
		num |= _buffer[_position++] << 8;
		return num | _buffer[_position++];
	}

	public ulong ReadUInt64()
	{
		if (_position + 8 > _buffer.Count)
		{
			throw new EndOfStreamException("The end of the stream is reached.");
		}
		ulong num = (ulong)_buffer[_position++] << 56;
		num |= (ulong)_buffer[_position++] << 48;
		num |= (ulong)_buffer[_position++] << 40;
		num |= (ulong)_buffer[_position++] << 32;
		num |= (ulong)_buffer[_position++] << 24;
		num |= (ulong)_buffer[_position++] << 16;
		num |= (ulong)_buffer[_position++] << 8;
		return num | _buffer[_position++];
	}

	public long ReadInt64()
	{
		return (long)ReadUInt64();
	}

	public float ReadSingle()
	{
		byte[] array = ReadBytes(4);
		Array.Reverse(array);
		return BitConverter.ToSingle(array, 0);
	}

	public string ReadString()
	{
		int count = Read7BitEncodedInt();
		byte[] bytes = ReadBytes(count);
		return Encoding.UTF8.GetString(bytes);
	}

	public void Delete(int count)
	{
		Delete(_position, count);
	}

	public void Delete(int index, int count)
	{
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException("index", "The index has to be a non negative number.");
		}
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("count", "The count has to be a non negative number.");
		}
		if (index + count > _buffer.Count)
		{
			throw new ArgumentException("The number of bytes to delete exceeds the stream's length.");
		}
		_buffer.RemoveRange(index, count);
		_position = index;
	}

	public void Clear()
	{
		Delete(0, _buffer.Count);
	}
}
