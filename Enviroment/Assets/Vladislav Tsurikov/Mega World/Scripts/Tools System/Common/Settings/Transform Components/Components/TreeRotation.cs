using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    [TransformComponent("Tree Rotation")]  
    public class TreeRotation : TransformComponent
    {
        public float RandomizeOrientationY = 100;
        public float RandomizeOrientationXZ = 3;
        public float SuccessRandomizeOrientationXZ = 20;

        public override void SetInstanceData(ref InstanceData instanceData, float fitness, Vector3 normal)
        {
            Vector3 randomVector = UnityEngine.Random.insideUnitSphere * 0.5f;

            float randomSuccess = UnityEngine.Random.Range(0.0f, 1.0f);

            if(randomSuccess < SuccessRandomizeOrientationXZ / 100)
            {
                Quaternion randomRotation = Quaternion.Euler(new Vector3(
                RandomizeOrientationXZ * 3.6f * randomVector.x,
                RandomizeOrientationY * 3.6f * randomVector.y,
                RandomizeOrientationXZ * 3.6f * randomVector.z));

                instanceData.Rotation = randomRotation;
            }
            else
            {
                Quaternion randomRotation = Quaternion.Euler(new Vector3(0, RandomizeOrientationY * 3.6f * randomVector.y, 0));

                instanceData.Rotation = randomRotation;
            }
        }

#if UNITY_EDITOR

        public override void DoGUI(Rect rect, int index) 
        {
            RandomizeOrientationY = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Randomize Y (%)"), RandomizeOrientationY, 0.0f, 100.0f);
            rect.y += EditorGUIUtility.singleLineHeight;
            RandomizeOrientationXZ = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Randomize XZ (%)"), RandomizeOrientationXZ, 0.0f, 50.0f);
            rect.y += EditorGUIUtility.singleLineHeight;

            SuccessRandomizeOrientationXZ = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Success Randomize XZ (%)"), SuccessRandomizeOrientationXZ, 0.0f, 100.0f);
            rect.y += EditorGUIUtility.singleLineHeight;
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