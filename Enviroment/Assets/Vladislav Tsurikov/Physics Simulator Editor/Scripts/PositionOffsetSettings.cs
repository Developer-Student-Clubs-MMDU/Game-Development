#if UNITY_EDITOR
using UnityEngine;
using System;
using VladislavTsurikov.Extensions;

namespace VladislavTsurikov.PhysicsSimulatorEditor
{
    [Serializable]
    public class PositionOffsetSettings
    {
        public bool EnableAutoOffset = true;
        public float PositionOffsetDown = 50;
        
        public void ApplyOffset(GameObject go) 
        {
            if(!EnableAutoOffset)
            {
                return;
            }
            
            Bounds bounds = go.GetInstantiatedBounds();



            go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y - Mathf.Lerp(0, bounds.extents.y, PositionOffsetDown / 100), go.transform.position.z);
        }
    }
}
#endif
