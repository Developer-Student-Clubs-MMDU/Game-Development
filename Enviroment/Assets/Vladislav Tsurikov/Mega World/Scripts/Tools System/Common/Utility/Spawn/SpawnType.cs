using UnityEngine;
using System.Collections.Generic;

namespace VladislavTsurikov.MegaWorldSystem
{
    public static class SpawnType 
    {
        public static void SpawnGameObject(Group group, AreaVariables areaVariables, bool registerUndo = true)
        {
            if(group.FilterType == FilterType.MaskFilter)
            {
                UpdateFilterMask.UpdateFilterMaskTexture(group, areaVariables);
            }

            ScatterSettings scatterSettings = (ScatterSettings)group.GetSettings(typeof(ScatterSettings));
            LayerSettings layerSettings = MegaWorldPath.CommonDataPackage.layerSettings;

            foreach (Vector2 sample in scatterSettings.Stack.Samples(areaVariables))
            {
                RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(new Vector3(sample.x, areaVariables.RayHit.Point.y, sample.y)), layerSettings.GetCurrentPaintLayers(group.ResourceType));
                if(rayHit != null)
                {
                    PrototypeGameObject proto = GetRandomPrototype.GetMaxSuccessProtoGameObject(group.ProtoGameObjectList);

                    if(proto == null || proto.Active == false)
                    {
                        continue;
                    }

                    float fitness = GetFitnessUtility.GetFitness(group, areaVariables.Bounds, rayHit);

                    float maskFitness = GrayscaleFromTexture.GetFromWorldPosition(areaVariables.Bounds, rayHit.Point, areaVariables.Mask);

                    fitness *= maskFitness;

                    if(fitness != 0)
                    {
                        if (UnityEngine.Random.Range(0f, 1f) < (1 - fitness))
                        {
                            continue;
                        }

                        SpawnPrototype.SpawnGameObject(group, proto, areaVariables, rayHit, fitness, registerUndo);
                    }
                }
            }
        }

        public static void SpawnInstantItem(Group group, AreaVariables areaVariables)
        {            
#if INSTANT_RENDERER
            if(InstantRendererController.IsSyncError(group) == false)
            {
                return;
            }

            if(group.FilterType == FilterType.MaskFilter)
            {
                UpdateFilterMask.UpdateFilterMaskTexture(group, areaVariables);
            }

            ScatterSettings scatterSettings = (ScatterSettings)group.GetSettings(typeof(ScatterSettings));
            LayerSettings layerSettings = MegaWorldPath.CommonDataPackage.layerSettings;

            foreach (Vector2 sample in scatterSettings.Stack.Samples(areaVariables))
            {
                RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(new Vector3(sample.x, areaVariables.RayHit.Point.y, sample.y)), layerSettings.GetCurrentPaintLayers(group.ResourceType));
                if(rayHit != null)
                {
                    PrototypeInstantItem proto = GetRandomPrototype.GetMaxSuccessProtoInstantItem(group.ProtoInstantItemList);

                    if(proto == null || proto.Active == false)
                    {
                        continue;
                    }

                    float fitness = GetFitnessUtility.GetFitness(group, areaVariables.Bounds, rayHit);

                    float maskFitness = GrayscaleFromTexture.GetFromWorldPosition(areaVariables.Bounds, rayHit.Point, areaVariables.Mask);

                    fitness *= maskFitness;

                    if(fitness != 0)
                    {
                        if (UnityEngine.Random.Range(0f, 1f) < (1 - fitness))
                        {
                            continue;
                        }

                        SpawnPrototype.SpawnInstantItem(group, proto, areaVariables, rayHit, fitness);
                    }
                }
            }
#endif
        }

        public static void SpawnTerrainDetails(Group group, List<PrototypeTerrainDetail> protoTerrainDetailList, AreaVariables areaVariables)
        {
            if(TerrainResourcesController.IsSyncError(group, Terrain.activeTerrain) == false)
            {
                return;
            }

            UpdateFilterMask.UpdateFilterMaskTextureForAllTerrainDetail(protoTerrainDetailList, areaVariables);            

            foreach (Terrain terrain in Terrain.activeTerrains)
            {
                Bounds terrainBounds = terrain.terrainData.bounds;
                terrainBounds.center = new Vector3(terrain.transform.position.x + terrainBounds.extents.x, terrain.terrainData.bounds.center.y, terrain.transform.position.z + terrainBounds.extents.z);

                if(terrainBounds.Intersects(areaVariables.Bounds))
                {
                    if(terrain.terrainData.detailPrototypes.Length == 0)
                    {
                        Debug.LogWarning("Add Terrain Details");
                        return;
                    }
        
                    foreach (PrototypeTerrainDetail proto in protoTerrainDetailList)
                    {
                        if(proto.Active == false)
                        {
                            continue;
                        }
                        
                        SpawnPrototype.SpawnTerrainDetails(proto, areaVariables, terrain);
                    }
                }
            }
        }

        public static void SpawnTerrainTexture(Group group, List<PrototypeTerrainTexture> prototypeTerrainTextures, AreaVariables areaVariables, float textureTargetStrength)
        {
            if(TerrainResourcesController.IsSyncError(group, Terrain.activeTerrain) == false)
            {
                return;
            }

            if(areaVariables.TerrainUnderCursor == null)
            {
                return;
            }

            TerrainPainterRenderHelper terrainPainterRenderHelper = new TerrainPainterRenderHelper(areaVariables);

            foreach (PrototypeTerrainTexture proto in prototypeTerrainTextures)
            {
                if(proto.Active)
                {
                    SpawnPrototype.SpawnTexture(proto, terrainPainterRenderHelper, textureTargetStrength);
                }
            }
        }
    }
}