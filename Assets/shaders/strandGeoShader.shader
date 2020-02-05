Shader "jg/strandGeoShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NormTex ("Texture", 2D) = "gray" {}
        _Width ("Line Width", Range(0.01,2))=0.1
        _SpecColor ("Specular Color", Color)=(1,1,1,1)
        _Metallic ("Metallic", Range(0, 1)) = 0
        _Roughness ("Roughness", Range(0, 1)) = 0.5
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    struct renderPoint{
        float3 P;
        float3 tangent;
        float u;
        float3 Cd;
    };


    float _Width;
    //StructuredBuffer<float4> renderPointBuffer;
    StructuredBuffer<renderPoint> renderPointBuffer;

    struct appdata
    {
        float4 vertex : POSITION;
        //float2 uv : TEXCOORD0;
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
        float3 norm : NORMAL;
        float2 uv : TEXCOORD0;
        float3 tan : TANGENT;
        float3 wpos : POSITIONT;
        float4 tspace0 : TEXCOORD1;
        float4 tspace1 : TEXCOORD2;
        float4 tspace2 : TEXCOORD3;
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
        //o.uv = v.uv;
        return o;
    }

    //////////////
    geometryOutput VertexOutput(float3 position, float3 normal, float3 tangent, float2 uv)
    {
        geometryOutput o;

        o.pos = UnityObjectToClipPos(position);
        o.uv = uv;
        o.norm = normal;
        o.tan = tangent;
        o.wpos = position;

        ///get the tangent for normals
        half3 bi = cross(normal, tangent) * unity_WorldTransformParams.w;
        o.tspace0 = float4(tangent.x, bi.x, normal.x, position.x);
        o.tspace1 = float4(tangent.y, bi.y, normal.y, position.y);
        o.tspace2 = float4(tangent.z, bi.z, normal.z, position.z);

        return o;
    }

    [maxvertexcount(6)]
    //void geo(triangle float4 IN[3] : SV_POSITION, inout TriangleStream<geometryOutput> triStream)
    void geo(triangle v2g IN[3] : SV_POSITION, inout TriangleStream<geometryOutput> triStream)
    {
        //geometryOutput o;
        //float3 pos = IN[1].vertex.xyz;//grab point 1 in the triangle
        ///////////////////////////////
        // get the camera basis vectors
        //https://gist.github.com/renaudbedard/7a90ec4a5a7359712202
        float3 forward = normalize(UNITY_MATRIX_V._m20_m21_m22);
        float3 up = float3(0, 1, 0); //normalize(UNITY_MATRIX_V._m10_m11_m12);
        float3 right = normalize(UNITY_MATRIX_V._m00_m01_m02);
        // rotate to face camera
        //float4x4 rotationMatrix = float4x4(right,0,up,0,forward,0,0,0,0,1);
        ///////////////////////////////

        ///set the vertex ID
        uint vertex_id = IN[1].vertex_id;
        uint next_vertex_id = vertex_id+1;//
        /*
        uint prev_vertex_id = max(vertex_id-1,0);
        uint next_next_vertex_id = vertex_id+2;

        //float3 p0 = renderPointBuffer[prev_vertex_id].xyz;
        //float3 p1 = renderPointBuffer[vertex_id].xyz;
        //float3 p2 = renderPointBuffer[next_vertex_id].xyz;
        //float3 p3 = renderPointBuffer[next_next_vertex_id].xyz;

        float3 p0 = renderPointBuffer[prev_vertex_id].P.xyz;
        float3 p1 = renderPointBuffer[vertex_id].P.xyz;
        float3 p2 = renderPointBuffer[next_vertex_id].P.xyz;
        float3 p3 = renderPointBuffer[next_next_vertex_id].P.xyz;


        ///get tangents
        float3 tan1 = normalize(p2-p1);
        float3 tan2 = (prev_vertex_id==0)?tan1:normalize(p1-p0);
        float3 tan3 = normalize(p3-p2);///for the second point

        float3 tangent1 = normalize((tan1+tan2)*0.5);
        float3 tangent2 = normalize((tan2+tan3)*0.5);

        ///get the vector from camera to this point
        //float3 camLook = normalize(_WorldSpaceCameraPos-p1);

        float3 direction1 = tangent1;
        float3 strandup1 = normalize(cross(direction1,forward));
        float3 width1 = strandup1*_Width*0.5;

        float3 direction2 = tangent2;
        float3 strandup2 = normalize(cross(direction2,forward));
        float3 width2 = strandup2*_Width*0.5;
        */

        ///////////////////////////////////////



        float3 p1 = renderPointBuffer[vertex_id].P.xyz;
        float3 p2 = renderPointBuffer[next_vertex_id].P.xyz;
        float3 t1 = renderPointBuffer[vertex_id].tangent.xyz;
        float3 t2 = renderPointBuffer[next_vertex_id].tangent.xyz;

        float3 camLook1 = normalize(_WorldSpaceCameraPos-mul(unity_ObjectToWorld,p1));//this aint right
        float3 camLook2 = normalize(_WorldSpaceCameraPos-mul(unity_ObjectToWorld,p2));
        //float3 camLook = normalize(UNITY_MATRIX_V._m03_m13_m23-p1);

        float3 strandup1 = normalize(cross(t1,-camLook1));
        float3 width1 = strandup1*_Width*0.5;

        float3 strandup2 = normalize(cross(t2,-camLook1));
        float3 width2 = strandup2*_Width*0.5;

        ///////////////////////////////////////


        triStream.Append(VertexOutput(p1+width1,camLook1,t1,float2(0,0)));
        triStream.Append(VertexOutput(p1-width1,camLook1,t1,float2(0,1)));
        triStream.Append(VertexOutput(p2-width2,camLook2,t2,float2(1,1)));

        triStream.RestartStrip();

        triStream.Append(VertexOutput(p1+width1,camLook1,t1,float2(0,0)));
        triStream.Append(VertexOutput(p2-width2,camLook2,t2,float2(1,1)));
        triStream.Append(VertexOutput(p2+width2,camLook2,t2,float2(1,0)));

    }
    //////////////

    ENDCG

    SubShader
    {
        //Tags { "RenderType"="Opaque" }
        Tags{ "LightMode" = "Deferred" }
        Blend Off

        LOD 100


        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geo
            #pragma fragment frag
            //#pragma surface frag Standard fullforwardshadows
            #pragma target 4.6
            #include "UnityGBuffer.cginc"
            #include "UnityStandardUtils.cginc"
            #pragma multi_compile_prepassfinal noshadowmask nodynlightmap nodirlightmap nolightmap


            sampler2D _MainTex;
            sampler2D _NormTex;
            //float4 _MainTex_ST;
            float4 _SpecColor;
            float _Metallic;
            float _Roughness;

            float3 perpixel_normalmap_2(float3 normalmap, float3 position, float3 normal, float3 tangent)
            {
                half3 bi = cross(normal, tangent) * unity_WorldTransformParams.w;
                float4 tspace0 = float4(tangent.x, bi.x, normal.x, position.x);
                float4 tspace1 = float4(tangent.y, bi.y, normal.y, position.y);
                float4 tspace2 = float4(tangent.z, bi.z, normal.z, position.z);

                float3 wn = normalize(float3(
                    dot(tspace0.xyz, normalmap),
                    dot(tspace1.xyz, normalmap),
                    dot(tspace2.xyz, normalmap)
                    ));

                return wn;
            }

            //https://github.com/tchpowdog/SurrealGrassShader/blob/master/SurrealGrassShader/Shaders/SurrealGrassShader.shader
            //float4 frag (geometryOutput i) : SV_Target
            void frag(geometryOutput i,out half4 outGBuffer0 : SV_Target0,out half4 outGBuffer1 : SV_Target1,out half4 outGBuffer2 : SV_Target2,out half4 outEmission : SV_Target3)
            {
                half4 normal = tex2D(_NormTex, i.uv.yx);
                normal.xyz = UnpackScaleNormal(normal, 1.0);

                //half4 normal2 = tex2D(_NormTex, i.uv.xy*float2(i.uv.y*_SecSkew*_SecScale,1));
                half4 normal2 = tex2D(_NormTex, (i.uv.xy+float2(i.uv.y*2,0.0))*float2(2,1));
                normal2.xyz = UnpackScaleNormal(normal2, 1.0);

                ///now get the normal value with the tube normals...
                float3 wn = normalize(float3(
                    dot(i.tspace0.xyz, normal),
                    dot(i.tspace1.xyz, normal),
                    dot(i.tspace2.xyz, normal)
                    ));

                //add in a second layer of normals, to make it tool wrapped
                float3 nwn = perpixel_normalmap_2(normal2.xyz,i.wpos.xyz,wn,i.tan.xyz);

                /////////////////////////
                //return float4(nwn.r, nwn.g, nwn.b, 1);

                /////////////////////////
                fixed4 c = fixed4(0.5,0.5,0.5,1.0);

                half3 c_diff, c_spec;
                half refl10;
                c_diff = DiffuseAndSpecularFromMetallic(
                    c, _Metallic,
                    c_spec, refl10
                );
                float roughness = 1-_Roughness;//tex2D(_Roughness1, IN.texcoord).a;

                UnityStandardData data;
                data.diffuseColor = half3(0.5,0.5,0.5);
                data.occlusion = 0.0; // data.occlusion = occ;
                data.specularColor = c_spec * _SpecColor;//data.specularColor = c_spec;
                data.smoothness = roughness;//data.smoothness = _Glossiness;
                data.normalWorld = nwn;// float3(0,1,0);
                UnityStandardDataToGbuffer(data, outGBuffer0, outGBuffer1, outGBuffer2);

                // Calculate ambient lighting and output to the emission buffer.
                float3 wp = float3(i.tspace0.w, i.tspace1.w, i.tspace2.w);
                half3 sh = ShadeSHPerPixel(data.normalWorld, half3(0,0,0), wp);//the half there is an ambient light
                outEmission = half4(sh * c_diff, 1);// * occ;
                

            }
            ENDCG
        }

        Pass
        {
            Tags
            {
                "LightMode" = "ShadowCaster"
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geo
            #pragma fragment frag
            //#pragma hull hull
            //#pragma domain domain
            #pragma target 4.6
            #pragma multi_compile_shadowcaster

            float4 frag(geometryOutput i) : SV_Target
            {
                SHADOW_CASTER_FRAGMENT(i)
            }

            ENDCG
        }
    }
}
