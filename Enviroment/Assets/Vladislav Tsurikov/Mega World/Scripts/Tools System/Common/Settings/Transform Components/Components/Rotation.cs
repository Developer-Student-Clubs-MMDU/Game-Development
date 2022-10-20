using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    [TransformComponent("Rotation", true)]  
    public class Rotation : TransformComponent
    {
        public float RandomizeOrientationX = 5;
        public float RandomizeOrientationY = 100;
        public float RandomizeOrientationZ = 5;

        public override void SetInstanceData(ref InstanceData instanceData, float fitness, Vector3 normal)
        {
            Vector3 randomVector = UnityEngine.Random.insideUnitSphere * 0.5f;
            Quaternion randomRotation = Quaternion.Euler(new Vector3(
                RandomizeOrientationX * 3.6f * randomVector.x,
                RandomizeOrientationY * 3.6f * randomVector.y,
                RandomizeOrientationZ * 3.6f * randomVector.z));

            instanceData.Rotation = randomRotation;
        }

#if UNITY_EDITOR
        public override void DoGUI(Rect rect, int index) 
        {
            RandomizeOrientationX = EditorGUI.Slider (new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Randomize X (%)"), RandomizeOrientationX, 0.0f, 100.0f);
            rect.y += EditorGUIUtility.singleLineHeight;
            RandomizeOrientationY = EditorGUI.Slider (new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Randomize Y (%)"), RandomizeOrientationY, 0.0f, 100.0f);
            rect.y += EditorGUIUtility.singleLineHeight;
            RandomizeOrientationZ = EditorGUI.Slider (new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Randomize Z (%)"), RandomizeOrientationZ, 0.0f, 100.0f);
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
