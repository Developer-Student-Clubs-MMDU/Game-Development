using System.Collections.Generic;
using UnityEngine;
using System;

namespace VladislavTsurikov.MegaWorldSystem.GameObjectStorage
{
    [Serializable]
    public class Cell : IHasRect
    {
        public Bounds Bounds;
        public List<GameObjectInfo> GameObjectInfoInfoList = new List<GameObjectInfo>();
        public int Index;

        public Cell(Bounds bounds)
        {
            this.Bounds = bounds;
        }

        public Rect Rectangle
        {
            get
            {
                return RectExtension.CreateRectFromBounds(Bounds);
            }
            set
            {
                Bounds = RectExtension.CreateBoundsFromRect(value);
            }
        }

        public void AddItemInstance(int ID, GameObject go)
        {
            GameObjectInfo persistentInfo = GetPersistentInfo(ID);
            if (persistentInfo == null)
            {
                persistentInfo = new GameObjectInfo 
                {
                    ID = ID
                };
                GameObjectInfoInfoList.Add(persistentInfo);
            }

            persistentInfo.AddPersistentItemInstance(ref go);
        }

        public GameObjectInfo GetPersistentInfo(int ID)
        {
            for (int i = 0; i <= GameObjectInfoInfoList.Count - 1; i++)
            {
                if (GameObjectInfoInfoList[i].ID == ID) 
                {
                    return GameObjectInfoInfoList[i];
                }
            }

            return null;
        }

        public void RemoveNullData()
        {
            foreach (GameObjectInfo gameObjectInfo in GameObjectInfoInfoList)
            {   
                gameObjectInfo.itemList.RemoveAll((go) => go == null);
            }
        }
    }
}