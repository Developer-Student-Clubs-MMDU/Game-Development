using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    /// <summary>
    /// A data class that can be used to define various types of noise.
    /// </summary>
    [System.Serializable]
    public class NoiseSettings //: ScriptableObject
    {
        /// <summary>
        /// Struct containing information about the transform (translation, rotation, scale) to be
        /// applied to the noise. Usage of these values is depending on the actual noise
        /// implementation.
        /// </summary>
        [System.Serializable]
        public class NoiseTransformSettings
        {
            /// <summary>
            /// The translation used when generating the noise field
            /// </summary>
            [Tooltip("The translational offset to use when generating the noise")]
            public Vector3          Translation = Vector3.zero;

            /// <summary>
            /// The scale used when generating the noise field
            /// </summary>
            [Tooltip("The scale of the generated noise")]
            public Vector3          Scale = Vector3.one * 40f;

            /// <summary>
            /// The rotation used when generating the noise field
            /// </summary>
            [Tooltip("The rotation of the generated noise")]
            public Vector3          Rotation = Vector3.zero;

            /// <summary>
            /// Determines whether the scale should be flipped across the x-axis
            /// </summary>
            [Tooltip("Whether or not the 2D noise field should be flipped along the X-axis")]
            public bool             FlipScaleX = false;

            /// <summary>
            /// Determines whether the scale should be flipped across the y-axis
            /// </summary>
            [Tooltip("Whether or not the 2D noise field should be flipped along the Y-axis")]
            public bool             FlipScaleY = false;

            /// <summary>
            /// Determines whether the scale should be flipped across the z-axis
            /// </summary>
            [Tooltip("Whether or not the 2D noise field should be flipped along the Z-axis")]
            public bool             FlipScaleZ = false;

            /// <summary>
            /// Resets the members of the transform struct to their default states and values
            /// </summary>
            public void Reset()
            {
                Translation = Vector3.zero;
                Scale = Vector3.one * 40f;
                Rotation = Vector3.zero;

                FlipScaleX = false;
                FlipScaleY = false;
                FlipScaleZ = false;
            }
        }

        /// <summary>
        /// Struct containing strings that reference the noise type and fractal type in use
        /// as well as the serialized data associated with the noise and fractal type.
        /// </summary>
        [System.Serializable]
        public class NoiseDomainSettings
        {
            /// <summary>
            /// String representing the name of the NoiseType that is in use
            /// </summary>
            [Tooltip("The type of noise being generated")]
            public string NoiseTypeName = PerlinNoise.instance.GetDescription().name;

            /// <summary>
            /// String representing the name of the FractalType that is in use
            /// </summary>
            [Tooltip("The type of fractal used with the generated noise")]
            public string FractalTypeName = FbmFractalType.instance.GetDescription().name;

            /// <summary>
            /// String containing serialized data for the active NoiseType
            /// </summary>
            [Tooltip("Settings specific to noise type")]
            public string NoiseTypeParams = PerlinNoise.instance.GetDefaultSerializedString();

            /// <summary>
            /// String containing serialized data for the active FractalType
            /// </summary>
            [Tooltip("Settings specific to noise type")]
            public string FractalTypeParams = FbmFractalType.instance.GetDefaultSerializedString();

            /// <summary>
            /// Resets the domain settings to the defaults for the built-in Perlin NoiseType and Fbm FractalType
            /// </summary>

            public void Reset()
            {
                NoiseTypeName = PerlinNoise.instance.GetDescription().name;
                FractalTypeName = FbmFractalType.instance.GetDescription().name;
                NoiseTypeParams = PerlinNoise.instance.GetDefaultSerializedString();
                FractalTypeParams = FbmFractalType.instance.GetDefaultSerializedString();
            }
        }

        public bool ShowNoiseTransformSettings = true;
        public bool ShowNoiseTypeSettings = true;
        public bool ShowNoisePreviewTexture = true;

        /// <summary>
        /// The transform settings for the defined noise field
        /// </summary>
        [Tooltip("Settings for noise transform and coordinate space")]
        public NoiseTransformSettings TransformSettings;
        /// <summary>
        /// The domain settings for the defined noise field. Contains serialized data
        /// defining the NoiseType and FractalType for this NoiseSettings instance.
        /// </summary>
        [Tooltip("Settings for noise domain")]
        public NoiseDomainSettings DomainSettings;


        /// <summary>
        /// The noise field's TRS transformation Matrix
        /// </summary>
        /// <returns> A Matrix4x4 for the noise field's TRS matrix </returns>
        public Matrix4x4 trs
        {
            get
            {
                // set noise transform values
                Vector3 scale = TransformSettings.Scale;
                scale.x = TransformSettings.FlipScaleX ? -scale.x : scale.x;
                scale.y = TransformSettings.FlipScaleY ? -scale.y : scale.y;
                scale.z = TransformSettings.FlipScaleZ ? -scale.z : scale.z;

                // scale.w = transformSettings.flipScaleW ? -scale.w : scale.w;

                Matrix4x4 trs = Matrix4x4.TRS(TransformSettings.Translation,
                                            Quaternion.Euler(TransformSettings.Rotation),
                                            scale);

                return trs;
            }
        }

        public void SetupMaterial(Material mat)
        {
            INoiseType noiseType = NoiseLib.GetNoiseTypeInstance(DomainSettings.NoiseTypeName);
            IFractalType fractalType = NoiseLib.GetFractalTypeInstance(DomainSettings.FractalTypeName);

            noiseType?.SetupMaterial(mat, DomainSettings.NoiseTypeParams);
            fractalType?.SetupMaterial(mat, DomainSettings.FractalTypeParams);
        }

        /// <summary>
        /// Class containing the string names of shader properties used by noise shaders
        /// </summary>
        public static class ShaderStrings
        {
            /// <summary>
            /// Property for noise translation. Value = "_NoiseTranslation"
            /// </summary>
            public static readonly string Translation = "_NoiseTranslation";
            /// <summary>
            /// Property for noise scale. Value = "_NoiseScale"
            /// </summary>
            public static readonly string Scale =       "_NoiseScale";
            /// <summary>
            /// Property for noise rotation. Value = "_NoiseRotation"
            /// </summary>
            public static readonly string Rotation =    "_NoiseRotation";
            /// <summary>
            /// Property for the noise transform. Value = "_NoiseTransform"
            /// </summary>
            public static readonly string Transform =   "_NoiseTransform";
        }
    }
}