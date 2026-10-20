using System;
using UnityEngine;

namespace Agens.Stickers
{
	[Serializable]
	public class StickerPackIcon
	{
		[Serializable]
		public class IconExportSettings
		{
			public Color BackgroundColor = Color.white;

			[Range(0f, 100f)]
			public int FillPercentage = 100;

			public FilterMode FilterMode = FilterMode.Trilinear;

			public ScaleMode ScaleMode = ScaleMode.ScaleToFit;
		}

		[Header("1024 x 768 px")]
		[SerializeField]
		private Texture2D appStore;

		public IconExportSettings Settings;

		public bool Override;

		[Header("148 x 110 px")]
		[SerializeField]
		private Texture2D messagesiPadPro2;

		[Header("134 x 100 px")]
		[SerializeField]
		private Texture2D messagesiPad2;

		[Header("120 x 90 px")]
		[SerializeField]
		private Texture2D messagesiPhone2;

		[Header("180 x 135 px")]
		[SerializeField]
		private Texture2D messagesiPhone3;

		[Header("54 x 40 px")]
		[SerializeField]
		private Texture2D messagesSmall2;

		[Header("81 x 60 px")]
		[SerializeField]
		private Texture2D messagesSmall3;

		[Header("64 x 48 px")]
		[SerializeField]
		private Texture2D messages2;

		[Header("96 x 72 px")]
		[SerializeField]
		private Texture2D messages3;

		[Header("58 x 58 px")]
		[SerializeField]
		private Texture2D iPhoneSettings2;

		[Header("87 x 87 px")]
		[SerializeField]
		private Texture2D iPhoneSettings3;

		[Header("58 x 58 px")]
		[SerializeField]
		private Texture2D iPadSettings2;

		public StickerIcon[] Icons
		{
			get
			{
				return new StickerIcon[12]
				{
					AppStoreIcon, MessagesIpadPro2Icon, MessagesIpad2Icon, MessagesiPhone2Icon, MessagesiPhone3Icon, MessagesSmall2Icon, MessagesSmall3Icon, Messages2Icon, Messages3Icon, IPhoneSettings2Icon,
					IPhoneSettings3Icon, IPadSettings2Icon
				};
			}
		}

		public Texture2D[] Textures
		{
			get
			{
				return new Texture2D[12]
				{
					AppStore, MessagesIpadPro2, MessagesIpad2, MessagesiPhone2, MessagesiPhone3, MessagesSmall2, MessagesSmall3, Messages2, Messages3, IPhoneSettings2,
					IPhoneSettings3, IPadSettings2
				};
			}
		}

		public Vector2[] Sizes
		{
			get
			{
				return new Vector2[12]
				{
					new Vector2(1024f, 768f),
					new Vector2(148f, 110f),
					new Vector2(134f, 100f),
					new Vector2(120f, 90f),
					new Vector2(180f, 135f),
					new Vector2(54f, 40f),
					new Vector2(81f, 60f),
					new Vector2(64f, 48f),
					new Vector2(96f, 72f),
					new Vector2(58f, 58f),
					new Vector2(87f, 87f),
					new Vector2(58f, 58f)
				};
			}
		}

		public Texture2D AppStore
		{
			get
			{
				if (Override)
				{
					return appStore;
				}
				return GetDefaultTexture(1024, 768);
			}
		}

		public StickerIcon AppStoreIcon
		{
			get
			{
				Texture2D texture2D = AppStore;
				if (texture2D != null)
				{
					return new StickerIcon(texture2D, 1024, 768, StickerIcon.Idiom.IosMarketing, StickerIcon.Scale.Original, "ios");
				}
				return null;
			}
		}

		public Texture2D MessagesIpadPro2
		{
			get
			{
				if (Override)
				{
					return messagesiPadPro2;
				}
				return GetDefaultTexture(148, 110);
			}
		}

		public StickerIcon MessagesIpadPro2Icon
		{
			get
			{
				Texture2D messagesIpadPro = MessagesIpadPro2;
				if (messagesIpadPro != null)
				{
					return new StickerIcon(messagesIpadPro, 74, 55, StickerIcon.Idiom.Ipad);
				}
				return null;
			}
		}

		public Texture2D MessagesIpad2
		{
			get
			{
				if (Override)
				{
					return messagesiPad2;
				}
				return GetDefaultTexture(134, 100);
			}
		}

		public StickerIcon MessagesIpad2Icon
		{
			get
			{
				Texture2D messagesIpad = MessagesIpad2;
				if (messagesIpad != null)
				{
					return new StickerIcon(messagesIpad, 67, 50, StickerIcon.Idiom.Ipad);
				}
				return null;
			}
		}

		public Texture2D MessagesiPhone2
		{
			get
			{
				if (Override)
				{
					return messagesiPhone2;
				}
				return GetDefaultTexture(120, 90);
			}
		}

		public StickerIcon MessagesiPhone2Icon
		{
			get
			{
				Texture2D texture2D = MessagesiPhone2;
				if (texture2D != null)
				{
					return new StickerIcon(texture2D, 60, 45, StickerIcon.Idiom.Iphone);
				}
				return null;
			}
		}

		public Texture2D MessagesiPhone3
		{
			get
			{
				if (Override)
				{
					return messagesiPhone3;
				}
				return GetDefaultTexture(180, 135);
			}
		}

		public StickerIcon MessagesiPhone3Icon
		{
			get
			{
				Texture2D texture2D = MessagesiPhone3;
				if (texture2D != null)
				{
					return new StickerIcon(texture2D, 60, 45, StickerIcon.Idiom.Iphone, StickerIcon.Scale.Triple);
				}
				return null;
			}
		}

		public Texture2D MessagesSmall2
		{
			get
			{
				if (Override)
				{
					return messagesSmall2;
				}
				return GetDefaultTexture(54, 40);
			}
		}

		public StickerIcon MessagesSmall2Icon
		{
			get
			{
				Texture2D texture2D = MessagesSmall2;
				if (texture2D != null)
				{
					return new StickerIcon(texture2D, 27, 20, StickerIcon.Idiom.Universal, StickerIcon.Scale.Double, "ios");
				}
				return null;
			}
		}

		public Texture2D MessagesSmall3
		{
			get
			{
				if (Override)
				{
					return messagesSmall3;
				}
				return GetDefaultTexture(81, 60);
			}
		}

		public StickerIcon MessagesSmall3Icon
		{
			get
			{
				Texture2D texture2D = MessagesSmall3;
				if (texture2D != null)
				{
					return new StickerIcon(texture2D, 27, 20, StickerIcon.Idiom.Universal, StickerIcon.Scale.Triple, "ios");
				}
				return null;
			}
		}

		public Texture2D Messages2
		{
			get
			{
				if (Override)
				{
					return messages2;
				}
				return GetDefaultTexture(64, 48);
			}
		}

		public StickerIcon Messages2Icon
		{
			get
			{
				Texture2D texture2D = Messages2;
				if (texture2D != null)
				{
					return new StickerIcon(texture2D, 32, 24, StickerIcon.Idiom.Universal, StickerIcon.Scale.Double, "ios");
				}
				return null;
			}
		}

		public Texture2D Messages3
		{
			get
			{
				if (Override)
				{
					return messages3;
				}
				return GetDefaultTexture(96, 72);
			}
		}

		public StickerIcon Messages3Icon
		{
			get
			{
				Texture2D texture2D = Messages3;
				if (texture2D != null)
				{
					return new StickerIcon(texture2D, 32, 24, StickerIcon.Idiom.Universal, StickerIcon.Scale.Triple, "ios");
				}
				return null;
			}
		}

		public Texture2D IPhoneSettings2
		{
			get
			{
				if (Override)
				{
					return iPhoneSettings2;
				}
				return GetDefaultTexture(58, 58);
			}
		}

		public StickerIcon IPhoneSettings2Icon
		{
			get
			{
				Texture2D texture2D = IPhoneSettings2;
				if (texture2D != null)
				{
					return new StickerIcon(texture2D, 29, 29, StickerIcon.Idiom.Iphone);
				}
				return null;
			}
		}

		public Texture2D IPhoneSettings3
		{
			get
			{
				if (Override)
				{
					return iPhoneSettings3;
				}
				return GetDefaultTexture(87, 87);
			}
		}

		public StickerIcon IPhoneSettings3Icon
		{
			get
			{
				Texture2D texture2D = IPhoneSettings3;
				if (texture2D != null)
				{
					return new StickerIcon(texture2D, 29, 29, StickerIcon.Idiom.Iphone, StickerIcon.Scale.Triple);
				}
				return null;
			}
		}

		public Texture2D IPadSettings2
		{
			get
			{
				if (Override)
				{
					return iPadSettings2;
				}
				return GetDefaultTexture(58, 58);
			}
		}

		public StickerIcon IPadSettings2Icon
		{
			get
			{
				Texture2D texture2D = IPadSettings2;
				if (texture2D != null)
				{
					return new StickerIcon(texture2D, 29, 29, StickerIcon.Idiom.Ipad);
				}
				return null;
			}
		}

		public Texture2D GetDefaultTexture(int width, int height)
		{
			if (appStore == null)
			{
				return null;
			}
			Texture2D texture2D = TextureScale.ScaledResized(appStore, width, height, Settings.BackgroundColor, (float)Settings.FillPercentage / 100f, Settings.FilterMode, Settings.ScaleMode);
			if (texture2D != null)
			{
				texture2D.name = width + "x" + height;
			}
			return texture2D;
		}
	}
}
