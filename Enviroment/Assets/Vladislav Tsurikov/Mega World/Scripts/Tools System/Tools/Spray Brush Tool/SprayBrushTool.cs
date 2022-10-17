using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
#if UNITY_EDITOR
using VladislavTsurikov.UndoSystem;
#endif
#if UNITY_2021_2_OR_NEWER
using UnityEngine.TerrainTools;
#else
using UnityEngine.Experimental.TerrainAPI;
#endif
using VladislavTsurikov.RaycastEditorSystem;

namespace VladislavTsurikov.MegaWorldSystem.SprayBrush
{
    [Tool(true, true, new ResourceType[]{ResourceType.GameObject, ResourceType.InstantItem})]    
    public class SprayBrushTool : ToolComponent
    {
        private int _editorHash = "Editor".GetHashCode();
        private MouseMoveData _mouseMoveData = new MouseMoveData();
        
#if UNITY_EDITOR
        public override void AddPrototypeSettingsTypes()
        {
            foreach (ResourceType resourceType in typeof(ResourceType).GetEnumValues())
            {
                switch (resourceType)
                {
                    case ResourceType.InstantItem:
                    {
                        List<System.Type> settingsTypes = new List<System.Type>();
                        settingsTypes.Add(typeof(SuccessSettings));
                        settingsTypes.Add(typeof(OverlapCheckSettings));

                        SettingsTypesStack.AddPrototypeSettingsTypes(ResourceType.InstantItem, settingsTypes);

                        break;
                    }
                    case ResourceType.GameObject:
                    {
                        List<System.Type> settingsTypes = new List<System.Type>();
                        settingsTypes.Add(typeof(SuccessSettings));
                        settingsTypes.Add(typeof(OverlapCheckSettings));

                        SettingsTypesStack.AddPrototypeSettingsTypes(ResourceType.GameObject, settingsTypes);

                        break;
                    }
                }
            }
        }

        public override void AddPrototypeToolSettingsTypes()
        {
            foreach (ResourceType resourceType in typeof(ResourceType).GetEnumValues())
            {
                switch (resourceType)
                {
                    case ResourceType.InstantItem:
                    {
                        List<System.Type> settingsTypes = new List<System.Type>();
                        settingsTypes.Add(typeof(TransformComponentSettings));

                        SettingsTypesStack.AddPrototypeToolSettingsTypes(ResourceType.InstantItem, this.GetType(), settingsTypes);

                        break;
                    }
                    case ResourceType.GameObject:
                    {
                        List<System.Type> settingsTypes = new List<System.Type>();
                        settingsTypes.Add(typeof(TransformComponentSettings));

                        SettingsTypesStack.AddPrototypeToolSettingsTypes(ResourceType.GameObject, this.GetType(), settingsTypes);

                        break;
                    }
                }
            }
        }

        public override void AddGroupToolSettingsTypes()
        {
            List<System.Type> settingsTypes = new List<System.Type>();
            settingsTypes.Add(typeof(SimpleFilterSettings));

            SettingsTypesStack.AddGroupToolSettingsTypes(this.GetType(), settingsTypes);
        }

        public override void DoTool()
        {            
            BrushSettings brush = SprayBrushToolPath.Settings.BrushSettings;
            
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
                            AreaVariables areaVariables = brush.GetAreaVariables(_mouseMoveData.Raycast);

                            if(areaVariables.RayHit != null)
                            {
                                PaintType(group, areaVariables);
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

                    float brushSpacing = brush.Spacing;

                    LayerSettings layerSettings = MegaWorldPath.CommonDataPackage.layerSettings;

                    _mouseMoveData.DragMouse(brushSpacing, (dragPoint) =>
                    {
                        foreach (Group group in MegaWorldPath.DataPackage.SelectedVariables.SelectedGroupList)
                        {
                            RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayFromCameraPosition(dragPoint), layerSettings.GetCurrentPaintLayers(group.ResourceType));

                            if(rayHit != null)
                            {
                                AreaVariables areaVariables = brush.GetAreaVariables(rayHit);

                                if(areaVariables.RayHit != null)
                                {
                                    PaintType(group, areaVariables);
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
                    SprayBrushToolVisualisation.Draw(areaVariables);

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
            SprayBrushToolPath.Settings.Save();
        }

        public override void HandleKeyboardEvents()
        {
			SprayBrushToolPath.Settings.BrushSettings.ScrollBrushRadiusEvent();
		}
#endif

        public void PaintType(Group group, AreaVariables areaVariables)
        {
            switch (group.ResourceType)
            {
                case ResourceType.InstantItem:
                {
                    SpawnTypeInstantItem(group, areaVariables);
                    
                    break;
                }
                case ResourceType.GameObject:
                {
                    SpawnTypeGameObject(group, areaVariables);
                    
                    break;
                }
            }
        }

        public void SpawnTypeInstantItem(Group group, AreaVariables areaVariables)
        {
            ScatterSettings scatterSettings = (ScatterSettings)group.GetSettings(typeof(ScatterSettings));
            LayerSettings layerSettings = MegaWorldPath.CommonDataPackage.layerSettings;;

            Vector3 positionForSpawn = areaVariables.RayHit.Point + Vector3.ProjectOnPlane(UnityEngine.Random.onUnitSphere, areaVariables.RayHit.Normal) * areaVariables.Radius;

            RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayFromCameraPosition(positionForSpawn), layerSettings.GetCurrentPaintLayers(group.ResourceType));
            if(rayHit != null)
            {
                PrototypeInstantItem proto = GetRandomPrototype.GetMaxSuccessProtoInstantItem(group.GetAllSelectedInstantItem());

                if(proto == null || proto.Active == false)
                {
                    return;
                }

                float fitness = GetFitnessUtility.GetFitnessFromSimpleFilter((SimpleFilterSettings)group.GetSettings(this.GetType(), typeof(SimpleFilterSettings)), rayHit);
                
                if(fitness != 0)
                {
                    SpawnInstantItem(group, proto, areaVariables, rayHit, fitness);
                }
            }
        }

        public void SpawnTypeGameObject(Group group, AreaVariables areaVariables)
        {
            ScatterSettings scatterSettings = (ScatterSettings)group.GetSettings(typeof(ScatterSettings));
            LayerSettings layerSettings = MegaWorldPath.CommonDataPackage.layerSettings;;

            Vector3 positionForSpawn = areaVariables.RayHit.Point + Vector3.ProjectOnPlane(UnityEngine.Random.onUnitSphere, areaVariables.RayHit.Normal) * areaVariables.Radius;

            RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayFromCameraPosition(positionForSpawn), layerSettings.GetCurrentPaintLayers(group.ResourceType));
            if(rayHit != null)
            {
                PrototypeGameObject proto = GetRandomPrototype.GetMaxSuccessProtoGameObject(group.GetAllSelectedGameObject());

                if(proto == null || proto.Active == false)
                {
                    return;
                }

                float fitness = GetFitnessUtility.GetFitnessFromSimpleFilter((SimpleFilterSettings)group.GetSettings(this.GetType(), typeof(SimpleFilterSettings)), rayHit);
                
                if(fitness != 0)
                {
                    SpawnGameObject(group, proto, areaVariables, rayHit, fitness);
                }
            }
        }

        public static void SpawnInstantItem(Group group, PrototypeInstantItem proto, AreaVariables areaVariables, RayHit rayHit, float fitness)
        {
#if INSTANT_RENDERER
            OverlapCheckSettings overlapCheckSettings = (OverlapCheckSettings)proto.GetSettings(typeof(OverlapCheckSettings));

            InstanceData instanceData = new InstanceData(rayHit.Point, proto.Prefab.transform.lossyScale, proto.Prefab.transform.rotation);

            TransformComponentSettings transformComponentSettings = (TransformComponentSettings)proto.GetSettings(typeof(SprayBrushTool), typeof(TransformComponentSettings));
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

            TransformComponentSettings transformComponentSettings = (TransformComponentSettings)proto.GetSettings(typeof(SprayBrushTool), typeof(TransformComponentSettings));
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
    }
}