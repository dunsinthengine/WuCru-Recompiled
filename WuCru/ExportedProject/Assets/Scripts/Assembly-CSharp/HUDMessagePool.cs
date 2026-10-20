using UnityEngine;

public class HUDMessagePool : MonoBehaviour
{
	public HUDMessagePoolItem[] _items;

	public void TriggerMessage(Vector3 worldPos)
	{
		int num = _items.Length;
		for (int i = 0; i < num; i++)
		{
			if (_items[i]._pIsAvailable)
			{
				_items[i].Trigger(worldPos);
				break;
			}
		}
	}

	public void TriggerMessage(Vector3 worldPos, string message)
	{
		int num = _items.Length;
		for (int i = 0; i < num; i++)
		{
			if (_items[i]._pIsAvailable)
			{
				_items[i].Trigger(worldPos, message);
				break;
			}
		}
	}

	public void OnShowScreen()
	{
		int num = _items.Length;
		for (int i = 0; i < num; i++)
		{
			_items[i].Init();
		}
	}

	public void OnUpdate()
	{
		int num = _items.Length;
		for (int i = 0; i < num; i++)
		{
			if (!_items[i]._pIsAvailable)
			{
				_items[i].OnUpdate();
			}
		}
	}
}
