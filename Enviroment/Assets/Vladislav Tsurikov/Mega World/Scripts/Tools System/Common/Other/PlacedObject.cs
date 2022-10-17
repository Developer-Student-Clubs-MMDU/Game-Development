using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class PlacedObject
    {
        public GameObject GameObject;
        public Bounds Bounds;

        public void CopyTransform(GameObject gameObject)
        {
            GameObject.transform.position = gameObject.transform.position;
            GameObject.transform.rotation = gameObject.transform.rotation;
            GameObject.transform.localScale = gameObject.transform.localScale;
        }
    }
}
    