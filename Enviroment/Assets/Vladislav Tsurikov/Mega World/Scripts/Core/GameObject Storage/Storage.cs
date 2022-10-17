using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VladislavTsurikov.MegaWorldSystem.GameObjectStorage
{
    public class Storage
    {
        private QuadTree<Cell> _cellQuadTree;
        private List<Cell> _modifiedСells = new List<Cell>();

        [NonSerialized]
        public List<Cell> CellList = new List<Cell>();
        
        public void RefreshCells(float cellSize)
        {
            GameObject[] sceneObjects = GameObject.FindObjectsOfType<GameObject>();

            CellList = new List<Cell>();

            CreateCells(GetWorldBounds(sceneObjects), cellSize);
            Populate(sceneObjects);
        }

        public void Populate(GameObject[] sceneObjects)
        {
            for (int i = 0; i < sceneObjects.Length; i++)
            {
                if(VladislavTsurikov.GameObjectUtility.GetPrefabRoot(sceneObjects[i]) == null)
                {
                    continue;
                }
                
                if(VladislavTsurikov.GameObjectUtility.GetPrefabRoot(sceneObjects[i]).name != sceneObjects[i].name)
                {
                    continue;
                }

                PrototypeGameObject proto = GetPrototype.GetCurrentPrototypeGameObject(sceneObjects[i]);

                if(proto != null)
                {
                    AddInstance(proto.ID, sceneObjects[i]);
                }
            }
        }

        public void CreateCells(Bounds worldBounds, float cellSize)
        {
            CellList.Clear(); 
            
            Bounds expandedBounds = new Bounds(worldBounds.center, worldBounds.size);
            expandedBounds.Expand(new Vector3(cellSize * 2f, 0, cellSize * 2f));

            Rect expandedRect = RectExtension.CreateRectFromBounds(expandedBounds);

            _cellQuadTree = new QuadTree<Cell>(expandedRect);

            int cellXCount = Mathf.CeilToInt(worldBounds.size.x / cellSize);
            int cellZCount = Mathf.CeilToInt(worldBounds.size.z / cellSize);

            Vector2 corner = new Vector2(worldBounds.center.x - worldBounds.size.x / 2f,
                worldBounds.center.z - worldBounds.size.z / 2f);

            Bounds bounds = new Bounds();

            for (int x = 0; x <= cellXCount - 1; x++)
            {
                for (int z = 0; z <= cellZCount - 1; z++)
                {
                    Rect rect = new Rect(
                        new Vector2(cellSize * x + corner.x, cellSize * z + corner.y),
                        new Vector2(cellSize, cellSize));

                    bounds = RectExtension.CreateBoundsFromRect(rect, worldBounds.center.y, worldBounds.size.y);

                    Cell cell = new Cell(bounds);

                    CellList.Add(cell);
                    cell.Index = CellList.Count - 1;
                    _cellQuadTree.Insert(cell);
                }
            }
        }

        public Bounds GetWorldBounds(GameObject[] sceneObjects)
        {
            Bounds worldBounds = new Bounds(Vector3.zero, Vector3.zero);

            for (int i = 0; i < sceneObjects.Length; i++)
            {
                if(VladislavTsurikov.GameObjectUtility.GetPrefabRoot(sceneObjects[i]) == null)
                {
                    continue;
                }

                if(VladislavTsurikov.GameObjectUtility.GetPrefabRoot(sceneObjects[i]).name != sceneObjects[i].name)
                {
                    continue;
                }

                if (!sceneObjects[i].activeInHierarchy)
                {
                    continue;
                }

                worldBounds.Encapsulate(VladislavTsurikov.GameObjectUtility.GetBoundsFromGameObject(sceneObjects[i]));
            } 

            return worldBounds;
        }

        public void AddInstance(int ID, GameObject go)
        {
            if (_cellQuadTree == null || go == null) 
            {
                return;
            }

            Rect positionRect = new Rect(new Vector2(go.transform.position.x, go.transform.position.z), Vector2.zero);

            List<Cell> overlapCellList = new List<Cell>();                 
            _cellQuadTree.Query(positionRect, overlapCellList);

            for (int i = 0; i <= overlapCellList.Count - 1; i++)
            {
                int cellIndex = overlapCellList[i].Index;
                if (CellList.Count > cellIndex)
                {
                    CellList[cellIndex].AddItemInstance(ID, go);
                }
            }
        }

        public void RemoveNullData(bool checkOnlyModifiedСells)
        {
            if(checkOnlyModifiedСells)
            {
                for (int i = 0; i < _modifiedСells.Count; i++)
                {                  
                    _modifiedСells[i].RemoveNullData();
                }

                _modifiedСells.Clear();
            }
            else
            {
                for (int i = 0; i < CellList.Count; i++)
                {            
                    CellList[i].RemoveNullData();
                }
            }
        }

        public void RepositionObject(GameObject go, int ID, int cellIndex)
        {
            AddInstance(ID, go);

            foreach (GameObjectInfo gameObjectInfo in CellList[cellIndex].GameObjectInfoInfoList)
            {
                if(gameObjectInfo.ID == ID)
                {
                    gameObjectInfo.itemList.Remove(go);
                    break;
                }
            }
        }

        public void RemoveObject(GameObject go, int ID, int cellIndex)
        {
            foreach (GameObjectInfo gameObjectInfo in CellList[cellIndex].GameObjectInfoInfoList)
            {
                if(gameObjectInfo.ID == ID)
                {
                    gameObjectInfo.itemList.Remove(go);
                    break;
                }
            }
        }

        public void IntersectBounds(Bounds bounds, Func<GameObjectInfo, int, bool> func)
        {
            Rect positionRect = RectExtension.CreateRectFromBounds(bounds);

            List<Cell> overlapCellList = new List<Cell>();                 
            _cellQuadTree.Query(positionRect, overlapCellList);

            for (int i = 0; i <= overlapCellList.Count - 1; i++)
            {
                int cellIndex = overlapCellList[i].Index;

                List<GameObjectInfo> persistentInfoList = CellList[cellIndex].GameObjectInfoInfoList;

                for (int persistentInfoIndex = 0; persistentInfoIndex < persistentInfoList.Count; persistentInfoIndex++)
                {
                    GameObjectInfo gameObjectInfo = persistentInfoList[persistentInfoIndex];

                    func.Invoke(gameObjectInfo, cellIndex);
                }
            }
        }

        public void AddModifiedСell(Cell cell)
        {
            if(CellList.Contains(cell) == false)
            {
                _modifiedСells.Add(cell);
            }
        }

#if UNITY_EDITOR
        public void ShowCells()
        {
            Handles.color = Color.yellow;

            for (int i = 0; i < CellList.Count; i++)
            {                  
                Handles.DrawWireCube(CellList[i].Bounds.center, CellList[i].Bounds.size);
            }
        }
#endif
        
    }
}