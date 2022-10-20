#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using VladislavTsurikov.EditorCoroutinesSystem;
using System.Collections;

namespace VladislavTsurikov.PhysicsSimulatorEditor
{
    public static class ObjectTimePhysicsSimulator
    {
        private static bool _pastSimulatePhysics = false;

        public static void SimulatePhysics() 
        {
            DisablePhysicsSupportWithDelay();

            PhysicsSimulator.Simulate();
        }

        public static void DisablePhysicsSupportWithDelay() 
        {            
            if(SimulatedBodyStack.SimulatedBodyList.Count == 0)
            {
                return;
            }

            if(PhysicsSimulatorPath.Settings.SimulatePhysics)
            {
                if(!_pastSimulatePhysics)
                {
                    foreach (SimulatedBody simulatedBody in SimulatedBodyStack.SimulatedBodyList)
                    {
                        EditorCoroutines.StartCoroutine(DisablePhysicsSupportWithDelay(PhysicsSimulatorPath.Settings.ObjectTime, simulatedBody), simulatedBody.GameObject);
                    }
                }

                _pastSimulatePhysics = true;
            }
            else
            {
                if(_pastSimulatePhysics)
                {
                    foreach (SimulatedBody simulatedBody in SimulatedBodyStack.SimulatedBodyList)
                    {
                        EditorCoroutines.StopCoroutine(DisablePhysicsSupportWithDelay(PhysicsSimulatorPath.Settings.ObjectTime, simulatedBody), simulatedBody.GameObject);               
                    }        
                }

                _pastSimulatePhysics = false;      
            }
        }

        public static void StopAllCoroutine() 
        {            
            foreach (SimulatedBody simulatedBody in SimulatedBodyStack.SimulatedBodyList)
            {
                EditorCoroutines.StopCoroutine(DisablePhysicsSupportWithDelay(PhysicsSimulatorPath.Settings.ObjectTime, simulatedBody), simulatedBody.GameObject);               
            } 
        }

        public static IEnumerator DisablePhysicsSupportWithDelay(float waitForSeconds, SimulatedBody simulatedBody) 
        {
            yield return new WaitForSeconds(waitForSeconds);

            SimulatedBodyStack.DisablePhysicsSupport(simulatedBody);
        }
    }
}
#endif