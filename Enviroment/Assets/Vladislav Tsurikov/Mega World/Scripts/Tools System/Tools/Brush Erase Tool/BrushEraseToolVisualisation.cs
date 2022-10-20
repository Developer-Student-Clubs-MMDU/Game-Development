#if UNITY_EDITOR
using UnityEngine;

namespace VladislavTsurikov.MegaWorldSystem.BrushErase
{
    public static class BrushEraseToolVisualisation 
    {
        public static void Draw(AreaVariables areaVariables)
        {
            if(areaVariables == null || areaVariables.RayHit == null)
            {
                return;
            }

            if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedGroup())
            {
                Group group = MegaWorldPath.DataPackage.SelectedVariables.SelectedGroup;

                switch (group.ResourceType)
                {
                    case ResourceType.GameObject:
                    case ResourceType.InstantItem:
                    {
                        if(group.FilterType != FilterType.MaskFilter)
                        {
                            SimpleFilterSettings simpleFilterSettings = (SimpleFilterSettings)group.GetSettings(typeof(BrushEraseTool), typeof(SimpleFilterSettings));

                            VisualisationUtility.DrawSimpleFilter(group, areaVariables.RayHit.Point, areaVariables, simpleFilterSettings);
                            VisualisationUtility.DrawCircleHandles(areaVariables.Size, areaVariables.RayHit);
                        }
                        else
                        {
                            MaskFilterSettings maskFilterSettings = (MaskFilterSettings)group.GetSettings(typeof(BrushEraseTool), typeof(MaskFilterSettings));
                            VisualisationUtility.DrawMaskFilterVisualization(maskFilterSettings.Stack, areaVariables);
                        }

                        break;
                    }
                    case ResourceType.TerrainDetail:
                    {
                        MaskFilterSettings maskFilterSettings = (MaskFilterSettings)group.GetSettings(typeof(BrushEraseTool), typeof(MaskFilterSettings));
                        VisualisationUtility.DrawMaskFilterVisualization(maskFilterSettings.Stack, areaVariables);

                        break;
                    }
                }
            }
            else
            {
                if(VisualisationUtility.IsActiveSimpleFilter(MegaWorldPath.DataPackage.SelectedVariables))
                {
                    VisualisationUtility.DrawCircleHandles(areaVariables.Size, areaVariables.RayHit);
                }
                else
                {
                    VisualisationUtility.DrawAreaPreview(areaVariables);
                }
            }
        }
    }
}
#endif