#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.CustomGUI;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class RaycastSettingsEditor 
    {
        public bool raycastSettingsFoldout = true;

        public void OnGUI(RaycastSettings raycastSettings)
        {
            RaycastSettings(raycastSettings);
        }

        public void RaycastSettings(RaycastSettings raycastSettings)
		{
			raycastSettingsFoldout = CustomEditorGUILayout.Foldout(raycastSettingsFoldout, "Raycast Settings");

			if(raycastSettingsFoldout)
			{
				EditorGUI.indentLevel++;

                raycastSettings.RaycastType = (RaycastType)CustomEditorGUILayout.EnumPopup(new GUIContent("Raycast Type", ""), raycastSettings.RaycastType);

				raycastSettings.Offset = CustomEditorGUILayout.FloatField(new GUIContent("Offset", "If you want to spawn objects under pawns or inside buildings or in other similar cases. You need to decrease the Spawn Check Offset."), raycastSettings.Offset);

				EditorGUI.indentLevel--;
			}
		}
    }
}
#endif