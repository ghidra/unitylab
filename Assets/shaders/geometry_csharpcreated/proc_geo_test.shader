Shader "jg/proc_geo_test"
{
	Properties
	{
		_ColorA ("ColorA", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" "LightMode"="ForwardBase"}
		LOD 100
		ZWrite Off
		Cull Off

		Pass
		{
			CGPROGRAM
			#pragma target 5.0
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc" // for _LightColor0
			#include "AutoLight.cginc"// shadow helper functions and macros
			#include "/Assets/shaders/NoiseLib.cginc"


			struct PS_INPUT
			{
				float4 position : SV_POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;//data
                SHADOW_COORDS(1)
				float4 color : COLOR;
				fixed4 diff : COLOR1;
				float4 view : COLOR2;
			};

			struct MeshVert
			{
				float4 P;///i might sneak uv in w of pos and norm
				float4 N;
				float4 distance;
			};

			StructuredBuffer<MeshVert> MeshBuffer  : register(t1) ;

			fixed4 _ColorA;

			PS_INPUT vert( uint vertex_id : SV_VertexID, uint instance_id : SV_InstanceID )
			{
				PS_INPUT o = (PS_INPUT)0;
				///Set data from buffers
				o.position = UnityObjectToClipPos( float4( MeshBuffer[vertex_id].P.xyz, 1) );
					//o.normal doesnt need to be set it looks like
				half3 worldNormal = UnityObjectToWorldNormal(MeshBuffer[vertex_id].N.xyz);

				float3 cnoise = (snoise3(MeshBuffer[vertex_id].P.xyz*0.23)+1.0)*0.5*0.04;
				float3 ncol = normalize(_ColorA*cnoise);


				o.color = float4(ncol,1.0);//+(MeshBuffer[vertex_id].P.xyz*0.05)
				o.color.a = 1.0f;

				return o;
			}

			fixed4 frag (PS_INPUT i) : SV_Target
			{
				i.color=float4(i.color.rgb,1.0f);
				return i.color;

			}
			ENDCG
		}

	}
}
