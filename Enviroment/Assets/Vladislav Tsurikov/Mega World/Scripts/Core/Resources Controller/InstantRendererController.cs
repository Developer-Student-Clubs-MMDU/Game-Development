#if INSTANT_RENDERER
using VladislavTsurikov.InstantRendererSystem;
using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.MegaWorldSystem
{
    public class InstantRendererController 
    {
        public enum InstantRendererSyncError
        {
            None,
            InstantRendererNull, 
            StorageTerrainCellsNull,
            NotAllProtoAvailable,
            MissingPrototypes
        }

        private static InstantRenderer s_instantRenderer;
        private static StorageTerrainCells s_storageTerrainCells;

        public static InstantRendererSyncError SyncError = InstantRendererSyncError.None;

        public static InstantRenderer InstantRenderer
        {
            get
            {
                if(s_instantRenderer == null)
                {
                    s_instantRenderer = (InstantRenderer)MonoBehaviour.FindObjectOfType(typeof(InstantRenderer));
                }
                
                return s_instantRenderer;

            }
            set
            {
                s_instantRenderer = value;
            }
        }

        public static StorageTerrainCells StorageTerrainCells
        {
            get
            {
                if(s_storageTerrainCells == null)
                {
                    s_storageTerrainCells = (StorageTerrainCells)MonoBehaviour.FindObjectOfType(typeof(StorageTerrainCells));
                }
                
                return s_storageTerrainCells;
            }
            set
            {
                s_storageTerrainCells = value;
            }
        }

        public static void CreateInstantRenderer()
        {
#if UNITY_EDITOR
            MegaWorldGUIUtility.CallMenu("GameObject/Instant Renderer/Add Instant Renderer");
#endif

            SetInstantRendererInfo();
        }

        public static void AddStorageTerrainCells()
        {
            StorageTerrainCells storageTerrainCells = InstantRenderer.gameObject.AddComponent<StorageTerrainCells>();
			storageTerrainCells.CreateCells();

            InstantRenderer.DetectAdditionalData();
        }

        public static void SetInstantRendererInfo()
        {
            InstantRenderer = (InstantRenderer)MonoBehaviour.FindObjectOfType(typeof(InstantRenderer));
            StorageTerrainCells = (StorageTerrainCells)MonoBehaviour.FindObjectOfType(typeof(StorageTerrainCells));
        }

        public static void UpdateInstantRenderer(Group group)
        {
            List<PrototypeInstantItem> protoInstantItemRemoveList = new List<PrototypeInstantItem>();

            List<InstantPrototype> InstantPrototypeList = new List<InstantPrototype>(InstantRenderer.InstantPrototypesPackage.PrototypeList);

            foreach (PrototypeInstantItem proto in group.ProtoInstantItemList)
            {
                bool find = false;

                for (int Id = 0; Id < InstantPrototypeList.Count; Id++)
                {
                    if (CommonUtility.IsSameGameObject(InstantPrototypeList[Id].PrefabObject, proto.Prefab, false))
                    {
                        find = true;
                    }
                }

                if(find == false)
                {
                    protoInstantItemRemoveList.Add(proto);
                }
            }

            foreach (PrototypeInstantItem proto in protoInstantItemRemoveList)
            {
                group.ProtoInstantItemList.Remove(proto);
            }

            InstantPrototype InstantPrototype;

            for (int Id = 0; Id < InstantPrototypeList.Count; Id++)
            {
                bool find = false;

                foreach (PrototypeInstantItem proto in group.ProtoInstantItemList)
                {
                    if (CommonUtility.IsSameGameObject(InstantPrototypeList[Id].PrefabObject, proto.Prefab, false))
                    {
                        find = true;
                    }
                }

                if(find == false)
                {
                    InstantPrototype = InstantPrototypeList[Id];
    
                    group.ProtoInstantItemList.Add(PrototypeInstantItem.Create(group, InstantPrototype.PrefabObject, InstantPrototype.Bounds.size / 2));
                }
            }
        }

        public static bool IsSyncID(Group group)
        {
            foreach (PrototypeInstantItem proto in group.ProtoInstantItemList)
            {
                if(InstantRenderer.InstantPrototypesPackage.GetInstantItem(proto.ID) == null)
                {
                    if(IsAllPrefabAvailable(group) == false)
                    {
                        return false;
                    }
                    else
                    {
                        SyncID(group);
                    }
                }
            }

            return true;
        }

        public static bool IsAllPrefabAvailable(Group group)
        {
            foreach (PrototypeInstantItem proto in group.ProtoInstantItemList)
            {
                if(InstantRenderer?.InstantPrototypesPackage.GetInstantItem(proto.Prefab) == null)
                {
                    return false;
                }
            }

            return true;
        }

        public static void SyncID(Group group)
        {
            foreach (PrototypeInstantItem proto in group.ProtoInstantItemList)
            {
                if(InstantRenderer.InstantPrototypesPackage.GetInstantItem(proto.Prefab) != null)
                {
                    proto.ID = InstantRenderer.InstantPrototypesPackage.GetInstantItem(proto.Prefab).ID;
                }
            }
        }

        public static void AddMissingInstantPrototype(List<PrototypeInstantItem> protoInstantItemList)
        {
            if(!CheckMissingData())
            {
                return;
            }

            foreach (PrototypeInstantItem protoInstantItem in protoInstantItemList)
            {
                InstantPrototype InstantPrototype = InstantRenderer.InstantPrototypesPackage.GetInstantItem(protoInstantItem.ID);

                if(InstantPrototype == null)
                {
                    InstantRenderer.InstantPrototypesPackage.AddPrototype(InstantRenderer, protoInstantItem.Prefab, InstantRenderer.InstantRendererCamera.Count, protoInstantItem.ID);
                }
            }
        }

        public static void RemoveAllInstantPrototype()
        {
            if(!CheckMissingData())
            {
                return;
            }

            InstantRenderer.InstantPrototypesPackage.RemovePrototypes(InstantRenderer);
        }

        public static void AddItemToStorageTerrainCells(int ID, InstanceData instanceData)
        {
            if(!CheckMissingData())
            {
                return;
            }

            StorageTerrainCellsAPI.AddItemInstance(StorageTerrainCells, ID, instanceData.Position, instanceData.Scale, instanceData.Rotation);
        }

        public static void DetectSyncError(Group group)
        {
            if(group.GetPrototypes().Count == 0)
            {
                SyncError = InstantRendererSyncError.MissingPrototypes;
                return;
            }

            if(InstantRenderer == null)
            {
                SyncError = InstantRendererSyncError.InstantRendererNull;
            }
			else if(StorageTerrainCells == null)
			{
                SyncError = InstantRendererSyncError.StorageTerrainCellsNull;
			}
			else if(IsSyncID(group) == false)
			{
                SyncError = InstantRendererSyncError.NotAllProtoAvailable;
            }
            else
            {
                SyncError = InstantRendererSyncError.None;
            }
        }

        public static bool IsSyncError(Group group)
        {
            DetectSyncError(group);

            switch (InstantRendererController.SyncError)
			{
				case InstantRendererSyncError.InstantRendererNull:
				{
					Debug.LogWarning("There is no Instant Renderer in the scene. Click the button \"Create Instant Renderer\"");
					return false;
				}
				case InstantRendererSyncError.StorageTerrainCellsNull:
				{
					Debug.LogWarning("There is no Storage Terrain Cells in the scene. Click the button \"Add Storage Terrain Cells\"");
                    return false;
				}
				case InstantRendererSyncError.NotAllProtoAvailable:
				{
					Debug.LogWarning("You need all prototypes of this group to be in Instant Renderer.");
					return false;
				}
			}

            return true;
        }

        public static bool CheckMissingData()
        {
            if(StorageTerrainCells == null || InstantRenderer == null)
            {
                SetInstantRendererInfo();
                return false;
            }

            return true;
        }
    }
}
#endif