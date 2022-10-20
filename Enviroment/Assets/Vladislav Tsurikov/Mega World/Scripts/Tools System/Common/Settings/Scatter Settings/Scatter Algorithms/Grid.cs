using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using UnityEditor;
using VladislavTsurikov.CustomGUI;

namespace VladislavTsurikov.MegaWorldSystem
{
    public enum RandomisationType 
    { 
        None, 
        Square,
        Sphere,
    }
    
    [Serializable]
    [Scatter("Grid")]  
    public class Grid : Scatter
    {
        public Vector3 VisualOrigin;
        public RandomisationType RandomisationType = RandomisationType.Square;
        [Range (0, 1)]
        public float Vastness = 1;
        public bool UniformGrid = true;
        public Vector2 GridStep = new Vector2(3, 3);
        public float GridAngle = 0;

        public override void Samples(AreaVariables areaVariables, List<Vector2> samples)
        {
            UpdateGrid(areaVariables.RayHit.Point, areaVariables.Size);

            Vector3 gridOrigin = Vector3.zero;

            Vector3 position = Vector3.zero;
            float halfSpawnRange = areaVariables.Radius;

            Vector3 maxPosition = gridOrigin + (Vector3.one * (halfSpawnRange * 2));

            for (position.x = gridOrigin.x; position.x < maxPosition.x; position.x += GridStep.x)
            {
                for (position.z = gridOrigin.z; position.z < maxPosition.y; position.z += GridStep.y)
                {
                    Vector3 newLocalPosition = CommonUtility.RotatePointAroundPivot(position, new Vector3(maxPosition.x / 2, 0, maxPosition.z / 2), Quaternion.AngleAxis(GridAngle, Vector3.up));
                    Vector2 offsetLocalPosition = new Vector2(VisualOrigin.x + newLocalPosition.x, VisualOrigin.z + newLocalPosition.z);
                    offsetLocalPosition = GetCurrentRandomPosition(offsetLocalPosition);
                    samples.Add(offsetLocalPosition);
                }
            }
        }

        public void UpdateGrid(Vector3 dragPoint, float size)
        {
            Vector3 gridOrigin = Vector3.zero;
            Vector3 localGridStep = new Vector3(GridStep.x, GridStep.y, 1);
            Vector3 gridNormal = Vector3.up;

            float halfSpawnRange = size / 2;  
            
            Vector3 point = new Vector3(dragPoint.x - halfSpawnRange, 0, dragPoint.z - halfSpawnRange);

            Matrix4x4 gridMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(GridAngle, gridNormal) * Quaternion.LookRotation(gridNormal), Vector3.one)
                                 * Matrix4x4.TRS(gridOrigin, Quaternion.identity, localGridStep);

            Vector3 gridSpacePoint = gridMatrix.inverse.MultiplyPoint(point);

            gridSpacePoint = new Vector3(Mathf.Round(gridSpacePoint.x), Mathf.Round(gridSpacePoint.y), gridSpacePoint.z);

            Vector3 snappedHitPoint = gridMatrix.MultiplyPoint(gridSpacePoint);
            VisualOrigin = snappedHitPoint;
        }

        public Vector2 GetRandomSquarePoint(Vector2 sample)
        {
            float halfDistanceX = GridStep.x / 2;
            float halfDistanceY = GridStep.y / 2;
            Vector2 distanceOffset = new Vector2(UnityEngine.Random.Range(-halfDistanceX, halfDistanceX), UnityEngine.Random.Range(-halfDistanceY, halfDistanceY));
            return sample + distanceOffset;
        }

        public Vector2 GetRandomSpherePoint(Vector2 sample)
        {
            float halfDistance = Mathf.Lerp(0, GridStep.x / 2, Vastness);
            Vector2 distanceOffset = new Vector2(UnityEngine.Random.Range(-halfDistance, halfDistance), UnityEngine.Random.Range(-halfDistance, halfDistance));
            return sample + distanceOffset;
        }

        public Vector2 GetCurrentRandomPosition(Vector2 sample)
        {
            switch (RandomisationType)
            {
                case RandomisationType.Square:
                {
                    return GetRandomSquarePoint(sample);
                }
                case RandomisationType.Sphere:
                {
                    return GetRandomSpherePoint(sample);
                }
            }

            return sample;
        }

#if UNITY_EDITOR
        public override void DoGUI(Rect rect, int index) 
        {
            RandomisationType = (RandomisationType)EditorGUI.EnumPopup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Randomisation Type"), RandomisationType);
            rect.y += EditorGUIUtility.singleLineHeight;

			if(RandomisationType == RandomisationType.Sphere)
			{
				Vastness = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Vastness"), Vastness, 0f, 1f);
                rect.y += EditorGUIUtility.singleLineHeight;
			}

			float distance = GridStep.x;

			UniformGrid = EditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Uniform Grid"), UniformGrid);
            rect.y += EditorGUIUtility.singleLineHeight;

			if(UniformGrid)
			{
				distance = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Distance"), distance, 0.1f, 50f);
                rect.y += EditorGUIUtility.singleLineHeight;
				
				GridAngle = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Angle"), GridAngle, 0, 360);
                rect.y += EditorGUIUtility.singleLineHeight;

				GridStep = Vector2.Max(new Vector2(0.5f, 0.5f), new Vector2(distance, distance));
                rect.y += EditorGUIUtility.singleLineHeight;
			}
			else
			{
				GridStep = Vector2.Max(new Vector2(0.1f, 0.1f), EditorGUI.Vector2Field(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Step"), GridStep));
				rect.y += EditorGUIUtility.singleLineHeight;
                rect.y += EditorGUIUtility.singleLineHeight;

				GridAngle = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Angle"), GridAngle, 0, 360);
                rect.y += EditorGUIUtility.singleLineHeight;
			}
        }

        public override float GetElementHeight(int index) 
        {
            float height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;

			if(RandomisationType == RandomisationType.Sphere)
			{
                height += EditorGUIUtility.singleLineHeight;
			}

            height += EditorGUIUtility.singleLineHeight;

			if(UniformGrid)
			{
				height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;
			}
			else
			{
				height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;
			}

            return height;
        }
#endif
    }
}