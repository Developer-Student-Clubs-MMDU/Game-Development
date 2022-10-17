using UnityEngine;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class MouseSensitivitySettings 
    {
        [SerializeField]
        private float _mouseSensitivity = 0.5f;

        public static float MinMouseSensitivity { get { return 0.01f; } }
        public static float MaxMouseSensitivity { get { return 1.0f; } }

        public float MouseSensitivity { get { return _mouseSensitivity; } set { _mouseSensitivity = Mathf.Clamp(value, MinMouseSensitivity, MaxMouseSensitivity); } }
    }
}