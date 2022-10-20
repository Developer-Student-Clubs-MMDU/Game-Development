#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using VladislavTsurikov.Extensions;

namespace VladislavTsurikov.MegaWorldSystem
{
    [ExecuteInEditMode]
    public class StorageDebug : MonoBehaviour
    {
        private void OnDrawGizmosSelected() 
        {
            MegaWorldPath.GameObjectStoragePackage.Storage.ShowCells();
        }
    }
}
#endif