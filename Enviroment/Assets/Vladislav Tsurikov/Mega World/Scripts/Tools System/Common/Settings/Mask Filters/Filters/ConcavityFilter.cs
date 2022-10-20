using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov;

namespace VladislavTsurikov.MegaWorldSystem 
{
    [Serializable]
    [MaskFilter("Concavity")]
    public class ConcavityFilter : MaskFilter 
    {
        public enum ConcavityMode 
        {
            Recessed = 0,
            Exposed = 1
        }

        public float ConcavityEpsilon = 1.0f; //kernel size
        public float ConcavityScalar = 1.0f;  //toggles the compute shader between recessed (1.0f) & exposed (-1.0f) surfaces
        public float ConcavityStrength = 1.0f;  //overall strength of the effect
        public AnimationCurve ConcavityRemapCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        
        readonly private int _remapTexWidth = 1024;
        private Texture2D _concavityRemapTex = null;

        Texture2D GetRemapTexture() 
        {
            if (_concavityRemapTex == null) 
            {
                _concavityRemapTex = new Texture2D(_remapTexWidth, 1, TextureFormat.RFloat, false, true);
                _concavityRemapTex.wrapMode = TextureWrapMode.Clamp;

                TextureUtility.AnimationCurveToTexture(ConcavityRemapCurve, ref _concavityRemapTex);
            }
            
            return _concavityRemapTex;
        }

        //Compute Shader resource helper
        ComputeShader _concavityCS = null;
        ComputeShader GetComputeShader() 
        {
            if (_concavityCS == null) 
            {
                _concavityCS = (ComputeShader)Resources.Load("Concavity");
            }
            return _concavityCS;
        }

        public override void Eval(MaskFilterContext fc, int index)
        {
            ComputeShader cs = GetComputeShader();
            int kidx = cs.FindKernel("ConcavityMultiply");

            Texture2D remapTex = GetRemapTexture();

            cs.SetTexture(kidx, "In_BaseMaskTex", fc.SourceRenderTexture);
            cs.SetTexture(kidx, "In_HeightTex", fc.HeightContext.sourceRenderTexture);
            cs.SetTexture(kidx, "OutputTex", fc.DestinationRenderTexture);
            cs.SetTexture(kidx, "RemapTex", remapTex);
            cs.SetInt("RemapTexRes", remapTex.width);
            cs.SetFloat("EffectStrength", ConcavityStrength);
            cs.SetVector("TextureResolution", new Vector4(fc.SourceRenderTexture.width, fc.SourceRenderTexture.height, ConcavityEpsilon, ConcavityScalar));

            //using 1s here so we don't need a multiple-of-8 texture in the compute shader (probably not optimal?)
            cs.Dispatch(kidx, fc.SourceRenderTexture.width, fc.SourceRenderTexture.height, 1);
        }

#if UNITY_EDITOR
        public override void DoGUI(Rect rect, int index)
        {
            //Precaculate dimensions
            float epsilonLabelWidth = GUI.skin.label.CalcSize(epsilonLabel).x;
            float modeLabelWidth = GUI.skin.label.CalcSize(modeLabel).x;
            float strengthLabelWidth = GUI.skin.label.CalcSize(strengthLabel).x;
            float curveLabelWidth = GUI.skin.label.CalcSize(curveLabel).x;
            float labelWidth = Mathf.Max(Mathf.Max(Mathf.Max(modeLabelWidth, epsilonLabelWidth), strengthLabelWidth), curveLabelWidth) + 4.0f;
            labelWidth += 50;

            //Concavity Mode Drop Down
            Rect modeRect = new Rect(rect.x + labelWidth, rect.y, rect.width - labelWidth, EditorGUIUtility.singleLineHeight);
            ConcavityMode mode = ConcavityScalar > 0.0f ? ConcavityMode.Recessed : ConcavityMode.Exposed;
            switch(EditorGUI.EnumPopup(modeRect, mode)) {
                case ConcavityMode.Recessed:
                    ConcavityScalar = 1.0f;
                    break;
                case ConcavityMode.Exposed:
                    ConcavityScalar = -1.0f;
                    break;
            }

            //Strength Slider
            Rect strengthLabelRect = new Rect(rect.x, modeRect.yMax, labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(strengthLabelRect, strengthLabel);
            Rect strengthSliderRect = new Rect(strengthLabelRect.xMax, strengthLabelRect.y, rect.width - labelWidth, strengthLabelRect.height);
            ConcavityStrength = EditorGUI.Slider(strengthSliderRect, ConcavityStrength, 0.0f, 1.0f);

            //Epsilon (kernel size) Slider
            Rect epsilonLabelRect = new Rect(rect.x, strengthSliderRect.yMax, labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(epsilonLabelRect, epsilonLabel);
            Rect epsilonSliderRect = new Rect(epsilonLabelRect.xMax, epsilonLabelRect.y, rect.width - labelWidth, epsilonLabelRect.height);
            ConcavityEpsilon = EditorGUI.Slider(epsilonSliderRect, ConcavityEpsilon, 1.0f, 20.0f);

            //Value Remap Curve
            Rect curveLabelRect = new Rect(rect.x, epsilonSliderRect.yMax, labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(curveLabelRect, curveLabel);
            Rect curveRect = new Rect(curveLabelRect.xMax, curveLabelRect.y, rect.width - labelWidth, curveLabelRect.height);

            EditorGUI.BeginChangeCheck();
            ConcavityRemapCurve = EditorGUI.CurveField(curveRect, ConcavityRemapCurve);
            if(EditorGUI.EndChangeCheck()) {
                Vector2 range = TextureUtility.AnimationCurveToTexture(ConcavityRemapCurve, ref _concavityRemapTex);
            }
        }

        public override float GetElementHeight(int index) 
        {
            return EditorGUIUtility.singleLineHeight * 5;
        }

        private static GUIContent strengthLabel = EditorGUIUtility.TrTextContent("Strength", "Controls the strength of the masking effect.");
        private static GUIContent epsilonLabel = EditorGUIUtility.TrTextContent("Feature Size", "Specifies the scale of Terrain features that affect the mask. This determines the size of features to which to apply the effect.");
        private static GUIContent modeLabel = EditorGUIUtility.TrTextContent("Mode");
        private static GUIContent curveLabel = EditorGUIUtility.TrTextContent("Remap Curve", "Remaps the concavity input before computing the final mask.");
#endif
    }
}
