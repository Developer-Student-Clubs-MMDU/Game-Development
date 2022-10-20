using UnityEngine;

namespace VladislavTsurikov.MegaWorldSystem
{
    public class AreaVariables
    {
        public float Size;
        public float SizeMultiplier = 1;
		public float Rotation = 0;
        public float CosAngle;
        public float SinAngle;
        public RayHit RayHit;
        public Bounds Bounds;
        public Terrain TerrainUnderCursor;
        public Texture2D Mask;

        public float Radius
        {
            get
            {
                return Size / 2;
            }
        }

        public AreaVariables() {}

        public AreaVariables(Bounds bounds, RayHit rayHit)
        {
            Mask = Texture2D.whiteTexture;
            Size = bounds.size.x;
            RayHit = rayHit;
            TerrainUnderCursor = CommonUtility.GetTerrain(rayHit.Point);
            Bounds = bounds;
        }
    }
}