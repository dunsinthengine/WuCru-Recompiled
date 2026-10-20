using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class CFX3_Demo : MonoBehaviour
{
	public bool orderedSpawns = true;

	public float step = 1f;

	public float range = 5f;

	private float order = -5f;

	public Renderer groundRenderer;

	public Collider groundCollider;

	private GameObject[] ParticleExamples;

	private int exampleIndex;

	private string randomSpawnsDelay = "0.5";

	private bool randomSpawns;

	private bool slowMo;

	private List<GameObject> onScreenParticles = new List<GameObject>();

	private void Awake()
	{
		List<GameObject> list = new List<GameObject>();
		int childCount = base.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			GameObject item = base.transform.GetChild(i).gameObject;
			list.Add(item);
		}
		ParticleExamples = list.ToArray();
		StartCoroutine("CheckForDeletedParticles");
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			prevParticle();
		}
		else if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			nextParticle();
		}
		else if (Input.GetKeyDown(KeyCode.Delete))
		{
			destroyParticles();
		}
		if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
		{
			RaycastHit hitInfo = default(RaycastHit);
			if (groundCollider.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 9999f))
			{
				GameObject gameObject = spawnParticle();
				gameObject.transform.position = hitInfo.point + gameObject.transform.position;
			}
		}
	}

	private void OnGUI()
	{
		GUILayout.BeginArea(new Rect(5f, 20f, Screen.width - 10, 60f));
		GUILayout.BeginHorizontal();
		GUILayout.Label("Effect", GUILayout.Width(50f));
		if (GUILayout.Button("<", GUILayout.Width(25f)))
		{
			prevParticle();
		}
		GUILayout.Label(ParticleExamples[exampleIndex].name, GUILayout.Width(265f));
		if (GUILayout.Button(">", GUILayout.Width(25f)))
		{
			nextParticle();
		}
		GUILayout.Space(80f);
		if (GUILayout.Button((!CFX_Demo_RotateCamera.rotating) ? "Rotate Camera" : "Pause Camera"))
		{
			CFX_Demo_RotateCamera.rotating = !CFX_Demo_RotateCamera.rotating;
		}
		if (GUILayout.Button((!randomSpawns) ? "Start Random Spawns" : "Stop Random Spawns", GUILayout.Width(140f)))
		{
			randomSpawns = !randomSpawns;
			if (randomSpawns)
			{
				StartCoroutine("RandomSpawnsCoroutine");
			}
			else
			{
				StopCoroutine("RandomSpawnsCoroutine");
			}
		}
		randomSpawnsDelay = GUILayout.TextField(randomSpawnsDelay, 10, GUILayout.Width(42f));
		randomSpawnsDelay = Regex.Replace(randomSpawnsDelay, "[^0-9.]", string.Empty);
		if (GUILayout.Button((!groundRenderer.enabled) ? "Show Ground" : "Hide Ground", GUILayout.Width(90f)))
		{
			groundRenderer.enabled = !groundRenderer.enabled;
		}
		if (GUILayout.Button((!slowMo) ? "Slow Motion" : "Normal Speed", GUILayout.Width(100f)))
		{
			slowMo = !slowMo;
			if (slowMo)
			{
				Time.timeScale = 0.33f;
			}
			else
			{
				Time.timeScale = 1f;
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
		GUILayout.BeginArea(new Rect(5f, 50f, Screen.width - 10, 60f));
		GUILayout.BeginHorizontal();
		GUILayout.Label("Click on the ground to spawn selected particles");
		GUILayout.FlexibleSpace();
		GUILayout.Label("Use the LEFT and RIGHT keys to switch effects; Press DEL to delete all effects on screen");
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	private GameObject spawnParticle()
	{
		GameObject gameObject = Object.Instantiate(ParticleExamples[exampleIndex]);
		gameObject.transform.position = new Vector3(0f, gameObject.transform.position.y, 0f);
		gameObject.SetActive(true);
		for (int i = 0; i < gameObject.transform.childCount; i++)
		{
			gameObject.transform.GetChild(i).gameObject.SetActive(true);
		}
		ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
		if (component != null && component.loop)
		{
			component.gameObject.AddComponent<CFX_AutoDestructShuriken>();
		}
		onScreenParticles.Add(gameObject);
		return gameObject;
	}

	private IEnumerator CheckForDeletedParticles()
	{
		while (true)
		{
			yield return new WaitForSeconds(5f);
			for (int num = onScreenParticles.Count - 1; num >= 0; num--)
			{
				if (onScreenParticles[num] == null)
				{
					onScreenParticles.RemoveAt(num);
				}
			}
		}
	}

	private IEnumerator RandomSpawnsCoroutine()
	{
		while (true)
		{
			GameObject particles = spawnParticle();
			if (orderedSpawns)
			{
				particles.transform.position = base.transform.position + new Vector3(order, particles.transform.position.y, 0f);
				order -= step;
				if (order < 0f - range)
				{
					order = range;
				}
			}
			else
			{
				particles.transform.position = base.transform.position + new Vector3(Random.Range(0f - range, range), 0f, Random.Range(0f - range, range)) + new Vector3(0f, particles.transform.position.y, 0f);
			}
			yield return new WaitForSeconds(float.Parse(randomSpawnsDelay));
		}
	}

	private void prevParticle()
	{
		exampleIndex--;
		if (exampleIndex < 0)
		{
			exampleIndex = ParticleExamples.Length - 1;
		}
	}

	private void nextParticle()
	{
		exampleIndex++;
		if (exampleIndex >= ParticleExamples.Length)
		{
			exampleIndex = 0;
		}
	}

	private void destroyParticles()
	{
		for (int num = onScreenParticles.Count - 1; num >= 0; num--)
		{
			if (onScreenParticles[num] != null)
			{
				Object.Destroy(onScreenParticles[num]);
			}
			onScreenParticles.RemoveAt(num);
		}
	}
}
