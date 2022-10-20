using UnityEngine;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class EditorSettings 
    {
        public float maxBrushSize = 200;
        public int maxChecks = 100;

        public RaycastSettings raycastSettings = new RaycastSettings();

#if UNITY_EDITOR
        public EditorSettingsEditor editorSettingsEditor = new EditorSettingsEditor();

        public void OnGUI()
        {
            editorSettingsEditor.OnGUI(this); 
        }
#endif
    }
}
