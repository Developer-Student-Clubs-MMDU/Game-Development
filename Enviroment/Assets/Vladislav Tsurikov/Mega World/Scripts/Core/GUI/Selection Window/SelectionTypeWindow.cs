#if UNITY_EDITOR
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.CustomGUI;
using VladislavTsurikov.Extensions;

namespace VladislavTsurikov.MegaWorldSystem
{
	[Serializable]
    public class SelectionTypeWindow 
    {
        private static bool _selectTypeFoldout = true;
		private static int _typeIconWidth  = 80;
        private static int _typeIconHeight = 95;
		private static float _typeWindowHeight = 100f;

        private BasicData _data;
        private SelectedVariables _selectedTypeVariables;
        private ClipboardBase _clipboard;

        public void OnGUI(BasicData data, SelectedVariables selectedTypeVariables, ClipboardBase clipboard)
        {
			this._selectedTypeVariables = selectedTypeVariables;
			this._clipboard = clipboard;

            _data = data;

            DrawTypes();
        }
	
        public void DrawTypes()
		{
			_selectTypeFoldout = CustomEditorGUILayout.Foldout(_selectTypeFoldout, "Groups");

			if(_selectTypeFoldout)
			{
				DrawSelectedWindowForTypes();
			}
		}

		private void DrawSelectedWindowForTypes()
		{
			++EditorGUI.indentLevel;

			bool dragAndDrop = false;

			Color InitialGUIColor = GUI.color;

			Event e = Event.current;

			Rect windowRect = EditorGUILayout.GetControlRect(GUILayout.Height(Mathf.Max(0.0f, _typeWindowHeight)) );
			windowRect = EditorGUI.IndentedRect(windowRect);

			Rect virtualRect = new Rect(windowRect);

			if(IsNecessaryToDrawIconsForTypes(windowRect, InitialGUIColor, ref dragAndDrop) == true)
			{
				SelectedWindowUtility.DrawLabelForIcons(InitialGUIColor, windowRect);

				DrawTypesIcons(e, windowRect);

				SelectedWindowUtility.DrawResizeBar(e, _typeIconHeight, ref _typeWindowHeight);
			}
			else
			{
				SelectedWindowUtility.DrawResizeBar(e, _typeIconHeight, ref _typeWindowHeight);
			}

			if(dragAndDrop == true)
			{
				DropOperationForType(e, virtualRect);
			}

			switch (e.type)
			{
				case EventType.ContextClick:
				{
            		if(virtualRect.Contains(e.mousePosition))
            		{
						WindowMenu.GroupWindowMenu(_data).ShowAsContext();
            		    e.Use();
            		}
            		break;
				}
			}

			--EditorGUI.indentLevel;
		}

		public void DrawTypesIcons(Event e, Rect windowRect)
		{
			Group draggingType = null;
			if (InternalDragAndDrop.IsDragging() || InternalDragAndDrop.IsDragPerform())
            {
                if(InternalDragAndDrop.GetData() is Group)
				{
					draggingType = (Group)InternalDragAndDrop.GetData();
				}      
            }

			Rect virtualRect = SelectedWindowUtility.GetVirtualRect(windowRect, _data.GroupList.Count, _typeIconWidth, _typeIconHeight);

			Vector2 windowScrollPos = _data.TypeWindowsScroll;
            windowScrollPos = GUI.BeginScrollView(windowRect, windowScrollPos, virtualRect, false, true);

			int y = (int)virtualRect.yMin;
			int x = (int)virtualRect.xMin;

			Rect dragRect = new Rect(0, 0, 0, 0);

			for (int typeIndex = 0; typeIndex < _data.GroupList.Count; typeIndex++)
			{
				Rect iconRect = new Rect(x, y, _typeIconWidth, _typeIconHeight);

				Color textColor;
				Color rectColor;

				SetColorForTypeIcon(_data.GroupList[typeIndex], out textColor, out rectColor);
				DrawIconRectForType(e, _data.GroupList[typeIndex], iconRect, windowRect, textColor, rectColor, windowScrollPos, () =>
				{
					if (InternalDragAndDrop.IsDragReady() && typeIndex != -1)
            		{
            		    InternalDragAndDrop.StartDrag(_data.GroupList[typeIndex]);
            		}

					if (draggingType != null && e.type == EventType.Repaint)
					{
						bool isAfter;

						SelectedWindowUtility.SetDragRect(e, iconRect, ref dragRect, out isAfter);

						if(InternalDragAndDrop.IsDragPerform())
            	        {
            	            SelectionTypeUtility.InsertSelectedGroup(_data.GroupList, typeIndex, isAfter);
            	        }
					}

					HandleSelectType(_data.GroupList, typeIndex, e);
				});

				SelectedWindowUtility.SetNextXYIcon(virtualRect, _typeIconWidth, _typeIconHeight, ref y, ref x);
			}

            if(draggingType != null)
			{
				EditorGUI.DrawRect(dragRect, Color.white);
			}

			_data.TypeWindowsScroll = windowScrollPos;

			GUI.EndScrollView();
		}

		private void SetColorForTypeIcon(Group group, out Color textColor, out Color rectColor)
		{
			textColor = EditorColors.Instance.LabelColor;

			if(group.Renaming)
			{
				rectColor = EditorColors.Instance.orangeNormal.WithAlpha(0.3f);
				
				if (EditorGUIUtility.isProSkin)
            	{
					textColor = EditorColors.Instance.orangeNormal; 
            	}
            	else
            	{
					textColor = EditorColors.Instance.orangeDark;
				}		
			}	

			else if(group.Selected)
			{
				rectColor = EditorColors.Instance.ToggleButtonActiveColor;
			}
			else
			{
				rectColor = EditorColors.Instance.ToggleButtonInactiveColor;
			}
		}

        private void DrawIconRectForType(Event e, Group group, Rect iconRect, Rect windowRect, Color textColor, Color rectColor, Vector2 windowScrollPos, UnityAction clickAction)
		{
			GUIStyle LabelTextForIcon = CustomEditorGUILayout.GetStyle(StyleName.LabelTextForIcon);
			GUIStyle LabelTextForSelectedArea = CustomEditorGUILayout.GetStyle(StyleName.LabelTextForSelectedArea);

            Rect iconRectScrolled = new Rect(iconRect);
            iconRectScrolled.position -= windowScrollPos;

            // only visible incons
            if(iconRectScrolled.Overlaps(windowRect))
            {
                if(iconRect.Contains(e.mousePosition))
				{
					clickAction.Invoke();
				}

				EditorGUI.DrawRect(iconRect, rectColor);
                    
				// Prefab preview 
                if(e.type == EventType.Repaint)
            	{
            	    Rect previewRect = new Rect(iconRect.x+2, iconRect.y+2, iconRect.width-4, iconRect.width-4);
            	    Color dimmedColor = new Color(0.4f, 0.4f, 0.4f, 1.0f);

            	    Rect[] icons =
            	    {   new Rect(previewRect.x, previewRect.y, previewRect.width / 2 - 1, previewRect.height / 2 - 1),
            	        new Rect(previewRect.x + previewRect.width / 2, previewRect.y, previewRect.width / 2, previewRect.height / 2 - 1),
            	        new Rect(previewRect.x, previewRect.y + previewRect.height/2, previewRect.width / 2 - 1, previewRect.height / 2),
            	        new Rect(previewRect.x + previewRect.width / 2, previewRect.y + previewRect.height / 2, previewRect.width / 2, previewRect.height / 2)
            	    };

					Texture2D[] textures = new Texture2D[4];

					switch (group.ResourceType)
					{
						case ResourceType.GameObject:
						{
							for(int i = 0, j = 0; i < group.ProtoGameObjectList.Count && j < 4; i++)
            	    		{
            	    		    if(group.ProtoGameObjectList[i].Prefab != null)
            	    		    {
            	        			textures[j] = MegaWorldGUIUtility.GetPrefabPreviewTexture(group.ProtoGameObjectList[i].Prefab);
            	    		        j++;
            	    		    }
            	    		}
							break;
						}
						case ResourceType.TerrainDetail:
						{
							for(int i = 0, j = 0; i < group.ProtoTerrainDetailList.Count && j < 4; i++)
            	    		{
								if(group.ProtoTerrainDetailList[i].PrefabType == PrefabType.Mesh)
								{
									if(group.ProtoTerrainDetailList[i].Prefab != null)
            	    		    	{
            	    		    	    textures[j] = MegaWorldGUIUtility.GetPrefabPreviewTexture(group.ProtoTerrainDetailList[i].Prefab);
            	    		    	    j++;
            	    		    	}
								}
								else
								{
									if(group.ProtoTerrainDetailList[i].DetailTexture != null)
            	    		    	{
										textures[j] = group.ProtoTerrainDetailList[i].DetailTexture;
            	    		    	    j++;
            	    		    	}
								}
            	    		}

							break;
						}
						case ResourceType.TerrainTexture:
						{
							for(int i = 0, j = 0; i < group.ProtoTerrainTextureList.Count && j < 4; i++)
            	    		{
								if(group.ProtoTerrainTextureList[i].TerrainTextureSettings.DiffuseTexture != null)
            	    		    {
									textures[j] = group.ProtoTerrainTextureList[i].TerrainTextureSettings.DiffuseTexture;
            	    		        j++;
            	    		    }
            	    		}

							break;
						}
						case ResourceType.InstantItem:
						{
							for(int i = 0, j = 0; i < group.ProtoInstantItemList.Count && j < 4; i++)
            	    		{
								if(group.ProtoInstantItemList[i].Prefab != null)
            	    		    {
            	    		        textures[j] = MegaWorldGUIUtility.GetPrefabPreviewTexture(group.ProtoInstantItemList[i].Prefab);
            	    		        j++;
            	    		    }
            	    		}

							break;
						}
					}

            	    for(int i = 0; i < 4; i++)
            	    {
            	        if(textures[i] != null)
            	        {
							EditorGUI.DrawPreviewTexture(icons[i], textures[i]);
            	        }
            	        else
						{
							EditorGUI.DrawRect(icons[i], dimmedColor);
						}
            	    }

					LabelTextForIcon.normal.textColor = textColor;
					LabelTextForIcon.Draw(iconRect, SelectedWindowUtility.GetShortNameForIcon(group.Name, _typeIconWidth), false, false, false, false);
            	}
			}
		}

		private bool IsNecessaryToDrawIconsForTypes(Rect windowRect, Color InitialGUIColor, ref bool dragAndDrop)
		{
			if(_data.GroupList.Count == 0)
			{
				SelectedWindowUtility.DrawLabelForIcons(InitialGUIColor, windowRect, "Drag & Drop Type Here");
				dragAndDrop = true;
				return false;
			}

			dragAndDrop = true;
			return true;
		}

		private void DropOperationForType(Event e, Rect virtualRect)
		{
			if (e.type == EventType.DragUpdated || e.type == EventType.DragPerform)
            {
                // Add Prefab
                if(virtualRect.Contains (e.mousePosition))
                {
                    if (DragAndDrop.objectReferences.Length > 0)
					{
						DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
					}
	
                    if (e.type == EventType.DragPerform)
					{
                        DragAndDrop.AcceptDrag();
	
						List<Group> groupList = new List<Group>();
	
                        foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences)
                        {
                            if (draggedObject is Group)
                            {
								if(draggedObject)
								{
									groupList.Add((Group)draggedObject);   
								}    
                            }
                        }
	
						if(groupList.Count > 0)
						{
							foreach (Group group in groupList)
							{
								if(_data.GroupList.Contains(group) == false)
								{
									_data.GroupList.Add(group);  
								}
							}  
						}
                    }
                    e.Use();
                } 
            }
		}

		public void HandleSelectType(List<Group> groupList, int typeIndex, Event e)
		{
			switch(e.type)
            {
            	case EventType.MouseDown:
				{
					if(e.button == 0)
                    {
						if (e.control)
                        {    
							SelectionTypeUtility.SelectTypeAdditive(groupList, typeIndex);               
                        }
                        else if (e.shift)
                        {          
							SelectionTypeUtility.SelectGroupRange(groupList, typeIndex);                
                        }
						else 
						{
							SelectionTypeUtility.DisableAllGroup(groupList);
							SelectionTypeUtility.SelectType(groupList, typeIndex);    
                        }

            	        e.Use();
					}
            	    break;
				}
				case EventType.ContextClick:
				{
					if (_data.GroupList[typeIndex].Selected)
            		{
            		    WindowMenu.GroupMenu(_data, _selectedTypeVariables, _clipboard, typeIndex).ShowAsContext();
            		}
            		else
            		{
						SelectionTypeUtility.DisableAllGroup(groupList);
						SelectionTypeUtility.SelectType(groupList, typeIndex);    
            		}
            		
            		e.Use();

            		break;
				}
			}
		}
    }
}
#endif