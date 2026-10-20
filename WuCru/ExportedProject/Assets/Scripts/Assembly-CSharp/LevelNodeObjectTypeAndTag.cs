public class LevelNodeObjectTypeAndTag
{
	public LevelNodeObjectType objectType;

	public string objectTag = string.Empty;

	public LevelNodeObjectTypeAndTag(LevelNodeObjectType objectType, string objectTag)
	{
		this.objectType = objectType;
		this.objectTag = objectTag;
	}

	public LevelNodeObjectTypeAndTag(LevelNodeObjectType type)
	{
		objectType = type;
	}
}
