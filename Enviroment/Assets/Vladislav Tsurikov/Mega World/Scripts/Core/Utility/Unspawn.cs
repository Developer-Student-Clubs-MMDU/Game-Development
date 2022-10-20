using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if INSTANT_RENDERER
using VladislavTsurikov.InstantRendererSystem;
#endif

namespace VladislavTsurikov.MegaWorldSystem
{
    public static class Unspawn 
    {
        public static void UnspawnAllProto(List<Group> groupList, bool unspawnSelected)
        {
            foreach (Group group in groupList)
            {
                UnspawnTerrainDetail(group.ProtoTerrainDetailList, unspawnSelected);
                UnspawnInstantItem(group, unspawnSelected);
                UnspawnGameObject(group, unspawnSelected);
            }
        }

        public static void UnspawnGameObject(Group group, bool unspawnSelected)
        {
            GameObject[] allGameObjects = GameObject.FindObjectsOfType<GameObject>();

            for (int index = 0; index < allGameObjects.Length; index++)
            {
                PrototypeGameObject proto = GetPrototype.GetCurrentPrototypeGameObject(group, allGameObjects[index]);

                if(proto != null)
                {
                    if(unspawnSelected)
                    {
                        if(proto.Selected == false)
                        {
                            continue;
                        }
                    }

                    UnityEngine.Object.DestroyImmediate(allGameObjects[index]);
                }
            }

            MegaWorldPath.GameObjectStoragePackage.Storage.RemoveNullData(false);
        }

        public static void UnspawnTerrainDetail(List<PrototypeTerrainDetail> protoTerrainDetailList, bool unspawnSelected)
        {
            for (int i = 0; i < protoTerrainDetailList.Count; i++)
            {
                if(unspawnSelected)
                {
                    if(protoTerrainDetailList[i].Selected == false)
                    {
                        continue;
                    }
                }

                foreach (var terrain in Terrain.activeTerrains)
                {
                    if(terrain.terrainData.detailResolution == 0)
                    {
                        continue;
                    }

                    if(protoTerrainDetailList[i].TerrainProtoId > terrain.terrainData.detailPrototypes.Length - 1)
                    {
                        Debug.LogWarning("You need all Terrain Details prototypes to be in the terrain. Click \"Add Missing Resources To Terrain\"");
                    }
                    else
                    {
                        terrain.terrainData.SetDetailLayer(0, 0, protoTerrainDetailList[i].TerrainProtoId, new int[terrain.terrainData.detailWidth, terrain.terrainData.detailWidth]);
                    }
                }
            }
        }

        public static void UnspawnInstantItem(Group group, bool unspawnSelected)
        {
#if INSTANT_RENDERER
            for (int index = 0; index < group.ProtoInstantItemList.Count; index++)
            {
                PrototypeInstantItem proto = group.ProtoInstantItemList[index];

                if(unspawnSelected)
                {
                    if(proto.Selected == false)
                    {
                        continue;
                    }
                }

                StorageTerrainCellsAPI.RemoveItemInstances(InstantRendererController.StorageTerrainCells, proto.ID);
            }
#endif
        }
    }
}