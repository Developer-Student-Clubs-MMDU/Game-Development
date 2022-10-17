using UnityEngine;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class BrushHandlesSettings 
    {
        public bool DrawSolidDisc = true;
        public Color CircleColor = new Color(0.2f, 0.5f, 0.7f, 1);
        public float CirclePixelWidth = 5f;

#if UNITY_EDITOR
        public BrushHandlesSettingsEditor BrushHandlesSettingsEditor = new BrushHandlesSettingsEditor();

        public void OnGUI()
        {
            BrushHandlesSettingsEditor.OnGUI(this);
        }
#endif
    }
}

