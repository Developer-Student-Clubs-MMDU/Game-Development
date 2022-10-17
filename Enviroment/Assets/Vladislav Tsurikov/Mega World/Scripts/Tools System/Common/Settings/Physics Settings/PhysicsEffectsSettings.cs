#if UNITY_EDITOR
using UnityEngine;
using System;
using VladislavTsurikov.PhysicsSimulatorEditor;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class PhysicsEffectsSettings
    {
        #region Force
        public bool ForceRange = true;
        public float MinForce = 10f;
        public float MaxForce = 40f;
        #endregion

        #region Direction
        public float RandomStrength = 50f;
        #endregion

        public PhysicsEffectsSettingsEditor settingsEditor = new PhysicsEffectsSettingsEditor();

        public void OnGUI()
        {
            settingsEditor.OnGUI(this);
        }
        
        public void ApplyForce(Rigidbody rigidbody) 
        {
            if (rigidbody == null) return;

            float radians = UnityEngine.Random.Range(0, 360) * Mathf.Deg2Rad;

            Vector3 forceDirection = new Vector3(Mathf.Sin(radians), 0, Mathf.Cos(radians));

            Vector3 force = Vector3.Lerp(new Vector3(0, -1, 0), forceDirection, RandomStrength / 100);

            float magnitude = ForceRange ? UnityEngine.Random.Range(MinForce, MaxForce) : MinForce;

            force *= magnitude;

            PhysicsUtility.ApplyForce(rigidbody, force);
        }
    }
}
#endif