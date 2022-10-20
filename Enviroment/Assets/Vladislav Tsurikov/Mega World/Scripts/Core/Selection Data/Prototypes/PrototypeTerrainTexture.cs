using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.MegaWorldSystem
{
    [System.Serializable]
    public class PrototypeTerrainTexture : Prototype
    {
        public string TerrainTextureName = "Default";

        public TerrainTextureSettings TerrainTextureSettings = new TerrainTextureSettings();
        public TerrainLayer TerrainLayer;

        public PrototypeTerrainTexture()
        {
            
        }

#if UNITY_EDITOR
        public override void SetIconInfo(out Texture2D preview, out string name)
		{
            if (TerrainTextureSettings.DiffuseTexture != null)
            {
                preview = TerrainTextureSettings.DiffuseTexture;      
				name = TerrainTextureName;
            }
			else
			{
				preview = null;
				name = "Missing Texture";
			}
		}
#endif

        public static PrototypeTerrainTexture Create(Group group, Texture2D texture2D)
        {
			PrototypeTerrainTexture proto = (PrototypeTerrainTexture)Prototype.Create(texture2D.name, group, typeof(PrototypeTerrainTexture));
			proto.Init(texture2D);
			return proto;
        }

        public static PrototypeTerrainTexture Create(Group group, TerrainLayer terrainLayer)
        {
			PrototypeTerrainTexture proto = (PrototypeTerrainTexture)Prototype.Create(terrainLayer.name, group, typeof(PrototypeTerrainTexture));
			proto.Init(terrainLayer);
			return proto;
        }

        private void Init(Texture2D texture)
        {
            TerrainTextureName = texture.name;
            TerrainTextureSettings = new TerrainTextureSettings(texture);

#if UNITY_EDITOR
            CreateMegaWorldWindowSettings.CreatePrototypeSettings(this);
#endif
            
        }

        private void Init(TerrainLayer terrainLayer)
        {
            this.TerrainLayer = terrainLayer;
            TerrainTextureName = terrainLayer.name;
            TerrainTextureSettings = new TerrainTextureSettings(terrainLayer);

#if UNITY_EDITOR
            CreateMegaWorldWindowSettings.CreatePrototypeSettings(this);
#endif
        }
    }
}