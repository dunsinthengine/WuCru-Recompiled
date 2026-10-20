using System;
using UnityEngine;

namespace DentedPixel.LTEditor
{
	[Serializable]
	public class LeanTweenItem
	{
		public string name = "Tween";

		public GameObject gameObject;

		public TweenAction action = TweenAction.MOVE;

		public int actionLast = -1;

		public string actionStr;

		public LeanTweenBetween between = LeanTweenBetween.To;

		public LeanTweenType ease = LeanTweenType.linear;

		public string easeStr = string.Empty;

		public AnimationCurve animationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		public Vector3 from;

		public Vector3 to;

		public Color colorTo = Color.red;

		public Vector3 axis = Vector3.forward;

		public float duration = 1f;

		public float delay;

		public float speed = 1f;

		public bool useSpeed;

		public bool alignWithPrevious;

		public bool foldout = true;

		public LeanTweenPath bezierPath;

		public LeanTweenPath splinePath;

		public AudioClip audioClip;

		public bool orientToPath = true;

		public bool isPath2d;

		public bool doesLoop;

		public int loopCount = -1;

		public LeanTweenType loopType;

		public Sprite[] sprites = new Sprite[0];

		public bool spritesMaximized = true;

		public float frameRate = 6f;

		public LeanTweenItem()
		{
		}

		public LeanTweenItem(float delay)
		{
			this.delay = delay;
		}

		public LeanTweenItem(LeanTweenItem item)
		{
			name = item.name;
			action = item.action;
			between = item.between;
			ease = item.ease;
			from = item.from;
			to = item.to;
			axis = item.axis;
			duration = item.duration;
			delay = item.delay;
			foldout = item.foldout;
		}
	}
}
