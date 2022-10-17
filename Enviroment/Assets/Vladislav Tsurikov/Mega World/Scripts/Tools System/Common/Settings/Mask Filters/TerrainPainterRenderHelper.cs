using UnityEngine;
#if UNITY_2021_2_OR_NEWER
#if UNITY_EDITOR
using UnityEditor.TerrainTools;
#endif
using UnityEngine.TerrainTools;
#else
#if UNITY_EDITOR
using UnityEditor.Experimental.TerrainAPI;
#endif
using UnityEngine.Experimental.TerrainAPI;
#endif

namespace VladislavTsurikov.MegaWorldSystem
{
	public class TerrainPainterRenderHelper
	{
		private readonly AreaVariables _areaVariables;
        private readonly BrushTransform _brushTransform;
        private readonly Rect _brushRect;

        public AreaVariables AreaVariables
        {
            get 
            {
                return _areaVariables;
            }
        }

        public BrushTransform BrushTransform
        {
            get
            {
                return _brushTransform;
            }
        }

		public TerrainPainterRenderHelper(AreaVariables areaVariables)
		{
			this._areaVariables = areaVariables;

            Vector2 currUV = CommonUtility.WorldPointToUV(areaVariables.RayHit.Point, areaVariables.TerrainUnderCursor);

            _brushTransform = TerrainPaintUtility.CalculateBrushTransform(areaVariables.TerrainUnderCursor, currUV, areaVariables.Size, areaVariables.Rotation);
            _brushRect = _brushTransform.GetBrushXYBounds();
		}

#region Rendering
		public void RenderBrush(PaintContext paintContext, Material material, int pass)
		{
			Texture sourceTexture = paintContext.sourceRenderTexture;
			RenderTexture destinationTexture = paintContext.destinationRenderTexture;

            Graphics.Blit(sourceTexture, destinationTexture, material, pass);
		}

#if UNITY_EDITOR
#if UNITY_2021_2_OR_NEWER
public void RenderAreaPreview(PaintContext paintContext, TerrainBrushPreviewMode previewTexture, Material material, int pass)
{
	TerrainPaintUtilityEditor.DrawBrushPreview(paintContext, previewTexture, _areaVariables.Mask, _brushTransform, material, pass);
}
#else
public void RenderAreaPreview(PaintContext paintContext, TerrainPaintUtilityEditor.BrushPreview previewTexture, Material material, int pass)
{
	TerrainPaintUtilityEditor.DrawBrushPreview(paintContext, previewTexture, _areaVariables.Mask, _brushTransform, material, pass);
}
#endif
		
#endif
		
#endregion

#region Material Set-up
		public void SetupTerrainToolMaterialProperties(PaintContext paintContext, Material material)
		{
            TerrainPaintUtility.SetupTerrainToolMaterialProperties(paintContext, _brushTransform, material);
		}
#endregion

#region Texture Acquisition
		public PaintContext AcquireHeightmap(int extraBorderPixels = 0)
		{
			return TerrainPaintUtility.BeginPaintHeightmap(_areaVariables.TerrainUnderCursor, _brushRect, extraBorderPixels);
		}

		public PaintContext AcquireTexture(TerrainLayer layer, int extraBorderPixels = 0)
		{
			return TerrainPaintUtility.BeginPaintTexture(_areaVariables.TerrainUnderCursor, _brushRect, layer, extraBorderPixels);
		}

		public PaintContext AcquireNormalmap(int extraBorderPixels = 0)
		{
			return TerrainPaintUtility.CollectNormals(_areaVariables.TerrainUnderCursor, _brushRect, extraBorderPixels);
		}

		public PaintContext AcquireHolesTexture(int extraBorderPixels = 0)
		{
#if UNITY_2019_3_OR_NEWER
			return TerrainPaintUtility.BeginPaintHoles(_areaVariables.TerrainUnderCursor, _brushRect, extraBorderPixels);
#else
			return null;
#endif
		}

#endregion
	}
}