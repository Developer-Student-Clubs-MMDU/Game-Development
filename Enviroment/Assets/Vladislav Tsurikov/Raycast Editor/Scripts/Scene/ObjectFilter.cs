#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using VladislavTsurikov.Extensions;

namespace VladislavTsurikov.RaycastEditorSystem
{
    public class ObjectFilter
    {
        private int _layerMask = ~0;
        private List<GameObject> _ignoreObjects = new List<GameObject>();

        public int LayerMask { get { return _layerMask; } set { _layerMask = value; } }

        public void ClearIgnoreObjects()
        {
            _ignoreObjects.Clear();
        }

        public void SetIgnoreObjects(List<GameObject> ignoreObjects)
        {
            if (ignoreObjects == null) return;
            _ignoreObjects = new List<GameObject>(ignoreObjects);
        }

        public bool IsObjectIgnored(GameObject gameObject)
        {
            return _ignoreObjects.Contains(gameObject);
        }

        public bool Filter(GameObject go)
        {
            if(go == null)
            {
                return false;
            }
            
            if(!LayerEx.IsLayerBitSet(_layerMask, go.layer) || _ignoreObjects.Contains(go))
            {
                return false;
            }

            return true;
        }
    }
}
#endif