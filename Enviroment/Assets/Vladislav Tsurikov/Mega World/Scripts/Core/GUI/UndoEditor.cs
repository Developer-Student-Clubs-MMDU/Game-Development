#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov;
using VladislavTsurikov.CustomGUI;

namespace VladislavTsurikov.MegaWorldSystem 
{
    [Serializable]
    public static class UndoEditor
    {
        public static void OnGUI()
		{
            GUILayout.BeginHorizontal();
            {
				GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
				if(CustomEditorGUILayout.ClickButton("Undo (" + UndoSystem.Undo.UndoRecordCount + "/" + UndoSystem.Undo.MaxNumberOfUndo + ")"))
				{
					UndoSystem.Undo.PerformUndo();
				}
				GUILayout.Space(3);
				if(CustomEditorGUILayout.ClickButton("Undo All"))
				{
					UndoSystem.Undo.PerformUndoAll();
				}
				GUILayout.Space(5);
			}
			GUILayout.EndHorizontal();
        }
    }
}
#endif