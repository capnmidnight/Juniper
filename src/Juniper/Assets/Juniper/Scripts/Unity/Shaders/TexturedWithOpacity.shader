Shader "Unlit/TexturedWithOpacity"
{
    Properties
    {
        _MainTex("Base (A=Opacity)", 2D) = "white" {}
        _Alpha("Opacity", Range(0, 1)) = 1
    }
        SubShader
        {
            Tags
            {
                "RenderType" = "Transparent"
                "Queue" = "Transparent+500"
                "ForceNoShadowCasting" = "True"
                "IgnoreProjector" = "True"
                "PreviewType" = "Plane"
            }

            ZWrite Off

            Blend SrcAlpha OneMinusSrcAlpha

            LOD 100

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                   struct appdata
                   {
                       float4 position : POSITION;
                       float2 uv : TEXCOORD0;
                   };

                   struct v2f
                   {
                       float4 position : SV_POSITION;
                       float2 uv : TEXCOORD0;
                   };

                v2f vert(appdata vertex)
                {
                       v2f o;
                       o.position = UnityObjectToClipPos(vertex.position);
                       o.uv = vertex.uv;
                       return o;
                }

                fixed _Alpha;
                sampler2D _MainTex;

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col = tex2D(_MainTex, i.uv);
                    return _Alpha * col;
                }
                ENDCG
            }
        }
}