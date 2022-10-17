using UnityEngine;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    public enum ColorHandlesType 
    { 
        Custom, 
        Standard,
    }

    public enum HandleResizingType 
    { 
        Distance, 
        Resolution,
        CustomSize
    }

    public enum HandlesType 
    { 
        Sphere, 
        DotCap 
    }

    [Serializable]
    public class AdvancedSimpleFilterSettings 
    {
        public bool EnableSpawnVisualization = true;
        
        public int VisualiserResolution = 15;
        public float CustomHandleSize = 0.3f;
        
        public HandlesType HandlesType = HandlesType.DotCap;
        public HandleResizingType HandleResizingType = HandleResizingType.Distance;
        public ColorHandlesType ColorHandlesType = ColorHandlesType.Standard;

        [Range (0f, 1f)]
        public float Alpha = 0.6f;
        public Color ActiveColor = Color.green;
        public Color InactiveColor = Color.red;

        #if UNITY_EDITOR
        public AdvancedSimpleFilterSettingsEditor AdvancedSimpleFilterSettingsEditor = new AdvancedSimpleFilterSettingsEditor();

        public void OnGUI()
        {
            AdvancedSimpleFilterSettingsEditor.OnGUI(this);
        }
        #endif
    }
}

