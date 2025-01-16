using System;
using System.IO;

namespace KFrame.Editor.ScriptTemplates
{
    public class ScriptsInfoRecoder : UnityEditor.AssetModificationProcessor
    {
        private static void OnWillCreateAsset(string path)
        {
            path = path.Replace(".meta", "");
            if (path.EndsWith(".cs") && File.Exists(path)) 
            {
                string str = File.ReadAllText(path);
                str = str.Replace("#AUTHORNAME#", Environment.UserName).Replace(
                    "#CREATETIME#", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss dddd"));
                File.WriteAllText(path, str);
            }
        }
    }
}