Shader "Custom/GUISheenPill"
{
	Properties
	{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		_FadeValue ("FadeValue", Float) = 1.0
	}

	SubShader
	{
		LOD 100

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			float _FadeValue;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.color = v.color * _Color;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				// Periodic diagonal sheen streak sweeping across the UI element
				float cycle = 2.8;
				float t = fmod(_Time.y, cycle);
				float sheen = 0.0;
				if (t < 0.8)
				{
					float progress = t / 0.8;
					float sweep = lerp(-1.0, 3.0, progress);
					float diag = i.texcoord.x + i.texcoord.y;
					float dist = abs(diag - sweep);
					sheen = saturate(1.0 - dist * 3.5);
					sheen = sheen * sheen;
				}

				fixed4 tex = tex2D(_MainTex, i.texcoord);
				fixed4 col = tex * i.color;
				col.a *= sheen * _FadeValue;
				return col;
			}
			ENDCG
		}
	}
	Fallback Off
}
