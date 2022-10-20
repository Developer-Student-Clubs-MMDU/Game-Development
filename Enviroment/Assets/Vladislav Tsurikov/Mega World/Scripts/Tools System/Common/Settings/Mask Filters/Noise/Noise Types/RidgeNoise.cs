using System.Collections.Generic;

namespace VladislavTsurikov.MegaWorldSystem
{
    /// <summary>
    /// A NoiseType implementation for Ridge noise
    /// </summary>
    [System.Serializable]
    public class RidgeNoise : NoiseType<RidgeNoise>
    {
        private static NoiseTypeDescriptor desc = new NoiseTypeDescriptor()
        {
            name = "Ridge",
            outputDir = "Assets/Vladislav Tsurikov/Mega World/Scripts/Tools System/Common/Settings/Mask Filters/Shaders/NoiseLib/",
            sourcePath = "Assets/Vladislav Tsurikov/Mega World/Scripts/Tools System/Common/Settings/Mask Filters/Shaders/NoiseLib/Implementation/RidgeImpl.hlsl",
            supportedDimensions = NoiseDimensionFlags._1D | NoiseDimensionFlags._2D | NoiseDimensionFlags._3D,
            inputStructDefinition = null
        };

        public override NoiseTypeDescriptor GetDescription() => desc;
    }
}
