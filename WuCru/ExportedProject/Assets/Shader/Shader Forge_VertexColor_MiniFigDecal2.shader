Shader "Shader Forge/VertexColor_MiniFigDecal2" {
	Properties {
		_DecalAsset ("Decal Asset", 2D) = "white" {}
		[MaterialToggle] _BlinkSwitch ("BlinkSwitch", Float) = 0
		_BlinkColor ("BlinkColor", Color) = (1,0,0,1)
		_OutlineSwitch ("OutlineSwitch", Range(0, 0.5)) = 0
		_OutlineColor ("OutlineColor", Color) = (1,1,1,1)
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
				float2 uv0 : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				fixed4 color : COLOR;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv0 : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float3 worldNormal : TEXCOORD2;
				fixed4 color : COLOR;
				float3 worldPos : TEXCOORD3;
			};

			sampler2D _DecalAsset;
			float4 _DecalAsset_ST;
			float _BlinkSwitch;
			fixed4 _BlinkColor;
			float _OutlineSwitch;
			fixed4 _OutlineColor;

			v2f vert(appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv0 = TRANSFORM_TEX(v.uv0, _DecalAsset);
				o.uv1 = TRANSFORM_TEX(v.uv1, _DecalAsset);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.color = v.color;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target {
				// Base color swatch of the specific body part (hands, arms, head skin, legs, torso)
				fixed4 baseColor = tex2D(_DecalAsset, i.uv1);
				// Decal texture (face features, kimono/armor torso pattern, leg prints)
				fixed4 decal = tex2D(_DecalAsset, i.uv0);

				// Blend decal on top of the base body part color using decal alpha
				fixed4 col;
				col.rgb = lerp(baseColor.rgb, decal.rgb, decal.a);
				col.a = 1.0;

				if (any(i.color.rgb)) {
					col.rgb *= i.color.rgb;
				}
				if (_BlinkSwitch > 0.5) {
					col.rgb = lerp(col.rgb, _BlinkColor.rgb, 0.7);
				}

				// Lego plastic Half-Lambert diffuse and specular
				half3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
				half ndl = max(0.0, dot(i.worldNormal, lightDir)) * 0.5 + 0.5;
				half3 ambient = ShadeSH9(float4(i.worldNormal, 1.0));

				half3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				half3 h = normalize(lightDir + viewDir);
				half nh = max(0.0, dot(i.worldNormal, h));
				half spec = pow(nh, 16.0) * 0.25;

				col.rgb = col.rgb * (ndl * _LightColor0.rgb + ambient) + spec * _LightColor0.rgb;
				return col;
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}
