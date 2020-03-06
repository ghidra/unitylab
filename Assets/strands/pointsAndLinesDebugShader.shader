Shader "jg/pointAndLinesDebugShader"
{
    Properties
    {
        _ColorA ("ColorA", Color) = (1,1,1,1)
    }
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
            
            struct PS_INPUT
            {
                float4 position : SV_POSITION;
                float4 color : COLOR;
            };

            StructuredBuffer<float4> renderPointBuffer;

            fixed4 _ColorA;

            PS_INPUT vert( uint vertex_id : SV_VertexID, uint instance_id : SV_InstanceID )
            {
                PS_INPUT o = (PS_INPUT)0;
                o.position = UnityObjectToClipPos( float4( renderPointBuffer[vertex_id].xyz, 1) );
                o.color = _ColorA;
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