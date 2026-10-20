namespace AmuzoEngine
{
	public class SimpleIntegerIdGenerator
	{
		private int _next;

		public int _pNew
		{
			get
			{
				return _next++;
			}
		}

		public SimpleIntegerIdGenerator()
		{
			Reset();
		}

		public void Reset()
		{
			_next = 0;
		}
	}
}
