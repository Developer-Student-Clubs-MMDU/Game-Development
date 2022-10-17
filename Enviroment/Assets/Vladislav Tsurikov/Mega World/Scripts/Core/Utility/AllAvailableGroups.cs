using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VladislavTsurikov.MegaWorldSystem
{
    public static class AllAvailableTypes
	{
        private static List<Group> _typeList = new List<Group>();

        static AllAvailableTypes()
        {
            Group[] groups = Resources.FindObjectsOfTypeAll(typeof(Group)) as Group[];

            foreach (Group group in groups)
            {
                AddType(group);
            }
        } 

        public static List<Group> GetAllTypes()
        {
            return _typeList;
        }

        public static void AddType(Group group)
        {
            if(!_typeList.Contains(group))
            {
                _typeList.Add(group);
            }
        }
	}
}