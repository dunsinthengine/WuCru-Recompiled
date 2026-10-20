using UnityEngine;

namespace Cape.UI
{
	[ExecuteInEditMode]
	public class TiledUISprite : MonoBehaviour
	{
		[SerializeField]
		private RectTransform _tiledImageTransform;

		[ReadOnly]
		[SerializeField]
		private bool _referenceSet;

		[ReadOnly]
		[SerializeField]
		private Vector2 _relativeSize;

		[ReadOnly]
		[SerializeField]
		private Vector2 _relativePosition;

		private int _screenWidth;

		private int _screenHeight;

		private void Awake()
		{
			if (_referenceSet)
			{
				_screenWidth = Screen.width;
				_screenHeight = Screen.height;
				UpdateSizeAndPosition();
			}
		}

		public void UpdateSizeAndPosition()
		{
			if (_referenceSet)
			{
				Vector2 sizeDelta = default(Vector2);
				Vector3 localPosition = default(Vector3);
				sizeDelta.x = _relativeSize.x * (float)_screenWidth;
				sizeDelta.y = _relativeSize.y * (float)_screenHeight;
				localPosition.x = Mathf.FloorToInt(_relativePosition.x * (float)_screenWidth);
				localPosition.y = Mathf.FloorToInt(_relativePosition.y * (float)_screenHeight);
				localPosition.z = 0f;
				_tiledImageTransform.sizeDelta = sizeDelta;
				_tiledImageTransform.localPosition = localPosition;
			}
		}
	}
}
