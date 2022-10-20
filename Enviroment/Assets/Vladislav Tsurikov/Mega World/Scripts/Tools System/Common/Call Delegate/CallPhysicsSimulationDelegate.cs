#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using VladislavTsurikov.PhysicsSimulatorEditor;
using VladislavTsurikov.RaycastEditorSystem;

namespace VladislavTsurikov.MegaWorldSystem
{
    [InitializeOnLoad]
    public static class СallPhysicsSimulationDelegate 
    {
        static СallPhysicsSimulationDelegate()
        {
            SimulatedBodyStack.DisablePhysicsSupportPerformed -= Performed;
            SimulatedBodyStack.DisablePhysicsSupportPerformed += Performed;
        }

        private static void Performed(GameObject go)
        {
            if(go == null) return;

            RaycastEditor.RegisterGameObject(go);
            PrototypeGameObject proto = GetPrototype.GetCurrentPrototypeGameObject(go);

            if(proto != null)
            {
                MegaWorldPath.GameObjectStoragePackage.Storage.AddInstance(proto.ID, go);
            }
        }
    }
}
#endif