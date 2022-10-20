using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class SettingsStack 
    {
        [SerializeField]
        public List<BaseSettings> Settings = new List<BaseSettings>();

        public bool HasSettings(System.Type group)
        {
            foreach (var setting in Settings)
            {
                if (setting.GetType() == group)
                    return true;
            }

            return false;
        }

        public BaseSettings GetSettings(System.Type group)
        {
            foreach (var setting in Settings)
            {
                if (setting.GetType() == group)
                    return setting;
            }

            return null;
        }

#if UNITY_EDITOR
        public void Create(System.Type systemType, ScriptableObject asset)
        {
            if(!HasSettings(systemType))
            {
                BaseSettings baseSettings = (BaseSettings)ModulesUtility.CreateAsset(systemType, asset);
                baseSettings.Init(asset);
                Settings.Add(baseSettings);
            }
        }

        public void Remove(int index)
        {
            var asset = Settings[index];
            Settings.RemoveAt(index);

            ModulesUtility.RemoveAsset(asset);
        }
#endif
    }
}