using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class BrushJitterSettings 
    {
        [SerializeField]
        private float _brushSizeJitter;

        [SerializeField]
        private float _brushScatter;

        [SerializeField]
        private float _brushRotationJitter;

        [SerializeField]
        private float _brushScatterJitter;

        public float BrushSizeJitter
        {
            get
            {
                return _brushSizeJitter;
            }
            set
            {
                _brushSizeJitter = Mathf.Clamp01(value);
            }
        }

        
        public float BrushScatter
        {
            get
            {
                return _brushScatter;
            }
            set
            {
                _brushScatter = Mathf.Clamp01(value);
            }
        }

        
        public float BrushRotationJitter
        {
            get
            {
                return _brushRotationJitter;
            }
            set
            {
                _brushRotationJitter = Mathf.Clamp01(value);
            }
        }

        public float BrushScatterJitter
        {
            get
            {
                return _brushScatterJitter;
            }
            set
            {
                _brushScatterJitter = Mathf.Clamp01(value);
            }
        }

        public AreaVariables GetAreaVariables(BrushSettings brush, Vector3 point, Group group)
        {
            System.Random rand = new System.Random(Time.frameCount);
            AreaVariables areaVariables = new AreaVariables();

            areaVariables.Size = brush.BrushSize;
            areaVariables.Rotation = brush.BrushRotation;
            areaVariables.Mask = brush.GetCurrentRaw();

            areaVariables.Size -= (brush.BrushRadius * BrushSizeJitter * (float)rand.NextDouble()) * 2;
            
            areaVariables.Rotation += Mathf.Sign((float)rand.NextDouble() - 0.5f) * brush.BrushRotation * BrushRotationJitter * (float)rand.NextDouble();

            Vector3 scatterDir = new Vector3((float)(rand.NextDouble() * 2 - 1), 0, (float)(rand.NextDouble() * 2 - 1)).normalized;
            float scatterLengthMultiplier = BrushScatter - (float)rand.NextDouble() * BrushScatterJitter;
            float scatterLength = brush.BrushRadius * scatterLengthMultiplier;

            point += scatterDir * scatterLength;

            LayerSettings layerSettings = MegaWorldPath.CommonDataPackage.layerSettings;

            areaVariables.RayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(point), layerSettings.GetCurrentPaintLayers(group.ResourceType));

            areaVariables.SizeMultiplier = Mathf.Abs(areaVariables.CosAngle = Mathf.Cos(areaVariables.Rotation * Mathf.Deg2Rad));
            areaVariables.SizeMultiplier += Mathf.Abs(areaVariables.SinAngle = Mathf.Sin(areaVariables.Rotation * Mathf.Deg2Rad));

            if(areaVariables.RayHit != null)
            {
                areaVariables.TerrainUnderCursor = CommonUtility.GetTerrain(areaVariables.RayHit.Point);

                areaVariables.Bounds = new Bounds();
                areaVariables.Bounds.size = new Vector3(areaVariables.Size, areaVariables.Size, areaVariables.Size);
                areaVariables.Bounds.center = areaVariables.RayHit.Point;
            }

            return areaVariables;
        }

        public static float GetAlpha(AreaVariables areaVariables, Vector2 pos, Vector2 brushSize)
        {
            if (areaVariables.Mask == null) { return 1.0f; }
            pos += Vector2Int.one;
            if (areaVariables.Rotation == 0.0f) { return GetAlphaRaw(pos, brushSize, areaVariables.Mask); }
            Vector2 halfTarget = (Vector2)brushSize / 2.0f;
            Vector2 origin = pos - halfTarget;
            origin *= areaVariables.SizeMultiplier;
            origin = new Vector2(
                origin.x * areaVariables.CosAngle - origin.y * areaVariables.SinAngle + halfTarget.x,
                origin.x * areaVariables.SinAngle + origin.y * areaVariables.CosAngle + halfTarget.y);

            if (origin.x < 0.0f || origin.x > brushSize.x || origin.y < 0.0f || origin.y > brushSize.y) { return 0.0f; }

            return GetAlphaRaw(origin, brushSize, areaVariables.Mask);
        }

        public static float GetAlphaRaw(Vector2 pos, Vector2 target, Texture2D mask)
        {
            if (mask == null) { return 1.0f; }
	        float x = (pos.x - 1) / target.x;
	        float y = (pos.y - 1) / target.y;

            return mask.GetPixelBilinear(x, y).grayscale;
        }
    }
}