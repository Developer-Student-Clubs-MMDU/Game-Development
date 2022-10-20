using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace VladislavTsurikov.MegaWorldSystem
{
    public class PrototypeSettingsTypesStack 
    {  
        public class PrototypeSettingsTypes
        {
            public ResourceType ResourceType;
            public List<System.Type> Types = new List<System.Type>();

            public PrototypeSettingsTypes(ResourceType resourceType, List<System.Type> settings)
            {
                ResourceType = resourceType;
                Types = settings;
            }
        }

        private List<PrototypeSettingsTypes> _prototypeSettingsTypesList = new List<PrototypeSettingsTypes>();

        public void AddTypesSettings(ResourceType resourceType, List<System.Type> settings)
        {
            if(settings == null)
            {
                return;
            }

            if(!HasResourceType(resourceType))
            {
                _prototypeSettingsTypesList.Add(new PrototypeSettingsTypes(resourceType, settings));
            }
        }

        public void GetSettingsTypes(ResourceType resourceType, Func<System.Type, bool> func)
        {
            if(HasResourceType(resourceType))
            {
                foreach (System.Type type in GetTypesSettingsFromResourceType(resourceType).Types)
                {
                    func.Invoke(type);
                }
            }
        }

        public PrototypeSettingsTypes GetTypesSettingsFromResourceType(ResourceType resourceType)
        {
            foreach (PrototypeSettingsTypes item in _prototypeSettingsTypesList)
            {
                if(item.ResourceType == resourceType)
                {
                    return item;
                }
            }

            return null;
        }

        public bool HasResourceType(ResourceType resourceType)
        {
            foreach (PrototypeSettingsTypes item in _prototypeSettingsTypesList)
            {
                if(item.ResourceType == resourceType)
                {
                    return true;
                }
            }

            return false;
        }
    }
}