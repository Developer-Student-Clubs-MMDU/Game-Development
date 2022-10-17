using System.Collections.Generic;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ScatterAttribute : Attribute
    {
        public readonly string Name;

        internal ScatterAttribute(string name)
        {
            Name = name;
        }
    }
}