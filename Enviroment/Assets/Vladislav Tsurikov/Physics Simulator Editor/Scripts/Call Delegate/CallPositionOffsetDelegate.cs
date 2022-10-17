#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using VladislavTsurikov.PhysicsSimulatorEditor;

namespace VladislavTsurikov.MegaWorldSystem
{
    [InitializeOnLoad]
    public static class CallPositionOffsetDelegate 
    {
        static CallPositionOffsetDelegate()
        {
            SimulatedBodyStack.DisablePhysicsSupportPerformed -= Performed;
            SimulatedBodyStack.DisablePhysicsSupportPerformed += Performed;
        }

        private static void Performed(GameObject go)
        {
            if(go == null) return;

            PhysicsSimulatorPath.Settings.PositionOffsetSettings.ApplyOffset(go);
        }
    }
}
#endif