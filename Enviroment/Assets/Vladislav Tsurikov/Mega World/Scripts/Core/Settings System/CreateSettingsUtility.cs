#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VladislavTsurikov.MegaWorldSystem
{
    public static class CreateSettingsUtility
	{
        public static void CreateGroupSettings(GroupSettingsTypesStack stack, Group group)
        {
            stack.GetSettingsTypes((settingsType) => 
            {
                group.SettingsStack.Create(settingsType, group);

                return true;
            });
        }

        public static void CreateToolGroupSettings(GroupSettingsToolTypesStack stack, System.Type toolType, Group group)
        {
            stack.GetSettingsTypes(toolType, (settingsType) => 
            {
                group.ToolSettingsStack.Create(toolType, settingsType, group);

                return true;
            });
        }

        public static void CreatePrototypeSettings(PrototypeSettingsTypesStack stack, ResourceType resourceType, Prototype proto)
        {
            stack.GetSettingsTypes(resourceType, (settingsType) => 
            {
                proto.SettingsStack.Create(settingsType, proto);

                return true;
            });
        }

        public static void CreateToolPrototypeSettings(PrototypeToolSettingsTypesStack stack, System.Type toolType, ResourceType resourceType, Prototype proto)
        {            
            stack.GetSettingsTypes(toolType, resourceType, (settingsType) => 
            {
                proto.ToolSettingsStack.Create(toolType, settingsType, proto);
 
                return true;
            });
        }
    }
}
#endif