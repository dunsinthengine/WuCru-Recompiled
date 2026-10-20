using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Text;
using LitJson;
using UnityEngine;

public static class Extensions
{
	public delegate void DInvokedFunction();

	private const string LOG_TAG = "[JsonExtensions] ";

	private static JsonWriter _jsonWriter;

	private static StringWriter _jsonStringWriter;

	private static bool _jsonIsBeginObject;

	public static string PrintFormatXML(string xml)
	{
		if (Application.isEditor)
		{
			return xml;
		}
		return xml.Replace("<", "&lt;").Replace(">", "&gt;");
	}

	public static void EnumerateDictionary<T1, T2>(Dictionary<T1, T2> dict, Action<T1, T2> action)
	{
		foreach (KeyValuePair<T1, T2> item in dict)
		{
			action(item.Key, item.Value);
		}
	}

	public static string StringJoin<T>(this T[] array, string separator)
	{
		if (array == null)
		{
			return "NULL";
		}
		StringBuilder stringBuilder = new StringBuilder();
		int i = 0;
		for (int num = array.Length; i < num; i++)
		{
			stringBuilder.Append(array[i].ToString());
			if (i != num - 1)
			{
				stringBuilder.Append(separator);
			}
		}
		return stringBuilder.ToString();
	}

	public static string StringJoin<T>(this List<T> array, string separator)
	{
		return array.ToArray().StringJoin(separator);
	}

	public static void hardDestroyGameObject(ref GameObject objectToDestroy)
	{
		if (Application.isPlaying)
		{
			UnityEngine.Object.Destroy(objectToDestroy);
		}
		else
		{
			UnityEngine.Object.DestroyImmediate(objectToDestroy);
		}
		objectToDestroy = null;
		objectToDestroy = new GameObject();
		if (Application.isPlaying)
		{
			UnityEngine.Object.Destroy(objectToDestroy);
		}
		else
		{
			UnityEngine.Object.DestroyImmediate(objectToDestroy);
		}
		objectToDestroy = null;
	}

	public static string GetFullName(this GameObject gob, char separator = '/')
	{
		return (!(gob.transform.parent != null)) ? (separator + gob.name) : (gob.transform.parent.gameObject.GetFullName(separator) + separator + gob.name);
	}

	public static string GetFullName(this MonoBehaviour bhv, char separator = '/')
	{
		return bhv.gameObject.GetFullName(separator);
	}

	public static ComponentT EnsureComponent<ComponentT>(this GameObject obj, Action<ComponentT> onNewComponent = null) where ComponentT : Component
	{
		ComponentT val = obj.GetComponent<ComponentT>();
		if (val == null)
		{
			val = obj.AddComponent<ComponentT>();
			if (onNewComponent != null)
			{
				onNewComponent(val);
			}
		}
		return val;
	}

	public static ComponentT EnsureComponent<ComponentT>(this MonoBehaviour bhv, Action<ComponentT> onNewComponent = null) where ComponentT : Component
	{
		return bhv.gameObject.EnsureComponent(onNewComponent);
	}

	public static void Add<T>(this List<T> list, params T[] elements)
	{
		list.AddRange(elements);
	}

	public static Vector3 LocalTransformPoint(this Transform t, Vector3 p)
	{
		return t.localRotation * (t.localPosition + Vector3.Scale(t.localScale, p));
	}

	public static Vector3 LocalTransformVector(this Transform t, Vector3 v)
	{
		return t.localRotation * Vector3.Scale(t.localScale, v);
	}

	public static Vector3 LocalTransformDirection(this Transform t, Vector3 d)
	{
		return t.localRotation * d;
	}

	public static TComponentToReturn Instantiate<TComponentToReturn>(this UnityEngine.Object o, GameObject original) where TComponentToReturn : Component
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(original);
		TComponentToReturn component = gameObject.GetComponent<TComponentToReturn>();
		if (!component)
		{
			Debug.LogError(string.Format("Object \"{0}\" does not have component \"{1}\" attached", gameObject, typeof(TComponentToReturn).ToString()));
			return (TComponentToReturn)null;
		}
		return component;
	}

	public static TComponentToReturn Instantiate<TComponentToReturn>(this UnityEngine.Object o, string prefabName) where TComponentToReturn : Component
	{
		GameObject gameObject = Resources.Load<GameObject>(prefabName);
		if (!gameObject)
		{
			Debug.LogError(string.Format("Couldn't load prefab \"{0}\" from resources", prefabName));
			return (TComponentToReturn)null;
		}
		return o.Instantiate<TComponentToReturn>(gameObject);
	}

	public static string ToRGBHash(this Color color)
	{
		int num = (int)(color.r * 255f);
		int num2 = (int)(color.g * 255f);
		int num3 = (int)(color.b * 255f);
		return "#" + num.ToString("X2") + num2.ToString("X2") + num3.ToString("X2");
	}

	public static void Invoke(this MonoBehaviour mb, DInvokedFunction method, float time)
	{
		mb.Invoke(method.Method.Name, time);
	}

	public static void Invoke_TimeScaleIndependent(this MonoBehaviour mb, DInvokedFunction method, float time)
	{
		mb.StartCoroutine(_Invoke_TimeScaleIndependent(method, time));
	}

	private static IEnumerator _Invoke_TimeScaleIndependent(DInvokedFunction method, float time)
	{
		float startTime = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup < startTime + time)
		{
			yield return null;
		}
		method();
	}

	public static void Invoke_AfterFrames(this MonoBehaviour mb, DInvokedFunction method, int frames)
	{
		mb.StartCoroutine(_Invoke_AfterFrames(method, frames));
	}

	private static IEnumerator _Invoke_AfterFrames(DInvokedFunction method, int frames)
	{
		int startFrame = Time.frameCount;
		while (Time.frameCount < startFrame + frames)
		{
			yield return null;
		}
		method();
	}

	public static void InvokeRepeating(this MonoBehaviour mb, DInvokedFunction method, float time, float repeatRate)
	{
		mb.InvokeRepeating(method.Method.Name, time, repeatRate);
	}

	public static void InvokeRepeating(this MonoBehaviour mb, DInvokedFunction method, float interval)
	{
		mb.InvokeRepeating(method, interval, interval);
	}

	public static void CancelInvoke(this MonoBehaviour mb, DInvokedFunction method)
	{
		mb.CancelInvoke(method.Method.Name);
	}

	public static WWW HttpPost(string url, Dictionary<string, string> post)
	{
		WWWForm wWWForm = new WWWForm();
		foreach (KeyValuePair<string, string> item in post)
		{
			wWWForm.AddField(item.Key, item.Value);
		}
		return new WWW(url, wWWForm);
	}

	public static bool IsArrayOrList(this Type targetType)
	{
		if (typeof(Array).IsAssignableFrom(targetType))
		{
			return true;
		}
		if (typeof(IList).IsAssignableFrom(targetType))
		{
			return true;
		}
		return false;
	}

	public static int GetArrayOrListSize(this Type targetType, object target)
	{
		if (typeof(Array).IsAssignableFrom(targetType))
		{
			return ((Array)target).Length;
		}
		if (typeof(IList).IsAssignableFrom(targetType))
		{
			return ((IList)target).Count;
		}
		return 0;
	}

	public static object GetArrayOrListElement(this Type targetType, object target, int index)
	{
		if (typeof(Array).IsAssignableFrom(targetType))
		{
			return ((Array)target).GetValue(index);
		}
		if (typeof(IList).IsAssignableFrom(targetType))
		{
			return ((IList)target)[index];
		}
		return null;
	}

	public static void SetArrayOrListElement(this Type targetType, object target, int index, object value)
	{
		if (typeof(Array).IsAssignableFrom(targetType))
		{
			((Array)target).SetValue(value, index);
		}
		if (typeof(IList).IsAssignableFrom(targetType))
		{
			((IList)target)[index] = value;
		}
	}

	public static bool IsArrayOrList(this object target)
	{
		Type type = target.GetType();
		return type.IsArrayOrList();
	}

	public static int GetArrayOrListSize(this object target)
	{
		Type type = target.GetType();
		return type.GetArrayOrListSize(target);
	}

	public static object GetArrayOrListElement(this object target, int index)
	{
		Type type = target.GetType();
		return type.GetArrayOrListElement(target, index);
	}

	public static void SetArrayOrListElement(this object target, int index, object value)
	{
		Type type = target.GetType();
		type.SetArrayOrListElement(target, index, value);
	}

	public static Type GetArrayOrListElementType(this Type arrayListType)
	{
		if (arrayListType.IsArray)
		{
			return arrayListType.GetElementType();
		}
		if (typeof(IList).IsAssignableFrom(arrayListType))
		{
			Type type = arrayListType.GetElementType();
			if (type == null)
			{
				Type[] genericArguments = arrayListType.GetGenericArguments();
				if (genericArguments.Length > 0)
				{
					type = genericArguments[0];
				}
			}
			return type;
		}
		return null;
	}

	public static bool HasFieldOrProperty(this object target, string name, Predicate<FieldInfo> isMatchField = null, Predicate<PropertyInfo> isMatchProperty = null)
	{
		FieldInfo field = target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (field != null && (isMatchField == null || isMatchField(field)))
		{
			return true;
		}
		PropertyInfo property = target.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (property != null && (isMatchProperty == null || isMatchProperty(property)))
		{
			return true;
		}
		return false;
	}

	public static bool HasFieldOrProperty(this object target, string name, Type type = null)
	{
		return target.HasFieldOrProperty(name, (FieldInfo fi) => type == null || fi.FieldType == type, (PropertyInfo pi) => type == null || pi.PropertyType == type);
	}

	public static void SetMember(Type memberType, object memberCurr, Action<object> setMember, object value, Action<string> onError = null)
	{
		Type type = ((value == null) ? null : value.GetType());
		if (type != null && typeof(Array).IsAssignableFrom(type))
		{
			Array array = (Array)value;
			if (typeof(Array).IsAssignableFrom(memberType))
			{
				Array obj = (Array)array.Clone();
				setMember(obj);
			}
			else if (typeof(IList).IsAssignableFrom(memberType))
			{
				IList list = (IList)memberCurr;
				list.Clear();
				for (int i = 0; i < array.Length; i++)
				{
					list.Add(array.GetValue(i));
				}
			}
			else if (onError != null)
			{
				onError("Member is not array/list!");
			}
		}
		else if (type != null && typeof(IList).IsAssignableFrom(type))
		{
			IList list2 = (IList)value;
			if (typeof(Array).IsAssignableFrom(memberType))
			{
				Array array2 = Array.CreateInstance(memberType.GetArrayOrListElementType(), list2.Count);
				list2.CopyTo(array2, 0);
				setMember(array2);
			}
			else if (typeof(IList).IsAssignableFrom(memberType))
			{
				IList list3 = (IList)memberCurr;
				list3.Clear();
				for (int j = 0; j < list2.Count; j++)
				{
					list3.Add(list2[j]);
				}
			}
			else if (onError != null)
			{
				onError("Member is not array/list!");
			}
		}
		else
		{
			setMember(value);
		}
	}

	public static void SetFieldOrProperty(this object target, string name, object value, bool logErrorIfNotFound = false)
	{
		FieldInfo fi = target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (fi != null)
		{
			SetMember(fi.FieldType, fi.GetValue(target), (object v) =>
			{
				fi.SetValue(target, v);
			}, value, (string err) =>
			{
				if (logErrorIfNotFound)
				{
					Debug.LogError(err + ", Field=" + name + ", Target=" + target);
				}
			});
			return;
		}
		PropertyInfo pi = target.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (pi != null)
		{
			SetMember(pi.PropertyType, pi.GetValue(target, null), (object v) =>
			{
				pi.SetValue(target, v, null);
			}, value, (string err) =>
			{
				if (logErrorIfNotFound)
				{
					Debug.LogError(err + ", Field=" + name + ", Target=" + target);
				}
			});
		}
		else if (logErrorIfNotFound)
		{
			Debug.LogError(string.Format("Property \"{0}\" not found on {1}!", name, target));
		}
	}

	public static object GetFieldOrProperty(this object target, string name, bool logErrorIfNotFound = false)
	{
		FieldInfo field = target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (field != null)
		{
			return field.GetValue(target);
		}
		PropertyInfo property = target.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (property != null)
		{
			return property.GetValue(target, null);
		}
		if (logErrorIfNotFound)
		{
			Debug.LogError(string.Format("Property \"{0}\" not found on {1}!", name, target));
		}
		return null;
	}

	public static Type GetFieldOrPropertyType(this object target, string name, bool logErrorIfNotFound = false)
	{
		FieldInfo field = target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (field != null)
		{
			return field.FieldType;
		}
		PropertyInfo property = target.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (property != null)
		{
			return property.PropertyType;
		}
		if (logErrorIfNotFound)
		{
			Debug.LogError(string.Format("Property \"{0}\" not found on {1}!", name, target));
		}
		return null;
	}

	public static Rect Translate(this Rect target, float x, float y)
	{
		return new Rect(target.xMin + x, target.yMin + y, target.width, target.height);
	}

	public static Rect Translate(this Rect target, Vector2 offset)
	{
		return new Rect(target.xMin + offset.x, target.yMin + offset.y, target.width, target.height);
	}

	public static Rect Combine(this Rect thisRect, Rect otherRect)
	{
		float num = Mathf.Min(thisRect.xMin, otherRect.xMin);
		float num2 = Mathf.Min(thisRect.yMin, otherRect.yMin);
		float num3 = Mathf.Max(thisRect.xMax, otherRect.xMax);
		float num4 = Mathf.Max(thisRect.yMax, otherRect.yMax);
		return new Rect(num, num2, num3 - num, num4 - num2);
	}

	public static bool IsPointInFrustrum(this Camera targ, Vector3 point, float normBorder = 0f)
	{
		Vector3 vector = targ.WorldToViewportPoint(point);
		return vector.x > normBorder && vector.x < 1f - normBorder && vector.y > normBorder && vector.y < 1f - normBorder && vector.z > 0f;
	}

	public static bool AreBoundsInView(this Camera targ, Bounds bounds)
	{
		return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(targ), bounds);
	}

	public static float SqrMagnitudeXZ(this Vector3 targ)
	{
		return targ.x * targ.x + targ.z * targ.z;
	}

	public static float MagnitudeXZ(this Vector3 targ)
	{
		return Mathf.Sqrt(targ.x * targ.x + targ.z * targ.z);
	}

	public static bool Is(this Type target, Type type)
	{
		if (target.BaseType == type)
		{
			return true;
		}
		if (target.BaseType == null)
		{
			return false;
		}
		return target.BaseType.Is(type);
	}

	public static JsonData JsonAtPath(this JsonData root, string path)
	{
		path = path.Replace(".", "/");
		return JsonAtPath(root, path.Split('/'), 0);
	}

	public static JsonData JsonAtPath(JsonData root, string[] nodes, int startIndex)
	{
		if (root == null)
		{
			return null;
		}
		JsonData jsonData = root;
		for (int i = startIndex; i < nodes.Length; i++)
		{
			jsonData = jsonData.TryGet(nodes[i]);
			if (jsonData == null)
			{
				return null;
			}
		}
		return jsonData;
	}

	public static JsonData LoadJson(string json)
	{
		if (json == null || json == string.Empty)
		{
			return null;
		}
		JsonReader reader = new JsonReader(json);
		return JsonMapper.ToObject(reader);
	}

	public static string PrettyPrint(JsonData data)
	{
		StringWriter stringWriter = new StringWriter();
		JsonWriter jsonWriter = new JsonWriter(stringWriter);
		jsonWriter.Validate = true;
		jsonWriter.PrettyPrint = true;
		data.ToJson(jsonWriter);
		return stringWriter.ToString();
	}

	public static string ToDebug(this JsonData data, string title = null)
	{
		StringWriter stringWriter = new StringWriter();
		if (title != null)
		{
			stringWriter.WriteLine(title + ":");
		}
		DebugJson(data, stringWriter, 0);
		return stringWriter.ToString();
	}

	public static JsonWriter BeginJson(bool isBeginObject = true, bool isWantValidate = true)
	{
		if (_jsonStringWriter != null)
		{
			Debug.LogError("BeginJson: need to EndJson() first!");
			return null;
		}
		_jsonStringWriter = new StringWriter();
		_jsonWriter = new JsonWriter(_jsonStringWriter);
		_jsonWriter.Validate = isWantValidate;
		_jsonIsBeginObject = isBeginObject;
		if (_jsonIsBeginObject)
		{
			_jsonWriter.WriteObjectStart();
		}
		return _jsonWriter;
	}

	public static string EndJson()
	{
		if (_jsonStringWriter == null)
		{
			Debug.LogError("EndJson: need to BeginJson() first!");
			return null;
		}
		if (_jsonIsBeginObject)
		{
			_jsonWriter.WriteObjectEnd();
		}
		string result = _jsonStringWriter.ToString();
		_jsonStringWriter = null;
		_jsonWriter = null;
		return result;
	}

	public static void WriteValue(this JsonWriter writer, string propertyName, object obj)
	{
		writer.WritePropertyName(propertyName);
		if (obj is bool)
		{
			writer.Write((bool)obj);
		}
		else if (obj is decimal)
		{
			writer.Write((decimal)obj);
		}
		else if (obj is double)
		{
			writer.Write((double)obj);
		}
		else if (obj is int)
		{
			writer.Write((int)obj);
		}
		else if (obj is long)
		{
			writer.Write((long)obj);
		}
		else if (obj is ulong)
		{
			writer.Write((ulong)obj);
		}
		else
		{
			writer.Write(obj.ToString());
		}
	}

	public static void WriteValue(this JsonWriter writer, string propertyName, bool boolean)
	{
		writer.WritePropertyName(propertyName);
		writer.Write(boolean);
	}

	public static void WriteValue(this JsonWriter writer, string propertyName, decimal number)
	{
		writer.WritePropertyName(propertyName);
		writer.Write(number);
	}

	public static void WriteValue(this JsonWriter writer, string propertyName, double number)
	{
		writer.WritePropertyName(propertyName);
		writer.Write(number);
	}

	public static void WriteValue(this JsonWriter writer, string propertyName, int number)
	{
		writer.WritePropertyName(propertyName);
		writer.Write(number);
	}

	public static void WriteValue(this JsonWriter writer, string propertyName, long number)
	{
		writer.WritePropertyName(propertyName);
		writer.Write(number);
	}

	public static void WriteValue(this JsonWriter writer, string propertyName, ulong number)
	{
		writer.WritePropertyName(propertyName);
		writer.Write(number);
	}

	public static void WriteValue(this JsonWriter writer, string propertyName, string str)
	{
		writer.WritePropertyName(propertyName);
		writer.Write(str);
	}

	public static void WriteArray(this JsonWriter writer, string propertyName, int[] array)
	{
		writer.WritePropertyName(propertyName);
		writer.WriteArrayStart();
		foreach (int number in array)
		{
			writer.Write(number);
		}
		writer.WriteArrayEnd();
	}

	public static void WriteArray(this JsonWriter writer, string propertyName, string[] array)
	{
		writer.WritePropertyName(propertyName);
		writer.WriteArrayStart();
		foreach (string str in array)
		{
			writer.Write(str);
		}
		writer.WriteArrayEnd();
	}

	public static void WriteList(this JsonWriter writer, string propertyName, List<int> list)
	{
		writer.WritePropertyName(propertyName);
		writer.WriteArrayStart();
		for (int i = 0; i < list.Count; i++)
		{
			int number = list[i];
			writer.Write(number);
		}
		writer.WriteArrayEnd();
	}

	public static void WriteArray(this JsonWriter writer, string propertyName, bool[] array)
	{
		writer.WritePropertyName(propertyName);
		writer.WriteArrayStart();
		foreach (bool boolean in array)
		{
			writer.Write(boolean);
		}
		writer.WriteArrayEnd();
	}

	public static void WriteList(this JsonWriter writer, string propertyName, List<bool> list)
	{
		writer.WritePropertyName(propertyName);
		writer.WriteArrayStart();
		for (int i = 0; i < list.Count; i++)
		{
			bool boolean = list[i];
			writer.Write(boolean);
		}
		writer.WriteArrayEnd();
	}

	public static bool WriteJsonData(this JsonWriter writer, JsonData jd, Func<JsonData, string, bool> errorHandler = null)
	{
		Func<string, bool> func = (string msg) => errorHandler != null && errorHandler(jd, msg);
		if (jd == null)
		{
			return func("Null JsonData");
		}
		switch (jd.GetJsonType())
		{
		default:
			return func("Invalid JsonData");
		case JsonType.Object:
			writer.WriteObjectStart();
			foreach (DictionaryEntry item in (IDictionary)jd)
			{
				if (item.Key == null || !(item.Key is string))
				{
					return func("Null or invalid object key");
				}
				if (item.Value == null || !(item.Value is JsonData))
				{
					return func("Null or invalid object value");
				}
				writer.WritePropertyName((string)item.Key);
				if (!writer.WriteJsonData((JsonData)item.Value, errorHandler))
				{
					return false;
				}
			}
			writer.WriteObjectEnd();
			break;
		case JsonType.Array:
		{
			writer.WriteArrayStart();
			for (int num = 0; num < jd.Count; num++)
			{
				if (jd[num] == null)
				{
					return func("Null array element");
				}
				if (!writer.WriteJsonData(jd[num], errorHandler))
				{
					return false;
				}
			}
			writer.WriteArrayEnd();
			break;
		}
		case JsonType.String:
			writer.Write((string)jd);
			break;
		case JsonType.Int:
			writer.Write((int)jd);
			break;
		case JsonType.Long:
			writer.Write((long)jd);
			break;
		case JsonType.Double:
			writer.Write((double)jd);
			break;
		case JsonType.Boolean:
			writer.Write((bool)jd);
			break;
		}
		return true;
	}

	public static string SafeToJson(this JsonData jd, Action<string> errorHandler = null)
	{
		if (jd == null)
		{
			return string.Empty;
		}
		Action<string> handleError = (string msg) =>
		{
			if (errorHandler != null)
			{
				errorHandler(msg);
			}
		};
		Func<JsonData, string, bool> errorHandler2 = (JsonData jsonData, string msg) =>
		{
			handleError(msg);
			return false;
		};
		JsonWriter jsonWriter = BeginJson(false, false);
		if (jsonWriter == null)
		{
			handleError("Cannot write json");
			return string.Empty;
		}
		if (!jsonWriter.WriteJsonData(jd, errorHandler2))
		{
			EndJson();
			handleError("Failed to write json");
			return string.Empty;
		}
		return EndJson();
	}

	public static JsonData TryGet(this JsonData data, string key)
	{
		if (data != null && data.Contains(key))
		{
			return data[key];
		}
		return null;
	}

	public static JsonData TryGet(this JsonData data, string key, JsonType type)
	{
		JsonData jsonData = data.TryGet(key);
		if (jsonData == null || jsonData.GetJsonType() != type)
		{
			return null;
		}
		return jsonData;
	}

	public static string TryGetString(this JsonData data, string key, string defaultValue = "")
	{
		if (data != null && data.Contains(key))
		{
			JsonData jsonData = data[key];
			if (jsonData.GetJsonType() == JsonType.String)
			{
				return (string)jsonData;
			}
		}
		return defaultValue;
	}

	public static int TryGetInt(this JsonData data, string key, int defaultValue = 0)
	{
		if (data != null && data.Contains(key))
		{
			JsonData jsonData = data[key];
			if (jsonData.GetJsonType() == JsonType.Int)
			{
				return (int)jsonData;
			}
		}
		return defaultValue;
	}

	public static double TryGetDouble(this JsonData data, string key, double defaultValue = 0.0)
	{
		if (data != null && data.Contains(key))
		{
			JsonData jsonData = data[key];
			if (jsonData.GetJsonType() == JsonType.Double)
			{
				return (double)jsonData;
			}
		}
		return defaultValue;
	}

	public static bool TryGetBool(this JsonData data, string key, bool defaultValue = false)
	{
		if (data != null && data.Contains(key))
		{
			JsonData jsonData = data[key];
			if (jsonData.GetJsonType() == JsonType.Boolean)
			{
				return (bool)jsonData;
			}
		}
		return defaultValue;
	}

	public static void AddChildren(this JsonData data, JsonData root)
	{
		foreach (DictionaryEntry item in (IDictionary)root)
		{
			string prop_name = (string)item.Key;
			data[prop_name] = item.Value as JsonData;
		}
	}

	public static void Enumerate(this JsonData data, Action<string, JsonData> action)
	{
		foreach (DictionaryEntry item in (IDictionary)data)
		{
			action((string)item.Key, item.Value as JsonData);
		}
	}

	public static JsonData DeepCopyJson(JsonData src)
	{
		JsonData jsonData = null;
		if (src != null)
		{
			switch (src.GetJsonType())
			{
			case JsonType.Array:
			{
				jsonData = new JsonData();
				jsonData.SetJsonType(JsonType.Array);
				for (int i = 0; i < src.Count; i++)
				{
					jsonData.Add(DeepCopyJson(src[i]));
				}
				break;
			}
			case JsonType.Boolean:
				jsonData = new JsonData((bool)src);
				break;
			case JsonType.Double:
				jsonData = new JsonData((double)src);
				break;
			case JsonType.Int:
				jsonData = new JsonData((int)src);
				break;
			case JsonType.Long:
				jsonData = new JsonData((long)src);
				break;
			case JsonType.None:
				jsonData = new JsonData();
				break;
			case JsonType.Object:
				jsonData = new JsonData();
				jsonData.SetJsonType(JsonType.Object);
				foreach (DictionaryEntry item in (IDictionary)src)
				{
					jsonData[(string)item.Key] = DeepCopyJson((JsonData)item.Value);
				}
				break;
			case JsonType.String:
				jsonData = new JsonData((string)src);
				break;
			}
		}
		return jsonData;
	}

	public static JsonData MergeJsonObjects(JsonData objA, JsonData objB)
	{
		JsonData jsonData = null;
		if (objA != null && objA.IsObject)
		{
			jsonData = DeepCopyJson(objA);
			if (objB != null && objB.IsObject)
			{
				MergeJsonObjectChildren(jsonData, objB);
			}
		}
		else if (objB != null && objB.IsObject)
		{
			jsonData = DeepCopyJson(objB);
		}
		else
		{
			jsonData = new JsonData();
			jsonData.SetJsonType(JsonType.Object);
		}
		return jsonData;
	}

	public static bool MergeJsonObjectChildren(JsonData objA, JsonData objB)
	{
		if (objA == null)
		{
			return false;
		}
		if (!objA.IsObject)
		{
			return false;
		}
		if (objB == null)
		{
			return false;
		}
		if (!objB.IsObject)
		{
			return false;
		}
		foreach (DictionaryEntry item in (IDictionary)objB)
		{
			string text = (string)item.Key;
			JsonData jsonData = (JsonData)item.Value;
			if (jsonData.IsObject && objA.Contains(text))
			{
				JsonData jsonData2 = objA[text];
				if (jsonData2.IsObject)
				{
					MergeJsonObjectChildren(jsonData2, jsonData);
				}
				else
				{
					objA[text] = DeepCopyJson(jsonData);
				}
			}
			else
			{
				objA[text] = DeepCopyJson(jsonData);
			}
		}
		return true;
	}

	public static void TryRemove(this JsonData thisData, string key)
	{
		if (thisData != null && thisData.IsObject)
		{
			if (((IDictionary)thisData).Contains((object)key))
			{
				((IDictionary)thisData).Remove((object)key);
			}
		}
	}

	public static void TryRemove(this JsonData thisData, JsonData childData)
	{
		if (thisData == null || (!thisData.IsArray && !thisData.IsObject))
		{
			return;
		}
		int count = thisData.Count;
		int i;
		for (i = 0; i < count && thisData[i] != childData; i++)
		{
		}
		if (i != count)
		{
			if (thisData.IsArray)
			{
				((IList)thisData).RemoveAt(i);
			}
			else if (thisData.IsObject)
			{
				((IOrderedDictionary)thisData).RemoveAt(i);
			}
		}
	}

	private static void DebugJson(IJsonWrapper obj, StringWriter sw, int indent)
	{
		sw.WriteLine("{0,14}", obj.GetJsonType());
		if (obj.IsArray)
		{
			foreach (object item in (IEnumerable)obj)
			{
				DebugJson((JsonData)item, sw, indent + 1);
			}
			return;
		}
		if (!obj.IsObject)
		{
			return;
		}
		foreach (DictionaryEntry item2 in (IDictionary)obj)
		{
			DebugJson((JsonData)item2.Value, sw, indent + 1);
		}
	}
}
