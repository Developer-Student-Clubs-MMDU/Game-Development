#if UNITY_EDITOR
using UnityEngine;
using UnityEditorInternal;
using UnityEditor;
using VladislavTsurikov.CustomGUI;
using System.Collections.Generic;

namespace VladislavTsurikov.MegaWorldSystem
{
    public class TransformComponentStackEditor
    {
        private static class Styles
        {
            public static Texture2D move;
            
            static Styles()
            {
                move = Resources.Load<Texture2D>("Images/move");
            }
        }

        private bool _headerFoldout = true;
        private ReorderableList _reorderableList;
        private GenericMenu _genericMenu;
        private GUIContent _reorderableListName;
        private bool _dragging;
        private bool _useSimpleComponent;

        public AllTransformComponentsTypes AllTransformComponentsTypes;
        public TransformComponentStack Stack;

        public TransformComponentStackEditor(Prototype proto, GUIContent reorderableListName, TransformComponentStack stack, List<System.Type> removeTransformTypes, bool useSimpleComponent)
        {
            Stack = stack;

            _useSimpleComponent = useSimpleComponent;
            _reorderableListName = reorderableListName;
            _reorderableList = new ReorderableList( stack.Settings, stack.Settings.GetType(), true, true, true, true );
            AllTransformComponentsTypes = new AllTransformComponentsTypes(removeTransformTypes, useSimpleComponent);

            SetupCallbacks();
        }

        public TransformComponentStackEditor(Prototype proto, GUIContent reorderableListName, TransformComponentStack stack, bool useSimpleComponent)
        {
            Stack = stack;

            _useSimpleComponent = useSimpleComponent;
            _reorderableListName = reorderableListName;
            _reorderableList = new ReorderableList( stack.Settings, stack.Settings.GetType(), true, true, true, true );
            AllTransformComponentsTypes = new AllTransformComponentsTypes(useSimpleComponent);

            SetupCallbacks();
        }

        public TransformComponentStackEditor(Prototype proto, GUIContent reorderableListName, TransformComponentStack stack)
        {
            Stack = stack;

            _useSimpleComponent = false;
            _reorderableListName = reorderableListName;
            _reorderableList = new ReorderableList( stack.Settings, stack.Settings.GetType(), true, true, true, true );
            AllTransformComponentsTypes = new AllTransformComponentsTypes();

            SetupCallbacks();
        }

        private void InitGenericMenu(Prototype proto)
        {
            _genericMenu = new GenericMenu();

            for(int i = 0; i < AllTransformComponentsTypes.TypeList.Count; ++i)
            {
                System.Type transformType = AllTransformComponentsTypes.TypeList[i];

                string name = transformType.GetAttribute<TransformComponentAttribute>().Name;

                if(!_useSimpleComponent)
                {
                    if(transformType.GetAttribute<TransformComponentAttribute>().SimpleComponent)
                    {
                        name = "Simple/" + transformType.GetAttribute<TransformComponentAttribute>().Name;
                    }
                    else
                    {
                        name = "Advanced/" + transformType.GetAttribute<TransformComponentAttribute>().Name;
                    }
                }
                
                bool exists = Stack.HasSettings(transformType);

                if (!exists)
                {
                    _genericMenu.AddItem(new GUIContent(name), false, () => Stack.AddSettings(transformType, proto));
                }
                else
                {
                    _genericMenu.AddDisabledItem(new GUIContent(name));
                }
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

        public void OnGUI(Prototype proto)
		{
			if(_headerFoldout)
			{
				Rect rect = EditorGUILayout.GetControlRect(true, _reorderableList.GetHeight());
				rect = EditorGUI.IndentedRect(rect);

				InitGenericMenu(proto);

                _reorderableList.DoList(rect);
			}
			else
			{
				_headerFoldout = CustomEditorGUILayout.Foldout(_headerFoldout, _reorderableListName.text);
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

            _headerFoldout = EditorGUI.Foldout(rect, _headerFoldout, _reorderableListName.text, true);
        }

        private float ElementHeightCB(int index)
        {
            TransformComponent component = GetComponentAtIndex(index);

            float height = EditorGUIUtility.singleLineHeight * 1.5f;

            if(component == null)
            {
                return EditorGUIUtility.singleLineHeight * 2;
            }

            if(!component.FoldoutGUI)
            {
                return EditorGUIUtility.singleLineHeight + 5;
            }
            else
            {
                height += component.GetElementHeight(index);
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

            TransformComponent component = GetComponentAtIndex(index);
            
            if(component == null)
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
                component.Enabled = EditorGUI.Toggle(toggleRect, "", component.Enabled);

                GUI.color = new Color(1f, 1f, 1f, 1f);

                component.FoldoutGUI = EditorGUI.Foldout(headerRect, component.FoldoutGUI, component.GetType().GetAttribute<TransformComponentAttribute>().Name, true);
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
                Undo.RecordObject(component, "Filter Changed");

                if(component.FoldoutGUI)
                {
                    EditorGUI.BeginChangeCheck();
                    component.DoGUI(totalRect, index);
                    changed |= EditorGUI.EndChangeCheck();
                }
            }

            if(changed)
            {
                EditorUtility.SetDirty(component);
            }

            GUI.color = prevColor;
        }

        private TransformComponent GetComponentAtIndex(int index)
        {
            return Stack.Settings[index];
        }

        private void AddCB(ReorderableList list)
        {
            _genericMenu.ShowAsContext();
        }
    }
}
#endif