using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    public enum MaskType
    {
        Custom,
        Procedural
    }

    public enum SpacingEqualsType 
    { 
        BrushSize, 
        HalfBrushSize,
        Custom,
    }


    [Serializable]
    public class BrushSettings 
    {
        [SerializeField]
        private float _brushSpacing = 30;

        [SerializeField]
        private float _brushRotation = 0.0f;

        [SerializeField]
        private float _brushSize = 100;

        private float _brushCosAngle = 0.0f;
        private float _brushSinAngle = 0.0f;
        private float _brushRotationSizeMultiplier = 1.0f;

        public ProceduralMask ProceduralMask = new ProceduralMask();
        public CustomMasks CustomMasks = new CustomMasks();
        public MaskType MaskType = MaskType.Procedural;
        public SpacingEqualsType SpacingEqualsType = SpacingEqualsType.HalfBrushSize;
        public BrushJitterSettings BrushJitterSettings = new BrushJitterSettings();

        public float Spacing
        {
            set
            {
                _brushSpacing = Mathf.Max(0.01f, value);
            }
            get
            {
                return _brushSpacing;
            }
        }

        public float BrushRotation
        {
            get 
            { 
                return _brushRotation; 
            }
            set
            {
                _brushRotation = value;
                _brushRotationSizeMultiplier = Mathf.Abs(_brushCosAngle = Mathf.Cos(_brushRotation * Mathf.Deg2Rad));
                _brushRotationSizeMultiplier += Mathf.Abs(_brushSinAngle = Mathf.Sin(_brushRotation * Mathf.Deg2Rad));
            }
        }

        public float BrushSize
        {
            get
            {
                return _brushSize;
            }
            set
            {
                _brushSize = Mathf.Max(0.01f, value);
            }
        }

        public float BrushRadius
        {
            get
            {
                return _brushSize / 2;
            }
        }

#if UNITY_EDITOR
        public BrushSettingsEditor BrushSettingsEditor = new BrushSettingsEditor();

        public void OnGUI(string content)
        {
            BrushSettingsEditor.OnGUI(this, content);
        }
#endif

        public float GetCurrentSpacing()
        {
            switch (SpacingEqualsType)
            {
                case SpacingEqualsType.BrushSize:
                {
                    return BrushSize;
                }
                case SpacingEqualsType.HalfBrushSize:
                {
                    return BrushSize / 2;
                }
                default:
                {
                    return Mathf.Max(0.01f, Spacing);
                }
            }
        }

        public Texture2D GetCurrentRaw()
        {
            switch (MaskType)
            {
                case MaskType.Custom:
                {
                    Texture2D texture = CustomMasks.GetSelectedBrush();

                    return texture;
                }
                case MaskType.Procedural:
                {
                    Texture2D texture = ProceduralMask.Mask;

                    return texture;
                }
            }

            return Texture2D.whiteTexture;
        }

        public void ScrollBrushRadiusEvent()
        {
            if(Event.current.shift)
            {
                if (Event.current.type == EventType.ScrollWheel) 
                {
                    BrushSize += Event.current.delta.y;
                    Event.current.Use();
			    }
            }
        }

        public AreaVariables GetAreaVariables(RayHit hit)
        {
            AreaVariables areaVariables = new AreaVariables();

            areaVariables.Mask = GetCurrentRaw();
            areaVariables.Size = BrushSize;
            areaVariables.Rotation = BrushRotation;
            areaVariables.TerrainUnderCursor = CommonUtility.GetTerrain(hit.Point);
            areaVariables.RayHit = hit;
            areaVariables.Bounds = new Bounds();
            areaVariables.Bounds.size = new Vector3(areaVariables.Size, areaVariables.Size, areaVariables.Size);
            areaVariables.Bounds.center = hit.Point;

            return areaVariables;
        }
    }
}
