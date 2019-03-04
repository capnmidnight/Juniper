Shader "Lit/SimpleLambert"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Main Color", Color) = (1.0, 1.0, 1.0, 1.0)
    }
        SubShader
        {
            Tags
            {
                "RenderType" = "Opaque"
            }

            CGPROGRAM
            #pragma surface surf Lambert
            struct Input
            {
                float2 uv_MainTex;
            };

            sampler2D _MainTex;
            float4 _Color;

            void surf(Input IN, inout SurfaceOutput o)
            {
                float4 tex = tex2D(_MainTex, IN.uv_MainTex);
                o.Albedo = (tex * _Color).rgb;
            }
            ENDCG
        }

            Fallback "Diffuse"
}