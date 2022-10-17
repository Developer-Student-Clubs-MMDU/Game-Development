#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;

namespace VladislavTsurikov.MegaWorldSystem
{
    public static class AllToolEditorTypes 
    {
        public static Dictionary<System.Type, System.Type> EditorTypes = new Dictionary<System.Type, System.Type>(); // SettingsType => EditorType

        static AllToolEditorTypes()
        {
            var types = ModulesUtility.GetAllTypesDerivedFrom<ToolBaseEditor>()
                                .Where(
                                    t => t.IsDefined(typeof(ToolEditorAttribute), false)
                                      && !t.IsAbstract
                                );

            foreach (var type in types)
            {
                var attribute = type.GetAttribute<ToolEditorAttribute>();
                EditorTypes.Add(attribute.SettingsType, type);
            }
        }
    }
}
#endif