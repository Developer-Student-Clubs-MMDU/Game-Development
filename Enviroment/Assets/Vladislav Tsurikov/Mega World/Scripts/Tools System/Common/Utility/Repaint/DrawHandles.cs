#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using VladislavTsurikov.Extensions;

namespace VladislavTsurikov.MegaWorldSystem
{
    public static class DrawHandles 
    {
        public static void DrawSpawnVisualizerPixel(SpawnVisualizerPixel spawnVisualizerPixel, float stepIncrement)
        {
            if(MegaWorldPath.AdvancedSettings.VisualisationSettings.SimpleFilterSettings.ColorHandlesType == ColorHandlesType.Custom)
            {
                Handles.color = Color.Lerp(MegaWorldPath.AdvancedSettings.VisualisationSettings.SimpleFilterSettings.InactiveColor, MegaWorldPath.AdvancedSettings.VisualisationSettings.SimpleFilterSettings.ActiveColor, spawnVisualizerPixel.fitness).WithAlpha(spawnVisualizerPixel.alpha);
            }
            else
            {
                if(spawnVisualizerPixel.fitness < 0.5)
                {
                    float difference = spawnVisualizerPixel.fitness / 0.5f;
                    Handles.color = Color.Lerp(Color.red, Color.yellow, difference).WithAlpha((spawnVisualizerPixel.alpha));
                }
                else
                {
                    float difference = (spawnVisualizerPixel.fitness - 0.5f) / 0.5f;
                    Handles.color = Color.Lerp(Color.yellow, Color.green, difference).WithAlpha((spawnVisualizerPixel.alpha));
                }
            }

            if(MegaWorldPath.AdvancedSettings.VisualisationSettings.SimpleFilterSettings.HandlesType == HandlesType.DotCap)
            {
                if(MegaWorldPath.AdvancedSettings.VisualisationSettings.SimpleFilterSettings.HandleResizingType == HandleResizingType.Resolution)
                {
                    DotCap(0, spawnVisualizerPixel.position, Quaternion.identity, stepIncrement / 3);
                }
                else if(MegaWorldPath.AdvancedSettings.VisualisationSettings.SimpleFilterSettings.HandleResizingType == HandleResizingType.Distance)
                {
                    DotCap(0, spawnVisualizerPixel.position, Quaternion.identity, HandleUtility.GetHandleSize(spawnVisualizerPixel.position) * 0.03f);
                }
                else
                {
                    DotCap(0, spawnVisualizerPixel.position, Quaternion.identity, MegaWorldPath.AdvancedSettings.VisualisationSettings.SimpleFilterSettings.CustomHandleSize);
                }
            }
            else
            {
                if(MegaWorldPath.AdvancedSettings.VisualisationSettings.SimpleFilterSettings.HandleResizingType == HandleResizingType.Resolution)
                {
                    Handles.SphereHandleCap(0, new Vector3(spawnVisualizerPixel.position.x, spawnVisualizerPixel.position.y, spawnVisualizerPixel.position.z), Quaternion.LookRotation(Vector3.up), 
                        stepIncrement / 2, EventType.Repaint);
                }
                else if(MegaWorldPath.AdvancedSettings.VisualisationSettings.SimpleFilterSettings.HandleResizingType == HandleResizingType.Distance)
                {
                    Handles.SphereHandleCap(0, new Vector3(spawnVisualizerPixel.position.x, spawnVisualizerPixel.position.y, spawnVisualizerPixel.position.z), Quaternion.LookRotation(Vector3.up), 
                        HandleUtility.GetHandleSize(spawnVisualizerPixel.position) * 0.05f, EventType.Repaint);
                }
                else
                {
                    Handles.SphereHandleCap(0, new Vector3(spawnVisualizerPixel.position.x, spawnVisualizerPixel.position.y, spawnVisualizerPixel.position.z), Quaternion.LookRotation(Vector3.up), 
                        MegaWorldPath.AdvancedSettings.VisualisationSettings.SimpleFilterSettings.CustomHandleSize, EventType.Repaint);
                }
            }
        }

        public static void DrawXYZCross(RayHit hit, Vector3 upwards, Vector3 right, Vector3 forward)
		{
			float handleSize = HandleUtility.GetHandleSize (hit.Point) * 0.5f;

			Handles.color = Color.green;
            Handles.DrawAAPolyLine(3, hit.Point + upwards * handleSize, hit.Point + upwards * -handleSize * 0.2f);
			Handles.color = Color.red;
            Handles.DrawAAPolyLine(3, hit.Point + right * handleSize, hit.Point + right * -handleSize * 0.2f);
			Handles.color = Color.blue;
            Handles.DrawAAPolyLine(3, hit.Point + forward * handleSize, hit.Point + forward * -handleSize * 0.2f);
		}

        public static void CircleCap(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            if(Event.current != null && (Event.current.type == EventType.Layout || Event.current.type == EventType.Repaint))
                Handles.CircleHandleCap(controlID, position, rotation, size, Event.current.type);
        }

        public static void DotCap(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            if(Event.current != null && (Event.current.type == EventType.Layout || Event.current.type == EventType.Repaint))
            {
                Handles.DotHandleCap(controlID, position, rotation, size, Event.current.type);
            } 
        }
    }
}
#endif