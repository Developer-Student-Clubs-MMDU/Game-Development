using UnityEngine;
using UnityEditor;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    public enum MaskOperations
    {
        Add,
        Multiply,
        Power,
        Invert,
        Clamp,
        Remap,
    }

    [Serializable]
    [MaskFilter("Mask Operations")]  
    public class MaskOperationsFilter : MaskFilter
    {
        public MaskOperations MaskOperations = MaskOperations.Add;

        #region Clamp
        public Vector2 ClampRange = new Vector2(0, 1);
        #endregion

        #region Remap
        public Vector2 RemapRange = new Vector2(0.4f, 0.6f);
        #endregion

        #region Invert
        public float StrengthInvert = 1;
        #endregion

        public float Value;

        private static Material _builtinMaterial;
        public static Material BuiltinMaterial
        {
            get
            {
                if(_builtinMaterial == null)
                {
                    _builtinMaterial = new Material(Shader.Find("Hidden/TerrainTools/MaskOperations"));
                }

                return _builtinMaterial;
            }
        }

        public override void Eval(MaskFilterContext fc, int index)
        {
            switch (MaskOperations)
            {
                case MaskOperations.Add:
                {
                    BuiltinMaterial.SetFloat("_Add", Value);

                    Graphics.Blit( fc.SourceRenderTexture, fc.DestinationRenderTexture, BuiltinMaterial, (int)MaskOperations.Add );
                    break;
                }
                case MaskOperations.Multiply:
                {
                    BuiltinMaterial.SetFloat("_Multiply", Value);

                    Graphics.Blit( fc.SourceRenderTexture, fc.DestinationRenderTexture, BuiltinMaterial, (int)MaskOperations.Multiply );
                    break;
                }
                case MaskOperations.Power:
                {
                    BuiltinMaterial.SetFloat("_Pow", Value);

                    Graphics.Blit( fc.SourceRenderTexture, fc.DestinationRenderTexture, BuiltinMaterial, (int)MaskOperations.Power );
                    break;
                }
                case MaskOperations.Clamp:
                {
                    BuiltinMaterial.SetVector("_ClampRange", ClampRange);

                    Graphics.Blit( fc.SourceRenderTexture, fc.DestinationRenderTexture, BuiltinMaterial, (int)MaskOperations.Clamp );
                    break;
                }
                case MaskOperations.Invert:
                {
                    BuiltinMaterial.SetFloat("_Strength", StrengthInvert);

                    Graphics.Blit( fc.SourceRenderTexture, fc.DestinationRenderTexture, BuiltinMaterial, (int)MaskOperations.Invert);
                    break;
                }
                case MaskOperations.Remap:
                {
                    BuiltinMaterial.SetFloat("RemapMin", RemapRange.x);
                    BuiltinMaterial.SetFloat("RemapMax", RemapRange.y);
        
                    Graphics.Blit( fc.SourceRenderTexture, fc.DestinationRenderTexture, BuiltinMaterial, (int)MaskOperations.Remap);
                    break;
                }
            }
            
        }

#if UNITY_EDITOR
        public override void DoGUI(Rect rect, int index)
        {
            MaskOperations = (MaskOperations)EditorGUI.EnumPopup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), MaskOperations);

            rect.y += EditorGUIUtility.singleLineHeight;

            switch (MaskOperations)
            {
                case MaskOperations.Add:
                {
                    Value = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Value"), Value, 0f, 1f);
                    break;
                }
                case MaskOperations.Multiply:
                {
                    Value = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Value"), Value, 0f, 1f);
                    break;
                }
                case MaskOperations.Power:
                {
                    Value = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Value"), Value, 1f, 10f);
                    break;
                }
                case MaskOperations.Clamp:
                {
                    GUIStyle alignmentStyleRight = new GUIStyle(GUI.skin.label);
                    alignmentStyleRight.alignment = TextAnchor.MiddleRight;
                    alignmentStyleRight.stretchWidth = true;
                    GUIStyle alignmentStyleLeft = new GUIStyle(GUI.skin.label);
                    alignmentStyleLeft.alignment = TextAnchor.MiddleLeft;
                    alignmentStyleLeft.stretchWidth = true;

                    EditorGUI.MinMaxSlider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Range"), ref ClampRange.x, ref ClampRange.y, 0, 1);

                    rect.y += EditorGUIUtility.singleLineHeight;

                    Rect numFieldRect = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, (rect.width - EditorGUIUtility.labelWidth) * 0.2f, EditorGUIUtility.singleLineHeight);
                    GUIContent minContent = new GUIContent("");
                    EditorGUI.LabelField(numFieldRect, minContent, alignmentStyleLeft);
                    numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
                    ClampRange.x = EditorGUI.FloatField(numFieldRect, ClampRange.x);
                    numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
                    EditorGUI.LabelField(numFieldRect, " ");
                    numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
                    ClampRange.y = EditorGUI.FloatField(numFieldRect, ClampRange.y);
                    numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
                    GUIContent maxContent = new GUIContent("");
                    EditorGUI.LabelField(numFieldRect, maxContent, alignmentStyleRight);

                    rect.y += EditorGUIUtility.singleLineHeight;
                    break;
                }
                case MaskOperations.Invert:
                {
                    StrengthInvert = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Strength"), StrengthInvert, 0f, 1f);

                    break;
                }
                case MaskOperations.Remap:
                {
                    GUIStyle alignmentStyleRight = new GUIStyle(GUI.skin.label);
                    alignmentStyleRight.alignment = TextAnchor.MiddleRight;
                    alignmentStyleRight.stretchWidth = true;
                    GUIStyle alignmentStyleLeft = new GUIStyle(GUI.skin.label);
                    alignmentStyleLeft.alignment = TextAnchor.MiddleLeft;
                    alignmentStyleLeft.stretchWidth = true;

                    EditorGUI.MinMaxSlider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Range"), ref RemapRange.x, ref RemapRange.y, 0, 1);

                    rect.y += EditorGUIUtility.singleLineHeight;

                    Rect numFieldRect = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, (rect.width - EditorGUIUtility.labelWidth) * 0.2f, EditorGUIUtility.singleLineHeight);
                    GUIContent minContent = new GUIContent("");
                    EditorGUI.LabelField(numFieldRect, minContent, alignmentStyleLeft);
                    numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
                    RemapRange.x = EditorGUI.FloatField(numFieldRect, RemapRange.x);
                    numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
                    EditorGUI.LabelField(numFieldRect, " ");
                    numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
                    RemapRange.y = EditorGUI.FloatField(numFieldRect, RemapRange.y);
                    numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
                    GUIContent maxContent = new GUIContent("");
                    EditorGUI.LabelField(numFieldRect, maxContent, alignmentStyleRight);

                    rect.y += EditorGUIUtility.singleLineHeight;
                    break;
                }
            }
        }

        public override float GetElementHeight(int index) 
        {
            float height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;

            switch (MaskOperations)
            {
                case MaskOperations.Add:
                case MaskOperations.Multiply:
                case MaskOperations.Power:
                case MaskOperations.Invert:
                {
                    height += EditorGUIUtility.singleLineHeight;
                    break;
                }
                case MaskOperations.Clamp:
                case MaskOperations.Remap:
                {
                    height += EditorGUIUtility.singleLineHeight;
                    height += EditorGUIUtility.singleLineHeight;
                    break;
                }
            }

            return height;
        }

        public override string GetAdditionalName()
        {
            return "[" + MaskOperations.ToString() + "]";
        }
#endif
    }
}