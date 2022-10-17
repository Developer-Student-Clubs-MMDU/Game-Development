using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class TerrainDetailSettings : BaseSettings
    {   
        public bool MinMax = true;
        public bool OnlyOneColor = true;

        [Range(0f, 1f)]
        public float NoiseSpread = 0.3f;
        public float MinWidth = 0.8f;
        public float MaxWidth = 1.4f;
        public float MinHeight = 0.8f;
        public float MaxHeight = 1.4f;
        public Color HealthyColour = Color.white;
        public Color DryColour = Color.white;
        public DetailRenderMode RenderMode;
        public bool Billboard = false;

#if UNITY_EDITOR
        public TerrainDetailsSettingsEditor TerrainDetailsSettingsEditor = new TerrainDetailsSettingsEditor();

        public override void OnGUI()
        {

        }

        public void OnGUI(PrototypeTerrainDetail protoTerrainDetail)
        {
            TerrainDetailsSettingsEditor.OnGUI(protoTerrainDetail);
        }
#endif

        public TerrainDetailSettings()
        {
            
        }

        public TerrainDetailSettings(TerrainDetailSettings other)
        {
            CopyFrom(other);
        }

        public void CopyFrom(TerrainDetailSettings other)
        {            
            NoiseSpread = other.NoiseSpread;
            MinWidth = other.MinWidth;
            MaxWidth = other.MaxWidth;
            MinHeight = other.MinHeight;
            MaxHeight = other.MaxHeight;
            HealthyColour = other.HealthyColour;
            DryColour = other.DryColour;
            RenderMode = other.RenderMode;
            Billboard = other.Billboard;
        }

        public void SetRandomForWidthHeight()
        {
            float min = UnityEngine.Random.Range(0.4f, 1f);
            float max = UnityEngine.Random.Range(1f, 1.7f);

            min = (float)Mathf.Round(min * 100f) / 100f;
            max = (float)Mathf.Round(max * 100f) / 100f;

            MinWidth = min;
            MaxWidth = max;

            MinHeight = min;
            MaxHeight = max;
        }

        public void SetRandomForColor()
        {
            HealthyColour = UnityEngine.Random.ColorHSV();
            DryColour = UnityEngine.Random.ColorHSV();
        }
    }
}

