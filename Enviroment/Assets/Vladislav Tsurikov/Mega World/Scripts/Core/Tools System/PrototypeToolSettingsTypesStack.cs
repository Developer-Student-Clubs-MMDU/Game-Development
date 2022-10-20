using System;
using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.MegaWorldSystem
{
    public class PrototypeToolSettingsTypesStack 
    {        
        public class ToolSettingsTypes
        {
            public System.Type ToolType;
            public ResourceType ResourceType;
            public PrototypeSettingsTypesStack TypesSettingsManager = new PrototypeSettingsTypesStack();

            public ToolSettingsTypes(System.Type toolType, ResourceType resourceType)
            {
                ToolType = toolType;
                ResourceType = resourceType;
            }
        }

        private List<ToolSettingsTypes> _toolSettingsTypesList = new List<ToolSettingsTypes>();

        public void AddTypesSettings(ResourceType resourceType, System.Type toolType, List<System.Type> settings)
        {
            if(settings == null)
            {
                return;
            }

            if(!HasSameData(toolType, resourceType))
            {
                ToolSettingsTypes toolSettingsTypes = new ToolSettingsTypes(toolType, resourceType);
                toolSettingsTypes.TypesSettingsManager.AddTypesSettings(resourceType, settings);
                
                _toolSettingsTypesList.Add(toolSettingsTypes);
            }
        }

        public void GetSettingsTypes(System.Type toolType, ResourceType resourceType, Func<System.Type, bool> func)
        {
            if(HasSameData(toolType, resourceType))
            {
                foreach (ToolSettingsTypes item in _toolSettingsTypesList)
                {
                    if(item.ToolType == toolType)
                    {
                        item.TypesSettingsManager.GetSettingsTypes(resourceType, (type) => 
                        {
                            func.Invoke(type);

                            return true;
                        });
                    }
                }
            }
        }

        public bool HasSameData(System.Type toolType, ResourceType resourceType)
        {
            foreach (ToolSettingsTypes item in _toolSettingsTypesList)
            {
                if(item.ToolType == toolType && item.ResourceType == resourceType)
                {
                    return true;
                }
            }

            return false;
        }
    }
}