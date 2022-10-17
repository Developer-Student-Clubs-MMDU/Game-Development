#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VladislavTsurikov.CustomGUI;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
	public class SelectionPrototypeWindow 
    {
		private static bool _selectPrototypeFoldout = true;
		private static int _protoIconWidth  = 80;
        private static int _protoIconHeight = 95;
		private static float _prototypeWindowHeight = 100f;

        private SelectedVariables _selectedTypeVariables;
        private ClipboardBase _clipboard;
		private TemplateStackEditor _templateStackEditor;
        private Group _type;
		private Type _toolType;

        public void OnGUI(SelectedVariables selectedTypeVariables, Type toolType, ClipboardBase clipboard, TemplateStackEditor templateStackEditor)
        {
            _selectedTypeVariables = selectedTypeVariables;
			_clipboard = clipboard;
			_templateStackEditor = templateStackEditor;
            _type = selectedTypeVariables.SelectedGroup;
			_toolType = toolType;

            DrawPrototypes();
        }

        public void DrawPrototypes()
        {
			EditorGUILayout.BeginHorizontal();
            _selectPrototypeFoldout = CustomEditorGUILayout.Foldout(_selectPrototypeFoldout, "Prototypes");
			
			if(_selectedTypeVariables.HasOneSelectedGroup())
			{
				SelectionResourcesType.DrawResourcesType(_type);
			}
			
            EditorGUILayout.EndHorizontal();
            
			if(_selectPrototypeFoldout)
			{
				DrawSelectedWindowForPrototypes();	
			}
        }

        public void DrawSelectedWindowForPrototypes()
		{
			++EditorGUI.indentLevel;

			bool dragAndDrop = false;

			Color InitialGUIColor = GUI.color;

			Event e = Event.current;

			Rect windowRect = EditorGUILayout.GetControlRect(GUILayout.Height(Mathf.Max(0.0f, _prototypeWindowHeight)) );
			windowRect = EditorGUI.IndentedRect(windowRect);

			Rect virtualRect = new Rect(windowRect);

			if(IsNecessaryToDrawIconsForPrototypes(_selectedTypeVariables, windowRect, InitialGUIColor, ref dragAndDrop) == true)
			{
				SelectedWindowUtility.DrawLabelForIcons(InitialGUIColor, windowRect);

				DrawProtoIcons(_type, e, _type.GetPrototypes(), windowRect);
			}

			SelectedWindowUtility.DrawResizeBar(e, _protoIconHeight, ref _prototypeWindowHeight);

			if(dragAndDrop)
			{
				DropOperation(_type, e, virtualRect);
			} 

			--EditorGUI.indentLevel;
		}

		private void DrawProtoIcons(Group group, Event e, List<Prototype> protoList, Rect windowRect)
		{
			Prototype draggingPrototype = null;
			if (InternalDragAndDrop.IsDragging() || InternalDragAndDrop.IsDragPerform())
            {
                if(InternalDragAndDrop.GetData() is Prototype)
				{
					draggingPrototype = (Prototype)InternalDragAndDrop.GetData();
				}      
            }

			Rect virtualRect = SelectedWindowUtility.GetVirtualRect(windowRect, protoList.Count, _protoIconWidth, _protoIconHeight);

			Vector2 windowScrollPos = group.PrototypeWindowsScroll;
            windowScrollPos = GUI.BeginScrollView(windowRect, windowScrollPos, virtualRect, false, true);

			Rect dragRect = new Rect(0, 0, 0, 0);

			int y = (int)virtualRect.yMin;
			int x = (int)virtualRect.xMin;

			for (int protoIndex = 0; protoIndex < protoList.Count; protoIndex++)
			{
				Rect iconRect = new Rect(x, y, _protoIconWidth, _protoIconHeight);

				Color textColor;
				Color rectColor;

				Texture2D preview;
				string name;

				SetColorForIcon(protoList[protoIndex].Selected, protoList[protoIndex].Active, out textColor, out rectColor);
				protoList[protoIndex].SetIconInfo(out preview, out name);

				bool drawTextureWithAlphaChannel = protoList[protoIndex] is PrototypeTerrainDetail;

				SelectedWindowUtility.DrawIconRect(iconRect, preview, name, textColor, rectColor, e, windowRect, windowScrollPos, _protoIconWidth, drawTextureWithAlphaChannel, () =>
				{
					if (InternalDragAndDrop.IsDragReady() && protoIndex != -1)
            		{
            		    InternalDragAndDrop.StartDrag(protoList[protoIndex]);
            		}

					if (draggingPrototype != null && e.type == EventType.Repaint)
					{
						bool isAfter;

						SelectedWindowUtility.SetDragRect(e, iconRect, ref dragRect, out isAfter);

						if(InternalDragAndDrop.IsDragPerform())
                		{
							SelectionPrototypeUtility.InsertSelectedProto(group, protoIndex, isAfter);
                		}
					}

					HandleSelectPrototype(group, protoIndex, e);
				});

				SelectedWindowUtility.SetNextXYIcon(virtualRect, _protoIconWidth, _protoIconHeight, ref y, ref x);
			}

			if(draggingPrototype != null)
			{
				EditorGUI.DrawRect(dragRect, Color.white);
			}

			group.PrototypeWindowsScroll = windowScrollPos;

			GUI.EndScrollView();
		}

        private bool IsNecessaryToDrawIconsForPrototypes(SelectedVariables selectedTypeVariables, Rect windowRect, Color InitialGUIColor, ref bool dragAndDrop)
		{
			if(selectedTypeVariables.HasOneSelectedGroup() == false)
			{
				SelectedWindowUtility.DrawLabelForIcons(InitialGUIColor, windowRect, "To Draw Prototype, you need to select one type");
				dragAndDrop = false;
				return false;
			}	

			Group group = selectedTypeVariables.SelectedGroup;

			switch (group.ResourceType)
            {
                case ResourceType.GameObject:
                {
					if(group.ProtoGameObjectList.Count == 0)
					{
						SelectedWindowUtility.DrawLabelForIcons(InitialGUIColor, windowRect, "Drag & Drop Prefabs Here");
						dragAndDrop = true;
						return false;
					}
                    break;
                }
				case ResourceType.TerrainDetail:
				{
					if(group.ProtoTerrainDetailList.Count == 0)
					{
						SelectedWindowUtility.DrawLabelForIcons(InitialGUIColor, windowRect, "Drag & Drop Prefabs or Textures Here");
						dragAndDrop = true;
						return false;
					}
					break;
				}
            	case ResourceType.TerrainTexture:
            	{
					if(group.ProtoTerrainTextureList.Count == 0)
					{
						SelectedWindowUtility.DrawLabelForIcons(InitialGUIColor, windowRect, "Drag & Drop Texture or Terrain Layers Here");
						dragAndDrop = true;
						return false;
					}
            	    break;
            	}
				case ResourceType.InstantItem:
				{
					if(group.ProtoInstantItemList.Count == 0)
					{
						SelectedWindowUtility.DrawLabelForIcons(InitialGUIColor, windowRect, "Drag & Drop Prefabs Here");
						dragAndDrop = true;
						return false;
					}
					break;
            	}
            }

			dragAndDrop = true;
			return true;
		}

		private void DropOperation(Group group, Event e, Rect virtualRect)
		{
			if (e.type == EventType.DragUpdated || e.type == EventType.DragPerform)
            {
                if(virtualRect.Contains(e.mousePosition))
                {
                    if (DragAndDrop.objectReferences.Length > 0)
					{
						DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
					}

                    if (e.type == EventType.DragPerform)
					{
                        DragAndDrop.AcceptDrag();

						group.DropOperation(DragAndDrop.objectReferences);
                    }
                    e.Use();
                } 
			}
		}

		public void HandleSelectPrototype(Group group, int prototypeIndex, Event e)
		{
			switch (e.type)
			{
				case EventType.MouseDown:
				{
					if(e.button == 0)
					{										
						if (e.control)
						{    
							SelectionPrototypeUtility.SelectPrototypeAdditive(group, prototypeIndex);               
						}
						else if (e.shift)
						{          
							SelectionPrototypeUtility.SelectPrototypeRange(group, prototypeIndex);                
						}
						else 
						{
							SelectionPrototypeUtility.SelectPrototype(group, prototypeIndex);
						}

            	    	e.Use();
					}

            	    break;
				}
				case EventType.ContextClick:
				{
					WindowMenu.PrototypeMenu(_toolType, group, _selectedTypeVariables, _clipboard, _templateStackEditor, prototypeIndex).ShowAsContext();

					e.Use();

            		break;
				}
			}
		}

        private void SetColorForIcon(bool selected, bool active, out Color textColor, out Color rectColor)
		{
			textColor = EditorColors.Instance.LabelColor;

			if(selected)
			{
				rectColor = active ? EditorColors.Instance.ToggleButtonActiveColor : EditorColors.Instance.redNormal;
			}
			else
			{
				rectColor = active ? EditorColors.Instance.ToggleButtonInactiveColor : EditorColors.Instance.redDark;
			}
		}
    }
}
#endif