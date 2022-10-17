#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VladislavTsurikov.EditorCoroutinesSystem;
using UnityEngine.Events;

namespace VladislavTsurikov.PhysicsSimulatorEditor
{
    public static class SimulatedBodyStack 
    {
        public static List<SimulatedBody> SimulatedBodyList = new List<SimulatedBody>();

        public static event UnityAction<GameObject> DisablePhysicsSupportPerformed;

        public static void DisablePhysicsSupportIfObjectStopped() 
        {            
            if(!PhysicsSimulatorPath.Settings.SimulatePhysics)
            {
                return;
            }

            List<SimulatedBody> removeSimulatedBodyList = new List<SimulatedBody>();

            foreach (SimulatedBody simulatedBody in SimulatedBodyList)
            {
                if(simulatedBody.HasObjectStopped())
                {
                    removeSimulatedBodyList.Add(simulatedBody);
                }
            }

            foreach (SimulatedBody simulatedBody in removeSimulatedBodyList)
            {
                DisablePhysicsSupport(simulatedBody);
            }
        }

        public static SimulatedBody GetSimulatedBody(GameObject gameObject)
        {
            foreach (SimulatedBody simulatedBody in SimulatedBodyList)
            {
                if(simulatedBody.GameObject == null)
                {
                    continue;
                }

                if(simulatedBody.GameObject == gameObject)
                {
                    return simulatedBody;
                }
            }

            return null;
        }

        public static void DisableAllPhysicsSupport() 
        {
            List<SimulatedBody> removeSimulatedBodyList = new List<SimulatedBody>();
            removeSimulatedBodyList.AddRange(SimulatedBodyList);

            foreach (SimulatedBody simulatedBody in removeSimulatedBodyList)
            {
                DisablePhysicsSupport(simulatedBody);
            }
        }

        public static void DisablePhysicsSupport(SimulatedBody simulatedBody) 
        {
            if(simulatedBody.GameObject == null)
            {
                SimulatedBodyList.Remove(simulatedBody);
                return;
            }

            if(!simulatedBody.HasRigidbody())
            {
                return;
            }

            simulatedBody.DisablePhysicsSupport();
            DisablePhysicsSupportPerformed(simulatedBody.GameObject);
            SimulatedBodyList.Remove(simulatedBody);
        }
    }
}
#endif