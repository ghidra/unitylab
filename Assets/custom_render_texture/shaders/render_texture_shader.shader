﻿Shader "jg/render_texture_shader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Tex("InputTex", 2D) = "white" {}
     }
     //https://docs.unity3d.com/Manual/class-CustomRenderTexture.html
     SubShader
     {
        Lighting Off
        Blend One Zero

        Pass
        {
            CGPROGRAM
            #include "UnityCustomRenderTexture.cginc"
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag
            #pragma target 3.0

            float4      _Color;
            sampler2D   _Tex;

            float4 frag(v2f_customrendertexture IN) : COLOR
            {
                
                return _Color * tex2D(_Tex, IN.localTexcoord.xy);
            }
            ENDCG
        }
    }
}