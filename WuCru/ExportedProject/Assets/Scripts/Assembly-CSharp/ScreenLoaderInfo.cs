using System;

[Serializable]
public class ScreenLoaderInfo
{
	public ScreenBase screenBase;

	public bool immediatelyShowOnLoad;

	public ScreenLoaderInfo(ScreenBase screen)
	{
		screenBase = screen;
	}
}
