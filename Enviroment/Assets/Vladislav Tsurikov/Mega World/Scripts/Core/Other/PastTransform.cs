using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    public class PastTransform
    {
        private Vector3 position;
        private Vector3 scale = Vector3.one;
        private Quaternion rotation;

        public Vector3 Position
        {
            get
            {
                return position;
            }
        }

        public Vector3 Scale
        {
            get
            {
                return scale;
            }
        }

        public Quaternion Rotation
        {
            get
            {
                return rotation;
            }
        }

        public PastTransform(Transform transform)
        {
            position = transform.position;
            scale = transform.lossyScale;
            rotation = transform.rotation;
        }
    }
}
    