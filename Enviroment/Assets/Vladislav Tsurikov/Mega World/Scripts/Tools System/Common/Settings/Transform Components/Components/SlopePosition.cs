using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    [TransformComponent("Slope Position")]  
    public class SlopePosition : TransformComponent
    {
        public float MaxSlope = 90;
        public float PositionOffsetY = -1;

        public override void SetInstanceData(ref InstanceData instanceData, float fitness, Vector3 normal)
        {
            float normalAngle = Vector3.Angle(normal, Vector3.up);
            float difference = normalAngle / MaxSlope;
            
            float positionY = Mathf.Lerp(0, PositionOffsetY, difference);

            instanceData.Position += new Vector3(0, positionY, 0);
        }

#if UNITY_EDITOR
        public override void DoGUI(Rect rect, int index) 
        {
            MaxSlope = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Max Slope"), MaxSlope, 0, 90);
            rect.y += EditorGUIUtility.singleLineHeight;
            PositionOffsetY = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Position Offset Y"), PositionOffsetY);
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
