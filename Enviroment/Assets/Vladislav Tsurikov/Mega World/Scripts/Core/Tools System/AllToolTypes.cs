#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;

namespace VladislavTsurikov.MegaWorldSystem
{
    public static class AllToolTypes 
    {
        public static List<System.Type> TypeList = new List<System.Type>();

        static AllToolTypes()
        {
            var types = ModulesUtility.GetAllTypesDerivedFrom<ToolComponent>()
                                .Where(
                                    t => t.IsDefined(typeof(ToolAttribute), false)
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