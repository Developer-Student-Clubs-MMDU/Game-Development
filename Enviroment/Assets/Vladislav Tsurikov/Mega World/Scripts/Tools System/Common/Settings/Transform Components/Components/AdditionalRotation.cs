using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    [TransformComponent("Additional Rotation")]    
    public class AdditionalRotation : TransformComponent
    {
        public Vector3 Rotation;

        public override void SetInstanceData(ref InstanceData instanceData, float fitness, Vector3 normal)
        {
            instanceData.Rotation *= Quaternion.Euler(Rotation);
        }

#if UNITY_EDITOR
        public override void DoGUI(Rect rect, int index) 
        {
            Rotation = EditorGUI.Vector3Field(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Additional Rotation"), Rotation);
            rect.y += EditorGUIUtility.singleLineHeight;
            rect.y += EditorGUIUtility.singleLineHeight;
        }

        public override float GetElementHeight(int index) 
        {
            float height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight * 2;

            return height;
        }
#endif
    }
}