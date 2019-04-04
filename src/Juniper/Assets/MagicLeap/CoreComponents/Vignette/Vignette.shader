// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

Shader "Magic Leap/Vignette"
{
    Properties
    {
        _MainTex ( "Screen", 2D ) = "black" {}
        _VignettePower("_VignettePower", Float) = 13
        _VignetteMode("_VignetteMode", Int) = 0
        [HideInInspector] _texcoord( "", 2D ) = "white" {}
    }

    SubShader
    {
        ZTest Always
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img_custom
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"

            struct appdata_img_custom
            {
                float4 vertex : POSITION;
                half2 texcoord : TEXCOORD0;
            };

            struct v2f_img_custom
            {
                float4 pos : SV_POSITION;
                half2 uv : TEXCOORD0;
                half2 stereoUV : TEXCOORD2;
        #if UNITY_UV_STARTS_AT_TOP
                half4 uv2 : TEXCOORD1;
                half4 stereoUV2 : TEXCOORD3;
        #endif
            };

            uniform sampler2D _MainTex;
            uniform half4 _MainTex_TexelSize;
            uniform half4 _MainTex_ST;
            uniform int _VignetteMode;
            uniform float _VignettePower;

            v2f_img_custom vert_img_custom (appdata_img_custom v)
            {
                v2f_img_custom o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = float4(v.texcoord.xy, 1, 1);

                #if UNITY_UV_STARTS_AT_TOP
                    o.uv2 = float4(v.texcoord.xy, 1, 1);
                    o.stereoUV2 = UnityStereoScreenSpaceUVAdjust(o.uv2, _MainTex_ST);

                    if (_MainTex_TexelSize.y < 0.0)
                    {
                        o.uv.y = 1.0 - o.uv.y;
                    }
                #endif
                o.stereoUV = UnityStereoScreenSpaceUVAdjust(o.uv, _MainTex_ST);
                return o;
            }

            half4 frag (v2f_img_custom i) : SV_Target
            {
                #ifdef UNITY_UV_STARTS_AT_TOP
                    half2 uv = i.uv2;
                    half2 stereoUV = i.stereoUV2;
                #else
                    half2 uv = i.uv;
                    half2 stereoUV = i.stereoUV;
                #endif

                float4 color = float4(1, 1, 1, 0);
                float2 uvTexture = i.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                float4 mainTexure = tex2D(_MainTex, uvTexture);

                float uvX = (uvTexture).x;
                float uvY = (uvTexture).y;
                float saturation =
                    saturate((((1.0 - saturate(pow(uvX, _VignettePower))) * (1.0 - saturate(pow(( 1.0 - uvX), _VignettePower)))) * ((1.0 - saturate(pow(uvY, _VignettePower))) * (1.0 - saturate(pow((1.0 - uvY), _VignettePower))))));

                float4 vignette = 0;
                if (_VignetteMode <= 1)
                {
                    vignette = (color * (mainTexure * saturation));
                }
                else
                {
                    vignette = (color * (mainTexure + (1.0 - saturation)));
                }

                return vignette;
            }
            ENDCG
        }
    }
}
