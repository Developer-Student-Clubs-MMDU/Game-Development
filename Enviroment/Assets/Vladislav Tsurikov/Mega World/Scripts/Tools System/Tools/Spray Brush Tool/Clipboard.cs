#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace VladislavTsurikov.MegaWorldSystem.SprayBrush
{
    public class Clipboard : ClipboardBase
    {
        private OverlapCheckSettings _copiedOverlapCheckSettings = null;
        private TransformComponentSettings _copiedTransformComponentSettings = null;
        private SimpleFilterSettings _copiedSimpleFilterSettings = null;
        private SuccessSettings _copiedSuccessSettings = null;

        public override void GroupGameObjectMenu(GenericMenu menu, SelectedVariables selectedDataVariables)
        {
            menu.AddSeparator ("");
			if(selectedDataVariables.HasOneSelectedGroup())
			{
				menu.AddItem(new GUIContent("Copy All Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => CopyAllGroupSettings(selectedDataVariables)));
			}

            menu.AddItem(new GUIContent("Paste Settings/Simple Filter Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => PasteSettings(selectedDataVariables.SelectedGroupList, typeof(SimpleFilterSettings))));	
        }

        public override void GroupInstantItemMenu(GenericMenu menu, SelectedVariables selectedDataVariables)
        {
            menu.AddSeparator ("");
			if(selectedDataVariables.HasOneSelectedGroup())
			{
				menu.AddItem(new GUIContent("Copy All Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => CopyAllGroupSettings(selectedDataVariables)));
			}

            menu.AddItem(new GUIContent("Paste Settings/Simple Filter Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => PasteSettings(selectedDataVariables.SelectedGroupList, typeof(SimpleFilterSettings))));	
        }

        public override void CopyAllGroupSettings(SelectedVariables selectedVariables)
    	{
            if(selectedVariables.HasOneSelectedGroup())
            {	
                Group group  = selectedVariables.SelectedGroup;

                ClipboardAction(group, typeof(SimpleFilterSettings), false);
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
            if(typeof(SimpleFilterSettings) == settingsType)
            {
                if(paste)
                {
                    PasteSettings(_copiedSimpleFilterSettings, group.GetSettings(typeof(SprayBrushTool), typeof(SimpleFilterSettings)));
                }
                else
                {
                    _copiedSimpleFilterSettings = (SimpleFilterSettings)CopySettings(_copiedSimpleFilterSettings, group.GetSettings(typeof(SprayBrushTool), typeof(SimpleFilterSettings)));
                }
            }
        }

        public override void PrototypeGameObjectMenu(GenericMenu menu, SelectedVariables selectedDataVariables)
        {
            menu.AddSeparator ("");
			if(selectedDataVariables.HasOneSelectedProtoGameObject())
			{
				menu.AddItem(new GUIContent("Copy All Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => CopyAllGameObjectSettings(selectedDataVariables)));
			}

            menu.AddItem(new GUIContent("Paste All Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => PasteAllGameObjectSettings(selectedDataVariables)));

            List<Prototype> protoList = new List<Prototype>() ;
            protoList.AddRange(selectedDataVariables.SelectedProtoGameObjectList);

            menu.AddItem(new GUIContent("Paste Settings/Success"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => PasteSettings(protoList, typeof(SuccessSettings))));	
            menu.AddItem(new GUIContent("Paste Settings/Overlap Check Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => PasteSettings(protoList, typeof(OverlapCheckSettings))));
			menu.AddItem(new GUIContent("Paste Settings/Transform Components Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => PasteSettings(protoList, typeof(TransformComponentSettings))));
        }

        public override void PrototypeInstantItemMenu(GenericMenu menu, SelectedVariables selectedDataVariables)
        {
            menu.AddSeparator ("");
			if(selectedDataVariables.HasOneSelectedProtoInstantItem())
			{
				menu.AddItem(new GUIContent("Copy All Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => CopyAllInstantItemSettings(selectedDataVariables)));
			}

            menu.AddItem(new GUIContent("Paste All Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => PasteAllInstantItemSettings(selectedDataVariables)));

            List<Prototype> protoList = new List<Prototype>() ;
            protoList.AddRange(selectedDataVariables.SelectedProtoInstantItemList);

            menu.AddItem(new GUIContent("Paste Settings/Success"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => PasteSettings(protoList, typeof(SuccessSettings))));
            menu.AddItem(new GUIContent("Paste Settings/Overlap Check Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => PasteSettings(protoList, typeof(OverlapCheckSettings))));
			menu.AddItem(new GUIContent("Paste Settings/Transform Components Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => PasteSettings(protoList, typeof(TransformComponentSettings))));
        }

        public override void CopyAllInstantItemSettings(SelectedVariables selectedVariables)
    	{
            if(selectedVariables.HasOneSelectedProtoInstantItem())
            {	
                PrototypeInstantItem selectedProto = selectedVariables.SelectedProtoInstantItem;

                ClipboardAction(selectedProto, typeof(SuccessSettings), false);
                ClipboardAction(selectedProto, typeof(OverlapCheckSettings), false);
                ClipboardAction(selectedProto, typeof(TransformComponentSettings), false);
            }
    	}

        public override void CopyAllGameObjectSettings(SelectedVariables selectedVariables)
    	{
            if(selectedVariables.HasOneSelectedProtoGameObject())
            {	
                PrototypeGameObject selectedProto = selectedVariables.SelectedProtoGameObject;

                ClipboardAction(selectedProto, typeof(SuccessSettings), false);
                ClipboardAction(selectedProto, typeof(OverlapCheckSettings), false);
                ClipboardAction(selectedProto, typeof(TransformComponentSettings), false);
            }
    	}

        public override void PasteAllGameObjectSettings(SelectedVariables selectedVariables)
    	{
            foreach(PrototypeGameObject proto in selectedVariables.SelectedProtoGameObjectList) 
            {
                ClipboardAction(proto, typeof(SimpleFilterSettings), true);
                ClipboardAction(proto, typeof(SuccessSettings), true);
                ClipboardAction(proto, typeof(OverlapCheckSettings), true);
                ClipboardAction(proto, typeof(TransformComponentSettings), true);
            }
    	}

        public override void PasteAllInstantItemSettings(SelectedVariables selectedVariables)
    	{
            foreach(PrototypeInstantItem proto in selectedVariables.SelectedProtoInstantItemList) 
            {
                ClipboardAction(proto, typeof(SimpleFilterSettings), true);
                ClipboardAction(proto, typeof(SuccessSettings), true);
                ClipboardAction(proto, typeof(OverlapCheckSettings), true);
                ClipboardAction(proto, typeof(TransformComponentSettings), true);
            }
    	}
        
        public void PasteSettings(List<Prototype> protoList, System.Type settingsType)
        {
            foreach (Prototype proto in protoList)
            {
                ClipboardAction(proto, settingsType, true);
            }
        }

        public void ClipboardAction(Prototype proto, System.Type settingsType, bool paste)
        {
            if(typeof(OverlapCheckSettings) == settingsType)
            {
                if(paste)
                {
                    PasteSettings(_copiedOverlapCheckSettings, proto.GetSettings(typeof(OverlapCheckSettings)));
                }
                else
                {
                    _copiedOverlapCheckSettings = (OverlapCheckSettings)CopySettings(_copiedOverlapCheckSettings, proto.GetSettings(typeof(OverlapCheckSettings)));
                }
            }
            else if(typeof(TransformComponentSettings) == settingsType)
            {
                if(paste)
                {
                    TransformComponentSettings transformComponentSettings = (TransformComponentSettings)proto.GetSettings(typeof(SprayBrushTool), typeof(TransformComponentSettings));
                    transformComponentSettings?.Stack.PasteStack(_copiedTransformComponentSettings.Stack, proto);
                }
                else
                {
                    _copiedTransformComponentSettings = (TransformComponentSettings)CopySettings(_copiedTransformComponentSettings, proto.GetSettings(typeof(SprayBrushTool), typeof(TransformComponentSettings)));
                }
            }
            else if(typeof(SimpleFilterSettings) == settingsType)
            {
                if(paste)
                {
                    PasteSettings(_copiedSimpleFilterSettings, proto.GetSettings(typeof(SprayBrushTool), typeof(SimpleFilterSettings)));
                }
                else
                {
                    _copiedSimpleFilterSettings = (SimpleFilterSettings)CopySettings(_copiedSimpleFilterSettings, proto.GetSettings(typeof(SprayBrushTool), typeof(SimpleFilterSettings)));
                }
            }
            else if(typeof(SuccessSettings) == settingsType)
            {
                if(paste)
                {
                    PasteSettings(_copiedSuccessSettings, proto.GetSettings(typeof(SuccessSettings)));
                }
                else
                {
                    _copiedSuccessSettings = (SuccessSettings)CopySettings(_copiedSuccessSettings, proto.GetSettings(typeof(SuccessSettings)));
                }
            }
        }
    }
}
#endif