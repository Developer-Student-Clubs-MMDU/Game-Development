#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VladislavTsurikov.MegaWorldSystem
{
    public static class CreateMegaWorldWindowSettings
	{
        public static void CreateSettings()
        {            
            foreach (SettingsTypesStack settingsTypesStack in AllSettingsTypes.SettingsTypesList)
            {
                foreach (Group group in AllAvailableTypes.GetAllTypes())
                {
                    CreateGroupSettings(settingsTypesStack, group);
                }

                foreach (Prototype proto in AllAvailablePrototypes.ProtoList)
                {
                    CreatePrototypeSettings(settingsTypesStack, proto);
                }
            }
        }

        public static void CreatePrototypeSettings(Prototype proto)
        {
            foreach (SettingsTypesStack settingsTypesStack in AllSettingsTypes.SettingsTypesList)
            {
                CreatePrototypeSettings(settingsTypesStack, proto);
            }
        }

        public static void CreateGroupSettings(Group group)
        {
            foreach (SettingsTypesStack settingsTypesStack in AllSettingsTypes.SettingsTypesList)
            {
                CreateGroupSettings(settingsTypesStack, group);
            }
        }

        public static void CreateGroupSettings(SettingsTypesStack settingsTypesStack, Group group)
        {
            CreateSettingsUtility.CreateGroupSettings(settingsTypesStack.GroupSettingsTypesStack, group);
            CreateSettingsUtility.CreateToolGroupSettings(settingsTypesStack.GroupSettingsToolTypesStack, settingsTypesStack.ToolType, group);
        }

        public static void CreatePrototypeSettings(SettingsTypesStack settingsTypesStack, Prototype proto)
        {
            if(proto is PrototypeInstantItem)
            {
                CreateSettingsUtility.CreatePrototypeSettings(settingsTypesStack.PrototypeSettingsTypesStack, ResourceType.InstantItem, proto);
                CreateSettingsUtility.CreateToolPrototypeSettings(settingsTypesStack.PrototypeToolSettingsTypesStack, settingsTypesStack.ToolType, ResourceType.InstantItem, proto);
            }
            else if(proto is PrototypeGameObject)
            {
                CreateSettingsUtility.CreatePrototypeSettings(settingsTypesStack.PrototypeSettingsTypesStack, ResourceType.GameObject, proto);
                CreateSettingsUtility.CreateToolPrototypeSettings(settingsTypesStack.PrototypeToolSettingsTypesStack, settingsTypesStack.ToolType, ResourceType.GameObject, proto);
            }
            else if(proto is PrototypeTerrainDetail)
            {
                CreateSettingsUtility.CreatePrototypeSettings(settingsTypesStack.PrototypeSettingsTypesStack, ResourceType.TerrainDetail, proto);
                CreateSettingsUtility.CreateToolPrototypeSettings(settingsTypesStack.PrototypeToolSettingsTypesStack, settingsTypesStack.ToolType, ResourceType.TerrainDetail, proto);
            }
            else if(proto is PrototypeTerrainTexture)
            {
                CreateSettingsUtility.CreatePrototypeSettings(settingsTypesStack.PrototypeSettingsTypesStack, ResourceType.TerrainTexture, proto);
                CreateSettingsUtility.CreateToolPrototypeSettings(settingsTypesStack.PrototypeToolSettingsTypesStack, settingsTypesStack.ToolType, ResourceType.TerrainTexture, proto);
            }
        }
    }
}
#endif