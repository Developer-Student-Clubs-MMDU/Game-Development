using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace VladislavTsurikov.MegaWorldSystem
{
    public enum BlendMode
    {
        Multiply,
        Add,
    }

    public enum ColorSpaceForBrushMaskFilter
    {
        СustomColor,
        Colorful,
        Heightmap
    }

    [Serializable]
    public class MaskFilterStack
    {
        public List<MaskFilter> Settings = new List<MaskFilter>();
        private static MaskFilter s_clipboardContent;

        public void Clear()
        {
#if UNITY_EDITOR
            foreach (MaskFilter asset in Settings)
            {
                ModulesUtility.RemoveAsset(asset);
            }

            Settings.Clear();
#endif
        }

        public void Eval(MaskFilterContext fc)
        {
            int count = Settings.Count;

            RenderTexture prev = RenderTexture.active;

            RenderTexture[] rts = new RenderTexture[2];

            int srcIndex = 0;
            int destIndex = 1;

            rts[0] = RenderTexture.GetTemporary(fc.DestinationRenderTexture.descriptor);
            rts[1] = RenderTexture.GetTemporary(fc.DestinationRenderTexture.descriptor);

            rts[0].enableRandomWrite = true;
            rts[1].enableRandomWrite = true;

            Graphics.Blit(Texture2D.whiteTexture, rts[0]);
            Graphics.Blit(Texture2D.blackTexture, rts[1]); //don't remove this! needed for compute shaders to work correctly.

            for( int i = 0; i < count; ++i )
            {
                if( Settings[ i ].Enabled )
                {
                    fc.SourceRenderTexture = rts[srcIndex];
                    fc.DestinationRenderTexture = rts[destIndex];

                    Settings[ i ].Eval(fc, i);

                    destIndex += srcIndex;
                    srcIndex = destIndex - srcIndex;
                    destIndex = destIndex - srcIndex;
                }
            }

            Graphics.Blit(rts[srcIndex], fc.Output);//fc.destinationRenderTexture);

            RenderTexture.ReleaseTemporary(rts[0]);
            RenderTexture.ReleaseTemporary(rts[1]);

            RenderTexture.active = prev;
        }

        public void PasteStack(MaskFilterStack filterStack, ScriptableObject asset)
        {
#if UNITY_EDITOR
            Clear();

            foreach (var item in filterStack.Settings)
            {
                var effect = (MaskFilter)ModulesUtility.CreateAsset(item.GetType(), asset);
                Settings.Add(effect);

                CopySettings(item);
                PasteSettings(Settings[Settings.Count - 1]);
            }
#endif
        }

        static void CopySettings(MaskFilter target)
        {
#if UNITY_EDITOR
            if (s_clipboardContent != null)
            {
                ModulesUtility.Destroy(s_clipboardContent);
                s_clipboardContent = null;
            }

            s_clipboardContent = (MaskFilter)ScriptableObject.CreateInstance(target.GetType());
            EditorUtility.CopySerializedIfDifferent(target, s_clipboardContent);
#endif
        }

        static void PasteSettings(MaskFilter target)
        {
#if UNITY_EDITOR
            EditorUtility.CopySerializedIfDifferent(s_clipboardContent, target);
#endif
        }

        public void AddSettings(System.Type type, ScriptableObject asset)
        {
#if UNITY_EDITOR
            Settings.Add((MaskFilter)ModulesUtility.CreateAsset(type, asset));
#endif
        }

#if UNITY_EDITOR
        public MaskFilter CreateSettingsAndAdd(System.Type settingsType, ScriptableObject parentAsset)
        {

            MaskFilter asset = (MaskFilter)ModulesUtility.CreateAsset(settingsType, parentAsset);
            Settings.Add(asset);
            return asset;
        }
#endif

        public void RemoveSettings(int index)
        {
#if UNITY_EDITOR
            var asset = Settings[index];
            Settings.RemoveAt(index);

            ModulesUtility.RemoveAsset(asset);
#endif
        }
    }
}