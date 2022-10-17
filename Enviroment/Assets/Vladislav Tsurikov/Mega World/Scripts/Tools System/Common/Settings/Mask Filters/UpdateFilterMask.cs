using UnityEngine;
using System.Collections.Generic;
#if UNITY_2021_2_OR_NEWER
using UnityEngine.TerrainTools;
#else
using UnityEngine.Experimental.TerrainAPI;
#endif

namespace VladislavTsurikov.MegaWorldSystem
{
    public static class UpdateFilterMask 
    {
        public static void UpdateFilterMaskTexture(Group group, AreaVariables areaVariables)
        {
            if(areaVariables.TerrainUnderCursor == null)
            {
                return;
            }

            MaskFilterSettings maskFilterSettings = (MaskFilterSettings)group.GetSettings(typeof(MaskFilterSettings));
            FilterMaskOperation.UpdateMaskTexture(maskFilterSettings, areaVariables);
        }

        public static void UpdateFilterMaskTextureForAllTerrainDetail(List<PrototypeTerrainDetail> protoTerrainDetailList, AreaVariables areaVariables)
        {
            if(areaVariables.TerrainUnderCursor == null)
            {
                return;
            }
            
            foreach (PrototypeTerrainDetail proto in protoTerrainDetailList)
            {
                MaskFilterSettings maskFilterSettings = (MaskFilterSettings)proto.GetSettings(typeof(MaskFilterSettings));
                FilterMaskOperation.UpdateMaskTexture(maskFilterSettings, areaVariables);
            }
        }
    }
}