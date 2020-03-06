Shader "jg/visualizeVoxel"
{
    SubShader
    {
        Tags { "IgnoreProjector" = "True" "Queue" = "Transparent"  "RenderType" = "Transparent" }
        LOD 100
        Blend One One
        ZWrite Off
        Pass
        {
            CGPROGRAM
            #pragma target 5.0
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "voxelStructs.cginc"
            
            struct PS_INPUT
            {
                float4 position : SV_POSITION;
                float4 color : COLOR;
            };

            StructuredBuffer<voxelPoint> renderPointBuffer;

            PS_INPUT vert( uint vertex_id : SV_VertexID, uint instance_id : SV_InstanceID )
            {
                PS_INPUT o = (PS_INPUT)0;
                o.position = UnityObjectToClipPos( float4( renderPointBuffer[vertex_id].P, 1) );
                o.color = float4(renderPointBuffer[vertex_id].Cd,1);
                return o;
            }
            fixed4 frag (PS_INPUT i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}