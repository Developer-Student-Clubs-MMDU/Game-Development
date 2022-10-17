#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace VladislavTsurikov
{
    public static class MegaWorldGUIUtility 
    {
        public static Texture2D GetPrefabPreviewTexture(GameObject prefab)
        {
#if UNITY_EDITOR
            Texture2D previewTexture;

            if((previewTexture = AssetPreview.GetAssetPreview(prefab)) != null)
			{
				return previewTexture;
			}
                
			return AssetPreview.GetMiniTypeThumbnail(typeof(GameObject));
#else
            return Texture2D.whiteTexture;
#endif
        }

        public static void ContextMenuCallback(object obj)
        {
            if (obj != null && obj is Action)
                (obj as Action).Invoke();
        }

        public static T GetAsset<T>(string prefix) where T : UnityEngine.Object
        {
            return AssetDatabase.LoadAssetAtPath("Assets/" + prefix + typeof(T).Name + ".asset", typeof(T)) as T;
        }

        public static T CreateAsset<T>(string prefix, bool isUniqueRenameMode) where T : ScriptableObject
        {
            var asset = ScriptableObject.CreateInstance<T>();

            if (isUniqueRenameMode)
            {
                ProjectWindowUtil.CreateAsset(asset, prefix + typeof(T).Name + ".asset");
            }
            else
            {
                AssetDatabase.CreateAsset(asset, "Assets/" + prefix + typeof(T).Name + ".asset");
            }
            return asset;
        }

        // Check folder is valid. If it is missing, create it.
        // Does not check for multiple folder levels. Just the last one
        public static void CheckFolder(string folderPath)
        {
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                int i = folderPath.LastIndexOf('/');

                if (i >= 0 && i < folderPath.Length - 2)
                {
                    Debug.Log("INFO MegaWorldEditorHelper - Creating new folder " + folderPath.Substring(i + 1) + " in " + folderPath.Substring(0, i));

                    AssetDatabase.CreateFolder(folderPath.Substring(0, i), folderPath.Substring(i + 1));
                }
            }
        }

        /// <summary>
        /// Call an item from the Unity menu. Menu can also be one custom created.
        /// USAGE: LBEditorHelper.CallMenu("Edit/Project Settings/Player");
        /// </summary>
        /// <param name="menuItemPath"></param>
        public static void CallMenu(string menuItemPath)
        {
            if (!string.IsNullOrEmpty(menuItemPath))
            {
                EditorApplication.ExecuteMenuItem(menuItemPath);
            }
        }

        public static GUIContent GetCurrentLODGUIContent(float lodWidth, float distance, int lodCount)
		{
			if(lodWidth < 20)
			{
				return new GUIContent("");
			}
			else if(lodWidth < 40)
			{
				return new GUIContent(distance.ToString("F0"));
			}
			else
			{
				return lodWidth < 100 ? new GUIContent(distance.ToString("F0") + "m") : new GUIContent ("LOD " + lodCount.ToString() + " - " + distance.ToString("F0") + "m");
			}
		}

#if UNITY_2018_2_OR_NEWER
        public static UnityEngine.Object GetCorrespondingObjectFromSource(UnityEngine.Object source)
        {
            return PrefabUtility.GetCorrespondingObjectFromSource(source);
        }
#else
        public static UnityEngine.Object GetCorrespondingObjectFromSource(UnityEngine.Object source)
        {
            return PrefabUtility.GetPrefabParent(source);
        }
#endif

        public static bool IsModifierDown(EventModifiers modifiers)
        {
            Event e = Event.current;
            
            if ((e.modifiers & EventModifiers.FunctionKey) != 0)
                return false;

            EventModifiers mask = EventModifiers.Alt | EventModifiers.Control | EventModifiers.Shift | EventModifiers.Command;
            modifiers &= mask;

            if (modifiers == 0 && (e.modifiers & (mask & ~modifiers)) == 0)
                return true;

            if ((e.modifiers & modifiers) != 0 && (e.modifiers & (mask & ~modifiers)) == 0)
                return true;

            return false;
        }
    }
}
#endif
