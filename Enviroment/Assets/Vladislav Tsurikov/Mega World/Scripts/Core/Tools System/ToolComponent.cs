using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class ToolComponent : ScriptableObject
    {
        public bool Enabled = false;

        public SettingsTypesStack SettingsTypesStack;

        public ToolComponent()
        {
            CreateSettingsTypesStack();
            AddSettingsTypes();
            OnEnable();
        }

        public void CreateSettingsTypesStack()
        {
            SettingsTypesStack = new SettingsTypesStack(this.GetType());
            AllSettingsTypes.SettingsTypesList.Add(SettingsTypesStack);
        }

        public bool IsToolSupportSelectedResourcesType()
        {
            if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedGroup())
			{
                if(this.GetType().GetAttribute<ToolAttribute>().SupportedResourceTypes.Contains(MegaWorldPath.DataPackage.SelectedVariables.SelectedGroup.ResourceType))
                {
                    return true;
                }
			}

			return false;
        }

        public bool IsToolSupportMultipleTypes()
        {
            if(MegaWorldPath.DataPackage.SelectedVariables.SelectedGroupList.Count > 1)
			{
                if(this.GetType().GetAttribute<ToolAttribute>().IsSupportMultipleTypes)
                {
                    return true;
                }
			}

            return false;
        }

        public bool IsToolSupportSelectedData()
        {
            if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedGroup())
			{
#if !INSTANT_RENDERER
                if(MegaWorldPath.DataPackage.SelectedVariables.SelectedGroup.ResourceType == ResourceType.InstantItem)
                {
                    return false;
                }
#endif
                if(this.GetType().GetAttribute<ToolAttribute>().SupportedResourceTypes.Contains(MegaWorldPath.DataPackage.SelectedVariables.SelectedGroup.ResourceType))
                {
                    return true;
                }
			}
            else
            {
                if(this.GetType().GetAttribute<ToolAttribute>().IsSupportMultipleTypes)
                {
                    return true;
                }
            }

            return false;
        }

        public void InternalHandleKeyboardEvents()
        {
            switch (Event.current.type)
            {
                case EventType.Layout:
                case EventType.Repaint:
                    return;
            }

            HandleKeyboardEvents();
        }

        public void AddSettingsTypes()
        {
            AddPrototypeSettingsTypes();
            AddPrototypeToolSettingsTypes();
            AddGroupSettingsTypes();
            AddGroupToolSettingsTypes();
        }

        public void InternalDoTool()
        {
#if UNITY_EDITOR
            if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedGroup())
			{
				if(MegaWorldPath.CommonDataPackage.ResourcesControllerEditor.IsSyncError(MegaWorldPath.DataPackage.SelectedVariables.SelectedGroup))
				{
					return;
				}
                else if(!MegaWorldPath.DataPackage.SelectedVariables.SelectedGroup.СontainsPrototype())
				{
					return;
				}
			}
#endif

            DoTool();
        }

        public virtual void AddPrototypeSettingsTypes(){}
        public virtual void AddPrototypeToolSettingsTypes(){}
        public virtual void AddGroupSettingsTypes(){}
        public virtual void AddGroupToolSettingsTypes(){}

        public virtual void OnEnable(){}
        public virtual void DoTool() {}
        public virtual void OnToolDisabled(){}
        public virtual void HandleKeyboardEvents(){}

#if UNITY_EDITOR
        public virtual void SaveSettings() {}
#endif
    }
}