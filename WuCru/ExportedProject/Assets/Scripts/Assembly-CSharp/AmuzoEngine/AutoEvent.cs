using System;

namespace AmuzoEngine
{
	public class AutoEvent
	{
		public event Action _pCallbacks;

		public void OnEvent()
		{
			if (_pCallbacks != null)
			{
				_pCallbacks();
			}
		}
	}
	public class AutoEvent<TArg>
	{
		public event Action<TArg> _pCallbacks;

		public void OnEvent(TArg arg)
		{
			if (_pCallbacks != null)
			{
				_pCallbacks(arg);
			}
		}
	}
	public class AutoEvent<TArg1, TArg2>
	{
		public event Action<TArg1, TArg2> _pCallbacks;

		public void OnEvent(TArg1 arg1, TArg2 arg2)
		{
			if (_pCallbacks != null)
			{
				_pCallbacks(arg1, arg2);
			}
		}
	}
	public class AutoEvent<TArg1, TArg2, TArg3>
	{
		public event Action<TArg1, TArg2, TArg3> _pCallbacks;

		public void OnEvent(TArg1 arg1, TArg2 arg2, TArg3 arg3)
		{
			if (_pCallbacks != null)
			{
				_pCallbacks(arg1, arg2, arg3);
			}
		}
	}
	public class AutoEvent<TArg1, TArg2, TArg3, TArg4>
	{
		public event Action<TArg1, TArg2, TArg3, TArg4> _pCallbacks;

		public void OnEvent(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
		{
			if (_pCallbacks != null)
			{
				_pCallbacks(arg1, arg2, arg3, arg4);
			}
		}
	}
}
