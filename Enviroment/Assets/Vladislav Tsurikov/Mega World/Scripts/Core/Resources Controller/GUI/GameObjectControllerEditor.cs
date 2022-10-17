#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.CustomGUI;
using VladislavTsurikov.RaycastEditorSystem;

namespace VladislavTsurikov.MegaWorldSystem 
{
    [Serializable]
    public static class GameObjectControllerEditor
    {
        public static void OnGUI()
		{
			MegaWorldPath.GameObjectStoragePackage.CellSize = CustomEditorGUILayout.FloatField(new GUIContent("Cell Size", ""), MegaWorldPath.GameObjectStoragePackage.CellSize);

            CustomEditorGUILayout.HelpBox("If you manually changed the position of the GameObject without using MegaWorld, please click on this button, otherwise, for example, Brush Erase will not be able to delete the changed GameObject.");

            GUILayout.BeginHorizontal();
            {
				GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
				if(CustomEditorGUILayout.ClickButton("Refresh Cells", ButtonStyle.Add))
				{
					MegaWorldPath.GameObjectStoragePackage.Storage.RefreshCells(MegaWorldPath.GameObjectStoragePackage.CellSize);
					RaycastEditor.RefreshObjectTree();
					
				}
				GUILayout.Space(5);
			}
			GUILayout.EndHorizontal();
        }
    }
}
#endif