//****************** 代码文件申明 ***********************
//* 文件：TextureUtility
//* 作者：wheat
//* 创建时间：2024/05/28 08:55:04 星期二
//* 描述：
//*******************************************************

using UnityEngine;
using UnityEditor;
using System;
using Object = UnityEngine.Object;

namespace KFrame.Editor
{
    public static class TextureUtility
    {
        private static Material extractSpriteMaterial;

        private static readonly string extractSpriteShader = "\r\n            Shader \"Hidden/Sirenix/Editor/GUIIcon\"\r\n            {\r\n\t            Properties\r\n\t            {\r\n                    _MainTex(\"Texture\", 2D) = \"white\" {}\r\n                    _Color(\"Color\", Color) = (1,1,1,1)\r\n                    _Rect(\"Rect\", Vector) = (0,0,0,0)\r\n                    _TexelSize(\"TexelSize\", Vector) = (0,0,0,0)\r\n\t            }\r\n                SubShader\r\n\t            {\r\n                    Blend SrcAlpha OneMinusSrcAlpha\r\n                    Pass\r\n                    {\r\n                        CGPROGRAM\r\n                            #pragma vertex vert\r\n                            #pragma fragment frag\r\n                            #include \"UnityCG.cginc\"\r\n\r\n                            struct appdata\r\n                            {\r\n                                float4 vertex : POSITION;\r\n\t\t\t\t\t            float2 uv : TEXCOORD0;\r\n\t\t\t\t            };\r\n\r\n                            struct v2f\r\n                            {\r\n                                float2 uv : TEXCOORD0;\r\n\t\t\t\t\t            float4 vertex : SV_POSITION;\r\n\t\t\t\t            };\r\n\r\n                            sampler2D _MainTex;\r\n                            float4 _Rect;\r\n\r\n                            v2f vert(appdata v)\r\n                            {\r\n                                v2f o;\r\n                                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);\r\n                                o.uv = v.uv;\r\n                                return o;\r\n                            }\r\n\r\n                            fixed4 frag(v2f i) : SV_Target\r\n\t\t\t\t            {\r\n                                float2 uv = i.uv;\r\n                                uv *= _Rect.zw;\r\n\t\t\t\t\t            uv += _Rect.xy;\r\n\t\t\t\t\t            return tex2D(_MainTex, uv);\r\n\t\t\t\t            }\r\n\t\t\t            ENDCG\r\n\t\t            }\r\n\t            }\r\n            }";

        //
        // 摘要:
        //     Creates a new texture with no mimapping, linier colors, and calls texture.LoadImage(bytes),
        //     DontDestroyOnLoad(tex) and sets hideFlags = DontUnloadUnusedAsset | DontSaveInEditor.
        //     Old description no longer relevant as we've moved past version 2017. Loads an
        //     image from bytes with the specified width and height. Use this instead of someTexture.LoadImage()
        //     if you're compiling to an assembly. Unity has moved the method in 2017, and Unity's
        //     assembly updater is not able to fix it for you. This searches for a proper LoadImage
        //     method in multiple locations, and also handles type name conflicts.
        public static Texture2D LoadImage(int width, int height, byte[] bytes)
        {
            Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, mipChain: false, linear: true);
            texture2D.LoadImage(bytes);
            Object.DontDestroyOnLoad(texture2D);
            texture2D.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontUnloadUnusedAsset;
            return texture2D;
        }

        //
        // 摘要:
        //     Crops a Texture2D into a new Texture2D.
        public static Texture2D CropTexture(this Texture texture, Rect source)
        {
            RenderTexture active = RenderTexture.active;
            RenderTexture renderTexture = (RenderTexture.active = RenderTexture.GetTemporary(texture.width, texture.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 8));
            bool sRGBWrite = GL.sRGBWrite;
            GL.sRGBWrite = false;
            GL.Clear(clearDepth: false, clearColor: true, new Color(1f, 1f, 1f, 0f));
            Graphics.Blit(texture, renderTexture);
            Texture2D texture2D = new Texture2D((int)source.width, (int)source.height, TextureFormat.ARGB32, mipChain: true, linear: false);
            texture2D.filterMode = FilterMode.Point;
            texture2D.ReadPixels(source, 0, 0);
            texture2D.Apply();
            GL.sRGBWrite = sRGBWrite;
            RenderTexture.active = active;
            RenderTexture.ReleaseTemporary(renderTexture);
            return texture2D;
        }

        //
        // 摘要:
        //     Resizes a texture by blitting, this allows you to resize unreadable textures.
        public static Texture2D ResizeByBlit(this Texture texture, int width, int height, FilterMode filterMode = FilterMode.Bilinear)
        {
            RenderTexture active = RenderTexture.active;
            RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 1);
            temporary.filterMode = FilterMode.Bilinear;
            RenderTexture.active = temporary;
            GL.Clear(clearDepth: false, clearColor: true, new Color(1f, 1f, 1f, 0f));
            bool sRGBWrite = GL.sRGBWrite;
            GL.sRGBWrite = false;
            Graphics.Blit(texture, temporary);
            Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, mipChain: true, linear: false);
            texture2D.filterMode = filterMode;
            texture2D.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
            texture2D.Apply();
            RenderTexture.active = active;
            RenderTexture.ReleaseTemporary(temporary);
            GL.sRGBWrite = sRGBWrite;
            return texture2D;
        }

        //
        // 摘要:
        //     Converts a Sprite to a Texture2D.
        //
        // 参数:
        //   sprite:
        public static Texture2D ConvertSpriteToTexture(Sprite sprite)
        {
            Rect rect = sprite.rect;
            if (extractSpriteMaterial == null || extractSpriteMaterial.shader == null)
            {
                extractSpriteMaterial = new Material(ShaderUtil.CreateShaderAsset(extractSpriteShader));
            }

            extractSpriteMaterial.SetVector("_TexelSize", new Vector2(1f / (float)sprite.texture.width, 1f / (float)sprite.texture.height));
            extractSpriteMaterial.SetVector("_Rect", new Vector4(rect.x / (float)sprite.texture.width, rect.y / (float)sprite.texture.height, rect.width / (float)sprite.texture.width, rect.height / (float)sprite.texture.height));
            bool sRGBWrite = GL.sRGBWrite;
            GL.sRGBWrite = false;
            RenderTexture active = RenderTexture.active;
            RenderTexture renderTexture = (RenderTexture.active = RenderTexture.GetTemporary((int)rect.width, (int)rect.height, 0));
            GL.Clear(clearDepth: false, clearColor: true, new Color(1f, 1f, 1f, 0f));
            Graphics.Blit(sprite.texture, renderTexture, extractSpriteMaterial);
            Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, mipChain: true, linear: false);
            texture2D.filterMode = FilterMode.Bilinear;
            texture2D.ReadPixels(new Rect(0f, 0f, renderTexture.width, renderTexture.height), 0, 0);
            texture2D.alphaIsTransparency = true;
            texture2D.Apply();
            RenderTexture.ReleaseTemporary(renderTexture);
            RenderTexture.active = active;
            GL.sRGBWrite = sRGBWrite;
            return texture2D;
        }
    }
}

