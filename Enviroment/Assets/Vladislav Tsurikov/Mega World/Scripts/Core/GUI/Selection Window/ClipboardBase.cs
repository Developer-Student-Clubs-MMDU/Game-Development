#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace VladislavTsurikov.MegaWorldSystem
{
    public class ClipboardBase 
    {
        public virtual void PrototypeGameObjectMenu(GenericMenu menu, SelectedVariables selectedDataVariables){}
        public virtual void PrototypeInstantItemMenu(GenericMenu menu, SelectedVariables selectedDataVariables){}
        public virtual void PrototypeUnityTerrainDetailMenu(GenericMenu menu, SelectedVariables selectedDataVariables){}
        public virtual void PrototypeUnityTerrainTextureMenu(GenericMenu menu, SelectedVariables selectedDataVariables){}
        public virtual void CopyAllGameObjectSettings(SelectedVariables selectedVariables){}
        public virtual void CopyAllTerrainDetailSettings(SelectedVariables selectedVariables){}
        public virtual void CopyAllInstantItemSettings(SelectedVariables selectedVariables){}
        public virtual void CopyAllTerrainTextureSettings(SelectedVariables selectedVariables){}
        public virtual void PasteAllGameObjectSettings(SelectedVariables selectedVariables){}
        public virtual void PasteAllTerrainDetailSettings(SelectedVariables selectedVariables){}
        public virtual void PasteAllInstantItemSettings(SelectedVariables selectedVariables){}

        public virtual void GroupGameObjectMenu(GenericMenu menu, SelectedVariables selectedDataVariables){}
        public virtual void GroupInstantItemMenu(GenericMenu menu, SelectedVariables selectedDataVariables){}
        public virtual void GroupUnityTerrainDetailMenu(GenericMenu menu, SelectedVariables selectedDataVariables){}
        public virtual void GroupUnityTerrainTextureMenu(GenericMenu menu, SelectedVariables selectedDataVariables){}
        public virtual void CopyAllGroupSettings(SelectedVariables selectedVariables){}
        public virtual void PasteAllGroupSettings(SelectedVariables selectedVariables){}

        public BaseSettings CopySettings(BaseSettings clipboardSettings, BaseSettings target)
        {
#if UNITY_EDITOR
            if (clipboardSettings != null)
            {
                ModulesUtility.Destroy(clipboardSettings);
                clipboardSettings = null;
            }

            clipboardSettings = (BaseSettings)ScriptableObject.CreateInstance(target.GetType());
            EditorUtility.CopySerializedIfDifferent(target, clipboardSettings);
            return clipboardSettings;
#endif
        }

        public void PasteSettings(BaseSettings source, BaseSettings dest)
        {
            if(source == null)
            {
                return;
            }

#if UNITY_EDITOR
            EditorUtility.CopySerializedIfDifferent(source, dest);
#endif
        }
    }
}
#endif