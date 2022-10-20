#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using VladislavTsurikov.CustomGUI;

namespace VladislavTsurikov.MegaWorldSystem.SprayBrush
{
	[ToolEditorAttribute(typeof(SprayBrushTool), "Spray Brush", true, true)]
    public class SprayBrushToolEditor : ToolBaseEditor
    {
		private Clipboard _clipboard = new Clipboard();
		private SprayBrushTool _tool;

		public override void OnEnable() 
		{
			_tool = (SprayBrushTool)target;
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

		public override void DrawInstantItemGroupSettings(Group group)
		{	
			SimpleFilterSettings simpleFilterSettings = (SimpleFilterSettings)group.GetSettings(_tool.GetType(), typeof(SimpleFilterSettings));
			simpleFilterSettings.OnGUI("Simple Filter Settings", true);
		}

		public override void DrawGameObjectGroupSettings(Group group)
		{	
			SimpleFilterSettings simpleFilterSettings = (SimpleFilterSettings)group.GetSettings(_tool.GetType(), typeof(SimpleFilterSettings));
			simpleFilterSettings.OnGUI("Simple Filter Settings", true);
		}

		public override void DrawInstantItemPrototypeSettings(PrototypeInstantItem proto)
		{
			proto.SettingsStack.GetSettings(typeof(SuccessSettings)).OnGUI();
			proto.SettingsStack.GetSettings(typeof(OverlapCheckSettings)).OnGUI();

			TransformComponentSettings transformComponentSettings = (TransformComponentSettings)proto.GetSettings(_tool.GetType(), typeof(TransformComponentSettings));
			transformComponentSettings.OnGUI(proto, "Transform Components Settings", true);
		}

        public override void DrawGameObjectPrototypeSettings(PrototypeGameObject proto)
		{
			proto.SettingsStack.GetSettings(typeof(SuccessSettings)).OnGUI();
			proto.SettingsStack.GetSettings(typeof(OverlapCheckSettings)).OnGUI();

			TransformComponentSettings transformComponentSettings = (TransformComponentSettings)proto.GetSettings(_tool.GetType(), typeof(TransformComponentSettings));
			transformComponentSettings.OnGUI(proto, "Transform Components Settings", true);
		}

		public override void DrawToolSettings()
		{
			_toolSettingsFoldout = CustomEditorGUILayout.Foldout(_toolSettingsFoldout, GetNameToolSettings());

			if(_toolSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				SprayBrushToolPath.Settings.BrushSettings.OnGUI();

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