using System.Collections;
using UnityEngine;

public class SwordTappingIcon : MonoBehaviour
{
	public SpriteRenderer spriteRenderer;

	public Material defaultSpriteMat;

	public Material enabledSpriteMat;

	public Material perfectSpriteMat;

	public Material inWindowMat;

	public Material outsideWindowMat;

	public Material perfectMat;

	private MeshRenderer meshRendererWeapon;

	private SkinnedMeshRenderer skinnedMeshRendererWeapon;

	private Material weaponMaterial;

	private float startTime;

	public float graceWindow = 0.3f;

	public float beginWindow = 0.5f;

	public float endWindow = 1.2f;

	public float duration = 1.2f;

	public float perfectBeginWindow = 0.9f;

	public float perfectEndWindows = 1f;

	private bool isInWindow;

	private float blinkStartTime;

	private float blinkInterval = 0.3f;

	private bool blinkUp = true;

	private bool isActive;

	private bool isInPerfectWindow;

	private bool enablePerfect;

	public Vector3 originalScale;

	public float targetScaleFactor = 0.75f;

	public float preShow = 0.05f;

	private float hitRadius;

	private float minHitRadius;

	private Vector3 _localTopLeft;

	private Vector3 _localTopRight;

	public bool IsInWindow
	{
		get
		{
			return isActive && isInWindow;
		}
	}

	public bool IsInPerfectWindow
	{
		get
		{
			return isActive && isInPerfectWindow;
		}
	}

	public bool IsDone
	{
		get
		{
			return !isActive && startTime + duration < Time.time;
		}
	}

	public bool IsInGrace
	{
		get
		{
			return isActive && startTime + graceWindow >= Time.time;
		}
	}

	private void Awake()
	{
		spriteRenderer.enabled = false;
		originalScale = base.transform.localScale;
		inWindowMat.renderQueue = -1;
		outsideWindowMat.renderQueue = -1;
		perfectMat.renderQueue = -1;
		defaultSpriteMat.renderQueue = -1;
		enabledSpriteMat.renderQueue = -1;
		perfectSpriteMat.renderQueue = -1;
		Quaternion rotation = base.transform.rotation;
		base.transform.rotation = Quaternion.identity;
		Vector3 extents = spriteRenderer.bounds.extents;
		_localTopLeft = new Vector3(0f - extents.x, extents.y, 0f) + base.transform.position;
		_localTopLeft = base.transform.InverseTransformPoint(_localTopLeft);
		_localTopRight = new Vector3(extents.x, extents.y, 0f) + base.transform.position;
		_localTopRight = base.transform.InverseTransformPoint(_localTopRight);
		base.transform.rotation = rotation;
		hitRadius = 0f;
		minHitRadius = 40f / 127f * Screen.dpi;
	}

	private void Start()
	{
		Initialize();
	}

	public void Initialize()
	{
		StartCoroutine(DoInitialize());
	}

	private IEnumerator DoInitialize()
	{
		yield return 0;
		Character character = base.gameObject.GetComponentInParent<Character>();
		if ((bool)character && character.isPlayer)
		{
			meshRendererWeapon = character.SwordTransform.GetComponentInChildren<Weapon>().GetComponentInChildren<MeshRenderer>();
			if (meshRendererWeapon != null)
			{
				weaponMaterial = meshRendererWeapon.material;
				yield break;
			}
			skinnedMeshRendererWeapon = character.SwordTransform.GetComponentInChildren<Weapon>().GetComponentInChildren<SkinnedMeshRenderer>();
			if (skinnedMeshRendererWeapon != null)
			{
				weaponMaterial = skinnedMeshRendererWeapon.material;
			}
			else
			{
				Debug.LogWarning("No renderer found for weapon");
			}
		}
		else
		{
			if (!character)
			{
				yield break;
			}
			meshRendererWeapon = character.SwordTransform.GetComponentInChildren<MeshRenderer>();
			if (meshRendererWeapon != null)
			{
				weaponMaterial = meshRendererWeapon.material;
				yield break;
			}
			skinnedMeshRendererWeapon = character.SwordTransform.GetComponentInChildren<SkinnedMeshRenderer>();
			if (skinnedMeshRendererWeapon != null)
			{
				weaponMaterial = skinnedMeshRendererWeapon.material;
			}
			else
			{
				Debug.LogWarning("No renderer found for weapon");
			}
		}
	}

	public void Show(bool visible, bool enablePerfect = false)
	{
		if (visible)
		{
			isInWindow = false;
			isInPerfectWindow = false;
			isActive = true;
			startTime = Time.time;
			base.transform.localScale = originalScale;
			this.enablePerfect = enablePerfect;
		}
		else
		{
			spriteRenderer.enabled = false;
			if (meshRendererWeapon != null && weaponMaterial != null)
			{
				meshRendererWeapon.material = weaponMaterial;
			}
			else if (skinnedMeshRendererWeapon != null && weaponMaterial != null)
			{
				skinnedMeshRendererWeapon.material = weaponMaterial;
			}
		}
	}

	public bool IsPointInsideIcon(Vector2 point)
	{
		Vector2 b = Camera.main.WorldToScreenPoint(base.transform.position);
		float num = Vector2.Distance(point, b);
		if (num <= hitRadius)
		{
			return true;
		}
		return false;
	}

	private void Update()
	{
		if (!isActive)
		{
			return;
		}
		if (GameController.Instance.PlayerProfile.HasSeenBriefing(PlayerBriefingType.PlayerAttack))
		{
			float num = Time.time - startTime;
			if (num < beginWindow)
			{
				spriteRenderer.enabled = false;
			}
			else if (num > beginWindow && num < endWindow && !isInWindow)
			{
				isInWindow = true;
				blinkStartTime = Time.time;
				if (GameController.Instance.PlayerProfile.Level <= 0)
				{
					spriteRenderer.material = enabledSpriteMat;
				}
				if (meshRendererWeapon != null)
				{
					meshRendererWeapon.material = inWindowMat;
				}
				else if (skinnedMeshRendererWeapon != null)
				{
					skinnedMeshRendererWeapon.material = inWindowMat;
				}
			}
			else if (num > perfectBeginWindow && num < perfectEndWindows && !isInPerfectWindow && enablePerfect)
			{
				isInPerfectWindow = true;
				if (GameController.Instance.PlayerProfile.Level <= 0)
				{
					spriteRenderer.material = perfectSpriteMat;
				}
				if (meshRendererWeapon != null)
				{
					meshRendererWeapon.material = perfectMat;
				}
				else if (skinnedMeshRendererWeapon != null)
				{
					skinnedMeshRendererWeapon.material = perfectMat;
				}
			}
			else if (num > perfectEndWindows && isInPerfectWindow)
			{
				isInPerfectWindow = false;
				if (GameController.Instance.PlayerProfile.Level <= 0)
				{
					spriteRenderer.material = enabledSpriteMat;
				}
				if (meshRendererWeapon != null)
				{
					meshRendererWeapon.material = inWindowMat;
				}
				else if (skinnedMeshRendererWeapon != null)
				{
					skinnedMeshRendererWeapon.material = inWindowMat;
				}
			}
			else if (num > endWindow && isInWindow)
			{
				isInWindow = false;
				isActive = false;
				if (GameController.Instance.PlayerProfile.Level <= 0)
				{
					spriteRenderer.material = defaultSpriteMat;
				}
				if (meshRendererWeapon != null)
				{
					meshRendererWeapon.material = weaponMaterial;
				}
				else if (skinnedMeshRendererWeapon != null)
				{
					skinnedMeshRendererWeapon.material = weaponMaterial;
				}
			}
			else if (num + preShow > endWindow && isInWindow && spriteRenderer.material != defaultSpriteMat)
			{
				if (GameController.Instance.PlayerProfile.Level <= 0)
				{
					spriteRenderer.material = defaultSpriteMat;
				}
				if (meshRendererWeapon != null)
				{
					meshRendererWeapon.material = weaponMaterial;
				}
				else if (skinnedMeshRendererWeapon != null)
				{
					skinnedMeshRendererWeapon.material = weaponMaterial;
				}
			}
			base.transform.localScale = Vector3.Lerp(originalScale, originalScale * targetScaleFactor, num / duration);
			if (isInWindow)
			{
				float num2 = 0f;
				num = (Time.time - blinkStartTime) / blinkInterval;
				num2 = ((!blinkUp) ? Mathf.Lerp(1f, 0f, num) : Mathf.Lerp(0f, 1f, num));
				if (num >= 1f)
				{
					blinkUp = !blinkUp;
				}
				if (meshRendererWeapon != null)
				{
					meshRendererWeapon.material.SetFloat("_OutlineSwitch", num2);
				}
				else if (skinnedMeshRendererWeapon != null)
				{
					skinnedMeshRendererWeapon.material.SetFloat("_OutlineSwitch", num2);
				}
			}
			else if (meshRendererWeapon != null)
			{
				meshRendererWeapon.material.SetFloat("_OutlineSwitch", 0f);
			}
			else if (skinnedMeshRendererWeapon != null)
			{
				skinnedMeshRendererWeapon.material.SetFloat("_OutlineSwitch", 0f);
			}
			return;
		}
		float num3 = Time.time - startTime;
		if (num3 < beginWindow)
		{
			spriteRenderer.enabled = false;
		}
		else if (num3 > beginWindow && !isInWindow)
		{
			isInWindow = true;
			blinkStartTime = Time.time;
			if (GameController.Instance.PlayerProfile.Level <= 0)
			{
				spriteRenderer.material = enabledSpriteMat;
			}
			if (meshRendererWeapon != null)
			{
				meshRendererWeapon.material = inWindowMat;
			}
			else if (skinnedMeshRendererWeapon != null)
			{
				skinnedMeshRendererWeapon.material = inWindowMat;
			}
		}
	}

	private void LateUpdate()
	{
		if (!isActive || !base.transform.hasChanged)
		{
			return;
		}
		if (Camera.main != null)
		{
			base.transform.LookAt(Camera.main.transform.position, Vector3.up);
			base.transform.Rotate(Vector3.up, 180f);
			Vector3 position = base.transform.TransformPoint(_localTopLeft);
			Vector3 position2 = base.transform.TransformPoint(_localTopRight);
			hitRadius = Camera.main.WorldToScreenPoint(position2).x - Camera.main.WorldToScreenPoint(position).x;
			hitRadius = 0.5f * Mathf.Abs(hitRadius);
			hitRadius = 0.1f * hitRadius + hitRadius;
			if (hitRadius < minHitRadius)
			{
				hitRadius = minHitRadius;
			}
		}
		base.transform.hasChanged = false;
	}

	public void Freeze()
	{
		isActive = false;
	}

	public void Unfreeze()
	{
		if (meshRendererWeapon != null)
		{
			meshRendererWeapon.material = weaponMaterial;
		}
		else if (skinnedMeshRendererWeapon != null)
		{
			skinnedMeshRendererWeapon.material = weaponMaterial;
		}
	}

	public void Apply(SwordTappingSettings settings)
	{
		graceWindow = settings.graceWindow;
		beginWindow = settings.beginWindow;
		endWindow = settings.endWindow;
		duration = settings.duration;
		perfectBeginWindow = settings.perfectBeginWindow;
		perfectEndWindows = settings.perfectEndWindows;
	}
}
