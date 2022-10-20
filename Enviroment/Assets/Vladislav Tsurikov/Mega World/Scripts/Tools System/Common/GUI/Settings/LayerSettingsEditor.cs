#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.CustomGUI;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class LayerSettingsEditor 
    {
        public bool layerSettingsFoldout = true;

        public void OnGUI(LayerSettings layerSettings, bool useOnlyCustomRaycast = false)
        {
            layerSettingsFoldout = CustomEditorGUILayout.Foldout(layerSettingsFoldout, "Layer Settings");

			if(layerSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				layerSettings.PaintLayers = CustomEditorGUILayout.LayerField(new GUIContent("Paint Layers", "Allows you to set the layers on which to spawn."), layerSettings.PaintLayers);

				EditorGUI.indentLevel--;
			}
        }
    }
}
#endif