#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using VladislavTsurikov;
using VladislavTsurikov.CustomGUI;
using VladislavTsurikov.ShortcutComboSystem;

namespace VladislavTsurikov.MegaWorldSystem
{
	public partial class MegaWorldWindow : EditorWindow
	{
		[MenuItem("Window/Vladislav Tsurikov/Mega World/Main")]
        static void Init()
        {
            MegaWorldWindow window = (MegaWorldWindow)EditorWindow.GetWindow(typeof(MegaWorldWindow));
            window.Show();
            window.titleContent = new GUIContent("Mega World");
            window.Focus();
            window.ShowUtility();
        }

		void OnInspectorUpdate()
    	{
    	    Repaint();
    	}

		void OnGUI()
        {
			MegaWorldPath.DataPackage.SelectedVariables.DeleteNullValueIfNecessary(MegaWorldPath.DataPackage.BasicData.GroupList);
			MegaWorldPath.DataPackage.SelectedVariables.SetAllSelectedParameters(MegaWorldPath.DataPackage.BasicData.GroupList);
			UpdateSceneViewEvent();

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

		private void UpdateSceneViewEvent()
		{
			Event e = Event.current;

			if (e.Equals(KeyDeleteEvent()))
			{
				Unspawn.UnspawnAllProto(MegaWorldPath.DataPackage.SelectedVariables.SelectedGroupList, true);
			}

			HandleKeyboardEvents(e);
			SceneViewEventHandler.HandleSceneViewEvent(e);
		}

		private void HandleKeyboardEvents(Event e)
        {
			if(e.keyCode == KeyCode.Escape && e.modifiers == 0)
            {
				if(MegaWorldPath.CommonDataPackage.SelectedTool != null)
				{
                    Tools.current = Tool.Move;
					MegaWorldPath.CommonDataPackage.ToolComponentsEditor.DisableAllTools();
				}

                Repaint();
            }
		}

		void OnMainGUI()
		{
			GUILayout.Space(5);

			CustomEditorGUILayout.ScreenRect = this.position;

			if(ToolsWindow.Window == null)
			{
				MegaWorldPath.CommonDataPackage.ToolComponentsEditor.DrawToolsWindow();
			}

			MegaWorldPath.CommonDataPackage.ToolComponentsEditor.DrawSelectedToolSettings();

			MegaWorldPath.DataPackage.SaveAllData();
		}

		public static Event KeyDeleteEvent()
        {
            Event retEvent = Event.KeyboardEvent("^" + "backspace");
            return retEvent;
        }
	}
}
#endif