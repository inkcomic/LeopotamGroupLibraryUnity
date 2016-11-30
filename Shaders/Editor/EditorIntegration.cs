//-------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2016 Leopotam <leopotam@gmail.com>
//-------------------------------------------------------

using System.IO;
using UnityEditor;

namespace LeopotamGroup.Shaders.UnityEditors {
    static class EditorIntegration {
        const string ShaderTemplate =
            "Shader \"Custom/Unlit<<TYPE>>\"{\tProperties{\t\t_MainTex(\"Texture\", 2D)=\"white\" <<>>\n\t}\n\n\tSubShader{" +
            "\t\tTags << \"RenderType\"=\"<<TYPE>>\" \"Queue\"=\"<<QUEUE>>\" \"IgnoreProjector\"=\"True\" \"ForceNoShadowCasting\"=\"True\" >>\n" +
            "\t\tLOD 100\n\n<<SHADERFLAGS>>\t\tCGINCLUDE\n\t\t#include \"UnityCG.cginc\"\n\n" +
            "\t\tsampler2D _MainTex;\t\tfloat4 _MainTex_ST;\n\t\tstruct v2f{\t\t\tfloat4 pos:SV_POSITION;" +
            "\t\t\tfloat2 uv:TEXCOORD0;\t\t};\n\t\tv2f vert(appdata_full v){\t\t\tv2f o;\t\t\to.pos=mul(UNITY_MATRIX_MVP,v.vertex);" +
            "\t\t\to.uv=TRANSFORM_TEX(v.texcoord,_MainTex);\t\t\treturn o;\t\t}\n\n\t\tfixed4 frag(v2f i):SV_Target{" +
            "\t\t\treturn tex2D(_MainTex,i.uv);\t\t}\n\t\tENDCG\n\n\t\tPass{\t\t\tTags << \"LightMode\"=\"ForwardBase\" >>\n" +
            "\t\t\tCGPROGRAM\n\t\t\t#pragma vertex vert\n\t\t\t#pragma fragment frag\n\t\t\tENDCG\n\t\t}\n\t}\n\tFallback Off\n}";

        const string ShaderAlphaBlendTags = "\t\tCull Off\n\t\tZWrite Off\n\t\tBlend SrcAlpha OneMinusSrcAlpha\n\n";

        static string GetAssetPath () {
            var path = AssetDatabase.GetAssetPath (Selection.activeObject);
            if (!string.IsNullOrEmpty (path) && AssetDatabase.Contains (Selection.activeObject)) {
                if (!AssetDatabase.IsValidFolder (path)) {
                    path = Path.GetDirectoryName (path);
                }
            } else {
                path = "Assets";
            }
            return path;
        }

        static string CreateShaderCode (string template, string renderType, string renderQueue, bool isAlphaBlend) {
            template = template.Replace ("<<TYPE>>", renderType);
            template = template.Replace ("<<QUEUE>>", renderQueue);
            template = template.Replace ("<<SHADERFLAGS>>", isAlphaBlend ? ShaderAlphaBlendTags : "");
            template = template.Replace ("\t", new string (' ', 4));
            template = template.Replace ("{", " {\n");
            template = template.Replace ("<<", "{");
            template = template.Replace (">>", "}");
            template = template.Replace ("(", " (");
            template = template.Replace ("=", " = ");
            template = template.Replace (":", " : ");
            template = template.Replace (",", ", ");
            template = template.Replace (";", ";\n");
            return template;
        }

        [MenuItem ("Assets/LeopotamGroup/Shaders/Create unlit opaque shader", false, 1)]
        static void CreateUnlitOpaqueShader () {
            File.WriteAllText (
                AssetDatabase.GenerateUniqueAssetPath (string.Format ("{0}/{1}.shader", GetAssetPath (), "UnlitOpaqueShader")),
                CreateShaderCode (ShaderTemplate, "Opaque", "Geometry", false));
            AssetDatabase.Refresh ();
        }

        [MenuItem ("Assets/LeopotamGroup/Shaders/Create unlit transparent shader", false, 1)]
        static void CreateUnlitTransparentShader () {
            File.WriteAllText (
                AssetDatabase.GenerateUniqueAssetPath (string.Format ("{0}/{1}.shader", GetAssetPath (), "UnlitTransparentShader")),
                CreateShaderCode (ShaderTemplate, "Transparent", "Transparent", true));
            AssetDatabase.Refresh ();
        }
    }
}