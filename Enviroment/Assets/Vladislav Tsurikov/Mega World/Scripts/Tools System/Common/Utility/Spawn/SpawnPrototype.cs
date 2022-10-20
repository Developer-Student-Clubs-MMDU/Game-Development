using UnityEngine;
#if UNITY_EDITOR
using VladislavTsurikov.UndoSystem;
#endif
#if UNITY_2021_2_OR_NEWER
using UnityEngine.TerrainTools;
#else
using UnityEngine.Experimental.TerrainAPI;
#endif
using VladislavTsurikov.RaycastEditorSystem;

namespace VladislavTsurikov.MegaWorldSystem
{
    public static class SpawnPrototype 
    {
        public static void SpawnInstantItem(Group group, PrototypeInstantItem proto, AreaVariables areaVariables, RayHit rayHit, float fitness)
        {
#if INSTANT_RENDERER
            OverlapCheckSettings overlapCheckSettings = (OverlapCheckSettings)proto.GetSettings(typeof(OverlapCheckSettings));

            InstanceData instanceData = new InstanceData(rayHit.Point, proto.Prefab.transform.lossyScale, proto.Prefab.transform.rotation);

            TransformComponentSettings transformComponentSettings = (TransformComponentSettings)proto.GetSettings(typeof(TransformComponentSettings));
            transformComponentSettings.Stack.SetInstanceData(ref instanceData, fitness, rayHit.Normal);

            if(OverlapCheckSettings.RunOverlapCheck(ResourceType.InstantItem, overlapCheckSettings, proto.Extents, instanceData))
            {
                InstantRendererController.AddItemToStorageTerrainCells(proto.ID, instanceData);
            }
#endif
        }

        public static void SpawnGameObject(Group group, PrototypeGameObject proto, AreaVariables areaVariables, RayHit rayHit, float fitness, bool registerUndo = true)
        {
            OverlapCheckSettings overlapCheckSettings = (OverlapCheckSettings)proto.GetSettings(typeof(OverlapCheckSettings));

            InstanceData instanceData = new InstanceData(rayHit.Point, proto.Prefab.transform.lossyScale, proto.Prefab.transform.rotation);

            TransformComponentSettings transformComponentSettings = (TransformComponentSettings)proto.GetSettings(typeof(TransformComponentSettings));
            transformComponentSettings.Stack.SetInstanceData(ref instanceData, fitness, rayHit.Normal);

            if(OverlapCheckSettings.RunOverlapCheck(ResourceType.GameObject, overlapCheckSettings, proto.Extents, instanceData))
            {
                PlacedObject objectInfo = PlacedObjectUtility.PlaceObject(proto.Prefab, instanceData.Position, instanceData.Scale, instanceData.Rotation);
                PlacedObjectUtility.ParentGameObject(group, objectInfo);
                MegaWorldPath.GameObjectStoragePackage.Storage.AddInstance(proto.ID, objectInfo.GameObject);

#if UNITY_EDITOR
                RaycastEditor.RegisterGameObject(objectInfo.GameObject);  
                
                if(registerUndo)
                {
                    UndoSystem.Undo.RegisterUndoAfterMouseUp(new CreatedGameObject(objectInfo.GameObject));
                }
#endif
                objectInfo.GameObject.transform.hasChanged = false;
                
            }
        }

        public static void SpawnTerrainDetails(PrototypeTerrainDetail proto, AreaVariables areaVariables, Terrain terrain)
        {
            Vector2Int spawnSize;
	        Vector2Int position, startPosition;
            
            spawnSize = new Vector2Int(
					CommonUtility.WorldToDetail(areaVariables.Size * areaVariables.SizeMultiplier, terrain.terrainData.size.x, terrain.terrainData),
					CommonUtility.WorldToDetail(areaVariables.Size * areaVariables.SizeMultiplier, terrain.terrainData.size.z, terrain.terrainData));
            
            Vector2Int halfBrushSize = spawnSize / 2;
            
            Vector2Int center = new Vector2Int(
                CommonUtility.WorldToDetail(areaVariables.RayHit.Point.x - terrain.transform.position.x, terrain.terrainData),
                CommonUtility.WorldToDetail(areaVariables.RayHit.Point.z - terrain.transform.position.z, terrain.terrainData.size.z, terrain.terrainData));
            
            position = center - halfBrushSize;
            startPosition = Vector2Int.Max(position, Vector2Int.zero);
                
            Vector2Int offset = startPosition - position;
            
            Vector2Int current;
            float fitness = 1;
            float detailmapResolution = terrain.terrainData.detailResolution;
            int x, y;
        
            int[,] localData = terrain.terrainData.GetDetailLayer(
                startPosition.x, startPosition.y,
                Mathf.Max(0, Mathf.Min(position.x + spawnSize.x, (int)detailmapResolution) - startPosition.x),
                Mathf.Max(0, Mathf.Min(position.y + spawnSize.y, (int)detailmapResolution) - startPosition.y), proto.TerrainProtoId);
        
            float widthY = localData.GetLength(1);
            float heightX = localData.GetLength(0);

            MaskFilterSettings maskFilterSettings = (MaskFilterSettings)proto.GetSettings(typeof(MaskFilterSettings));
            SpawnDetailSettings spawnDetailSettings = (SpawnDetailSettings)proto.GetSettings(typeof(SpawnDetailSettings));
                   
            for (y = 0; y < widthY; y++)
            {
                for (x = 0; x < heightX; x++)
                {
                    current = new Vector2Int(y, x);
        
                    Vector2 normal = current + startPosition; 
                    normal /= detailmapResolution;
        
                    Vector2 worldPostion = CommonUtility.GetTerrainWorldPositionFromRange(normal, terrain);

                    if (maskFilterSettings.Stack.Settings.Count > 0)
			        {
                        fitness = GrayscaleFromTexture.GetFromWorldPosition(areaVariables.Bounds, new Vector3(worldPostion.x, 0, worldPostion.y), maskFilterSettings.FilterMaskTexture2D);
                    }
                    
                    float maskFitness = BrushJitterSettings.GetAlpha(areaVariables, current + offset, spawnSize);
        
                    int targetStrength = 0;
        
                    if (spawnDetailSettings.UseRandomOpacity) 
                    {
                        float randomFitness = fitness;
                        randomFitness *= UnityEngine.Random.value;
        
                        targetStrength = Mathf.RoundToInt(Mathf.Lerp(0, spawnDetailSettings.Density, randomFitness));
                    }
                    else
                    {
                        targetStrength = Mathf.RoundToInt(Mathf.Lerp(0, spawnDetailSettings.Density, fitness));
                    }
        
                    targetStrength = Mathf.RoundToInt(Mathf.Lerp(localData[x, y], targetStrength, maskFitness));

                    if (UnityEngine.Random.Range(0f, 1f) < (1 - fitness) || UnityEngine.Random.Range(0f, 1f) < spawnDetailSettings.FailureRate / 100)
                    {
                        targetStrength = 0;
                    }

                    if (UnityEngine.Random.Range(0f, 1f) < (1 - maskFitness))
                    {
                        targetStrength = localData[x, y];
                    }
        
                    localData[x, y] = targetStrength;
                }
            }
        
            terrain.terrainData.SetDetailLayer(startPosition.x, startPosition.y, proto.TerrainProtoId, localData);
        }

        public static void SpawnTexture(PrototypeTerrainTexture proto, TerrainPainterRenderHelper terrainPainterRenderHelper, float textureTargetStrength)
        {
            MaskFilterSettings maskFilterSettings = (MaskFilterSettings)proto.GetSettings(typeof(MaskFilterSettings));

            PaintContext paintContext = terrainPainterRenderHelper.AcquireTexture(proto.TerrainLayer);

			if (paintContext != null)
			{
                FilterMaskOperation.UpdateFilterContext(ref maskFilterSettings.FilterContext, maskFilterSettings.Stack, terrainPainterRenderHelper.AreaVariables);

                RenderTexture filterMaskRT = maskFilterSettings.FilterContext.GetFilterMaskRT();

				Material mat = MaskFilterUtility.GetPaintMaterial();

                // apply brush
                float targetAlpha = textureTargetStrength;
                float s = 1;
				Vector4 brushParams = new Vector4(s, targetAlpha, 0.0f, 0.0f);
				mat.SetTexture("_BrushTex", terrainPainterRenderHelper.AreaVariables.Mask);
				mat.SetVector("_BrushParams", brushParams);
				mat.SetTexture("_FilterTex", filterMaskRT);
                mat.SetTexture("_SourceAlphamapRenderTextureTex", paintContext.sourceRenderTexture);

                terrainPainterRenderHelper.SetupTerrainToolMaterialProperties(paintContext, mat);

                terrainPainterRenderHelper.RenderBrush(paintContext, mat, 0);

                TerrainPaintUtility.EndPaintTexture(paintContext, "Terrain Paint - Texture");

                if(maskFilterSettings.FilterContext != null)
                {
                    maskFilterSettings.FilterContext.DisposeUnmanagedMemory();
                }

                TerrainPaintUtility.ReleaseContextResources(paintContext);
			}
        }
    }
}