#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VladislavTsurikov.UndoSystem
{
    public class CreatedGameObject : UndoRecord
    {
        private List<GameObject> _gameObjectList = new List<GameObject>();

        public CreatedGameObject(GameObject gameObject) 
        {
            _gameObjectList.Add(gameObject);
        }

        public override void Merge(UndoRecord record)
        {
            if (record is CreatedGameObject)
            {
                CreatedGameObject gameObjectUndo = (CreatedGameObject)record;
                _gameObjectList.AddRange(gameObjectUndo._gameObjectList);
            }
        }

        public override void Undo()
        {
            foreach (GameObject gameObject in _gameObjectList)
            {
                GameObject.DestroyImmediate(gameObject);
            }
        }
    }
}
#endif