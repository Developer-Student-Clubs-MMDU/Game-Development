using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    public enum RaycastType
    {
        UnityRaycast,
        CustomRaycast
    }

    [Serializable]
    public class RaycastSettings
    {
        public RaycastType RaycastType = RaycastType.UnityRaycast;
        public float MaxRayDistance = 6500f;
        public float Offset = 500;

        #if UNITY_EDITOR
        public RaycastSettingsEditor raycastSettingsEditor = new RaycastSettingsEditor();

        public void OnGUI()
        {
            raycastSettingsEditor.OnGUI(this);
        }
        #endif
    }
}
