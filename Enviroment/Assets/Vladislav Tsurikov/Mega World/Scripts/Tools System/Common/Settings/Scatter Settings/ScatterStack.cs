using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Collections;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class ScatterStack 
    {
        private Scatter s_clipboardContent;

        [SerializeField]
        public List<Scatter> Settings = new List<Scatter>();
        public bool Init = false; 

        public void Clear()
        {
#if UNITY_EDITOR
            foreach (Scatter asset in Settings)
            {
                ModulesUtility.RemoveAsset(asset);
            }

            Settings.Clear();
#endif
        }

        public void PasteStack(ScatterStack stack, ScriptableObject asset)
        {
#if UNITY_EDITOR
            Clear();

            foreach (var item in stack.Settings)
            {
                Settings.Add((Scatter)ModulesUtility.CreateAsset(item.GetType(), asset));

                CopySettings(item);
                PasteSettings(Settings[Settings.Count - 1]);
            }
#endif
        }

        void CopySettings(Scatter target)
        {
#if UNITY_EDITOR
            if (s_clipboardContent != null)
            {
                ModulesUtility.Destroy(s_clipboardContent);
                s_clipboardContent = null;
            }

            s_clipboardContent = (Scatter)ScriptableObject.CreateInstance(target.GetType());
            EditorUtility.CopySerializedIfDifferent(target, s_clipboardContent);
#endif
        }

        void PasteSettings(Scatter target)
        {
#if UNITY_EDITOR
            EditorUtility.CopySerializedIfDifferent(s_clipboardContent, target);
#endif
        }

        public void AddSettings(System.Type settingsType, ScriptableObject asset)
        {
#if UNITY_EDITOR
            Settings.Add((Scatter)ModulesUtility.CreateAsset(settingsType, asset));
#endif
        }

#if UNITY_EDITOR
        public Scatter CreateSettingsAndAdd(System.Type settingsType, ScriptableObject parentAsset)
        {

            Scatter asset = (Scatter)ModulesUtility.CreateAsset(settingsType, parentAsset);
            Settings.Add(asset);
            return asset;
        }
#endif

        public void RemoveSettings(int index)
        {
            var transformComponent = Settings[index];
            Settings.RemoveAt(index);

#if UNITY_EDITOR
            ModulesUtility.RemoveAsset(transformComponent);
#endif
        }

        public List<Vector2> Samples(AreaVariables areaVariables)
        {
            List<Vector2> samples = new List<Vector2>();

            foreach (Scatter item in Settings)
            {
                if(item.Enabled)
                {
                    item.Samples(areaVariables, samples);
                }
            }

            return samples;
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