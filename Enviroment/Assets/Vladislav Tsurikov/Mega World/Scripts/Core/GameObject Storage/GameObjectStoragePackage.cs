using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

namespace VladislavTsurikov.MegaWorldSystem.GameObjectStorage
{
    [Serializable]
    public class GameObjectStoragePackage : ScriptableObject
    {
        [NonSerialized]
        private Storage _storage = null;

        public float CellSize = 100;

        public Storage Storage
        {
            get
            {
                if(_storage == null)
                {
                    _storage = new Storage();
                    _storage.RefreshCells(CellSize);
                }

                return _storage;
            }
            set
            {
                _storage = value;
            }
        }

        public GameObjectStoragePackage()
        {
#if UNITY_EDITOR
            SceneManagement.ActiveSceneChangedInEditMode -= ChangedActiveScene;
            SceneManagement.ActiveSceneChangedInEditMode += ChangedActiveScene;
#endif
        }

#if UNITY_EDITOR
        private void ChangedActiveScene(Scene current, Scene next)
        {
            Storage.RefreshCells(CellSize);
        }
#endif
    }
}