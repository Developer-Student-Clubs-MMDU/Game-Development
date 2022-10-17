using System.Collections.Generic;

namespace VladislavTsurikov.MegaWorldSystem
{
    /// <summary>
    /// A NoiseType implementation for Voronoi noise
    /// </summary>
    [System.Serializable]
    public class VoronoiNoise : NoiseType<VoronoiNoise>
    {
        private static NoiseTypeDescriptor desc = new NoiseTypeDescriptor()
        {
            name = "Voronoi",
            outputDir = "Assets/Vladislav Tsurikov/Mega World/Scripts/Tools System/Common/Settings/Mask Filters/Shaders/NoiseLib/",
            sourcePath = "Assets/Vladislav Tsurikov/Mega World/Scripts/Tools System/Common/Settings/Mask Filters/Shaders/NoiseLib/Implementation/VoronoiImpl.hlsl",
            supportedDimensions = NoiseDimensionFlags._1D | NoiseDimensionFlags._2D | NoiseDimensionFlags._3D,
            inputStructDefinition = null
        };

        public override NoiseTypeDescriptor GetDescription() => desc;
    }
}