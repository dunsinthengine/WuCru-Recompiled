Shader "Unlit/WaterShader" {
	Properties {
		_MainColor ("MainColor", Color) = (1,1,1,1)
		_ColorSpec ("ColorSpec", Color) = (0.9, 0.8, 0.8, 1)
		_MainTex ("MainTex", 2D) = "white" {}
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
				float3 worldNormal : TEXCOORD1;
				fixed4 color : COLOR;
				float3 worldPos : TEXCOORD2;
			};

			fixed4 _MainColor;
			fixed4 _ColorSpec;
			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert(appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.color = v.color;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target {
				fixed4 tex = tex2D(_MainTex, i.uv);
				fixed4 col = _MainColor * tex;
				if (any(i.color.rgb)) {
					col *= i.color;
				}
				half3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
				half ndl = max(0.0, dot(i.worldNormal, lightDir)) * 0.5 + 0.5;
				half3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				half3 h = normalize(lightDir + viewDir);
				half spec = pow(max(0.0, dot(i.worldNormal, h)), 16.0) * 0.35;
				half3 ambient = ShadeSH9(float4(i.worldNormal, 1.0));
				col.rgb = col.rgb * (ndl * _LightColor0.rgb + ambient) + spec * _ColorSpec.rgb * _LightColor0.rgb;
				return col;
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}
