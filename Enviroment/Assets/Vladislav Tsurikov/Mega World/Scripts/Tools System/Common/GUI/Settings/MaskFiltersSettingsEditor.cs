#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.CustomGUI;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class MaskFiltersSettingsEditor 
    {
        private bool maskFiltersSettingsFoldout = true;

        public void OnGUI(MaskFiltersSettings brushMaskFiltersSettings)
        {
            BrushMaskFiltersSettings(brushMaskFiltersSettings);
        }

        public void BrushMaskFiltersSettings(MaskFiltersSettings brushMaskFiltersSettings)
		{
			maskFiltersSettingsFoldout = CustomEditorGUILayout.Foldout(maskFiltersSettingsFoldout, "Mask Filters Settings");

			if(maskFiltersSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				brushMaskFiltersSettings.ColorSpace = (ColorSpaceForBrushMaskFilter)CustomEditorGUILayout.EnumPopup(new GUIContent("Color Space"), brushMaskFiltersSettings.ColorSpace);
				
				switch (brushMaskFiltersSettings.ColorSpace)
				{
					case ColorSpaceForBrushMaskFilter.СustomColor:
					{
						brushMaskFiltersSettings.Color = CustomEditorGUILayout.ColorField(new GUIContent("Color"), brushMaskFiltersSettings.Color);
						brushMaskFiltersSettings.EnableStripe = CustomEditorGUILayout.Toggle(new GUIContent("Enable Brush Stripe"), brushMaskFiltersSettings.EnableStripe);

						brushMaskFiltersSettings.AlphaVisualisationType = (AlphaVisualisationType)CustomEditorGUILayout.EnumPopup(new GUIContent("Alpha Visualisation Type"), brushMaskFiltersSettings.AlphaVisualisationType);
						
						break;
					}
					case ColorSpaceForBrushMaskFilter.Colorful:
					{							
						brushMaskFiltersSettings.AlphaVisualisationType = (AlphaVisualisationType)CustomEditorGUILayout.EnumPopup(new GUIContent("Alpha Visualisation Type"), brushMaskFiltersSettings.AlphaVisualisationType);

						break;
					}
					case ColorSpaceForBrushMaskFilter.Heightmap:
					{
						brushMaskFiltersSettings.AlphaVisualisationType = (AlphaVisualisationType)CustomEditorGUILayout.EnumPopup(new GUIContent("Alpha Visualisation Type"), brushMaskFiltersSettings.AlphaVisualisationType);

						break;
					}
				}

				brushMaskFiltersSettings.CustomAlpha = CustomEditorGUILayout.Slider(new GUIContent("Alpha"), brushMaskFiltersSettings.CustomAlpha, 0, 1);
				brushMaskFiltersSettings.EnableDefaultPreviewMaterial = CustomEditorGUILayout.Toggle(new GUIContent("Enable Default Brush Preview Material"), brushMaskFiltersSettings.EnableDefaultPreviewMaterial);
				
				EditorGUI.indentLevel--;
			}
		}
    }
}
#endif