using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace VladislavTsurikov.MegaWorldSystem
{
    public static class NoiseUtility
    {        
        public static void NormalizeArray(float[,] arr, int width, int height, ref float rangeMin, ref float rangeMax)
        {
            float min = float.PositiveInfinity;
            float max = float.NegativeInfinity;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float v = arr[x, y];
                    if (v < min)
                    {
                        min = v;
                    }
                    if (v > max) 
                    {
                        max = v;
                    }
                }
            }

            rangeMin = min;
            rangeMax = max;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float v = arr[x, y];
                    arr[x, y] = (v - min) / (max - min);
                }
            }
        }
    } 
}