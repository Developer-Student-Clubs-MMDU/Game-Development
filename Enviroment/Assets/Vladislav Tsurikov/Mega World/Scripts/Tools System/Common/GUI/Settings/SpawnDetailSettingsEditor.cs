#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.CustomGUI;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class SpawnDetailSettingsEditor 
    {
        public bool SpawnDetailSettingsFoldout = true;

        public void OnGUI(SpawnDetailSettings settings)
        {
            SpawnDetailSettingsFoldout = CustomEditorGUILayout.Foldout(SpawnDetailSettingsFoldout, "Spawn Detail Settings");

			if(SpawnDetailSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				CustomEditorGUILayout.BeginChangeCheck();
				
				settings.UseRandomOpacity = CustomEditorGUILayout.Toggle(_useRandomOpacity, settings.UseRandomOpacity);
				settings.Density = CustomEditorGUILayout.IntSlider(_density, settings.Density, 0, 10);
				settings.FailureRate = CustomEditorGUILayout.Slider(_failureRate, settings.FailureRate, 0f, 100f);

				if(CustomEditorGUILayout.EndChangeCheck())
				{
					EditorUtility.SetDirty(settings);
				}

				EditorGUI.indentLevel--;
			}
        }

		private GUIContent _density = new GUIContent("Density");
		private GUIContent _useRandomOpacity = new GUIContent("Use Random Opacity");
		private GUIContent _failureRate = new GUIContent("Failure Rate (%)", "The larger this value, the less likely it is to spawn an object.");
    }
}
#endif
