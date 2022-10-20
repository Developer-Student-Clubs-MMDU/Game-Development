using System;
using UnityEngine;

namespace VladislavTsurikov
{
    [Serializable]
    public class CommonPath : ScriptableObject
    {
        public static string Resources = "Resources";
        public static string ShaderVariantCollectionName = "Shader Variant Collection";

        public static string PathToResources = CombinePath("Assets", Resources);
        public static string PathToShaderVariantCollection = CombinePath(PathToResources, ShaderVariantCollectionName);

        public static string CombinePath(string path1, string path2)
        {
            return path1 + "/" + path2;
        }
    }
}