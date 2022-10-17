#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VladislavTsurikov.UndoSystem;
using VladislavTsurikov.RaycastEditorSystem;

namespace VladislavTsurikov.MegaWorldSystem
{
    [InitializeOnLoad]
    public static class СallUndoDelegate 
    {
        static СallUndoDelegate()
        {
            DestroyedGameObject.UndoPerformed -= DestroyedGameObjectUndoPerformed;
            DestroyedGameObject.UndoPerformed += DestroyedGameObjectUndoPerformed;
        }

        private static void DestroyedGameObjectUndoPerformed(List<GameObject> gameObjectList)
        {
            foreach (GameObject go in gameObjectList)
            {
                RaycastEditor.RegisterGameObject(go);
                MegaWorldPath.GameObjectStoragePackage.Storage.AddInstance(GetPrototype.GetCurrentPrototypeGameObject(go).ID, go);
            }
        }
    }
}
#endif