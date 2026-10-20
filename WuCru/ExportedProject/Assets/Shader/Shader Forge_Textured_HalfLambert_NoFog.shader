Shader "Shader Forge/Textured_HalfLambert_NoFog" {
	Properties {
		_MainTex ("MainTex", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "RenderType" = "Opaque" "Queue" = "Geometry" }
		LOD 200

		Pass {
			Tags { "LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
				float3 worldNormal : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;

			v2f vert(appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.color = v.color * _Color;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target {
				fixed4 tex = tex2D(_MainTex, i.uv);
				fixed4 col = tex * i.color;
				half3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
				half ndl = max(0.0, dot(i.worldNormal, lightDir)) * 0.5 + 0.5;
				half3 ambient = ShadeSH9(float4(i.worldNormal, 1.0));
				col.rgb *= (ndl * _LightColor0.rgb + ambient);
				return col;
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}
