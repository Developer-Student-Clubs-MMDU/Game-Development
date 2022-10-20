using System;
using UnityEngine;
using UnityEditor;

namespace VladislavTsurikov.MegaWorldSystem.SprayBrush
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public static class SprayBrushToolPath 
    {
        private static SprayBrushToolSettings _settings;
        public static SprayBrushToolSettings Settings
        {
            get
            {
                if (_settings == null) _settings = GetPackage();
                return _settings; 
            }
        }

        public static string ToolName = "Spray Brush Tool";

        static SprayBrushToolPath()
        {
            if (_settings == null) _settings = GetPackage();
        } 

        private static SprayBrushToolSettings GetPackage()
        {
            string path = CommonPath.CombinePath(MegaWorldPath.MegaWorld, VladislavTsurikov.MegaWorldSystem.MegaWorldPath.ToolsSettingsName);

            SprayBrushToolSettings package = Resources.Load<SprayBrushToolSettings>(CommonPath.CombinePath(path, ToolName));
            
            if (package == null)
            {
                package = ScriptableObject.CreateInstance<SprayBrushToolSettings>();
#if UNITY_EDITOR
                if (!System.IO.Directory.Exists(MegaWorldPath.pathToToolsSettings))
                {
                    System.IO.Directory.CreateDirectory(MegaWorldPath.pathToToolsSettings);
                }

                AssetDatabase.CreateAsset(package, CommonPath.CombinePath(MegaWorldPath.pathToToolsSettings, ToolName) + ".asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
#endif
            }

            return package;
        }
    }
}