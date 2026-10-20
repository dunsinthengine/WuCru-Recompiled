using UnityEngine;

namespace DentedPixel.LTEditor
{
	public class LTVisualShared : MonoBehaviour
	{
		public static string[] LT_2_12_MethodNames = new string[32]
		{
			"MOVE_X", "MOVE_Y", "MOVE_Z", "MOVE_LOCAL_X", "MOVE_LOCAL_Y", "MOVE_LOCAL_Z", "MOVE_CURVED", "MOVE_CURVED_LOCAL", "MOVE_SPLINE", "MOVE_SPLINE_LOCAL",
			"SCALE_X", "SCALE_Y", "SCALE_Z", "ROTATE_X", "ROTATE_Y", "ROTATE_Z", "ROTATE_AROUND", "ALPHA", "ALPHA_VERTEX", "CALLBACK",
			"MOVE", "MOVE_LOCAL", "ROTATE", "ROTATE_LOCAL", "SCALE", "VALUE3", "GUI_MOVE", "GUI_MOVE_MARGIN", "GUI_SCALE", "GUI_ALPHA",
			"GUI_ROTATE", "DELAYED_SOUND"
		};

		public static object[][] methodLabelsGrouping = new object[36][]
		{
			new object[2] { "move", "0" },
			new object[2] { "moveLocal", "0" },
			new object[2] { "moveX", "0" },
			new object[2] { "moveY", "0" },
			new object[2] { "moveZ", "0" },
			new object[2] { "moveLocalX", "0" },
			new object[2] { "moveLocalY", "0" },
			new object[2] { "moveLocalZ", "0" },
			new object[2] { "move (bezier)", "0" },
			new object[2] { "moveLocal (bezier)", "0" },
			new object[2] { "moveSpline", "0" },
			new object[2] { "moveSplineLocal", "0" },
			new object[2] { "move (canvas)", "1" },
			new object[2] { "scale", "0" },
			new object[2] { "scaleX", "0" },
			new object[2] { "scaleY", "0" },
			new object[2] { "scaleZ", "0" },
			new object[2] { "scale (canvas)", "1" },
			new object[2] { "rotateAround", "0" },
			new object[2] { "rotateAroundLocal", "0" },
			new object[2] { "play (canvas)", "1" },
			new object[2] { "rotateX", "0" },
			new object[2] { "rotateY", "0" },
			new object[2] { "rotateZ", "0" },
			new object[2] { "alpha", "0" },
			new object[2] { "alpha (background)", "1" },
			new object[2] { "alpha (text)", "1" },
			new object[2] { "alphaVertex", "0" },
			new object[2] { "color", "0" },
			new object[2] { "color (background)", "1" },
			new object[2] { "color (text)", "1" },
			new object[2] { "delayedSound", "0" },
			new object[2] { "rotate (deprecated)", "0" },
			new object[2] { "rotateLocal (deprecated)", "0" },
			new object[2] { "rotateAround (canvas)", "1" },
			new object[2] { "rotateAroundLocal (canvas)", "1" }
		};

		public static string[] methodLabels;

		public static string[] methodStrMapping;

		public static int[] methodIntMapping;

		public static string[] methodStrMappingGrouping = new string[44]
		{
			"MOVE", "MOVE_LOCAL", "MOVE_X", "MOVE_Y", "MOVE_Z", "MOVE_LOCAL_X", "MOVE_LOCAL_Y", "MOVE_LOCAL_Z", "MOVE_CURVED", "MOVE_CURVED_LOCAL",
			"MOVE_SPLINE", "MOVE_SPLINE_LOCAL", "CANVAS_MOVE", "SCALE", "SCALE_X", "SCALE_Y", "SCALE_Z", "CANVAS_SCALE", "ROTATE_AROUND", "ROTATE_AROUND_LOCAL",
			"CANVAS_PLAYSPRITE", "ROTATE_X", "ROTATE_Y", "ROTATE_Z", "ALPHA", "CANVAS_ALPHA", "TEXT_ALPHA", "ALPHA_VERTEX", "COLOR", "CANVAS_COLOR",
			"TEXT_COLOR", "DELAYED_SOUND", "ROTATE", "ROTATE_LOCAL", "CANVAS_ROTATEAROUND", "CANVAS_ROTATEAROUND_LOCAL", "VALUE3", "GUI_MOVE", "GUI_MOVE_MARGIN", "GUI_SCALE",
			"GUI_ALPHA", "GUI_ROTATE", "CALLBACK_COLOR", "CALLBACK"
		};

		public static int[] methodIntMappingGrouping = new int[44]
		{
			34, 35, 0, 1, 2, 3, 4, 5, 6, 7,
			8, 9, 47, 39, 10, 11, 12, 48, 16, 17,
			20, 13, 14, 15, 21, 23, 22, 25, 26, 29,
			28, 46, 37, 38, 18, 19, 40, 41, 42, 43,
			44, 45, 27, 33
		};

		public static int[] easeIntMapping = new int[39]
		{
			0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
			10, 11, 12, 13, 14, 15, 16, 17, 18, 19,
			20, 21, 22, 23, 24, 25, 26, 27, 28, 29,
			30, 31, 32, 33, 34, 35, 36, 37, 38
		};

		public static string[] easeStrMapping = new string[39]
		{
			"notUsed", "linear", "easeOutQuad", "easeInQuad", "easeInOutQuad", "easeInCubic", "easeOutCubic", "easeInOutCubic", "easeInQuart", "easeOutQuart",
			"easeInOutQuart", "easeInQuint", "easeOutQuint", "easeInOutQuint", "easeInSine", "easeOutSine", "easeInOutSine", "easeInExpo", "easeOutExpo", "easeInOutExpo",
			"easeInCirc", "easeOutCirc", "easeInOutCirc", "easeInBounce", "easeOutBounce", "easeInOutBounce", "easeInBack", "easeOutBack", "easeInOutBack", "easeInElastic",
			"easeOutElastic", "easeInOutElastic", "easeSpring", "easeShake", "punch", "once", "clamp", "pingPong", "animationCurve"
		};

		public static void updateTweens(LeanTweenVisual tween)
		{
			if (tween.versionNum == 0)
			{
				foreach (LeanTweenGroup group in tween.groupList)
				{
					foreach (LeanTweenItem item in group.itemList)
					{
						Debug.Log("to action:" + (int)item.action);
						if ((int)item.action < LT_2_12_MethodNames.Length - 1)
						{
							item.actionStr = LT_2_12_MethodNames[(int)item.action];
						}
						else
						{
							item.actionStr = LT_2_12_MethodNames[LT_2_12_MethodNames.Length - 1];
						}
						Debug.Log("item.action toStr:" + item.actionStr);
					}
				}
				tween.versionNum = 220;
			}
			else
			{
				tween.versionNum = 220;
			}
			string obj = "0";
			if ((bool)tween.gameObject.GetComponent<RectTransform>())
			{
				obj = "1";
			}
			int num = 0;
			for (int i = 0; i < methodLabelsGrouping.Length; i++)
			{
				for (int j = 0; j < methodLabelsGrouping[i].Length; j++)
				{
					if (methodLabelsGrouping[i][j].Equals(obj))
					{
						num++;
					}
				}
			}
			methodLabels = new string[num];
			methodStrMapping = new string[num];
			methodIntMapping = new int[num];
			int num2 = 0;
			for (int k = 0; k < methodLabelsGrouping.Length; k++)
			{
				for (int l = 0; l < methodLabelsGrouping[k].Length; l++)
				{
					if (methodLabelsGrouping[k][l].Equals(obj))
					{
						methodLabels[num2] = (string)methodLabelsGrouping[k][0];
						methodStrMapping[num2] = methodStrMappingGrouping[k];
						methodIntMapping[num2] = methodIntMappingGrouping[k];
						num2++;
					}
				}
			}
			foreach (LeanTweenGroup group2 in tween.groupList)
			{
				foreach (LeanTweenItem item2 in group2.itemList)
				{
					int num3 = actionIndex(item2);
					item2.action = (TweenAction)methodIntMapping[num3];
					item2.actionStr = methodStrMapping[num3];
				}
			}
		}

		public static int actionIndex(LeanTweenItem item)
		{
			int num = -1;
			for (int i = 0; i < methodStrMapping.Length; i++)
			{
				if (item.actionStr == methodStrMapping[i])
				{
					num = i;
					break;
				}
			}
			if (num < 0)
			{
				num = 0;
			}
			return num;
		}

		public static void setActionIndex(LeanTweenItem item, int newIndex)
		{
			item.action = (TweenAction)methodIntMapping[newIndex];
			item.actionStr = methodStrMapping[newIndex];
		}

		public static int easeIndex(LeanTweenItem item)
		{
			if (item.easeStr.Length <= 0)
			{
				item.easeStr = easeStrMapping[(int)item.ease];
				if (item.ease == LeanTweenType.notUsed)
				{
					item.easeStr = "linear";
				}
			}
			int num = -1;
			for (int i = 0; i < easeStrMapping.Length; i++)
			{
				if (item.easeStr == easeStrMapping[i])
				{
					num = i;
					break;
				}
			}
			if (num < 0)
			{
				num = 0;
			}
			return num;
		}

		public static void setEaseIndex(LeanTweenItem item, int newIndex)
		{
			item.ease = (LeanTweenType)easeIntMapping[newIndex];
			item.easeStr = easeStrMapping[newIndex];
		}
	}
}
