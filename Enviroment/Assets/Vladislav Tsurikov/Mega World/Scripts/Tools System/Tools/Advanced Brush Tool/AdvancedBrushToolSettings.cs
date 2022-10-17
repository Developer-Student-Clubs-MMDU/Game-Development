using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VladislavTsurikov.MegaWorldSystem.AdvancedBrush
{
    [Serializable]
    public class AdvancedBrushToolSettings : ScriptableObject
    {
        public BrushSettings BrushSettings = new BrushSettings();
        public float TextureTargetStrength = 1.0f;

        public bool EnableFailureRateOnMouseDrag = true;
        public float FailureRate = 50f;

        public void Save() 
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
    }
}