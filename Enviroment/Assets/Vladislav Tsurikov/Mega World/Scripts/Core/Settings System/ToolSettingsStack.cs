using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class ToolSettings
    {
        public string ToolTypeName;
        public SettingsStack SettingsStack = new SettingsStack();

        public ToolSettings(System.Type toolType)
        {
            ToolTypeName = toolType.ToString();
        }
    }

    [Serializable]
    public class ToolSettingsStack 
    {
        public List<ToolSettings> ToolSettings = new List<ToolSettings>();

        public bool HasSettings(System.Type toolType)
        {
            foreach (var setting in ToolSettings)
            {
                if(setting.ToolTypeName == toolType.ToString())
                {
                    return true;
                }
            }

            return false;
        }

        public BaseSettings GetSettings(System.Type toolType, System.Type settingsType)
        {
            foreach (var setting in ToolSettings)
            {
                if(setting.ToolTypeName == toolType.ToString())
                {
                    return setting.SettingsStack.GetSettings(settingsType);
                }
            } 

            return null;
        }

        public SettingsStack GetSettingsStack(System.Type toolType)
        {
            foreach (var setting in ToolSettings)
            {
                if(setting.ToolTypeName == toolType.ToString())
                {
                    return setting.SettingsStack;
                }
            }

            return null;
        }

#if UNITY_EDITOR
        public void Create(System.Type toolType, System.Type systemType, ScriptableObject asset)
        {
            if(!HasSettings(toolType))
            {
                ToolSettings toolSettings = new ToolSettings(toolType);
                ToolSettings.Add(toolSettings);

                toolSettings.SettingsStack.Create(systemType, asset);
            }
            else
            {
                GetSettingsStack(toolType).Create(systemType, asset);
            }
        }
#endif
    }
}