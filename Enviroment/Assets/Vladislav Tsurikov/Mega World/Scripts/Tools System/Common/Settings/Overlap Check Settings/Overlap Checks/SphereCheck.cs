using UnityEngine;
using System;
using VladislavTsurikov.Extensions;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class SphereCheck
    {
        public bool VegetationMode = true;
        public float Size = 3.5f;

        public int Priority = 0;
        public float ViabilitySize = 4f;
        public float TrunkSize = 0.8f;

        public SphereCheck()
        {

        }

        public SphereCheck(SphereCheck other)
        {
            CopyFrom(other);
        }

        public void CopyFrom(SphereCheck other)
        {            
            VegetationMode = other.VegetationMode;
            Size = other.Size;
            Priority = other.Priority;
            ViabilitySize = other.ViabilitySize;
            TrunkSize = other.TrunkSize;
        }

        public bool OverlapCheck(SphereCheck sphereCheck, Vector3 position, Vector3 scale, Bounds checkBounds)
        {
            if(VegetationMode)
            {
                if(sphereCheck.Priority >= Priority)
                {
                    Bounds itemViabilityBounds = new Bounds(position, new Vector3(sphereCheck.ViabilitySize * scale.x, sphereCheck.ViabilitySize * scale.x, sphereCheck.ViabilitySize * scale.x));

                    if(checkBounds.Intersects(itemViabilityBounds))
                    {
                        return true;
                    }
                }

                Bounds itemTrunkBounds = new Bounds(position, new Vector3(sphereCheck.TrunkSize, sphereCheck.TrunkSize * scale.x, sphereCheck.TrunkSize * scale.x));

                if(checkBounds.Intersects(itemTrunkBounds))
                {
                    return true;
                }
            }
            else
            {
                Bounds itemBounds = new Bounds(position, new Vector3(sphereCheck.Size * scale.x, sphereCheck.Size * scale.x, sphereCheck.Size * scale.x));

                if(checkBounds.Intersects(itemBounds))
                {
                    return true;
                }
            }

            return false;
        }

#if UNITY_EDITOR
        public static void DrawOverlapСheck(Vector3 position, Vector3 scale, SphereCheck vegetationCheck)
        {
            if(vegetationCheck.VegetationMode)
            {
                float trunkSize = (vegetationCheck.TrunkSize * scale.x) / 2;
                float viabilitySize = (vegetationCheck.ViabilitySize * scale.x) / 2;
                
                Handles.color = Color.red;
                DrawHandles.CircleCap(1, position, Quaternion.LookRotation(Vector3.up), trunkSize);

                Handles.color = Color.red.WithAlpha(0.1f);
                Handles.DrawSolidDisc(position, Vector3.up, trunkSize);

                Handles.color = Color.blue;
                DrawHandles.CircleCap(1, position, Quaternion.LookRotation(Vector3.up), viabilitySize);

                Handles.color = Color.blue.WithAlpha(0.1f);
                Handles.DrawSolidDisc(position, Vector3.up, viabilitySize);
            }
            else
            {
                float size = (vegetationCheck.Size * scale.x) / 2;

                Handles.color = Color.red;
                DrawHandles.CircleCap(1, position, Quaternion.LookRotation(Vector3.up), size);

                Handles.color = Color.red.WithAlpha(0.1f);
                Handles.DrawSolidDisc(position, Vector3.up, size);
            }
        }
#endif

        public static Bounds GetBounds(SphereCheck vegetationCheck, Vector3 position, Vector3 scale)
        {
            Bounds bounds = new Bounds();
            bounds.center = position;   
            bounds.size = new Vector3(vegetationCheck.Size * scale.x, vegetationCheck.Size * scale.x, vegetationCheck.Size * scale.x);

            if(vegetationCheck.VegetationMode)
            {
                bounds.size = new Vector3(vegetationCheck.ViabilitySize * scale.x, vegetationCheck.ViabilitySize * scale.x, vegetationCheck.ViabilitySize * scale.x);
            }

            return bounds;
        }
    }
}
