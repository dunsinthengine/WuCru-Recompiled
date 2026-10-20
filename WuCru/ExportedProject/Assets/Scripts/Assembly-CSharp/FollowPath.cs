using System.Collections.Generic;
using UnityEngine;

public class FollowPath : AIBehaviour
{
	private FollowPathState state;

	private float restTimeStart;

	private float restTimeDuration = 3.8f;

	private List<AIWaypoint> pathNodes;

	private int pathNodeIndex;

	private AIWaypoint currentTarget;

	private bool backwards;

	private GameObject pathMesh;

	public override void Initialize(AIController controller)
	{
		pathNodes = GamePlayer.Instance.Level.ChunkDB.GetAIAgentPath(controller.agent.AgentId);
		state = FollowPathState.Idle;
		pathNodeIndex = 0;
		backwards = false;
		if (!(pathMesh == null))
		{
			return;
		}
		List<Vector3> list = new List<Vector3>();
		List<Vector2> list2 = new List<Vector2>();
		List<int> list3 = new List<int>();
		for (int i = 0; i < pathNodes.Count; i++)
		{
			Vector3 vector = LevelHelper.NodeToVector3(pathNodes[i].levelNode) + Vector3.up * 0.05f;
			int count = list.Count;
			float num = ((i != 0 && i != pathNodes.Count - 1) ? 0.175f : 0.25f);
			list.Add(vector + new Vector3(0f - num, 0f, 0f - num));
			list.Add(vector + new Vector3(0f - num, 0f, num));
			list.Add(vector + new Vector3(num, 0f, num));
			list.Add(vector + new Vector3(num, 0f, 0f - num));
			list2.Add(new Vector2(0f, 0f));
			list2.Add(new Vector2(0f, 1f));
			list2.Add(new Vector2(1f, 1f));
			list2.Add(new Vector2(1f, 0f));
			list3.Add(count);
			list3.Add(count + 1);
			list3.Add(count + 2);
			list3.Add(count + 2);
			list3.Add(count + 3);
			list3.Add(count);
			if (i < pathNodes.Count - 1 || controller.agent.PathMode == AIPathMode.Cycle)
			{
				Vector3 vector2 = ((i != pathNodes.Count - 1) ? (LevelHelper.NodeToVector3(pathNodes[i + 1].levelNode) + Vector3.up * 0.05f) : (LevelHelper.NodeToVector3(pathNodes[0].levelNode) + Vector3.up * 0.05f));
				Vector3 vector3 = Vector3.Normalize(vector2 - vector);
				float num2 = Vector3.Distance(vector2, vector);
				float num3 = num2 - 1.4f * num;
				if (num3 > 0.2f)
				{
					Vector3 vector4 = Vector3.Cross(Vector3.up, vector3) * 0.1f;
					list.Add(vector - vector4 + vector3 * 0.7f * num);
					list.Add(vector - vector4 + vector3 * (0.7f * num + num3));
					list.Add(vector + vector4 + vector3 * (0.7f * num + num3));
					list.Add(vector + vector4 + vector3 * 0.7f * num);
					list2.Add(new Vector2(0f, 15f / 32f));
					list2.Add(new Vector2(0f, 17f / 32f));
					list2.Add(new Vector2(1f, 17f / 32f));
					list2.Add(new Vector2(1f, 15f / 32f));
					list3.Add(count + 4);
					list3.Add(count + 4 + 1);
					list3.Add(count + 4 + 2);
					list3.Add(count + 4 + 2);
					list3.Add(count + 4 + 3);
					list3.Add(count + 4);
				}
			}
		}
		pathMesh = new GameObject("AIPath");
		pathMesh.tag = "AIPath";
		pathMesh.transform.parent = GameObject.Find("LevelRoot").transform;
		Mesh mesh = new Mesh();
		mesh.vertices = list.ToArray();
		mesh.triangles = list3.ToArray();
		mesh.uv = list2.ToArray();
		mesh.RecalculateNormals();
		MeshFilter meshFilter = pathMesh.AddComponent<MeshFilter>();
		meshFilter.mesh = mesh;
		MeshRenderer meshRenderer = pathMesh.AddComponent<MeshRenderer>();
		meshRenderer.material = new Material(Shader.Find("Mobile/Particles/Alpha Blended"));
		meshRenderer.material.mainTexture = GamePlayer.Instance.Level.LevelStyle.pathEndpointTexture;
		pathMesh.SetActive(false);
	}

	public override void Update(AIController controller)
	{
		if (pathNodes.Count == 0)
		{
			controller.Character.transform.LookAt(new Vector3(0f, controller.Character.transform.position.y, controller.Character.transform.position.z));
		}
		else if (state == FollowPathState.Idle)
		{
			state = FollowPathState.SeekingTarget;
			if (pathNodes != null && pathNodes.Count > 0)
			{
				GetNewPathTarget(controller);
			}
			else
			{
				Debug.LogError("PathNodes is null or empty - can't find a waypoint");
			}
		}
		else if (state == FollowPathState.SeekingTarget)
		{
			float num = Vector3.Distance(LevelHelper.NodeToVector3(currentTarget.levelNode), controller.agent.Character.transform.position);
			if (!(num < 0.1f))
			{
				return;
			}
			if (pathNodes.Count <= 1)
			{
				controller.Character.transform.LookAt(new Vector3(0f, controller.Character.transform.position.y, controller.Character.transform.position.z));
				state = FollowPathState.StandingAtTheOnlyWaypoint;
			}
			else
			{
				restTimeStart = Time.time;
				restTimeDuration = currentTarget.restTime;
				if (restTimeDuration > 0f)
				{
					state = FollowPathState.Resting;
				}
				else
				{
					GetNewPathTarget(controller);
				}
			}
			if ((bool)pathMesh && !pathMesh.activeSelf && GamePlayer.Instance.ShowAIPaths)
			{
				pathMesh.SetActive(true);
			}
		}
		else if (state == FollowPathState.Resting)
		{
			if (Time.time >= restTimeStart + restTimeDuration)
			{
				state = FollowPathState.Idle;
			}
		}
		else if (state != FollowPathState.StandingAtTheOnlyWaypoint)
		{
		}
	}

	public override void Exit(AIController controller)
	{
		if (pathMesh != null)
		{
			pathMesh.SetActive(false);
		}
	}

	private void GetNewPathTarget(AIController controller)
	{
		if (pathNodes == null || pathNodes.Count <= 0)
		{
			return;
		}
		currentTarget = pathNodes[pathNodeIndex];
		controller.WalkTo(currentTarget.levelNode);
		if (!backwards)
		{
			if (pathNodeIndex == pathNodes.Count - 1)
			{
				if (controller.agent.PathMode == AIPathMode.PingPong)
				{
					backwards = true;
					pathNodeIndex--;
					if (pathNodeIndex < 0)
					{
						pathNodeIndex = 0;
					}
				}
				else if (controller.agent.PathMode == AIPathMode.Cycle)
				{
					pathNodeIndex = 0;
				}
				else
				{
					Debug.LogError("Unhandled PathMode type in FollowPath");
				}
			}
			else
			{
				pathNodeIndex++;
			}
		}
		else if (pathNodeIndex <= 0)
		{
			backwards = false;
			pathNodeIndex++;
			if (pathNodeIndex >= pathNodes.Count)
			{
				pathNodeIndex = pathNodes.Count - 1;
			}
		}
		else
		{
			pathNodeIndex--;
		}
	}
}
