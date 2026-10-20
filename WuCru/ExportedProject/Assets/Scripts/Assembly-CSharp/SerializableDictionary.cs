using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
	[SerializeField]
	public List<TKey> keys = new List<TKey>();

	[SerializeField]
	public List<TValue> values = new List<TValue>();

	protected virtual TKey GetDefaultKey()
	{
		return default(TKey);
	}

	public void OnBeforeSerialize()
	{
		keys.Clear();
		values.Clear();
		try
		{
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<TKey, TValue> current = enumerator.Current;
					keys.Add(current.Key);
					values.Add(current.Value);
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Error with serializable dictionary: " + ex.Message);
		}
	}

	public void OnAfterDeserialize()
	{
		Clear();
		while (values.Count > keys.Count)
		{
			keys.Add(GetDefaultKey());
		}
		while (values.Count < keys.Count)
		{
			keys.RemoveAt(keys.Count - 1);
		}
		if (values.Count != keys.Count)
		{
			Debug.LogError("Mismatch between keys/values: " + keys.Count + " keys and " + values.Count + " values");
		}
		for (int i = 0; i < keys.Count; i++)
		{
			Add(keys[i], values[i]);
		}
	}
}
