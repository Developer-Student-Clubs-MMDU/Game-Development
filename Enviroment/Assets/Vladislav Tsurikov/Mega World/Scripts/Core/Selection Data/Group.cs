using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using VladislavTsurikov.Extensions;

namespace VladislavTsurikov.MegaWorldSystem
{
    public enum FilterType
    {
        SimpleFilter,
        MaskFilter
    }

    public enum ResourceType 
    {  
        InstantItem,
        GameObject, 
        TerrainDetail,
        TerrainTexture
    }

    public class Group : ScriptableObject
    {
        #region Main Properties
        public string Name = "Default";
        public bool Selected = false;

        public ResourceType ResourceType = ResourceType.InstantItem; 

        public List<PrototypeInstantItem> ProtoInstantItemList = new List<PrototypeInstantItem>();
        public List<PrototypeGameObject> ProtoGameObjectList = new List<PrototypeGameObject>();
        public List<PrototypeTerrainDetail> ProtoTerrainDetailList = new List<PrototypeTerrainDetail>(); 
        public List<PrototypeTerrainTexture> ProtoTerrainTextureList = new List<PrototypeTerrainTexture>();
        #endregion

        #region GUI Properties
        public Vector2 PrototypeWindowsScroll = Vector2.zero;
        public string RenamingName = "Default";
        public bool Renaming = false;
        #endregion

        public ToolSettingsStack ToolSettingsStack = new ToolSettingsStack();
        public SettingsStack SettingsStack = new SettingsStack();

        #region Tools Properties
        public FilterType FilterType = FilterType.MaskFilter;
        public GameObject ContainerForGameObjects;
        #endregion

        #region Stamper Tool Properties
        public int RandomSeed = 0;
        public bool GenerateRandomSeed = false;
        #endregion

        public void OnEnable() 
        {
            AllAvailableTypes.AddType(this);
        }

        public void DropOperation(object[] objectReferences)
        {
            switch (ResourceType)
            {
                case ResourceType.InstantItem:
                {
                    DropOperationInstantItem(objectReferences);
                    break;
                }
                case ResourceType.GameObject:
                {
                    DropOperationGameObject(objectReferences);
                    break;
                }
                case ResourceType.TerrainDetail:
                {
                    DropOperationTerrainDetail(objectReferences);
                    break;
                }
                case ResourceType.TerrainTexture:
                {
                    DropOperationTerrainTexture(objectReferences);
                    break;
                }
            }
        }

        public void DropOperationInstantItem(object[] objectReferences)
        {
#if UNITY_EDITOR
            List<GameObject> draggedGameObjects = new List<GameObject>();

            foreach (UnityEngine.Object draggedObject in objectReferences)
            {
                if (draggedObject is GameObject && 
                    PrefabUtility.GetPrefabAssetType(draggedObject as GameObject) != PrefabAssetType.NotAPrefab &&
                    AssetDatabase.Contains(draggedObject))
                {
					draggedGameObjects.Add((GameObject)draggedObject);   
                }
            }

            foreach (GameObject draggedGameObject in draggedGameObjects)
            {
                Bounds localBounds = draggedGameObject.GetInstantiatedBounds();
                PrototypeInstantItem proto = PrototypeInstantItem.Create(this, draggedGameObject, localBounds.size / 2);
                ProtoInstantItemList.Add(proto);
            }
#endif 
        }

        public void DropOperationGameObject(object[] objectReferences)
        {
#if UNITY_EDITOR
            List<GameObject> draggedGameObjects = new List<GameObject>();

            foreach (UnityEngine.Object draggedObject in objectReferences)
            {
                if (draggedObject is GameObject && 
                    PrefabUtility.GetPrefabAssetType(draggedObject as GameObject) != PrefabAssetType.NotAPrefab &&
                    AssetDatabase.Contains(draggedObject))
                {
					draggedGameObjects.Add((GameObject)draggedObject);   
                }
            }

            foreach (GameObject draggedGameObject in draggedGameObjects)
            {
                Bounds localBounds = draggedGameObject.GetInstantiatedBounds();
                PrototypeGameObject proto = PrototypeGameObject.Create(this, draggedGameObject, localBounds.size / 2);
                ProtoGameObjectList.Add(proto) ;
            }       
#endif 
        }

        public void DropOperationTerrainDetail(object[] objectReferences)
        {
#if UNITY_EDITOR
            foreach (UnityEngine.Object draggedObject in objectReferences)
            {
                if (draggedObject is GameObject && 
                    PrefabUtility.GetPrefabAssetType(draggedObject as GameObject) != PrefabAssetType.NotAPrefab &&
                    AssetDatabase.Contains(draggedObject))
                {
                    GameObject go = (GameObject)draggedObject;
                    PrototypeTerrainDetail proto = PrototypeTerrainDetail.Create(this, go);
					ProtoTerrainDetailList.Add(proto);
                }
				if (draggedObject is Texture2D)
                {
                    Texture2D texture2D = (Texture2D)draggedObject;

                    PrototypeTerrainDetail proto = PrototypeTerrainDetail.Create(this, texture2D);
					ProtoTerrainDetailList.Add(proto);
                }
            }
#endif
        }

        public void DropOperationTerrainTexture(object[] objectReferences)
        {
            foreach (UnityEngine.Object draggedObject in objectReferences)
            {
				if (draggedObject is Texture2D)
                {
                    Texture2D texture2D = (Texture2D)draggedObject;

                    PrototypeTerrainTexture proto = PrototypeTerrainTexture.Create(this, texture2D);
					ProtoTerrainTextureList.Add(proto);
                }
				else if(draggedObject is TerrainLayer)
				{
                    TerrainLayer terrainLayer = (TerrainLayer)draggedObject;

                    PrototypeTerrainTexture proto = PrototypeTerrainTexture.Create(this, terrainLayer);
					ProtoTerrainTextureList.Add(proto);
				}
            }
        }

        public List<PrototypeGameObject> GetAllSelectedGameObject()
        {
            List<PrototypeGameObject> protoList = new List<PrototypeGameObject>();

            foreach (PrototypeGameObject proto in ProtoGameObjectList)
            {
                if(proto.Selected)
                {
                    protoList.Add(proto);
                }
            }

            return protoList;
        }

        public List<PrototypeInstantItem> GetAllSelectedInstantItem()
        {
            List<PrototypeInstantItem> protoList = new List<PrototypeInstantItem>();

            foreach (PrototypeInstantItem proto in ProtoInstantItemList)
            {
                if(proto.Selected)
                {
                    protoList.Add(proto);
                }
            }

            return protoList;
        }

        public List<PrototypeTerrainDetail> GetAllSelectedUnityTerrainDetail()
        {
            List<PrototypeTerrainDetail> protoList = new List<PrototypeTerrainDetail>();

            foreach (PrototypeTerrainDetail proto in ProtoTerrainDetailList)
            {
                if(proto.Selected)
                {
                    protoList.Add(proto);
                }
            }

            return protoList;
        }

        public bool СontainsPrototype()
        {
            switch (ResourceType)
            {
                case ResourceType.InstantItem:
                {
                    return ProtoInstantItemList.Count > 0;
                }
                case ResourceType.GameObject:
                {
                    return ProtoGameObjectList.Count > 0;
                }
                case ResourceType.TerrainDetail:
                {
                    return ProtoTerrainDetailList.Count > 0;
                }
                case ResourceType.TerrainTexture:
                {
                    return ProtoTerrainTextureList.Count > 0;
                }
            }

            return false;
        }

        public List<Prototype> GetPrototypes()
        {
            switch (ResourceType)
            {
                case ResourceType.InstantItem:
                {
                    List<Prototype> protoList = new List<Prototype>();
                    protoList.AddRange(ProtoInstantItemList);

                    return protoList;
                }
                case ResourceType.GameObject:
                {
                    List<Prototype> protoList = new List<Prototype>();
                    protoList.AddRange(ProtoGameObjectList);

                    return protoList;
                }
                case ResourceType.TerrainDetail:
                {
                    List<Prototype> protoList = new List<Prototype>();
                    protoList.AddRange(ProtoTerrainDetailList);

                    return protoList;
                }
                case ResourceType.TerrainTexture:
                {
                    List<Prototype> protoList = new List<Prototype>();
                    protoList.AddRange(ProtoTerrainTextureList);

                    return protoList;
                }
            }

            return null;
        }

        public void SetPrototypes(List<Prototype> protoList)
        {
            switch (ResourceType)
            {
                case ResourceType.InstantItem:
                {
                    ProtoInstantItemList.Clear();

                    foreach (Prototype proto in protoList)
                    {
                        ProtoInstantItemList.Add((PrototypeInstantItem)proto);
                    }

                    break;;
                }
                case ResourceType.GameObject:
                {
                    ProtoGameObjectList.Clear();

                    foreach (Prototype proto in protoList)
                    {
                        ProtoGameObjectList.Add((PrototypeGameObject)proto);
                    }

                    break;
                }
                case ResourceType.TerrainDetail:
                {
                    ProtoTerrainDetailList.Clear();

                    foreach (Prototype proto in protoList)
                    {
                        ProtoTerrainDetailList.Add((PrototypeTerrainDetail)proto);
                    }

                    break;
                }
                case ResourceType.TerrainTexture:
                {
                    ProtoTerrainTextureList.Clear();

                    foreach (Prototype proto in protoList)
                    {
                        ProtoTerrainTextureList.Add((PrototypeTerrainTexture)proto);
                    }

                    break;
                }
            }
        }

        public BaseSettings GetSettings(System.Type settingsType)
        {
            return SettingsStack.GetSettings(settingsType);
        }

        public BaseSettings GetSettings(System.Type toolType, System.Type settingsType)
        {
            return ToolSettingsStack.GetSettings(toolType, settingsType);
        }

        public void ChangeRandomSeed() 
        {
            UnityEngine.Random.InitState(UnityEngine.Random.Range(0, int.MaxValue));
        }

        public void Save() 
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
    }
}