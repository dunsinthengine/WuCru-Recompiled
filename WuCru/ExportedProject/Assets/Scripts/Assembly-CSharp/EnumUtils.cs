using System;
using System.Collections.Generic;

public class EnumUtils
{
	public class FromStringMap<EnumType> where EnumType : struct, IConvertible
	{
		private Dictionary<string, EnumType> _dict;

		private EnumType _defaultValue;

		private bool _isCaseSensitive;

		public FromStringMap()
		{
			_isCaseSensitive = true;
			Initialize();
			_defaultValue = default(EnumType);
		}

		public FromStringMap(bool isCaseSensitive)
		{
			_isCaseSensitive = isCaseSensitive;
			Initialize();
			_defaultValue = default(EnumType);
		}

		public FromStringMap(EnumType defaultValue)
		{
			_isCaseSensitive = true;
			Initialize();
			_defaultValue = defaultValue;
		}

		public FromStringMap(EnumType defaultValue, bool isCaseSensitive)
		{
			_isCaseSensitive = isCaseSensitive;
			Initialize();
			_defaultValue = defaultValue;
		}

		private void Initialize()
		{
			Type typeFromHandle = typeof(EnumType);
			string[] names = Enum.GetNames(typeFromHandle);
			EnumType[] array = Enum.GetValues(typeFromHandle) as EnumType[];
			int num = names.Length;
			_dict = new Dictionary<string, EnumType>(num);
			for (int i = 0; i < num; i++)
			{
				if (_isCaseSensitive)
				{
					_dict.Add(names[i], array[i]);
				}
				else
				{
					_dict.Add(names[i].ToUpper(), array[i]);
				}
			}
		}

		public EnumType Lookup(string name)
		{
			if (_dict == null)
			{
				return _defaultValue;
			}
			if (!_isCaseSensitive)
			{
				name = name.ToUpper();
			}
			if (!_dict.ContainsKey(name))
			{
				return _defaultValue;
			}
			return _dict[name];
		}

		public bool TryLookup(string name, out EnumType value)
		{
			value = _defaultValue;
			if (_dict == null)
			{
				return false;
			}
			if (!_isCaseSensitive)
			{
				name = name.ToUpper();
			}
			if (!_dict.ContainsKey(name))
			{
				return false;
			}
			value = _dict[name];
			return true;
		}
	}

	public static bool ToEnum<EnumType>(object input, ref EnumType enumerator)
	{
		try
		{
			enumerator = (EnumType)Enum.Parse(typeof(EnumType), input.ToString());
		}
		catch (ArgumentException)
		{
			return false;
		}
		return true;
	}

	public static bool EnumHasValue<EnumType>(object input)
	{
		return Enum.IsDefined(typeof(EnumType), input.ToString());
	}
}
