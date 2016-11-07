//-------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2016 Leopotam <leopotam@gmail.com>
//-------------------------------------------------------

Shader "Hidden/LeopotamGroup/Gui/Font" {
    Properties {
        _MainTex ("Texture (RGB)", 2D) = "black" {}
        _ClipData ("clip-data", Vector) = (0,0,0,0)
    }

    SubShader {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "ForceNoShadowCasting" = "True" }

        LOD 100
        Fog { Mode Off }
        AlphaTest Off
        Lighting Off
        ZWrite Off
        Cull Off
        ColorMask RGB
        Blend SrcAlpha OneMinusSrcAlpha
        Offset -1, -1

        Pass {
            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile __ GUI_CLIP_RANGE
            //#pragma glsl_no_auto_normalization
            #pragma vertex vert
            #pragma fragment frag
     
            sampler2D _MainTex;

            #ifdef GUI_CLIP_RANGE
            float4 _ClipData;
            #endif
     
            struct v2f {
                float4 pos : SV_POSITION;
                fixed2 uv : TEXCOORD0;
                fixed4 color : TEXCOORD1;

                #ifdef GUI_CLIP_RANGE
                float2 clipPos : TEXCOORD2;
                #endif
            };

            v2f vert(appdata_full v) {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv = v.texcoord;
                o.color = v.color;

                #ifdef GUI_CLIP_RANGE
                o.clipPos = mul(unity_ObjectToWorld, v.vertex).xy;
                #endif

                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 c = i.color;
                c.a *= tex2D (_MainTex, i.uv).a;

                #ifdef GUI_CLIP_RANGE
                fixed2 t = step(_ClipData.xy, i.clipPos.xy) * step(i.clipPos.xy, _ClipData.zw);
                c.a *= t.x * t.y;
                #endif

                return c;
            }
            ENDCG
        }
    }
    FallBack Off
}