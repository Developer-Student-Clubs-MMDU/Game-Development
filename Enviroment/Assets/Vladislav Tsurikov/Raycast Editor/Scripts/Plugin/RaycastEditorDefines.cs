#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VladislavTsurikov.InstantRendererSystem
{
    [InitializeOnLoad]
    public class RaycastEditorDefines
    {
        private static readonly string DEFINE_RAYCAST_EDITOR = "RAYCAST_EDITOR";

        static RaycastEditorDefines()
        {
            ScriptingDefineSymbolsUtility.SetScriptingDefineSymbols(DEFINE_RAYCAST_EDITOR);
        }
    }
}
#endif