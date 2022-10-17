#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using VladislavTsurikov.CustomGUI;

namespace VladislavTsurikov.MegaWorldSystem.AdvancedBrush
{
	[ToolEditor(typeof(AdvancedBrushTool), "Advanced Brush",  true, true, false, false, true)]
    public class AdvancedBrushToolEditor : ToolBaseEditor
    {
		private CommonClipboard _clipboard = new CommonClipboard();
		private TemplateStackEditor _templateStackEditor = new TemplateStackEditor();

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
					group.GetSettings(typeof(SimpleFilterSettings)).OnGUI();
					break;
				}
				case FilterType.MaskFilter:
				{
					CustomEditorGUILayout.HelpBox("\"Mask Filter\" works only with Unity terrain");
					MaskFilterSettings maskFilterSettings = (MaskFilterSettings)group.GetSettings(typeof(MaskFilterSettings));
					maskFilterSettings.OnGUI(group, "Mask Filters Settings");
					break;
				}
			}

			ScatterSettings scatterSettings = (ScatterSettings)group.GetSettings(typeof(ScatterSettings));
			scatterSettings.OnGUI(group);
		}

		public override void DrawInstantItemGroupSettings(Group group)
		{
			group.FilterType = (FilterType)CustomEditorGUILayout.EnumPopup(new GUIContent("Filter Type"), group.FilterType);
			switch (MegaWorldPath.DataPackage.SelectedVariables.SelectedGroup.FilterType)
			{
				case FilterType.SimpleFilter:
				{
					group.GetSettings(typeof(SimpleFilterSettings)).OnGUI();
					break;
				}
				case FilterType.MaskFilter:
				{
					CustomEditorGUILayout.HelpBox("\"Mask Filter\" works only with Unity terrain");
					MaskFilterSettings maskFilterSettings = (MaskFilterSettings)group.GetSettings(typeof(MaskFilterSettings));
					maskFilterSettings.OnGUI(group, "Mask Filters Settings");
					break;
				}
			}
			ScatterSettings scatterSettings = (ScatterSettings)group.GetSettings(typeof(ScatterSettings));
			scatterSettings.OnGUI(group);
		}

		public override void DrawInstantItemPrototypeSettings(PrototypeInstantItem proto)
		{
			proto.SettingsStack.GetSettings(typeof(SuccessSettings)).OnGUI();

			proto.SettingsStack.GetSettings(typeof(OverlapCheckSettings)).OnGUI();

			TransformComponentSettings transformComponentSettings = (TransformComponentSettings)proto.GetSettings(typeof(TransformComponentSettings));
			transformComponentSettings.OnGUI(proto, "Transform Components Settings");
		}

        public override void DrawGameObjectPrototypeSettings(PrototypeGameObject proto)
		{
			proto.SettingsStack.GetSettings(typeof(SuccessSettings)).OnGUI();

			proto.SettingsStack.GetSettings(typeof(OverlapCheckSettings)).OnGUI();

			TransformComponentSettings transformComponentSettings = (TransformComponentSettings)proto.GetSettings(typeof(TransformComponentSettings));
			transformComponentSettings.OnGUI(proto, "Transform Components Settings");
		}
		
        public override void DrawUnityTerrainDetailPrototypeSettings(PrototypeTerrainDetail proto)
		{
			proto.GetSettings(typeof(SpawnDetailSettings)).OnGUI();
			MaskFilterSettings maskFilterSettings = (MaskFilterSettings)proto.GetSettings(typeof(MaskFilterSettings));
			maskFilterSettings.OnGUI(proto, "Mask Filters Settings");
			TerrainDetailSettings terrainDetailSettings = (TerrainDetailSettings)proto.GetSettings(typeof(TerrainDetailSettings));
			terrainDetailSettings.OnGUI(proto);
		}

        public override void DrawUnityTerrainTexturePrototypeSettings(PrototypeTerrainTexture proto)
		{
			MaskFilterSettings maskFilterSettings = (MaskFilterSettings)proto.GetSettings(typeof(MaskFilterSettings));
			maskFilterSettings.OnGUI(proto, "Mask Filters Settings");
		}

		public override void DrawToolSettings()
		{
			_toolSettingsFoldout = CustomEditorGUILayout.Foldout(_toolSettingsFoldout, GetNameToolSettings());

			if(_toolSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedGroup())
				{
					switch (MegaWorldPath.DataPackage.SelectedVariables.SelectedGroup.ResourceType)
					{
						case ResourceType.InstantItem:
						case ResourceType.GameObject:
						{
							AdvancedBrushToolPath.Settings.EnableFailureRateOnMouseDrag = CustomEditorGUILayout.Toggle(new GUIContent("Enable Failure Rate On Mouse Drag"), AdvancedBrushToolPath.Settings.EnableFailureRateOnMouseDrag);

							if(AdvancedBrushToolPath.Settings.EnableFailureRateOnMouseDrag)
							{
								EditorGUI.indentLevel++;

								AdvancedBrushToolPath.Settings.FailureRate = CustomEditorGUILayout.Slider(new GUIContent("Failure Rate (%)"), AdvancedBrushToolPath.Settings.FailureRate, 0f, 100f);

								EditorGUI.indentLevel--;
							}

							break;
						}
						case ResourceType.TerrainTexture:
						{
							AdvancedBrushToolPath.Settings.TextureTargetStrength = CustomEditorGUILayout.Slider(new GUIContent("Target Strength"), AdvancedBrushToolPath.Settings.TextureTargetStrength, 0, 1);

							break;
						}
					}
				}

				AdvancedBrushToolPath.Settings.BrushSettings.OnGUI("Brush Settings");

				EditorGUI.indentLevel--;
			}
		}

		public override ClipboardBase GetClipboard()
		{
			return _clipboard;
		}

		public override TemplateStackEditor GetTemplateStackEditor()
		{
			return _templateStackEditor;
		}
    }
}
#endif