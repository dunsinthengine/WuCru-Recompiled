using System;
using System.IO;
using UnityEngine;

public static class FileHelper
{
	public static void WriteTextFile(string filePath, string fileData)
	{
		string directoryName = Path.GetDirectoryName(filePath);
		if (directoryName != null && directoryName != string.Empty)
		{
			try
			{
				Directory.CreateDirectory(directoryName);
			}
			catch (Exception)
			{
				Debug.LogWarning("Failed to create directory: " + directoryName.ToString());
			}
		}
		try
		{
			File.WriteAllText(filePath, fileData);
		}
		catch (Exception)
		{
			Debug.LogWarning("Failed to write file:" + filePath.ToString());
		}
	}

	public static void ForEachFileInDirectory(string dirPath, Action<FileInfo> action, Predicate<FileInfo> match = null)
	{
		ForEachFileInDirectory(new DirectoryInfo(dirPath), action, match);
	}

	public static void ForEachFileInDirectory(DirectoryInfo dirInfo, Action<FileInfo> action, Predicate<FileInfo> match = null)
	{
		if (dirInfo != null && dirInfo.Exists)
		{
			ForEachFile(dirInfo.GetFiles(), action, match);
		}
	}

	public static void ForEachFile(FileInfo[] fileInfos, Action<FileInfo> action, Predicate<FileInfo> match = null)
	{
		foreach (FileInfo obj in fileInfos)
		{
			if (match == null || match(obj))
			{
				action(obj);
			}
		}
	}

	public static void ForEachSubDirectory(string dirPath, Action<DirectoryInfo> action, Predicate<DirectoryInfo> match = null)
	{
		ForEachSubDirectory(new DirectoryInfo(dirPath), action, match);
	}

	public static void ForEachSubDirectory(DirectoryInfo dirInfo, Action<DirectoryInfo> action, Predicate<DirectoryInfo> match = null)
	{
		if (dirInfo != null && dirInfo.Exists)
		{
			ForEachDirectory(dirInfo.GetDirectories(), action, match);
		}
	}

	public static void ForEachDirectory(DirectoryInfo[] dirInfos, Action<DirectoryInfo> action, Predicate<DirectoryInfo> match = null)
	{
		foreach (DirectoryInfo obj in dirInfos)
		{
			if (match == null || match(obj))
			{
				action(obj);
			}
		}
	}

	public static string AddLocalFileUrlScheme(string absolutePath)
	{
		if (string.IsNullOrEmpty(absolutePath))
		{
			return "file:///";
		}
		if (absolutePath.StartsWith("/"))
		{
			return "file://" + absolutePath;
		}
		return "file:///" + absolutePath;
	}
}
