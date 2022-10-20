using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ToolAttribute : Attribute
    {
        public readonly bool IsSupportMultipleTypes = true;
        public readonly bool DisableToolIfUnityToolActive;
        public readonly ResourceType[] SupportedResourceTypes;

        internal ToolAttribute(bool isSupportMultipleTypes, bool disableToolIfUnityToolActive, ResourceType[] supportedResourceTypes)
        {
            IsSupportMultipleTypes = isSupportMultipleTypes;
            DisableToolIfUnityToolActive = disableToolIfUnityToolActive;
            SupportedResourceTypes = supportedResourceTypes;
        }
    }
}