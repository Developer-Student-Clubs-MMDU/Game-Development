#if UNITY_EDITOR
using UnityEngine;

namespace VladislavTsurikov.MegaWorldSystem.SprayBrush
{
    public static class SprayBrushToolVisualisation 
    {
        public static void Draw(AreaVariables areaVariables)
        {
            if(areaVariables == null)
            {
                return;
            }

            if(areaVariables.RayHit == null)
            {
                return;
            }

            if(MegaWorldPath.AdvancedSettings.VisualisationSettings.VisualizeOverlapCheckSettings)
            { 
                if(MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoGameObjectList.Count != 0)
                {
                    OverlapCheckSettings.VisualizeOverlapForGameObject(areaVariables.Bounds);
                }

                if(MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoInstantItemList.Count != 0)
                {
                    OverlapCheckSettings.VisualizeOverlapForInstantItem(areaVariables.Bounds);
                }
            }

            if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedGroup())
            {
                Group group = MegaWorldPath.DataPackage.SelectedVariables.SelectedGroup;

                SimpleFilterSettings simpleFilterSettings = (SimpleFilterSettings)group.GetSettings(typeof(SprayBrushTool), typeof(SimpleFilterSettings));

                if(simpleFilterSettings.HasOneActiveFilter())
                {
                    VisualisationUtility.DrawSimpleFilter(group, areaVariables.RayHit.Point, areaVariables, simpleFilterSettings, true);
                }
                
                VisualisationUtility.DrawCircleHandles(areaVariables.Size, areaVariables.RayHit);
            }
            else
            {
                VisualisationUtility.DrawCircleHandles(areaVariables.Size, areaVariables.RayHit);
            }
        }
    }
}
#endif
