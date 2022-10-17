using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    [TransformComponent("Align", true)]    
    public class Align : TransformComponent
    {
        public bool UseNormalWeight = true;
        public bool MinMaxRange = true;
		public float MinWeightToNormal = 0;
		public float MaxWeightToNormal = 0.2f;

        public override void SetInstanceData(ref InstanceData instanceData, float fitness, Vector3 normal)
        {
            Quaternion normalRotation = Quaternion.FromToRotation(Vector3.up, normal);

            if(UseNormalWeight == true)
            {
                float normalWeight;

                if(MinMaxRange == true)
                {
                    normalWeight = UnityEngine.Random.Range(MinWeightToNormal, MaxWeightToNormal);
                }
                else
                {
                    normalWeight = MaxWeightToNormal;
                }

                instanceData.Rotation = Quaternion.Lerp(instanceData.Rotation, normalRotation, normalWeight) * instanceData.Rotation;
            }
            else
            {
                instanceData.Rotation = normalRotation * instanceData.Rotation;
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

            UseNormalWeight = EditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Use Normal Weight"), UseNormalWeight);
            rect.y += EditorGUIUtility.singleLineHeight;
            
            if(UseNormalWeight == true)
            {
                MinMaxRange = EditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Min Max Range"), MinMaxRange);
                rect.y += EditorGUIUtility.singleLineHeight;

                if(MinMaxRange == true)
                {
                    EditorGUI.MinMaxSlider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Weight To Normal"), ref MinWeightToNormal, ref MaxWeightToNormal, 0, 1);
                    rect.y += EditorGUIUtility.singleLineHeight;

                    EditorGUI.LabelField(new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), new GUIContent(""));
                    //Min Label
                    Rect numFieldRect = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, (rect.width - EditorGUIUtility.labelWidth) * 0.2f, EditorGUIUtility.singleLineHeight);
                    GUIContent minContent = new GUIContent("");

                    EditorGUI.LabelField(numFieldRect, minContent, alignmentStyleLeft);
                    numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);

                    MinWeightToNormal = EditorGUI.FloatField(numFieldRect, MinWeightToNormal);
                    numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);

                    EditorGUI.LabelField(numFieldRect, " ");
                    numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);

                    MaxWeightToNormal = EditorGUI.FloatField(numFieldRect, MaxWeightToNormal);
                    numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);

                    GUIContent maxContent = new GUIContent("");
                    EditorGUI.LabelField(numFieldRect, maxContent, alignmentStyleRight);
                }
                else
                {
                    MaxWeightToNormal = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Weight To Normal"), MaxWeightToNormal, 0, 1);
                    rect.y += EditorGUIUtility.singleLineHeight;
                }
            }
        }

        public override float GetElementHeight(int index) 
        {
            float height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;
            
            if(UseNormalWeight == true)
            {
                height += EditorGUIUtility.singleLineHeight;

                if(MinMaxRange == true)
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

