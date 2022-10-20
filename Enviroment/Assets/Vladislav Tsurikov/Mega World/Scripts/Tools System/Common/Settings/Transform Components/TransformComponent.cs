using System;
using UnityEngine;
using UnityEditor;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class TransformComponent : ScriptableObject
    {
        public bool Enabled = true;
        public bool FoldoutGUI = true;
        
        public virtual void SetInstanceData(ref InstanceData instanceData, float fitness, Vector3 normal) {}
#if UNITY_EDITOR
        public virtual void DoGUI(Rect rect, int index) {}
        public virtual float GetElementHeight(int index) => EditorGUIUtility.singleLineHeight * 2;
#endif
    }
}
