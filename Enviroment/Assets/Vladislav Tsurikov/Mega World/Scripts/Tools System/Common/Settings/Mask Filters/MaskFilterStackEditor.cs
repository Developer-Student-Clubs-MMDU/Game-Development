#if UNITY_EDITOR
using UnityEngine;
using UnityEditorInternal;
using UnityEditor;
using VladislavTsurikov.CustomGUI;

namespace VladislavTsurikov.MegaWorldSystem
{
    public class MaskFilterStackEditor
    {
        private static class Styles
        {
            public static Texture2D move;
            
            static Styles()
            {
                move = Resources.Load<Texture2D>("Images/move");
            }
        }

        public bool headerFoldout = true;

        private ReorderableList _reorderableList;
        private GenericMenu _ContextMenu;
        private GUIContent _label;
        private bool _dragging;

        public MaskFilterStack Stack;

        public MaskFilterStackEditor(GUIContent label, MaskFilterStack filterStack)
        {
            _label = label;
            Stack = filterStack;
            _reorderableList = new ReorderableList( filterStack.Settings, filterStack.Settings.GetType(), true, true, true, true );

            SetupCallbacks();
        }

        private void InitGenericMenu(ScriptableObject asset)
        {
            _ContextMenu = new GenericMenu();

            for(int i = 0; i < AllFilterTypes.TypeList.Count; ++i)
            {
                System.Type type = AllFilterTypes.TypeList[i];
                string name = type.GetAttribute<MaskFilterAttribute>().Name;

                _ContextMenu.AddItem(new GUIContent(name), false, () => Stack.AddSettings(type, asset));
            }
        }

        private void SetupCallbacks()
        {
            _reorderableList.drawHeaderCallback = DrawHeaderCB;
            _reorderableList.drawElementCallback = DrawElementCB;
            _reorderableList.elementHeightCallback = ElementHeightCB;
            _reorderableList.onAddCallback = AddCB;
            _reorderableList.onRemoveCallback = RemoveFilter;
        }

        public void OnGUI(ScriptableObject asset)
		{
			if(headerFoldout)
			{
				Rect maskRect = EditorGUILayout.GetControlRect(true, _reorderableList.GetHeight());
				maskRect = EditorGUI.IndentedRect(maskRect);

				InitGenericMenu(asset);

                _reorderableList.DoList(maskRect);
			}
			else
			{
				headerFoldout = CustomEditorGUILayout.Foldout(headerFoldout, _label.text);
			}
		}

        void RemoveFilter(ReorderableList list)
        {
            Stack.RemoveSettings(list.index);
        }

        private void DrawHeaderCB(Rect rect)
        {
            if(CustomEditorGUILayout.IsInspector == false)
            {
                rect.x -= 15;
            }

            headerFoldout = EditorGUI.Foldout(rect, headerFoldout, _label.text, true);
        }

        private float ElementHeightCB(int index)
        {
            MaskFilter filter = GetFilterAtIndex(index);

            float height = EditorGUIUtility.singleLineHeight * 1.5f;

            if(filter == null)
            {
                return EditorGUIUtility.singleLineHeight * 2;
            }

            if(!filter.FoldoutGUI)
            {
                return EditorGUIUtility.singleLineHeight + 5;
            }
            else
            {
                height += filter.GetElementHeight(index);
                return height;
            }
        }

        private void DrawElementCB(Rect totalRect, int index, bool isActive, bool isFocused)
        {
            float dividerSize = 1f;
            float paddingV = 6f;
            float paddingH = 4f;
            float iconSize = 14f;

            bool isSelected = _reorderableList.index == index;

            Color bgColor;

            if(EditorGUIUtility.isProSkin)
            {
                if(isSelected)
                {
                    ColorUtility.TryParseHtmlString("#424242", out bgColor);
                }
                else
                {
                    ColorUtility.TryParseHtmlString("#383838", out bgColor);
                }
            }
            else
            {
                if(isSelected)
                {
                    ColorUtility.TryParseHtmlString("#b4b4b4", out bgColor);
                }
                else
                {
                    ColorUtility.TryParseHtmlString("#c2c2c2", out bgColor);
                }
            }

            Color dividerColor;

            if(isSelected)
            {
                dividerColor = EditorColors.Instance.ToggleButtonActiveColor;
            }
            else
            {
                if(EditorGUIUtility.isProSkin)
                {
                    ColorUtility.TryParseHtmlString("#202020", out dividerColor);
                }
                else
                {
                    ColorUtility.TryParseHtmlString("#a8a8a8", out dividerColor);
                }
            }

            Color prevColor = GUI.color;

            // modify total rect so it hides the builtin list UI
            totalRect.xMin -= 20f;
            totalRect.xMax += 4f;
            
            bool containsMouse = totalRect.Contains(Event.current.mousePosition);

            // modify currently selected element if mouse down in this elements GUI rect
            if(containsMouse && Event.current.type == EventType.MouseDown)
            {
                _reorderableList.index = index;
            }

            // draw list element separator
            Rect separatorRect = totalRect;
            // separatorRect.height = dividerSize;
            GUI.color = dividerColor;
            GUI.DrawTexture(separatorRect, Texture2D.whiteTexture, ScaleMode.StretchToFill);
            GUI.color = prevColor;

            // Draw BG texture to hide ReorderableList highlight
            totalRect.yMin += dividerSize;
            totalRect.xMin += dividerSize;
            totalRect.xMax -= dividerSize;
            totalRect.yMax -= dividerSize;
            
            GUI.color = bgColor;
            GUI.DrawTexture(totalRect, Texture2D.whiteTexture, ScaleMode.StretchToFill, false);

            GUI.color = new Color(.7f, .7f, .7f, 1f);

            MaskFilter filter = GetFilterAtIndex( index );
            
            if(filter == null)
            {
                return;
            }

            bool changed = false;
            
            Rect moveRect = new Rect(totalRect.xMin + paddingH, totalRect.yMin + paddingV, iconSize, iconSize );

            // draw move handle rect
            EditorGUIUtility.AddCursorRect(moveRect, UnityEditor.MouseCursor.Pan);
            GUI.DrawTexture(moveRect, Styles.move, ScaleMode.StretchToFill);

            Rect toggleRect = totalRect;

            toggleRect.x += 20;
            toggleRect.height = EditorGUIUtility.singleLineHeight * 1.3f;
            toggleRect.width = 30;

            Rect headerRect = toggleRect;
            headerRect.height = EditorGUIUtility.singleLineHeight * 1.3f;

            if(CustomEditorGUILayout.IsInspector)
            {
                headerRect.x += 30;
            }
            else
            {
                headerRect.x += 20;
            }

            EditorGUI.BeginChangeCheck();
            {
                filter.Enabled = EditorGUI.Toggle(toggleRect, "", filter.Enabled);

                GUI.color = new Color(1f, 1f, 1f, 1f);

                filter.FoldoutGUI = EditorGUI.Foldout(headerRect, filter.FoldoutGUI, filter.GetType().GetAttribute<MaskFilterAttribute>().Name + " " + filter.GetAdditionalName(), true);
            }

            changed |= EditorGUI.EndChangeCheck();
            
            // update dragging state
            if(containsMouse && isSelected)
            {
                if (Event.current.type == EventType.MouseDrag && !_dragging && isFocused)
                {
                    _dragging = true;
                    _reorderableList.index = index;
                }
            }

            if(_dragging)
            {
                if(Event.current.type == EventType.MouseUp)
                {
                    _dragging = false;
                }
            }

            using( new EditorGUI.DisabledScope( !Stack.Settings[index].Enabled) )
            {
                float rectX = 50;

                if(CustomEditorGUILayout.IsInspector == false)
                {
                    rectX = 53;
                }

                totalRect.x += rectX;
                totalRect.y += 2;
                totalRect.width -= rectX + 15;
                totalRect.height = EditorGUIUtility.singleLineHeight;

                totalRect.y += EditorGUIUtility.singleLineHeight * 1.5f;
                
                GUI.color = prevColor;
                Undo.RecordObject(filter, "Filter Changed");

                if(filter.FoldoutGUI)
                {
                    EditorGUI.BeginChangeCheck();
                    filter.DoGUI(totalRect, index);
                    changed |= EditorGUI.EndChangeCheck();
                }
            }

            if(changed)
            {
                EditorUtility.SetDirty(filter);
            }

            GUI.color = prevColor;
        }

        private MaskFilter GetFilterAtIndex(int index)
        {
            return Stack.Settings[index];
        }

        private void AddCB(ReorderableList list)
        {
            _ContextMenu.ShowAsContext();
        }
    }
}
#endif