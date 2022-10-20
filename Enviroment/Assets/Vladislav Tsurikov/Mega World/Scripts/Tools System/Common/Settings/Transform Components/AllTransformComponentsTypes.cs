#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;

namespace VladislavTsurikov.MegaWorldSystem
{
    public class AllTransformComponentsTypes 
    {
        public List<System.Type> TypeList = new List<System.Type>();

        public AllTransformComponentsTypes(List<System.Type> removeTransformTypes, bool useSimpleComponent = false)
        {
            var types = ModulesUtility.GetAllTypesDerivedFrom<TransformComponent>()
                                .Where(
                                    t => t.IsDefined(typeof(TransformComponentAttribute), false)
                                      && !t.IsAbstract
                                ); 

            foreach (var type in types)
            {
                if(useSimpleComponent)
                {
                    if(!type.GetAttribute<TransformComponentAttribute>().SimpleComponent)
                    {
                        continue;
                    }
                }

                TypeList.Add(type);
            }

            TypeList.RemoveAll(t => removeTransformTypes.Contains(t));
        }

        public AllTransformComponentsTypes(bool useSimpleComponent = false)
        {
            var types = ModulesUtility.GetAllTypesDerivedFrom<TransformComponent>()
                                .Where(
                                    t => t.IsDefined(typeof(TransformComponentAttribute), false)
                                      && !t.IsAbstract
                                ); 

            foreach (var type in types)
            {
                if(useSimpleComponent)
                {
                    if(!type.GetAttribute<TransformComponentAttribute>().SimpleComponent)
                    {
                        continue;
                    }
                }

                TypeList.Add(type);
            }
        }
    }
}
#endif