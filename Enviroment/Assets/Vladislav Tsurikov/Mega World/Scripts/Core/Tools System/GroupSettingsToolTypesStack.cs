using System;
using System.Collections.Generic;

namespace VladislavTsurikov.MegaWorldSystem
{
    public class GroupSettingsToolTypesStack 
    {        
        public class ToolTypesSettings
        {
            public System.Type ToolType;
            public GroupSettingsTypesStack TypesSettingsManager = new GroupSettingsTypesStack();

            public ToolTypesSettings(System.Type toolType)
            {
                ToolType = toolType;
            }
        }

        private List<ToolTypesSettings> _toolTypesSettingsList = new List<ToolTypesSettings>();

        public void AddTypesSettings(System.Type toolType, List<System.Type> settings)
        {
            if(settings == null)
            {
                return;
            }

            if(!HasTool(toolType))
            {
                ToolTypesSettings toolSettingsTypes = new ToolTypesSettings(toolType);
                toolSettingsTypes.TypesSettingsManager.AddTypesSettings(settings);
                
                _toolTypesSettingsList.Add(toolSettingsTypes);
            }
        }

        public void GetSettingsTypes(System.Type toolType, Func<System.Type, bool> func)
        {
            if(HasTool(toolType))
            {
                foreach (ToolTypesSettings item in _toolTypesSettingsList)
                {
                    if(item.ToolType == toolType)
                    {
                        item.TypesSettingsManager.GetSettingsTypes((type) => 
                        {
                            func.Invoke(type);

                            return true;
                        });
                    }
                }
            }
        }

        public bool HasTool(System.Type toolType)
        {
            foreach (ToolTypesSettings item in _toolTypesSettingsList)
            {
                if(item.ToolType == toolType)
                {
                    return true;
                }
            }

            return false;
        }
    }
}