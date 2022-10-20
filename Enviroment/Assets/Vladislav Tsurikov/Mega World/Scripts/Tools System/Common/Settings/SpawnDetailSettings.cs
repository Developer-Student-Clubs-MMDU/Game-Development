using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.MegaWorldSystem
{
    [System.Serializable]
    public class SpawnDetailSettings : BaseSettings
    {
        public bool UseRandomOpacity = true;
        public int Density = 5;
        public float FailureRate = 80f;

        #if UNITY_EDITOR
        public SpawnDetailSettingsEditor SpawnDetailSettingsEditor = new SpawnDetailSettingsEditor();

        public override void OnGUI()
        {
            SpawnDetailSettingsEditor.OnGUI(this);
        }
        #endif

        public SpawnDetailSettings()
        {

        }

        public SpawnDetailSettings(SpawnDetailSettings other)
        {
            CopyFrom(other);
        }

        public void CopyFrom(SpawnDetailSettings other)
        {            
            UseRandomOpacity = other.UseRandomOpacity;
            Density = other.Density;
        }
    }
}

