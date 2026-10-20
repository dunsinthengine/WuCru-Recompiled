using UnityEngine;

namespace Cape.Utils.UI
{
	public class ParallaxLayer : MonoBehaviour
	{
		[SerializeField]
		private Vector2 _velocity = Vector2.zero;

		public Vector2 Velocity
		{
			get
			{
				return _velocity;
			}
		}

		public RectTransform AnchoredTransform { get; private set; }

		public Vector3 OriginalPosition { get; private set; }

		public Vector2 OriginalPositionCanvas { get; private set; }

		public void Awake()
		{
			AnchoredTransform = GetComponent<RectTransform>();
			OriginalPosition = base.transform.localPosition;
			if (AnchoredTransform != null)
			{
				OriginalPositionCanvas = AnchoredTransform.anchoredPosition;
			}
		}
	}
}
