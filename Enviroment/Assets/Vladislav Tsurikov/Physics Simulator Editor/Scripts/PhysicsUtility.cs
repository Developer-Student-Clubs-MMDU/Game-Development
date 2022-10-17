#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace VladislavTsurikov.PhysicsSimulatorEditor
{
    [InitializeOnLoad]
    public static class PhysicsUtility
    {
        public static void ApplyForce(Rigidbody rigidbody, Vector3 force) 
        {
            if (rigidbody != null) 
            {
                rigidbody.AddForce(force, ForceMode.Impulse);
            }
        }
    }
}
#endif