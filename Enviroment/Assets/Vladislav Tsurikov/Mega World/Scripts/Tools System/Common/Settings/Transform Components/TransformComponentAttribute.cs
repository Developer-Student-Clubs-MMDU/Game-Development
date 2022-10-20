using System.Collections.Generic;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TransformComponentAttribute : Attribute
    {
        public readonly string Name;
        public readonly bool SimpleComponent;

        internal TransformComponentAttribute(string name)
        {
            Name = name;
            SimpleComponent = false;
        }

        internal TransformComponentAttribute(string name, bool simpleComponent)
        {
            Name = name;
            SimpleComponent = simpleComponent;
        }
    }
}