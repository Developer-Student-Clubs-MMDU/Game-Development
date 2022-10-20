#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using VladislavTsurikov.CustomGUI;
using System.Collections.Generic;

namespace VladislavTsurikov.MegaWorldSystem.BrushErase
{
    [ToolEditorAttribute(typeof(BrushEraseTool), "Brush Erase", true, true)]
    public class BrushEraseToolEditor : ToolBaseEditor
    {
		private BrushEraseTool _tool;
		private Clipboard _clipboard = new Clipboard();

		public override void OnEnable() 
		{
			_tool = (BrushEraseTool)target;
		}

		public override void DrawFirstSettings()
		{
			if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedGroup())
			{
				switch (MegaWorldPath.DataPackage.SelectedVariables.SelectedGroup.ResourceType)
				{
					case ResourceType.GameObject:
					{
						UndoEditor.OnGUI();
						break;
					}
				}
			}
		}

		public override void DrawGameObjectGroupSettings(Group group)
		{	
			group.FilterType = (FilterType)CustomEditorGUILayout.EnumPopup(new GUIContent("Filter Type"), group.FilterType);
			switch (MegaWorldPath.DataPackage.SelectedVariables.SelectedGroup.FilterType)
			{
				case FilterType.SimpleFilter:
				{
					group.GetSettings(_tool.GetType(), typeof(SimpleFilterSettings)).OnGUI();
					break;
				}
				case FilterType.MaskFilter:
				{
					CustomEditorGUILayout.HelpBox("\"Mask Filter\" works only with Unity terrain");
					MaskFilterSettings maskFilterSettings = (MaskFilterSettings)group.GetSettings(_tool.GetType(), typeof(MaskFilterSettings));
					maskFilterSettings.OnGUI(group, "Mask Filters Settings");
					break;
				}
			}
		}

		public override void DrawInstantItemGroupSettings(Group group)
		{
			group.FilterType = (FilterType)CustomEditorGUILayout.EnumPopup(new GUIContent("Filter Type"), group.FilterType);
			switch (MegaWorldPath.DataPackage.SelectedVariables.SelectedGroup.FilterType)
			{
				case FilterType.SimpleFilter:
				{
					group.GetSettings(_tool.GetType(), typeof(SimpleFilterSettings)).OnGUI();
					break;
				}
				case FilterType.MaskFilter:
				{
					CustomEditorGUILayout.HelpBox("\"Mask Filter\" works only with Unity terrain");
					MaskFilterSettings maskFilterSettings = (MaskFilterSettings)group.GetSettings(_tool.GetType(), typeof(MaskFilterSettings));
					maskFilterSettings.OnGUI(group, "Mask Filters Settings");
					break;
				}
			}
		}

		public override void DrawUnityTerrainDetailGroupSettings(Group group)
		{
			MaskFilterSettings maskFilterSettings = (MaskFilterSettings)group.GetSettings(_tool.GetType(), typeof(MaskFilterSettings));
			maskFilterSettings.OnGUI(group, "Mask Filters Settings");
		}

		public override void DrawInstantItemPrototypeSettings(PrototypeInstantItem proto)
		{
			proto.GetSettings(_tool.GetType(), typeof(AdditionalEraseSettings)).OnGUI();
		}

		public override void DrawGameObjectPrototypeSettings(PrototypeGameObject proto)
		{
			proto.GetSettings(_tool.GetType(), typeof(AdditionalEraseSettings)).OnGUI();
		}

		public override void DrawUnityTerrainDetailPrototypeSettings(PrototypeTerrainDetail proto)
		{
			proto.GetSettings(_tool.GetType(), typeof(AdditionalEraseSettings)).OnGUI(); 
			proto.GetSettings(typeof(TerrainDetailSettings)).OnGUI();
		}

		public override void DrawToolSettings()
		{
			_toolSettingsFoldout = CustomEditorGUILayout.Foldout(_toolSettingsFoldout, GetNameToolSettings());

			if(_toolSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				BrushEraseToolPath.Settings.EraseStrength = CustomEditorGUILayout.Slider(new GUIContent("Erase Strength"), BrushEraseToolPath.Settings.EraseStrength, 0, 1);

				BrushEraseToolPath.Settings.BrushSettingsForErase.OnGUI("Erase Brush Settings");

				EditorGUI.indentLevel--;
			}
		}

		public override ClipboardBase GetClipboard()
		{
			return _clipboard;
		}
    }
}
#endif