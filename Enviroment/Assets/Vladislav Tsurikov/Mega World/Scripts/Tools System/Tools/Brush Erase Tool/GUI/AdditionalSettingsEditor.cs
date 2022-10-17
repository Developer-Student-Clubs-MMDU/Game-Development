#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.CustomGUI;

namespace VladislavTsurikov.MegaWorldSystem.BrushErase
{
    [Serializable]
    public class AdditionalSettingsEditor
    {
        public void OnGUI(AdditionalEraseSettings settings)
        {
            settings.SuccessForErase = CustomEditorGUILayout.Slider(success, settings.SuccessForErase, 0f, 100f);
        }

		private GUIContent success = new GUIContent("Success of Erase (%)");
    }
}
#endif