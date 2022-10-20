#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.CustomGUI;

namespace VladislavTsurikov.MegaWorldSystem.SprayBrush
{
    [Serializable]
    public class BrushSettingsEditor 
    {
        public bool brushSettingsFoldout = true;

        public void OnGUI(BrushSettings brush)
        {
            brushSettingsFoldout = CustomEditorGUILayout.Foldout(brushSettingsFoldout, "Brush Settings");

			if(brushSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				brush.Spacing = CustomEditorGUILayout.Slider(spacing, brush.Spacing, 0.1f, 5);

                brush.BrushSize = CustomEditorGUILayout.Slider(brushSize, brush.BrushSize, 0.1f, MegaWorldPath.AdvancedSettings.EditorSettings.maxBrushSize);

				EditorGUI.indentLevel--;
			}
        }

        [NonSerialized]
        private GUIContent brushSize = new GUIContent("Brush Size", "Selected prototypes will only spawn in this range around the center of Brush.");
        
		[NonSerialized]
		private GUIContent spacing = new GUIContent("Spacing", "Controls the distance between brush marks.");
    }
}
#endif
