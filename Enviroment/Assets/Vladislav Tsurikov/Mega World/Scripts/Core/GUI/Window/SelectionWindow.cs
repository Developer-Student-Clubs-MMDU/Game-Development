#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using VladislavTsurikov;
using VladislavTsurikov.CustomGUI;

namespace VladislavTsurikov.MegaWorldSystem
{
	public class SelectionWindow : EditorWindow
	{
		public static SelectionWindow Window;

		[MenuItem("Window/Vladislav Tsurikov/Mega World/Separate Window/Selection")]
        static void Init()
        {
			OpenWindow();
        }

		private void OnEnable() 
		{
			Window = this;
		}

		public static void OpenWindow()
		{
			Window = (SelectionWindow)EditorWindow.GetWindow(typeof(SelectionWindow));
            Window.Show();
            Window.titleContent = new GUIContent("Selection");
            Window.Focus();
            Window.ShowUtility();
		}

		void OnInspectorUpdate()
    	{
    	    Repaint();
    	}

		void OnGUI()
        {
			MegaWorldPath.DataPackage.SelectedVariables.DeleteNullValueIfNecessary(MegaWorldPath.DataPackage.BasicData.GroupList);
			MegaWorldPath.DataPackage.SelectedVariables.SetAllSelectedParameters(MegaWorldPath.DataPackage.BasicData.GroupList);

			EditorGUI.indentLevel = 0;

			CustomEditorGUILayout.IsInspector = false;

			InternalDragAndDrop.OnBeginGUI();

			OnMainGUI();

			InternalDragAndDrop.OnEndGUI();

            // repaint every time for dinamic effects like drag scrolling
            if(InternalDragAndDrop.IsDragging() || InternalDragAndDrop.IsDragPerform())
			{
				Repaint();
			}
        }

		void OnMainGUI()
		{
			GUILayout.Space(5);

			CustomEditorGUILayout.ScreenRect = this.position;

			ToolBaseEditor toolBaseEditor = MegaWorldPath.CommonDataPackage.ToolComponentsEditor.GetSelected();

			if(toolBaseEditor != null)
			{
				MegaWorldPath.DataPackage.BasicData.OnGUI(toolBaseEditor.GetDrawBasicData(), toolBaseEditor.target.GetType(), toolBaseEditor.GetClipboard(), toolBaseEditor.GetTemplateStackEditor());
			}
		}
	}
}
#endif