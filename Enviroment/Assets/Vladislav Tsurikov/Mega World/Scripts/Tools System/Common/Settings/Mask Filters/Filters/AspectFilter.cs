using UnityEngine;
using UnityEditor;
using System;

namespace VladislavTsurikov.MegaWorldSystem 
{
    [Serializable]
    [MaskFilter("Aspect")]
    public class AspectFilter : MaskFilter 
    {
        public BlendMode BlendMode = BlendMode.Multiply;
        public float Rotation = 0;
        public float Epsilon = 1.0f; //kernel size
        public float EffectStrength = 1.0f;  //overall strength of the effect

        private static readonly int s_remapTexWidth = 1024;

        //We bake an AnimationCurve to a texture to control value remapping
        [SerializeField]
        private AnimationCurve _remapCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        private Texture2D _remapTex = null;

        Texture2D GetRemapTexture() 
        {
            if (_remapTex == null) 
            {
                _remapTex = new Texture2D(s_remapTexWidth, 1, TextureFormat.RFloat, false, true);
                _remapTex.wrapMode = TextureWrapMode.Clamp;

                TextureUtility.AnimationCurveToTexture(_remapCurve, ref _remapTex);
            }
            
            return _remapTex;
        }

        //Compute Shader resource helper
        ComputeShader m_AspectCS = null;
        ComputeShader GetComputeShader() {
            if (m_AspectCS == null) {
                m_AspectCS = (ComputeShader)Resources.Load("Aspect");
            }
            return m_AspectCS;
        }

        public override void Eval(MaskFilterContext fc, int index) 
        {
            ComputeShader cs = GetComputeShader();
            int kidx = cs.FindKernel("AspectRemap");

            //using 1s here so we don't need a multiple-of-8 texture in the compute shader (probably not optimal?)
            int[] numWorkGroups = { 1, 1, 1 };

            Texture2D remapTex = GetRemapTexture();

            //float rotRad = (fc.properties["brushRotation"] - 90.0f) * Mathf.Deg2Rad;
            float rotRad = (Rotation - 90.0f) * Mathf.Deg2Rad;

            if(index == 0)
            {
                cs.SetInt("_BlendMode", (int)BlendMode.Multiply);
            }
            else
            {
                cs.SetInt("_BlendMode", (int)BlendMode);
            }

            cs.SetTexture(kidx, "In_BaseMaskTex", fc.SourceRenderTexture);
            cs.SetTexture(kidx, "In_HeightTex", fc.HeightContext.sourceRenderTexture);
            cs.SetTexture(kidx, "OutputTex", fc.DestinationRenderTexture);
            cs.SetTexture(kidx, "RemapTex", remapTex);
            cs.SetInt("RemapTexRes", remapTex.width);
            cs.SetFloat("EffectStrength", EffectStrength);
            cs.SetVector("TextureResolution", new Vector4(fc.SourceRenderTexture.width, fc.SourceRenderTexture.height, 0.0f, 0.0f));
            cs.SetVector("AspectValues", new Vector4(Mathf.Cos(rotRad), Mathf.Sin(rotRad), Epsilon, 0.0f));

            cs.Dispatch(kidx, fc.SourceRenderTexture.width / numWorkGroups[0], fc.SourceRenderTexture.height / numWorkGroups[1], numWorkGroups[2]);
        }

#if UNITY_EDITOR
        public override void DoGUI(Rect rect, int index) 
        {
            if(index != 0)
            {
                BlendMode = (BlendMode)EditorGUI.EnumPopup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Blend Mode"), BlendMode);
                rect.y += EditorGUIUtility.singleLineHeight;
            }

            EditorGUI.BeginChangeCheck();

            Rotation = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Rotation"), Rotation, -180, 180);
            rect.y += EditorGUIUtility.singleLineHeight;
            EffectStrength = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), strengthLabel, EffectStrength, 0.0f, 1.0f);
            rect.y += EditorGUIUtility.singleLineHeight;
            _remapCurve = EditorGUI.CurveField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), curveLabel, _remapCurve);

            if(EditorGUI.EndChangeCheck()) 
            {
                Vector2 range = TextureUtility.AnimationCurveToTexture(_remapCurve, ref _remapTex);
            }
        }

        public override float GetElementHeight(int index) 
        {
            float height = EditorGUIUtility.singleLineHeight;

            if(index != 0)
            {
                height += EditorGUIUtility.singleLineHeight;
            }

            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;

            return height;
        }

        private static GUIContent strengthLabel = EditorGUIUtility.TrTextContent("Strength", "Controls the strength of the masking effect.");
        private static GUIContent curveLabel = EditorGUIUtility.TrTextContent("Remap Curve", "Remaps the concavity input before computing the final mask.");
#endif
    }
}