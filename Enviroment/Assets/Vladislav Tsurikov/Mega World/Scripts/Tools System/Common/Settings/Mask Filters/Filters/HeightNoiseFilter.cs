#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace VladislavTsurikov.MegaWorldSystem 
{
    [Serializable]
    [MaskFilter("Height Noise")]
    public class HeightNoiseFilter : MaskFilter 
    {
        public NoiseSettings NoiseSettings;

        public BlendMode BlendMode = BlendMode.Multiply;

        public float MinHeight = 0;
        public float MaxHeight = 0;

        [SerializeField]
        private float _maxRangeNoise = 5f;
        public float MaxRangeNoise
        {
            get
            {
                return _maxRangeNoise;
            }
            set
            {
                if(value < 0.1)
                {
                    _maxRangeNoise = 0.1f;
                }
                else
                {
                    _maxRangeNoise = value;
                }
            }
        }

        [SerializeField]
        private float _minRangeNoise = 5f;
        public float MinRangeNoise
        {
            get
            {
                return _minRangeNoise;
            }
            set
            {
                if(value < 0.1)
                {
                    _minRangeNoise = 0.1f;
                }
                else
                {
                    _minRangeNoise = value;
                }
            }
        }

        private Material _heightNoiseMat = null;
        public Material GetMaterial() 
        {
            if (_heightNoiseMat == null) 
            {
                _heightNoiseMat = new Material( Shader.Find( "Hidden/MegaWorld/HeightNoise"));
            }
            return _heightNoiseMat;
        }

        public override void Eval(MaskFilterContext fc, int index)
        {
            CreateNoiseSettingsIfNecessary();

            Vector3 brushPosWS = fc.BrushPos;
            float brushSize = fc.AreaVariables.Size;
            float brushRotation = fc.AreaVariables.Rotation;

            // TODO(wyatt): remove magic number and tie it into NoiseSettingsGUI preview size somehow
            float previewSize = 1 / 512f;

            // get proper noise material from current noise settings
            Material mat = NoiseUtils.GetDefaultBlitMaterial(NoiseSettings);

            // setup the noise material with values in noise settings
            NoiseSettings.SetupMaterial( mat );

            // convert brushRotation to radians
            brushRotation *= Mathf.PI / 180;

            // change pos and scale so they match the noiseSettings preview
            bool isWorldSpace = true;
            //brushSize = isWorldSpace ? brushSize * previewSize : 1;
            brushSize = brushSize * previewSize;
            brushPosWS = isWorldSpace ? brushPosWS * previewSize : Vector3.zero;

            // // override noise transform
            Quaternion rotQ             = Quaternion.AngleAxis( -brushRotation, Vector3.up );
            Matrix4x4 translation       = Matrix4x4.Translate( brushPosWS );
            Matrix4x4 rotation          = Matrix4x4.Rotate( rotQ );
            Matrix4x4 scale             = Matrix4x4.Scale( Vector3.one * brushSize );
            Matrix4x4 noiseToWorld      = translation * scale;

            mat.SetMatrix( NoiseSettings.ShaderStrings.Transform, NoiseSettings.trs * noiseToWorld );

            int pass = NoiseUtils.kNumBlitPasses * NoiseLib.GetNoiseIndex( NoiseSettings.DomainSettings.NoiseTypeName );

            RenderTextureDescriptor desc = new RenderTextureDescriptor(fc.DestinationRenderTexture.width, fc.DestinationRenderTexture.height, RenderTextureFormat.RFloat);
            RenderTexture rt = RenderTexture.GetTemporary( desc );

            Graphics.Blit(fc.SourceRenderTexture, rt, mat, pass);

            Material matFinal = GetMaterial(); 

            matFinal.SetTexture("_NoiseTex", rt);
            matFinal.SetTexture("_BaseMaskTex", fc.SourceRenderTexture);
            matFinal.SetTexture("_HeightTex", fc.HeightContext.sourceRenderTexture);

            SetMaterial(matFinal, fc, index);

            Graphics.Blit(fc.SourceRenderTexture, fc.DestinationRenderTexture, matFinal, 0);

            RenderTexture.ReleaseTemporary(rt);
        }

        public void SetMaterial(Material cs, MaskFilterContext fc, int index)
        {
            if(index == 0)
            {
                cs.SetInt("_BlendMode", (int)BlendMode.Multiply);
            }
            else
            {
                cs.SetInt("_BlendMode", (int)BlendMode);
            }

            cs.SetFloat("_MinHeight", MinHeight);
            cs.SetFloat("_MaxHeight", MaxHeight);

            cs.SetFloat("_ClampMinHeight", fc.AreaVariables.TerrainUnderCursor.transform.position.y);
            cs.SetFloat("_ClampMaxHeight", fc.AreaVariables.TerrainUnderCursor.terrainData.size.y + fc.AreaVariables.TerrainUnderCursor.transform.position.y);

            cs.SetFloat("_MaxRangeNoise", MaxRangeNoise);
            cs.SetFloat("_MinRangeNoise", MinRangeNoise);
        }

#if UNITY_EDITOR
        private NoiseSettingsGUI m_noiseSettingsGUI;
        private NoiseSettingsGUI noiseSettingsGUI
        {
            get
            {
                if(m_noiseSettingsGUI == null)
                {
                    m_noiseSettingsGUI = new NoiseSettingsGUI(NoiseSettings);
                }

                return m_noiseSettingsGUI;
            }
        }

        public override void DoGUI(Rect rect, int index) 
        {
            if(index != 0)
            {
                BlendMode = (BlendMode)EditorGUI.EnumPopup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Blend Mode"), BlendMode);
                rect.y += EditorGUIUtility.singleLineHeight;
            }

            CreateNoiseSettingsIfNecessary();

            DrawHeightSettings(ref rect);

            noiseSettingsGUI.OnGUI(rect);
        }

        public void DrawHeightSettings(ref Rect rect)
        {
            MinHeight = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Min Height"), MinHeight);

            rect.y += EditorGUIUtility.singleLineHeight;

            MaxHeight = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Max Height"), MaxHeight);
                        
			rect.y += EditorGUIUtility.singleLineHeight;     

            MinRangeNoise = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Min Range Noise"), MinRangeNoise);
            rect.y += EditorGUIUtility.singleLineHeight;
			MaxRangeNoise = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Max Range Noise"), MaxRangeNoise);
            rect.y += EditorGUIUtility.singleLineHeight;
        }

        public override float GetElementHeight(int index) 
        {
            CreateNoiseSettingsIfNecessary();
            
            float height = EditorGUIUtility.singleLineHeight * 8;
            height += EditorGUIUtility.singleLineHeight;

            if (NoiseSettings.ShowNoisePreviewTexture)
            {
                height += 256f + 40f;
            }
            else
            {
                height += EditorGUIUtility.singleLineHeight;
            }
            if (NoiseSettings.ShowNoiseTransformSettings)
            {
                height += EditorGUIUtility.singleLineHeight * 7;
            }
            else
            {
                height += EditorGUIUtility.singleLineHeight;
            }
            if (NoiseSettings.ShowNoiseTypeSettings)
            {
                height += EditorGUIUtility.singleLineHeight * 15;
            }
            else
            {
                height += EditorGUIUtility.singleLineHeight;
            }
            height += EditorGUIUtility.singleLineHeight;

            return height;
        }
#endif

        private void CreateNoiseSettingsIfNecessary()
        {
            if(NoiseSettings == null)
            {
                NoiseSettings = new NoiseSettings();
            }
        }
    }
}
#endif