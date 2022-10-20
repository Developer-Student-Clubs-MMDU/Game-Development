#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.CustomGUI;

namespace VladislavTsurikov.PhysicsSimulatorEditor 
{
	[Serializable]
	public static class PhysicsSimulatorSettingsEditor 
	{
		public static bool physicsSimulatorSettingsFoldout = true;

		public static void OnGUI(PhysicsSimulatorSettings settings, DisablePhysicsMode disablePhysicsMode, bool accelerationPhysics = true)
		{
			physicsSimulatorSettingsFoldout = CustomEditorGUILayout.Foldout(physicsSimulatorSettingsFoldout, "Physics Simulator Settings");

			if (physicsSimulatorSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				string disablePhysicsModeText = disablePhysicsMode == DisablePhysicsMode.GlobalTime ? "Global Time" : "Object Time";

 				CustomEditorGUILayout.Label(new GUIContent("Disable Physics Mode" + " (" + disablePhysicsModeText + ")"));

				CustomEditorGUILayout.BeginChangeCheck();

				settings.SimulatePhysics = CustomEditorGUILayout.Toggle(new GUIContent("Simulate Physics"), settings.SimulatePhysics);

				if(disablePhysicsMode == DisablePhysicsMode.GlobalTime)
				{
					settings.GlobalTime = CustomEditorGUILayout.FloatField(new GUIContent("Global Time"), settings.GlobalTime);
				}
				else
				{
					settings.ObjectTime = CustomEditorGUILayout.FloatField(new GUIContent("Object Time"), settings.ObjectTime);
				}

				if(accelerationPhysics)
				{
					settings.AccelerationPhysics = CustomEditorGUILayout.IntSlider(new GUIContent("Acceleration Physics"), settings.AccelerationPhysics, 1, 100);
				}

				PositionOffsetSettingsEditor.OnGUI(settings.PositionOffsetSettings);

				if(CustomEditorGUILayout.EndChangeCheck())
				{
					settings.Save();
				}

				EditorGUI.indentLevel--;
			}
		}
	}
}
#endif