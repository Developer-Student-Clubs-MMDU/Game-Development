using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VladislavTsurikov.MegaWorldSystem
{
    public abstract class Scatter : ScriptableObject
    {
        public bool Enabled = true;
        public bool FoldoutGUI = true;

        public abstract void Samples(AreaVariables areaVariables, List<Vector2> samples);
#if UNITY_EDITOR
        public abstract void DoGUI(Rect rect, int index);
        public abstract float GetElementHeight(int index);
#endif
    }
}