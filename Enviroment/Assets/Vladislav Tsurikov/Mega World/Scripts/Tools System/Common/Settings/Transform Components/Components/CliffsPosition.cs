using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    [TransformComponent("Cliffs Position")]  
    public class CliffsPosition : TransformComponent
    {
        public float OffsetPosition = 1;

        public override void SetInstanceData(ref InstanceData instanceData, float fitness, Vector3 normal)
        {
            Quaternion rotation = Quaternion.identity;

            Vector3 direction = new Vector3(normal.x, 0, normal.z);

            instanceData.Position += direction + new Vector3(OffsetPosition, 0, OffsetPosition);
        }

#if UNITY_EDITOR
        public override void DoGUI(Rect rect, int index) 
        {
            OffsetPosition = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Additional Rotation"), OffsetPosition);
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