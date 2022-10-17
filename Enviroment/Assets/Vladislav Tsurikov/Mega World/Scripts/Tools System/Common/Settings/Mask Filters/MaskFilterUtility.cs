using UnityEngine;

namespace VladislavTsurikov.MegaWorldSystem
{
    public static class MaskFilterUtility
    {
        private static Material m_paintTextureMaterial;
        public static Material GetPaintMaterial()
		{
			if (m_paintTextureMaterial == null)
            {
                m_paintTextureMaterial = new Material(Shader.Find("PaintTexture"));
            }
				
			return m_paintTextureMaterial;
		}

        private static Material m_blendModesMaterial;
        public static Material blendModesMaterial
        {
            get
            {
                if( m_blendModesMaterial == null )
                {
                    m_blendModesMaterial = new Material( Shader.Find( "Hidden/MegaWorld/BlendModes" ) );
                }

                return m_blendModesMaterial;
            }
        }

        public static Material m_BlendMat = null;
		public static Material GetBlendMaterial()
		{
			if (m_BlendMat == null)
			{
				m_BlendMat = new Material(Shader.Find("Hidden/TerrainTools/BlendModes"));
			}
			return m_BlendMat;
		}

		public static Material m_BrushPreviewMat = null;
		public static Material GetBrushPreviewMaterial()
		{
			if (m_BrushPreviewMat == null)
			{
				m_BrushPreviewMat = new Material(Shader.Find("Hidden/MegaWorld/PaintMaterialBrushPreview"));
			}
			return m_BrushPreviewMat;
		}
    }
}