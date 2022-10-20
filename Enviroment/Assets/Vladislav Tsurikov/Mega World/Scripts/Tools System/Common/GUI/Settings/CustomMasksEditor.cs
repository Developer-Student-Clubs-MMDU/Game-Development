#if UNITY_EDITOR
using UnityEngine.Events;
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov;
using VladislavTsurikov.CustomGUI;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class CustomMasksEditor 
    {
        private int alphaBrushesIconWidth  = 60;
        private int alphaBrushesIconHeight = 60;

		public Vector2 alphaBrushesWindowsScroll = Vector2.zero;
        public float alphaBrushesWindowHeight = 100.0f;

        public void OnGUI(CustomMasks customMasks)
        {
            DrawCustomBrushes(customMasks);
        }

        public void DrawCustomBrushes(CustomMasks customMasks)
		{
			Event e = Event.current;

        	// Settings
        	Color initialGUIColor = GUI.backgroundColor;

			Rect windowRect = EditorGUILayout.GetControlRect(GUILayout.Height(Mathf.Max(0.0f, alphaBrushesWindowHeight)) );
			windowRect = EditorGUI.IndentedRect(windowRect);

			Rect virtualRect = new Rect(windowRect);

			if(IsNecessaryToDrawIconsForCustomBrushes(windowRect, initialGUIColor, customMasks))
			{
				SelectedWindowUtility.DrawLabelForIcons(initialGUIColor, windowRect);
				DrawBrushIcons(e, customMasks, windowRect);
			}

            SelectedWindowUtility.DrawResizeBar(e, alphaBrushesIconHeight, ref alphaBrushesWindowHeight);

			DropOperationForBrush(e, virtualRect, customMasks);

			switch (e.type)
			{
				case EventType.ContextClick:
				{
            		if(virtualRect.Contains(e.mousePosition))
            		{
						BrushesWindowMenu(customMasks).ShowAsContext();
            		    e.Use();
            		}
            		break;
				}
			}

			GUILayout.Space(3);
		}

        private void DrawBrushIcons(Event e, CustomMasks customBrushes, Rect windowRect)
		{
			Rect virtualRect = SelectedWindowUtility.GetVirtualRect(windowRect, customBrushes.customBrushes.Count, alphaBrushesIconWidth, alphaBrushesIconHeight);

			Vector2 brushWindowScrollPos = alphaBrushesWindowsScroll;
            brushWindowScrollPos = GUI.BeginScrollView(windowRect, brushWindowScrollPos, virtualRect, false, true);

			int y = (int)virtualRect.yMin;
			int x = (int)virtualRect.xMin;

			for (int alphaBrushesIndex = 0; alphaBrushesIndex < customBrushes.customBrushes.Count; alphaBrushesIndex++)
			{
				Rect brushIconRect = new Rect(x, y, alphaBrushesIconWidth, alphaBrushesIconHeight);

				Color rectColor;

				if (alphaBrushesIndex == customBrushes.selectedCustomBrush)
				{
					rectColor = EditorColors.Instance.ToggleButtonActiveColor;
				}
        	    else 
				{ 
					rectColor = Color.white; 
				}

				DrawIconRectForBrush(brushIconRect, customBrushes.customBrushes[alphaBrushesIndex], rectColor, e, windowRect, brushWindowScrollPos, () =>
				{
					HandleSelectBrush(alphaBrushesIndex, customBrushes, e);
				});

				SelectedWindowUtility.SetNextXYIcon(virtualRect, alphaBrushesIconWidth, alphaBrushesIconHeight, ref y, ref x);
			}

			alphaBrushesWindowsScroll = brushWindowScrollPos;

			GUI.EndScrollView();
		}

        public void HandleSelectBrush(int brushIndex, CustomMasks customBrushes, Event e)
		{
			switch (e.type)
			{
				case EventType.MouseDown:
				{
					if(e.button == 0)
					{																		
						customBrushes.selectedCustomBrush = brushIndex; 
            			e.Use();
					}

            	    break;
				}
				case EventType.ContextClick:
				{
					BrushesMenu(customBrushes, brushIndex).ShowAsContext();
					e.Use();

            		break;
				}
			}
		}

        private bool IsNecessaryToDrawIconsForCustomBrushes(Rect brushWindowRect, Color initialGUIColor, CustomMasks customBrushes)
		{
			if(customBrushes.customBrushes.Count == 0)
			{
				SelectedWindowUtility.DrawLabelForIcons(initialGUIColor, brushWindowRect, "Drag & Drop Textures Here");
				return false;
			}

			return true;
		}

        private void DrawIconRectForBrush(Rect brushIconRect, Texture2D preview, Color rectColor, Event e, Rect brushWindowRect, Vector2 brushWindowScrollPos, UnityAction clickAction)
		{
			GUIStyle LabelTextForIcon = CustomEditorGUILayout.GetStyle(StyleName.LabelTextForIcon);
			GUIStyle LabelTextForSelectedArea = CustomEditorGUILayout.GetStyle(StyleName.LabelTextForSelectedArea);

            Rect brushIconRectScrolled = new Rect(brushIconRect);
            brushIconRectScrolled.position -= brushWindowScrollPos;

            // only visible incons
            if(brushIconRectScrolled.Overlaps(brushWindowRect))
            {
                if(brushIconRect.Contains(e.mousePosition))
				{
					clickAction.Invoke();
				}

				EditorGUI.DrawRect(brushIconRect, rectColor);
                    
				// Prefab preview 
                if(e.type == EventType.Repaint)
                {
                    Rect previewRect = new Rect(brushIconRect.x+2, brushIconRect.y+2, brushIconRect.width-4, brushIconRect.width-4);

					if(preview != null)
					{
						GUI.DrawTexture(previewRect, preview);
					}
					else
					{
						Color dimmedColor = new Color(0.4f, 0.4f, 0.4f, 1.0f);
						EditorGUI.DrawRect(previewRect, dimmedColor);
					}
                }
			}
		}

        private void DropOperationForBrush(Event e, Rect virtualRect, CustomMasks customBrushes)
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

                        foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences)
                        {
							if (draggedObject is Texture2D)
                            {
								customBrushes.customBrushes.Add((Texture2D)draggedObject);
                            }
                        }

                    }
                    e.Use();
                } 
			}
		}

		private GenericMenu BrushesWindowMenu(CustomMasks customBrushes)
        {
            GenericMenu menu = new GenericMenu();

			menu.AddItem(new GUIContent("Get Polaris Brushes"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => customBrushes.GetPolarisBrushes()));

			menu.AddSeparator ("");

			menu.AddItem(new GUIContent("Delete All"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => { customBrushes.customBrushes.Clear(); }));

            return menu;
        }

        private GenericMenu BrushesMenu(CustomMasks customBrushes, int selectedAlphaBrush)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Delete"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => customBrushes.customBrushes.RemoveAt((selectedAlphaBrush))));

            return menu;
        }
    }
}
#endif
