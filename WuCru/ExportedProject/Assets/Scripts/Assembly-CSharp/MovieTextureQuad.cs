using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MovieTextureQuad : MonoBehaviour
{
	public Vector3[] newVertices = new Vector3[4]
	{
		new Vector3(-1f, 1f, 1.1f),
		new Vector3(1f, 1f, 1.1f),
		new Vector3(-1f, -1f, 1.1f),
		new Vector3(1f, -1f, 1.1f)
	};

	public Vector2[] newUV = new Vector2[4]
	{
		new Vector2(0f, 1f),
		new Vector2(1f, 1f),
		new Vector2(0f, 0f),
		new Vector2(1f, 0f)
	};

	public Vector2[] newUV2 = new Vector2[4]
	{
		new Vector2(0f, 1f),
		new Vector2(1f, 1f),
		new Vector2(0f, 0f),
		new Vector2(1f, 0f)
	};

	public int[] newTriangles = new int[6] { 0, 1, 2, 2, 1, 3 };

	private void Awake()
	{
		init();
	}

	public void init()
	{
		Mesh mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		mesh.vertices = newVertices;
		mesh.uv = newUV;
		mesh.uv2 = newUV2;
		mesh.triangles = newTriangles;
	}
}
