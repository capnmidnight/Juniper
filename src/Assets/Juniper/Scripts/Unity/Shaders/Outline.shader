Shader "UI/Unlit/Outline"
{
    Properties
    {
        _OutlineColor("Outline color", Color) = (0,0,0,1)
        _Outline("Outline width", Range(0.0, 0.25)) = 0.05
        _Center("Mesh Center", Vector) = (0,0,0,1)
    }

        SubShader
    {
        Tags{ "Queue" = "Transparent" }

        Pass
        {
            Name "OUTLINE"
            Tags{ "LightMode" = "Always" }
            Cull Front
            ZWrite Off
            ZTest Less
            ColorMask RGB // alpha not used

            // you can choose what kind of blending mode you want for the outline
            //Blend SrcAlpha OneMinusSrcAlpha // Normal
            Blend One One // Additive
            //Blend One OneMinusDstColor // Soft Additive
            //Blend DstColor Zero // Multiplicative
            //Blend DstColor SrcColor // 2x Multiplicative

            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                uniform float4 _OutlineColor;
                uniform float _Outline;
                uniform float4 _Center;

                struct v2f
                {
                    float4 pos : POSITION;
                };

                v2f vert(appdata_base v)
                {
                    float4 pos = v.vertex;
                    float4 dir = v.vertex - _Center;
                    v2f o;
                    o.pos = UnityObjectToClipPos(pos + _Outline * dir);
                    return o;
                }

                float4 frag(v2f i) : COLOR
                {
                    return _OutlineColor;
                }
            ENDCG
        }
    }

        Fallback "Diffuse"
}
