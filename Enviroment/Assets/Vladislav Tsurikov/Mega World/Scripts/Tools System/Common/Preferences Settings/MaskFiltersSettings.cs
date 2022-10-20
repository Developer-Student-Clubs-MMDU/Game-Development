using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    public enum AlphaVisualisationType
    {
        Default,
        BrushFilter,
        None
    }

    [Serializable]
    public class MaskFiltersSettings 
    {
        public Color Color = new Color(128, 171, 78, 255);
        public bool EnableStripe = true;
        public ColorSpaceForBrushMaskFilter ColorSpace = ColorSpaceForBrushMaskFilter.Colorful;
        public bool EnableDefaultPreviewMaterial = false;
        public AlphaVisualisationType AlphaVisualisationType = AlphaVisualisationType.None;
        public float CustomAlpha = 0.3f;

        #if UNITY_EDITOR
        public MaskFiltersSettingsEditor maskFiltersSettingsEditor = new MaskFiltersSettingsEditor();

        public void OnGUI()
        {
            maskFiltersSettingsEditor.OnGUI(this);
        }
        #endif
    }
}

