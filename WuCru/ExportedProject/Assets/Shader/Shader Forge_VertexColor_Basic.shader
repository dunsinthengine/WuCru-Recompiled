Shader "Shader Forge/VertexColor_Basic" {
	Properties {
		_MainColor ("MainColor", Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "RenderType" = "Opaque" "Queue" = "Geometry" }
		LOD 100

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				fixed4 color : COLOR;
			};

			fixed4 _MainColor;

			v2f vert(appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.color = v.color * _MainColor;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target {
				return i.color;
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}
