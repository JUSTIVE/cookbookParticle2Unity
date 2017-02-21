Shader "Unlit/NewUnlitShader 1"
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
			float3 pos;
			float3 vel;
		};
		struct v2f {
			float4 pos : SV_POSITION;
		};

		StructuredBuffer<Point> particle;

		v2f vert(uint id : SV_VertexID) {
			v2f o;
			float4 worldPos = float4(particle[id].pos,1.0);
			o.pos = mul(UNITY_MATRIX_VP, worldPos);
			//o.pos = float4(worldPos, 1.0f);
			return o;
		}
		float4 frag(v2f i) : COLOR{
			return float4(0.0f,0.0f,0.0,1.0f);
		}
			ENDCG
		}
	}
	Fallback Off
}
