Shader "Unlit/NewUnlitShader"
{
	SubShader{
		Pass{
				// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members pos,col)
				CGPROGRAM

		#pragma exclude_renderers d3d11_9x d3d11 xbox360

		#pragma target 5.0  
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"

			struct Point {
				float4 pos;
				float4 vel;
			};
			struct v2f {
				float4 pos : SV_POSITION;
				float3 col : COLOR0;
			};
			float x, y, z;
			StructuredBuffer<Point> particles;

			v2f vert(uint id : SV_VertexID) {
				v2f o;
				float4 worldPos = particles[id].pos;
				o.pos = mul(UNITY_MATRIX_MVP, worldPos);
				//o.pos = float4(worldPos, 1.0f);
				//o.pos = worldPos;
				o.col.x = (id / (x*x));
				o.col.y = 1.0f - (id /(y*y));
				o.col.z = 0.5f + ((id / 1024.0f) / 10.0f);
				return o;
			}
			float4 frag(v2f i) : COLOR{
				return float4(i.col,1.0f);
			}
			ENDCG
		}
	}
		Fallback Off
}
