#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using VladislavTsurikov.Extensions;

namespace VladislavTsurikov.CustomGUI
{
    //[CreateAssetMenu()]
    public class EditorColors : ScriptableObject
    {
        private static EditorColors instance;

        public static EditorColors Instance
        {
            get
            {
                if (instance != null) return instance;
                instance = AssetDatabase.LoadAssetAtPath<EditorColors>(CommonPath.CombinePath(CustomGUIPath.pathToGUISkin, "EditorColors.asset"));
                return instance;
            }
        }

        public Color redNormal = new Color().ColorFrom256(255, 0, 66);
    	public Color greenNormal = new Color().ColorFrom256(0, 255, 131);
    	public Color redDark = new Color().ColorFrom256(128, 12, 34);
    	public Color greenDark = new Color().ColorFrom256(0, 115, 59);

    	public Color orangeNormal = new Color().ColorFrom256(255, 145, 0);
    	public Color orangeDark = new Color().ColorFrom256(128, 73, 0);

        public Color docsButtonColor = new Color(0.89f, 0.74f, 0.3f, 1f);
        public Color docsLabelColor = Color.gray;

        #region LightTheme
		public Color labelColorLightTheme = new Color(0f, 0f, 0f, 1f);
        public Color clickButtonColorLightTheme = new Color(0.95f, 0.95f, 0.95f, 1f);
        public Color toggleButtonActiveColorLightTheme = new Color(0.26f, 0.6f, 0.83f, 1f);
        public Color toggleButtonInactiveColorLightTheme = new Color(0.95f, 0.95f, 0.95f, 1f);
        public Color boxColorLightTheme = new Color(0.8f, 0.8f, 0.8f, 1f);
        #endregion

        
        #region DarkTheme
        public Color labelColorDarkTheme = new Color(0.9f, 0.9f, 0.9f, 1f);
        public Color clickButtonColorDarkTheme = new Color(0.3f, 0.3f, 0.3f, 1f);
        public Color toggleButtonActiveColorDarkTheme = new Color(0.22f, 0.52f, 0.71f, 1f);
        public Color toggleButtonInactiveColorDarkTheme = new Color(0.3f, 0.3f, 0.3f, 1f);
        public Color boxColorDarkTheme = new Color(0.3f, 0.3f, 0.3f, 1f);
        #endregion        

        public Color Green
        {
            get
            {
                if (EditorGUIUtility.isProSkin)
                {
                    return greenNormal;
                    
                }
                else
                {
                    return greenDark;
                }
            }
        }

        public Color Red
        {
            get
            {
                if (EditorGUIUtility.isProSkin)
                {
                    return redNormal;
                }
                else
                {
                    return redDark;
                }
            }
        }

        public Color LabelColor
        {
            get
            {
                if (EditorGUIUtility.isProSkin)
                {
                    return labelColorDarkTheme;
                }
                else
                {
                    return labelColorLightTheme;
                }
            }
        }

        public Color ClickButtonColor
        {
            get
            {
                if (EditorGUIUtility.isProSkin)
                {
                    return clickButtonColorDarkTheme;
                }
                else
                {
                    return clickButtonColorLightTheme;
                }
            }
        }

        public Color ToggleButtonActiveColor
        {
            get
            {
                if (EditorGUIUtility.isProSkin)
                {
                    return toggleButtonActiveColorDarkTheme;
                }
                else
                {
                    return toggleButtonActiveColorLightTheme;
                }
            }
        }

        public Color ToggleButtonInactiveColor
        {
            get
            {
                if (EditorGUIUtility.isProSkin)
                {
                    return toggleButtonInactiveColorDarkTheme;
                }
                else
                {
                    return toggleButtonInactiveColorLightTheme;
                }
            }
        }

        public Color boxColor
        {
            get
            {
                if (EditorGUIUtility.isProSkin)
                {
                    return boxColorDarkTheme;
                }
                else
                {
                    return boxColorLightTheme;
                }
            }
        }

        public Color separatorColor = new Color32(127, 127, 127, 255);
    }

    [CustomEditor(typeof(EditorColors))]
    public class EditorColorsEditor : UnityEditor.Editor
    {
        protected readonly Dictionary<string, SerializedProperty> SerializedProperties = new Dictionary<string, SerializedProperty>();

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUIStyle labelStyle = new GUIStyle("Label") { fontStyle = FontStyle.Italic };

            if (EditorGUIUtility.isProSkin)
            {
                labelStyle.normal.textColor = new Color(1f, 1f, 1f);
            }
            else
            {
                labelStyle.normal.textColor = new Color(0f, 0f, 0f);
            }

            EditorGUILayout.LabelField("Colors", labelStyle);
            EditorGUILayout.Space(3);
            

            EditorGUILayout.PropertyField(GetProperty("redNormal"), new GUIContent("Red Normal"), true);
            EditorGUILayout.PropertyField(GetProperty("greenNormal"), new GUIContent("Green Normal"), true);

            EditorGUILayout.PropertyField(GetProperty("redDark"), new GUIContent("Red Dark"), true);
            EditorGUILayout.PropertyField(GetProperty("greenDark"), new GUIContent("Green Dark"), true);
            EditorGUILayout.PropertyField(GetProperty("orangeNormal"), new GUIContent("Orange Normal"), true);
            EditorGUILayout.PropertyField(GetProperty("orangeDark"), new GUIContent("Orange Dark"), true);

            EditorGUILayout.PropertyField(GetProperty("docsButtonColor"), new GUIContent("Docs Button Color"), true);
            EditorGUILayout.PropertyField(GetProperty("docsLabelColor"), new GUIContent("Docs Label Color"), true);
            
            EditorGUILayout.Space(3);
            EditorGUILayout.LabelField("Light Colors", labelStyle);
            EditorGUILayout.Space(3);

            EditorGUILayout.PropertyField(GetProperty("labelColorLightTheme"), new GUIContent("Label"), true);
            EditorGUILayout.PropertyField(GetProperty("clickButtonColorLightTheme"), new GUIContent("Click Button Color"), true);
            EditorGUILayout.PropertyField(GetProperty("toggleButtonActiveColorLightTheme"), new GUIContent("Toggle Button Active Color"), true);
            EditorGUILayout.PropertyField(GetProperty("toggleButtonInactiveColorLightTheme"), new GUIContent("Toggle Button Inactive Color"), true);
            EditorGUILayout.PropertyField(GetProperty("boxColorLightTheme"), new GUIContent("Box Color"), true);

            EditorGUILayout.Space(3);
            EditorGUILayout.LabelField("Dark Colors", labelStyle);
            EditorGUILayout.Space(3);

            EditorGUILayout.PropertyField(GetProperty("labelColorDarkTheme"), new GUIContent("Label"), true);
            EditorGUILayout.PropertyField(GetProperty("clickButtonColorDarkTheme"), new GUIContent("Click Button Color"), true);
            EditorGUILayout.PropertyField(GetProperty("toggleButtonActiveColorDarkTheme"), new GUIContent("Toggle Button Active Color"), true);
            EditorGUILayout.PropertyField(GetProperty("toggleButtonInactiveColorDarkTheme"), new GUIContent("Toggle Button Inactive Color"), true);
            EditorGUILayout.PropertyField(GetProperty("boxColorDarkTheme"), new GUIContent("Box Color"), true);

            EditorGUILayout.Space(3);
            EditorGUILayout.LabelField("Other Color", labelStyle);
            EditorGUILayout.Space(3);

            EditorGUILayout.PropertyField(GetProperty("separatorColor"), new GUIContent("Separator Color"), true);

            serializedObject.ApplyModifiedProperties();
        }

        protected SerializedProperty GetProperty(string propertyName)
        {
            string key = propertyName;
            if (SerializedProperties.ContainsKey(key)) return SerializedProperties[key];
            SerializedProperty s = serializedObject.FindProperty(propertyName);
            if (s == null) return null;
            SerializedProperties.Add(key, s);
            return s;
        }
    }
}
#endif