#if UNITY_EDITOR
using UnityEngine;

namespace VladislavTsurikov.MegaWorldSystem.Stamper
{
    public class StamperVisualisation 
    {
        public bool UpdateMask = false;
        public MaskFilterContext FilterContext;
        public MaskFilterStack PastMaskFilterStack;

        public void Draw(AreaVariables areaVariables, BasicData data, float multiplyAlpha)
        {
            if(areaVariables == null)
            {
                return;
            }

            if(areaVariables.RayHit == null)
            {
                return;
            }

            if(data.SelectedVariables.HasOneSelectedGroup())
            {
                Group group = data.SelectedVariables.SelectedGroup;

                switch (group.ResourceType)
                {
                    case ResourceType.GameObject:
                    case ResourceType.InstantItem:
                    {
                        if(group.FilterType != FilterType.MaskFilter)
                        {
                            SimpleFilterSettings simpleFilterSettings = (SimpleFilterSettings)group.GetSettings(typeof(SimpleFilterSettings));

                            VisualisationUtility.DrawSimpleFilter(group, areaVariables.RayHit.Point, areaVariables, simpleFilterSettings);
                        }
                        else
                        {
                            MaskFilterSettings maskFilterSettings = (MaskFilterSettings)group.GetSettings(typeof(MaskFilterSettings));
                            DrawVisualization(maskFilterSettings.Stack, areaVariables, multiplyAlpha);
                        }

                        break;
                    }
                    default:
                    {
                        if(data.SelectedVariables.HasOneSelectedResourceProto())
                        {
                            DrawVisualization(GetCurrentMaskFilter(data), areaVariables, multiplyAlpha);
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
                if(!VisualisationUtility.IsActiveSimpleFilter(data.SelectedVariables))
                {
                    VisualisationUtility.DrawAreaPreview(areaVariables);
                }
            }
        }

        public void DrawVisualization(MaskFilterStack maskFilterStack, AreaVariables areaVariables, float multiplyAlpha = 1)
        {
            if(areaVariables.TerrainUnderCursor == null)
            {
                return;
            }

            if (maskFilterStack.Settings.Count > 0)
            {
                UpdateMaskIfNecessary(maskFilterStack, areaVariables);

                VisualisationUtility.DrawMaskFilter(FilterContext, areaVariables, multiplyAlpha);
            }
            else
            {
                VisualisationUtility.DrawAreaPreview(areaVariables);
            }
        }

        public void UpdateMaskIfNecessary(MaskFilterStack maskFilterStack, AreaVariables areaVariables)
        {
            if(FilterContext == null)
            {
                FilterContext = new MaskFilterContext(areaVariables);
                FilterMaskOperation.UpdateFilterContext(ref FilterContext, maskFilterStack, areaVariables);

                UpdateMask = false;

                return;
            }

            if(PastMaskFilterStack != maskFilterStack || UpdateMask)
            {
                FilterContext.DisposeUnmanagedMemory();
                FilterMaskOperation.UpdateFilterContext(ref FilterContext, maskFilterStack, areaVariables);

                PastMaskFilterStack = maskFilterStack;

                UpdateMask = false;
            }
        }

        public MaskFilterStack GetCurrentMaskFilter(BasicData data)
        {
            if(data.SelectedVariables.HasOneSelectedProtoTerrainDetail())
            {
                MaskFilterSettings maskFilterSettings = (MaskFilterSettings)data.SelectedVariables.SelectedProtoTerrainDetail.SettingsStack.GetSettings(typeof(MaskFilterSettings));
                return maskFilterSettings.Stack;
            }
            if(data.SelectedVariables.HasOneSelectedProtoTerrainTexture())
            {
                MaskFilterSettings maskFilterSettings = (MaskFilterSettings)data.SelectedVariables.SelectedProtoTerrainTexture.SettingsStack.GetSettings(typeof(MaskFilterSettings));
                return maskFilterSettings.Stack;
            }

            return null;
        }
    }
}
#endif