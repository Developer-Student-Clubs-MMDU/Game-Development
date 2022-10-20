using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    [TransformComponent("Slope Scale")]  
    public class SlopeScale : TransformComponent
    {
        public bool UniformScaleOffset = true;
        [Range(0.1f, 90f)]
        public float MaxSlope = 30;
        [Min(0.1f)]
        public float MaxUniformScaleOffset = 2;
        public Vector3 MaxScaleOffset = new Vector3(2, 2, 0.5f);

        public override void SetInstanceData(ref InstanceData instanceData, float fitness, Vector3 normal)
        {
            float normalAngle = Vector3.Angle(normal, Vector3.up);
            float difference = normalAngle / MaxSlope;

            if(UniformScaleOffset)
            {
                float value = Mathf.Lerp(0, MaxUniformScaleOffset, difference);

                instanceData.Scale += new Vector3(value, value, value);
            }
            else
            {
                float valueX = Mathf.Lerp(0, MaxScaleOffset.x, difference);
                float valueY = Mathf.Lerp(0, MaxScaleOffset.y, difference);
                float valueZ = Mathf.Lerp(0, MaxScaleOffset.z, difference);

                instanceData.Scale += new Vector3(valueX, valueY, valueZ);
            }
        }

#if UNITY_EDITOR
        public override void DoGUI(Rect rect, int index) 
        {
            UniformScaleOffset = EditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Uniform Scale Offset"), UniformScaleOffset);
            rect.y += EditorGUIUtility.singleLineHeight;

            MaxSlope = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Max Slope"), MaxSlope);
            rect.y += EditorGUIUtility.singleLineHeight;

            if(UniformScaleOffset)
            {
                MaxUniformScaleOffset = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Max Uniform Scale Offset"), MaxUniformScaleOffset);
                rect.y += EditorGUIUtility.singleLineHeight;
            }
            else
            {
                MaxScaleOffset = EditorGUI.Vector3Field(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Max Scale Offset"), MaxScaleOffset);
                rect.y += EditorGUIUtility.singleLineHeight;
                rect.y += EditorGUIUtility.singleLineHeight;
            }
        }

        public override float GetElementHeight(int index) 
        {
            float height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;

            if(UniformScaleOffset)
            {
                height += EditorGUIUtility.singleLineHeight;
            }
            else
            {

                height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;
            }

            return height;
        }
#endif
    }
}

