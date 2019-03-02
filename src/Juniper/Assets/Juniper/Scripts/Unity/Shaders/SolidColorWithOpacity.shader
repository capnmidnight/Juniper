Shader "Unlit/SolidColorWithOpacity"
{
    Properties
    {
        _Color("Color", Color) = (0.0, 0.0, 0.0, 1.0)
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

            float4 vert(float4 vertex : POSITION) : SV_POSITION
            {
                return UnityObjectToClipPos(vertex);
            }

            fixed _Alpha;
            fixed4 _Color;

            fixed4 frag() : SV_Target
            {
                return _Alpha * _Color;
            }
            ENDCG
        }
    }
}
