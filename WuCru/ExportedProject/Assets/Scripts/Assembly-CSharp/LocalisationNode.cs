using LitJson;

public class LocalisationNode : ILocalisationProvider
{
	protected JsonData _localisation;

	public int _numChildren
	{
		get
		{
			if (_localisation == null)
			{
				return 0;
			}
			return _localisation.Count;
		}
	}

	public LocalisationNode()
	{
	}

	public LocalisationNode(JsonData root)
	{
		_localisation = root;
	}

	public LocalisationNode GetNode(string path)
	{
		if (_localisation == null)
		{
			LocalisationFacade.Log("The localisation node at [" + path + "] has no localisation data");
			return null;
		}
		JsonData jsonData = _localisation.JsonAtPath(path);
		if (jsonData == null)
		{
			return null;
		}
		return new LocalisationNode(jsonData);
	}

	public string GetString(string path)
	{
		return GetString(path, LocalisationFacade.Instance.language);
	}

	public string GetString(string path, string language)
	{
		if (path == string.Empty || path == null)
		{
			return null;
		}
		JsonData jsonData = _localisation.JsonAtPath(path);
		if (jsonData == null)
		{
			return null;
		}
		JsonData jsonData2 = jsonData.TryGet(language);
		if (jsonData2 == null)
		{
			jsonData2 = jsonData.TryGet(LocalisationFacade._pDefaultLanguage);
			if (jsonData2 == null)
			{
				return LocalisationFacade.INVALID_STRING;
			}
		}
		return postProcessResponse((string)jsonData2);
	}

	private string postProcessResponse(string response)
	{
		if (LocalisationFacade.DO_DOUBLE_TEXT)
		{
			response = response + " " + response;
		}
		if (LocalisationFacade.DO_OVERRIDE_TEXT_COLOUR)
		{
			response = string.Format(LocalisationFacade.OVERRIDE_TEMPLATE, LocalisationFacade.OVERRIDE_COLOUR, response);
		}
		return response;
	}
}
