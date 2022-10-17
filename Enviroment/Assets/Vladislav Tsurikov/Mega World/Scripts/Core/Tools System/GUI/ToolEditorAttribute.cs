using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ToolEditorAttribute : Attribute
    {
        public readonly System.Type SettingsType;

        public readonly string ContextMenu;

        public readonly bool DrawGameObjectGroupSettings;
        public readonly bool DrawInstantItemTypeSettings;
        public readonly bool DrawUnityTerrainDetailTypeSettings;
        public readonly bool DrawUnityTerrainTextureTypeSettings;

        public readonly bool DrawPrototypeSettings;

        public ToolEditorAttribute(System.Type settingsType, string contextMenu, bool drawGroupSettings, bool drawPrototypeSettings)
        {
            ContextMenu = contextMenu;

            SettingsType = settingsType;
            DrawPrototypeSettings = drawPrototypeSettings;

            DrawGameObjectGroupSettings = drawGroupSettings;
            DrawInstantItemTypeSettings = drawGroupSettings;
            DrawUnityTerrainDetailTypeSettings = drawGroupSettings;
            DrawUnityTerrainTextureTypeSettings = drawGroupSettings;
        }

        public ToolEditorAttribute(System.Type settingsType, string contextMenu, bool drawGameObjectGroupSettings, bool drawInstantItemTypeSettings, bool drawUnityTerrainDetailTypeSettings,
            bool drawUnityTerrainTextureTypeSettings, bool drawPrototypeSettings)
        {
            ContextMenu = contextMenu;
            
            SettingsType = settingsType;
            DrawPrototypeSettings = drawPrototypeSettings;

            DrawGameObjectGroupSettings = drawGameObjectGroupSettings;
            DrawInstantItemTypeSettings = drawInstantItemTypeSettings;
            DrawUnityTerrainDetailTypeSettings = drawUnityTerrainDetailTypeSettings;
            DrawUnityTerrainTextureTypeSettings = drawUnityTerrainTextureTypeSettings;
        }

        public bool DrawTypeSettings()
        {
            return DrawGameObjectGroupSettings || DrawInstantItemTypeSettings || DrawUnityTerrainDetailTypeSettings || DrawUnityTerrainTextureTypeSettings;
        }
    }
}

