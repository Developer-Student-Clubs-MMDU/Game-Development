using UnityEngine;
using UnityEditor;
using VladislavTsurikov;
using System.Collections.Generic;
using System;

namespace VladislavTsurikov.MegaWorldSystem.AdvancedBrush
{
    [Tool(true, true, new ResourceType[]{ResourceType.InstantItem, ResourceType.GameObject, ResourceType.TerrainDetail, ResourceType.TerrainTexture})]    
    public class AdvancedBrushTool : ToolComponent
    {
        private int _editorHash = "Editor".GetHashCode();
        private MouseMoveData _mouseMoveData = new MouseMoveData();
        private bool _startDrag = false;
        
#if UNITY_EDITOR
        public override void AddPrototypeSettingsTypes()
        {
            foreach (ResourceType resourceType in typeof(ResourceType).GetEnumValues())
            {
                List<System.Type> settingsTypes = new List<System.Type>();

                switch (resourceType)
                {
                    case ResourceType.InstantItem:
                    {
                        settingsTypes.Add(typeof(SuccessSettings));
                        settingsTypes.Add(typeof(OverlapCheckSettings));
                        settingsTypes.Add(typeof(TransformComponentSettings));

                        SettingsTypesStack.AddPrototypeSettingsTypes(ResourceType.InstantItem, settingsTypes);

                        break;
                    }
                    case ResourceType.GameObject:
                    {
                        settingsTypes.Add(typeof(SuccessSettings));
                        settingsTypes.Add(typeof(OverlapCheckSettings));
                        settingsTypes.Add(typeof(TransformComponentSettings));

                        SettingsTypesStack.AddPrototypeSettingsTypes(ResourceType.GameObject, settingsTypes);

                        break;
                    }
                    case ResourceType.TerrainDetail:
                    {
                        settingsTypes.Add(typeof(MaskFilterSettings));
                        settingsTypes.Add(typeof(SpawnDetailSettings));

                        SettingsTypesStack.AddPrototypeSettingsTypes(ResourceType.TerrainDetail, settingsTypes);

                        break;
                    }
                    case ResourceType.TerrainTexture:
                    {
                        settingsTypes.Add(typeof(MaskFilterSettings));

                        SettingsTypesStack.AddPrototypeSettingsTypes(ResourceType.TerrainTexture, settingsTypes);

                        break;
                    }
                }
            }
        }

        public override void AddGroupSettingsTypes()
        {
            List<System.Type> settingsTypes = new List<System.Type>();
            settingsTypes.Add(typeof(ScatterSettings));
            settingsTypes.Add(typeof(SimpleFilterSettings));
            settingsTypes.Add(typeof(MaskFilterSettings));

            SettingsTypesStack.AddGroupSettingsTypes(settingsTypes);
        }

        public override void DoTool()
        {            
            BrushSettings brush = AdvancedBrushToolPath.Settings.BrushSettings;
            
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
                                PaintType(group, areaVariables);
                            }
                        }
                    }
                    
                    _mouseMoveData.DragDistance = 0;
                    _mouseMoveData.PrevRaycast = _mouseMoveData.Raycast;
                    _startDrag = true;

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

                    float brushSpacing = brush.GetCurrentSpacing();
                    if(_startDrag)
                    {
                        if(MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoInstantItemList.Count != 0
                        || MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoGameObjectList.Count != 0)
                        {
                            if(brushSpacing < brush.BrushSize / 2)
                            {
                                brushSpacing = brush.BrushSize / 2;
                            }
                        }
                    }

                    _mouseMoveData.DragMouse(brushSpacing, (dragPoint) =>
                    {
                        _startDrag = false;

                        foreach (Group group in MegaWorldPath.DataPackage.SelectedVariables.SelectedGroupList)
                        {
                            LayerSettings layerSettings = MegaWorldPath.CommonDataPackage.layerSettings;
                            RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(dragPoint), layerSettings.GetCurrentPaintLayers(group.ResourceType));

                            if(rayHit != null)
                            {
                                AreaVariables areaVariables = brush.BrushJitterSettings.GetAreaVariables(brush, rayHit.Point, group);

                                if(areaVariables.RayHit != null)
                                {
                                    PaintType(group, areaVariables, AdvancedBrushToolPath.Settings.EnableFailureRateOnMouseDrag);
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
                    AdvancedBrushToolVisualisation.Draw(areaVariables);

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
            AdvancedBrushToolPath.Settings.Save();
        }

        public override void HandleKeyboardEvents()
        {
			AdvancedBrushToolPath.Settings.BrushSettings.ScrollBrushRadiusEvent();
		}
#endif

        public void PaintType(Group group, AreaVariables areaVariables, bool dragMouse = false)
        {
            switch (group.ResourceType)
            {
                case ResourceType.GameObject:
                {
                    SpawnTypeGameObject(group, areaVariables, dragMouse);
                    
                    break;
                }
                case ResourceType.TerrainDetail:
                {
                    SpawnType.SpawnTerrainDetails(group, group.GetAllSelectedUnityTerrainDetail(), areaVariables);

                    break;
                }
                case ResourceType.TerrainTexture:
                {
                    SpawnType.SpawnTerrainTexture(group, MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoTerrainTextureList, areaVariables, AdvancedBrushToolPath.Settings.TextureTargetStrength);

                    break;
                }
                case ResourceType.InstantItem:
                {
                    SpawnTypeInstantItem(group, areaVariables, dragMouse);

                    break;
                }
            }
        }

        public void SpawnTypeGameObject(Group group, AreaVariables areaVariables, bool dragMouse)
        {
            if(group.FilterType == FilterType.MaskFilter)
            {
                UpdateFilterMask.UpdateFilterMaskTexture(group, areaVariables);
            }

            ScatterSettings scatterSettings = (ScatterSettings)group.GetSettings(typeof(ScatterSettings));
            LayerSettings layerSettings = MegaWorldPath.CommonDataPackage.layerSettings;

            foreach (Vector2 sample in scatterSettings.Stack.Samples(areaVariables))
            {
                if(dragMouse)
                {
                    if(UnityEngine.Random.Range(0f, 100f) < AdvancedBrushToolPath.Settings.FailureRate)
                    {
                        continue;
                    }
                }

                RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(new Vector3(sample.x, areaVariables.RayHit.Point.y, sample.y)), layerSettings.GetCurrentPaintLayers(group.ResourceType));
                if(rayHit != null)
                {
                    PrototypeGameObject proto = GetRandomPrototype.GetMaxSuccessProtoGameObject(group.GetAllSelectedGameObject());

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

                        SpawnPrototype.SpawnGameObject(group, proto, areaVariables, rayHit, fitness);
                    }
                }
            }
        }

        public void SpawnTypeInstantItem(Group group, AreaVariables areaVariables, bool dragMouse = false)
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
            
            LayerSettings layerSettings = MegaWorldPath.CommonDataPackage.layerSettings;
            ScatterSettings scatterSettings = (ScatterSettings)group.GetSettings(typeof(ScatterSettings));
            
            foreach (Vector2 sample in scatterSettings.Stack.Samples(areaVariables))
            {
                if(dragMouse)
                {
                    if(UnityEngine.Random.Range(0f, 100f) < AdvancedBrushToolPath.Settings.FailureRate)
                    {
                        continue;
                    }
                }

                RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(new Vector3(sample.x, areaVariables.RayHit.Point.y, sample.y)), layerSettings.GetCurrentPaintLayers(group.ResourceType));
                if(rayHit != null)
                {
                    PrototypeInstantItem proto = GetRandomPrototype.GetMaxSuccessProtoInstantItem(group.GetAllSelectedInstantItem());

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
    }
}