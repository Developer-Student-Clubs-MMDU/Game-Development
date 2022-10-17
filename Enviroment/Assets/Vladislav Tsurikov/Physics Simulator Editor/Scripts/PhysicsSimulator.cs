#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using VladislavTsurikov.EditorCoroutinesSystem;
using System.Collections;

namespace VladislavTsurikov.PhysicsSimulatorEditor
{
    public enum DisablePhysicsMode
    {
        GlobalTime,
        ObjectTime
    }

    [InitializeOnLoad]
    public static class PhysicsSimulator
    {
        private static DisablePhysicsMode s_disablePhysicsTimeMode = DisablePhysicsMode.GlobalTime;
        private static bool s_useAccelerationPhysics = true;
        private static bool s_active = false;
        private static bool s_enablePermanentPhysics = false;

        static PhysicsSimulator() 
        {
            EditorApplication.update -= SimulatePhysics;
            EditorApplication.update += SimulatePhysics;
        }

        public static void Simulate()
        {
            bool prevAutoSimulation = Physics.autoSimulation;

            Physics.autoSimulation = false;

            float accelerationPhysics = s_useAccelerationPhysics ? PhysicsSimulatorPath.Settings.AccelerationPhysics : 1;

            for (int i = 0; i < accelerationPhysics; i++)
            {
                Physics.Simulate(Time.deltaTime);

                SimulatedBodyStack.DisablePhysicsSupportIfObjectStopped();
                if(SimulatedBodyStack.SimulatedBodyList.Count == 0)
                {
                    break;
                }
            }

            Physics.autoSimulation = prevAutoSimulation;
        }
        
        public static void Activate(DisablePhysicsMode disablePhysicsTimeMode, bool useAccelerationPhysics = true, bool enablePermanentPhysics = false)
        {
            if(s_disablePhysicsTimeMode != disablePhysicsTimeMode)
            {
                if(s_disablePhysicsTimeMode == DisablePhysicsMode.ObjectTime)
                {
                    ObjectTimePhysicsSimulator.StopAllCoroutine();
                }
            }

            s_disablePhysicsTimeMode = disablePhysicsTimeMode;
            s_useAccelerationPhysics = useAccelerationPhysics;

            s_active = true;
            s_enablePermanentPhysics = enablePermanentPhysics;
        }

        public static void SimulatePhysics() 
        {
            if(!s_active)
            {
                if(!s_enablePermanentPhysics)
                {
                    return;
                }
            }

            if (PhysicsSimulatorPath.Settings.SimulatePhysics) 
            {
                switch (s_disablePhysicsTimeMode)
                {
                    case DisablePhysicsMode.GlobalTime:
                    {
                        ActiveTimePhysicsSimulator.SimulatePhysics();
                        break;
                    }
                    case DisablePhysicsMode.ObjectTime:
                    {
                        ObjectTimePhysicsSimulator.SimulatePhysics();
                        break;
                    }
                }
            }

            if(SimulatedBodyStack.SimulatedBodyList.Count == 0)
            {
                s_active = false;
            }
        }

        public static void RegisterGameObject(SimulatedBody simulatedBody)
        {
            if(SimulatedBodyStack.GetSimulatedBody(simulatedBody.GameObject) == null)
            {
                SimulatedBodyStack.SimulatedBodyList.Add(simulatedBody);
            }

            if(s_disablePhysicsTimeMode == DisablePhysicsMode.ObjectTime)
            {
                if(PhysicsSimulatorPath.Settings.SimulatePhysics)
                {
                    EditorCoroutines.StartCoroutine(ObjectTimePhysicsSimulator.DisablePhysicsSupportWithDelay(PhysicsSimulatorPath.Settings.ObjectTime, simulatedBody), simulatedBody.GameObject);
                }
            }
        }
    }
}
#endif