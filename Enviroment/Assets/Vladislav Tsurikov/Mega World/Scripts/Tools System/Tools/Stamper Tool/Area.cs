using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
#if GRIFFIN_2020 || GRIFFIN_2021
using Pinwheel.Griffin;
#endif

namespace VladislavTsurikov.MegaWorldSystem.Stamper
{
    public enum HandleSettingsMode
    { 
        Custom,
        Standard
    }

    [Serializable]
    public class Area 
    {
        public bool UseSpawnCells = false;
        public float CellSize = 1000;
        public List<Bounds> CellList = new List<Bounds>();

        public bool UseMask = false;
        public MaskType MaskType = MaskType.Procedural;
        public ProceduralMask ProceduralMask = new ProceduralMask();
        public CustomMasks CustomMasks = new CustomMasks();
        
        public Vector3 PastThisPosition = Vector3.zero;
        public Vector3 PastScale = Vector3.one;
        public Bounds Bounds;
        public bool HandlesSettingsFoldout = false;        

        public Color ColorCube = Color.HSVToRGB(0.0f, 0.75f, 1.0f);
        public float PixelWidth = 4.0f;
        public bool Dotted = false;
        public HandleSettingsMode HandleSettingsMode = HandleSettingsMode.Standard;
        public bool DrawHandleIfNotSelected = false;

#if UNITY_EDITOR
        public AreaEditor AreaEditor = new AreaEditor();
        public bool AreaSettingsFoldout = true;
        public bool ShowCells = true;

        public void OnGUI(StamperTool stamper)
        {
            AreaEditor.OnGUI(stamper, this);
        }
#endif

        public void SetAreaBounds(StamperTool stamperTool)
        {
            Bounds = new Bounds();
            Bounds.size = new Vector3(stamperTool.transform.localScale.x, stamperTool.transform.localScale.y, stamperTool.transform.localScale.z);
            Bounds.center = stamperTool.transform.position;
        }

        public void FitToTerrainSize(StamperTool stamperTool)
        {
            if(Terrain.activeTerrains.Length != 0)
            {
                Bounds newBounds = new Bounds(Vector3.zero, Vector3.zero);
                for (int i = 0; i < Terrain.activeTerrains.Length; i++)
                {
                    Terrain terrain = Terrain.activeTerrains[i];

                    Bounds terrainBounds = new Bounds(terrain.terrainData.bounds.center + terrain.transform.position, terrain.terrainData.bounds.size);;

                    if (i == 0)
                    {
                        newBounds = terrainBounds;
                    }
                    else
                    {
                        newBounds.Encapsulate(terrainBounds);
                    }
                }

                stamperTool.transform.position = newBounds.center + new Vector3(1, 0, 1);
                stamperTool.transform.localScale = newBounds.size + new Vector3(1, 0, 1);
            }
            #if GRIFFIN_2020 || GRIFFIN_2021
            else
            {
                Bounds b = GCommon.GetLevelBounds();
                stamperTool.transform.position = new Vector3(b.center.x, b.size.y / 2, b.center.z);
                stamperTool.transform.localScale = new Vector3(b.size.x, b.size.y, b.size.z);
            }
            #endif
        }

        public void CreateCells()
        {
            CellList.Clear();

            Bounds expandedBounds = new Bounds(this.Bounds.center, this.Bounds.size);
            expandedBounds.Expand(new Vector3(CellSize * 2f, 0, CellSize * 2f));

            int cellXCount = Mathf.CeilToInt(this.Bounds.size.x / CellSize);
            int cellZCount = Mathf.CeilToInt(this.Bounds.size.z / CellSize);

            Vector2 corner = new Vector2(this.Bounds.center.x - this.Bounds.size.x / 2f, this.Bounds.center.z - this.Bounds.size.z / 2f);

            Bounds bounds = new Bounds();

            for (int x = 0; x <= cellXCount - 1; x++)
            {
                for (int z = 0; z <= cellZCount - 1; z++)
                {
                    Rect rect = new Rect(
                        new Vector2(CellSize * x + corner.x, CellSize * z + corner.y),
                        new Vector2(CellSize, CellSize));

                    bounds = RectExtension.CreateBoundsFromRect(rect, this.Bounds.center.y, this.Bounds.size.y);

                    CellList.Add(bounds);
                }
            }
        }

        public void SetAreaBoundsIfNecessary(StamperTool stamperTool, bool setForce = false)
        {
            bool hasChangedPosition = PastThisPosition != stamperTool.transform.position;
            bool hasChangedSize = stamperTool.transform.localScale != PastScale;

            if(hasChangedPosition || hasChangedSize)
            {
#if UNITY_EDITOR
                stamperTool.StamperVisualisation.UpdateMask = true;
#endif
                
                SetAreaBounds(stamperTool);

                if(UseSpawnCells)
                {
                    CellList.Clear();
                }
            }
            else if(setForce)
            {
#if UNITY_EDITOR
                stamperTool.StamperVisualisation.UpdateMask = true;
#endif

                SetAreaBounds(stamperTool);
            }

            PastScale = stamperTool.transform.localScale;

            PastThisPosition = stamperTool.transform.position;
        }

        public Texture2D GetCurrentRaw()
        {
            if(UseMask == false || UseSpawnCells)
            {
                return Texture2D.whiteTexture;
            }

            switch (MaskType)
            {
                case MaskType.Custom:
                {
                    Texture2D texture = CustomMasks.GetSelectedBrush();

                    return texture;
                }
                case MaskType.Procedural:
                {
                    Texture2D texture = ProceduralMask.Mask;

                    return texture;
                }
            }

            return Texture2D.whiteTexture;
        }

        public AreaVariables GetAreaVariables(RayHit hit)
        {
            AreaVariables areaVariables = new AreaVariables();

            areaVariables.Mask = GetCurrentRaw();
            areaVariables.Size = Bounds.size.x;
            areaVariables.RayHit = hit;
            areaVariables.TerrainUnderCursor = CommonUtility.GetTerrain(hit.Point);
            areaVariables.Bounds = Bounds;

            return areaVariables;
        }
    }

#if UNITY_EDITOR 
    public class AreaGizmoDrawer
    {
        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NonSelected | GizmoType.NotInSelectionHierarchy | GizmoType.Selected)]
        static void DrawGizmoForArea(StamperTool stamperTool, GizmoType gizmoType)
        {
            bool isFaded = (int)gizmoType == (int)GizmoType.NonSelected || (int)gizmoType == (int)GizmoType.NotInSelectionHierarchy || (int)gizmoType == (int)GizmoType.NonSelected + (int)GizmoType.NotInSelectionHierarchy;
            
            if(stamperTool.Area.DrawHandleIfNotSelected == false)
            {
                if(isFaded == true)
                {
                    return;
                }
            }

            if(stamperTool.Data.SelectedVariables.SelectedProtoGameObjectList.Count != 0)
            {
                Bounds bounds = new Bounds(Camera.current.transform.position, new Vector3(50f, 50f, 50f));
                OverlapCheckSettings.VisualizeOverlapForGameObject(bounds, true);
            }
            
			if(stamperTool.Data.SelectedVariables.SelectedProtoInstantItemList.Count != 0)
			{
                Bounds bounds = new Bounds(Camera.current.transform.position, new Vector3(50f, 50f, 50f));
                OverlapCheckSettings.VisualizeOverlapForInstantItem(bounds, true);
			}

            float opacity = isFaded ? 0.5f : 1.0f;

            DrawStamperVisualisationifNecessary(stamperTool, opacity);

            DrawBox(stamperTool, opacity);

            if(stamperTool.Area.UseSpawnCells)
			{
                DebugCells(stamperTool);
            }
        }

        public static void DrawStamperVisualisationifNecessary(StamperTool stamperTool, float multiplyAlpha)
        {
            if(stamperTool.StamperToolControllerSettings.Visualisation == false)
            {
                return;
            }

            if(stamperTool.Data.SelectedVariables.HasOneSelectedGroup() == false)
            {
                return;
            }

            Group group = stamperTool.Data.SelectedVariables.SelectedGroup;

            LayerSettings layerSettings = MegaWorldPath.CommonDataPackage.layerSettings;

            RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(stamperTool.Area.Bounds.center), 
                layerSettings.GetCurrentPaintLayers(group.ResourceType));
            if(rayHit != null)
            {
                AreaVariables areaVariables = stamperTool.Area.GetAreaVariables(rayHit);
                stamperTool.StamperVisualisation.Draw(areaVariables, stamperTool.Data, multiplyAlpha);
            }
        }

        public static void DrawBox(StamperTool stamperTool, float alpha)
        {
            Transform newTransform = stamperTool.transform;
            newTransform.rotation = Quaternion.identity;
            newTransform.transform.localScale = new Vector3 (Mathf.Max(1f, newTransform.transform.localScale.z), Mathf.Max(1f, newTransform.transform.localScale.y), Mathf.Max(1f, newTransform.transform.localScale.z));

            if(stamperTool.Area.HandleSettingsMode == HandleSettingsMode.Custom)
            {
                Color color = stamperTool.Area.ColorCube;
                color.a *= alpha;
                VladislavTsurikov.DrawHandles.DrawCube(newTransform.localToWorldMatrix, color, stamperTool.Area.PixelWidth, stamperTool.Area.Dotted);
            }
            else
            {
                float thickness = 4.0f;
                Color color = Color.yellow;
                color.a *= alpha;
                VladislavTsurikov.DrawHandles.DrawCube(newTransform.localToWorldMatrix, color, thickness);
            }
        }

        public static void DebugCells(StamperTool stamperTool)
        {
            if(stamperTool.Area.ShowCells)
			{
                List<Bounds> cellList = stamperTool.Area.CellList;

                for (int i = 0; i <= cellList.Count - 1; i++)
                {                  
                    Gizmos.color = new Color(0, 1, 1, 1);
                    Gizmos.DrawWireCube(cellList[i].center, cellList[i].size);
                }
            }
        }
    }
#endif
}