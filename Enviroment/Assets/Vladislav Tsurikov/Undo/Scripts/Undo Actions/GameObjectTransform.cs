#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.MegaWorldSystem;

namespace VladislavTsurikov.UndoSystem
{
    public class TransformData
    {
        public GameObject GameObject;
        public InstanceData InstanceData;

        public TransformData(GameObject gameObject)
        {
            GameObject = gameObject;
            InstanceData = new InstanceData(gameObject);
        }

        public void SetTransform()
        {
            GameObject.transform.position = InstanceData.Position;
            GameObject.transform.localScale = InstanceData.Scale; 
            GameObject.transform.rotation = InstanceData.Rotation;
        }
    }

    public class GameObjectTransform : UndoRecord
    {
        private List<TransformData> _transformList = new List<TransformData>();

        public GameObjectTransform(GameObject gameObject) 
        {
            _transformList.Add(new TransformData(gameObject));
        }

        public override void Merge(UndoRecord record)
        {
            if (record is GameObjectTransform)
            {
                GameObjectTransform gameObjectUndo = (GameObjectTransform)record;
                _transformList.AddRange(gameObjectUndo._transformList);
            }
        }

        public override void Undo()
        {
            foreach (TransformData transformUndoData in _transformList)
            {
                if(transformUndoData.GameObject != null)
                {
                    transformUndoData.SetTransform();
                }
            }
        }
    }
}
#endif