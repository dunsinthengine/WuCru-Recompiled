using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DentedPixel.LTEditor
{
	public class LeanTweenVisual : MonoBehaviour
	{
		public List<LeanTweenGroup> groupList = new List<LeanTweenGroup>();

		public bool repeat;

		public float repeatDelay;

		public int repeatCount;

		public int repeatIter;

		public float nextCall;

		public bool restartOnEnable;

		public bool playOnStart = true;

		public int versionNum;

		private StringBuilder codeBuild;

		private string tabs;

		private LTDescr tween;

		private float allTweensDelaySaved;

		private void Awake()
		{
			LTVisualShared.updateTweens(this);
		}

		private void Start()
		{
			if (playOnStart)
			{
				buildAllTweens(false);
			}
		}

		public void start(GameObject target = null)
		{
			buildAllTweens(false, target);
		}

		private void Update()
		{
		}

		private void OnEnable()
		{
			if (restartOnEnable)
			{
				buildAllTweens(false);
			}
		}

		private void OnDisable()
		{
			if (restartOnEnable)
			{
				LeanTween.cancel(base.gameObject);
			}
		}

		private void OnDestroy()
		{
			LeanTween.cancel(base.gameObject);
		}

		public void CopyTo(LeanTweenVisual tween)
		{
			tween.nextCall = nextCall;
			tween.repeat = repeat;
			tween.repeatDelay = repeatDelay;
			tween.restartOnEnable = restartOnEnable;
			tween.groupList = new List<LeanTweenGroup>();
			foreach (LeanTweenGroup group in groupList)
			{
				tween.groupList.Add(new LeanTweenGroup(group));
			}
		}

		public void buildAllTweensAgain()
		{
			LeanTween.delayedCall(base.gameObject, repeatDelay, buildAllTweensAgainNow);
		}

		public void buildAllTweensAgainNow()
		{
			buildAllTweens(false);
		}

		public LTDescr append(string method, float to, float duration)
		{
			codeBuild.Append(tabs + "LeanTween." + method + "(gameObject, " + to + "f, " + duration + "f)");
			return null;
		}

		public LTDescr appendRect(string method, float to, float duration)
		{
			codeBuild.Append(tabs + "LeanTween." + method + "(GetComponent<RectTransform>(), " + to + "f, " + duration + "f)");
			return null;
		}

		public LTDescr append(string method, Vector3 to, float duration)
		{
			codeBuild.Append(tabs + "LeanTween." + method + "(gameObject, " + vecToStr(to) + ", " + duration + "f)");
			return null;
		}

		public LTDescr appendRect(string method, Vector3 to, float duration)
		{
			codeBuild.Append(tabs + "LeanTween." + method + "(GetComponent<RectTransform>(), " + vecToStr(to) + ", " + duration + "f)");
			return null;
		}

		public LTDescr appendRect(string method, Color color, float duration)
		{
			codeBuild.Append(tabs + "LeanTween." + method + "(GetComponent<RectTransform>(), " + colorToStr(color) + ", " + duration + "f)");
			return null;
		}

		public LTDescr append(string method, Color color, float duration)
		{
			codeBuild.Append(tabs + "LeanTween." + method + "(gameObject, " + colorToStr(color) + ", " + duration + "f)");
			return null;
		}

		public LTDescr append(string method, Vector3[] to, float duration)
		{
			codeBuild.Append(tabs + "LeanTween." + method + "(gameObject, new Vector3[]{");
			if (to == null)
			{
				codeBuild.Append("null");
			}
			else
			{
				for (int i = 0; i < to.Length; i++)
				{
					codeBuild.Append(vecToStr(to[i]));
					if (i < to.Length - 1)
					{
						codeBuild.Append(", ");
					}
				}
			}
			codeBuild.Append("} , " + duration + "f)");
			return null;
		}

		public void append(AnimationCurve curve)
		{
			codeBuild.Append("new AnimationCurve(");
			for (int i = 0; i < curve.length; i++)
			{
				codeBuild.Append("new Keyframe(" + curve[i].time + "f," + curve[i].value + "f)");
				if (i < curve.length - 1)
				{
					codeBuild.Append(", ");
				}
			}
			codeBuild.Append(")");
		}

		private string vecToStr(Vector3 vec3)
		{
			return "new Vector3(" + vec3.x + "f," + vec3.y + "f," + vec3.z + "f)";
		}

		private string colorToStr(Color color)
		{
			return "new Color(" + color.r + "f," + color.g + "f," + color.b + "f," + color.a + "f)";
		}

		private void buildTween(LeanTweenItem item, float delayAdd, bool generateCode)
		{
			float num = item.delay + delayAdd;
			bool flag = generateCode;
			float duration = item.duration;
			if (item.gameObject == null)
			{
				item.gameObject = base.gameObject;
			}
			if (item.action == TweenAction.ALPHA)
			{
				tween = ((!flag) ? LeanTween.alpha(item.gameObject, item.to.x, duration) : append("alpha", item.to.x, duration));
			}
			else if (item.action == TweenAction.ALPHA_VERTEX)
			{
				tween = ((!flag) ? LeanTween.alphaVertex(item.gameObject, item.to.x, duration) : append("alphaVertex", item.to.x, duration));
			}
			else if (item.action == TweenAction.MOVE)
			{
				tween = ((!flag) ? LeanTween.move(item.gameObject, item.to, duration) : append("move", item.to, duration));
			}
			else if (item.action == TweenAction.MOVE_LOCAL)
			{
				tween = ((!flag) ? LeanTween.moveLocal(item.gameObject, item.to, duration) : append("moveLocal", item.to, duration));
			}
			else if (item.action == TweenAction.MOVE_LOCAL_X)
			{
				tween = ((!flag) ? LeanTween.moveLocalX(item.gameObject, item.to.x, duration) : append("moveLocalX", item.to.x, duration));
			}
			else if (item.action == TweenAction.MOVE_LOCAL_Y)
			{
				tween = ((!flag) ? LeanTween.moveLocalY(item.gameObject, item.to.x, duration) : append("moveLocalY", item.to.x, duration));
			}
			else if (item.action == TweenAction.MOVE_LOCAL_Z)
			{
				tween = ((!flag) ? LeanTween.moveLocalZ(item.gameObject, item.to.x, duration) : append("moveLocalZ", item.to.x, duration));
			}
			else if (item.action == TweenAction.MOVE_X)
			{
				tween = ((!flag) ? LeanTween.moveX(item.gameObject, item.to.x, duration) : append("moveX", item.to.x, duration));
			}
			else if (item.action == TweenAction.MOVE_Y)
			{
				tween = ((!flag) ? LeanTween.moveY(item.gameObject, item.to.x, duration) : append("moveY", item.to.x, duration));
			}
			else if (item.action == TweenAction.MOVE_Z)
			{
				tween = ((!flag) ? LeanTween.moveZ(item.gameObject, item.to.x, duration) : append("moveZ", item.to.x, duration));
			}
			else if (item.action == TweenAction.MOVE_CURVED)
			{
				tween = ((!flag) ? LeanTween.move(item.gameObject, item.bezierPath.vec3, duration) : append("move", (!item.bezierPath) ? null : item.bezierPath.vec3, duration));
				if (item.orientToPath)
				{
					if (flag)
					{
						codeBuild.Append(".setOrientToPath(" + item.orientToPath.ToString().ToLower() + ")");
					}
					else
					{
						tween.setOrientToPath(item.orientToPath);
					}
				}
				if (item.isPath2d)
				{
					if (flag)
					{
						codeBuild.Append(".setOrientToPath2d(true)");
					}
					else
					{
						tween.setOrientToPath2d(item.isPath2d);
					}
				}
			}
			else if (item.action == TweenAction.MOVE_CURVED_LOCAL)
			{
				tween = ((!flag) ? LeanTween.moveLocal(item.gameObject, item.bezierPath.vec3, duration) : append("moveLocal", (!item.bezierPath) ? null : item.bezierPath.vec3, duration));
				if (item.orientToPath)
				{
					if (flag)
					{
						codeBuild.Append(".setOrientToPath(" + item.orientToPath + ")");
					}
					else
					{
						tween.setOrientToPath(item.orientToPath);
					}
				}
				if (item.isPath2d)
				{
					if (flag)
					{
						codeBuild.Append(".setOrientToPath2d(true)");
					}
					else
					{
						tween.setOrientToPath2d(item.isPath2d);
					}
				}
			}
			else if (item.action == TweenAction.MOVE_SPLINE)
			{
				tween = ((!flag) ? LeanTween.moveSpline(item.gameObject, item.splinePath.splineVector(), duration) : append("moveSpline", (!item.splinePath) ? null : item.splinePath.splineVector(), duration));
				if (item.orientToPath)
				{
					if (flag)
					{
						codeBuild.Append(".setOrientToPath(" + item.orientToPath + ")");
					}
					else
					{
						tween.setOrientToPath(item.orientToPath);
					}
				}
				if (item.isPath2d)
				{
					if (flag)
					{
						codeBuild.Append(".setOrientToPath2d(true)");
					}
					else
					{
						tween.setOrientToPath2d(item.isPath2d);
					}
				}
			}
			else if (item.action == TweenAction.ROTATE)
			{
				tween = ((!flag) ? LeanTween.rotate(item.gameObject, item.to, duration) : append("rotate", item.to, duration));
			}
			else if (item.action == TweenAction.ROTATE_AROUND)
			{
				if (generateCode)
				{
					codeBuild.Append(tabs + "LeanTween.rotateAround(gameObject, " + vecToStr(item.axis) + ", " + item.to.x + "f , " + duration + "f)");
				}
				else
				{
					tween = LeanTween.rotateAround(item.gameObject, item.axis, item.to.x, duration);
				}
			}
			else if (item.action == TweenAction.ROTATE_AROUND_LOCAL)
			{
				if (generateCode)
				{
					codeBuild.Append(tabs + "LeanTween.rotateAroundLocal(gameObject, " + vecToStr(item.axis) + ", " + item.to.x + "f , " + duration + "f)");
				}
				else
				{
					tween = LeanTween.rotateAroundLocal(item.gameObject, item.axis, item.to.x, duration);
				}
			}
			else if (item.action == TweenAction.ROTATE_LOCAL)
			{
				tween = ((!flag) ? LeanTween.rotateLocal(item.gameObject, item.to, duration) : append("rotateLocal", item.to, duration));
			}
			else if (item.action == TweenAction.ROTATE_X)
			{
				tween = ((!flag) ? LeanTween.rotateX(item.gameObject, item.to.x, duration) : append("rotateX", item.to.x, duration));
			}
			else if (item.action == TweenAction.ROTATE_Y)
			{
				tween = ((!flag) ? LeanTween.rotateY(item.gameObject, item.to.x, duration) : append("rotateY", item.to.x, duration));
			}
			else if (item.action == TweenAction.ROTATE_Z)
			{
				tween = ((!flag) ? LeanTween.rotateZ(item.gameObject, item.to.x, duration) : append("rotateZ", item.to.x, duration));
			}
			else if (item.action == TweenAction.SCALE)
			{
				tween = ((!flag) ? LeanTween.scale(item.gameObject, item.to, duration) : append("scale", item.to, duration));
			}
			else if (item.action == TweenAction.SCALE_X)
			{
				tween = ((!flag) ? LeanTween.scaleX(item.gameObject, item.to.x, duration) : append("scaleX", item.to.x, duration));
			}
			else if (item.action == TweenAction.SCALE_Y)
			{
				tween = ((!flag) ? LeanTween.scaleY(item.gameObject, item.to.x, duration) : append("scaleY", item.to.x, duration));
			}
			else if (item.action == TweenAction.SCALE_Z)
			{
				tween = ((!flag) ? LeanTween.scaleZ(item.gameObject, item.to.x, duration) : append("scaleZ", item.to.x, duration));
			}
			else if (item.action == TweenAction.CANVAS_MOVE)
			{
				tween = ((!flag) ? LeanTween.move(item.gameObject.GetComponent<RectTransform>(), item.to, duration) : appendRect("move", item.to, duration));
			}
			else if (item.action == TweenAction.CANVAS_SCALE)
			{
				tween = ((!flag) ? LeanTween.scale(item.gameObject.GetComponent<RectTransform>(), item.to, duration) : appendRect("scale", item.to, duration));
			}
			else if (item.action == TweenAction.CANVAS_ROTATEAROUND)
			{
				if (generateCode)
				{
					codeBuild.Append(tabs + "LeanTween.rotateAround(GetComponent<RectTransform>(), " + vecToStr(item.axis) + ", " + item.to.x + "f , " + duration + "f)");
				}
				else
				{
					tween = LeanTween.rotateAround(item.gameObject.GetComponent<RectTransform>(), item.axis, item.to.x, duration);
				}
			}
			else if (item.action == TweenAction.CANVAS_ROTATEAROUND_LOCAL)
			{
				if (generateCode)
				{
					codeBuild.Append(tabs + "LeanTween.rotateAroundLocal(GetComponent<RectTransform>(), " + vecToStr(item.axis) + ", " + item.to.x + "f , " + duration + "f)");
				}
				else
				{
					tween = LeanTween.rotateAroundLocal(item.gameObject.GetComponent<RectTransform>(), item.axis, item.to.x, duration);
				}
			}
			else if (item.action == TweenAction.CANVAS_ALPHA)
			{
				tween = ((!flag) ? LeanTween.alpha(item.gameObject.GetComponent<RectTransform>(), item.to.x, duration) : appendRect("alpha", item.to.x, duration));
			}
			else if (item.action == TweenAction.CANVAS_COLOR)
			{
				tween = ((!flag) ? LeanTween.color(item.gameObject.GetComponent<RectTransform>(), item.colorTo, duration) : appendRect("color", item.colorTo, duration));
			}
			else if (item.action == TweenAction.TEXT_ALPHA)
			{
				tween = ((!flag) ? LeanTween.textAlpha(item.gameObject.GetComponent<RectTransform>(), item.to.x, duration) : appendRect("textAlpha", item.to.x, duration));
			}
			else if (item.action == TweenAction.TEXT_COLOR)
			{
				tween = ((!flag) ? LeanTween.textColor(item.gameObject.GetComponent<RectTransform>(), item.colorTo, duration) : appendRect("textColor", item.colorTo, duration));
			}
			else if (item.action == TweenAction.CANVAS_PLAYSPRITE)
			{
				if (generateCode)
				{
					codeBuild.Append(tabs + "LeanTween.play(GetComponent<RectTransform>(), new UnityEngine.Sprite[" + item.sprites.Length + "]).setFrameRate(" + item.frameRate + "f)");
				}
				else
				{
					tween = LeanTween.play(item.gameObject.GetComponent<RectTransform>(), item.sprites).setFrameRate(item.frameRate);
				}
			}
			else if (item.action == TweenAction.COLOR)
			{
				tween = ((!flag) ? LeanTween.color(item.gameObject, item.colorTo, duration) : append("color", item.colorTo, duration));
			}
			else
			{
				if (item.action != TweenAction.DELAYED_SOUND)
				{
					tween = null;
					Debug.Log("The tween '" + item.action.ToString() + "' has not been implemented. info item:" + item);
					return;
				}
				if (generateCode)
				{
					codeBuild.Append(tabs + "LeanTween.delayedSound(gameObject, passAudioClipHere, " + vecToStr(item.from) + ", " + duration + "f)");
				}
				else
				{
					tween = LeanTween.delayedSound(base.gameObject, item.audioClip, item.from, item.duration);
				}
			}
			if (generateCode)
			{
				if (num > 0f)
				{
					codeBuild.Append(".setDelay(" + num + "f)");
				}
			}
			else
			{
				tween = tween.setDelay(num);
			}
			if (item.ease == LeanTweenType.animationCurve)
			{
				if (generateCode)
				{
					codeBuild.Append(".setEase(");
					append(item.animationCurve);
					codeBuild.Append(")");
				}
				else
				{
					tween.setEase(item.animationCurve);
				}
			}
			else if (generateCode)
			{
				if (item.ease != LeanTweenType.linear)
				{
					codeBuild.Append(string.Concat(".setEase(LeanTweenType.", item.ease, ")"));
				}
			}
			else
			{
				tween.setEase(item.ease);
			}
			if (item.between == LeanTweenBetween.FromTo)
			{
				if (generateCode)
				{
					codeBuild.Append(".setFrom(" + vecToStr(item.from) + ")");
				}
				else
				{
					tween.setFrom(item.from);
				}
			}
			if (item.doesLoop)
			{
				if (generateCode)
				{
					codeBuild.Append(".setRepeat(" + item.loopCount + ")");
				}
				else
				{
					tween.setRepeat(item.loopCount);
				}
				if (item.loopType == LeanTweenType.pingPong)
				{
					if (generateCode)
					{
						codeBuild.Append(".setLoopPingPong()");
					}
					else
					{
						tween.setLoopPingPong();
					}
				}
			}
			if (generateCode)
			{
				if (item.useSpeed)
				{
					codeBuild.Append(".setSpeed(" + item.speed + "f)");
				}
			}
			else if (item.useSpeed)
			{
				tween.setSpeed(item.speed);
			}
			if (generateCode)
			{
				codeBuild.Append(";\n");
			}
		}

		public void buildGroup(object g)
		{
			LeanTweenGroup leanTweenGroup = (LeanTweenGroup)g;
			if (leanTweenGroup.gameObject == null)
			{
				leanTweenGroup.gameObject = base.gameObject;
			}
			if (leanTweenGroup.overrideGameObject != null)
			{
				leanTweenGroup.gameObject = leanTweenGroup.overrideGameObject;
			}
			foreach (LeanTweenItem item in leanTweenGroup.itemList)
			{
				if (leanTweenGroup.overrideGameObject != null)
				{
					item.gameObject = leanTweenGroup.overrideGameObject;
				}
				buildTween(item, leanTweenGroup.delay, leanTweenGroup.generateCode);
			}
			if (!leanTweenGroup.generateCode && leanTweenGroup.repeat && (leanTweenGroup.repeatCount < 0 || leanTweenGroup.repeatIter < leanTweenGroup.repeatCount - 1))
			{
				LeanTween.delayedCall(leanTweenGroup.gameObject, leanTweenGroup.endTime, buildGroup).setOnCompleteParam(leanTweenGroup).setDelay(leanTweenGroup.delay);
				leanTweenGroup.repeatIter++;
			}
		}

		public string buildAllTweens(bool generateCode, GameObject overrideGameObject = null)
		{
			if (generateCode)
			{
				codeBuild = new StringBuilder(1024);
				tabs = string.Empty;
				if (repeat)
				{
					tabs += "\t";
				}
			}
			else
			{
				LeanTween.cancel(base.gameObject);
			}
			string text = tabs;
			float num = 0f;
			int num2 = 0;
			foreach (LeanTweenGroup group in groupList)
			{
				bool flag = (group.repeat && group.itemList.Count > 0) || group.delay > 0f;
				if (generateCode)
				{
					if (num2 != 0)
					{
						codeBuild.Append("\n");
					}
					codeBuild.Append(tabs + "// " + group.name + " Group\n");
				}
				if (generateCode && flag)
				{
					codeBuild.Append(tabs + "LeanTween.delayedCall(gameObject, " + group.endTime + "f, ()=>{\n");
					tabs += "\t";
				}
				if (generateCode)
				{
					group.setGenerateCode();
				}
				group.repeatIter = 0;
				group.overrideGameObject = overrideGameObject;
				buildGroup(group);
				tabs = text;
				if (generateCode && flag)
				{
					if (group.delay > 0f)
					{
						codeBuild.Append(".setDelay(" + group.delay + "f)");
					}
					codeBuild.Append(tabs + "}).setOnCompleteOnStart(true)");
					if (group.repeat)
					{
						codeBuild.Append(".setRepeat(" + ((group.repeatCount >= 0) ? (group.repeatCount - 1) : (-1)) + ")");
					}
					codeBuild.Append(";\n");
				}
				if (group.endTime > num)
				{
					num = group.endTime;
				}
				num2++;
			}
			if (repeat)
			{
				if (generateCode)
				{
					codeBuild.Insert(0, "LeanTween.delayedCall(gameObject, " + num + "f, ()=>{\n");
					codeBuild.Append("}).setRepeat(" + repeatIter + ").setOnCompleteOnStart(true);\n");
				}
				else
				{
					repeatIter = 0;
					LeanTween.delayedCall(base.gameObject, num, buildAllTweensAgain);
				}
			}
			if (generateCode)
			{
				return codeBuild.ToString();
			}
			return null;
		}
	}
}
