#if UNITY_EDITOR
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    public static class WindowMenu 
    {
        public static GenericMenu GroupWindowMenu(BasicData data)
        {
            GenericMenu menu = new GenericMenu();

			menu.AddItem(new GUIContent("Add Group/Instant Item"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => SelectionTypeUtility.AddGroup(data.GroupList, ResourceType.InstantItem))); 
            menu.AddItem(new GUIContent("Add Group/GameObject"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => SelectionTypeUtility.AddGroup(data.GroupList, ResourceType.GameObject))); 
            menu.AddItem(new GUIContent("Add Group/Terrain Detail"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => SelectionTypeUtility.AddGroup(data.GroupList, ResourceType.TerrainDetail))); 
            menu.AddItem(new GUIContent("Add Group/Terrain Texture"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => SelectionTypeUtility.AddGroup(data.GroupList, ResourceType.TerrainTexture))); 

            return menu;
        }

		public static GenericMenu GroupMenu(BasicData data, SelectedVariables selectedVariables, ClipboardBase clipboard, int typeIndex)
        {
            GenericMenu menu = new GenericMenu();

			Group group = data.GroupList[typeIndex];

            menu.AddItem(new GUIContent("Reveal in Project"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => EditorGUIUtility.PingObject(group)));
            menu.AddSeparator ("");

            menu.AddItem(new GUIContent("Delete"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => SelectionTypeUtility.DeleteSelectedGroups(data.GroupList)));

            if(selectedVariables.HasOneSelectedGroup())
            {
                switch (group.ResourceType)
                {
                    case ResourceType.InstantItem:
                    {
                        clipboard?.GroupInstantItemMenu(menu, selectedVariables);
                        break;
                    }
                    case ResourceType.GameObject:
                    {
                        clipboard?.GroupGameObjectMenu(menu, selectedVariables);
                        break;
                    }
                    case ResourceType.TerrainDetail:
                    {
                        clipboard?.GroupUnityTerrainDetailMenu(menu, selectedVariables);
                        break;
                    }

                    case ResourceType.TerrainTexture:
                    {
                        clipboard?.GroupUnityTerrainTextureMenu(menu, selectedVariables);
                        break;
                    }
                }

                menu.AddSeparator ("");
			    menu.AddItem(new GUIContent("Rename"), data.GroupList[typeIndex].Renaming, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => data.GroupList[typeIndex].Renaming = !data.GroupList[typeIndex].Renaming));
            }

            return menu;
        }

        public static GenericMenu PrototypeMenu(Type toolType, Group group, SelectedVariables selectedTypeVariables, ClipboardBase clipboard, TemplateStackEditor templateStackEditor, int prototypeIndex)
        {
            switch (group.ResourceType)
            {
                case ResourceType.InstantItem:
                {
                    return PrototypeInstantItemMenu(toolType, group, selectedTypeVariables, clipboard, templateStackEditor, prototypeIndex);
                }
                case ResourceType.GameObject:
                {
                    return PrototypeGameObjectMenu(toolType, group, selectedTypeVariables, clipboard, templateStackEditor, prototypeIndex);
                }
                case ResourceType.TerrainDetail:
                {
                    return PrototypeTerrainDetailMenu(toolType, group, selectedTypeVariables, clipboard, prototypeIndex);
                }

                case ResourceType.TerrainTexture:
                {
                    return PrototypeTerrainTextureMenu(toolType, group, selectedTypeVariables, clipboard, prototypeIndex);
                }
            }

            return null;
        }

		public static GenericMenu PrototypeGameObjectMenu(Type toolType, Group group, SelectedVariables selectedTypeVariables, ClipboardBase clipboard, TemplateStackEditor templateStackEditor, int prototypeIndex)
        {
            GenericMenu menu = new GenericMenu();

			PrototypeGameObject localProto = group.ProtoGameObjectList[prototypeIndex];
			
            GameObject prefab = localProto.Prefab;

            if(prefab != null)
            {
                menu.AddItem(new GUIContent("Reveal in Project"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => EditorGUIUtility.PingObject(prefab)));
                menu.AddSeparator ("");
            }

            menu.AddItem(new GUIContent("Delete"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => SelectionPrototypeUtility.DeleteSelectedProtoGameObject(group)));

            clipboard?.PrototypeGameObjectMenu(menu, selectedTypeVariables);
            templateStackEditor?.ShowManu(menu, toolType, selectedTypeVariables);
			
			menu.AddSeparator ("");
            menu.AddItem(new GUIContent("Select All"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => SelectionPrototypeUtility.SetSelectedAllPrototypes(group, true)));
			menu.AddItem(new GUIContent("Active"), localProto.Active, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => 
				selectedTypeVariables.SelectedProtoGameObjectList.ForEach ((proto) => { proto.Active = !proto.Active;})));


            return menu;
        }

        public static GenericMenu PrototypeInstantItemMenu(Type toolType, Group group, SelectedVariables selectedTypeVariables, ClipboardBase clipboard, TemplateStackEditor templateStackEditor, int prototypeIndex)
        {			
            GenericMenu menu = new GenericMenu();

			PrototypeInstantItem localProto = group.ProtoInstantItemList[prototypeIndex];
			
            GameObject prefab = localProto.Prefab;

            if(prefab != null)
            {
                menu.AddItem(new GUIContent("Reveal in Project"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => EditorGUIUtility.PingObject(prefab)));
                menu.AddSeparator ("");
            }

            menu.AddItem(new GUIContent("Delete"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => SelectionPrototypeUtility.DeleteSelectedProtoInstantItem(group)));

            clipboard?.PrototypeInstantItemMenu(menu, selectedTypeVariables);
            templateStackEditor?.ShowManu(menu, toolType, selectedTypeVariables);
            
			menu.AddSeparator ("");
            menu.AddItem(new GUIContent("Select All"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => SelectionPrototypeUtility.SetSelectedAllPrototypes(group, true)));
			menu.AddItem(new GUIContent("Active"), localProto.Active, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => 
				selectedTypeVariables.SelectedProtoInstantItemList.ForEach ((proto) => { proto.Active = !proto.Active;})));

            return menu;
        }

		public static GenericMenu PrototypeTerrainDetailMenu(Type toolType, Group group, SelectedVariables selectedTypeVariables, ClipboardBase clipboard, int prototypeIndex)
        {
            GenericMenu menu = new GenericMenu();
			
			PrototypeTerrainDetail localProto = group.ProtoTerrainDetailList[prototypeIndex];

			if(localProto.DetailTexture != null)
           	{
           	    menu.AddItem(new GUIContent("Reveal in Project"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => EditorGUIUtility.PingObject(localProto.DetailTexture)));
           	    menu.AddSeparator ("");
           	}
			else if(localProto.Prefab != null)
            {
                menu.AddItem(new GUIContent("Reveal in Project"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => EditorGUIUtility.PingObject(localProto.Prefab)));
                menu.AddSeparator ("");
            }

            menu.AddItem(new GUIContent("Delete"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => SelectionPrototypeUtility.DeleteSelectedProtoTerrainDetail(group)));

            clipboard?.PrototypeUnityTerrainDetailMenu(menu, selectedTypeVariables);
			
			menu.AddSeparator ("");
            menu.AddItem(new GUIContent("Select All"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => SelectionPrototypeUtility.SetSelectedAllPrototypes(group, true)));
			menu.AddItem(new GUIContent("Active"), localProto.Active, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => 
				selectedTypeVariables.SelectedProtoTerrainDetailList.ForEach ((proto) => { proto.Active = !proto.Active;})));

            return menu;
        }

		public static GenericMenu PrototypeTerrainTextureMenu(Type toolType, Group group, SelectedVariables selectedTypeVariables, ClipboardBase clipboard, int prototypeIndex)
        {
            GenericMenu menu = new GenericMenu();

			PrototypeTerrainTexture localProto = group.ProtoTerrainTextureList[prototypeIndex];

			if(localProto.TerrainLayer != null)
            {
                menu.AddItem(new GUIContent("Reveal in Project"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => EditorGUIUtility.PingObject(localProto.TerrainLayer)));
                menu.AddSeparator ("");
            }
			else if(localProto.TerrainTextureSettings.DiffuseTexture)
			{
				menu.AddItem(new GUIContent("Reveal in Project"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => EditorGUIUtility.PingObject(localProto.TerrainTextureSettings.DiffuseTexture)));
                menu.AddSeparator ("");
			}

            menu.AddItem(new GUIContent("Delete"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => SelectionPrototypeUtility.DeleteSelectedProtoTerrainTexture(group)));

            clipboard?.PrototypeUnityTerrainTextureMenu(menu, selectedTypeVariables);

			menu.AddSeparator ("");
            menu.AddItem(new GUIContent("Select All"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => SelectionPrototypeUtility.SetSelectedAllPrototypes(group, true)));
			menu.AddItem(new GUIContent("Active"), localProto.Active, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => 
				selectedTypeVariables.SelectedProtoTerrainTextureList.ForEach ((proto) => { proto.Active = !proto.Active;})));

            return menu;
        }
    }
}
#endif