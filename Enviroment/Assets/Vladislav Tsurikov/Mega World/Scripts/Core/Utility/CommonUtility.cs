using System.Collections.Generic;
using UnityEngine;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    public static class CommonUtility
    {        
        public static bool IsSameVector(Vector3 a, Vector3 b, float epsilon = 0.001f)
        {            
            return Mathf.Abs (a.x - b.x) < epsilon && Mathf.Abs (a.y - b.y) < epsilon && Mathf.Abs (a.z - b.z) < epsilon;
        }

        public static bool IsSameTexture(Texture2D tex1, Texture2D tex2, bool checkID = false)
        {
            if (tex1 == null || tex2 == null)
            {
                return false;
            }

            if (checkID)
            {
                if (tex1.GetInstanceID() != tex2.GetInstanceID())
                {
                    return false;
                }
                return true;
            }

            if (tex1.name != tex2.name)
            {
                return false;
            }

            if (tex1.width != tex2.width)
            {
                return false;
            }

            if (tex1.height != tex2.height)
            {
                return false;
            }

            return true;
        }

        public static bool IsSameGameObject(GameObject go1, GameObject go2, bool checkID = false)
        {
            if (go1 == null || go2 == null)
            {
                return false;
            }

            if (checkID)
            {
                if (go1.GetInstanceID() != go2.GetInstanceID())
                {
                    return false;
                }
                return true;
            }

            if (go1.name != go2.name)
            {
                return false;
            }

            return true;
        }

        public static float WorldToDetailf(float pos, float terrainSize, TerrainData td)
        {
            return (pos / terrainSize) * td.detailResolution;
        }

        public static int WorldToDetail(float pos, float size, TerrainData td)
        {
            return Mathf.RoundToInt(WorldToDetailf(pos, size, td));
        }

        public static int WorldToDetail(float pos, TerrainData td)
        {
            return WorldToDetail(pos, td.size.x, td);
        }

        public static Vector2 GetTerrainWorldPositionFromRange(Vector2 normal, Terrain terrain)
        {
            Vector2 localTerrainPosition = new Vector2(Mathf.Lerp(0, terrain.terrainData.size.x, normal.x), Mathf.Lerp(0, terrain.terrainData.size.z, normal.y));
            return localTerrainPosition + new Vector2(terrain.GetPosition().x, terrain.GetPosition().z);
        }

        public static Vector2 WorldPointToUV(Vector3 point, Terrain activeTerrain)
        {
            if (activeTerrain == null)
            {
                return Vector2.zero;
            }
                
            Vector3 localPoint = activeTerrain.transform.InverseTransformPoint(point);
            Vector3 terrainSize = new Vector3(activeTerrain.terrainData.size.x, activeTerrain.terrainData.size.y, activeTerrain.terrainData.size.z);
            Vector2 uv = new Vector2(
                InverseLerpUnclamped(0, terrainSize.x, localPoint.x),
                InverseLerpUnclamped(0, terrainSize.z, localPoint.z));

            return uv;
        }

        public static float InverseLerpUnclamped(float a, float b, float value)
        {
            if (a != b)
            {
                return (value - a) / (b - a);
            }
            return 0f;
        }

        public static Terrain GetTerrain(Vector3 location)
        {
            Vector3 terrainMin = new Vector3();
            Vector3 terrainMax = new Vector3();

            for (int idx = 0; idx < Terrain.activeTerrains.Length; idx++)
            {
                Terrain terrain = Terrain.activeTerrains[idx];
                terrainMin = terrain.GetPosition();
                terrainMax = terrainMin + terrain.terrainData.size;
                if (location.x >= terrainMin.x && location.x <= terrainMax.x)
                {
                    if (location.z >= terrainMin.z && location.z <= terrainMax.z)
                    {
                        return terrain;
                    }
                }
            }
            return null;
		}

        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation) 
        {
            return rotation * (point - pivot) + pivot;
        }

        public static void GetOrientation(Vector3 normal, FromDirection mode, float weightToNormal, out Vector3 upwards, out Vector3 right, out Vector3 forward)
        {
            switch (mode)
            {
                case FromDirection.SurfaceNormal:
                    upwards = Vector3.Lerp(Vector3.up, normal, weightToNormal);
                    break;
                case FromDirection.X:
                    upwards = new Vector3(1, 0, 0);
                    break;
                default:
                case FromDirection.Y:
                    upwards = new Vector3(0, 1, 0);
                    break;
                case FromDirection.Z:
                    upwards = new Vector3(0, 0, 1);
                    break;
            }

            TransformAxes.GetRightForward(upwards, out right, out forward);
        }
    } 
}