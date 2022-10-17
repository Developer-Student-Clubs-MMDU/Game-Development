#if UNITY_EDITOR
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace VladislavTsurikov.MegaWorldSystem.BrushErase
{
    [Serializable]
    public class Clipboard : ClipboardBase
    {
        private MaskFilterSettings _copiedMaskFilterSettings = null;
        private SimpleFilterSettings _copiedSimpleFilterSettings = null;

        public override void GroupInstantItemMenu(GenericMenu menu, SelectedVariables selectedTypeVariables)
        {
            menu.AddSeparator ("");
			if(selectedTypeVariables.HasOneSelectedGroup())
			{
				menu.AddItem(new GUIContent("Copy All Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => CopyAllGroupSettings(selectedTypeVariables)));
			}

            if(selectedTypeVariables.SelectedGroup.FilterType == FilterType.MaskFilter)
            {
                menu.AddItem(new GUIContent("Paste Settings/Mask Filter Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => PasteSettings(selectedTypeVariables.SelectedGroupList, typeof(MaskFilterSettings))));	
            }
            else
            {
                menu.AddItem(new GUIContent("Paste Settings/Simple Filter Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => PasteSettings(selectedTypeVariables.SelectedGroupList, typeof(SimpleFilterSettings))));	
            }
        }

        public override void GroupGameObjectMenu(GenericMenu menu, SelectedVariables selectedTypeVariables)
        {
            menu.AddSeparator ("");
			if(selectedTypeVariables.HasOneSelectedGroup())
			{
				menu.AddItem(new GUIContent("Copy All Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => CopyAllGroupSettings(selectedTypeVariables)));
			}

            if(selectedTypeVariables.SelectedGroup.FilterType == FilterType.MaskFilter)
            {
                menu.AddItem(new GUIContent("Paste Settings/Mask Filter Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => PasteSettings(selectedTypeVariables.SelectedGroupList, typeof(MaskFilterSettings))));	
            }
            else
            {
                menu.AddItem(new GUIContent("Paste Settings/Simple Filter Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => PasteSettings(selectedTypeVariables.SelectedGroupList, typeof(SimpleFilterSettings))));	
            }
        }

        public override void GroupUnityTerrainDetailMenu(GenericMenu menu, SelectedVariables selectedTypeVariables)
        {
            menu.AddSeparator ("");
			if(selectedTypeVariables.HasOneSelectedGroup())
			{
				menu.AddItem(new GUIContent("Copy All Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => CopyAllGroupSettings(selectedTypeVariables)));
			}

            menu.AddItem(new GUIContent("Paste Settings/Mask Filter Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => PasteSettings(selectedTypeVariables.SelectedGroupList, typeof(MaskFilterSettings))));	
        }

        public override void CopyAllGroupSettings(SelectedVariables selectedVariables)
    	{
            if(selectedVariables.HasOneSelectedGroup())
            {	
                Group group  = selectedVariables.SelectedGroup;

                if(selectedVariables.SelectedGroup.FilterType == FilterType.MaskFilter)
                {
                    ClipboardAction(group, typeof(MaskFilterSettings), false);
                }
                else
                {
                    ClipboardAction(group, typeof(SimpleFilterSettings), false);
                }
            }
    	}

        public void PasteSettings(List<Group> groupList, System.Type settingsType)
        {
            foreach (Group group in groupList)
            {
                ClipboardAction(group, settingsType, true);
            }
        }

        public void ClipboardAction(Group group, System.Type settingsType, bool paste)
        {
            if(typeof(MaskFilterSettings) == settingsType)
            {
                if(paste)
                {
                    MaskFilterSettings maskFilterSettings = (MaskFilterSettings)group.GetSettings(typeof(BrushEraseTool), typeof(MaskFilterSettings));
                    maskFilterSettings.Stack.PasteStack(_copiedMaskFilterSettings.Stack, group);
                }
                else
                {
                    _copiedMaskFilterSettings = (MaskFilterSettings)CopySettings(_copiedMaskFilterSettings, group.GetSettings(typeof(BrushEraseTool), typeof(MaskFilterSettings)));
                }
            }
            else if(typeof(SimpleFilterSettings) == settingsType)
            {
                if(paste)
                {
                    PasteSettings(_copiedSimpleFilterSettings, group.GetSettings(typeof(BrushEraseTool), typeof(SimpleFilterSettings)));
                }
                else
                {
                    _copiedSimpleFilterSettings = (SimpleFilterSettings)CopySettings(_copiedSimpleFilterSettings, group.GetSettings(typeof(BrushEraseTool), typeof(SimpleFilterSettings)));
                }
            }
        }
    }
}
#endif