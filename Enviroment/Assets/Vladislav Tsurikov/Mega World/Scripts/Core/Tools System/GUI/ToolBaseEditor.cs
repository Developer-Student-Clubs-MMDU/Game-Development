#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using VladislavTsurikov.CustomGUI;

namespace VladislavTsurikov.MegaWorldSystem
{
    public class ToolBaseEditor
    {
        private Vector2 _windowScrollPos;

        protected bool _groupSettingsFoldout = true;
		protected bool _prototypeSettingsFoldout = true;
        protected bool _toolSettingsFoldout = true;
		protected bool _commonSettingsFoldout = true;

        internal ToolComponent target { get; private set; }

        internal void Init(ToolComponent target)
        {
            this.target = target;
            OnEnable();
        }

        public virtual void OnGUI()
        {
			if(SelectionWindow.Window == null)
			{
				MegaWorldPath.DataPackage.BasicData.OnGUI(GetDrawBasicData(), target.GetType(), GetClipboard(), GetTemplateStackEditor());
			}

			if(MegaWorldPath.DataPackage.SelectedVariables.SelectedGroupList.Count == 0)
            {
                return;
            }

            if(!DrawWarningAboutUnsupportedResourceType())
            {
                return;
            }

#if !INSTANT_RENDERER
			if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedGroup())
            {
                if(MegaWorldPath.DataPackage.SelectedVariables.SelectedGroup.ResourceType == ResourceType.InstantItem)
				{
					CustomEditorGUILayout.HelpBox("Instant Renderer is missing in the project. Instant Renderer is only available by sponsor through Patreon."); 

					GUILayout.BeginHorizontal();
         			{
						GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
						CustomEditorGUILayout.DrawHelpBanner("https://docs.google.com/document/d/1jIPRTMlCR3jsuUrT9CedmDwRC8SsPAf0qc_flbhMOLM/edit#heading=h.1mzix67heftb", "Learn more about Instant Renderer");
						GUILayout.Space(5);
					}
					GUILayout.EndHorizontal();
					
					return;
				}
            }
#endif

			if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedGroup())
			{
				if(!MegaWorldPath.DataPackage.SelectedVariables.SelectedGroup.СontainsPrototype())
				{
					CustomEditorGUILayout.HelpBox("This group does not contain more than one prototype."); 
					DrawResourceController(false);
					return;
				}
				else if(MegaWorldPath.CommonDataPackage.ResourcesControllerEditor.IsSyncError(MegaWorldPath.DataPackage.SelectedVariables.SelectedGroup))
				{
					DrawResourceController(false);
					return;
				}
			}

			_windowScrollPos = EditorGUILayout.BeginScrollView(_windowScrollPos);

            DrawFirstSettings();
			DrawToolSettings();
			DrawResourceController();

			if(this.GetType().GetAttribute<ToolEditorAttribute>().DrawTypeSettings())
			{
				DrawTypeSettings();
			} 

			if(this.GetType().GetAttribute<ToolEditorAttribute>().DrawPrototypeSettings)
			{
				DrawPrototypeSettings();
			} 

			DrawCommonSettings();

			EditorGUILayout.EndScrollView();
        }

		public virtual ClipboardBase GetClipboard()
        {
            return null;
        } 

		public virtual TemplateStackEditor GetTemplateStackEditor()
        {
            return null;
        } 

		public virtual DrawBasicData GetDrawBasicData()
        {
            return new DrawGeneralBasicData();
        } 

		public virtual void DrawCommonSettings()
		{
			if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedGroup())
			{
				switch (MegaWorldPath.DataPackage.SelectedVariables.SelectedGroup.ResourceType)
				{
					case ResourceType.InstantItem:
					case ResourceType.GameObject:
					{
						_commonSettingsFoldout = CustomEditorGUILayout.Foldout(_commonSettingsFoldout, "Common Settings");

						if(_commonSettingsFoldout)
						{
							EditorGUI.indentLevel++;

							LayerSettings layerSettings = MegaWorldPath.CommonDataPackage.layerSettings;
							layerSettings.OnGUI();

							EditorGUI.indentLevel--;
						}

						break;
					}
				}
			}
			else
			{
				_commonSettingsFoldout = CustomEditorGUILayout.Foldout(_commonSettingsFoldout, "Common Settings");

				if(_commonSettingsFoldout)
				{
					EditorGUI.indentLevel++;

					LayerSettings layerSettings = MegaWorldPath.CommonDataPackage.layerSettings;
					layerSettings.OnGUI();

					EditorGUI.indentLevel--;
				}
			}
		}

        public virtual void OnEnable(){}
        public virtual void OnDisable(){}

        public virtual void DrawFirstSettings(){}
        public virtual void DrawToolSettings(){}

		public virtual void DrawGameObjectGroupSettings(Group group){}
		public virtual void DrawInstantItemGroupSettings(Group group){}
		public virtual void DrawUnityTerrainDetailGroupSettings(Group group){}
		public virtual void DrawUnityTerrainTextureGroupSettings(Group group){}

        public virtual void DrawInstantItemPrototypeSettings(PrototypeInstantItem proto){}
        public virtual void DrawGameObjectPrototypeSettings(PrototypeGameObject proto){}
        public virtual void DrawUnityTerrainDetailPrototypeSettings(PrototypeTerrainDetail proto){}
        public virtual void DrawUnityTerrainTexturePrototypeSettings(PrototypeTerrainTexture proto){}

		public void DrawPrototypeSettings()
        {
			if(!MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedGroup())
			{
				return;
			}
			
            if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedProtoInstantItem())
			{
				PrototypeInstantItem proto = MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoInstantItem;

				_prototypeSettingsFoldout = CustomEditorGUILayout.Foldout(_prototypeSettingsFoldout, "Prototype Settings (" + proto.Prefab.name + ")");

				if(_prototypeSettingsFoldout)
				{
					EditorGUI.indentLevel++;

					DrawInstantItemPrototypeSettings(proto);

					EditorGUI.indentLevel--;
				}
			}
            else if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedProtoGameObject())
			{
				PrototypeGameObject proto = MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoGameObject;

				_prototypeSettingsFoldout = CustomEditorGUILayout.Foldout(_prototypeSettingsFoldout, "Prototype Settings (" + proto.Prefab.name + ")");

				if(_prototypeSettingsFoldout)
				{
					EditorGUI.indentLevel++;

					DrawGameObjectPrototypeSettings(proto);

					EditorGUI.indentLevel--;
				}
			}
			
			else if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedProtoTerrainDetail())
			{
				PrototypeTerrainDetail proto = MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoTerrainDetail;

				_prototypeSettingsFoldout = CustomEditorGUILayout.Foldout(_prototypeSettingsFoldout, "Prototype Settings (" + proto.TerrainDetailName + ")" );

				if(_prototypeSettingsFoldout)
				{
					EditorGUI.indentLevel++;

					DrawUnityTerrainDetailPrototypeSettings(proto);		

					EditorGUI.indentLevel--;
				}
			}
			else if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedProtoTerrainTexture())
			{
				PrototypeTerrainTexture proto = MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoTerrainTexture;
				
				_prototypeSettingsFoldout = CustomEditorGUILayout.Foldout(_prototypeSettingsFoldout, "Prototype Settings (" + proto.TerrainTextureName + ")");

				if(_prototypeSettingsFoldout)
				{
					EditorGUI.indentLevel++;

					DrawUnityTerrainTexturePrototypeSettings(proto);

					EditorGUI.indentLevel--;
				}
			}
			else
			{
				CustomEditorGUILayout.HelpBox("Select one prototype to display prototype settings.");
			}
        }

        public void DrawTypeSettings()
        {
            if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedGroup())
			{
                Group group = MegaWorldPath.DataPackage.SelectedVariables.SelectedGroup;

				switch (MegaWorldPath.DataPackage.SelectedVariables.SelectedGroup.ResourceType)
				{
            	    case ResourceType.InstantItem:
					{
						if(this.GetType().GetAttribute<ToolEditorAttribute>().DrawInstantItemTypeSettings)
						{
							_groupSettingsFoldout = CustomEditorGUILayout.Foldout(_groupSettingsFoldout, "Group Settings (" + group.name + ")");

							if(_groupSettingsFoldout)
							{
								EditorGUI.indentLevel++;

								DrawInstantItemGroupSettings(group);

								EditorGUI.indentLevel--;
							}
						}
            	        
            	        break;
					}
					case ResourceType.GameObject:
					{
						if(this.GetType().GetAttribute<ToolEditorAttribute>().DrawGameObjectGroupSettings)
						{
							_groupSettingsFoldout = CustomEditorGUILayout.Foldout(_groupSettingsFoldout, "Group Settings (" + group.name + ")");

							if(_groupSettingsFoldout)
							{
								EditorGUI.indentLevel++;

								DrawGameObjectGroupSettings(group);

								EditorGUI.indentLevel--;
							}
						}
						
            	        break;
					}
					case ResourceType.TerrainDetail:
					{
						if(this.GetType().GetAttribute<ToolEditorAttribute>().DrawUnityTerrainDetailTypeSettings)
						{
							_groupSettingsFoldout = CustomEditorGUILayout.Foldout(_groupSettingsFoldout, "Group Settings (" + group.name + ")");

							if(_groupSettingsFoldout)
							{
								EditorGUI.indentLevel++;

								DrawUnityTerrainDetailGroupSettings(group);

								EditorGUI.indentLevel--;
							}
						}
						
            	        break;
					}
            	    case ResourceType.TerrainTexture:
				    {
						if(this.GetType().GetAttribute<ToolEditorAttribute>().DrawUnityTerrainTextureTypeSettings)
						{
							_groupSettingsFoldout = CustomEditorGUILayout.Foldout(_groupSettingsFoldout, "Group Settings (" + group.name + ")");

							if(_groupSettingsFoldout)
							{
								EditorGUI.indentLevel++;

								DrawUnityTerrainTextureGroupSettings(group);

								EditorGUI.indentLevel--;
							}
						}
						
            	        break;
				    }
				}
			}
			else 
			{
                CustomEditorGUILayout.HelpBox("Select one group to display group settings");   
			}
        }

		public void DrawResourceController(bool drawFoldout = true)
        {
			if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedGroup())
			{
				MegaWorldPath.CommonDataPackage.ResourcesControllerEditor.OnGUI(MegaWorldPath.DataPackage.SelectedVariables.SelectedGroup, drawFoldout);
			}
		}

        public bool DrawWarningAboutUnsupportedResourceType()
        {
            if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedGroup())
			{
				if(!target.IsToolSupportSelectedResourcesType())
                {
					ResourceType[] supportedResourceTypes = target.GetType().GetAttribute<ToolAttribute>().SupportedResourceTypes;

					string text = "";

					for (int i = 0; i < supportedResourceTypes.Length; i++)
					{
						if(i == supportedResourceTypes.Length - 1)
						{
							text += supportedResourceTypes[i].ToString();
						}
						else
						{
							text += supportedResourceTypes[i].ToString() + ", ";
						}
					}

					CustomEditorGUILayout.HelpBox("This tool only works with these Resource Types: " + text); 
					
                    return false;
                }

				return true;
			}
			else
			{
				if(!target.IsToolSupportMultipleTypes())
                {
                    CustomEditorGUILayout.HelpBox("This tool does not support multiple selected types."); 
                    return false;
                }
			}

            return true;
        }

        public string GetNameToolSettings()
        {
            return "Tool Settings (" + GetName() + ")";
        }

        public string GetName()
        {
            return this.GetType().GetAttribute<ToolEditorAttribute>().ContextMenu.Split('/').Last();
        }
    }
}
#endif