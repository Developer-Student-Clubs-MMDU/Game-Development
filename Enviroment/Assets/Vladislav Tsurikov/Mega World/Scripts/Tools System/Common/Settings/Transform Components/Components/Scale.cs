using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    [TransformComponent("Scale", true)]  
    public class Scale : TransformComponent
    {
        public bool UniformScale = true;
        public Vector3 MinScale = new Vector3(0.8f, 0.8f, 0.8f);
        public Vector3 MaxScale = new Vector3(1.2f, 1.2f, 1.2f);

        public override void SetInstanceData(ref InstanceData instanceData, float fitness, Vector3 normal)
        {
            if (UniformScale)
            {
                float resScale = UnityEngine.Random.Range(MinScale.x, MaxScale.x);
                instanceData.Scale = new Vector3(resScale, resScale, resScale);
            }
            else
            {
                instanceData.Scale = new Vector3(
                    UnityEngine.Random.Range(MinScale.x, MaxScale.x),
                    UnityEngine.Random.Range(MinScale.y, MaxScale.y),
                    UnityEngine.Random.Range(MinScale.z, MaxScale.z));
            }
        }

#if UNITY_EDITOR
        public override void DoGUI(Rect rect, int index) 
        {
            UniformScale = EditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Uniform Scale"), UniformScale);
            rect.y += EditorGUIUtility.singleLineHeight;

            if(UniformScale)
			{
				float minSameScaleValue = MinScale.x;
				float maxSameScaleValue = MaxScale.x;

                GUIStyle alignmentStyleRight = new GUIStyle(GUI.skin.label);
                alignmentStyleRight.alignment = TextAnchor.MiddleRight;
                alignmentStyleRight.stretchWidth = true;
                GUIStyle alignmentStyleLeft = new GUIStyle(GUI.skin.label);
                alignmentStyleLeft.alignment = TextAnchor.MiddleLeft;
                alignmentStyleLeft.stretchWidth = true;
                GUIStyle alignmentStyleCenter = new GUIStyle(GUI.skin.label);
                alignmentStyleCenter.alignment = TextAnchor.MiddleCenter;
                alignmentStyleCenter.stretchWidth = true;
    
                EditorGUI.MinMaxSlider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Scale"), ref minSameScaleValue, ref maxSameScaleValue, 0f, 5f);
                rect.y += EditorGUIUtility.singleLineHeight * 0.5f;
                Rect slopeLabelRect = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, (rect.width - EditorGUIUtility.labelWidth) * 0.2f, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(slopeLabelRect, "0", alignmentStyleLeft);
                slopeLabelRect = new Rect(rect.x + EditorGUIUtility.labelWidth + (rect.width - EditorGUIUtility.labelWidth) * 0.2f, rect.y, (rect.width - EditorGUIUtility.labelWidth) * 0.6f, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(slopeLabelRect, "2.5", alignmentStyleCenter);
                slopeLabelRect = new Rect(rect.x + EditorGUIUtility.labelWidth + (rect.width - EditorGUIUtility.labelWidth) * 0.8f, rect.y, (rect.width - EditorGUIUtility.labelWidth) * 0.2f, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(slopeLabelRect, "5", alignmentStyleRight);
                rect.y += EditorGUIUtility.singleLineHeight;
    
                //Label
                EditorGUI.LabelField(new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), new GUIContent(""));
                //Min Label
                Rect numFieldRect = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, (rect.width - EditorGUIUtility.labelWidth) * 0.2f, EditorGUIUtility.singleLineHeight);
                GUIContent minContent = new GUIContent("");
    
                EditorGUI.LabelField(numFieldRect, minContent, alignmentStyleLeft);
                numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
    
                minSameScaleValue = EditorGUI.FloatField(numFieldRect, minSameScaleValue);
                numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
                
                EditorGUI.LabelField(numFieldRect, " ");
                numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
    
                maxSameScaleValue = EditorGUI.FloatField(numFieldRect, maxSameScaleValue);
                numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
    
                GUIContent maxContent = new GUIContent("");
                EditorGUI.LabelField(numFieldRect, maxContent, alignmentStyleRight);
    
                rect.y += EditorGUIUtility.singleLineHeight;

				MinScale = new Vector3(minSameScaleValue, minSameScaleValue, minSameScaleValue);
				MaxScale = new Vector3(maxSameScaleValue, maxSameScaleValue, maxSameScaleValue);
			}
			else
			{
                MinScale = EditorGUI.Vector3Field(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Min Scale"), MinScale);
                rect.y += EditorGUIUtility.singleLineHeight;
                rect.y += EditorGUIUtility.singleLineHeight;
				MaxScale = EditorGUI.Vector3Field(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Max Scale"), MaxScale);
                rect.y += EditorGUIUtility.singleLineHeight;
                rect.y += EditorGUIUtility.singleLineHeight;
			}
        }

        public override float GetElementHeight(int index) 
        {
            float height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;

            if(UniformScale)
			{
                height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;                
			}
			else
			{
                height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;
			}

            return height;
        }
#endif
    }
}

