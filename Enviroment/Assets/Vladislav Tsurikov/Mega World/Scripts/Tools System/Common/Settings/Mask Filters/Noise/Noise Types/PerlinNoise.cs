using System.Collections.Generic;

namespace VladislavTsurikov.MegaWorldSystem
{
    /// <summary>
    /// A NoiseType implementation for Perlin noise
    /// </summary>
    [System.Serializable]
    public class PerlinNoise : NoiseType<PerlinNoise>
    {
        private static NoiseTypeDescriptor desc = new NoiseTypeDescriptor()
        {            
            name = "Perlin",
            outputDir = "Assets/Vladislav Tsurikov/Mega World/Scripts/Tools System/Common/Settings/Mask Filters/Shaders/NoiseLib/",
            sourcePath = "Assets/Vladislav Tsurikov/Mega World/Scripts/Tools System/Common/Settings/Mask Filters/Shaders/NoiseLib/Implementation/PerlinImpl.hlsl",
            supportedDimensions = NoiseDimensionFlags._1D | NoiseDimensionFlags._2D | NoiseDimensionFlags._3D,
            inputStructDefinition = null
        };

        public override NoiseTypeDescriptor GetDescription() => desc;
    }
}
