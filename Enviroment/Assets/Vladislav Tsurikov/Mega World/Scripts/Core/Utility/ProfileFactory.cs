using System.IO;
using UnityEngine;
using UnityEditor;
using VladislavTsurikov.MegaWorldSystem.GameObjectStorage;

namespace VladislavTsurikov.MegaWorldSystem
{
    public static class ProfileFactory
    {        
        public static Group CreateGroup(string targetName)
        {
#if UNITY_EDITOR
                Directory.CreateDirectory(MegaWorldPath.pathToDataPackage);
                Directory.CreateDirectory(MegaWorldPath.pathToGroup);

                var path = string.Empty;

                path += targetName + " Group.asset";

                path = CommonPath.CombinePath(MegaWorldPath.pathToGroup, path);
                path = AssetDatabase.GenerateUniqueAssetPath(path);

                var asset = ScriptableObject.CreateInstance<Group>();
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                CreateMegaWorldWindowSettings.CreateGroupSettings(asset);
                return asset;

#else 
                return null;
#endif
        }

        public static TerrainLayer SaveTerrainLayerAsAsset(string textureName, TerrainLayer terrainLayer)
        {
#if UNITY_EDITOR
                Directory.CreateDirectory(MegaWorldPath.terrainLayerStoragePath);

                string path = MegaWorldPath.terrainLayerStoragePath + "/" + textureName + ".asset";

                path = AssetDatabase.GenerateUniqueAssetPath(path);

                AssetDatabase.CreateAsset(terrainLayer, path);
                AssetDatabase.SaveAssets();

                return AssetDatabase.LoadAssetAtPath<TerrainLayer>(path);

#else
                return null;
#endif
        }

        public static AdvancedSettings GetDefaultAdvancedSettings()
        {
            AdvancedSettings advancedSettings = Resources.Load<AdvancedSettings>(CommonPath.CombinePath(MegaWorldPath.MegaWorld, MegaWorldPath.AdvancedSettingsName));
                
            if (advancedSettings == null)
            {
                advancedSettings = ScriptableObject.CreateInstance<AdvancedSettings>();
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    if (!System.IO.Directory.Exists(MegaWorldPath.pathToResourcesMegaWorld))
                    {
                        System.IO.Directory.CreateDirectory(MegaWorldPath.pathToResourcesMegaWorld);
                    }

                    AssetDatabase.CreateAsset(advancedSettings, MegaWorldPath.pathToAdvancedSettings + ".asset");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
#endif
            }
            
            return advancedSettings;
        }

        public static DataPackage GetDataPackage()
        {
            DataPackage asset = Resources.Load<DataPackage>(CommonPath.CombinePath(MegaWorldPath.MegaWorld, MegaWorldPath.DataPackageName));
            
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<DataPackage>();
#if UNITY_EDITOR
                if (!System.IO.Directory.Exists(MegaWorldPath.pathToResourcesMegaWorld))
                {
                    System.IO.Directory.CreateDirectory(MegaWorldPath.pathToResourcesMegaWorld);
                }

                AssetDatabase.CreateAsset(asset, MegaWorldPath.pathToDataPackage + ".asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
#endif
            }

            return asset;
        }

        public static CommonDataPackage GetCommonDataPackage()
        {
            CommonDataPackage asset = Resources.Load<CommonDataPackage>(CommonPath.CombinePath(MegaWorldPath.MegaWorld, MegaWorldPath.CommonDataPackageName));
            
            if (asset == null)
            {                
                asset = ScriptableObject.CreateInstance<CommonDataPackage>();
#if UNITY_EDITOR
                if (!System.IO.Directory.Exists(MegaWorldPath.pathToResourcesMegaWorld))
                {
                    System.IO.Directory.CreateDirectory(MegaWorldPath.pathToResourcesMegaWorld);
                }

                AssetDatabase.CreateAsset(asset, MegaWorldPath.pathToCommonDataPackage + ".asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
#endif
            }

            return asset;
        }

        public static GameObjectStoragePackage GetGameObjectStoragePackage()
        {
            GameObjectStoragePackage asset = Resources.Load<GameObjectStoragePackage>(CommonPath.CombinePath(MegaWorldPath.MegaWorld, MegaWorldPath.GameObjectStoragePackageName));
            
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<GameObjectStoragePackage>();
#if UNITY_EDITOR
                if (!System.IO.Directory.Exists(MegaWorldPath.pathToResourcesMegaWorld))
                {
                    System.IO.Directory.CreateDirectory(MegaWorldPath.pathToResourcesMegaWorld);
                }

                AssetDatabase.CreateAsset(asset, MegaWorldPath.pathToGameObjectStoragePackage + ".asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
#endif
            }

            return asset;
        }
    }
}