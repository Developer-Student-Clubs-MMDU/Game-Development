using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VladislavTsurikov.MegaWorldSystem.SprayBrush
{
    [Serializable]
    public class BrushSettings 
    {
        [SerializeField]
        private float _brushSpacing = 30;

        [SerializeField]
        private float _brushSize = 100;

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

        public void OnGUI()
        {
            BrushSettingsEditor.OnGUI(this);
        }
#endif

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

            areaVariables.Mask = Texture2D.whiteTexture;
            areaVariables.Size = BrushSize;
            areaVariables.TerrainUnderCursor = CommonUtility.GetTerrain(hit.Point);
            areaVariables.RayHit = hit;
            areaVariables.Bounds = new Bounds();
            areaVariables.Bounds.size = new Vector3(areaVariables.Size, areaVariables.Size, areaVariables.Size);
            areaVariables.Bounds.center = hit.Point;

            return areaVariables;
        }
    }
}
