#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using VladislavTsurikov.CustomGUI;

namespace VladislavTsurikov.MegaWorldSystem
{
    public class ToolStackEditor
    {
        private static int s_TabsWindowHash = "PPGUI.TabsWindow".GetHashCode();

        private ToolEditorStack _toolEditorStack;

        public ToolStack Stack;

        public ToolStackEditor(ToolStack stack)
        {
            Stack = stack;

            if(Stack.Tools.Count == 0)
            {
                for(int i = 0; i < AllToolTypes.TypeList.Count; ++i) 
                {
                    System.Type filterType = AllToolTypes.TypeList[i];

                    Stack.Create(filterType);
                }
            }
            
            _toolEditorStack = new ToolEditorStack(stack);
        }

        private void ShowManu()
        {
            GenericMenu contextMenu = new GenericMenu();

            foreach (var item in AllToolEditorTypes.EditorTypes)
            {
                System.Type toolType = item.Key;

                string name = item.Value.GetAttribute<ToolEditorAttribute>().ContextMenu;

                bool exists = Stack.HasSettings(toolType);

                if (!exists)
                {
                    contextMenu.AddItem(new GUIContent(name), false, () => Create(toolType));
                }
                else
                {
                    contextMenu.AddDisabledItem(new GUIContent(name)); 
                }
            }

            contextMenu.ShowAsContext();
        }

        public void DrawSelectedToolSettings()
        {
            if(MegaWorldPath.CommonDataPackage.SelectedTool == null)
			{
				return;
			}

            for (int i = 0; i < Stack.Tools.Count; i++)
            {
                if(Stack.Tools[i].Enabled)
                {
                    Stack.Tools[i].InternalHandleKeyboardEvents();
                    _toolEditorStack.Editors[i].OnGUI();
                    Stack.Tools[i].SaveSettings();
                    break;
                }
            }
        }

        public void DrawToolsWindow()
        {
            DrawToolsButtonWindow();

            MegaWorldPath.CommonDataPackage.SelectedTool = Stack.GetSelected();

            if(MegaWorldPath.CommonDataPackage.SelectedTool == null)
			{
				EditorGUILayout.BeginVertical(EditorStyles.helpBox);
           		EditorGUILayout.LabelField("No Tool Selected");
           		EditorGUILayout.EndVertical();

				return;
			}
        }

        public void DrawToolsButtonWindow()
        {
            Event e = Event.current;

			Color InitialGUIColor = GUI.color;

            int tabWidth = 130;
			int tabHeight = 25;

            int tabCount = Stack.Tools.Count;
            if(tabCount == 0)
            {
                Rect lineRect = EditorGUILayout.GetControlRect(GUILayout.Height(tabHeight));
                Rect tabPlusRect = new Rect(0, lineRect.y, tabWidth/2, tabHeight);

				CustomEditorGUILayout.RectTab(tabPlusRect, "+", ButtonStyle.Add, tabHeight, 28);

                if(tabPlusRect.Contains(e.mousePosition) && e.type == EventType.MouseDown && e.button == 0)
                {
                    ShowManu();
                }
                return;
            }

            float windowWidth = EditorGUIUtility.currentViewWidth;
            tabCount = tabCount + 1;

            int tabsPerLine = Mathf.Max(1, Mathf.FloorToInt(windowWidth / tabWidth));
            int tabLines = Mathf.CeilToInt((float)tabCount / tabsPerLine);
            int tabIndex = 0;
			int tabUnderCursor = -1;

            ToolComponent draggingTab = null;
            Rect dragRect = new Rect();
            if (InternalDragAndDrop.IsDragging() || InternalDragAndDrop.IsDragPerform())
            {
                if(InternalDragAndDrop.GetData() is ToolComponent)
				{
					draggingTab = (ToolComponent)InternalDragAndDrop.GetData();
				}      
            }

            int tabWindowControlID = GUIUtility.GetControlID(s_TabsWindowHash, FocusType.Passive);

			Color color = GUI.color;

            for(int line = 0; line < tabLines; line++)
            {
                Rect lineRect = EditorGUILayout.GetControlRect(GUILayout.Height(tabHeight));

                for(int tab = 0; tab < tabsPerLine && tabIndex < tabCount; tab++, tabIndex++)
                {
                    if(tabIndex == tabCount - 1)
                    {
                        Rect tabPlusRect = new Rect(lineRect.x + tabWidth * tab, lineRect.y, tabWidth/2, tabHeight);

						CustomEditorGUILayout.RectTab(tabPlusRect, "+", ButtonStyle.Add, tabHeight, 28);

                        if(tabPlusRect.Contains(e.mousePosition) && e.type == EventType.MouseDown && e.button == 0)
                        {
                            ShowManu();
                        }
                        break;
                    }

                    Rect tabRect = new Rect(lineRect.x + tabWidth * tab, lineRect.y, tabWidth, tabHeight);

                    // Tab under cursor
                    if(tabRect.Contains(e.mousePosition))
                    {
						tabUnderCursor = tabIndex;
						
                       if (draggingTab != null)
                       {
							EditorGUIUtility.AddCursorRect (tabRect, UnityEditor.MouseCursor.MoveArrow);

                            bool isAfter = (e.mousePosition.x - tabRect.xMin) > tabRect.width/2;

                            dragRect = new Rect(tabRect);

                            if(isAfter)
                            {
                                dragRect.xMin = dragRect.xMax - 2;
                                dragRect.xMax = dragRect.xMax + 2;
                            }
                            else
                            {
                                dragRect.xMax = dragRect.xMin + 2;
                                dragRect.xMin = dragRect.xMin - 2;
                            }

                            if(InternalDragAndDrop.IsDragPerform())
                            {
                                InsertSelected(tabIndex, isAfter);
                            }
                       }
                    }

                    ToolComponent tool = Stack.Tools[tabIndex];

					CustomEditorGUILayout.RectTab(tabRect, _toolEditorStack.Editors[tabIndex].GetName(), tool.Enabled, tabHeight, ButtonStyle.General);

                    // Tab under cursor
                    if(tabRect.Contains(e.mousePosition))
                    {	
						tabUnderCursor = tabIndex;
                    }
                }
            }

            if(draggingTab != null)
                EditorGUI.DrawRect(dragRect, Color.white);

			switch(e.type)
            {
            	case EventType.MouseDown:
				{
					if(tabUnderCursor != -1 && e.button == 0)
                	{
						GUIUtility.keyboardControl = tabWindowControlID;
            			GUIUtility.hotControl = 0;

						Select(tabUnderCursor);

						e.Use();
					}
            	    break;
				}
                case EventType.ContextClick:
				{
					if(tabUnderCursor != -1)
                	{
						GUIUtility.keyboardControl = tabWindowControlID;
            			GUIUtility.hotControl = 0;

            	    	if(Stack.Tools[tabUnderCursor].Enabled == false)
            	    	{
            	    	    Select(tabUnderCursor);
            	    	} 
						else 
						{
            	    	    TabMenu(tabUnderCursor).ShowAsContext();
            	    	}

						e.Use();
					}
					
            	    break;
				}
			}

            if (InternalDragAndDrop.IsDragReady() && tabUnderCursor != -1)
            {
                InternalDragAndDrop.StartDrag(Stack.Tools[tabUnderCursor]);
            }
        }

        public void DisableAllTools()
        {
            for (int i = 0; i < Stack.Tools.Count; i++)
            {
                Stack.Tools[i].Enabled = false;
                Stack.Tools[i].OnToolDisabled();
            }

#if INSTANT_RENDERER
            if(InstantRendererController.StorageTerrainCells != null)
            {
                InstantRendererController.StorageTerrainCells.CellModifier.RemoveAfterConvert = true;
            }
#endif
            
                
           Selection.objects = new UnityEngine.Object[0];
           Tools.current = Tool.None;
        }

        public void Select(int index)
        {
            for (int i = 0; i < Stack.Tools.Count; i++)
            {
                Stack.Tools[i].Enabled = false;
                Stack.Tools[i].OnToolDisabled();
            }

            Stack.Tools[index].Enabled = true;

#if INSTANT_RENDERER
            if(InstantRendererController.StorageTerrainCells != null)
            {
                InstantRendererController.StorageTerrainCells.CellModifier.RemoveAfterConvert = true;
            }
#endif
                
           Selection.objects = new UnityEngine.Object[0];
           Tools.current = Tool.None;
        }

        private GenericMenu TabMenu(int currentTabIndex)
        {
            GenericMenu menu = new GenericMenu();

            if(Stack.Tools.Count > 1)
                menu.AddItem(new GUIContent("Delete"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => { Remove(currentTabIndex); }));
            else
                menu.AddDisabledItem(new GUIContent("Delete"));    

            return menu;
        }

        public void InsertSelected(int index, bool after)
        {
            List<ToolComponent> selectedTools = new List<ToolComponent>();
            Stack.Tools.ForEach ((brush) => { if(brush.Enabled) selectedTools.Add(brush); });

            if(selectedTools.Count > 0)
            {
                index += after ? 1 : 0;
                index = Mathf.Clamp(index, 0, Stack.Tools.Count);

                Stack.Tools.Insert(index, null);    // insert null marker
                Stack.Tools.RemoveAll (b => b != null && b.Enabled); // remove all selected
                Stack.Tools.InsertRange(Stack.Tools.IndexOf(null), selectedTools); // insert selected brushes after null marker
                Stack.Tools.RemoveAll ((b) => b == null); // remove null marter
            }

            index += after ? 1 : 0;
            index = Mathf.Clamp(index, 0, Stack.Tools.Count);

            _toolEditorStack.Reload(Stack);
        }

        public void Create(System.Type type)
        {
            _toolEditorStack.Create(Stack.Create(type));
        }

        public void Remove(int index)
        {
            Stack.Remove(index);
            _toolEditorStack.Remove(index);
        }

        public ToolBaseEditor GetSelected()
        {
            foreach (ToolBaseEditor tool in _toolEditorStack.Editors)
            {
                if(tool.target.Enabled)
                {
                    return tool;
                }
            }

            return null;
        }
    }
}
#endif