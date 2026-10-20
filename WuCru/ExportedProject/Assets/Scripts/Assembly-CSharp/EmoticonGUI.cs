using UnityEngine;
using UnityEngine.UI;

public class EmoticonGUI : MonoBehaviour
{
	public SpriteRenderer spriteRenderer;

	public Image offscreenImage;

	public Image imageNinjaBg;

	public Image imageNinjaMask;

	public RawImage imageNinja;

	private EmoticonGUIType currentType;

	public Transform owner;

	private AIAgent ownerAgent;

	private Vector3 imageNinjaOffset;

	private Vector3 imageNinjaBgOffset;

	private Vector3 imageNinjaMaskOffset;

	private SpriteBlinker blinker;

	private bool hasBlinked;

	public EmoticonGUIType CurrentType
	{
		get
		{
			return currentType;
		}
	}

	private void Awake()
	{
		spriteRenderer.enabled = false;
		offscreenImage.enabled = false;
		imageNinja.enabled = false;
		imageNinjaBg.enabled = false;
		currentType = EmoticonGUIType.None;
		imageNinjaOffset = new Vector3(0f, 0f, 0f);
		imageNinjaBgOffset = new Vector3(0f, 0f, 0f);
		imageNinjaMaskOffset = new Vector3(0f, 0f, 0f);
		blinker = spriteRenderer.gameObject.GetComponentInChildren<SpriteBlinker>();
	}

	private void Start()
	{
		ownerAgent = owner.GetComponent<AIAgent>();
	}

	private void Update()
	{
		if (!(GameController.Instance.currentState is GameControllerStatePlaying))
		{
			return;
		}
		if ((ownerAgent != null && ownerAgent.Character.IsDead()) || GamePlayer.Instance.Player.Character.CurrentStateNameHash() == StateHash.missionEnding)
		{
			if (offscreenImage.enabled)
			{
				offscreenImage.enabled = false;
			}
			if (spriteRenderer.enabled)
			{
				spriteRenderer.enabled = false;
			}
		}
		else
		{
			if (!(ownerAgent == null) && ownerAgent.Character.IsDead())
			{
				return;
			}
			if (!offscreenImage.enabled && currentType != EmoticonGUIType.None && !IsVisible() && ownerAgent != null)
			{
				imageNinja.enabled = true;
				imageNinjaBg.enabled = true;
				imageNinjaMask.enabled = true;
			}
			if (offscreenImage.enabled && (currentType == EmoticonGUIType.None || IsVisible() || (ownerAgent != null && ownerAgent.Character.IsDead())))
			{
				offscreenImage.enabled = false;
				imageNinja.enabled = false;
				imageNinjaBg.enabled = false;
				imageNinjaMask.enabled = false;
			}
			if (currentType != EmoticonGUIType.Alerted && hasBlinked)
			{
				hasBlinked = false;
			}
			spriteRenderer.gameObject.transform.localScale = Vector3.one;
			if (currentType == EmoticonGUIType.Alerted && ownerAgent != null && ownerAgent.State < AIAgentState.Alert)
			{
				if (!spriteRenderer.enabled)
				{
					spriteRenderer.enabled = true;
				}
				spriteRenderer.gameObject.transform.localScale = new Vector3(ownerAgent.SpottetPlayerMeter, ownerAgent.SpottetPlayerMeter, ownerAgent.SpottetPlayerMeter);
				if (!hasBlinked && ownerAgent.SpottetPlayerMeter > 0.99f && GamePlayer.Instance.State != GamePlayerState.Combat)
				{
					blinker.Blink(Color.white, 0.5f);
					hasBlinked = true;
				}
			}
			if (GamePlayer.Instance.State == GamePlayerState.Combat && ownerAgent != null && currentType == EmoticonGUIType.Alerted)
			{
				if (GamePlayer.Instance.CombatManager.SecondaryAttacker == ownerAgent && !spriteRenderer.enabled)
				{
					spriteRenderer.enabled = true;
				}
				else if (GamePlayer.Instance.CombatManager.SecondaryAttacker != ownerAgent && spriteRenderer.enabled)
				{
					blinker.Reset();
					spriteRenderer.enabled = false;
				}
			}
		}
	}

	private void LateUpdate()
	{
		if (!(GameController.Instance.currentState is GameControllerStatePlaying))
		{
			return;
		}
		if (spriteRenderer.enabled)
		{
			spriteRenderer.gameObject.transform.LookAt(Camera.main.transform.position, Vector3.up);
		}
		if (offscreenImage.enabled)
		{
			Vector3 zero = Vector3.zero;
			zero = ((!(ownerAgent == null)) ? Camera.main.WorldToViewportPoint(ownerAgent.Character.headTransform.position) : Camera.main.WorldToViewportPoint(owner.position));
			Vector3 vector = new Vector3(Mathf.Clamp(zero.x, 0f, 1f), Mathf.Clamp(zero.y, 0f, 1f), 0f);
			float num = vector.x * (float)Screen.width;
			float num2 = vector.y * (float)Screen.height;
			if (num + offscreenImage.rectTransform.sizeDelta.x / 2f > (float)Screen.width)
			{
				num += (float)Screen.width - (num + offscreenImage.rectTransform.sizeDelta.x / 2f);
			}
			if (num2 + offscreenImage.rectTransform.sizeDelta.y / 2f > (float)Screen.height)
			{
				num2 += (float)Screen.height - (num2 + offscreenImage.rectTransform.sizeDelta.y / 2f);
			}
			if (num - offscreenImage.rectTransform.sizeDelta.x / 2f < 0f)
			{
				num -= num - offscreenImage.rectTransform.sizeDelta.x / 2f;
			}
			if (num2 - offscreenImage.rectTransform.sizeDelta.y / 2f < 0f)
			{
				num2 -= num2 - offscreenImage.rectTransform.sizeDelta.y / 2f;
			}
			offscreenImage.transform.parent.transform.position = new Vector3(num, num2, 0f);
			imageNinja.transform.position = new Vector3(num, num2, 0f) + imageNinjaOffset;
			imageNinjaBg.transform.position = new Vector3(num, num2, 0f) + imageNinjaBgOffset;
			imageNinjaMask.transform.position = new Vector3(num, num2, 0f) + imageNinjaMaskOffset;
			Vector3 zero2 = Vector3.zero;
			zero2 = ((!(ownerAgent == null)) ? (Camera.main.ViewportToWorldPoint(offscreenImage.transform.parent.transform.position) - ownerAgent.Character.headTransform.position) : (Camera.main.ViewportToWorldPoint(offscreenImage.transform.parent.transform.position) - owner.position));
			offscreenImage.transform.parent.transform.eulerAngles = new Vector3(0f, 0f, 57.29578f * Mathf.Atan2(zero2.x, zero2.z) + 90f);
		}
		if (GamePlayer.Instance.Player.Character.CurrentStateNameHash() == StateHash.missionEnding)
		{
			if (spriteRenderer.enabled)
			{
				spriteRenderer.enabled = false;
			}
			if (offscreenImage.enabled)
			{
				offscreenImage.enabled = false;
			}
		}
	}

	public void SetEmoticon(EmoticonGUIType type)
	{
		switch (type)
		{
		case EmoticonGUIType.None:
			if (currentType != EmoticonGUIType.None)
			{
				if (spriteRenderer.enabled)
				{
					spriteRenderer.enabled = false;
				}
				currentType = type;
			}
			break;
		case EmoticonGUIType.Alerted:
			if (currentType != EmoticonGUIType.Alerted)
			{
				if (!spriteRenderer.enabled)
				{
					spriteRenderer.enabled = true;
				}
				spriteRenderer.sprite = Resources.Load<Sprite>(Paths.emoticonAlertedIcon);
				offscreenImage.sprite = Resources.Load<Sprite>(Paths.emoticonAlertedOffscreen);
				SetNinjaImage();
				currentType = type;
			}
			break;
		case EmoticonGUIType.Suspicious:
			if (currentType != EmoticonGUIType.Suspicious)
			{
				if (!spriteRenderer.enabled)
				{
					spriteRenderer.enabled = true;
				}
				spriteRenderer.sprite = Resources.Load<Sprite>(Paths.emoticonSuspicious);
				offscreenImage.sprite = Resources.Load<Sprite>(Paths.emoticonSuspiciousOffscreen);
				SetNinjaImage();
				currentType = type;
			}
			break;
		case EmoticonGUIType.Door:
			if (currentType != EmoticonGUIType.Door)
			{
				if (!spriteRenderer.enabled)
				{
					spriteRenderer.enabled = true;
				}
				offscreenImage.sprite = Resources.Load<Sprite>(Paths.emoticonDoorOffscreen);
				spriteRenderer.sprite = null;
				currentType = type;
				Debug.Log("Set Door Icon", offscreenImage);
			}
			break;
		case EmoticonGUIType.Attacking:
			if (currentType != EmoticonGUIType.Attacking)
			{
				if (!spriteRenderer.enabled)
				{
					spriteRenderer.enabled = true;
				}
				spriteRenderer.sprite = Resources.Load<Sprite>(Paths.emoticonAttackingIcon);
				offscreenImage.sprite = Resources.Load<Sprite>(Paths.emoticonAttackingOffscreen);
				currentType = type;
			}
			break;
		default:
			Debug.LogError("Unknown emoticion type...");
			break;
		}
	}

	private bool SetNinjaImage()
	{
		if (ownerAgent == null)
		{
			return false;
		}
		RenderTexture renderTexture = imageNinja.texture as RenderTexture;
		if ((bool)renderTexture)
		{
			renderTexture.Release();
		}
		int[] ids = new int[1] { ownerAgent.Character.NinjaID };
		Texture2D[] array = CharacterPreviewRenderer.Generate(ids, (int)imageNinja.rectTransform.sizeDelta.x, (int)imageNinja.rectTransform.sizeDelta.y);
		imageNinja.texture = array[0];
		return true;
	}

	public void OnDestroy()
	{
		RenderTexture renderTexture = imageNinja.texture as RenderTexture;
		if ((bool)renderTexture)
		{
			renderTexture.Release();
		}
	}

	private bool IsVisible()
	{
		Vector3 zero = Vector3.zero;
		zero = ((!(ownerAgent == null)) ? Camera.main.WorldToViewportPoint(ownerAgent.Character.headTransform.position) : Camera.main.WorldToViewportPoint(owner.position));
		if (zero.x <= 0f || zero.x >= 1f || zero.y <= 0f || zero.y >= 1f)
		{
			return false;
		}
		return true;
	}
}
