using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.MegaWorldSystem
{
    [System.Serializable]
    public class TerrainTextureSettings
    {
        public Texture2D DiffuseTexture = null;

        public TerrainLayer Convert()
        {
            return new TerrainLayer
            {
                metallic = 0.0f,
                normalMapTexture = null,
                smoothness = 0.0f,
                diffuseTexture = DiffuseTexture,
                tileOffset = Vector2.zero,
                tileSize = Vector2.one,
                specular = Color.black
            };
        }

        public TerrainTextureSettings()
        {

        }
        
        public TerrainTextureSettings(Texture2D texture)
        {
            DiffuseTexture = texture;
        }
        
        public TerrainTextureSettings(TerrainLayer terrainLayer)
        {
            if (terrainLayer != null)
            {
                DiffuseTexture = terrainLayer.diffuseTexture;
            }
        }
    }
}
