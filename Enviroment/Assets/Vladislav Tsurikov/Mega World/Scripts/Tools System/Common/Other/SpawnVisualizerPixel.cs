using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.MegaWorldSystem
{
    public class SpawnVisualizerPixel
    {
        public Vector3 position;
        public float fitness;
        public float alpha = 1f;
    
        public SpawnVisualizerPixel(Vector3 position, float fitness, float alpha)
        {
            this.position = position;
            this.fitness = fitness;
            this.alpha = alpha;
        }
    }
}
