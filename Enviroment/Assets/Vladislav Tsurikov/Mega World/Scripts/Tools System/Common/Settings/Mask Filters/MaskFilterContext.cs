using System.Collections.Generic;
using UnityEngine;
#if UNITY_2021_2_OR_NEWER
using UnityEngine.TerrainTools;
#else
using UnityEngine.Experimental.TerrainAPI;
#endif

namespace VladislavTsurikov.MegaWorldSystem
{
    public class MaskFilterContext 
    {
        public AreaVariables AreaVariables;
        public Vector3 BrushPos;
        public RenderTexture SourceRenderTexture;
        public RenderTexture DestinationRenderTexture;
        public Dictionary<string, float> Properties;
        public PaintContext HeightContext; 
        public PaintContext NormalContext;
        public RenderTexture Output;

        public MaskFilterContext(AreaVariables areaVariables)
        {
            AreaVariables = areaVariables;
        }

        public MaskFilterContext(MaskFilterStack maskFilterStack, PaintContext heightContext, PaintContext normalContext, RenderTexture output, AreaVariables areaVariables)
        {
            AreaVariables = areaVariables;
            BrushPos = new Vector3(areaVariables.RayHit.Point.x, 0, areaVariables.RayHit.Point.z);
            Properties = new Dictionary<string, float>();
            SourceRenderTexture = null;
            DestinationRenderTexture = null;
            HeightContext = heightContext;
            NormalContext = normalContext;
            Output = output;
            Properties.Add("brushRotation", areaVariables.Rotation);
            Properties.Add("terrainScale", Mathf.Sqrt(areaVariables.TerrainUnderCursor.terrainData.size.x * areaVariables.TerrainUnderCursor.terrainData.size.x + 
                areaVariables.TerrainUnderCursor.terrainData.size.z * areaVariables.TerrainUnderCursor.terrainData.size.z));
            DestinationRenderTexture = output;
            
            maskFilterStack.Eval(this);
        }

        public RenderTexture GetFilterMaskRT()
        {
            return Output;
        }

        public void DisposeUnmanagedMemory()
        {
            if(HeightContext != null)
            {
                TerrainPaintUtility.ReleaseContextResources(HeightContext);
                HeightContext = null;
            }
            if(NormalContext != null)
            {
                TerrainPaintUtility.ReleaseContextResources(NormalContext);
                NormalContext = null;
            }
            if(Output != null)
            {
                Output.Release();
                Output = null;
            }
        }
    }
}