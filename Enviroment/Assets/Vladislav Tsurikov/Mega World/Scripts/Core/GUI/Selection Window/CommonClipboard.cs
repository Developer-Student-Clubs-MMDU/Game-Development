#if UNITY_EDITOR
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace VladislavTsurikov.MegaWorldSystem
{
    public class CommonClipboard : ClipboardBase
    {
        private ScatterSettings _copiedScatterSettings = null;
        private TerrainDetailSettings _copiedTerrainDetailSettings = null;
        private SpawnDetailSettings _copiedSpawnDetailSettings = null;
        private OverlapCheckSettings _copiedOverlapCheckSettings = null;
        private TransformComponentSettings _copiedTransformComponentSettings = null;
        private MaskFilterSettings _copiedMaskFilterSettings = null; 
        private SimpleFilterSettings _copiedSimpleFilterSettings = null;
        private SuccessSettings _copiedSuccessSettings = null;

        public override void GroupGameObjectMenu(GenericMenu menu, SelectedVariables selectedDataVariables)
        {
            menu.AddSeparator("");
			if(selectedDataVariables.HasOneSelectedGroup())
			{
				menu.AddItem(new GUIContent("Copy All Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => CopyAllGroupSettings(selectedDataVariables)));
			}

            menu.AddItem(new GUIContent("Paste All Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => PasteAllGroupSettings(selectedDataVariables)));

            if(selectedDataVariables.SelectedGroup.FilterType == FilterType.MaskFilter)
            {
                menu.AddItem(new GUIContent("Paste Settings/Mask Filter Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => PasteSettings(selectedDataVariables.SelectedGroupList, typeof(MaskFilterSettings))));	
            }
            else
            {
                menu.AddItem(new GUIContent("Paste Settings/Simple Filter Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => PasteSettings(selectedDataVariables.SelectedGroupList, typeof(SimpleFilterSettings))));	
            }

            menu.AddItem(new GUIContent("Paste Settings/Scatter Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => PasteSettings(selectedDataVariables.SelectedGroupList, typeof(ScatterSettings))));	
        }

        public override void GroupInstantItemMenu(GenericMenu menu, SelectedVariables selectedDataVariables)
        {
            menu.AddSeparator("");
			if(selectedDataVariables.HasOneSelectedGroup())
			{
				menu.AddItem(new GUIContent("Copy All Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => CopyAllGroupSettings(selectedDataVariables)));
			}

            menu.AddItem(new GUIContent("Paste All Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => PasteAllGroupSettings(selectedDataVariables)));

            if(selectedDataVariables.SelectedGroup.FilterType == FilterType.MaskFilter)
            {
                menu.AddItem(new GUIContent("Paste Settings/Mask Filter Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => PasteSettings(selectedDataVariables.SelectedGroupList, typeof(MaskFilterSettings))));	
            }
            else
            {
                menu.AddItem(new GUIContent("Paste Settings/Simple Filter Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => PasteSettings(selectedDataVariables.SelectedGroupList, typeof(SimpleFilterSettings))));	
            }

            menu.AddItem(new GUIContent("Paste Settings/Scatter Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => PasteSettings(selectedDataVariables.SelectedGroupList, typeof(ScatterSettings))));	
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

                ClipboardAction(group, typeof(ScatterSettings), false);
            }
    	}

        public override void PasteAllGroupSettings(SelectedVariables selectedVariables)
    	{
            foreach(Group group in selectedVariables.SelectedGroupList) 
            {
                ClipboardAction(group, typeof(MaskFilterSettings), true);
                ClipboardAction(group, typeof(SimpleFilterSettings), true);
                ClipboardAction(group, typeof(ScatterSettings), true);
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
                    MaskFilterSettings maskFilterSettings = (MaskFilterSettings)group.GetSettings(typeof(MaskFilterSettings));
                    maskFilterSettings.Stack.PasteStack(_copiedMaskFilterSettings.Stack, group);
                }
                else
                {
                    _copiedMaskFilterSettings = (MaskFilterSettings)CopySettings(_copiedMaskFilterSettings, group.GetSettings(typeof(MaskFilterSettings)));
                }
            }
            else if(typeof(SimpleFilterSettings) == settingsType)
            {
                if(paste)
                {
                    PasteSettings(_copiedSimpleFilterSettings, group.GetSettings(typeof(SimpleFilterSettings)));
                }
                else
                {
                    _copiedSimpleFilterSettings = (SimpleFilterSettings)CopySettings(_copiedSimpleFilterSettings, group.GetSettings(typeof(SimpleFilterSettings)));
                }
            }
            else if(typeof(ScatterSettings) == settingsType)
            {
                if(paste)
                {
                    ScatterSettings scatterSettings = (ScatterSettings)group.GetSettings(typeof(ScatterSettings));
                    scatterSettings.Stack.PasteStack(_copiedScatterSettings.Stack, group);
                }
                else
                {
                    _copiedScatterSettings = (ScatterSettings)CopySettings(_copiedScatterSettings, group.GetSettings(typeof(ScatterSettings)));
                }
            }
        }

        public override void PrototypeGameObjectMenu(GenericMenu menu, SelectedVariables selectedDataVariables)
        {
            menu.AddSeparator("");
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
            menu.AddSeparator("");
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

        public override void PrototypeUnityTerrainDetailMenu(GenericMenu menu, SelectedVariables selectedDataVariables)
        {
            menu.AddSeparator("");
			if(selectedDataVariables.HasOneSelectedProtoTerrainDetail())
			{
				menu.AddItem(new GUIContent("Copy All Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => CopyAllTerrainDetailSettings(selectedDataVariables)));
			}

            menu.AddItem(new GUIContent("Paste All Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => PasteAllTerrainDetailSettings(selectedDataVariables)));

            List<Prototype> protoList = new List<Prototype>() ;
            protoList.AddRange(selectedDataVariables.SelectedProtoTerrainDetailList);

            menu.AddItem(new GUIContent("Paste Settings/Spawn Detail Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => PasteSettings(protoList, typeof(SpawnDetailSettings))));
			menu.AddItem(new GUIContent("Paste Settings/Mask Filters Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => PasteSettings(protoList, typeof(MaskFilterSettings))));
			menu.AddItem(new GUIContent("Paste Settings/Terrain Detail Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => PasteSettings(protoList, typeof(TerrainDetailSettings))));
        }

        public override void PrototypeUnityTerrainTextureMenu(GenericMenu menu, SelectedVariables selectedDataVariables)
        {
            menu.AddSeparator("");
			if(selectedDataVariables.HasOneSelectedProtoTerrainTexture())
			{
				menu.AddItem(new GUIContent("Copy All Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => CopyAllTerrainTextureSettings(selectedDataVariables)));
			}

            List<Prototype> protoList = new List<Prototype>() ;
            protoList.AddRange(selectedDataVariables.SelectedProtoTerrainTextureList);
                
			menu.AddItem(new GUIContent("Paste Settings/Mask Filters Settings"), false, MegaWorldGUIUtility.ContextMenuCallback, new Action(() => PasteSettings(protoList, typeof(MaskFilterSettings))));
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

        public override void CopyAllTerrainDetailSettings(SelectedVariables selectedVariables)
    	{
            if(selectedVariables.HasOneSelectedProtoTerrainDetail())
            {	
                PrototypeTerrainDetail selectedProto = selectedVariables.SelectedProtoTerrainDetail;

                ClipboardAction(selectedProto, typeof(MaskFilterSettings), false);
                ClipboardAction(selectedProto, typeof(SpawnDetailSettings), false);
                ClipboardAction(selectedProto, typeof(TerrainDetailSettings), false);
            }
    	}

        public override void CopyAllTerrainTextureSettings(SelectedVariables selectedVariables)
    	{
            if(selectedVariables.HasOneSelectedProtoTerrainTexture())
            {	
                PrototypeTerrainTexture selectedProto = selectedVariables.SelectedProtoTerrainTexture;

                ClipboardAction(selectedProto, typeof(MaskFilterSettings), false);
            }
    	}

        public override void PasteAllGameObjectSettings(SelectedVariables selectedVariables)
    	{
            foreach(PrototypeGameObject proto in selectedVariables.SelectedProtoGameObjectList) 
            {
                ClipboardAction(proto, typeof(MaskFilterSettings), true);
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
                ClipboardAction(proto, typeof(MaskFilterSettings), true);
                ClipboardAction(proto, typeof(SimpleFilterSettings), true);
                ClipboardAction(proto, typeof(SuccessSettings), true);
                ClipboardAction(proto, typeof(OverlapCheckSettings), true);
                ClipboardAction(proto, typeof(TransformComponentSettings), true);
            }
    	}

        public override void PasteAllTerrainDetailSettings(SelectedVariables selectedVariables)
    	{
            foreach(PrototypeTerrainDetail proto in selectedVariables.SelectedProtoTerrainDetailList) 
            {
                ClipboardAction(proto, typeof(MaskFilterSettings), true);
                ClipboardAction(proto, typeof(SpawnDetailSettings), true);
                ClipboardAction(proto, typeof(TerrainDetailSettings), true);
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
            if(typeof(TerrainDetailSettings) == settingsType)
            {
                if(paste)
                {
                    PasteSettings(_copiedTerrainDetailSettings, proto.GetSettings(typeof(TerrainDetailSettings)));
                }
                else
                {
                    _copiedTerrainDetailSettings = (TerrainDetailSettings)CopySettings(_copiedTerrainDetailSettings, proto.GetSettings(typeof(TerrainDetailSettings)));
                }
                
            }
            else if(typeof(SpawnDetailSettings) == settingsType)
            {
                if(paste)
                {
                    PasteSettings(_copiedSpawnDetailSettings, proto.GetSettings(typeof(SpawnDetailSettings)));
                }
                else
                {
                    _copiedSpawnDetailSettings = (SpawnDetailSettings)CopySettings(_copiedSpawnDetailSettings, proto.GetSettings(typeof(SpawnDetailSettings)));
                }
            }
            else if(typeof(OverlapCheckSettings) == settingsType)
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
                    TransformComponentSettings transformComponentSettings = (TransformComponentSettings)proto.GetSettings(typeof(TransformComponentSettings));
                    transformComponentSettings?.Stack.PasteStack(_copiedTransformComponentSettings.Stack, proto);
                }
                else
                {
                    _copiedTransformComponentSettings = (TransformComponentSettings)CopySettings(_copiedTransformComponentSettings, proto.GetSettings(typeof(TransformComponentSettings)));
                }
            }
            else if(typeof(MaskFilterSettings) == settingsType)
            {
                if(paste)
                {
                    MaskFilterSettings maskFilterSettings = (MaskFilterSettings)proto.GetSettings(typeof(MaskFilterSettings));
                    maskFilterSettings?.Stack.PasteStack(_copiedMaskFilterSettings.Stack, proto);
                }
                else
                {
                    _copiedMaskFilterSettings = (MaskFilterSettings)CopySettings(_copiedMaskFilterSettings, proto.GetSettings(typeof(MaskFilterSettings)));
                }
            }
            else if(typeof(SimpleFilterSettings) == settingsType)
            {
                if(paste)
                {
                    PasteSettings(_copiedSimpleFilterSettings, proto.GetSettings(typeof(SimpleFilterSettings)));
                }
                else
                {
                    _copiedSimpleFilterSettings = (SimpleFilterSettings)CopySettings(_copiedSimpleFilterSettings, proto.GetSettings(typeof(SimpleFilterSettings)));
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