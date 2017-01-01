//-------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
//-------------------------------------------------------

Shader "Hidden/LeopotamGroup/FX/ScreenFade" {
	Properties {
	}

	SubShader {
		Tags { "RenderType" = "Overlay" "Queue" = "Overlay" }
		LOD 100

        ZTest Off
        Cull Off
        ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGINCLUDE
		#include "UnityCG.cginc"

		struct v2f {
			float4 pos : SV_POSITION;
			fixed4 color : TEXCOORD0;
		};
        
		v2f vert (appdata_full v) {
			v2f o;
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			o.color = v.color;
			return o;
		}

        fixed4 frag (v2f i) : SV_Target {
            return i.color;
        }
		ENDCG

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag         
			ENDCG 
		}
	}
	Fallback Off
}