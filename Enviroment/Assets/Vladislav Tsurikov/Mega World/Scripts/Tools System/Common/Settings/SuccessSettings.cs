using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class SuccessSettings : BaseSettings
    {
        [Range (0, 100)]
        public float SuccessValue = 100f;

#if UNITY_EDITOR
        public SuccessSettingsEditor SettingsEditor = new SuccessSettingsEditor();

        public override void OnGUI()
        {
            SettingsEditor.OnGUI(this);
        }
#endif

        public SuccessSettings()
        {
        }
    }
}
