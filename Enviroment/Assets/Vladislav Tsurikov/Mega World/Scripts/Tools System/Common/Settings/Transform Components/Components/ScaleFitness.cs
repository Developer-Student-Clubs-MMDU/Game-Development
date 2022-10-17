using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    [TransformComponent("Scale Fitness")]  
    public class ScaleFitness : TransformComponent
    {
        public float OffsetScale = -0.7f;

        public override void SetInstanceData(ref InstanceData instanceData, float fitness, Vector3 normal)
        {
            float value = Mathf.Lerp(OffsetScale, 0, fitness);

            instanceData.Scale += new Vector3(value, value, value);
        }

#if UNITY_EDITOR
        public override void DoGUI(Rect rect, int index) 
        {
            OffsetScale = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Offset Scale"), OffsetScale);
            rect.y += EditorGUIUtility.singleLineHeight;
        }

        public override float GetElementHeight(int index) 
        {   
            float height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;

            return height;
        }
#endif
    }
}