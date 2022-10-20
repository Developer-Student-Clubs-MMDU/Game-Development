using UnityEngine;
using UnityEditor;

namespace VladislavTsurikov.MegaWorldSystem
{
    public class AdvancedSettings : ScriptableObject
    {
        public EditorSettings EditorSettings = new EditorSettings();
        public VisualisationSettings VisualisationSettings = new VisualisationSettings();

#if UNITY_EDITOR
        public void Save()
        {
            EditorUtility.SetDirty(this);
        }
#endif
    }
}