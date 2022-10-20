#if UNITY_EDITOR
using UnityEngine;

namespace VladislavTsurikov
{
    public static class ShaderVariantCollectionUtility 
    {
        public static void AddShaderVariantToCollection(Shader shader, string[] shaderKeywords)
        {
            if (shader != null)
            {
                ShaderVariantCollection.ShaderVariant shaderVariant = new ShaderVariantCollection.ShaderVariant();
                shaderVariant.shader = shader;
                shaderVariant.keywords = shaderKeywords;
                ShaderVariantCollectionPath.ShaderVariantCollection.Add(shaderVariant);
            }
        }

        public static void AddShaderVariantToCollection(Material material)
        {
            if (!Application.isPlaying && !string.IsNullOrEmpty(material.shader.name) && material)
            {
                Shader shader = material.shader;
                if (shader != null) 
                {
                    ShaderVariantCollection.ShaderVariant shaderVariant = new ShaderVariantCollection.ShaderVariant();
                    shaderVariant.shader = shader;
                    shaderVariant.keywords = material.shaderKeywords;
                    ShaderVariantCollectionPath.ShaderVariantCollection.Add(shaderVariant);
                }
            }
        }
    }
}
#endif