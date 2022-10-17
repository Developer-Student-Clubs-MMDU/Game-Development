#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.CustomGUI;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class BrushSettingsEditor 
    {
		public BrushJitterSettingsEditor brushJitterSettingsEditor = new BrushJitterSettingsEditor();
        public bool brushSettingsFoldout = true;

        public void OnGUI(BrushSettings brush, string content)
        {
            BrushSettingsWindowGUI(brush, content);
        }

        public void BrushSettingsWindowGUI(BrushSettings brush, string content)
		{
			brushSettingsFoldout = CustomEditorGUILayout.Foldout(brushSettingsFoldout, content);

			if(brushSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				GeneralBrushSettings(brush);

				EditorGUI.indentLevel--;
			}
		}

		public void GeneralBrushSettings(BrushSettings brush)
		{
			brush.SpacingEqualsType = (SpacingEqualsType)CustomEditorGUILayout.EnumPopup(spacingEqualsType, brush.SpacingEqualsType);

			if(brush.SpacingEqualsType == SpacingEqualsType.Custom)
			{
				brush.Spacing = CustomEditorGUILayout.FloatField(spacing, brush.Spacing);
			}

			brush.MaskType = (MaskType)CustomEditorGUILayout.EnumPopup(maskType, brush.MaskType);
			
			switch (brush.MaskType)
			{
				case MaskType.Custom:
				{
					brush.CustomMasks.OnGUI();

					break;
				}
				case MaskType.Procedural:
				{
					brush.ProceduralMask.OnGUI();

					break;
				}
			}

			brushJitterSettingsEditor.OnGUI(brush, brush.BrushJitterSettings);
		}

		[NonSerialized]
		private GUIContent spacingMode = new GUIContent("Spacing Mode", "Allows you to disable or enable Brush Drag.");
		[NonSerialized]
		private GUIContent spacingEqualsType = new GUIContent("Spacing Equals", "Allows you to set what size the Spacing will be.");
		[NonSerialized]
		private GUIContent spacingRange = new GUIContent("Spacing Range", "Sets limits on possible Spacing.");
		[NonSerialized]
		private GUIContent spacing = new GUIContent("Spacing", "Controls the distance between brush marks.");
		[NonSerialized]
		private GUIContent maskType = new GUIContent("Mask Type", "Allows you to choose which brush mask will be used.");
    }
}
#endif
