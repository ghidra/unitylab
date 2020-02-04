Shader "jg/strandGeoShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Width ("Line Width", Range(0.01,2))=0.1
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    float _Width;
    StructuredBuffer<float4> renderPointBuffer;

    struct appdata
    {
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
        uint vertex_id : SV_VertexID;
    };
    struct v2g
    {
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
        uint vertex_id : VertexID;
    };
    struct geometryOutput
    {
        float4 pos : SV_POSITION;
        float2 uv : TEXCOORD0;
    };


    // Simple noise function, sourced from http://answers.unity.com/answers/624136/view.html
    // Extended discussion on this function can be found at the following link:
    // https://forum.unity.com/threads/am-i-over-complicating-this-random-function.454887/#post-2949326
    // Returns a number in the 0...1 range.
    float rand(float3 co)
    {
        return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 53.539))) * 43758.5453);
    }

    // Construct a rotation matrix that rotates around the provided axis, sourced from:
    // https://gist.github.com/keijiro/ee439d5e7388f3aafc5296005c8c3f33
    float3x3 AngleAxis3x3(float angle, float3 axis)
    {
        float c, s;
        sincos(angle, s, c);

        float t = 1 - c;
        float x = axis.x;
        float y = axis.y;
        float z = axis.z;

        return float3x3(
            t * x * x + c, t * x * y - s * z, t * x * z + s * y,
            t * x * y + s * z, t * y * y + c, t * y * z - s * x,
            t * x * z - s * y, t * y * z + s * x, t * z * z + c
            );
    }

    v2g vert(appdata v)
    {
        v2g o;
        o.vertex = v.vertex;
        o.vertex_id = v.vertex_id;
        o.uv = v.uv;
        return o;
    }

    //////////////

    [maxvertexcount(6)]
    //void geo(triangle float4 IN[3] : SV_POSITION, inout TriangleStream<geometryOutput> triStream)
    void geo(triangle v2g IN[3] : SV_POSITION, inout TriangleStream<geometryOutput> triStream)
    {
        //geometryOutput o;
        //float3 pos = IN[1].vertex.xyz;//grab point 1 in the triangle
        ///////////////////////////////
        // get the camera basis vectors
        //https://gist.github.com/renaudbedard/7a90ec4a5a7359712202
        float3 forward = -normalize(UNITY_MATRIX_V._m20_m21_m22);
        float3 up = float3(0, 1, 0); //normalize(UNITY_MATRIX_V._m10_m11_m12);
        float3 right = normalize(UNITY_MATRIX_V._m00_m01_m02);
        // rotate to face camera
        float4x4 rotationMatrix = float4x4(right,0,up,0,forward,0,0,0,0,1);
        ///////////////////////////////

        ///set the vertex ID
        uint vertex_id = IN[1].vertex_id;
        float3 p1 = renderPointBuffer[vertex_id].xyz;

        ///get the next point id in the strand
        uint next_vertex_id = vertex_id+1;
        float3 p2 = renderPointBuffer[next_vertex_id].xyz;

        float3 ray = p2-p1;
        float3 direction = normalize(ray);
        float3 strandup = cross(direction,forward);
        float3 width = strandup*_Width*0.5;

        ///make first point of triangle
        geometryOutput v1,v2,v3,v4,v5,v6;

        v1.pos = UnityObjectToClipPos(p1+width);
        v2.pos = UnityObjectToClipPos(p1-width);
        v3.pos = UnityObjectToClipPos(p2-width);

        v4.pos = UnityObjectToClipPos(p1+width);
        v5.pos = UnityObjectToClipPos(p2-width);
        v6.pos = UnityObjectToClipPos(p2+width);

        v1.uv = float2(0,0);
        v2.uv = float2(0,1);
        v3.uv = float2(1,1);

        v6.uv = float2(1,1);
        v4.uv = float2(0,0);
        v5.uv = float2(1,0);

        triStream.Append(v1);
        triStream.Append(v2);
        triStream.Append(v3);

        triStream.Append(v6);
        triStream.Append(v5);
        triStream.Append(v4);

        
/*
        /////////////
        ///get data from the buffer
        float3 buffer_position = p1;
        /////////////

        o.uv = IN[1].uv;

        o.pos = UnityObjectToClipPos(buffer_position +mul( float3(0.5, 0, 0),rotationMatrix));
        triStream.Append(o);

        o.pos = UnityObjectToClipPos(buffer_position + mul( float3(-0.5, 0, 0),rotationMatrix));
        triStream.Append(o);

        o.pos = UnityObjectToClipPos(buffer_position + mul( float3(0, 1, 0),rotationMatrix));
        triStream.Append(o);
*/
    }
    //////////////

    ENDCG

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry geo
            #pragma target 4.6

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 frag (geometryOutput i) : SV_Target
            {   

                return float4(i.uv.x, i.uv.x, i.uv.x, 1);
            }
            ENDCG
        }
    }
}
