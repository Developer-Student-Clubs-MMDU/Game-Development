using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace VladislavTsurikov.MegaWorldSystem
{
    public class SettingsTypesStack
    {
        public Type ToolType;

        public GroupSettingsTypesStack GroupSettingsTypesStack = new GroupSettingsTypesStack();
        public GroupSettingsToolTypesStack GroupSettingsToolTypesStack = new GroupSettingsToolTypesStack();

        public PrototypeToolSettingsTypesStack PrototypeToolSettingsTypesStack = new PrototypeToolSettingsTypesStack();
        public PrototypeSettingsTypesStack PrototypeSettingsTypesStack = new PrototypeSettingsTypesStack();

        public SettingsTypesStack(Type toolType)
        {
            ToolType = toolType;
        }

        public void AddGroupSettingsTypes(List<Type> settingsTypes)
        {
            GroupSettingsTypesStack.AddTypesSettings(settingsTypes);
        }

        public void AddGroupToolSettingsTypes(Type toolType, List<Type> settings)
        {
            GroupSettingsToolTypesStack.AddTypesSettings(toolType, settings);
        }

        public void AddPrototypeToolSettingsTypes(ResourceType resourceType, Type toolType, List<Type> settings)
        {
            PrototypeToolSettingsTypesStack.AddTypesSettings(resourceType, toolType, settings);
        }

        public void AddPrototypeSettingsTypes(ResourceType resourceType, List<Type> settings)
        {
            PrototypeSettingsTypesStack.AddTypesSettings(resourceType, settings);
        }
    }

    public static class AllSettingsTypes 
    {  
        public static List<SettingsTypesStack> SettingsTypesList = new List<SettingsTypesStack>();
    }
}