#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace VladislavTsurikov
{
    [InitializeOnLoad]
    public static class ShaderVariantCollectionPath
    {        
        private static ShaderVariantCollection _shaderVariantCollection;
        public static ShaderVariantCollection ShaderVariantCollection
        {
            get
            {
                if (_shaderVariantCollection == null) _shaderVariantCollection = GetPackage();
                return _shaderVariantCollection;
            }
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            if (_shaderVariantCollection == null) _shaderVariantCollection = GetPackage();
        }

        public static ShaderVariantCollection GetPackage()
        {
            ShaderVariantCollection shaderVariantCollection = Resources.Load<ShaderVariantCollection>(CommonPath.ShaderVariantCollectionName); 

            if (shaderVariantCollection == null)
            {
                shaderVariantCollection = new ShaderVariantCollection();

                if (!Application.isPlaying)
                {
                    if (!System.IO.Directory.Exists(CommonPath.PathToResources))
                    {
                        System.IO.Directory.CreateDirectory(CommonPath.PathToResources);
                    }

                    AssetDatabase.CreateAsset(shaderVariantCollection, CommonPath.PathToShaderVariantCollection + ".shadervariants");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }

            return shaderVariantCollection;
        }
    }
}
#endif