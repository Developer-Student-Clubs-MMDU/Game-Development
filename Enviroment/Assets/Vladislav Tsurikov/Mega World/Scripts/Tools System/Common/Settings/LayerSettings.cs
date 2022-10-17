using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class LayerSettings
    {
        public LayerMask PaintLayers = 1;

#if UNITY_EDITOR
        public LayerSettingsEditor LayerSettingsEditor = new LayerSettingsEditor();

        public void OnGUI(bool useOnlyCustomRaycast = false)
        {
            LayerSettingsEditor.OnGUI(this, useOnlyCustomRaycast);
        }
#endif

        public LayerMask GetCurrentPaintLayers(ResourceType resourceType)
        {
            switch (resourceType)
            {
                case ResourceType.TerrainDetail:
                {
                    if(Terrain.activeTerrain == null)
                    {
                        Debug.LogWarning("Not present in the scene with an active Unity Terrain.");
                    }

                    return LayerMask.GetMask(LayerMask.LayerToName(Terrain.activeTerrain.gameObject.layer));
                }
                case ResourceType.TerrainTexture:
                {
                    if(Terrain.activeTerrain == null)
                    {
                        Debug.LogWarning("Not present in the scene with an active Unity Terrain.");
                    }
                    
                    return LayerMask.GetMask(LayerMask.LayerToName(Terrain.activeTerrain.gameObject.layer));
                }
                default:
                {
                    return PaintLayers;
                }
            }
        }
    }
}