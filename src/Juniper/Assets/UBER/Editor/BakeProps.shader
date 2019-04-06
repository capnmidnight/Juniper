Shader "Hidden/UBER_BakeProps" {
	Properties {
		[NoKeywordToggle] _NormalTangentFlag("Normal/Tangent", Float) = 0
	}
	SubShader {

	Pass {
		ZWrite Off
		ZTest Off
		Cull Off
		
		CGPROGRAM
		// compile directives
		#pragma vertex vert_surf
		#pragma fragment frag_surf
		#include "HLSLSupport.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#include "AutoLight.cginc"
		
		float _NormalTangentFlag;
		
		struct v2f_surf {
			float4 pos : SV_POSITION;
			float4 bakedValue : TEXCOORD0;
//			float3 _vertex : TEXCOORD1;
//			float4 _tangent : TEXCOORD2;
		};

		// vertex shader
		v2f_surf vert_surf (appdata_full v) {
			v2f_surf o;
			UNITY_INITIALIZE_OUTPUT(v2f_surf,o); 
//			o._vertex=v.vertex.xyz;
//			o._tangent=v.tangent;
			// OpenGL right now needs to actually use incoming vertex position,
			// so use it in a very dummy way
			// (workaround taken from unity meta pass)
			v.vertex.x = v.vertex.x > 0 ? 1.0e-4f : 0.0f;
			v.vertex.y = v.vertex.y > 0 ? 1.0e-4f : 0.0f;
			v.vertex.z = v.vertex.z > 0 ? 1.0e-4f : 0.0f;
			o.pos=float4(v.texcoord.xy*2-1,1,1);
			#if !defined(SHADER_API_OPENGL) && !defined(SHADER_API_GLCORE) && !defined(SHADER_API_GLES3)
				o.pos.y = -o.pos.y;
			#endif
			o.bakedValue = _NormalTangentFlag==0 ? float4(v.normal.xyz*0.5+0.5,0) : float4(v.tangent*0.5+0.5);
			
			// decode scale (curvature - not used here) from vertex
			float2 Curv=frac(v.texcoord3);
			float2 Scl=(v.texcoord3-Curv)/100; // scale represented with 0.01 resolution (fair enough)
			//Curv=Curv*20-10; // Curv=(Curv-0.5)*10; // we assume curvature won't be higher than +/- 10
			Scl=min(12.0, Scl)/12.0; // lower resolution when we've got it in texture (0.1, range 0..12) -  set the same in UBER_StandardUtils2.cginc (GetTanBasisPerPixel() function)
			// put it in w channel (with tangent sign for tan texture)
			o.bakedValue.w = _NormalTangentFlag==0 ? Scl.x : Scl.y*v.tangent.w;
			
			return o;
		}

		// fragment shader
		half4 frag_surf (v2f_surf i) : SV_Target {
			half4 c=i.bakedValue;
//			float uratio=ddx(dot(i._vertex, i._tangent.xyz))*_ScreenParams.x*0.5;
//			return uratio;
//			c.w=(c.w-0.1)*4;//+0.5;
			c.w=c.w*0.5+0.5;
			return c;
		}

		ENDCG

	}
}
Fallback Off
}
