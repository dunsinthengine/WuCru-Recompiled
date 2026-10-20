using UnityEngine;

public class FormDataEntry
{
	public string key;

	public string value;

	public bool doEncrypt;

	public FormDataEntry(string key, string value, bool doEncrypt = true)
	{
		this.key = key;
		this.value = value;
		this.doEncrypt = doEncrypt;
		if (value == null)
		{
			Debug.LogError("FormDataEntry created with NULL value. Key [" + key + "]");
		}
	}
}
