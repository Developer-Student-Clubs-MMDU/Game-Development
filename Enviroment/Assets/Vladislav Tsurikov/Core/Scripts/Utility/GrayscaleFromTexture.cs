using System.Collections.Generic;
using UnityEngine;
using System;

namespace VladislavTsurikov
{
    public static class GrayscaleFromTexture 
    {
        public static float GetFromWorldPosition(Bounds bounds, Vector3 worldPosition, Texture2D texture)
        {
            if(texture == null)
            {
                return 0;
            }

            float inverseY = Mathf.InverseLerp(bounds.center.z - bounds.extents.z, bounds.center.z + bounds.extents.z, worldPosition.z);
            float inverseX = Mathf.InverseLerp(bounds.center.x - bounds.extents.x, bounds.center.x + bounds.extents.x, worldPosition.x);

            int pixelY = Mathf.RoundToInt(Mathf.Lerp(0, texture.width, inverseY));
            int pixelX = Mathf.RoundToInt(Mathf.Lerp(0, texture.height, inverseX));

            return texture.GetPixel(pixelX, pixelY).grayscale;
        }

        public static float Get(Vector2 normal, Texture2D texture)
        {
            if(texture == null)
            {
                return 0;
            }

            int pixelY = Mathf.RoundToInt(Mathf.Lerp(0, texture.width, normal.y));
            int pixelX = Mathf.RoundToInt(Mathf.Lerp(0, texture.height, normal.x));

            return texture.GetPixel(pixelX, pixelY).grayscale;
        }
    }
}