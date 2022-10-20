#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;

namespace VladislavTsurikov.MegaWorldSystem
{
    public static class AllFilterTypes 
    {
        public static List<System.Type> TypeList = new List<System.Type>();

        static AllFilterTypes()
        {
            var types = ModulesUtility.GetAllTypesDerivedFrom<MaskFilter>()
                                .Where(
                                    t => t.IsDefined(typeof(MaskFilterAttribute), false)
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