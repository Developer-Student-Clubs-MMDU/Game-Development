using System.Collections.Generic;

namespace VladislavTsurikov.MegaWorldSystem
{
    /// <summary>
    /// A NoiseType implementation for Value noise
    /// </summary>
    [System.Serializable]
    public class ValueNoise : NoiseType<ValueNoise>
    {
        private static NoiseTypeDescriptor desc = new NoiseTypeDescriptor()
        {
            name = "Value",
            outputDir = "Assets/Vladislav Tsurikov/Mega World/Scripts/Tools System/Common/Settings/Mask Filters/Shaders/NoiseLib/",
            sourcePath = "Assets/Vladislav Tsurikov/Mega World/Scripts/Tools System/Common/Settings/Mask Filters/Shaders/NoiseLib/Implementation/ValueImpl.hlsl",
            supportedDimensions = NoiseDimensionFlags._1D | NoiseDimensionFlags._2D | NoiseDimensionFlags._3D,
            inputStructDefinition = null
        };

        public override NoiseTypeDescriptor GetDescription() => desc;
    }
}