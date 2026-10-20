using LitJson;
using UnityEngine;

public class JsonMapperLite
{
	private static bool DO_DEBUG;

	private static bool _loadedAnything;

	public static JsonData ToObject(JsonReader reader)
	{
		JsonData arrayRoot = null;
		_loadedAnything = false;
		while (reader.Read())
		{
			if (reader.Token == JsonToken.ObjectStart || reader.Token == JsonToken.ArrayStart)
			{
				if (DO_DEBUG)
				{
					Debug.Log("JsonMapperLite: Json checked and started with a " + ((reader.Token != JsonToken.ObjectStart) ? "Array" : "Object") + ".");
				}
				if (reader.Token == JsonToken.ArrayStart)
				{
					parseArray(reader, ref arrayRoot);
				}
				else
				{
					arrayRoot = ParseObject(reader, true);
				}
			}
			else if (reader.Token != JsonToken.ObjectEnd)
			{
				Debug.LogError("Unexpected Token when reading JSONData");
				return null;
			}
		}
		if (!_loadedAnything)
		{
			return null;
		}
		return arrayRoot;
	}

	private static JsonData ParseObject(JsonReader reader, bool isRoot = false)
	{
		string text = null;
		JsonData jsonData = new JsonData();
		while (reader.Read())
		{
			if (reader.Token == JsonToken.PropertyName)
			{
				text = (string)reader.Value;
				if (DO_DEBUG)
				{
					Debug.Log("JsonMapperLite: Property name found [" + text + "]");
				}
				continue;
			}
			if (reader.Token == JsonToken.ObjectEnd)
			{
				if (DO_DEBUG)
				{
					Debug.Log("JsonMapperLite: Object Ended");
				}
				return jsonData;
			}
			if (reader.Token == JsonToken.ObjectStart)
			{
				if (DO_DEBUG)
				{
					Debug.Log("JsonMapperLite: Object Started");
				}
				JsonData value = ParseObject(reader);
				jsonData[text] = value;
				_loadedAnything = true;
			}
			else if (reader.Token == JsonToken.String)
			{
				jsonData[text] = (string)reader.Value;
				if (DO_DEBUG)
				{
					Debug.Log("JsonMapperLite: String found [" + (string)reader.Value + "]");
				}
				_loadedAnything = true;
			}
			else if (reader.Token == JsonToken.Boolean)
			{
				jsonData[text] = (bool)reader.Value;
				if (DO_DEBUG)
				{
					Debug.Log("JsonMapperLite: Boolean found [" + (bool)reader.Value + "]");
				}
				_loadedAnything = true;
			}
			else if (reader.Token == JsonToken.Int)
			{
				jsonData[text] = (int)reader.Value;
				if (DO_DEBUG)
				{
					Debug.Log("JsonMapperLite: Int found [" + (int)reader.Value + "]");
				}
				_loadedAnything = true;
			}
			else if (reader.Token == JsonToken.Long)
			{
				jsonData[text] = (long)reader.Value;
				if (DO_DEBUG)
				{
					Debug.Log("JsonMapperLite: Long found [" + (long)reader.Value + "]");
				}
				_loadedAnything = true;
			}
			else if (reader.Token == JsonToken.Double)
			{
				jsonData[text] = (double)reader.Value;
				if (DO_DEBUG)
				{
					Debug.Log("JsonMapperLite: Double found [" + (double)reader.Value + "]");
				}
				_loadedAnything = true;
			}
			else if (reader.Token == JsonToken.ArrayStart)
			{
				JsonData arrayRoot = new JsonData();
				_loadedAnything = true;
				parseArray(reader, ref arrayRoot);
				jsonData[text] = arrayRoot;
			}
			else if (reader.Token == JsonToken.Null)
			{
				jsonData[text] = "Null";
				if (DO_DEBUG)
				{
					Debug.Log("JsonMapperLite: String found [Null]");
				}
				_loadedAnything = true;
			}
			else
			{
				Debug.LogWarning("Unhandled Token: " + reader.Token);
			}
		}
		return jsonData;
	}

	private static void parseArray(JsonReader reader, ref JsonData arrayRoot)
	{
		if (arrayRoot == null)
		{
			arrayRoot = new JsonData();
		}
		arrayRoot.Add(new JsonData(false));
		arrayRoot.Clear();
		if (DO_DEBUG)
		{
			Debug.Log("JsonMapperLite: Array Started");
		}
		while (reader.Read())
		{
			if (reader.Token == JsonToken.ArrayEnd)
			{
				if (DO_DEBUG)
				{
					Debug.Log("JsonMapperLite: Array Ended");
				}
				break;
			}
			if (reader.Token == JsonToken.ObjectStart)
			{
				if (DO_DEBUG)
				{
					Debug.Log("JsonMapperLite: Array Object Started");
				}
				JsonData value = ParseObject(reader);
				arrayRoot.Add(value);
			}
			else if (reader.Token == JsonToken.String)
			{
				if (DO_DEBUG)
				{
					Debug.Log("JsonMapperLite: Array string found [" + (string)reader.Value + "]");
				}
				arrayRoot.Add(new JsonData((string)reader.Value));
			}
			else if (reader.Token == JsonToken.Boolean)
			{
				if (DO_DEBUG)
				{
					Debug.Log("JsonMapperLite: Array bool found [" + (bool)reader.Value + "]");
				}
				arrayRoot.Add(new JsonData((bool)reader.Value));
			}
			else if (reader.Token == JsonToken.Int)
			{
				if (DO_DEBUG)
				{
					Debug.Log("JsonMapperLite: Array int found [" + (int)reader.Value + "]");
				}
				arrayRoot.Add(new JsonData((int)reader.Value));
			}
			else if (reader.Token == JsonToken.Double)
			{
				if (DO_DEBUG)
				{
					Debug.Log("JsonMapperLite: Array double found [" + (double)reader.Value + "]");
				}
				arrayRoot.Add(new JsonData((double)reader.Value));
			}
			else if (reader.Token == JsonToken.Long)
			{
				if (DO_DEBUG)
				{
					Debug.Log("JsonMapperLite: Array long found [" + (long)reader.Value + "]");
				}
				arrayRoot.Add(new JsonData((long)reader.Value));
			}
			else if (reader.Token == JsonToken.ArrayStart)
			{
				JsonData arrayRoot2 = new JsonData();
				parseArray(reader, ref arrayRoot2);
				arrayRoot.Add(arrayRoot2);
			}
		}
	}
}
