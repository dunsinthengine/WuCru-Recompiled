using UnityEngine;

namespace Fabric
{
	[ExecuteInEditMode]
	public class FabricSpringBoard : MonoBehaviour
	{
		public string _fabricManagerPrefabPath;

		public static bool _isPresent;

		public FabricSpringBoard()
		{
			_isPresent = true;
		}

		private void OnEnable()
		{
			_isPresent = true;
		}

		private void Awake()
		{
			Load();
		}

		public void Load()
		{
			FabricManager fabricManagerInEditor = GetFabricManagerInEditor();
			if (!fabricManagerInEditor)
			{
				GameObject gameObject = Resources.Load(_fabricManagerPrefabPath, typeof(GameObject)) as GameObject;
				if ((bool)gameObject)
				{
					Object.Instantiate(gameObject);
				}
			}
		}

		public static FabricManager GetFabricManagerInEditor()
		{
			FabricManager[] array = Resources.FindObjectsOfTypeAll(typeof(FabricManager)) as FabricManager[];
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].gameObject != null && array[i].hideFlags != HideFlags.HideInHierarchy)
				{
					return array[i];
				}
			}
			return null;
		}
	}
}
