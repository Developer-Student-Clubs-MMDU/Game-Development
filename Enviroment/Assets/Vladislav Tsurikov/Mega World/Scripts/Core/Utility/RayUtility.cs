using System.Collections.Generic;
using UnityEngine;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    public static class RayUtility 
    {
        public static Ray GetRayDown(Vector3 point)
        {
            return new Ray(new Vector3(point.x, point.y + MegaWorldPath.AdvancedSettings.EditorSettings.raycastSettings.Offset, point.z), Vector3.down);
        }

        public static Ray GetRayFromCameraPosition(Vector3 point)
        {
            return new Ray(Camera.current.transform.position, (point - Camera.current.transform.position).normalized);
        }
    }
}