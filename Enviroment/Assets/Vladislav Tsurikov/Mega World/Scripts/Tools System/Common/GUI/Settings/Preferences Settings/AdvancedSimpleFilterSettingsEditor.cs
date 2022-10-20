#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.CustomGUI;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class AdvancedSimpleFilterSettingsEditor 
    {
        bool simpleFilterVisualisationSettingsFoldout = true;

        public void OnGUI(AdvancedSimpleFilterSettings simpleFilterSettings)
        {
            SimpleFilterSettings(simpleFilterSettings);
        }

        public void SimpleFilterSettings(AdvancedSimpleFilterSettings simpleFilterSettings)
		{
			simpleFilterVisualisationSettingsFoldout = CustomEditorGUILayout.Foldout(simpleFilterVisualisationSettingsFoldout, "Simple Filter Settings");

			if(simpleFilterVisualisationSettingsFoldout)
			{
				EditorGUI.indentLevel++;
				
				simpleFilterSettings.EnableSpawnVisualization = CustomEditorGUILayout.Toggle(new GUIContent("Enable", "I recommend turning off visualization if rendering slows down performance while spawning"), simpleFilterSettings.EnableSpawnVisualization);

				if(simpleFilterSettings.EnableSpawnVisualization)
				{
					simpleFilterSettings.VisualiserResolution = CustomEditorGUILayout.IntSlider(new GUIContent("Visualiser Resolution"), simpleFilterSettings.VisualiserResolution, 1, 60);
										
					simpleFilterSettings.HandlesType = (HandlesType) CustomEditorGUILayout.EnumPopup(new GUIContent("Handles Type"), simpleFilterSettings.HandlesType);
					simpleFilterSettings.HandleResizingType = (HandleResizingType) CustomEditorGUILayout.EnumPopup(new GUIContent("Handle Resizing Type"), simpleFilterSettings.HandleResizingType);
					
					if(simpleFilterSettings.HandleResizingType == HandleResizingType.CustomSize)
					{
						EditorGUI.indentLevel++;

						simpleFilterSettings.CustomHandleSize = CustomEditorGUILayout.Slider(new GUIContent("Handle Size"), simpleFilterSettings.CustomHandleSize, 0.1f, 3f);
				
						EditorGUI.indentLevel--;
					}

					simpleFilterSettings.ColorHandlesType = (ColorHandlesType) CustomEditorGUILayout.EnumPopup(new GUIContent("Color Handles Type"), simpleFilterSettings.ColorHandlesType);
						
					if(simpleFilterSettings.ColorHandlesType == ColorHandlesType.Custom)
					{
						EditorGUI.indentLevel++;

						simpleFilterSettings.ActiveColor = CustomEditorGUILayout.ColorField(new GUIContent("Active Color"), simpleFilterSettings.ActiveColor);
						simpleFilterSettings.InactiveColor = CustomEditorGUILayout.ColorField(new GUIContent("Inactive Color"), simpleFilterSettings.InactiveColor);

						EditorGUI.indentLevel--;
					}

					simpleFilterSettings.Alpha = CustomEditorGUILayout.Slider(new GUIContent("Alpha"), simpleFilterSettings.Alpha, 0, 1);
				}

				EditorGUI.indentLevel--;
			}
		}
    }
}
#endif