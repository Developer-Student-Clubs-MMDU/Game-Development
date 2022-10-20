#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.CustomGUI;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class BrushJitterSettingsEditor 
    {
        public void OnGUI(BrushSettings brush, BrushJitterSettings jitter)
        {
            brush.BrushSize = CustomEditorGUILayout.Slider(brushSize, brush.BrushSize, 0.1f, MegaWorldPath.AdvancedSettings.EditorSettings.maxBrushSize);

            jitter.BrushSizeJitter = CustomEditorGUILayout.Slider(brushJitter, jitter.BrushSizeJitter, 0f, 1f);

			CustomEditorGUILayout.Separator();

			jitter.BrushScatter = CustomEditorGUILayout.Slider(brushScatter, jitter.BrushScatter, 0f, 1f);
            jitter.BrushScatterJitter = CustomEditorGUILayout.Slider(brushJitter, jitter.BrushScatterJitter, 0f, 1f);

			CustomEditorGUILayout.Separator();

			if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedGroup())
			{
				if(MegaWorldPath.DataPackage.SelectedVariables.SelectedGroup.ResourceType == ResourceType.TerrainDetail
				|| MegaWorldPath.DataPackage.SelectedVariables.SelectedGroup.ResourceType == ResourceType.TerrainTexture)
				{
					brush.BrushRotation = CustomEditorGUILayout.Slider(brushRotation, brush.BrushRotation, -180f, 180f);
            		jitter.BrushRotationJitter = CustomEditorGUILayout.Slider(brushJitter, jitter.BrushRotationJitter, 0f, 1f);

					CustomEditorGUILayout.Separator();
				}
			}
        }

		[NonSerialized]
        private GUIContent brushSize = new GUIContent("Brush Size", "Selected prototypes will only spawn in this range around the center of Brush.");
		[NonSerialized]
		private GUIContent brushJitter = new GUIContent("Jitter", "Control brush stroke randomness.");
		[NonSerialized]
		private GUIContent brushScatter = new GUIContent("Brush Scatter", "Randomize brush position by an offset.");
		[NonSerialized]
		private GUIContent brushRotation = new GUIContent("Brush Rotation", "Rotation of the brush.");
    }
}
#endif