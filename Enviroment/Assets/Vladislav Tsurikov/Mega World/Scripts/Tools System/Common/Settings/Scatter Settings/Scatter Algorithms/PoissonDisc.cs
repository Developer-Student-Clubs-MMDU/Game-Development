using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using UnityEditor;
using VladislavTsurikov.CustomGUI;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    [Scatter("Poisson Disc")]  
    public class PoissonDisc : Scatter
    {
        /// Helper struct to calculate the x and y indices of a sample in the grid
        private struct GridPos
        {
            public int x;
            public int y;

            public GridPos(Vector2 sample, float cellSize)
            {
                x = (int)(sample.x / cellSize);
                y = (int)(sample.y / cellSize);
            }
        }

        public float PoissonDiscSize = 4;

        private const int k = 30;  // Maximum number of attempts before marking a sample as inactive.

        private Rect rect;
        private float radius2;  // radius squared
        private float cellSize;
        private Vector2[,] grid;
        private List<Vector2> activeSamples = new List<Vector2>();

        /// Create a sampler with the following parameters:
        ///
        /// width:  each sample's x coordinate will be between [0, width]
        /// height: each sample's y coordinate will be between [0, height]
        /// radius: each sample will be at least `radius` units away from any other sample, and at most 2 * `radius`.

        public override void Samples(AreaVariables areaVariables, List<Vector2> samples)
        {
            Init(areaVariables.Bounds.size.z, areaVariables.Bounds.size.x, PoissonDiscSize / 2);

            AddSample(areaVariables, samples, new Vector2(UnityEngine.Random.value * rect.width, UnityEngine.Random.value * rect.height));

            while (activeSamples.Count > 0)
            {
                // Pick a random active sample
                int i = (int)UnityEngine.Random.value * activeSamples.Count;
                Vector2 sample = activeSamples[i];

                // Try `k` random candidates between [radius, 2 * radius] from that sample.
                bool found = false;
                for (int j = 0; j < k; ++j)
                {
                    float angle = 2 * Mathf.PI * UnityEngine.Random.value;
                    float r = Mathf.Sqrt(UnityEngine.Random.value * 3 * radius2 + radius2); // See: http://stackoverflow.com/questions/9048095/create-random-number-within-an-annulus/9048443#9048443
                    Vector2 candidate = sample + r * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                    // Accept candidates if it's inside the rect and farther than 2 * radius to any existing sample.
                    if (rect.Contains(candidate) && IsFarEnough(candidate))
                    {
                        found = true;
                        AddSample(areaVariables, samples, candidate);
                        break;
                    }
                }

                // If we couldn't find a valid candidate after k attempts, remove this sample from the active samples queue
                if (!found)
                {
                    activeSamples[i] = activeSamples[activeSamples.Count - 1];
                    activeSamples.RemoveAt(activeSamples.Count - 1);
                }
            }

            foreach (Vector3 sample in activeSamples)
            {
                float x = areaVariables.RayHit.Point.x + sample.x - areaVariables.Radius;
                float z = areaVariables.RayHit.Point.z + sample.y - areaVariables.Radius;

                Debug.Log(new Vector2(x, z));
                samples.Add(new Vector2(x, z));
            }
        }

        public void Init(float width, float height, float radius)
        {
            rect = new Rect(0, 0, width, height);
            radius2 = radius * radius;
            cellSize = radius / Mathf.Sqrt(2);
            grid = new Vector2[Mathf.CeilToInt(width / cellSize),
                               Mathf.CeilToInt(height / cellSize)];
        }

        private bool IsFarEnough(Vector2 sample)
        {
            GridPos pos = new GridPos(sample, cellSize);

            int xmin = Mathf.Max(pos.x - 2, 0);
            int ymin = Mathf.Max(pos.y - 2, 0);
            int xmax = Mathf.Min(pos.x + 2, grid.GetLength(0) - 1);
            int ymax = Mathf.Min(pos.y + 2, grid.GetLength(1) - 1);

            for (int y = ymin; y <= ymax; y++)
            {
                for (int x = xmin; x <= xmax; x++)
                {
                    Vector2 s = grid[x, y];
                    if (s != Vector2.zero)
                    {
                        Vector2 d = s - sample;
                        if (d.x * d.x + d.y * d.y < radius2) return false;
                    }
                }
            }

            return true;

            // Note: we use the zero vector to denote an unfilled cell in the grid. This means that if we were
            // to randomly pick (0, 0) as a sample, it would be ignored for the purposes of proximity-testing
            // and we might end up with another sample too close from (0, 0). This is a very minor issue.
        }

        /// Adds the sample to the active samples queue and the grid before returning it
        private Vector2 AddSample(AreaVariables areaVariables, List<Vector2> samples, Vector2 sample)
        {
            float x = areaVariables.RayHit.Point.x + sample.x - areaVariables.Radius;
            float z = areaVariables.RayHit.Point.z + sample.y - areaVariables.Radius;

            samples.Add(new Vector2(x, z));

            activeSamples.Add(sample);
            GridPos pos = new GridPos(sample, cellSize);
            grid[pos.x, pos.y] = sample;
            return sample;
        }

        #if UNITY_EDITOR
        public override void DoGUI(Rect rect, int index) 
        {
            PoissonDiscSize = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Poisson Disc Size"), PoissonDiscSize);
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