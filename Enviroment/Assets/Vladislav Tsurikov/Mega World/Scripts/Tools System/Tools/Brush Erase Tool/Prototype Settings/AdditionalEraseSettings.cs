using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VladislavTsurikov.MegaWorldSystem.BrushErase
{
    [Serializable]
    public class AdditionalEraseSettings : BaseSettings
    {
        [Range (0, 100)]
        public float SuccessForErase = 100f;

#if UNITY_EDITOR
        public AdditionalSettingsEditor AdditionalSettingsEditor = new AdditionalSettingsEditor();

        public override void OnGUI()
        {
            AdditionalSettingsEditor.OnGUI(this);
        }
#endif
    }
}