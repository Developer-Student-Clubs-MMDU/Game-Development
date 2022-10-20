using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    [TransformComponent("Position Offset", true)]  
    public class PositionOffset : TransformComponent
    {
        public float MinPositionOffsetY = -0.15f;
        public float MaxPositionOffsetY = 0f;

        public override void SetInstanceData(ref InstanceData instanceData, float fitness, Vector3 normal)
        {
            instanceData.Position += new Vector3(0, UnityEngine.Random.Range(MinPositionOffsetY, MaxPositionOffsetY), 0);
        }

#if UNITY_EDITOR
        public override void DoGUI(Rect rect, int index) 
        {
            MinPositionOffsetY = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Min Position Offset Y"), MinPositionOffsetY);
            rect.y += EditorGUIUtility.singleLineHeight;
            MaxPositionOffsetY = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Max Position Offset Y"), MaxPositionOffsetY);
            rect.y += EditorGUIUtility.singleLineHeight;
        }

        public override float GetElementHeight(int index) 
        {
            float height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;

            return height;
        }
#endif
    }
}

