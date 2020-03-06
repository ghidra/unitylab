Shader "jg/tmp_voxel3dtexture_test"
{
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

            float4 frag(v2f_customrendertexture IN) : COLOR
            {
                return float4(0.0,IN.globalTexcoord.y,0.0,1.0);
                //return float4(IN.globalTexcoord,1.0);//_Color * tex3D(_Tex, IN.localTexcoord.xyz);
            }
            ENDCG
        }
    }
}
