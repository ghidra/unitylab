Shader "jg/tmp_draw3dTextureToPlane"
{
    Properties
    {
        _d ("Texture Depth", range(0,1)) = 0.5
        _Tex("Input Texture", 3D) = "white" {}
     }

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

            float    _d;
            sampler3D   _Tex;

            float4 frag(v2f_customrendertexture IN) : COLOR
            {
                return tex3D(_Tex, float3(IN.localTexcoord.xy,_d));
            }
            ENDCG
        }
    }
}
