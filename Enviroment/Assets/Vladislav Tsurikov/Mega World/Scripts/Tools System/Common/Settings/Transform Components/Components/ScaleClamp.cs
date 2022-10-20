using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    [TransformComponent("Scale Clamp")]  
    public class ScaleClamp : TransformComponent
    {
        public float MaxScale = 2;
        public float MinScale = 0.5f;

        public override void SetInstanceData(ref InstanceData instanceData, float fitness, Vector3 normal)
        {
            if(instanceData.Scale.x > MaxScale)
            {
                instanceData.Scale.x = MaxScale;
            }
            else if(instanceData.Scale.x < MinScale)
            {
                instanceData.Scale.x = MinScale;
            }

            if(instanceData.Scale.y > MaxScale)
            {
                instanceData.Scale.y = MaxScale;
            }
            else if(instanceData.Scale.y < MinScale)
            {
                instanceData.Scale.y = MinScale;
            }

            if(instanceData.Scale.z > MaxScale)
            {
                instanceData.Scale.z = MaxScale;
            }
            else if(instanceData.Scale.z < MinScale)
            {
                instanceData.Scale.z = MinScale;
            }
        }

#if UNITY_EDITOR
        public override void DoGUI(Rect rect, int index) 
        {
            GUIStyle alignmentStyleRight = new GUIStyle(GUI.skin.label);
            alignmentStyleRight.alignment = TextAnchor.MiddleRight;
            alignmentStyleRight.stretchWidth = true;
            GUIStyle alignmentStyleLeft = new GUIStyle(GUI.skin.label);
            alignmentStyleLeft.alignment = TextAnchor.MiddleLeft;
            alignmentStyleLeft.stretchWidth = true;
            GUIStyle alignmentStyleCenter = new GUIStyle(GUI.skin.label);
            alignmentStyleCenter.alignment = TextAnchor.MiddleCenter;
            alignmentStyleCenter.stretchWidth = true;
    
            EditorGUI.MinMaxSlider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Scale Clamp"), ref MinScale, ref MaxScale, 0f, 5f);
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
    
            MinScale = EditorGUI.FloatField(numFieldRect, MinScale);
            numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
            
            EditorGUI.LabelField(numFieldRect, " ");
            numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
    
            MaxScale = EditorGUI.FloatField(numFieldRect, MaxScale);
            numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
    
            GUIContent maxContent = new GUIContent("");
            EditorGUI.LabelField(numFieldRect, maxContent, alignmentStyleRight);
        }

        public override float GetElementHeight(int index) 
        {   
            float height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;

            return height;
        }
#endif
    }
}