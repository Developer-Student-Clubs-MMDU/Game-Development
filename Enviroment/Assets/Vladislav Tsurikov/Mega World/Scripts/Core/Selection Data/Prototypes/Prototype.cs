using System;
using UnityEngine;
using System.IO;
using UnityEditor;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public abstract class Prototype : ScriptableObject
    {
        [SerializeField]
        protected Group _type;

        public GameObject Prefab;
        public bool Selected = false;
        public bool Active = true;

        public ToolSettingsStack ToolSettingsStack = new ToolSettingsStack();
        public SettingsStack SettingsStack = new SettingsStack();

        [NonSerialized]
		public PastTransform PastTransform;
        public int ID;
		public Vector3 Extents = Vector3.one;

        public BaseSettings GetSettings(System.Type settingsType)
        {
            return SettingsStack.GetSettings(settingsType);
        }

        public BaseSettings GetSettings(System.Type toolSystem, System.Type settingsType)
        {
            return ToolSettingsStack.GetSettings(toolSystem, settingsType);
        }

        private void Init(Group type)
        {
            _type = type;
        }

        public void OnEnable() 
        {
            AllAvailablePrototypes.Add(this);
        }

        public static Prototype Create(string targetName, Group group, System.Type prototypeType)
        {
#if UNITY_EDITOR
            Directory.CreateDirectory(MegaWorldPath.pathToDataPackage);
            Directory.CreateDirectory(MegaWorldPath.pathToGroup);
            Directory.CreateDirectory(MegaWorldPath.pathToPrototype);

            var path = string.Empty;

            path += targetName + ".asset";

            path = CommonPath.CombinePath(MegaWorldPath.pathToPrototype, path);
            path = AssetDatabase.GenerateUniqueAssetPath(path);

            var profile = (Prototype)ScriptableObject.CreateInstance(prototypeType);
            profile.Init(group);
            AssetDatabase.CreateAsset(profile, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            return profile;

#else 
            return null;
#endif
        }

        public bool IsNullType() 
        {
            return _type == null;
        }

        public void Save() 
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

#if UNITY_EDITOR
        public abstract void SetIconInfo(out Texture2D preview, out string name);
#endif
    }
}
