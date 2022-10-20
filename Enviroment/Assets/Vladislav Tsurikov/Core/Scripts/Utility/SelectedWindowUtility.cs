#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEditor;
using VladislavTsurikov.CustomGUI;
using VladislavTsurikov.Extensions;

namespace VladislavTsurikov
{
    public static class SelectedWindowUtility
    {
        static int s_WindowResizeBarHash = "PPGUI.BrushWindowResize".GetHashCode();
		const int kBrushWindowResizeBarHeight = 8;

        public static void DrawLabelForIcons(Color InitialGUIColor, Rect windowRect, string text = null)
		{
			GUIStyle LabelTextForSelectedArea = CustomEditorGUILayout.GetStyle(StyleName.LabelTextForSelectedArea);
			GUIStyle boxStyle = CustomEditorGUILayout.GetStyle(StyleName.Box);

			GUI.color = EditorColors.Instance.boxColor;
			GUI.Label(windowRect, "", boxStyle);
			GUI.color = InitialGUIColor;

			if(text != null)
			{
				GUI.color = EditorColors.Instance.LabelColor;
				EditorGUI.LabelField(windowRect, text, LabelTextForSelectedArea);
				GUI.color = InitialGUIColor;
			}
		}

        public static Rect GetVirtualRect(Rect brushWindowRect, int count, int iconWidth, int iconHeight)
    	{
    		Rect virtualRect = new Rect(brushWindowRect);
            {
                virtualRect.width = Mathf.Max(virtualRect.width - 20, 1); // space for scroll 

                int presetColumns = Mathf.FloorToInt(Mathf.Max(1, (virtualRect.width) / iconWidth));
                int virtualRows   = Mathf.CeilToInt((float)count / presetColumns);

                virtualRect.height = Mathf.Max(virtualRect.height, iconHeight * virtualRows);
            }

    		return virtualRect;
    	}

    	public static void DrawResizeBar(Event e, int IconHeight, ref float windowHeight)
    	{
    		Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(kBrushWindowResizeBarHeight));
            int controlID = GUIUtility.GetControlID(s_WindowResizeBarHash, FocusType.Passive, rect);

    		EditorGUIUtility.AddCursorRect(rect, UnityEditor.MouseCursor.SplitResizeUpDown);

            switch(e.type)
            {
            	case EventType.MouseDown:
    			{
    			   	if(rect.Contains(e.mousePosition) && e.button == 0)
            		{
            		    GUIUtility.keyboardControl = controlID;
            		    GUIUtility.hotControl = controlID;
            		    e.Use();
            		}
            		break;
    			}
            	case EventType.MouseUp:
    			{
    			   	if (GUIUtility.hotControl == controlID && e.button == 0)
            		{
            		    GUIUtility.hotControl = 0;
            		    e.Use();
            		}
            		break;
    			}
            	case EventType.MouseDrag:
    			{
            		if (GUIUtility.hotControl == controlID)
            		{
						if(CustomEditorGUILayout.IsInspector)
						{
							windowHeight = Mathf.Max(IconHeight, windowHeight + e.delta.y);

            		    	e.Use();
						}
						else
						{
							Rect windowRect = EditorGUIUtility.ScreenToGUIRect(CustomEditorGUILayout.ScreenRect);

            		    	windowHeight = Mathf.Clamp(windowHeight + e.delta.y,
            		    	IconHeight, windowHeight + (windowRect.yMax - rect.yMax)); 

            		    	e.Use();
						}
            		}
            		break;
    			}
            	case EventType.Repaint:
            	{
            	    Rect drawRect = rect;
            	    drawRect.yMax -= 2; drawRect.yMin += 2;
    				drawRect = EditorGUI.IndentedRect(drawRect);
            	    EditorGUI.DrawRect(drawRect, EditorColors.Instance.orangeDark.WithAlpha(0.7f));
    				break;
            	}   
            }
    	}

		public static bool Separator(Event e, Color color)
    	{
    		Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(kBrushWindowResizeBarHeight));
            int controlID = GUIUtility.GetControlID(s_WindowResizeBarHash, FocusType.Passive, rect);

    		EditorGUIUtility.AddCursorRect(rect, UnityEditor.MouseCursor.SplitResizeUpDown);

            switch(e.type)
            {
            	case EventType.MouseDown:
    			{
    			   	if(rect.Contains(e.mousePosition) && e.button == 0)
            		{
            		    GUIUtility.keyboardControl = controlID;
            		    GUIUtility.hotControl = controlID;
            		    e.Use();
            		}
            		break;
    			}
            	case EventType.MouseUp:
    			{
    			   	if (GUIUtility.hotControl == controlID && e.button == 0)
            		{
            		    GUIUtility.hotControl = 0;
            		    e.Use();
            		}
            		break;
    			}
            	case EventType.MouseDrag:
    			{
            		if (GUIUtility.hotControl == controlID)
            		{
						if(e.delta.y != 0)
						{
							e.Use();
							return true;
						}
            		}
            		break;
    			}
            	case EventType.Repaint:
            	{
            	    Rect drawRect = rect;
            	    drawRect.yMax -= 2; drawRect.yMin += 2;
    				drawRect = EditorGUI.IndentedRect(drawRect);
            	    EditorGUI.DrawRect(drawRect, color);
    				break;
            	}   
            }

			return false;
    	}

        public static void DrawIconRect(Rect brushIconRect, Texture2D preview, string name, Color textColor, Color rectColor, Event e, Rect brushWindowRect, Vector2 brushWindowScrollPos, 
			int iconWidth, bool drawTextureWithAlphaChannel, UnityAction clickAction)
		{
			GUIStyle LabelTextForIcon = CustomEditorGUILayout.GetStyle(StyleName.LabelTextForIcon);
			GUIStyle LabelTextForSelectedArea = CustomEditorGUILayout.GetStyle(StyleName.LabelTextForSelectedArea);

            Rect iconRectScrolled = new Rect(brushIconRect);
            iconRectScrolled.position -= brushWindowScrollPos;

            // only visible icons
            if(iconRectScrolled.Overlaps(brushWindowRect))
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

					if (preview != null)
               		{						
						if(drawTextureWithAlphaChannel)
						{
							GUI.DrawTexture(previewRect, preview);
						}
						else
						{
							EditorGUI.DrawPreviewTexture(previewRect, preview);
						}						
               		}
               		else
               		{
               		    Color dimmedColor = new Color(0.4f, 0.4f, 0.4f, 1.0f);
						EditorGUI.DrawRect(previewRect, dimmedColor);
               		}

					LabelTextForIcon.normal.textColor = textColor;
					LabelTextForIcon.Draw(brushIconRect, GetShortNameForIcon(name, iconWidth), false, false, false, false);
                }
			}
		}

		public static string GetShortNameForIcon(string name, int iconWidth)
        {
			Dictionary<string, string> brushShortNamesDictionary = new Dictionary<string, string>();

			GUIStyle LabelTextForIcon = CustomEditorGUILayout.GetStyle(StyleName.LabelTextForIcon);

            if(name == null || name.Length == 0)
                return "";

            string shortString;

            if(brushShortNamesDictionary.TryGetValue(name, out shortString))
                return shortString;

            return brushShortNamesDictionary[name] = TruncateString(name, LabelTextForIcon, iconWidth);
        }

		public static string TruncateString(string str, GUIStyle style, int maxWidth)
        {
            GUIContent ellipsis = new GUIContent("...");
            string shortStr = "";

            float ellipsisSize = style.CalcSize(ellipsis).x;
            GUIContent textContent = new GUIContent("");

            char[] charArray = str.ToCharArray();
            for(int i = 0; i < charArray.Length; i++)
            {
                textContent.text += charArray[i];

                float size = style.CalcSize(textContent).x;

                if (size > maxWidth - ellipsisSize)
                {
                    shortStr += ellipsis.text;
                    break;
                }

                shortStr += charArray[i];
            }

            return shortStr;
        }

        public static void SetDragRect(Event e, Rect brushIconRect, ref Rect dragRect, out bool isAfter)
		{
			isAfter = (e.mousePosition.x - brushIconRect.xMin) > brushIconRect.width / 2;

            dragRect = new Rect(brushIconRect);

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
		}

        public static void SetNextXYIcon(Rect virtualRect, int iconWidth, int iconHeight, ref int y, ref int x)
		{
			if(x + iconWidth < (int)virtualRect.xMax - iconWidth)
            {
                x += iconWidth;
            }
            else if(y < (int)virtualRect.yMax)
            {
                y += iconHeight;
                x = (int)virtualRect.xMin;
            }
		}

		public static string DelayedTextField(Rect position, string text)
		{
			#if (UNITY_5_4_OR_NEWER)
            return EditorGUI.DelayedTextField (position, text);
			#else
			return EditorGUI.TextField(position, text);
			#endif
		}

		public static void AddDelayedAction(EditorApplication.CallbackFunction action)
        {
            EditorApplication.delayCall += action;
        }
    }
}
#endif