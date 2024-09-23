//****************** 代码文件申明 ***********************
//* 文件：TextureUtility
//* 作者：wheat
//* 创建时间：2024/05/28 08:55:04 星期二
//* 描述：
//*******************************************************

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using Object = UnityEngine.Object;

namespace KFrame.Editor
{
    public static class TextureUtility
    {
        private static Material extractSpriteMaterial;

        private static readonly string extractSpriteShader = "\r\n            Shader \"Hidden/KFrame/Editor/GUIIcon\"\r\n            {\r\n\t            Properties\r\n\t            {\r\n                    _MainTex(\"Texture\", 2D) = \"white\" {}\r\n                    _Color(\"Color\", Color) = (1,1,1,1)\r\n                    _Rect(\"Rect\", Vector) = (0,0,0,0)\r\n                    _TexelSize(\"TexelSize\", Vector) = (0,0,0,0)\r\n\t            }\r\n                SubShader\r\n\t            {\r\n                    Blend SrcAlpha OneMinusSrcAlpha\r\n                    Pass\r\n                    {\r\n                        CGPROGRAM\r\n                            #pragma vertex vert\r\n                            #pragma fragment frag\r\n                            #include \"UnityCG.cginc\"\r\n\r\n                            struct appdata\r\n                            {\r\n                                float4 vertex : POSITION;\r\n\t\t\t\t\t            float2 uv : TEXCOORD0;\r\n\t\t\t\t            };\r\n\r\n                            struct v2f\r\n                            {\r\n                                float2 uv : TEXCOORD0;\r\n\t\t\t\t\t            float4 vertex : SV_POSITION;\r\n\t\t\t\t            };\r\n\r\n                            sampler2D _MainTex;\r\n                            float4 _Rect;\r\n\r\n                            v2f vert(appdata v)\r\n                            {\r\n                                v2f o;\r\n                                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);\r\n                                o.uv = v.uv;\r\n                                return o;\r\n                            }\r\n\r\n                            fixed4 frag(v2f i) : SV_Target\r\n\t\t\t\t            {\r\n                                float2 uv = i.uv;\r\n                                uv *= _Rect.zw;\r\n\t\t\t\t\t            uv += _Rect.xy;\r\n\t\t\t\t\t            return tex2D(_MainTex, uv);\r\n\t\t\t\t            }\r\n\t\t\t            ENDCG\r\n\t\t            }\r\n\t            }\r\n            }";

        //
        // 摘要:
        //     Creates a new texture with no mimapping, linier colors, and calls texture.LoadImage(bytes),
        //     DontDestroyOnLoad(tex) and sets hideFlags = DontUnloadUnusedAsset | DontSaveInEditor.
        //     Old description no longer relevant as we've moved past version 2017. Loads an
        //     image from bytes with the specified width and height. Use this instead of someTexture.LoadImage()
        //     if you're compiling to an assembly. Unity has moved the method in 2017, and Unity's
        //     assembly updater is not able to fix it for you. This searches for a proper LoadImage
        //     method in multiple locations, and also handles type name conflicts.
        /// <summary>
        /// 通过二进制数据创建加载一个Texture
        /// </summary>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        /// <param name="bytes">数据</param>
        /// <returns>创建的Texture</returns>
        public static Texture2D LoadImage(int width, int height, byte[] bytes)
        {
            Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, mipChain: false, linear: true);
            texture2D.LoadImage(bytes);
            Object.DontDestroyOnLoad(texture2D);
            texture2D.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontUnloadUnusedAsset;
            return texture2D;
        }

        /// <summary>
        /// 将Texture转为Texture2D
        /// </summary>
        /// <param name="texture">原texture</param>
        /// <param name="source"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 重新调整Texture的大小
        /// </summary>
        /// <param name="texture">原texture</param>
        /// <param name="width">新的宽</param>
        /// <param name="height">新的高</param>
        /// <param name="filterMode">模式</param>
        /// <returns>调整尺寸后的Texture</returns>
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

        /// <summary>
        /// 把sprite转为Texture
        /// </summary>
        /// <param name="sprite">要转换的sprite</param>
        /// <returns></returns>
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
        
        /// <summary>
        /// 生成所有单独的子Texture 并返回所有的路径
        /// </summary>
        /// <param name="multiImage"></param>
        /// <returns></returns>
        public static List<string> GeneratorChildTextureToPath(Texture2D multiImage)
        {
            //存放所有转换而来的png路径
            List<string> paths = new List<string>();
            //获取路径名称  
            string rootPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(multiImage));
            //图片路径名称 
            string path = rootPath + "/" + multiImage.name + ".PNG";
            //获取图片导入
            TextureImporter texImp = AssetImporter.GetAtPath(path) as TextureImporter;
            //创建文件夹
            AssetDatabase.CreateFolder(rootPath, multiImage.name);

            if (Directory.Exists(rootPath + "/" + multiImage.name))
            {
                //清空文件夹内容
                Directory.Delete(rootPath + "/" + multiImage.name, true);

                AssetDatabase.Refresh();
            }


            //遍历其中图集
            foreach (SpriteMetaData metaData in texImp.spritesheet)
            {
                Texture2D image = new Texture2D((int)metaData.rect.width, (int)metaData.rect.height);

                image.filterMode = FilterMode.Point;


                for (int y = (int)metaData.rect.y; y < metaData.rect.y + metaData.rect.height; y++) //Y轴像素  
                {
                    for (int x = (int)metaData.rect.x; x < metaData.rect.x + metaData.rect.width; x++)
                    {
                        image.SetPixel(x - (int)metaData.rect.x, y - (int)metaData.rect.y, multiImage.GetPixel(x, y));
                    }
                }

                //转换纹理到兼容格式
                if (image.format != TextureFormat.ARGB32 && image.format != TextureFormat.RGB24)
                {
                    Texture2D newTexture = new Texture2D(image.width, image.height);
                    newTexture.SetPixels(image.GetPixels(0), 0);
                    image = newTexture;
                }

                byte[] pngData = image.EncodeToPNG();
                string output_path = rootPath + "/" + multiImage.name + "/" + metaData.name + ".PNG"; //子图片输出路径
                File.WriteAllBytes(output_path, pngData);                                             //输出子PNG图片
                paths.Add(output_path);
            }

            // 刷新资源窗口界面  
            AssetDatabase.Refresh();

            return paths;
        }

        public static List<Texture2D> GeneratorChildTexture(Texture2D multImage)
        {
            //存放所有转换而来的png路径
            List<string> paths = new List<string>();
            List<Texture2D> child = new();
            //获取路径名称  
            string rootPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(multImage));
            //图片路径名称 
            string path = rootPath + "/" + multImage.name + ".PNG";
            //获取图片导入
            TextureImporter texImp = AssetImporter.GetAtPath(path) as TextureImporter;


            //创建文件夹
            AssetDatabase.CreateFolder(rootPath, multImage.name);

            if (Directory.Exists(rootPath + "/" + multImage.name))
            {
                //清空文件夹内容
                Directory.Delete(rootPath + "/" + multImage.name, true);

                AssetDatabase.Refresh();
            }


            //遍历其中图集
            foreach (SpriteMetaData metaData in texImp.spritesheet)
            {
                Texture2D image = new Texture2D((int)metaData.rect.width, (int)metaData.rect.height);

                image.filterMode = FilterMode.Point;

                for (int y = (int)metaData.rect.y; y < metaData.rect.y + metaData.rect.height; y++) //Y轴像素  
                {
                    for (int x = (int)metaData.rect.x; x < metaData.rect.x + metaData.rect.width; x++)
                    {
                        image.SetPixel(x - (int)metaData.rect.x, y - (int)metaData.rect.y, multImage.GetPixel(x, y));
                    }
                }

                //转换纹理到兼容格式
                if (image.format != TextureFormat.ARGB32 && image.format != TextureFormat.RGB24)
                {
                    Texture2D newTexture = new Texture2D(image.width, image.height);
                    newTexture.SetPixels(image.GetPixels(0), 0);
                    image = newTexture;
                }

                byte[] pngData = image.EncodeToPNG();
                string output_path = rootPath + "/" + multImage.name + "/" + metaData.name + ".PNG"; //子图片输出路径
                File.WriteAllBytes(output_path, pngData);                                            //输出子PNG图片
                paths.Add(output_path);
            }

            // 刷新资源窗口界面  
            AssetDatabase.Refresh();

            foreach (var s in paths)
            {
                child.Add(AssetDatabase.LoadAssetAtPath<Texture2D>(s));
            }

            return child;
        }


        /// <summary>
        /// 返回所有子图集
        /// </summary>
        /// <param name="multImage"></param>
        /// <returns></returns>
        public static List<Texture2D> GetChildTextures(Texture2D multImage)
        {
            List<Texture2D> textures = new();

            //获取路径名称  
            string rootPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(multImage));
            //图片路径名称 
            string path = rootPath + "/" + multImage.name + ".PNG";
            //获取图片导入
            TextureImporter texImp = AssetImporter.GetAtPath(path) as TextureImporter;

            //遍历其中图集
            foreach (SpriteMetaData metaData in texImp.spritesheet)
            {
                Texture2D image = new Texture2D((int)metaData.rect.width, (int)metaData.rect.height);

                for (int y = (int)metaData.rect.y; y < metaData.rect.y + metaData.rect.height; y++) //Y轴像素  
                {
                    for (int x = (int)metaData.rect.x; x < metaData.rect.x + metaData.rect.width; x++)
                        image.SetPixel(x - (int)metaData.rect.x, y - (int)metaData.rect.y, image.GetPixel(x, y));
                }

                textures.Add(image);
            }

            return textures;
        }
    }
}

