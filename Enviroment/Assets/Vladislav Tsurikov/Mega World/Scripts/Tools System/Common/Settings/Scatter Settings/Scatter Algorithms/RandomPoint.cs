using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    [Scatter("Random Point")]  
    public class RandomPoint : Scatter
    {
        public int MinChecks = 15;
		public int MaxChecks = 15;

        public override void Samples(AreaVariables areaVariables, List<Vector2> samples)
        {
            long numberOfChecks = UnityEngine.Random.Range((int)MinChecks, (int)MaxChecks);

            for (int checks = 0; checks < numberOfChecks; checks++)
            {
                samples.Add(GetRandomPoint(areaVariables));
            }
        }

        public Vector2 GetRandomPoint(AreaVariables areaVariables)
        {
            Vector2 spawnOffset = new Vector3(UnityEngine.Random.Range(-areaVariables.Radius, areaVariables.Radius), UnityEngine.Random.Range(-areaVariables.Radius, areaVariables.Radius));
            return new Vector2(spawnOffset.x + areaVariables.RayHit.Point.x, spawnOffset.y + areaVariables.RayHit.Point.z);
        }

        #if UNITY_EDITOR
        public override void DoGUI(Rect rect, int index) 
        {
            GUIStyle alignmentStyleRight = new GUIStyle(GUI.skin.label);
            alignmentStyleRight.alignment = TextAnchor.MiddleRight;
            alignmentStyleRight.stretchWidth = true;
            GUIStyle alignmentStyleLeft = new GUIStyle(GUI.skin.label);
            alignmentStyleLeft.alignment = TextAnchor.MiddleLeft;
            alignmentStyleLeft.stretchWidth = true;

            float minimumTmp = MinChecks;
            float maximumTmp = MaxChecks;

			EditorGUI.MinMaxSlider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Checks"), ref minimumTmp, ref maximumTmp, 1, MegaWorldPath.AdvancedSettings.EditorSettings.maxChecks);

			MinChecks = (int)minimumTmp;
            MaxChecks = (int)maximumTmp;

            rect.y += EditorGUIUtility.singleLineHeight;

            EditorGUI.LabelField(new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), new GUIContent(""));
            Rect numFieldRect = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, (rect.width - EditorGUIUtility.labelWidth) * 0.2f, EditorGUIUtility.singleLineHeight);
            GUIContent minContent = new GUIContent("");

            EditorGUI.LabelField(numFieldRect, minContent, alignmentStyleLeft);
            numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);

            MinChecks = EditorGUI.IntField(numFieldRect, MinChecks);
            numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
            
            EditorGUI.LabelField(numFieldRect, " ");
            numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);

            MaxChecks = EditorGUI.IntField(numFieldRect, MaxChecks);
            numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);

            GUIContent maxContent = new GUIContent("");
            EditorGUI.LabelField(numFieldRect, maxContent, alignmentStyleRight);

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