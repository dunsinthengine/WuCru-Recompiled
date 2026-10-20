using System;
using System.Collections.Generic;
using UnityEngine;

namespace DentedPixel.LTEditor
{
	[Serializable]
	public class LeanTweenGroup
	{
		public string name = "Tweens";

		public bool repeat;

		public float delay;

		public int repeatCount = -1;

		public int repeatIter;

		public bool foldout = true;

		public GameObject gameObject;

		public GameObject overrideGameObject;

		public List<LeanTweenItem> itemList = new List<LeanTweenItem>();

		private bool _generateCode;

		public float startTime
		{
			get
			{
				float num = float.MaxValue;
				foreach (LeanTweenItem item in itemList)
				{
					num = Mathf.Min(item.delay + delay, num);
				}
				if (itemList.Count == 0)
				{
					return 0f;
				}
				return num;
			}
		}

		public float endTime
		{
			get
			{
				float num = float.MinValue;
				foreach (LeanTweenItem item in itemList)
				{
					num = Mathf.Max(item.delay + item.duration + delay, num);
				}
				if (itemList.Count == 0)
				{
					return 0f;
				}
				return num;
			}
		}

		public bool generateCode
		{
			get
			{
				return _generateCode;
			}
		}

		public LeanTweenGroup()
		{
		}

		public LeanTweenGroup(float delay)
		{
			this.delay = delay;
		}

		public LeanTweenGroup(LeanTweenGroup group)
		{
			name = group.name;
			delay = group.delay;
			foldout = group.foldout;
			itemList.Clear();
			foreach (LeanTweenItem item in group.itemList)
			{
				itemList.Add(new LeanTweenItem(item));
			}
		}

		public void setGenerateCode()
		{
			_generateCode = true;
		}
	}
}
