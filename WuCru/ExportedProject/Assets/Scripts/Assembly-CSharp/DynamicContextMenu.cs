using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicContextMenu : MonoBehaviour
{
	public Vector3 screenPosition;

	public GameObject root;

	private List<GameObject> menuItems = new List<GameObject>();

	private int spacing = 28;

	public void AddMenuItem(string text, Action callback)
	{
		Debug.Log("Adding menu item");
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Dialogs/MenuItem"));
		gameObject.transform.parent = root.transform;
		menuItems.Add(gameObject);
		gameObject.transform.position = new Vector3(screenPosition.x, screenPosition.y - (float)(menuItems.Count * spacing), screenPosition.z);
		Button component = gameObject.GetComponent<Button>();
		component.onClick.AddListener(() =>
		{
			AudioPlayer.PlaySound("ui/button/click");
			callback();
			Close();
		});
		Text componentInChildren = gameObject.GetComponentInChildren<Text>();
		componentInChildren.text = text;
		Debug.Log("MenuItem text: " + text);
	}

	public void Close()
	{
		Debug.Log("ContextMenu close");
		foreach (GameObject menuItem in menuItems)
		{
			Button component = menuItem.GetComponent<Button>();
			component.onClick.RemoveAllListeners();
			UnityEngine.Object.Destroy(menuItem);
		}
		menuItems.Clear();
	}
}
