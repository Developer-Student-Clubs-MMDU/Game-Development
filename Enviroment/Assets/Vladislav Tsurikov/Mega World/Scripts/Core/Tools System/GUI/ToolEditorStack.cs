#if UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace VladislavTsurikov.MegaWorldSystem
{
    public sealed class ToolEditorStack
    {
        public List<ToolBaseEditor> Editors;

        public ToolEditorStack(ToolStack stack)
        {
            Reload(stack);
        }

        public void Reload(ToolStack stack)
        {
            Editors = new List<ToolBaseEditor>();

            for (int i = 0; i < stack.Tools.Count; i++)
            {
                Create(stack.Tools[i]);
            }
        }

        public void Create(ToolComponent settings)
        {
            var settingsType = settings.GetType();
            System.Type editorType;

            if (AllToolEditorTypes.EditorTypes.TryGetValue(settingsType, out editorType))
            {
                var editor = (ToolBaseEditor)Activator.CreateInstance(editorType);
                editor.Init(settings);
                Editors.Add(editor);
            }
        }

        public void Remove(int index)
        {
            Editors[index].OnDisable();
            Editors.RemoveAt(index);
        }
    }
}
#endif