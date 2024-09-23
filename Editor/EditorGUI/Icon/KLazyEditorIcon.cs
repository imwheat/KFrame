//****************** 代码文件申明 ***********************
//* 文件：KLazyEditorIcon
//* 作者：wheat
//* 创建时间：2024/05/28 08:52:18 星期二
//* 描述：
//*******************************************************

using UnityEngine;
using UnityEditor;
using KFrame;
using KFrame.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;

namespace KFrame.Editor
{
    public class KLazyEditorIcon : KEditorIcon, IDisposable
    {
        #region 显示风格配置

        /// <summary>
        /// 图片的shader
        /// </summary>
        private static readonly string iconShader = "\r\nShader \"Hidden/KFrame/Editor/GUIIcon\"\r\n{\r\n\tProperties\r\n\t{\r\n        _MainTex(\"Texture\", 2D) = \"white\" {}\r\n        _Color(\"Color\", Color) = (1,1,1,1)\r\n\t}\r\n    SubShader\r\n\t{\r\n        Blend SrcAlpha Zero\r\n        Pass\r\n        {\r\n            CGPROGRAM\r\n                #pragma vertex vert\r\n                #pragma fragment frag\r\n                #include \"UnityCG.cginc\"\r\n\r\n                struct appdata\r\n                {\r\n                    float4 vertex : POSITION;\r\n\t\t\t\t\tfloat2 uv : TEXCOORD0;\r\n\t\t\t\t};\r\n\r\n                struct v2f\r\n                {\r\n                    float2 uv : TEXCOORD0;\r\n\t\t\t\t\tfloat4 vertex : SV_POSITION;\r\n\t\t\t\t};\r\n\r\n                sampler2D _MainTex;\r\n                float4 _Color;\r\n\r\n                v2f vert(appdata v)\r\n                {\r\n                    v2f o;\r\n                    o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);\r\n                    o.uv = v.uv;\r\n                    return o;\r\n                }\r\n\r\n                fixed4 frag(v2f i) : SV_Target\r\n\t\t\t\t{\r\n                    // drop shadow:\r\n                    // float texelSize = 1.0 / 34.0;\r\n                    // float2 shadowUv = clamp(i.uv + float2(-texelSize, texelSize * 2), float2(0, 0), float2(1, 1));\r\n                    // fixed4 shadow = fixed4(0, 0, 0, tex2D(_MainTex, shadowUv).a); \r\n\r\n\t\t\t\t\tfixed4 col = _Color;\r\n\t\t\t\t\tcol.a *= tex2D(_MainTex, i.uv).a;\r\n\r\n                    // drop shadow:\r\n                    // col = lerp(shadow, col, col.a);\r\n\r\n\t\t\t\t\treturn col;\r\n\t\t\t\t}\r\n\t\t\tENDCG\r\n\t\t}\r\n\t}\r\n}\r\n";

        /// <summary>
        /// 未激活时颜色(深色)
        /// </summary>
        private static Color inactiveColorPro = new Color(0.4f, 0.4f, 0.4f, 1f);

        /// <summary>
        /// 激活时颜色(深色)
        /// </summary>
        private static Color activeColorPro = new Color(0.55f, 0.55f, 0.55f, 1f);

        /// <summary>
        /// 高亮时颜色(深色)
        /// </summary>
        private static Color highlightedColorPro = new Color(0.9f, 0.9f, 0.9f, 1f);

        /// <summary>
        /// 未激活时颜色(浅色)
        /// </summary>
        private static Color inactiveColor = new Color(0.72f, 0.72f, 0.72f, 1f);

        /// <summary>
        /// 激活时颜色(浅色)
        /// </summary>
        private static Color activeColor = new Color(0.4f, 0.4f, 0.4f, 1f);

        /// <summary>
        /// 高亮时颜色(浅色)
        /// </summary>
        private static Color highlightedColor = new Color(0.2f, 0.2f, 0.2f, 1f);

        #endregion

        #region 材质纹理参数

        /// <summary>
        /// 图片材质
        /// </summary>
        private static Material iconMat;

        /// <summary>
        /// 图片纹理
        /// </summary>
        private Texture2D icon;

        /// <summary>
        /// 未激活时的纹理
        /// </summary>
        private Texture inactive;

        /// <summary>
        /// 激活时的纹理
        /// </summary>
        private Texture active;

        /// <summary>
        /// 高亮显示时的纹理
        /// </summary>
        private Texture highlighted;

        /// <summary>
        /// 图片的字符串数据
        /// </summary>
        private string data;

        /// <summary>
        /// 图片宽度
        /// </summary>
        private int width;

        /// <summary>
        /// 图片高度
        /// </summary>
        private int height;

        #endregion

        #region 各个状态下的材质纹理

        /// <summary>
        /// 高亮显示时的GUI材质纹理
        /// </summary>
        public override Texture Highlighted
        {
            get
            {
                if (highlighted == null)
                {
                    highlighted = RenderIcon(EditorGUIUtility.isProSkin ? highlightedColorPro : highlightedColor);
                }

                return highlighted;
            }
        }

        /// <summary>
        /// 激活时的GUI材质纹理
        /// </summary>
        public override Texture Active
        {
            get
            {
                if (active == null)
                {
                    active = RenderIcon(EditorGUIUtility.isProSkin ? activeColorPro : activeColor);
                }

                return active;
            }
        }

        /// <summary>
        /// 没激活时的GUI材质纹理
        /// </summary>
        public override Texture Inactive
        {
            get
            {
                if (inactive == null)
                {
                    inactive = RenderIcon(EditorGUIUtility.isProSkin ? inactiveColorPro : inactiveColor);
                }

                return inactive;
            }
        }

        /// <summary>
        /// 未加工的GUI材质纹理
        /// </summary>
        public override Texture2D Raw
        {
            get
            {
                if (icon == null)
                {
                    byte[] bytes = Convert.FromBase64String(data);
                    icon = TextureUtility.LoadImage(width, height, bytes);
                }

                return icon;
            }
        }

        #endregion

        #region 构造与释放

        /// <summary>
        /// 从图集里获取图片
        /// </summary>
        public KLazyEditorIcon(int width, int height, string base64ImageDataPngOrJPG)
        {
            this.width = width;
            this.height = height;
            data = base64ImageDataPngOrJPG;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (icon != null)
            {
                UnityEngine.Object.DestroyImmediate(icon);
            }

            if (inactive != null)
            {
                UnityEngine.Object.DestroyImmediate(inactive);
            }

            if (active != null)
            {
                UnityEngine.Object.DestroyImmediate(active);
            }

            if (highlighted != null)
            {
                UnityEngine.Object.DestroyImmediate(highlighted);
            }
        }

        #endregion

        /// <summary>
        /// 根据Shader渲染图片然后再调一下颜色
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private Texture RenderIcon(Color color)
        {
            //如果没有材质的话先创建一个
            if (iconMat == null || iconMat.shader == null)
            {
                //先尝试从SessionState获取
                int @int = SessionState.GetInt("kframe_inspector_lazy_icons", 0);
                if (@int != 0)
                {
                    iconMat = EditorUtility.InstanceIDToObject(@int) as Material;
                }

                //如果没有的话那就创建
                if (iconMat == null)
                {
                    Shader shader = ShaderUtil.CreateShaderAsset(iconShader);
                    iconMat = new Material(shader);
                    UnityEngine.Object.DontDestroyOnLoad(shader);
                    UnityEngine.Object.DontDestroyOnLoad(iconMat);
                    iconMat.hideFlags = HideFlags.DontUnloadUnusedAsset;
                    SessionState.SetInt("kframe_inspector_lazy_icons", iconMat.GetInstanceID());
                }
            }

            //配置颜色
            iconMat.SetColor("_Color", color);

            //使用RenderTexture渲染一下材质纹理

            //先保存原先状态
            bool sRGBWrite = GL.sRGBWrite;
            GL.sRGBWrite = true;
            RenderTexture renderTexture = RenderTexture.active;

            //创建一个临时的纹理，然后用Shader进行绘制
            RenderTexture renderTexture2 = (RenderTexture.active = RenderTexture.GetTemporary(width, height, 0));
            GL.Clear(clearDepth: false, clearColor: true, new Color(1f, 1f, 1f, 0f));
            Graphics.Blit(Raw, renderTexture2, iconMat);

            //创建一个新的纹理，然后读取复制经过Shader绘制的纹理
            Texture2D texture2D = new Texture2D(renderTexture2.width, renderTexture2.height, TextureFormat.ARGB32, mipChain: false, linear: true);
            texture2D.filterMode = FilterMode.Bilinear;
            texture2D.ReadPixels(new Rect(0f, 0f, renderTexture2.width, renderTexture2.height), 0, 0);
            texture2D.alphaIsTransparency = true;
            texture2D.Apply();

            //释放资源、恢复原先状态
            RenderTexture.ReleaseTemporary(renderTexture2);
            RenderTexture.active = renderTexture;
            GL.sRGBWrite = sRGBWrite;

            //返回经过shader绘制、调过色的材质纹理
            return texture2D;
        }
    }
}

