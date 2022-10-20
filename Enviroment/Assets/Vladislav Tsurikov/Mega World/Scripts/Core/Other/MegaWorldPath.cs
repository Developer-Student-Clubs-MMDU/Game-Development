using System;
using UnityEngine;
using VladislavTsurikov.MegaWorldSystem.GameObjectStorage;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class MegaWorldPath 
    {
        private static AdvancedSettings advancedSettings;
        public static AdvancedSettings AdvancedSettings
        {
            get
            {
                if (advancedSettings == null)
                    advancedSettings = ProfileFactory.GetDefaultAdvancedSettings();
                return advancedSettings;
            }
            set
            {
                advancedSettings = value;
            }
        }

        private static DataPackage dataPackage;
        public static DataPackage DataPackage
        {
            get
            {
                if (dataPackage == null)
                    dataPackage = ProfileFactory.GetDataPackage();
                return dataPackage;
            }
            set
            {
                dataPackage = value;
            }
        }

        private static CommonDataPackage _commonDataPackage;
        public static CommonDataPackage CommonDataPackage
        {
            get
            {
                if (_commonDataPackage == null)
                {
                    _commonDataPackage = ProfileFactory.GetCommonDataPackage();
#if UNITY_EDITOR
                    CreateMegaWorldWindowSettings.CreateSettings();
#endif
                    
                }
                    
                return _commonDataPackage;
            }
            set
            {
                _commonDataPackage = value;
            }
        }

        private static GameObjectStoragePackage gameObjectStoragePackage;
        public static GameObjectStoragePackage GameObjectStoragePackage
        {
            get
            {
                if (gameObjectStoragePackage == null)
                    gameObjectStoragePackage = ProfileFactory.GetGameObjectStoragePackage();
                return gameObjectStoragePackage;
            }
            set
            {
                gameObjectStoragePackage = value;
            }
        }
        
        public static string MegaWorld = "MegaWorld";
        public static string DataPackageName = "Data Package";
        public static string CommonDataPackageName = "Common Data Package";
        public static string GameObjectStoragePackageName = "GameObject Storage";
        
        public static string Groups = "Groups"; 
        public static string Prototypes = "Prototypes";
        public static string Resources = "Resources";
        public static string PolarisBrushes = "Polaris Brushes";
        public static string MegaWorldTerrainLayers = "Mega World Terrain Layers";
        public static string AdvancedSettingsName = "Advanced Settings";
        public static string ToolsSettingsName = "Tools Settings";

        public static string pathToResources = CommonPath.CombinePath("Assets", Resources);
        public static string pathToResourcesMegaWorld = CommonPath.CombinePath(pathToResources, MegaWorld);
        public static string pathToDataPackage = CommonPath.CombinePath(pathToResourcesMegaWorld, DataPackageName);
        public static string pathToCommonDataPackage = CommonPath.CombinePath(pathToResourcesMegaWorld, CommonDataPackageName);
        public static string pathToGameObjectStoragePackage = CommonPath.CombinePath(pathToResourcesMegaWorld, GameObjectStoragePackageName);
        public static string pathToGroup = CommonPath.CombinePath(pathToDataPackage, Groups);
        public static string pathToPrototype = CommonPath.CombinePath(pathToGroup, Prototypes);
        public static string pathToAdvancedSettings = CommonPath.CombinePath(pathToResourcesMegaWorld, AdvancedSettingsName);
        public static string terrainLayerStoragePath = CommonPath.CombinePath("Assets", MegaWorldTerrainLayers);
        public static string pathToToolsSettings = CommonPath.CombinePath(pathToResourcesMegaWorld, ToolsSettingsName);
    }
}