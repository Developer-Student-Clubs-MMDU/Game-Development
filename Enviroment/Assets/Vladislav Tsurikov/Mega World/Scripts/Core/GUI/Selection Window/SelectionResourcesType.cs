#if UNITY_EDITOR
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov;
using VladislavTsurikov.CustomGUI;

namespace VladislavTsurikov.MegaWorldSystem
{
    public static class SelectionResourcesType 
    {
        public static void DrawResourcesType(Group group)
        {
            string name = GetName(group.ResourceType);
            Rect rect = GUILayoutUtility.GetRect(140, 20, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
            CustomEditorGUILayout.RectTabWithFoldout(rect, name, 20, new Action(() => ResourcesTypeMenu(group)));
        }

        private static void ResourcesTypeMenu(Group group)
        {
            GenericMenu menu = new GenericMenu();

            foreach (ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
            {
                menu.AddItem(new GUIContent(GetName(resourceType)), group.ResourceType == resourceType, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => 
                    group.ResourceType = resourceType
                ));
            }

            menu.ShowAsContext();
        }

        public static string GetName(ResourceType resourceType)
        {
            switch (resourceType)
            {
                case ResourceType.InstantItem:
                {
                    return "Instant Item";
                }
                case ResourceType.GameObject:
                {
                    return "GameObject";
                }
                case ResourceType.TerrainDetail:
                {
                    return "Terrain Detail";
                }
                case ResourceType.TerrainTexture:
                {
                    return "Terrain Texture";
                }
            }

            return "NONE";
        }
    }
}
#endif