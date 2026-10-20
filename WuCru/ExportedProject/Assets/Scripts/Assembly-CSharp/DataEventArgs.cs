using System;

public class DataEventArgs<T> : EventArgs
{
	public T data { get; set; }
}
