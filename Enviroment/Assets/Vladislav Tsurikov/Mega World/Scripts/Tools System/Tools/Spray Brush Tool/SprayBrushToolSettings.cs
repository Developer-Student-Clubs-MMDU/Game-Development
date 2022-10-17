using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VladislavTsurikov.MegaWorldSystem.SprayBrush
{
    [Serializable]
    public class SprayBrushToolSettings : ScriptableObject
    {
        public BrushSettings BrushSettings = new BrushSettings();

        public void Save() 
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
    }
}