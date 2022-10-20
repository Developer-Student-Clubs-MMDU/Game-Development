#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VladislavTsurikov.MegaWorldSystem
{
    public class AllScattersTypes 
    {
        public List<System.Type> TypeList = new List<System.Type>();

        public AllScattersTypes()
        {
            var types = ModulesUtility.GetAllTypesDerivedFrom<Scatter>()
                                .Where(
                                    t => t.IsDefined(typeof(ScatterAttribute), false)
                                ); 

            TypeList = types.ToList();
        }
    }
}
#endif