using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class MaskFilter : ScriptableObject
    {
        public bool Enabled = true;
        public bool FoldoutGUI = true;
        
        public virtual void Eval( MaskFilterContext filterContext, int index) {}
#if UNITY_EDITOR
        public virtual void DoGUI( Rect rect, int index ) {}
        public virtual float GetElementHeight(int index) => EditorGUIUtility.singleLineHeight * 2;
        public virtual string GetAdditionalName() => "";
#endif
    }
}