using UnityEngine;

public class BaseGUI : MonoBehaviour
{
	private GUITypeFlag flag;

	protected void Register()
	{
		flag = GUIMonitor.Instance.TypeToFlag(GetType());
		GUIMonitor.Instance.Register(flag, base.gameObject);
	}

	protected void Deregister()
	{
		GUIMonitor.Instance.Deregister(flag);
	}
}
