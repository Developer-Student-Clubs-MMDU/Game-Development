using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VladislavTsurikov.MegaWorldSystem.BrushErase
{
    [Serializable]
    public class BrushEraseToolSettings : ScriptableObject
    {   
        public BrushSettings BrushSettingsForErase = new BrushSettings();
        public float EraseStrength = 1.0f;

        public void Save() 
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
    }   
}
