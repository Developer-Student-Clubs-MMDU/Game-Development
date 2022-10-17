#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;

namespace VladislavTsurikov.PhysicsSimulatorEditor
{
    [InitializeOnLoad]
    public static class PhysicsSimulatorPath 
    {
        private static PhysicsSimulatorSettings _settings;
        public static PhysicsSimulatorSettings Settings
        {
            get
            {
                if (_settings == null) _settings = GetPackage();
                return _settings; 
            }
        }

        public static string SettingsName = "Physics Simulation Settings";

        static PhysicsSimulatorPath()
        {
            if (_settings == null) _settings = GetPackage();
        } 

        private static PhysicsSimulatorSettings GetPackage()
        {
            string path = CommonPath.CombinePath(CommonPath.PathToResources, SettingsName);

            PhysicsSimulatorSettings package = Resources.Load<PhysicsSimulatorSettings>(SettingsName);
            
            if (package == null)
            {
                package = ScriptableObject.CreateInstance<PhysicsSimulatorSettings>();

                if (!System.IO.Directory.Exists(CommonPath.PathToResources))
                {
                    System.IO.Directory.CreateDirectory(CommonPath.PathToResources);
                }

                AssetDatabase.CreateAsset(package, path + ".asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            return package;
        }
    }
}
#endif