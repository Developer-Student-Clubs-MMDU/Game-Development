using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VladislavTsurikov.MegaWorldSystem.GameObjectStorage
{
    [Serializable]
    public class GameObjectInfo
    {
        public int ID;

        [SerializeField]
        public List<GameObject> itemList = new List<GameObject>();

        public void AddPersistentItemInstance(ref GameObject go)
        {
            itemList.Add(go);
        }
    }
}