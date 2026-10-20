using System;

[Flags]
public enum GUITypeFlag
{
	None = 0,
	Settings = 1,
	ConfirmPlay = 2,
	ConfirmQuit = Settings | ConfirmPlay,
	SkillUpgraded = 4,
	LoadScreen = Settings | SkillUpgraded
}
