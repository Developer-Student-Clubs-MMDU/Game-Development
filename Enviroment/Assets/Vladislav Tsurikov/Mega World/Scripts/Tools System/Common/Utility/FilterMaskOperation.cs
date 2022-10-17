using UnityEngine;
using System.Collections.Generic;
#if UNITY_2021_2_OR_NEWER
using UnityEngine.TerrainTools;
#else
using UnityEngine.Experimental.TerrainAPI;
#endif

namespace VladislavTsurikov.MegaWorldSystem
{
    public static class FilterMaskOperation 
    {
        public static void UpdateMaskTexture(MaskFilterSettings maskFilterSettings, AreaVariables areaVariables)
        {
            if(maskFilterSettings.Stack.Settings.Count != 0)
            {
                UpdateFilterContext(ref maskFilterSettings.FilterContext, maskFilterSettings.Stack, areaVariables);
                RenderTexture filterMaskRT = maskFilterSettings.FilterContext.GetFilterMaskRT();
                maskFilterSettings.FilterMaskTexture2D = TextureUtility.ToTexture2D(filterMaskRT);
                DisposeMaskTexture(maskFilterSettings);
            }
        }

        public static bool UpdateFilterContext(ref MaskFilterContext filterContext, MaskFilterStack maskFilterStack, AreaVariables areaVariables)
        {
            if(filterContext != null)
            {
                filterContext.DisposeUnmanagedMemory();
            }

            TerrainPainterRenderHelper terrainPainterRenderHelper = new TerrainPainterRenderHelper(areaVariables);

            PaintContext heightContext = terrainPainterRenderHelper.AcquireHeightmap();
            PaintContext normalContext = terrainPainterRenderHelper.AcquireNormalmap();

            RenderTexture output = new RenderTexture(heightContext.sourceRenderTexture.width, heightContext.sourceRenderTexture.height, heightContext.sourceRenderTexture.depth, RenderTextureFormat.ARGB32);
            //RenderTexture output = new RenderTexture(heightContext.sourceRenderTexture.width, heightContext.sourceRenderTexture.height, heightContext.sourceRenderTexture.depth, GraphicsFormat.R16_SFloat);
            output.enableRandomWrite = true;
            output.Create();

    		filterContext = new MaskFilterContext(maskFilterStack, heightContext, normalContext, output, terrainPainterRenderHelper.AreaVariables);

            return true;
        }

        public static void DisposeMaskTexture(MaskFilterSettings maskFilterSettings)
        {
            if(maskFilterSettings.FilterContext != null)
            {
                maskFilterSettings.FilterContext.DisposeUnmanagedMemory();
                maskFilterSettings.FilterContext = null;
            }
        }
    }
}