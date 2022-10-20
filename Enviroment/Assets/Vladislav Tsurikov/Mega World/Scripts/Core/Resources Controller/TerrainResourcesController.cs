using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VladislavTsurikov.MegaWorldSystem
{
    public static class TerrainResourcesController
    {
        public enum TerrainResourcesSyncError
        {
            None,
            NotAllProtoAvailable,
            MissingPrototypes,
        }

        public static TerrainResourcesSyncError SyncError = TerrainResourcesSyncError.None;

        public static void AddMissingPrototypesToTerrains(Group group)
        {
            foreach (Terrain terrain in Terrain.activeTerrains)
            {
                AddMissingPrototypesToTerrain(terrain, group);
            }
        }

        public static void RemoveAllPrototypesFromTerrains(Group group)
        {
            foreach (Terrain terrain in Terrain.activeTerrains)
            {
                RemoveAllPrototypesFromTerrains(terrain, group);
            }
        }

        public static void AddMissingPrototypesToTerrain(Terrain terrain, Group group)
        {
            if (terrain == null)
            {
                Debug.LogWarning("Can not add resources to the terrain as no terrain has been supplied.");
                return;
            }

            switch (group.ResourceType)
            {
                case ResourceType.TerrainDetail:
                {
                    AddTerrainDetailToTerrain(terrain, group.ProtoTerrainDetailList);

                    break;
                }
                case ResourceType.TerrainTexture:
                {
                    AddTerrainTexturesToTerrain(terrain, group.ProtoTerrainTextureList);

                    break;
                }
            }

            terrain.Flush();

            SyncTerrainID(terrain, group);
        }

        public static void RemoveAllPrototypesFromTerrains(Terrain terrain, Group group)
        {
            if (terrain == null)
            {
                Debug.LogWarning("Can not add resources to the terrain as no terrain has been supplied.");
                return;
            }
            
            switch (group.ResourceType)
            {
                case ResourceType.TerrainDetail:
                {
                    List<DetailPrototype> terrainDetails = new List<DetailPrototype>();
                    
                    terrain.terrainData.detailPrototypes = terrainDetails.ToArray();

                    break;
                }
                case ResourceType.TerrainTexture:
                {
                    List<TerrainLayer> terrainLayers = new List<TerrainLayer>();

                    terrain.terrainData.terrainLayers = terrainLayers.ToArray();

                    break;
                }
            }

            terrain.Flush();

            SyncTerrainID(terrain, group);
        }

        public static void SetTerrainDetailSettings(Terrain activeTerrain, PrototypeTerrainDetail proto)
        {
            if(activeTerrain == null)
            {
                return;
            }

            DetailPrototype[] unityProto = activeTerrain.terrainData.detailPrototypes;

            TerrainDetailSettings terrainDetailSettings = (TerrainDetailSettings)proto.GetSettings(typeof(TerrainDetailSettings));
            			
			for (int index = 0; index < unityProto.Length; index++)
			{
				bool found = false;

				if(proto.PrefabType == PrefabType.Mesh)
				{
					if (CommonUtility.IsSameGameObject(unityProto[index].prototype, proto.Prefab, false))
                    {
                        found = true;
                    }
				}
				else
				{
					if (CommonUtility.IsSameTexture(unityProto[index].prototypeTexture, proto.DetailTexture, false))
                    {
                        found = true;
                    }
				}

				if (found)
            	{
            	    unityProto[index].renderMode = terrainDetailSettings.RenderMode;
	
					if (proto.Prefab != null)
            	    {
                        if(unityProto[index].renderMode == DetailRenderMode.GrassBillboard)
                        {
                            unityProto[index].renderMode = DetailRenderMode.Grass;
                        }

            	        unityProto[index].usePrototypeMesh = true;
            	        unityProto[index].prototype = proto.Prefab;
            	    }
            	    else
            	    {
            	        unityProto[index].usePrototypeMesh = false;
            	        unityProto[index].prototypeTexture = proto.DetailTexture;
            	    }

            	    unityProto[index].dryColor = terrainDetailSettings.DryColour;
            	    unityProto[index].healthyColor = terrainDetailSettings.HealthyColour;
            	    unityProto[index].maxHeight = terrainDetailSettings.MaxHeight;
            	    unityProto[index].maxWidth = terrainDetailSettings.MaxWidth;
            	    unityProto[index].minHeight = terrainDetailSettings.MinHeight;
            	    unityProto[index].minWidth = terrainDetailSettings.MinWidth;
            	    unityProto[index].noiseSpread = terrainDetailSettings.NoiseSpread;
            	}
			}

            activeTerrain.terrainData.detailPrototypes = unityProto;
        }

        public static void AddTerrainDetailToTerrain(Terrain terrain, List<PrototypeTerrainDetail> protoTerrainDetailList)
        {
            bool found = false;

            DetailPrototype newDetail;
            List<DetailPrototype> terrainDetails = new List<DetailPrototype>(terrain.terrainData.detailPrototypes);
            foreach (PrototypeTerrainDetail proto in protoTerrainDetailList)
            {
                TerrainDetailSettings terrainDetailSettings = (TerrainDetailSettings)proto.GetSettings(typeof(TerrainDetailSettings));

                found = false;
                foreach (DetailPrototype dp in terrainDetails)
                {
                    if (dp.renderMode == terrainDetailSettings.RenderMode)
                    {
                        if (CommonUtility.IsSameTexture(dp.prototypeTexture, proto.DetailTexture, false))
                        {
                            found = true;
                        }
                        if (CommonUtility.IsSameGameObject(dp.prototype, proto.Prefab, false))
                        {
                            found = true;
                        }
                    }
                }

                if (!found)
                {
                    newDetail = new DetailPrototype();
                    
                    if (proto.Prefab != null)
                    {
                        newDetail.renderMode = terrainDetailSettings.RenderMode;
                        newDetail.usePrototypeMesh = true;
                        newDetail.prototype = proto.Prefab;
                    }
                    else
                    {
                        newDetail.renderMode = terrainDetailSettings.RenderMode;
                        newDetail.usePrototypeMesh = false;
                        newDetail.prototypeTexture = proto.DetailTexture;
                    }

                    newDetail.dryColor = terrainDetailSettings.DryColour;
                    newDetail.healthyColor = terrainDetailSettings.HealthyColour;
                    newDetail.maxHeight = terrainDetailSettings.MaxHeight;
                    newDetail.maxWidth = terrainDetailSettings.MaxWidth;
                    newDetail.minHeight = terrainDetailSettings.MinHeight;
                    newDetail.minWidth = terrainDetailSettings.MinWidth;
                    newDetail.noiseSpread = terrainDetailSettings.NoiseSpread;
                    terrainDetails.Add(newDetail);
                }
            }

            terrain.terrainData.detailPrototypes = terrainDetails.ToArray();

            foreach (PrototypeTerrainDetail protoTerrainDetail in protoTerrainDetailList)
            {
                SetTerrainDetailSettings(terrain, protoTerrainDetail);
            }
        }

        public static void AddTerrainTexturesToTerrain(Terrain terrain, List<PrototypeTerrainTexture> protoTerrainTextureList)
        {
            bool found = false;

            TerrainLayer[] currentTerrainLayers = terrain.terrainData.terrainLayers;

            List<TerrainLayer> terrainLayers = new List<TerrainLayer>(terrain.terrainData.terrainLayers);

            foreach (PrototypeTerrainTexture splat in protoTerrainTextureList)
            {
                found = false;

                if(splat.TerrainLayer == null)
                {
                    foreach (TerrainLayer layer in currentTerrainLayers)
                    {
                        if (CommonUtility.IsSameTexture(layer.diffuseTexture, splat.TerrainTextureSettings.DiffuseTexture, false))
                        {
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        terrainLayers.Add(splat.TerrainTextureSettings.Convert());

                        RemoveTerrainLayerAssetFiles(splat.TerrainTextureSettings.DiffuseTexture.name);

                        terrainLayers[terrainLayers.Count - 1] = ProfileFactory.SaveTerrainLayerAsAsset(splat.TerrainTextureSettings.DiffuseTexture.name, terrainLayers.Last());

                        splat.TerrainLayer = terrainLayers[terrainLayers.Count - 1];
                    }
                }
                else
                {
                    foreach (TerrainLayer layer in currentTerrainLayers)
                    {
                        if (CommonUtility.IsSameTexture(layer.diffuseTexture, splat.TerrainTextureSettings.DiffuseTexture, false))
                        {
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        terrainLayers.Add(splat.TerrainLayer);
                    }
                }
            }

            terrain.terrainData.terrainLayers = terrainLayers.ToArray();
        }

        private static void RemoveTerrainLayerAssetFiles(string terrainName)
        {
            string megaWorldDirectory = "";
            string terrainLayerDirectory = megaWorldDirectory + "Profiles/TerrainLayers";
            DirectoryInfo info = new DirectoryInfo(terrainLayerDirectory);

            if (info.Exists)
            {
                FileInfo[] fileInfo = info.GetFiles(terrainName + "*.asset");

                for (int i = 0; i < fileInfo.Length; i++)
                {
                    File.Delete(fileInfo[i].FullName);
                }
            }
        }

        public static void SyncTerrainID(Terrain terrain, Group group)
        {
            List<DetailPrototype> terrainDetails = new List<DetailPrototype>(terrain.terrainData.detailPrototypes);

            foreach (PrototypeTerrainDetail proto in group.ProtoTerrainDetailList)
            {
                TerrainDetailSettings terrainDetailSettings = (TerrainDetailSettings)proto.GetSettings(typeof(TerrainDetailSettings));

                for (int Id = 0; Id < terrainDetails.Count; Id++)
                {
                    if (terrainDetails[Id].renderMode == terrainDetailSettings.RenderMode)
                    {
                        if (CommonUtility.IsSameTexture(terrainDetails[Id].prototypeTexture, proto.DetailTexture, false))
                        {
                            proto.TerrainProtoId = Id;
                        }
                        if (CommonUtility.IsSameGameObject(terrainDetails[Id].prototype, proto.Prefab, false))
                        {
                            proto.TerrainProtoId = Id;
                        }
                    }
                }
            }
        }

        public static void UpdateOnlyTerrainTexture(Terrain terrain, Group group)
        {
            List<PrototypeTerrainTexture> protoTerrainTextureRemoveList = new List<PrototypeTerrainTexture>();

            List<TerrainLayer> terrainLayers = new List<TerrainLayer>(terrain.terrainData.terrainLayers);

            foreach (PrototypeTerrainTexture texture in group.ProtoTerrainTextureList)
            {
                bool find = false;

                for (int Id = 0; Id < terrainLayers.Count; Id++)
                {
                    if (CommonUtility.IsSameTexture(terrainLayers[Id].diffuseTexture, texture.TerrainTextureSettings.DiffuseTexture, false))
                    {
                        find = true;
                    }
                }

                if(find == false)
                {
                    protoTerrainTextureRemoveList.Add(texture);
                }
            }

            foreach (PrototypeTerrainTexture proto in protoTerrainTextureRemoveList)
            {
                group.ProtoTerrainTextureList.Remove(proto);
            }

            for (int Id = 0; Id < terrainLayers.Count; Id++)
            {
                bool find = false;

                foreach (PrototypeTerrainTexture texture in group.ProtoTerrainTextureList)
                {
                    if (CommonUtility.IsSameTexture(terrainLayers[Id].diffuseTexture, texture.TerrainTextureSettings.DiffuseTexture, false))
                    {
                        find = true;
                    }
                }

                if(find == false)
                {
                    group.ProtoTerrainTextureList.Add(PrototypeTerrainTexture.Create(group, terrainLayers[Id]));
                }
            }

            SyncTerrainID(terrain, group);
        }

        public static void UpdateOnlyTerrainDetail(Terrain terrain, Group group)
        {
            List<PrototypeTerrainDetail> protoTerrainDetailRemoveList = new List<PrototypeTerrainDetail>();

            List<DetailPrototype> terrainDetails = new List<DetailPrototype>(terrain.terrainData.detailPrototypes);

            foreach (PrototypeTerrainDetail proto in group.ProtoTerrainDetailList)
            {
                TerrainDetailSettings terrainDetailSettings = (TerrainDetailSettings)proto.GetSettings(typeof(TerrainDetailSettings));
                
                bool find = false;

                for (int Id = 0; Id < terrainDetails.Count; Id++)
                {
                    if (terrainDetails[Id].renderMode == terrainDetailSettings.RenderMode)
                    {
                        if (CommonUtility.IsSameTexture(terrainDetails[Id].prototypeTexture, proto.DetailTexture, false))
                        {
                            find = true;
                        }
                        if (CommonUtility.IsSameGameObject(terrainDetails[Id].prototype, proto.Prefab, false))
                        {
                            find = true;
                        }
                    }
                }

                if(find == false)
                {
                    protoTerrainDetailRemoveList.Add(proto);
                }
            }

            foreach (PrototypeTerrainDetail proto in protoTerrainDetailRemoveList)
            {
                group.ProtoTerrainDetailList.Remove(proto);
            }

            DetailPrototype unityProto;
            PrototypeTerrainDetail localProto;

            for (int Id = 0; Id < terrainDetails.Count; Id++)
            {
                bool find = false;

                foreach (PrototypeTerrainDetail proto in group.ProtoTerrainDetailList)
                {
                    TerrainDetailSettings terrainDetailSettings = (TerrainDetailSettings)proto.GetSettings(typeof(TerrainDetailSettings));

                    if (terrainDetails[Id].renderMode == terrainDetailSettings.RenderMode)
                    {
                        if (CommonUtility.IsSameTexture(terrainDetails[Id].prototypeTexture, proto.DetailTexture, false))
                        {
                            find = true;
                        }
                        if (CommonUtility.IsSameGameObject(terrainDetails[Id].prototype, proto.Prefab, false))
                        {
                            find = true;
                        }
                    }
                }

                if(find == false)
                {
                    unityProto = terrain.terrainData.detailPrototypes[Id];
                    
                    if (unityProto.prototype != null)
                    {
                        localProto = PrototypeTerrainDetail.Create(group, unityProto.prototype);
                        localProto.PrefabType = PrefabType.Mesh;
                    }
                    else
                    {
                        localProto = PrototypeTerrainDetail.Create(group, unityProto.prototypeTexture);
                        localProto.PrefabType = PrefabType.Texture;
                    }

                    TerrainDetailSettings terrainDetailSettings = (TerrainDetailSettings)localProto.GetSettings(typeof(TerrainDetailSettings));

                    terrainDetailSettings.RenderMode = unityProto.renderMode;
                    terrainDetailSettings.DryColour = unityProto.dryColor;
                    terrainDetailSettings.HealthyColour = unityProto.healthyColor;
                    terrainDetailSettings.MaxHeight = unityProto.maxHeight;
                    terrainDetailSettings.MaxWidth = unityProto.maxWidth;
                    terrainDetailSettings.MinHeight = unityProto.minHeight;
                    terrainDetailSettings.MinWidth = unityProto.minWidth;
                    terrainDetailSettings.NoiseSpread = unityProto.noiseSpread;
    
                    group.ProtoTerrainDetailList.Add(localProto);  
                }
            }

            SyncTerrainID(terrain, group);
        }

        public static void UpdatePrototypesFromTerrain(Terrain terrain, Group group)
        {
            if (terrain == null)
            {
                Debug.LogWarning("Missing active terrain.");
                return;
            }

            switch (group.ResourceType)
            {
                case ResourceType.TerrainTexture:
                {
                    UpdateOnlyTerrainTexture(terrain, group);
                    break;
                }
                case ResourceType.TerrainDetail:
                {
                    UpdateOnlyTerrainDetail(terrain, group);
                    break;
                }
            }

            SyncTerrainID(terrain, group);
        }

        public static void DetectSyncError(Group group, Terrain terrain)
        {
            if(group.GetPrototypes().Count == 0)
            {
                SyncError = TerrainResourcesSyncError.MissingPrototypes;
                return;
            }

            switch (group.ResourceType)
            {
                case ResourceType.TerrainDetail:
                {
                    for (int i = 0; i < group.ProtoTerrainDetailList.Count; i++)
                    {
                        bool find = false;

                        for (int Id = 0; Id < terrain.terrainData.detailPrototypes.Length; Id++)
                        {
                            TerrainDetailSettings terrainDetailSettings = (TerrainDetailSettings)group.ProtoTerrainDetailList[i].GetSettings(typeof(TerrainDetailSettings));

                            if (terrain.terrainData.detailPrototypes[Id].renderMode == terrainDetailSettings.RenderMode)
                            {
                                if (CommonUtility.IsSameTexture(terrain.terrainData.detailPrototypes[Id].prototypeTexture, group.ProtoTerrainDetailList[i].DetailTexture, false))
                                {
                                    SyncError = TerrainResourcesSyncError.None;
                                    return;
                                }
                                if (CommonUtility.IsSameGameObject(terrain.terrainData.detailPrototypes[Id].prototype, group.ProtoTerrainDetailList[i].Prefab, false))
                                {
                                    SyncError = TerrainResourcesSyncError.None;
                                    return;
                                }
                            }
                        }

                        if(find == false)
                        {
                            SyncError = TerrainResourcesSyncError.NotAllProtoAvailable;
                        }
                    }

                    break;
                }
                case ResourceType.TerrainTexture:
                {
                    for (int i = 0; i < group.ProtoTerrainTextureList.Count; i++)
                    {
                        bool find = false;

                        for (int Id = 0; Id < terrain.terrainData.terrainLayers.Length; Id++)
                        {
                            if (CommonUtility.IsSameTexture(terrain.terrainData.terrainLayers[Id].diffuseTexture, group.ProtoTerrainTextureList[i].TerrainTextureSettings.DiffuseTexture, false))
                            {
                                SyncError = TerrainResourcesSyncError.None;
                                return;
                            }
                        }

                        if(find == false)
                        {
                            SyncError = TerrainResourcesSyncError.NotAllProtoAvailable;
                            return;
                        }
                    }

                    break;
                }
            }

            SyncError = TerrainResourcesSyncError.NotAllProtoAvailable;
        }

        public static bool IsSyncError(Group group, Terrain terrain)
        {
            DetectSyncError(group, terrain);

            if(TerrainResourcesSyncError.NotAllProtoAvailable == SyncError)
            {
                switch (group.ResourceType)
                {
			        case ResourceType.TerrainDetail:
                	{
                        Debug.LogWarning("You need all Terrain Details prototypes to be in the terrain. Click \"Add Missing Resources To Terrain\"");
                        return false;
			        }
			        case ResourceType.TerrainTexture:
                	{
                        Debug.LogWarning("You need all Terrain Textures prototypes to be in the terrain. Click \"Add Missing Resources To Terrain\"");
                        return false;
			        }
			    }
            }

            return true;
        }

        public static void SyncAllTerrains(Group group, Terrain terrain)
        {
            if(group == null || terrain == null)
            {
                return;
            }

            if(Terrain.activeTerrains.Length == 0)
            {
                return;
            }

            switch (group.ResourceType)
            {
                case ResourceType.TerrainDetail:
                {
                    List<DetailPrototype> terrainDetails = new List<DetailPrototype>(terrain.terrainData.detailPrototypes);

                    foreach (Terrain item in Terrain.activeTerrains)
                    {
                        item.terrainData.detailPrototypes = terrainDetails.ToArray();
                    }

                    break;
                }
                case ResourceType.TerrainTexture:
                {
                    List<TerrainLayer> terrainTextures = new List<TerrainLayer>(terrain.terrainData.terrainLayers);

                    foreach (Terrain item in Terrain.activeTerrains)
                    {
                        item.terrainData.terrainLayers = terrainTextures.ToArray();
                    }

                    break;
                }
            }
        }
    }
}