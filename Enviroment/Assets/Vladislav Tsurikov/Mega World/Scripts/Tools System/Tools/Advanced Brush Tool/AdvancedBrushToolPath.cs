using System;
using UnityEngine;
using UnityEditor;

namespace VladislavTsurikov.MegaWorldSystem.AdvancedBrush
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public static class AdvancedBrushToolPath 
    {
        private static AdvancedBrushToolSettings _settings;
        public static AdvancedBrushToolSettings Settings
        {
            get
            {
                if (_settings == null) _settings = GetPackage();
                return _settings; 
            }
        }

        public static string ToolName = "Advanced Brush Tool";

        static AdvancedBrushToolPath()
        {
            if (_settings == null) _settings = GetPackage();
        } 

        private static AdvancedBrushToolSettings GetPackage()
        {
            string path = CommonPath.CombinePath(MegaWorldPath.MegaWorld, VladislavTsurikov.MegaWorldSystem.MegaWorldPath.ToolsSettingsName);

            AdvancedBrushToolSettings package = Resources.Load<AdvancedBrushToolSettings>(CommonPath.CombinePath(path, ToolName));
            
            if (package == null)
            {
                package = ScriptableObject.CreateInstance<AdvancedBrushToolSettings>();
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