using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VladislavTsurikov.MegaWorldSystem
{
    public static class AllAvailablePrototypes 
    {   
        public static List<Prototype> ProtoList = new List<Prototype>();

        public static void Add(Prototype proto)
        {
            ProtoList.Add(proto);
        }
    }   
}