#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.CustomGUI;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class SuccessSettingsEditor
    {
        public void OnGUI(SuccessSettings settings)
        {
            CustomEditorGUILayout.BeginChangeCheck();

            settings.SuccessValue = CustomEditorGUILayout.Slider(success, settings.SuccessValue, 0f, 100f);

            if(CustomEditorGUILayout.EndChangeCheck())
			{
				EditorUtility.SetDirty(settings);
			}
        }

		private GUIContent success = new GUIContent("Success (%)");
    }
}
#endif
