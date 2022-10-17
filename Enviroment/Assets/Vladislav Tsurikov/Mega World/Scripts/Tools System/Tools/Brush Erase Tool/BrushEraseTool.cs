#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using VladislavTsurikov.UndoSystem;
using System;
#if INSTANT_RENDERER
using VladislavTsurikov.InstantRendererSystem;
#endif

namespace VladislavTsurikov.MegaWorldSystem.BrushErase
{
    [Tool(true, true, new ResourceType[]{ResourceType.InstantItem, ResourceType.GameObject, ResourceType.TerrainDetail})]    
    public class BrushEraseTool : ToolComponent
    {
        private int _editorHash = "Editor".GetHashCode();

        private MouseMoveData _mouseMoveData = new MouseMoveData();

        public override void AddPrototypeToolSettingsTypes()
        {
            foreach (ResourceType resourceType in typeof(ResourceType).GetEnumValues())
            {
                switch (resourceType)
                {
                    case ResourceType.InstantItem:
                    {
                        List<System.Type> settingsTypes = new List<System.Type>();
                        settingsTypes.Add(typeof(AdditionalEraseSettings));

                        SettingsTypesStack.AddPrototypeToolSettingsTypes(ResourceType.InstantItem, this.GetType(), settingsTypes);

                        break;
                    }
                    case ResourceType.GameObject:
                    {
                        List<System.Type> settingsTypes = new List<System.Type>();
                        settingsTypes.Add(typeof(AdditionalEraseSettings));

                        SettingsTypesStack.AddPrototypeToolSettingsTypes(ResourceType.GameObject, this.GetType(), settingsTypes);

                        break;
                    }
                    case ResourceType.TerrainDetail:
                    {
                        List<System.Type> settingsTypes = new List<System.Type>();
                        settingsTypes.Add(typeof(AdditionalEraseSettings));

                        SettingsTypesStack.AddPrototypeToolSettingsTypes(ResourceType.TerrainDetail, this.GetType(), settingsTypes);

                        break;
                    }
                }
            }
        }

        public override void AddGroupToolSettingsTypes()
        {
            List<System.Type> settingsTypes = new List<System.Type>();
            settingsTypes.Add(typeof(SimpleFilterSettings));
            settingsTypes.Add(typeof(MaskFilterSettings));

            SettingsTypesStack.AddGroupToolSettingsTypes(this.GetType(), settingsTypes);
        }
        
        public override void DoTool()
        {
            BrushSettings brush = BrushEraseToolPath.Settings.BrushSettingsForErase;
            
            int controlID = GUIUtility.GetControlID(_editorHash, FocusType.Passive);
            Event e = Event.current;
            EventType eventType = e.GetTypeForControl(controlID);

            switch (eventType)
            {
                case EventType.MouseDown:
                {
                    if (e.button != 0 || Event.current.alt)
                    {
                        return;
                    }

                    if(_mouseMoveData.UpdatePosition() == false)
                    {
                        return;
                    }

                    if(_mouseMoveData.Raycast != null)
                    {
                        foreach (Group group in MegaWorldPath.DataPackage.SelectedVariables.SelectedGroupList)
                        {
                            AreaVariables areaVariables = brush.BrushJitterSettings.GetAreaVariables(brush, _mouseMoveData.Raycast.Point, group);

                            if(areaVariables.RayHit != null)
                            {
                                EraseType(group, areaVariables);
                            }
                        }
                    }
                    
                    _mouseMoveData.DragDistance = 0;
                    _mouseMoveData.PrevRaycast = _mouseMoveData.Raycast;

                    break;
                }
                case EventType.MouseDrag:
                {
                    if (e.button != 0 || Event.current.alt)
                    {
                        return;
                    }

                    if(_mouseMoveData.UpdatePosition() == false)
                    {
                        return;
                    }

                    _mouseMoveData.DragMouse(brush.GetCurrentSpacing(), (dragPoint) =>
                    {
                        foreach (Group group in MegaWorldPath.DataPackage.SelectedVariables.SelectedGroupList)
                        {
                            LayerSettings layerSettings = MegaWorldPath.CommonDataPackage.layerSettings;

                            RayHit originalRaycastInfo = RaycastUtility.Raycast(RayUtility.GetRayDown(dragPoint), layerSettings.GetCurrentPaintLayers(group.ResourceType));

                            if(originalRaycastInfo != null)
                            {
                                AreaVariables areaVariables = brush.GetAreaVariables(originalRaycastInfo);

                                if(areaVariables.RayHit != null)
                                {
                                    EraseType(group, areaVariables);
                                }
                            }
                        }
                        
                        return true;
                    });

                    e.Use();

                    break;
                }
                case EventType.MouseMove:
                {
                    if(_mouseMoveData.UpdatePosition() == false)
                    {
                        return;
                    }

                    e.Use();
                    break;
                }
                case EventType.Repaint:
                {           
	                if(_mouseMoveData.Raycast == null)
                    {
                        return;
                    }

                    AreaVariables areaVariables = brush.GetAreaVariables(_mouseMoveData.Raycast);
                    BrushEraseToolVisualisation.Draw(areaVariables);

                    break;
                }
                case EventType.Layout:
                {           
                    HandleUtility.AddDefaultControl(controlID);

                    break;
                }
                case EventType.KeyDown:
                {
                    switch (e.keyCode)
                    {
                        case KeyCode.F:
                        {
                            if (MegaWorldGUIUtility.IsModifierDown(EventModifiers.None) && _mouseMoveData.Raycast != null)
                            {
                                SceneView.lastActiveSceneView.LookAt(_mouseMoveData.Raycast.Point, SceneView.lastActiveSceneView.rotation, brush.BrushSize);
                                e.Use();
                            }
                        }

                        break;
                    }
                    break;
                }
            }
        }

        public override void SaveSettings()
        {
            BrushEraseToolPath.Settings.Save();
        }

        public void EraseType(Group group, AreaVariables areaVariables)
        {
            switch (group.ResourceType)
            {
                case ResourceType.InstantItem:
                {
                    BrushEraseInstantItem(group, areaVariables);

                    break;
                }
                case ResourceType.GameObject:
                {
                    BrushEraseGameObject(group, areaVariables);

                    break;
                }
                case ResourceType.TerrainDetail:
                {
                    EraseTerrainDetails(group, areaVariables);

                    break;
                }
            }                                   
        }

        public void BrushEraseInstantItem(Group group, AreaVariables areaVariables)
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

            Rect positionRect = RectExtension.CreateRectFromBounds(areaVariables.Bounds);

            List<Cell> overlapCellList = new List<Cell>();                 
            InstantRendererController.InstantRenderer.StorageTerrainCells.CellQuadTree.Query(positionRect, overlapCellList);

            LayerSettings layerSettings = MegaWorldPath.CommonDataPackage.layerSettings;
            MaskFilterSettings maskFilterSettings = (MaskFilterSettings)group.GetSettings(typeof(BrushEraseTool), typeof(MaskFilterSettings));
            SimpleFilterSettings simpleFilterSettings = (SimpleFilterSettings)group.GetSettings(typeof(BrushEraseTool), typeof(SimpleFilterSettings));

            for (int i = 0; i <= overlapCellList.Count - 1; i++)
            {
                int cellIndex = overlapCellList[i].Index;

                InstantRendererController.InstantRenderer.StorageTerrainCells.CellModifier.AddModifiedСell(overlapCellList[i], false, true);

                List<ItemInfo> persistentInfoList = InstantRendererController.InstantRenderer.StorageTerrainCells.PersistentStoragePackage.CellList[cellIndex].ItemInfoList;
                
                for (int persistentInfoIndex = 0; persistentInfoIndex < persistentInfoList.Count; persistentInfoIndex++)
                {
                    ItemInfo persistentInfo = persistentInfoList[persistentInfoIndex];

                    List<VladislavTsurikov.InstantRendererSystem.InstanceData> persistentItemForDestroy = new List<VladislavTsurikov.InstantRendererSystem.InstanceData>();

                    PrototypeInstantItem proto = GetPrototype.GetCurrentInstantItem(persistentInfo.ID, true);

                    if(proto == null || proto.Active == false || proto.Selected == false)
                    {
                        continue;
                    }

                    AdditionalEraseSettings additionalEraseSettings = (AdditionalEraseSettings)proto.GetSettings(typeof(BrushEraseTool), typeof(AdditionalEraseSettings));

                    for (int itemIndex = 0; itemIndex < persistentInfo.InstanceDataList.Count; itemIndex++)
                    {
                        VladislavTsurikov.InstantRendererSystem.InstanceData persistentItem = persistentInfo.InstanceDataList[itemIndex];

                        if(areaVariables.Bounds.Contains(persistentInfo.InstanceDataList[itemIndex].Position) == true)
                        {
                            float fitness = 1;

                            if(group.FilterType == FilterType.SimpleFilter)
                            {
                                RaycastHit hit;
                                if (Physics.Raycast(RayUtility.GetRayFromCameraPosition(persistentItem.Position), out hit, MegaWorldPath.AdvancedSettings.EditorSettings.raycastSettings.MaxRayDistance, 
                                    layerSettings.GetCurrentPaintLayers(group.ResourceType)))
		                        {
                                    fitness = simpleFilterSettings.GetFitness(hit.point, hit.normal);
                                }
                            }
                            else
                            {
                                if(maskFilterSettings.Stack.Settings.Count != 0)
                                {
                                    fitness = GrayscaleFromTexture.GetFromWorldPosition(areaVariables.Bounds, persistentItem.Position, maskFilterSettings.FilterMaskTexture2D);
                                }
                            }

                            float maskFitness = GrayscaleFromTexture.GetFromWorldPosition(areaVariables.Bounds, persistentItem.Position, areaVariables.Mask);

                            fitness *= maskFitness;

                            fitness *= BrushEraseToolPath.Settings.EraseStrength;

                            float successOfErase = UnityEngine.Random.Range(0.0f, 1.0f);

                            if(successOfErase < fitness)
                            {
                                float randomSuccessForErase = UnityEngine.Random.Range(0.0f, 1.0f);

                                if(randomSuccessForErase < additionalEraseSettings.SuccessForErase / 100)
                                {
                                    persistentItemForDestroy.Add(persistentItem);
                                    InstantRendererController.InstantRenderer.StorageTerrainCells.SavePersistentStoragePackage = true;
                                }
                            }
                        } 
                    }

                    foreach (VladislavTsurikov.InstantRendererSystem.InstanceData item in persistentItemForDestroy)
                    {
                        persistentInfo.InstanceDataList.Remove(item);
                    }
                }
            }
#endif
        }

        public void BrushEraseGameObject(Group group, AreaVariables areaVariables)
        {
            if(group.FilterType == FilterType.MaskFilter)
            {
                UpdateFilterMask.UpdateFilterMaskTexture(group, areaVariables);
            }

            LayerSettings layerSettings = MegaWorldPath.CommonDataPackage.layerSettings;
            MaskFilterSettings maskFilterSettings = (MaskFilterSettings)group.GetSettings(typeof(BrushEraseTool), typeof(MaskFilterSettings));
            SimpleFilterSettings simpleFilterSettings = (SimpleFilterSettings)group.GetSettings(typeof(BrushEraseTool), typeof(SimpleFilterSettings));

            MegaWorldPath.GameObjectStoragePackage.Storage.IntersectBounds(areaVariables.Bounds, (gameObjectInfo, cellIndex) =>
            {
                PrototypeGameObject proto = GetPrototype.GetCurrentPrototypeGameObject(gameObjectInfo.ID, true);

                if(proto == null || proto.Active == false || proto.Selected == false)
                {
                    return true;
                }

                AdditionalEraseSettings additionalEraseSettings = (AdditionalEraseSettings)proto.GetSettings(typeof(BrushEraseTool), typeof(AdditionalEraseSettings));

                MegaWorldPath.GameObjectStoragePackage.Storage.AddModifiedСell(MegaWorldPath.GameObjectStoragePackage.Storage.CellList[cellIndex]);

                for (int itemIndex = 0; itemIndex < gameObjectInfo.itemList.Count; itemIndex++)
                {
                    GameObject go = gameObjectInfo.itemList[itemIndex];

                    if (go == null)
                    {
                        continue;
                    }

                    GameObject prefabRoot = VladislavTsurikov.GameObjectUtility.GetPrefabRoot(go);
                    if (prefabRoot == null)
                    {
                        return true;
                    }

                    if(areaVariables.Bounds.Contains(prefabRoot.transform.position) == true)
                    {
                        float fitness = 1;

                        if(group.FilterType == FilterType.SimpleFilter)
                        {
                            RaycastHit hit;
                            if (Physics.Raycast(RayUtility.GetRayFromCameraPosition(prefabRoot.transform.position), out hit, MegaWorldPath.AdvancedSettings.EditorSettings.raycastSettings.MaxRayDistance, 
                                layerSettings.GetCurrentPaintLayers(group.ResourceType)))
    		                {
                                fitness = simpleFilterSettings.GetFitness(hit.point, hit.normal);
                            }
                        }
                        else
                        {
                            if(maskFilterSettings.Stack.Settings.Count != 0)
                            {
                                fitness = GrayscaleFromTexture.GetFromWorldPosition(areaVariables.Bounds, prefabRoot.transform.position, maskFilterSettings.FilterMaskTexture2D);
                            }
                        }

                        float maskFitness = GrayscaleFromTexture.GetFromWorldPosition(areaVariables.Bounds, prefabRoot.transform.position, areaVariables.Mask);

                        fitness *= maskFitness;

                        fitness *= BrushEraseToolPath.Settings.EraseStrength;

                        float successOfErase = UnityEngine.Random.Range(0.0f, 1.0f);

                        if(successOfErase < fitness)
                        {
                            float randomSuccessForErase = UnityEngine.Random.Range(0.0f, 1.0f);

                            if(randomSuccessForErase < additionalEraseSettings.SuccessForErase / 100)
                            {
                                UndoSystem.Undo.RegisterUndoAfterMouseUp(new DestroyedGameObject(go));
                                UnityEngine.Object.DestroyImmediate(prefabRoot);
                            }
                        }
                    }
                }

                return true;
            });

            MegaWorldPath.GameObjectStoragePackage.Storage.RemoveNullData(true);
        }

        private void EraseTerrainDetails(Group group, AreaVariables areaVariables)
        {
            if(TerrainResourcesController.IsSyncError(group, Terrain.activeTerrain) == false)
            {
                return;
            }

            UpdateFilterMask.UpdateFilterMaskTexture(group, areaVariables);
            
            Vector2Int eraseSize;
	        Vector2Int position, startPosition;
        
            eraseSize = new Vector2Int(
					CommonUtility.WorldToDetail(areaVariables.Size * areaVariables.SizeMultiplier, areaVariables.TerrainUnderCursor.terrainData.size.x, areaVariables.TerrainUnderCursor.terrainData),
					CommonUtility.WorldToDetail(areaVariables.Size * areaVariables.SizeMultiplier, areaVariables.TerrainUnderCursor.terrainData.size.z, areaVariables.TerrainUnderCursor.terrainData));
        
            Vector2Int halfBrushSize = eraseSize / 2;

            Vector2Int center = new Vector2Int(
                CommonUtility.WorldToDetail(areaVariables.RayHit.Point.x - areaVariables.TerrainUnderCursor.transform.position.x, areaVariables.TerrainUnderCursor.terrainData),
                CommonUtility.WorldToDetail(areaVariables.RayHit.Point.z - areaVariables.TerrainUnderCursor.transform.position.z, areaVariables.TerrainUnderCursor.terrainData.size.z, 
                areaVariables.TerrainUnderCursor.terrainData));
        
            position = center - halfBrushSize;
            startPosition = Vector2Int.Max(position, Vector2Int.zero);
        
            Vector2Int offset = startPosition - position;
        
            Vector2Int current;
            float fitness = 1;
            float detailmapResolution = areaVariables.TerrainUnderCursor.terrainData.detailResolution;
            int x, y;

            MaskFilterSettings maskFilterSettings = (MaskFilterSettings)group.GetSettings(typeof(BrushEraseTool), typeof(MaskFilterSettings));

            foreach (PrototypeTerrainDetail proto in group.ProtoTerrainDetailList)
            {
                if(proto.Active == false || proto.Selected == false)
                {
                    continue;
                }

                AdditionalEraseSettings additionalEraseSettings = (AdditionalEraseSettings)proto.GetSettings(typeof(BrushEraseTool), typeof(AdditionalEraseSettings));

                int[,] localData = areaVariables.TerrainUnderCursor.terrainData.GetDetailLayer(
                    startPosition.x, startPosition.y,
                    Mathf.Max(0, Mathf.Min(position.x + eraseSize.x, (int)detailmapResolution) - startPosition.x),
                    Mathf.Max(0, Mathf.Min(position.y + eraseSize.y, (int)detailmapResolution) - startPosition.y), proto.TerrainProtoId);

                float widthY = localData.GetLength(1);
                float heightX = localData.GetLength(0);
                
                if (maskFilterSettings.Stack.Settings.Count > 0)
			    {
                    Texture2D filterMaskTexture2D = maskFilterSettings.FilterMaskTexture2D;

                    for (y = 0; y < widthY; y++)
                    {
                        for (x = 0; x < heightX; x++)
                        {
                            current = new Vector2Int(y, x);

                            float randomSuccess = UnityEngine.Random.Range(0.0f, 1.0f);

                            if(randomSuccess < additionalEraseSettings.SuccessForErase / 100)
                            {
                                Vector2 normal = Vector2.zero;
                                normal.y = Mathf.InverseLerp(0, eraseSize.y, current.y);
                                normal.x = Mathf.InverseLerp(0, eraseSize.x, current.x);

                                fitness = GrayscaleFromTexture.Get(normal, maskFilterSettings.FilterMaskTexture2D);

                                float maskFitness = BrushJitterSettings.GetAlpha(areaVariables, current + offset, eraseSize);

                                fitness *= maskFitness;

                                fitness *= BrushEraseToolPath.Settings.EraseStrength;

                                int targetStrength = Mathf.Max(0, localData[x, y] - Mathf.RoundToInt(Mathf.Lerp(0, 10, fitness)));

                                localData[x, y] = targetStrength;
                            }
                        }
                    }

                    areaVariables.TerrainUnderCursor.terrainData.SetDetailLayer(startPosition.x, startPosition.y, proto.TerrainProtoId, localData);
                }
                else
                {
                    for (y = 0; y < widthY; y++)
                    {
                        for (x = 0; x < heightX; x++)
                        {
                            current = new Vector2Int(y, x);

                            float randomSuccess = UnityEngine.Random.Range(0.0f, 1.0f);

                            if(randomSuccess < additionalEraseSettings.SuccessForErase / 100)
                            {
                                float maskFitness = BrushJitterSettings.GetAlpha(areaVariables, current + offset, eraseSize);

                                maskFitness *= BrushEraseToolPath.Settings.EraseStrength;

                                int targetStrength = Mathf.Max(0, localData[x, y] - Mathf.RoundToInt(Mathf.Lerp(0, 10, maskFitness)));

                                localData[x, y] = targetStrength;
                            }
                        }
                    }

                    areaVariables.TerrainUnderCursor.terrainData.SetDetailLayer(startPosition.x, startPosition.y, proto.TerrainProtoId, localData);
                }
            }
        }

        public override void HandleKeyboardEvents()
        {
			BrushEraseToolPath.Settings.BrushSettingsForErase.ScrollBrushRadiusEvent();
		}
    }
}
#endif