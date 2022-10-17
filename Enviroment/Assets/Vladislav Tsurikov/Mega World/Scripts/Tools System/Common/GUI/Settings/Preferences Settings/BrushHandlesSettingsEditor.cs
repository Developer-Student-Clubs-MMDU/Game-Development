#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.CustomGUI;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class BrushHandlesSettingsEditor 
    {
        private bool brushHandlesSettingsFoldout = true;

        public void OnGUI(BrushHandlesSettings brushHandlesSettings)
        {
            BrushHandlesSettings(brushHandlesSettings);
        }

        public void BrushHandlesSettings(BrushHandlesSettings brushHandlesSettings)
		{
			brushHandlesSettingsFoldout = CustomEditorGUILayout.Foldout(brushHandlesSettingsFoldout, "Brush Handles Settings");

			if(brushHandlesSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				brushHandlesSettings.DrawSolidDisc = CustomEditorGUILayout.Toggle(new GUIContent("Draw Solid Disc"), brushHandlesSettings.DrawSolidDisc);
				brushHandlesSettings.CircleColor = CustomEditorGUILayout.ColorField(new GUIContent("Сircle Color"), brushHandlesSettings.CircleColor);       				
				brushHandlesSettings.CirclePixelWidth = CustomEditorGUILayout.Slider(new GUIContent("Сircle Pixel Width"), brushHandlesSettings.CirclePixelWidth, 1f, 5f);

				EditorGUI.indentLevel--;
			}
		}
    }
}
#endif