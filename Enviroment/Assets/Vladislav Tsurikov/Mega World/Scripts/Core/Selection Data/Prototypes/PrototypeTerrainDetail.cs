using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.MegaWorldSystem
{
    public enum PrefabType
    {
        Mesh = 0,
        Texture = 1
    }
    
    [System.Serializable]
    public class PrototypeTerrainDetail : Prototype
    {
        public string TerrainDetailName = "Default";
        public int TerrainProtoId = 0;

        public PrefabType PrefabType = PrefabType.Mesh;
        public Texture2D DetailTexture;

        public PrototypeTerrainDetail()
        {
            
        }

#if UNITY_EDITOR
        public override void SetIconInfo(out Texture2D preview, out string name)
		{
			if(PrefabType == PrefabType.Mesh)
			{
            	if (Prefab != null)
            	{
            	    preview = MegaWorldGUIUtility.GetPrefabPreviewTexture(Prefab);      
					name = TerrainDetailName;
            	}
				else
				{
					preview = null;
					name = "Missing Prefab";
				}
			}
			else
			{
            	if (DetailTexture != null)
            	{
            	    preview = DetailTexture;      
					name = TerrainDetailName;
            	}
				else
				{
					preview = null;
					name = "Missing Texture";
				}
			}
		}
#endif
        
        public static PrototypeTerrainDetail Create(Group group, GameObject detailProtoype)
        {
			PrototypeTerrainDetail proto = (PrototypeTerrainDetail)Prototype.Create(detailProtoype.name, group, typeof(PrototypeTerrainDetail));
			proto.Init(detailProtoype);
			return proto;
        }

        public static PrototypeTerrainDetail Create(Group group, Texture2D detailTexture)
        {
			PrototypeTerrainDetail proto = (PrototypeTerrainDetail)Prototype.Create(detailTexture.name, group, typeof(PrototypeTerrainDetail));
			proto.Init(detailTexture, detailTexture.name);
			return proto;
        }

        private void Init(GameObject detailProtoype)
        {
            PrefabType = PrefabType.Mesh;
            
            TerrainDetailName = detailProtoype.name;
            Prefab = detailProtoype;

#if UNITY_EDITOR
            SettingsStack.Create(typeof(TerrainDetailSettings), this);
            TerrainDetailSettings terrainDetailSettings = (TerrainDetailSettings)SettingsStack.GetSettings(typeof(TerrainDetailSettings));
            terrainDetailSettings.RenderMode = DetailRenderMode.Grass;

            CreateMegaWorldWindowSettings.CreatePrototypeSettings(this);
            SpawnDetailSettings spawnDetailSettings = (SpawnDetailSettings)this.GetSettings(typeof(SpawnDetailSettings));
            spawnDetailSettings.FailureRate = 0;
#endif
        }

        private void Init(Texture2D detailTexture, string name)
        {
            PrefabType = PrefabType.Texture;

            this.DetailTexture = detailTexture;
            TerrainDetailName = name;

#if UNITY_EDITOR
            SettingsStack.Create(typeof(TerrainDetailSettings), this);
            TerrainDetailSettings terrainDetailSettings = (TerrainDetailSettings)SettingsStack.GetSettings(typeof(TerrainDetailSettings));
            terrainDetailSettings.RenderMode = DetailRenderMode.GrassBillboard;

            CreateMegaWorldWindowSettings.CreatePrototypeSettings(this);
            SpawnDetailSettings spawnDetailSettings = (SpawnDetailSettings)this.GetSettings(typeof(SpawnDetailSettings));
            spawnDetailSettings.FailureRate = 0;
#endif
        }
    }
}