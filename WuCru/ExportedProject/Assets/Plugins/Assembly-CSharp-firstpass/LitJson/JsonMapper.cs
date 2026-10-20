namespace LitJson
{
	public class JsonMapper
	{
		public static JsonData ToObject(JsonReader reader)
		{
			return JsonMapperLite.ToObject(reader);
		}
	}
}
