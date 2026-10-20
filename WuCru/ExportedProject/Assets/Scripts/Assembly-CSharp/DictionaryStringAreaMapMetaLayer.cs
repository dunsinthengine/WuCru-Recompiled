using System;

[Serializable]
public class DictionaryStringAreaMapMetaLayer : SerializableDictionary<string, AreaMapMetaLayer>
{
	protected override string GetDefaultKey()
	{
		return "key" + keys.Count;
	}
}
