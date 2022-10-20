using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    [Scatter("Failure Rate")]  
    public class FailureRate : Scatter
    {
        public float Value = 70;

        public override void Samples(AreaVariables areaVariables, List<Vector2> samples)
        {
            List<Vector3> removeSamples = new List<Vector3>();

            foreach (Vector3 sample in samples)
            {
                if(UnityEngine.Random.Range(0f, 100f) < Value)
                {
                    removeSamples.Add(sample);
                }
            }

            foreach (Vector3 sample in removeSamples)
            {
                samples.Remove(sample);
            }
        }

        #if UNITY_EDITOR
        public override void DoGUI(Rect rect, int index) 
        {
            Value = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Value (%)"), Value, 0f, 100f);
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