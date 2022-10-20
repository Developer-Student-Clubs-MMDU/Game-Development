#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using VladislavTsurikov;
using VladislavTsurikov.CustomGUI;

namespace VladislavTsurikov.MegaWorldSystem 
{
	[Serializable]
    public static class TerrainResourcesControllerEditor 
    {
		[Serializable]
		public class Layer
		{
			public bool selected = false;
			public TerrainLayer AssignedLayer = null;
		}

		private static List<Layer> paletteLayers = new List<Layer>();
        
		private static int protoIconWidth  = 64;
        private static int protoIconHeight = 76;
		private static float prototypeWindowHeight = 100f;

		private static Vector2 prototypeWindowsScroll = Vector2.zero;

		static void UpdateLayerPalette(Terrain terrain)
		{
			if (terrain == null)
			{
				return;
			}

			bool[] selectedList = new bool[paletteLayers.Count];
			for(int i = 0; i < paletteLayers.Count; i++)
			{
				if (paletteLayers[i] != null)
				{
					selectedList[i] = paletteLayers[i].selected;
				}				
			}

			paletteLayers.Clear();

			int index = 0;
			foreach (TerrainLayer layer in terrain.terrainData.terrainLayers)
			{
				if(layer != null) 
				{
					Layer paletteLayer = new Layer();//ScriptableObject.CreateInstance<Layer>();
					paletteLayer.AssignedLayer = layer; 
					paletteLayer.selected = selectedList.ElementAtOrDefault(index);
					paletteLayers.Add(paletteLayer); 
					index++;
				}
			}
		}

		public static void OnGUI(Group group)
		{
			if(Terrain.activeTerrains.Length != 0)
			{	
				TerrainResourcesController.DetectSyncError(group, Terrain.activeTerrain);

				switch (TerrainResourcesController.SyncError)
				{
					case TerrainResourcesController.TerrainResourcesSyncError.None:
					{
						string getResourcesFromTerrain = "Get/Update Resources From Terrain";
		
						GUILayout.BeginHorizontal();
						{
							GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
							GUILayout.BeginVertical();
							{
								if(CustomEditorGUILayout.ClickButton(getResourcesFromTerrain, ButtonStyle.ButtonClick, ButtonSize.ClickButton))
								{
									TerrainResourcesController.UpdatePrototypesFromTerrain(Terrain.activeTerrain, group);
								}
	
								GUILayout.Space(3);
	
								GUILayout.BeginHorizontal();
								{
									if(CustomEditorGUILayout.ClickButton("Add Missing Resources", ButtonStyle.Add))
									{
										TerrainResourcesController.AddMissingPrototypesToTerrains(group);
									}
	
									GUILayout.Space(2);
	
									if(CustomEditorGUILayout.ClickButton("Remove All Resources", ButtonStyle.Remove))
									{
										if (EditorUtility.DisplayDialog("WARNING!",
											"Are you sure you want to remove all Terrain Resources from the scene?",
											"OK", "Cancel"))
										{
											TerrainResourcesController.RemoveAllPrototypesFromTerrains(group);
										}
									}
								}
								GUILayout.EndHorizontal();
							}
							GUILayout.EndVertical();
							GUILayout.Space(5);
						}
						GUILayout.EndHorizontal();

						break;
					}
					case TerrainResourcesController.TerrainResourcesSyncError.NotAllProtoAvailable:
					{
						switch (group.ResourceType)
            			{
							case ResourceType.TerrainDetail:
            				{
								CustomEditorGUILayout.WarningBox("You need all Terrain Details prototypes to be in the terrain. Click \"Add Missing Resources To Terrain\"");   
								break;
							}
							case ResourceType.TerrainTexture:
            				{
								CustomEditorGUILayout.WarningBox("You need all Terrain Textures prototypes to be in the terrain. Click \"Add Missing Resources To Terrain\"");   
								break;
							}
						}
	
						string getResourcesFromTerrain = "Get/Update Resources From Terrain";
		
						GUILayout.BeginHorizontal();
						{
							GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
							GUILayout.BeginVertical();
							{
								if(CustomEditorGUILayout.ClickButton(getResourcesFromTerrain, ButtonStyle.ButtonClick, ButtonSize.ClickButton))
								{
									TerrainResourcesController.UpdatePrototypesFromTerrain(Terrain.activeTerrain, group);
								}
	
								GUILayout.Space(3);
	
								GUILayout.BeginHorizontal();
								{
									if(CustomEditorGUILayout.ClickButton("Add Missing Resources", ButtonStyle.Add))
									{
										TerrainResourcesController.AddMissingPrototypesToTerrains(group);
									}
	
									GUILayout.Space(2);
	
									if(CustomEditorGUILayout.ClickButton("Remove All Resources", ButtonStyle.Remove))
									{
										if (EditorUtility.DisplayDialog("WARNING!",
											"Are you sure you want to remove all Terrain Resources from the scene?",
											"OK", "Cancel"))
										{
											TerrainResourcesController.RemoveAllPrototypesFromTerrains(group);
										}
									}
								}
								GUILayout.EndHorizontal();
							}
							GUILayout.EndVertical();
							GUILayout.Space(5);
						}
						GUILayout.EndHorizontal();

						break;
					}
					case TerrainResourcesController.TerrainResourcesSyncError.MissingPrototypes:
					{
						string getResourcesFromTerrain = "Get/Update Resources From Terrain";
	
						GUILayout.BeginHorizontal();
						{
							GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
							GUILayout.BeginVertical();
							{
								if(CustomEditorGUILayout.ClickButton(getResourcesFromTerrain, ButtonStyle.ButtonClick, ButtonSize.ClickButton))
								{
									TerrainResourcesController.UpdatePrototypesFromTerrain(Terrain.activeTerrain, group);
								}
							}
							GUILayout.EndVertical();
							GUILayout.Space(5);
						}
						GUILayout.EndHorizontal();

						break;
					}
				}

				if(group.ResourceType == ResourceType.TerrainTexture)
				{
					CustomEditorGUILayout.Header("Active Terrain: Layer Palette");   

					if(Terrain.activeTerrain != null)
					{
						DrawSelectedWindowForTerrainTextureResources(Terrain.activeTerrain, group);
					}
				}

				GUILayout.Space(3);
			}
			else
			{
				CustomEditorGUILayout.WarningBox("There is no active terrain in the scene.");
			}
		}

		public static void DrawSelectedWindowForTerrainTextureResources(Terrain terrain, Group group)
		{
			bool dragAndDrop = false;

			Color InitialGUIColor = GUI.color;

			Event e = Event.current;

			Rect windowRect = EditorGUILayout.GetControlRect(GUILayout.Height(Mathf.Max(0.0f, prototypeWindowHeight)) );
			windowRect = EditorGUI.IndentedRect(windowRect);

			Rect virtualRect = new Rect(windowRect);

			if(IsNecessaryToDrawIconsForPrototypes(windowRect, InitialGUIColor, terrain, group, ref dragAndDrop) == true)
			{
				SelectedWindowUtility.DrawLabelForIcons(InitialGUIColor, windowRect);

				UpdateLayerPalette(terrain);
				DrawProtoTerrainTextureIcons(e, group, paletteLayers, windowRect);

				SelectedWindowUtility.DrawResizeBar(e, protoIconHeight, ref prototypeWindowHeight);
			}
			else
			{
				SelectedWindowUtility.DrawResizeBar(e, protoIconHeight, ref prototypeWindowHeight);
			}
		}

		private static bool IsNecessaryToDrawIconsForPrototypes(Rect brushWindowRect, Color InitialGUIColor, Terrain terrain, Group group, ref bool dragAndDrop)
		{
			switch (group.ResourceType)
            {
				case ResourceType.TerrainDetail:
				{
					if(terrain.terrainData.detailPrototypes.Length == 0)
					{
						SelectedWindowUtility.DrawLabelForIcons(InitialGUIColor, brushWindowRect, "Missing \"Terrain Detail\" on terrain");
						dragAndDrop = true;
						return false;
					}
					break;
				}
            	case ResourceType.TerrainTexture:
            	{
					if(terrain.terrainData.terrainLayers.Length == 0)
					{
						SelectedWindowUtility.DrawLabelForIcons(InitialGUIColor, brushWindowRect, "Missing \"Terrain Layers\" on terrain");
						dragAndDrop = true;
						return false;
					}
            	    break;
            	}
            }

			dragAndDrop = true;
			return true;
		}

		private static void DrawProtoTerrainTextureIcons(Event e, Group group, List<Layer> paletteLayers, Rect brushWindowRect)
		{
			Layer draggingPrototypeTerrainTexture = null;
			if (InternalDragAndDrop.IsDragging() || InternalDragAndDrop.IsDragPerform())
            {
                if(InternalDragAndDrop.GetData() is Layer)
				{
					draggingPrototypeTerrainTexture = (Layer)InternalDragAndDrop.GetData();
				}      
            }

			Rect virtualRect = SelectedWindowUtility.GetVirtualRect(brushWindowRect, paletteLayers.Count, protoIconWidth, protoIconHeight);

			Vector2 brushWindowScrollPos = prototypeWindowsScroll;
            brushWindowScrollPos = GUI.BeginScrollView(brushWindowRect, brushWindowScrollPos, virtualRect, false, true);

			Rect dragRect = new Rect(0, 0, 0, 0);

			int y = (int)virtualRect.yMin;
			int x = (int)virtualRect.xMin;

			for (int protoIndexForGUI = 0; protoIndexForGUI < paletteLayers.Count; protoIndexForGUI++)
			{
				Rect brushIconRect = new Rect(x, y, protoIconWidth, protoIconHeight);

				Color textColor;
				Color rectColor;

				Texture2D preview;
				string name;

				SetColorForIcon(paletteLayers[protoIndexForGUI].selected, out textColor, out rectColor);
				SetIconInfoForTexture(paletteLayers[protoIndexForGUI].AssignedLayer, out preview, out name);

				SelectedWindowUtility.DrawIconRect(brushIconRect, preview, name, textColor, rectColor, e, brushWindowRect, brushWindowScrollPos, protoIconWidth, false, () =>
				{
					if (InternalDragAndDrop.IsDragReady() && protoIndexForGUI != -1)
            		{
            		    InternalDragAndDrop.StartDrag(paletteLayers[protoIndexForGUI]);
            		}

					if (draggingPrototypeTerrainTexture != null && e.type == EventType.Repaint)
					{
						bool isAfter;

						SelectedWindowUtility.SetDragRect(e, brushIconRect, ref dragRect, out isAfter);

						if(InternalDragAndDrop.IsDragPerform())
                		{
							InsertSelectedLayer(protoIndexForGUI, isAfter, group);
							TerrainResourcesController.SyncTerrainID(Terrain.activeTerrain, group);
                		}
					}

					HandleSelectLayer(protoIndexForGUI, group, e);
				});

				SelectedWindowUtility.SetNextXYIcon(virtualRect, protoIconWidth, protoIconHeight, ref y, ref x);
			}

            if(draggingPrototypeTerrainTexture != null)
			{
				EditorGUI.DrawRect(dragRect, Color.white);
			}

			prototypeWindowsScroll = brushWindowScrollPos;

			GUI.EndScrollView();
		}

		private static void SetColorForIcon(bool selected, out Color textColor, out Color rectColor)
		{
			textColor = EditorColors.Instance.LabelColor;

			if(selected)
			{
				rectColor = EditorColors.Instance.ToggleButtonActiveColor;
			}
			else
			{
				rectColor = EditorColors.Instance.ToggleButtonInactiveColor;
			}
		}

		private static void SetIconInfoForTexture(TerrainLayer protoTerrainTexture, out Texture2D preview, out string name)
		{
            if (protoTerrainTexture.diffuseTexture != null)
            {
                preview = protoTerrainTexture.diffuseTexture;      
				name = protoTerrainTexture.name;
            }
			else
			{
				preview = null;
				name = "Missing Texture";
			}
		}

		public static void HandleSelectLayer(int prototypeIndex, Group group, Event e)
		{
			switch (e.type)
			{
				case EventType.MouseDown:
				{
					if(e.button == 0)
					{										
						if (e.control)
						{    
							SelectLayerAdditive(prototypeIndex);               
						}
						else if (e.shift)
						{          
							SelectLayerRange(prototypeIndex);                
						}
						else 
						{
							SelectLayer(prototypeIndex);
						}

            	    	e.Use();
					}

            	    break;
				}
				case EventType.ContextClick:
				{
					PrototypeTerrainTextureMenu(group).ShowAsContext();

					e.Use();

            		break;
				}
			}
		}

		public static void SelectLayer(int prototypeIndex)
        {
            SetSelectedAllLayer(false);

            if(prototypeIndex < 0 && prototypeIndex >= paletteLayers.Count)
            {
                return;
            }

            paletteLayers[prototypeIndex].selected = true;
        }

        public static void SelectLayerAdditive(int prototypeIndex)
        {
            if(prototypeIndex < 0 && prototypeIndex >= paletteLayers.Count)
            {
                return;
            }
        
            paletteLayers[prototypeIndex].selected = !paletteLayers[prototypeIndex].selected;
        }

        public static void SelectLayerRange(int prototypeIndex)
        {
            if(prototypeIndex < 0 && prototypeIndex >= paletteLayers.Count)
            {
                return;
            }

            int rangeMin = prototypeIndex;
            int rangeMax = prototypeIndex;

            for (int i = 0; i < paletteLayers.Count; i++)
            {
                if (paletteLayers[i].selected)
                {
                    rangeMin = Mathf.Min(rangeMin, i);
                    rangeMax = Mathf.Max(rangeMax, i);
                }
            }

            for (int i = rangeMin; i <= rangeMax; i++) 
            {
                if (paletteLayers[i].selected != true)
                {
                    break;
                }
            }

            for (int i = rangeMin; i <= rangeMax; i++) 
            {
                paletteLayers[i].selected = true;
            }
		}

		public static void SetSelectedAllLayer(bool select)
        {
            foreach (Layer proto in paletteLayers)
            {
                proto.selected = select;
            }
        }

        public static void InsertSelectedLayer(int index, bool after, Group group)
        {
            List<Layer> selectedProto = new List<Layer>();
            paletteLayers.ForEach ((proto) => { if(proto.selected) selectedProto.Add(proto); });

            if(selectedProto.Count > 0)
            {
                index += after ? 1 : 0;
                index = Mathf.Clamp(index, 0, paletteLayers.Count);

                paletteLayers.Insert(index, null);    // insert null marker
                paletteLayers.RemoveAll (b => b != null && b.selected); // remove all selected
                paletteLayers.InsertRange(paletteLayers.IndexOf(null), selectedProto); // insert selected brushes after null marker
                paletteLayers.RemoveAll ((b) => b == null); // remove null marter
            }

			SetToTerrainLayers(group);
        }

		public static void SetToTerrainLayers(Group group)
		{
			List<TerrainLayer> layers = new List<TerrainLayer>();

			foreach (Layer item in paletteLayers)
			{
				layers.Add(item.AssignedLayer);
			}

#if UNITY_2019_2_OR_NEWER
			Terrain.activeTerrain.terrainData.SetTerrainLayersRegisterUndo(layers.ToArray(), "Reorder Terrain Layers");
#else
			Terrain.activeTerrain.terrainData.terrainLayers = layers.ToArray();
#endif

			TerrainResourcesController.SyncAllTerrains(group, Terrain.activeTerrain);
		}

		private static GenericMenu PrototypeTerrainTextureMenu(Group group)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Delete"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => DeleteSelectedProtoTerrainTexture(group)));

			menu.AddSeparator ("");
			
            menu.AddItem(new GUIContent("Select All"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => SetSelectedAllLayer(true)));

            return menu;
        }

        public static void DeleteSelectedProtoTerrainTexture(Group group)
        {
            paletteLayers.RemoveAll((prototype) => prototype.selected);
			SetToTerrainLayers(group);
        }
	}
}
#endif