#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace VladislavTsurikov.MegaWorldSystem
{
	public static class AllTemplateTypes 
    {
        public static List<System.Type> TypeList = new List<System.Type>();

        static AllTemplateTypes()
        {
            var types = ModulesUtility.GetAllTypesDerivedFrom<Template>()
                                .Where(
                                    t => t.IsDefined(typeof(TemplateAttribute), false)
                                      && !t.IsAbstract
                                ); 

            foreach (var type in types)
            {
                TypeList.Add(type);
            }
        }
    }
}
#endif