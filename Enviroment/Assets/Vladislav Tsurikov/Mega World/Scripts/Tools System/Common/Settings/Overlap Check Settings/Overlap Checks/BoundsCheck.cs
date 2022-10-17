using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VladislavTsurikov.MegaWorldSystem
{
    public enum BoundsCheckType 
    { 
        Custom,
        BoundsPrefab
    }
    
    [Serializable]
    public class BoundsCheck
    {
        public BoundsCheckType BoundsType = BoundsCheckType.BoundsPrefab;
        public bool UniformBoundsSize = false;
        public Vector3 BoundsSize = Vector3.one;
        public float MultiplyBoundsSize = 1;

        public BoundsCheck()
        {

        }

        public BoundsCheck(BoundsCheck other)
        {
            CopyFrom(other);
        }

        public void CopyFrom(BoundsCheck other)
        {            
            BoundsType = other.BoundsType;
            UniformBoundsSize = other.UniformBoundsSize;
            MultiplyBoundsSize = other.MultiplyBoundsSize;
        }

#if UNITY_EDITOR
        public static void DrawIntersectionСheckType(Vector3 position, Vector3 scale, Vector3 extents, BoundsCheck boundsCheck)
        {
            Bounds bounds = BoundsCheck.GetBounds(boundsCheck, position, scale, extents);
            Handles.color = Color.red;
            Handles.DrawWireCube(bounds.center, bounds.size);
        }
#endif

        public static Bounds GetBounds(BoundsCheck boundsCheck, Vector3 position, Vector3 scaleFactor, Vector3 extents)
        {
            Vector3 boundsSize = Vector3.zero;

            if(boundsCheck.BoundsType == BoundsCheckType.Custom)
            {
                if(boundsCheck.UniformBoundsSize)
                {
                    boundsSize.x = boundsCheck.BoundsSize.x;
                    boundsSize.y = boundsCheck.BoundsSize.x;
                    boundsSize.z = boundsCheck.BoundsSize.x;
                }
                else
                {
                    boundsSize = boundsCheck.BoundsSize;
                }
            }
            else if(boundsCheck.BoundsType == BoundsCheckType.BoundsPrefab)
            {
                boundsSize.x = scaleFactor.x * (extents.x * 2);
                boundsSize.y = scaleFactor.y * (extents.y * 2);
                boundsSize.z = scaleFactor.z * (extents.z * 2);
            }

            boundsSize.x *= boundsCheck.MultiplyBoundsSize;
            boundsSize.y *= boundsCheck.MultiplyBoundsSize;
            boundsSize.z *= boundsCheck.MultiplyBoundsSize;

            position = new Vector3(position.x , position.y + (boundsSize.y / 2), position.z);

            Bounds bounds = new Bounds();
            bounds.center = position;   
            bounds.size = new Vector3(boundsSize.x, boundsSize.y, boundsSize.z);

            return bounds;
        }
    }
}