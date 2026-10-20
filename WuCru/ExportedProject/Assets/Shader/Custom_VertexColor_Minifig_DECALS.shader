Shader "Custom/VertexColor_Minifig_DECALS" {
	Properties {
		_DecalTexture ("Decal Texture", 2D) = "white" {}
		[HideInInspector] _Cutoff ("Alpha cutoff", Range(0, 1)) = 0.5
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

			sampler2D _DecalTexture;
			float4 _DecalTexture_ST;

			v2f vert(appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _DecalTexture);
				o.color = v.color;
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target {
				fixed4 decal = tex2D(_DecalTexture, i.uv);
				fixed4 col = lerp(i.color, decal, decal.a);
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
