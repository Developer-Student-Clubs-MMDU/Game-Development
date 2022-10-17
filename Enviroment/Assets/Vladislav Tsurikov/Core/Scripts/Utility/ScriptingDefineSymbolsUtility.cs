#if UNITY_EDITOR

#endif
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
 
namespace VladislavTsurikov
{
    public static class ScriptingDefineSymbolsUtility
    {
        public static void SetScriptingDefineSymbols(string define)
        {
            List<string> defineList = new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';'));
            if (!defineList.Contains(define))
            {
                defineList.Add(define);
                string defines = string.Join(";", defineList.ToArray());
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);
            }
        }
    }
}
#endif