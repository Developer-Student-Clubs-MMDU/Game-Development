using System;
using UnityEngine;
using UnityEditor;

namespace VladislavTsurikov.MegaWorldSystem.BrushErase
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public static class BrushEraseToolPath 
    {
        private static BrushEraseToolSettings _settings;
        public static BrushEraseToolSettings Settings
        {
            get
            {
                if (_settings == null) _settings = GetPackage();
                return _settings;
            }
        }

        public static string ToolName = "Brush Erase Tool";

        static BrushEraseToolPath()
        {
            if (_settings == null) _settings = GetPackage();
        }

        private static BrushEraseToolSettings GetPackage()
        {
            string path = CommonPath.CombinePath(MegaWorldPath.MegaWorld, VladislavTsurikov.MegaWorldSystem.MegaWorldPath.ToolsSettingsName);

            BrushEraseToolSettings package = Resources.Load<BrushEraseToolSettings>(CommonPath.CombinePath(path, ToolName));
            
            if (package == null)
            {
                package = ScriptableObject.CreateInstance<BrushEraseToolSettings>();
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