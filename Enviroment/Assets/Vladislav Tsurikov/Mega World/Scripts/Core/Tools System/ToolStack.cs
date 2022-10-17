using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class ToolStack 
    {
        [SerializeField]
        public List<ToolComponent> Tools = new List<ToolComponent>();

#if UNITY_EDITOR
        public ToolStack()
        {
            EditorApplication.update -= DisableToolsIfNecessary;
            EditorApplication.update += DisableToolsIfNecessary;
        }

        public void DisableToolsIfNecessary()
        {
            if (UnityEditor.Tools.current != Tool.None)
            {
                for (int i = 0; i < Tools.Count; i++)
                {
                    if(Tools[i].Enabled)
                    {
                        if(Tools[i].GetType().GetAttribute<ToolAttribute>().DisableToolIfUnityToolActive)
                        {
                            Tools[i].Enabled = false;
                        }

                        return;
                    }
                }
            }
        }
#endif

        public void DoSelectedTool()
        {
            MegaWorldPath.CommonDataPackage.SelectedTool = GetSelected();

            if (MegaWorldPath.CommonDataPackage.SelectedTool == null)
            {
                return;
            }

            for (int i = 0; i < Tools.Count; i++)
            {
                if(Tools[i].Enabled)
                {
                    if(Tools[i].IsToolSupportSelectedData())
                    {
                        Tools[i].InternalHandleKeyboardEvents();
                        Tools[i].InternalDoTool();
                    }

                    return;
                }
            }
        }

        public bool HasSettings(System.Type type)
        {
            foreach (var setting in Tools)
            {
                if (setting.GetType() == type)
                    return true;
            }

            return false;
        }

        public ToolComponent GetSelected()
        {
            foreach (ToolComponent tool in Tools)
            {
                if(tool.Enabled)
                {
                    return tool;
                }
            }

            return null;
        }

        public void RemoveNullTool()
        {
            Tools.RemoveAll(tool => tool == null);
        }

#if UNITY_EDITOR
        public ToolComponent Create(System.Type type)
        {
            ToolComponent asset = (ToolComponent)ModulesUtility.CreateAsset(type, MegaWorldPath.CommonDataPackage);
            Tools.Add(asset);

            CreateMegaWorldWindowSettings.CreateSettings();

            return asset;
        }

        public void Remove(int index)
        {
            var asset = Tools[index];
            Tools.RemoveAt(index);

            ModulesUtility.RemoveAsset(asset);
        }
#endif
    }
}
