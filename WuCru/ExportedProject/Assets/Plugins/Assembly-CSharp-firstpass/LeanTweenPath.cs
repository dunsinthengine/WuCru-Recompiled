using UnityEngine;

[AddComponentMenu("LeanTween/LeanTweenPath")]
public class LeanTweenPath : MonoBehaviour
{
	public enum LeanTweenPathType
	{
		bezier = 0,
		spline = 1
	}

	public int count;

	public Transform[] pts;

	public Vector3[] path;

	public LeanTweenPathType pathType;

	public float controlSize = 1f;

	public bool showArrows = true;

	public bool nodesMaximized = true;

	public bool creatorMaximized;

	public bool importMaximized;

	private int i;

	private int k;

	public int lastCount = 1;

	public static Color curveColor;

	public static Color lineColor;

	public Vector3[] vec3
	{
		get
		{
			if (pathType == LeanTweenPathType.bezier)
			{
				Vector3[] array = new Vector3[pts.Length];
				for (i = 0; i < array.Length; i++)
				{
					array[i] = ((i <= 3 || i % 4 != 0) ? pts[i].position : pts[i - 1].position);
				}
				return array;
			}
			return splineVector();
		}
		set
		{
		}
	}

	private void init()
	{
		if (path != null && path.Length != 0 && (pts == null || pts.Length == 0))
		{
			for (i = base.transform.childCount - 1; i >= 0; i--)
			{
				Object.DestroyImmediate(base.transform.GetChild(i).gameObject);
			}
			pts = new Transform[path.Length];
			for (i = 0; i < path.Length; i++)
			{
				if (pathType == LeanTweenPathType.bezier)
				{
					if (i <= 3 || i % 4 != 0)
					{
						pts[i] = createChild(i, path[i]);
					}
				}
				else
				{
					pts[i] = createChild(i, path[i]);
				}
			}
			reset();
			path = new Vector3[0];
			lastCount = (count = path.Length);
		}
		if (pts == null || pts.Length == 0)
		{
			if (pathType == LeanTweenPathType.bezier)
			{
				pts = new Transform[4]
				{
					createChild(0, new Vector3(0f, 0f, 0f)),
					createChild(1, new Vector3(5f, 0f, 0f)),
					createChild(2, new Vector3(4f, 0f, 0f)),
					createChild(3, new Vector3(5f, 5f, 0f))
				};
			}
			else
			{
				pts = new Transform[3]
				{
					createChild(0, new Vector3(0f, 0f, 0f)),
					createChild(1, new Vector3(2f, 0f, 0f)),
					createChild(2, new Vector3(3f, 2f, 0f))
				};
			}
			reset();
		}
		if (lastCount != count)
		{
			if (pathType == LeanTweenPathType.bezier)
			{
				Vector3 vector = Vector3.zero;
				Transform[] array = new Transform[count];
				if (lastCount > count)
				{
					k = 0;
					for (i = 0; i < pts.Length; i++)
					{
						if (i % 4 != 0 && pts[i] == null)
						{
							Object.DestroyImmediate(pts[i - 1].gameObject);
							Object.DestroyImmediate(pts[i - 2].gameObject);
							i++;
							k += -2;
						}
						else if (k < array.Length)
						{
							array[k] = pts[i];
							initNode(array[k], k);
							k++;
							if ((bool)pts[i])
							{
								vector = pts[i].localPosition;
							}
						}
					}
				}
				else
				{
					int num = 0;
					for (i = 0; i < array.Length; i++)
					{
						if (i < pts.Length)
						{
							array[i] = pts[i];
							if ((bool)pts[i])
							{
								vector = pts[i].position;
							}
							num = i;
						}
						else
						{
							k = i;
							Vector3 vector2 = new Vector3(5f, 5f, 0f);
							if (i >= 2)
							{
								Vector3 position = array[num].position;
								Vector3 position2 = array[num - 2].position;
								vector2 = position - position2;
								if (vector2.magnitude < controlSize * 2f)
								{
									vector2 = vector2.normalized * controlSize * 2f;
								}
							}
							if (i % 4 == 1)
							{
								array[i] = createChild(k, vector + vector2 * 0.6f);
							}
							else if (i % 4 == 2)
							{
								array[i] = createChild(k, vector + vector2 * 0.3f);
							}
							else if (i % 4 == 3)
							{
								array[i] = createChild(k, vector + vector2);
							}
						}
					}
				}
				pts = array;
			}
			else
			{
				Transform[] array2 = new Transform[count];
				k = 0;
				for (i = 0; i < pts.Length; i++)
				{
					if (pts[i] != null && k < array2.Length)
					{
						array2[k] = pts[i];
						initNode(array2[k], k);
						k++;
					}
				}
				k = 0;
				if (count > lastCount)
				{
					int num2 = count - lastCount;
					for (i = 0; i < num2; i++)
					{
						k = pts.Length + i;
						if (k <= array2.Length - 1)
						{
							Vector3 pos = new Vector3(1f, 1f, 0f);
							if (k >= 2)
							{
								Vector3 vector3 = array2[k - 1].localPosition - array2[k - 2].localPosition;
								pos = array2[k - 1].localPosition + vector3;
							}
							array2[k] = createChild(k, pos);
						}
					}
				}
				pts = array2;
			}
			lastCount = count;
		}
		reset();
	}

	private void reset()
	{
		if (pathType == LeanTweenPathType.bezier)
		{
			for (i = 0; i < pts.Length; i++)
			{
				LeanTweenPathControl[] array = new LeanTweenPathControl[2];
				if (i % 4 == 0)
				{
					if (i + 2 < pts.Length && (bool)pts[i + 2])
					{
						array[0] = pts[i + 2].gameObject.GetComponent<LeanTweenPathControl>();
					}
				}
				else if (i % 4 == 3)
				{
					array[0] = base.gameObject.GetComponent<LeanTweenPathControl>();
					if (i + 3 < pts.Length && (bool)pts[i + 3])
					{
						array[1] = pts[i + 3].gameObject.GetComponent<LeanTweenPathControl>();
					}
				}
				if ((bool)pts[i])
				{
					LeanTweenPathControl component = pts[i].gameObject.GetComponent<LeanTweenPathControl>();
					component.init(array, i, i % 4 == 0 || i % 4 == 3);
				}
			}
		}
		else
		{
			for (i = 0; i < pts.Length; i++)
			{
				if ((bool)pts[i])
				{
					LeanTweenPathControl component2 = pts[i].gameObject.GetComponent<LeanTweenPathControl>();
					component2.init(i);
				}
			}
		}
		count = (lastCount = pts.Length);
	}

	public Transform createChild(int i, Vector3 pos)
	{
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
		gameObject.AddComponent<LeanTweenPathControl>();
		Transform transform = gameObject.transform;
		Object.DestroyImmediate(gameObject.GetComponent<BoxCollider>());
		transform.parent = base.transform;
		initNode(transform, i);
		transform.position = pos;
		return transform;
	}

	private void initNode(Transform trans, int i)
	{
		if (!(trans != null))
		{
			return;
		}
		if (pathType == LeanTweenPathType.bezier)
		{
			int num = i % 4;
			bool flag = num == 0 || num == 3;
			string text = ((!flag) ? "control" : "node");
			int num2 = (i + 2) / 4 + 1;
			string empty = string.Empty;
			if (!flag)
			{
				trans.localScale = new Vector3(0.5f, 0.5f, 0.25f);
			}
			else
			{
				trans.localScale = Vector3.one * 0.5f;
			}
			trans.name = text + num2 + empty;
		}
		else
		{
			trans.localScale = Vector3.one * 0.25f;
			trans.name = "node" + i;
		}
	}

	private void Start()
	{
		init();
		for (i = 0; i < pts.Length; i++)
		{
			if ((bool)pts[i])
			{
				pts[i].gameObject.SetActive(false);
			}
		}
	}

	public void OnDrawGizmos()
	{
		init();
		if (pathType == LeanTweenPathType.bezier)
		{
			for (int i = 0; i < pts.Length - 3; i += 4)
			{
				if ((bool)pts[i + 1] && (bool)pts[i + 2] && (bool)pts[i + 3])
				{
					Vector3 vector = Vector3.zero;
					if (i > 3 && (bool)pts[i - 1])
					{
						vector = pts[i - 1].position;
					}
					else if ((bool)pts[i])
					{
						vector = pts[i].position;
					}
					Gizmos.color = Color.magenta;
					LeanTween.drawBezierPath(vector, pts[i + 2].position, pts[i + 1].position, pts[i + 3].position, (!showArrows) ? 0f : (controlSize * 0.25f), base.transform);
					Gizmos.color = Color.white;
					Gizmos.DrawLine(vector, pts[i + 2].position);
					Gizmos.DrawLine(pts[i + 1].position, pts[i + 3].position);
				}
			}
			for (int j = 0; j < pts.Length; j++)
			{
				int num = j % 4;
				bool flag = num == 0 || num == 3;
				if ((bool)pts[j])
				{
					pts[j].localScale = ((!flag) ? (new Vector3(1f, 1f, 0.5f) * controlSize * 0.5f) : (Vector3.one * controlSize * 0.5f));
				}
			}
			return;
		}
		this.i = 0;
		while (this.i < pts.Length)
		{
			if ((bool)pts[this.i])
			{
				pts[this.i].localScale = Vector3.one * controlSize * 0.25f;
			}
			this.i++;
		}
		LTSpline lTSpline = new LTSpline(splineVector());
		Gizmos.color = Color.magenta;
		lTSpline.gizmoDraw();
	}

	public Vector3[] splineVector()
	{
		Vector3[] array = new Vector3[pts.Length + 2];
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			if ((bool)pts[num])
			{
				array[i] = pts[num].position;
				if (i >= 1 && i < array.Length - 2)
				{
					num++;
				}
			}
		}
		return array;
	}

	public Vector3[] splineVectorNoEndCaps()
	{
		Vector3[] array = new Vector3[pts.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = pts[i].position;
		}
		return array;
	}

	private void resetPath()
	{
		pts = null;
		for (int num = base.gameObject.transform.childCount - 1; num >= 0; num--)
		{
			Object.DestroyImmediate(base.gameObject.transform.GetChild(num).gameObject);
		}
	}

	public void addNode()
	{
		count += ((pathType != LeanTweenPathType.bezier) ? 1 : 4);
	}

	public void addNodeAfter(int after)
	{
		if (after >= pts.Length - 1)
		{
			addNode();
			return;
		}
		bool flag = pathType == LeanTweenPathType.bezier;
		Vector3[] array = ((!flag) ? splineVectorNoEndCaps() : vec3);
		pts = null;
		int num = ((pathType != LeanTweenPathType.bezier) ? 1 : 4);
		path = new Vector3[array.Length + num];
		Vector3 vector = Vector3.zero;
		Vector3 zero = Vector3.zero;
		Vector3 vector2 = Vector3.zero;
		if (flag)
		{
			for (int i = 0; i < path.Length; i++)
			{
				if (i == after + 1)
				{
					vector = array[i];
					zero = ((array.Length <= 4) ? array[3] : array[i + 3]);
					vector2 = zero - vector;
				}
				if (i > after && i < after + 4)
				{
					if (i % 4 == 0)
					{
						path[i] = vector + vector2 * 0f;
					}
					else if (i % 4 == 1)
					{
						path[i] = vector + vector2 * 0.33f;
					}
					else if (i % 4 == 2)
					{
						path[i] = vector + vector2 * 0.17f;
					}
				}
				else if (i <= after)
				{
					path[i] = array[i];
				}
				else if (i < after + 8)
				{
					if (i % 4 != 0)
					{
						if (i % 4 == 1)
						{
							path[i] = vector + vector2 * 0.83f;
						}
						else if (i % 4 == 2)
						{
							path[i] = vector + vector2 * 0.67f;
						}
						else
						{
							path[i] = vector + vector2 * 0.5f;
						}
					}
				}
				else
				{
					path[i] = array[i - 4];
				}
			}
			init();
			return;
		}
		vector = array[after];
		zero = array[after + 1];
		vector2 = zero - vector;
		for (int j = 0; j < path.Length; j++)
		{
			if (j > after && j < after + 1)
			{
				path[j] = vector + vector2 * 0.33f;
			}
			else if (j <= after)
			{
				path[j] = array[j];
			}
			else if (j < after + 2)
			{
				path[j] = vector + vector2 * 0.66f;
			}
			else
			{
				path[j] = array[j - 1];
			}
		}
		init();
	}

	public void deleteNode(int i)
	{
		Transform transform = pts[i];
		Object.DestroyImmediate(transform.gameObject);
		count += ((pathType != LeanTweenPathType.bezier) ? (-1) : (-4));
	}
}
