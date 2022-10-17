#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace VladislavTsurikov.MegaWorldSystem
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TemplateAttribute : Attribute
    {
        public readonly string Name;
        public readonly Type[] ToolTypes;
        public readonly ResourceType[] SupportedResourceTypes;

        internal TemplateAttribute(string name, Type[] toolTypes, ResourceType[] supportedResourceTypes)
        {
            ToolTypes = toolTypes;
            Name = name;
            SupportedResourceTypes = supportedResourceTypes;
        }
    }
}
#endif