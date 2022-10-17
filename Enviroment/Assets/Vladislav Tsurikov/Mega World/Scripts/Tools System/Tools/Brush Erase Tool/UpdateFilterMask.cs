#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
#if UNITY_2021_2_OR_NEWER
using UnityEngine.TerrainTools;
#else
using UnityEngine.Experimental.TerrainAPI;
#endif

namespace VladislavTsurikov.MegaWorldSystem.BrushErase
{
    public static class UpdateFilterMask 
    {
        public static void UpdateFilterMaskTexture(Group group, AreaVariables areaVariables)
        {
            if(areaVariables.TerrainUnderCursor == null)
            {
                return;
            }

            MaskFilterSettings maskFilterSettings = (MaskFilterSettings)group.GetSettings(typeof(BrushEraseTool), typeof(MaskFilterSettings));

            FilterMaskOperation.UpdateMaskTexture(maskFilterSettings, areaVariables);
        }
    }
}
#endif