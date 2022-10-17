using UnityEngine;

namespace VladislavTsurikov.MegaWorldSystem
{
    internal class NoiseBlitShaderGenerator : NoiseShaderGenerator<NoiseBlitShaderGenerator>
    {
        private static ShaderGeneratorDescriptor m_desc = new ShaderGeneratorDescriptor()
        {
            name = "NoiseBlit",
            shaderCategory = "Hidden/TerrainTools/Noise/NoiseBlit",
            outputDir = "Assets/Vladislav Tsurikov/Mega World/Scripts/Tools System/Common/Settings/Mask Filters/Shaders/Generated/",
            templatePath = "Assets/Vladislav Tsurikov/Mega World/Scripts/Tools System/Common/Settings/Mask Filters/Shaders/NoiseLib/Templates/Blit.noisehlsltemplate"
        };

        public override ShaderGeneratorDescriptor GetDescription() => m_desc;
    }
}