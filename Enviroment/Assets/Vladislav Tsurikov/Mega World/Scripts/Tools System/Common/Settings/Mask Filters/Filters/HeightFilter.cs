using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VladislavTsurikov.MegaWorldSystem 
{
    [Serializable]
    [MaskFilter("Height")]
    public class HeightFilter : MaskFilter 
    {
        public BlendMode BlendMode = BlendMode.Multiply;

        public float MinHeight = 0;
        public float MaxHeight = 0;

        public FalloffType HeightFalloffType = FalloffType.Add;
        public bool HeightFalloffMinMax = false;

        public float MinAddHeightFalloff = 30;
        public float MaxAddHeightFalloff = 30;

        [Min(0)]
        public float AddHeightFalloff = 30;

        public Vector2 Height = new Vector2(0.0f, 1.0f);
        public float HeightFeather = 0.0f;

        private ComputeShader _heightCS = null;
        public ComputeShader GetComputeShader() 
        {
            if (_heightCS == null) {
                _heightCS = (ComputeShader)Resources.Load("Height");
            }
            return _heightCS;
        }

        public override void Eval(MaskFilterContext fc, int index) 
        {
            ComputeShader cs = GetComputeShader();
            int kidx = cs.FindKernel("Height");

            cs.SetTexture(kidx, "In_BaseMaskTex", fc.SourceRenderTexture);
            cs.SetTexture(kidx, "In_HeightTex", fc.HeightContext.sourceRenderTexture);
            cs.SetTexture(kidx, "OutputTex", fc.DestinationRenderTexture);

            cs.SetVector("HeightRange", new Vector4(Height.x, Height.y, HeightFeather, 0.0f));

            SetMaterial(cs, fc, index);

            //using workgroup size of 1 here to avoid needing to resize render textures
            cs.Dispatch(kidx, fc.SourceRenderTexture.width, fc.SourceRenderTexture.height, 1);
        }

        public void SetMaterial(ComputeShader cs, MaskFilterContext fс, int index)
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

            cs.SetFloat("_ClampMinHeight", fс.AreaVariables.TerrainUnderCursor.transform.position.y);
            cs.SetFloat("_ClampMaxHeight", fс.AreaVariables.TerrainUnderCursor.terrainData.size.y + fс.AreaVariables.TerrainUnderCursor.transform.position.y);

            switch (HeightFalloffType)
            {
                case FalloffType.Add:
                {
                    float localMinAddHeightFalloff = AddHeightFalloff;
                    float localMaxAddHeightFalloff = AddHeightFalloff;

                    if(HeightFalloffMinMax)
                    {
                        localMinAddHeightFalloff = MinAddHeightFalloff;
                        localMaxAddHeightFalloff = MaxAddHeightFalloff;
                    }

                    cs.SetInt("_HeightFalloffType", 1);
                    cs.SetFloat("_MinAddHeightFalloff", localMinAddHeightFalloff);
                    cs.SetFloat("_MaxAddHeightFalloff", localMaxAddHeightFalloff);
                    break;
                }
                default:
                {
                    cs.SetInt("_HeightFalloffType", 0);
                    break;
                }   
            }
        }

#if UNITY_EDITOR
        public override void DoGUI(Rect rect, int index) 
        {
            if(index != 0)
            {
                BlendMode = (BlendMode)EditorGUI.EnumPopup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Blend Mode"), BlendMode);
                rect.y += EditorGUIUtility.singleLineHeight;
            }

            MinHeight = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Min Height"), MinHeight);

            rect.y += EditorGUIUtility.singleLineHeight;

            MaxHeight = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Max Height"), MaxHeight);

            rect.y += EditorGUIUtility.singleLineHeight;

            HeightFalloffType = (FalloffType)EditorGUI.EnumPopup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Height Falloff Type"), HeightFalloffType);

			rect.y += EditorGUIUtility.singleLineHeight;
            
            if(HeightFalloffType != FalloffType.None)
			{
				HeightFalloffMinMax = EditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Height Falloff Min Max"), HeightFalloffMinMax);
			
				rect.y += EditorGUIUtility.singleLineHeight;
                
                if(HeightFalloffMinMax)
				{
					MinAddHeightFalloff = Mathf.Max(0.1f, EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Min Add Height Falloff"), MinAddHeightFalloff));
					
                    rect.y += EditorGUIUtility.singleLineHeight;
                    
                    MaxAddHeightFalloff = Mathf.Max(0.1f, EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Max Add Height Falloff"), MaxAddHeightFalloff));
				}
				else
				{
					AddHeightFalloff = Mathf.Max(0.1f, EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Add Height Falloff"), AddHeightFalloff));

                    rect.y += EditorGUIUtility.singleLineHeight;
				}
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
            
            if(HeightFalloffType != FalloffType.None)
			{			
				height += EditorGUIUtility.singleLineHeight;
                
                if(HeightFalloffMinMax)
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
