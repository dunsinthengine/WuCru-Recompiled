using System.Diagnostics;
using UnityEngine;

public static class NoDebugLogger
{
	[Conditional("DEBUG_MESSAGES")]
	public static void Log(object message)
	{
	}

	[Conditional("DEBUG_MESSAGES")]
	public static void Log(object message, Object context)
	{
	}

	[Conditional("DEBUG_MESSAGES")]
	public static void LogWarning(object message)
	{
	}

	[Conditional("DEBUG_MESSAGES")]
	public static void LogWarning(object message, Object context)
	{
	}

	[Conditional("DEBUG_MESSAGES")]
	public static void LogError(object message)
	{
	}

	[Conditional("DEBUG_MESSAGES")]
	public static void LogError(object message, Object context)
	{
	}
}
