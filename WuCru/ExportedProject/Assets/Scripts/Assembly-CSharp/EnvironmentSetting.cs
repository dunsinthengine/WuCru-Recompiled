using UnityEngine;
using UnityEngine.Rendering;

public class EnvironmentSetting : MonoBehaviour
{
	public const float AMBIENT_MULTIPLIER = 0.25f;

	public string environmentName = "Unnamed Environment Setting";

	public Light directionalLight;

	public bool isNight;

	public ParticleSystem levelFX;

	public ParticleSystem playerFX;

	public Vector3 playerFXOffset = new Vector3(0f, 5f, 0f);

	public Color fogColor = Color.white;

	public Color ambientLight = Color.white;

	public float ambientIntensity = 0.2f;

	public float fogStartDistance = 50f;

	public float fogEndDistance = 100f;

	public float fogDensity = 0.012f;

	public float fogHeightStart = 1f;

	public float fogHeightEnd;

	public Color occlusionColor = Color.black;

	private GameObject root;

	private void Awake()
	{
		base.enabled = false;
	}

	public void Setup(GameObject levelRoot, ref string environmentName)
	{
		if (root != null)
		{
			Object.Destroy(root);
		}
		root = new GameObject();
		root.name = "EnvironmentSetting";
		if (levelRoot != null)
		{
			root.transform.parent = levelRoot.transform;
		}
		InGameCam._pClearColor = fogColor;
		RenderSettings.fogColor = fogColor;
		RenderSettings.fogMode = FogMode.Linear;
		RenderSettings.fogStartDistance = fogStartDistance;
		RenderSettings.fogEndDistance = fogEndDistance;
		RenderSettings.fogDensity = fogDensity;
		RenderSettings.ambientMode = AmbientMode.Flat;
		RenderSettings.ambientLight = ambientLight * 0.25f;
		RenderSettings.ambientIntensity = ambientIntensity;
		NinjagoFog ninjagoFog = Object.FindObjectOfType<NinjagoFog>();
		if ((bool)ninjagoFog)
		{
			ninjagoFog.heightStart = fogHeightStart;
			ninjagoFog.heightEnd = fogHeightEnd;
		}
		OcclusionRenderer occlusionRenderer = Object.FindObjectOfType<OcclusionRenderer>();
		if ((bool)occlusionRenderer)
		{
			occlusionRenderer.SetOccludedColor(occlusionColor);
		}
		Light[] array = Object.FindObjectsOfType<Light>();
		Light[] array2 = array;
		foreach (Light light in array2)
		{
			if (light.type == LightType.Directional)
			{
				Object.Destroy(light.gameObject);
			}
		}
		GameObject gameObject = Object.Instantiate(directionalLight).gameObject;
		gameObject.transform.parent = root.transform;
		Light component = gameObject.GetComponent<Light>();
		if (component != null)
		{
			component.shadows = LightShadows.None;
		}
		if (levelFX != null)
		{
			GameObject gameObject2 = Object.Instantiate(levelFX).gameObject;
			gameObject2.transform.parent = root.transform;
		}
		environmentName = this.environmentName;
	}
}
