using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    [TransformComponent("Snap Rotation")]  
    public class SnapRotation : TransformComponent
    {
        [Range(0.1f, 360f)]
        public float SnapRotationAngle = 90f;
        public bool RotateAxisX = false;
        public bool RotateAxisY = true;
        public bool RotateAxisZ = false;

        public override void SetInstanceData(ref InstanceData instanceData, float fitness, Vector3 normal)
        {
            List<float> rotationValueList = new List<float>();
            int count = 0;
            for (float value = 0; value <= 360f; value += SnapRotationAngle)
            {
                rotationValueList.Add(value);
                count++;
            }

            if(count != 0)
            {
                float randomX = 0;
                float randomY = 0;
                float randomZ = 0;

                if(RotateAxisX)
                {
                    randomX = rotationValueList[UnityEngine.Random.Range(0, rotationValueList.Count)];
                }
                if(RotateAxisY)
                {
                    randomY = rotationValueList[UnityEngine.Random.Range(0, rotationValueList.Count)];
                }
                if(RotateAxisZ)
                {
                    randomZ = rotationValueList[UnityEngine.Random.Range(0, rotationValueList.Count)];
                }

                Quaternion rotationX = Quaternion.AngleAxis(randomX, Vector3.right);
                Quaternion rotationY = Quaternion.AngleAxis(randomY, Vector3.up);
                Quaternion rotationZ = Quaternion.AngleAxis(randomZ, Vector3.forward);

                instanceData.Rotation = rotationX * rotationY * rotationZ;
            }
        }

#if UNITY_EDITOR
        public override void DoGUI(Rect rect, int index) 
        {
            RotateAxisX = EditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Rotate Axis X"), RotateAxisX);
            rect.y += EditorGUIUtility.singleLineHeight;
            RotateAxisY = EditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Rotate Axis Y"), RotateAxisY);
            rect.y += EditorGUIUtility.singleLineHeight;
            RotateAxisZ = EditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Rotate Axis Z"), RotateAxisZ);
            rect.y += EditorGUIUtility.singleLineHeight;
            SnapRotationAngle = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Snap Rotation Angle"), SnapRotationAngle);
            rect.y += EditorGUIUtility.singleLineHeight;
        }

        public override float GetElementHeight(int index) 
        {
            float height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;

            return height;
        }
#endif
    }
}
