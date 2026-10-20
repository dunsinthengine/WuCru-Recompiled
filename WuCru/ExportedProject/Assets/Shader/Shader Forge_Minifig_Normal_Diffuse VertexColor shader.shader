Shader "Shader Forge/Minifig_Normal_Diffuse VertexColor shader" {
	Properties {
		_SpecPower ("SpecPower", Range(0, 10)) = 1.91105
		_Color ("Color", Color) = (1,1,1,1)
		_MapUV1 ("Map UV1", 2D) = "white" {}
		_NormalUV0 ("Normal UV0", 2D) = "bump" {}
		[MaterialToggle] _BlinkSwitch ("BlinkSwitch", Float) = 0
		_BlinkColor ("BlinkColor", Color) = (1,0,0,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" "Queue"="Geometry" }
		LOD 200

		Pass {
			Tags { "LightMode"="ForwardBase" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float2 uv0 : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				fixed4 color : COLOR;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv0 : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float3 worldNormal : TEXCOORD2;
				float3 worldTangent : TEXCOORD3;
				float3 worldBitangent : TEXCOORD4;
				fixed4 color : COLOR;
				float3 worldPos : TEXCOORD5;
			};

			float _SpecPower;
			fixed4 _Color;
			sampler2D _MapUV1;
			float4 _MapUV1_ST;
			sampler2D _NormalUV0;
			float4 _NormalUV0_ST;
			float _BlinkSwitch;
			fixed4 _BlinkColor;

			v2f vert(appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv0 = TRANSFORM_TEX(v.uv0, _NormalUV0);

				// Determine swatch UV:
				// Meshes with UV1 (FullMask, KaiHairLow1, Ninja_Helmet, etc.) point UV1 to the color swatch.
				// Single-channel meshes (Hair_Kai, Hair_Jay, etc.) have UV0 pinned directly to the swatch.
				// If UV1 is present on the mesh (non-zero), sample UV1; otherwise fall back to UV0.
				float2 swatchCoord = (any(v.uv1) && dot(v.uv1, v.uv1) > 0.0001) ? v.uv1 : v.uv0;
				o.uv1 = TRANSFORM_TEX(swatchCoord, _MapUV1);

				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
				o.worldBitangent = cross(o.worldNormal, o.worldTangent) * (v.tangent.w * unity_WorldTransformParams.w);
				o.color = v.color;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target {
				// Normal mapping
				half3 normalMap = UnpackNormal(tex2D(_NormalUV0, i.uv0));
				half3 worldNormal;
				if (dot(i.worldTangent, i.worldTangent) > 0.01) {
					worldNormal = normalize(normalMap.x * i.worldTangent + normalMap.y * i.worldBitangent + normalMap.z * i.worldNormal);
				} else {
					worldNormal = normalize(i.worldNormal);
				}

				// Diffuse swatch / color from _MapUV1
				fixed4 tex = tex2D(_MapUV1, i.uv1);
				fixed4 col;
				col.rgb = tex.rgb * _Color.rgb;
				col.a = 1.0;

				if (any(i.color.rgb)) {
					col.rgb *= i.color.rgb;
				}
				if (_BlinkSwitch > 0.5) {
					col.rgb = lerp(col.rgb, _BlinkColor.rgb, 0.7);
				}

				// Lighting: Half-Lambert + ambient + specular
				half3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
				half ndl = max(0.0, dot(worldNormal, lightDir)) * 0.5 + 0.5;
				half3 ambient = ShadeSH9(float4(worldNormal, 1.0));

				half3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				half3 h = normalize(lightDir + viewDir);
				half nh = max(0.0, dot(worldNormal, h));
				half spec = pow(nh, max(1.0, exp2(_SpecPower * 2.0))) * 0.35;

				col.rgb = col.rgb * (ndl * _LightColor0.rgb + ambient) + spec * _LightColor0.rgb;
				return col;
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}
