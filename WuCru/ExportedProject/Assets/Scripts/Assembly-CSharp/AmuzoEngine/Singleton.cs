namespace AmuzoEngine
{
	public struct Singleton<T> where T : class
	{
		private T _instance;

		public T _pInstance
		{
			get
			{
				return _instance;
			}
			set
			{
				if (value != null)
				{
					Set(value);
				}
				else
				{
					Unset(_instance);
				}
			}
		}

		public bool _pExists
		{
			get
			{
				return _instance != null;
			}
		}

		public bool Set(T inst)
		{
			return Set(ref _instance, inst);
		}

		public bool Unset(T inst)
		{
			return Unset(ref _instance, inst);
		}

		public static bool Set(ref T instanceRef, T inst)
		{
			if (inst == null)
			{
				return false;
			}
			if (instanceRef != null)
			{
				return false;
			}
			instanceRef = inst;
			return true;
		}

		public static bool Unset(ref T instanceRef, T inst)
		{
			if (inst == null)
			{
				return false;
			}
			if (instanceRef != inst)
			{
				return false;
			}
			instanceRef = (T)null;
			return true;
		}
	}
}
