using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if INSTANT_RENDERER
using VladislavTsurikov.InstantRendererSystem;
#endif
using VladislavTsurikov;

namespace VladislavTsurikov.MegaWorldSystem
{
    public enum OverlapShape 
    { 
        None, 
        Bounds,
        Sphere,
    }
    
    [Serializable]
    public class OverlapCheckSettings : BaseSettings
    {
        public OverlapShape OverlapShape = OverlapShape.Sphere;
        public BoundsCheck BoundsCheck = new BoundsCheck();
        public SphereCheck SphereCheck = new SphereCheck();
        public CollisionCheck CollisionCheck = new CollisionCheck();

#if UNITY_EDITOR
        public OverlapCheckSettingsEditor OverlapСheckTypeEditor = new OverlapCheckSettingsEditor();

        public override void OnGUI()
        {
            OverlapСheckTypeEditor.OnGUI(this);
        }
#endif

        public OverlapCheckSettings()
        {

        }

        public static bool RunOverlapCheck(ResourceType resourceType, OverlapCheckSettings overlapCheckSettings, Vector3 extents, InstanceData instanceData)
        {
            if(RunCollisionCheck(overlapCheckSettings, extents, instanceData))
            {
                return false;
            }

            if(overlapCheckSettings.OverlapShape == OverlapShape.None)
            {
                return true;
            }

            if(resourceType == ResourceType.GameObject)
            {
                if(!RunOverlapCheckForGameObject(overlapCheckSettings, overlapCheckSettings.GetCheckBounds(instanceData, extents)))
                {
                    return true;
                }
            }
            else 
            {
                if(!RunOverlapCheckForInstantItem(overlapCheckSettings, overlapCheckSettings.GetCheckBounds(instanceData, extents)))
                {
                    return true;
                }
            }

            return false;
        }


        public static bool RunOverlapCheckForInstantItem(OverlapCheckSettings overlapCheckSettings, Bounds checkBounds)
        {
#if INSTANT_RENDERER
            if(InstantRendererController.InstantRenderer == null)
            {
                return false;
            }

            List<Cell> overlapCellList = new List<Cell>();  
                           
            InstantRendererController.StorageTerrainCells.CellQuadTree.Query(RectExtension.CreateRectFromBounds(checkBounds), overlapCellList);

            for (int i = 0; i <= overlapCellList.Count - 1; i++)
            {
                List<ItemInfo> persistentInfoList = InstantRendererController.StorageTerrainCells.PersistentStoragePackage.CellList[overlapCellList[i].Index].ItemInfoList;
                
                for (int infoIndex = 0; infoIndex < persistentInfoList.Count; infoIndex++)
                {
                    ItemInfo persistentInfo = persistentInfoList[infoIndex];

                    PrototypeInstantItem proto = GetPrototype.GetCurrentInstantItem(persistentInfo.ID, false);

                    if(proto == null)
                    {
                        continue;
                    }

                    OverlapCheckSettings localOverlapCheckSettings = (OverlapCheckSettings)proto.SettingsStack.GetSettings(typeof(OverlapCheckSettings));

                    if(localOverlapCheckSettings.OverlapShape == OverlapShape.None)
                    {
                        continue;
                    }

                    for (int itemIndex = 0; itemIndex < persistentInfo.InstanceDataList.Count; itemIndex++)
                    {
                        VladislavTsurikov.InstantRendererSystem.InstanceData persistentItem = persistentInfo.InstanceDataList[itemIndex];

                        if(OverlapCheck(localOverlapCheckSettings, persistentItem.Position, persistentItem.Scale, proto.Extents, checkBounds, overlapCheckSettings))
                        {
                            return true;
                        }
                    }
                }
            }
#endif
            return false;
        }


        private static bool RunOverlapCheckForGameObject(OverlapCheckSettings overlapCheckSettings, Bounds checkBounds)
        {
            bool overlaps = false;

            MegaWorldPath.GameObjectStoragePackage.Storage.IntersectBounds(checkBounds, (gameObjectInfo, cellIndex) =>
            {
                PrototypeGameObject proto = GetPrototype.GetCurrentPrototypeGameObject(gameObjectInfo.ID, false);

                if(proto == null)
                {
                    return true;
                }

                OverlapCheckSettings localOverlapCheckSettings = (OverlapCheckSettings)proto.SettingsStack.GetSettings(typeof(OverlapCheckSettings));

                if(localOverlapCheckSettings.OverlapShape == OverlapShape.None)
                {
                    return true;
                }

                for (int itemIndex = 0; itemIndex < gameObjectInfo.itemList.Count; itemIndex++)
                {
                    GameObject go = gameObjectInfo.itemList[itemIndex];

                    if (go == null)
                    {
                        continue;
                    }

                    GameObject prefabRoot = GameObjectUtility.GetPrefabRoot(go);
                    if (prefabRoot == null)
                    {
                        continue;
                    }

                    if(OverlapCheck(localOverlapCheckSettings, prefabRoot.transform.position, prefabRoot.transform.localScale, proto.Extents, checkBounds, overlapCheckSettings))
                    {
                        overlaps = true;
                        return false;
                    }
                }

                return true;
            });

            return overlaps;
        }

        private static bool OverlapCheck(OverlapCheckSettings localOverlapCheckSettings, Vector3 localPosition, Vector3 localScale, Vector3 extents, Bounds checkBounds, OverlapCheckSettings checkOverlapCheckSettings)
        {
            switch (localOverlapCheckSettings.OverlapShape)
            {
                case OverlapShape.Bounds:
                {
                    Bounds localBounds = BoundsCheck.GetBounds(localOverlapCheckSettings.BoundsCheck, localPosition, localScale, extents);

                    return localBounds.Intersects(checkBounds);
                }
                case OverlapShape.Sphere:
                {
                    return checkOverlapCheckSettings.SphereCheck.OverlapCheck(localOverlapCheckSettings.SphereCheck, localPosition, localScale, checkBounds);
                }
            }

            return false;
        }

        public static bool RunCollisionCheck(OverlapCheckSettings overlapCheckSettings, Vector3 prefabExtents, InstanceData instanceData)
        {
            if(overlapCheckSettings.CollisionCheck.collisionCheckType)
            {
                Vector3 extents = Vector3.Scale(prefabExtents * overlapCheckSettings.CollisionCheck.multiplyBoundsSize, instanceData.Scale);

                if(overlapCheckSettings.CollisionCheck.IsBoundHittingWithCollisionsLayers(instanceData.Position, instanceData.Rotation.eulerAngles.y, extents))
                {
                    return true;
                }
            }

            return false;
        }

        public Bounds GetCheckBounds(InstanceData instanceData, Vector3 extents)
        {
            if(OverlapShape == OverlapShape.Bounds)
            {
                return BoundsCheck.GetBounds(BoundsCheck, instanceData.Position, instanceData.Scale, extents);
            }
            else if(OverlapShape == OverlapShape.Sphere)
            {
                return SphereCheck.GetBounds(SphereCheck, instanceData.Position, instanceData.Scale);
            }

            return new Bounds(Vector3.zero, Vector3.zero);
        }

#if UNITY_EDITOR
        public static void VisualizeOverlapForInstantItem(Bounds bounds, bool showSelectedProto = false)
        {
#if INSTANT_RENDERER
            if (InstantRendererController.CheckMissingData() == false) return;

            Rect positionRect = RectExtension.CreateRectFromBounds(bounds);

            List<Cell> overlapCellList = new List<Cell>();                 
            InstantRendererController.InstantRenderer.StorageTerrainCells.CellQuadTree.Query(positionRect, overlapCellList);

            for (int i = 0; i <= overlapCellList.Count - 1; i++)
            {
                int cellIndex = overlapCellList[i].Index;

                List<ItemInfo> persistentInfoList = InstantRendererController.InstantRenderer.StorageTerrainCells.PersistentStoragePackage.CellList[cellIndex].ItemInfoList;

                for (int persistentInfoIndex = 0; persistentInfoIndex < persistentInfoList.Count; persistentInfoIndex++)
                {
                    ItemInfo persistentInfo = persistentInfoList[persistentInfoIndex];

                    PrototypeInstantItem proto = GetPrototype.GetCurrentInstantItem(persistentInfo.ID, false);

                    if(proto == null)
                    {
                        continue;
                    }

                    OverlapCheckSettings overlapCheckSettings = (OverlapCheckSettings)proto.SettingsStack.GetSettings(typeof(OverlapCheckSettings));

                    if(showSelectedProto)
                    {
                        if(proto.Selected == false)
                        {
                            continue;
                        }
                    }

                    for (int itemIndex = 0; itemIndex < persistentInfo.InstanceDataList.Count; itemIndex++)
                    {
                        VladislavTsurikov.InstantRendererSystem.InstanceData persistentItem = persistentInfo.InstanceDataList[itemIndex];

                        if(bounds.Contains(persistentItem.Position) == true)
                        {
                            DrawOverlapСheckType(persistentItem.Position, persistentItem.Scale, proto.Extents, overlapCheckSettings);
                        }
                    }
                }
            }
#endif
        }

        public static void VisualizeOverlapForGameObject(Bounds bounds, bool showSelectedProto = false)
        {
            MegaWorldPath.GameObjectStoragePackage.Storage.IntersectBounds(bounds, (gameObjectInfo, cellIndex) =>
            {
                PrototypeGameObject proto = GetPrototype.GetCurrentPrototypeGameObject(gameObjectInfo.ID, false);

                if(proto == null)
                {
                    return true;
                }

                OverlapCheckSettings overlapCheckSettings = (OverlapCheckSettings)proto.SettingsStack.GetSettings(typeof(OverlapCheckSettings));

                if(showSelectedProto)
                {
                    if(proto.Selected == false)
                    {
                        return true;
                    }
                }

                for (int itemIndex = 0; itemIndex < gameObjectInfo.itemList.Count; itemIndex++)
                {
                    GameObject go = gameObjectInfo.itemList[itemIndex];

                    if (go == null)
                    {
                        continue;
                    }

                    GameObject prefabRoot = GameObjectUtility.GetPrefabRoot(go);
                    if (prefabRoot == null)
                    {
                        continue;
                    }

                    if(bounds.Contains(prefabRoot.transform.position) == false)
                    {
                        continue;
                    }

                    DrawOverlapСheckType(prefabRoot.transform.position, prefabRoot.transform.localScale, proto.Extents, overlapCheckSettings);
                }

                return true;
            });
        }

        public static void DrawOverlapСheckType(Vector3 position, Vector3 scale, Vector3 extents, OverlapCheckSettings overlapCheckSettings)
        {
            switch (overlapCheckSettings.OverlapShape)
            {
                case OverlapShape.Sphere:
                {
                    SphereCheck.DrawOverlapСheck(position, scale, overlapCheckSettings.SphereCheck);

                    break;
                }
                case OverlapShape.Bounds:
                {
                    BoundsCheck.DrawIntersectionСheckType(position, scale, extents, overlapCheckSettings.BoundsCheck);
                    
                    break;
                }
            }
        }
#endif
    }
}