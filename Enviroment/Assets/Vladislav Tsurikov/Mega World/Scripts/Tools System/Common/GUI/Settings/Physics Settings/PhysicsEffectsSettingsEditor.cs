#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.CustomGUI;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class PhysicsEffectsSettingsEditor 
    {
        public bool physicsEffectsFoldout = true;

		public bool forceFoldout = true;
		public bool directionFoldout = true;

        public void OnGUI(PhysicsEffectsSettings settings)
        {
			physicsEffectsFoldout = CustomEditorGUILayout.Foldout(physicsEffectsFoldout, "Physics Effects");

			if (physicsEffectsFoldout)
			{
				EditorGUI.indentLevel++;

				forceFoldout = CustomEditorGUILayout.Foldout(forceFoldout, "Force");

				if (forceFoldout)
				{
					EditorGUI.indentLevel++;

					settings.ForceRange = CustomEditorGUILayout.Toggle(new GUIContent("Force Range"), settings.ForceRange);
				
					if(settings.ForceRange)
					{
						settings.MinForce = Mathf.Max(0, CustomEditorGUILayout.Slider(new GUIContent("Min Force"), settings.MinForce, 0, 100));
						settings.MaxForce = Mathf.Max(settings.MinForce, CustomEditorGUILayout.Slider(new GUIContent("Max Force"), settings.MaxForce, 0, 100));
					}
					else
					{
						settings.MinForce =  Mathf.Max(0, CustomEditorGUILayout.Slider(new GUIContent("Force"), settings.MinForce, 0, 100));
					}

					EditorGUI.indentLevel--;
				}

				directionFoldout = CustomEditorGUILayout.Foldout(directionFoldout, "Direction");

				if (directionFoldout)
				{
					EditorGUI.indentLevel++;

					settings.RandomStrength = CustomEditorGUILayout.Slider(new GUIContent("Random Strength"),settings.RandomStrength, 0, 100);

					EditorGUI.indentLevel--;
				}

				EditorGUI.indentLevel--;
			}
        }
    }
}
#endif