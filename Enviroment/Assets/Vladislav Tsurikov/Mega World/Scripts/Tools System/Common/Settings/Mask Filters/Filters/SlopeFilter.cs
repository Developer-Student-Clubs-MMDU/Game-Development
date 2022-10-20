using UnityEngine;
using UnityEditor;
using System;

namespace VladislavTsurikov.MegaWorldSystem 
{
    [Serializable]
    [MaskFilter("Slope")]
    public class SlopeFilter : MaskFilter 
    {
        public BlendMode BlendMode = BlendMode.Multiply;

        public float MinSlope = 0f;
        public float MaxSlope = 20f;

        public FalloffType SlopeFalloffType = FalloffType.Add;
        public bool SlopeFalloffMinMax = false;

        public float MinAddSlopeFalloff = 30;
        public float MaxAddSlopeFalloff = 30;

        [Min(0)]
        public float AddSlopeFalloff = 30;
        
        Material slopeMat = null;
        Material GetMaterial() 
        {
            if (slopeMat == null) 
            {
                slopeMat = new Material( Shader.Find("Hidden/MegaWorld/Slope"));
            }
            return slopeMat;
        }

        public override void Eval(MaskFilterContext fc, int index) 
        {
            Material mat = GetMaterial();

            mat.SetTexture("_BaseMaskTex", fc.SourceRenderTexture);
            mat.SetTexture("_NormalTex", fc.NormalContext.sourceRenderTexture);

            SetMaterial(mat, index);

            Graphics.Blit(fc.SourceRenderTexture, fc.DestinationRenderTexture, mat, 0);
        }

        public void SetMaterial(Material mat, int index)
        {
            if(index == 0)
            {
                mat.SetInt("_BlendMode", (int)BlendMode.Multiply);
            }
            else
            {
                mat.SetInt("_BlendMode", (int)BlendMode);
            }

            mat.SetFloat("_MinSlope", MinSlope);
            mat.SetFloat("_MaxSlope", MaxSlope);

            switch (SlopeFalloffType)
            {
                case FalloffType.Add:
                {
                    float localMinAddSlopeFalloff = AddSlopeFalloff;
                    float localMaxAddSlopeFalloff = AddSlopeFalloff;

                    if(SlopeFalloffMinMax)
                    {
                        localMinAddSlopeFalloff = MinAddSlopeFalloff;
                        localMaxAddSlopeFalloff = MaxAddSlopeFalloff;
                    }

                    mat.SetInt("_SlopeFalloffType", 1);
                    mat.SetFloat("_MinAddSlopeFalloff", localMinAddSlopeFalloff);
                    mat.SetFloat("_MaxAddSlopeFalloff", localMaxAddSlopeFalloff);

                    break;
                }
                default:
                {
                    mat.SetInt("_SlopeFalloffType", 0);
                    break;
                }   
            }
        }

#if UNITY_EDITOR
        public override void DoGUI(Rect rect, int index) 
        {
            if(Terrain.activeTerrain == null)
            {
                EditorGUI.HelpBox(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "There is no terrain in the scene", MessageType.Warning);

			    rect.y += EditorGUIUtility.singleLineHeight;
            	return;
            }
            else if(Terrain.activeTerrain.drawInstanced == false)
            {
                EditorGUI.HelpBox(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Turn on Draw Instanced in all terrains, because this filter requires Normal Map", MessageType.Warning);

			    rect.y += EditorGUIUtility.singleLineHeight;
            	return;
            }

            if(index != 0)
            {
                BlendMode = (BlendMode)EditorGUI.EnumPopup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Blend Mode"), BlendMode);
                rect.y += EditorGUIUtility.singleLineHeight;
            }

            GUIStyle alignmentStyleRight = new GUIStyle(GUI.skin.label);
            alignmentStyleRight.alignment = TextAnchor.MiddleRight;
            alignmentStyleRight.stretchWidth = true;
            GUIStyle alignmentStyleLeft = new GUIStyle(GUI.skin.label);
            alignmentStyleLeft.alignment = TextAnchor.MiddleLeft;
            alignmentStyleLeft.stretchWidth = true;
            GUIStyle alignmentStyleCenter = new GUIStyle(GUI.skin.label);
            alignmentStyleCenter.alignment = TextAnchor.MiddleCenter;
            alignmentStyleCenter.stretchWidth = true;

            EditorGUI.MinMaxSlider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Slope"), ref MinSlope, ref MaxSlope, 0f, 90);
            rect.y += EditorGUIUtility.singleLineHeight * 0.5f;
            Rect slopeLabelRect = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, (rect.width - EditorGUIUtility.labelWidth) * 0.2f, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(slopeLabelRect, "0°", alignmentStyleLeft);
            slopeLabelRect = new Rect(rect.x + EditorGUIUtility.labelWidth + (rect.width - EditorGUIUtility.labelWidth) * 0.2f, rect.y, (rect.width - EditorGUIUtility.labelWidth) * 0.6f, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(slopeLabelRect, "45°", alignmentStyleCenter);
            slopeLabelRect = new Rect(rect.x + EditorGUIUtility.labelWidth + (rect.width - EditorGUIUtility.labelWidth) * 0.8f, rect.y, (rect.width - EditorGUIUtility.labelWidth) * 0.2f, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(slopeLabelRect, "90°", alignmentStyleRight);
            rect.y += EditorGUIUtility.singleLineHeight;

            //Label
            EditorGUI.LabelField(new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), new GUIContent(""));
            //Min Label
            Rect numFieldRect = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, (rect.width - EditorGUIUtility.labelWidth) * 0.2f, EditorGUIUtility.singleLineHeight);
            GUIContent minContent = new GUIContent("");

            EditorGUI.LabelField(numFieldRect, minContent, alignmentStyleLeft);
            numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);

            MinSlope = EditorGUI.FloatField(numFieldRect, MinSlope);
            numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
            
            EditorGUI.LabelField(numFieldRect, " ");
            numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);

            MaxSlope = EditorGUI.FloatField(numFieldRect, MaxSlope);
            numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);

            GUIContent maxContent = new GUIContent("");
            EditorGUI.LabelField(numFieldRect, maxContent, alignmentStyleRight);

            rect.y += EditorGUIUtility.singleLineHeight;
            
            SlopeFalloffType = (FalloffType)EditorGUI.EnumPopup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Slope Falloff Type"), SlopeFalloffType);

			rect.y += EditorGUIUtility.singleLineHeight;
            
            if(SlopeFalloffType != FalloffType.None)
			{
				SlopeFalloffMinMax = EditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Slope Falloff Min Max"), SlopeFalloffMinMax);

				rect.y += EditorGUIUtility.singleLineHeight;
                
                if(SlopeFalloffMinMax == true)
				{
					MinAddSlopeFalloff = Mathf.Max(0.1f, EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Min Add Slope Falloff"), MinAddSlopeFalloff));
					
                    rect.y += EditorGUIUtility.singleLineHeight;
                    
                    MaxAddSlopeFalloff = Mathf.Max(0.1f, EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Max Add Slope Falloff"), MaxAddSlopeFalloff));
				}
				else
				{
					AddSlopeFalloff = Mathf.Max(0.1f, EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Add Slope Falloff"), AddSlopeFalloff));
				}
			}
        }

        public override float GetElementHeight(int index) 
        {
            float height = EditorGUIUtility.singleLineHeight;

            if(Terrain.activeTerrain == null)
            {
			    height += EditorGUIUtility.singleLineHeight;
            	return height;
            }
            else if(Terrain.activeTerrain.drawInstanced == false)
            {
			    height += EditorGUIUtility.singleLineHeight;
            	return height;
            }

            if(index != 0)
            {
                height += EditorGUIUtility.singleLineHeight;
            }

            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;
			height += EditorGUIUtility.singleLineHeight;
            
            if(SlopeFalloffType != FalloffType.None)
			{
				height += EditorGUIUtility.singleLineHeight;
                
                if(SlopeFalloffMinMax == true)
				{					
                    height += EditorGUIUtility.singleLineHeight;
                    height += EditorGUIUtility.singleLineHeight;
                }
				else
				{
                    height += EditorGUIUtility.singleLineHeight;
                }
			}

            return height;
        }
#endif
    }
}
