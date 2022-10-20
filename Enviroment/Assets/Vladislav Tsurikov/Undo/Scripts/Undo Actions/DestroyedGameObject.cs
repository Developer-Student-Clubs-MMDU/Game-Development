#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.MegaWorldSystem;
using UnityEditor;
using UnityEngine.Events;

namespace VladislavTsurikov.UndoSystem
{
    public class DestroyedData
    {
        public GameObject Prefab;
        public GameObject Parent;
        public InstanceData InstanceData;

        public DestroyedData(GameObject gameObject, GameObject parent)
        {
            Parent = parent;
            Prefab = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
            InstanceData = new InstanceData(gameObject); 
        }

        public GameObject Instantiate()
        {
            GameObject go;

            go = PrefabUtility.InstantiatePrefab(Prefab) as GameObject;

            go.transform.position = InstanceData.Position;
            go.transform.localScale = InstanceData.Scale;
            go.transform.rotation = InstanceData.Rotation;

            GameObjectUtility.ParentGameObjectIfNecessary(go, Parent);

            return go;
        }
    }

    public class DestroyedGameObject : UndoRecord
    {
        private List<DestroyedData> _destroyDataList = new List<DestroyedData>();
        public static event UnityAction<List<GameObject>> UndoPerformed;

        public DestroyedGameObject(GameObject gameObject) 
        {
            if(gameObject.transform.parent == null)
            {
                _destroyDataList.Add(new DestroyedData(gameObject, null));
            }
            else
            {
                _destroyDataList.Add(new DestroyedData(gameObject, gameObject.transform.parent.gameObject));
            }
        }

        public override void Merge(UndoRecord record)
        {
            if (record is DestroyedGameObject)
            {
                DestroyedGameObject gameObjectUndo = (DestroyedGameObject)record;
                _destroyDataList.AddRange(gameObjectUndo._destroyDataList);
            }
        }

        public override void Undo()
        {
            List<GameObject> gameObjectList = new List<GameObject>();

            foreach (DestroyedData destroyData in _destroyDataList)
            {
                if(destroyData.Prefab != null)
                {
                    gameObjectList.Add(destroyData.Instantiate());
                }
            }

            UndoPerformed(gameObjectList);
        }
    }
}
#endif