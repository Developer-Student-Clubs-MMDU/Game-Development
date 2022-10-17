using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
 
namespace VladislavTsurikov
{
    public static class FindRenderPipeline
    { 
        private const string HDRP_PACKAGE = "HDRenderPipelineAsset";
        private const string URP_PACKAGE = "UniversalRenderPipelineAsset";

        public static bool IsHDRP;
        public static bool IsURP;
        public static bool IsStandardRP;
        
#if UNITY_EDITOR
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            IsHDRP = ModulesUtility.DoesTypeExist(HDRP_PACKAGE);
            IsURP = ModulesUtility.DoesTypeExist(URP_PACKAGE);

            if(!(IsHDRP || IsURP))
            {
                IsStandardRP = true;
            }
        }
#endif
    }
}