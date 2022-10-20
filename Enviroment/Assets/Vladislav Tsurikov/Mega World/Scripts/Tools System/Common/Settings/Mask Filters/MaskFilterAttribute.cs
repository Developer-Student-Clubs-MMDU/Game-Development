using System.Collections.Generic;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MaskFilterAttribute : Attribute
    {
        public readonly string Name;

        internal MaskFilterAttribute(string name)
        {
            Name = name;
        }
    }
}