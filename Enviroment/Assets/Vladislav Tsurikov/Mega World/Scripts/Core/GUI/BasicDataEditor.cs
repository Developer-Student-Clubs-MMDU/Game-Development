#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using VladislavTsurikov.CustomGUI;
using VladislavTsurikov.Extensions;

namespace VladislavTsurikov.MegaWorldSystem
{
	[Serializable]
    public class BasicDataEditor 
    {
		public void OnGUI(BasicData basicData,  DrawBasicData drawBasicData, Type toolType, ClipboardBase clipboard, TemplateStackEditor templateStackEditor)
		{
			drawBasicData.OnGUI(basicData, toolType, clipboard, templateStackEditor);

			if(basicData.SelectedVariables.HasOneSelectedGroup())
			{
				RenameGroupWindowGUI(basicData.SelectedVariables.SelectedGroup); 
			}
		}

		public static void RenameGroupWindowGUI(Group group) 
		{
			if(group.Renaming == false)
			{
				return;
			}

			GUIStyle barStyle = CustomEditorGUILayout.GetStyle(StyleName.ActiveBar);
			GUIStyle labelStyle = CustomEditorGUILayout.GetStyle(StyleName.LabelButton);
			GUIStyle labelTextStyle = CustomEditorGUILayout.GetStyle(StyleName.LabelText);

			Color InitialGUIColor = GUI.color;

			GUILayout.BeginHorizontal();
			{
				GUILayout.Space(5);

				if (EditorGUIUtility.isProSkin)
                {
                    CustomEditorGUILayout.Button(group.Name, labelStyle, barStyle, EditorColors.Instance.orangeNormal, EditorColors.Instance.orangeDark.WithAlpha(0.3f), 20);
                }
                else
                {
                    CustomEditorGUILayout.Button(group.Name, labelStyle, barStyle, EditorColors.Instance.orangeDark, EditorColors.Instance.orangeNormal.WithAlpha(0.3f), 20);
                }				

				GUILayout.Space(5);
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginVertical();
			{
                GUILayout.BeginHorizontal();
                {
					GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace() + 15);
					
					GUILayout.Label(new GUIContent("Rename to"), labelTextStyle);

					GUI.color = EditorColors.Instance.orangeNormal;

                    group.RenamingName = EditorGUILayout.TextField(GUIContent.none, group.RenamingName); //rename to field
					
                    GUI.color = InitialGUIColor;

                    if (CustomEditorGUILayout.DrawIcon(StyleName.IconButtonOk, EditorColors.Instance.Green) || Event.current.keyCode == KeyCode.Return && Event.current.type == EventType.KeyUp) //rename OK button
                    {
						group.Name = group.RenamingName;
						group.Renaming = false;

						string assetPath =  AssetDatabase.GetAssetPath(group);
 						AssetDatabase.RenameAsset(assetPath, group.RenamingName);
 						AssetDatabase.SaveAssets();

						Event.current.Use();
                    }

                    if (CustomEditorGUILayout.DrawIcon(StyleName.IconButtonCancel, EditorColors.Instance.Red)) //rename CANCEL button
                    {
						group.RenamingName = group.Name;
						group.Renaming = false;

                        Event.current.Use();
                    }

					GUILayout.Space(5);
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(2);
			}
			GUILayout.EndVertical();
			GUI.color = InitialGUIColor;
		}
	}
}
#endif