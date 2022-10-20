using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class TransformComponentStack 
    {
        private TransformComponent s_clipboardContent;
        
        [SerializeField]
        public List<TransformComponent> Settings = new List<TransformComponent>();
        public bool Init;

        public void Clear()
        {
#if UNITY_EDITOR
            foreach (TransformComponent asset in Settings)
            {
                ModulesUtility.RemoveAsset(asset);
            }

            Settings.Clear();
#endif
        }

        public void PasteStack(TransformComponentStack stack, Prototype proto)
        {
#if UNITY_EDITOR
            Clear();

            foreach (var item in stack.Settings)
            {
                Settings.Add((TransformComponent)ModulesUtility.CreateAsset(item.GetType(), proto));

                CopySettings(item);
                PasteSettings(Settings[Settings.Count - 1]);
            }
#endif
        }

        public void CopySettings(TransformComponent target)
        {
#if UNITY_EDITOR
            if (s_clipboardContent != null)
            {
                ModulesUtility.Destroy(s_clipboardContent);
                s_clipboardContent = null;
            }

            s_clipboardContent = (TransformComponent)ScriptableObject.CreateInstance(target.GetType());
            EditorUtility.CopySerializedIfDifferent(target, s_clipboardContent);
#endif
        }

        public void PasteSettings(TransformComponent target)
        {
#if UNITY_EDITOR
            EditorUtility.CopySerializedIfDifferent(s_clipboardContent, target);
#endif
        }

        public void AddSettings(System.Type type, ScriptableObject asset)
        {
#if UNITY_EDITOR
            Settings.Add((TransformComponent)ModulesUtility.CreateAsset(type, asset));
#endif
        }

#if UNITY_EDITOR
        public TransformComponent CreateSettingsAndAdd(System.Type settingsType, ScriptableObject parentAsset)
        {

            TransformComponent asset = (TransformComponent)ModulesUtility.CreateAsset(settingsType, parentAsset);
            Settings.Add(asset);
            return asset;
        }
#endif

        public void RemoveSettings(int index)
        {
#if UNITY_EDITOR
            var transformComponent = Settings[index];
            Settings.RemoveAt(index);

            ModulesUtility.RemoveAsset(transformComponent);
#endif
        }

        public void SetInstanceData(ref InstanceData instanceData, float fitness, Vector3 normal)
        {
            foreach (TransformComponent item in Settings)
            {
                if(item.Enabled)
                {
                    item.SetInstanceData(ref instanceData, fitness, normal);
                }
            }
        }

        public bool HasSettings(System.Type type)
        {
            foreach (var setting in Settings)
            {
                if (setting.GetType() == type)
                    return true;
            }

            return false;
        }
    }
}
