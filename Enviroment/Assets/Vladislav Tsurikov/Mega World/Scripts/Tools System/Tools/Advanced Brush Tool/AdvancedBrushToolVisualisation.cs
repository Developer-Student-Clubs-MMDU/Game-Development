#if UNITY_EDITOR
using UnityEngine;

namespace VladislavTsurikov.MegaWorldSystem.AdvancedBrush
{
    public static class AdvancedBrushToolVisualisation 
    {
        public static void Draw(AreaVariables areaVariables)
        {
            if(areaVariables == null || areaVariables.RayHit == null)
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

                switch (group.ResourceType)
                {
                    case ResourceType.GameObject:
                    case ResourceType.InstantItem:
                    {
                        if(group.FilterType != FilterType.MaskFilter)
                        {
                            SimpleFilterSettings simpleFilterSettings = (SimpleFilterSettings)group.GetSettings(typeof(SimpleFilterSettings));

                            if(simpleFilterSettings.HasOneActiveFilter())
                            {
                                VisualisationUtility.DrawSimpleFilter(group, areaVariables.RayHit.Point, areaVariables, simpleFilterSettings);
                            }

                            VisualisationUtility.DrawCircleHandles(areaVariables.Size, areaVariables.RayHit);
                        }
                        else
                        {
                            MaskFilterSettings maskFilterSettings = (MaskFilterSettings)group.GetSettings(typeof(MaskFilterSettings));
                            VisualisationUtility.DrawMaskFilterVisualization(maskFilterSettings.Stack, areaVariables);
                        }

                        break;
                    }
                    default:
                    {
                        if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedResourceProto())
                        {
                            VisualisationUtility.DrawMaskFilterVisualization(GetCurrentMaskFilter(), areaVariables);
                        }
                        else
                        {
                            VisualisationUtility.DrawAreaPreview(areaVariables);
                        }

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

        public static MaskFilterStack GetCurrentMaskFilter()
        {
            if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedProtoTerrainDetail())
            {
                MaskFilterSettings maskFilterSettings = (MaskFilterSettings)MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoTerrainDetail.GetSettings(typeof(MaskFilterSettings));
                return maskFilterSettings.Stack;
            }
            if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedProtoTerrainTexture())
            {
                MaskFilterSettings maskFilterSettings = (MaskFilterSettings)MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoTerrainTexture.GetSettings(typeof(MaskFilterSettings));
                return maskFilterSettings.Stack;
            }

            return null;
        }
    }
}
#endif