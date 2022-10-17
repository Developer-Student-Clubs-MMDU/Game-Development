#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov;
using VladislavTsurikov.CustomGUI;

namespace VladislavTsurikov.MegaWorldSystem.Stamper
{
    [CustomEditor(typeof(StamperTool))]
    public class StamperToolEditor : Editor
    {
		private StamperTool _stamperTool;
		private CommonClipboard _clipboard = new CommonClipboard();
		private TemplateStackEditor _templateStackEditor = new TemplateStackEditor();

        #region UI Settings
		private string _stamperToolName = "Stamper Tool";
		private bool _groupSettingsFoldout = true;
		private bool _prototypeSettingsFoldout = true;
        private bool _stamperToolControllerFoldout = true;
		private bool _commonSettingsFoldout = true;
		private bool _toolSettingsFoldout = true;
		#endregion

        void OnEnable()
        {
            _stamperTool = (StamperTool)target;

            if(_stamperTool.Area.Bounds == null)
            {
                _stamperTool.Area.SetAreaBounds(_stamperTool);
            }
        }

        public override void OnInspectorGUI()
        {
			InternalDragAndDrop.OnBeginGUI();

			OnGUI();

			InternalDragAndDrop.OnEndGUI();

            // repaint every time for dinamic effects like drag scrolling
            if(InternalDragAndDrop.IsDragging() || InternalDragAndDrop.IsDragPerform())
			{
				Repaint();
			}
        }

		public void OnGUI()
        {
			_stamperTool.Data.SelectedVariables.DeleteNullValueIfNecessary(_stamperTool.Data.GroupList);
            _stamperTool.Data.SelectedVariables.SetAllSelectedParameters(_stamperTool.Data.GroupList);

			CustomEditorGUILayout.IsInspector = true;

			_stamperTool.Data.OnGUI(new DrawGeneralBasicData(), typeof(StamperTool), _clipboard, _templateStackEditor);

			if(_stamperTool.Data.SelectedVariables.SelectedGroupList.Count == 0)
            {
                return;
            }

#if !INSTANT_RENDERER
			if(_stamperTool.Data.SelectedVariables.HasOneSelectedGroup())
            {
                if(_stamperTool.Data.SelectedVariables.SelectedGroup.ResourceType == ResourceType.InstantItem)
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

			if(_stamperTool.Data.SelectedVariables.HasOneSelectedGroup())
			{
				if(!_stamperTool.Data.SelectedVariables.SelectedGroup.СontainsPrototype())
				{
					CustomEditorGUILayout.HelpBox("This group does not contain more than one prototype."); 
					DrawResourceController(false);
					return;
				}
				else if(MegaWorldPath.CommonDataPackage.ResourcesControllerEditor.IsSyncError(_stamperTool.Data.SelectedVariables.SelectedGroup))
				{
					DrawResourceController(false);
					return;
				}
			}

			DrawToolSettings();
			DrawResourceController();
			DrawTypeSettings();
			DrawPrototypeSettings();

			DrawCommonSettings();

			EditorUtility.SetDirty(_stamperTool);
			_stamperTool.Data.SaveAllData();
        }

		public void DrawTypeSettings()
        {
            if(_stamperTool.Data.SelectedVariables.HasOneSelectedGroup())
			{
                Group group = _stamperTool.Data.SelectedVariables.SelectedGroup;

				switch (_stamperTool.Data.SelectedVariables.SelectedGroup.ResourceType)
				{
            	    case ResourceType.InstantItem:
					{
						_groupSettingsFoldout = CustomEditorGUILayout.Foldout(_groupSettingsFoldout, "Group Settings (" + group.name + ")");

						if(_groupSettingsFoldout)
						{
							EditorGUI.indentLevel++;
							DrawInstantItemGroupSettings(group);
							EditorGUI.indentLevel--;
						}
            	        
            	        break;
					}
					case ResourceType.GameObject:
					{
						_groupSettingsFoldout = CustomEditorGUILayout.Foldout(_groupSettingsFoldout, "Group Settings (" + group.name + ")");

						if(_groupSettingsFoldout)
						{
							EditorGUI.indentLevel++;
							DrawGameObjectGroupSettings(group);
							EditorGUI.indentLevel--;
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

		public void DrawGameObjectGroupSettings(Group group)
		{	
			CustomEditorGUILayout.BeginChangeCheck();

			group.GenerateRandomSeed = CustomEditorGUILayout.Toggle(new GUIContent("Generate Random Seed"), group.GenerateRandomSeed);
			if(group.GenerateRandomSeed == false)
			{
				EditorGUI.indentLevel++;
				
				group.RandomSeed = CustomEditorGUILayout.IntField(new GUIContent("Random Seed"), group.RandomSeed);

				EditorGUI.indentLevel--;
			}

			group.FilterType = (FilterType)CustomEditorGUILayout.EnumPopup(new GUIContent("Filter Type"), group.FilterType);
			switch (_stamperTool.Data.SelectedVariables.SelectedGroup.FilterType)
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
					DrawMaskFilterSettings(maskFilterSettings, group);
					break;
				}
			}

			ScatterSettings scatterSettings = (ScatterSettings)group.GetSettings(typeof(ScatterSettings));
			scatterSettings.OnGUI(group);

			if(CustomEditorGUILayout.EndChangeCheck())
			{
				_stamperTool.AutoRespawnController.StartAutoRespawn(_stamperTool.Data.SelectedVariables.SelectedGroup, _stamperTool);
			}
		}

		public void DrawInstantItemGroupSettings(Group group)
		{
			CustomEditorGUILayout.BeginChangeCheck();

			group.GenerateRandomSeed = CustomEditorGUILayout.Toggle(new GUIContent("Generate Random Seed"), group.GenerateRandomSeed);
			if(group.GenerateRandomSeed == false)
			{
				EditorGUI.indentLevel++;
				
				group.RandomSeed = CustomEditorGUILayout.IntField(new GUIContent("Random Seed"), group.RandomSeed);

				EditorGUI.indentLevel--;
			}

			group.FilterType = (FilterType)CustomEditorGUILayout.EnumPopup(new GUIContent("Filter Type"), group.FilterType);
			switch (_stamperTool.Data.SelectedVariables.SelectedGroup.FilterType)
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
					DrawMaskFilterSettings(maskFilterSettings, group);
					break;
				}
			}
			ScatterSettings scatterSettings = (ScatterSettings)group.GetSettings(typeof(ScatterSettings));
			scatterSettings.OnGUI(group);

			if(CustomEditorGUILayout.EndChangeCheck())
			{
				_stamperTool.AutoRespawnController.StartAutoRespawn(_stamperTool.Data.SelectedVariables.SelectedGroup, _stamperTool);
			}
		}

		public void DrawPrototypeSettings()
        {
			if(!_stamperTool.Data.SelectedVariables.HasOneSelectedGroup())
			{
				return;
			}
			
            if(_stamperTool.Data.SelectedVariables.HasOneSelectedProtoInstantItem())
			{
				PrototypeInstantItem proto = _stamperTool.Data.SelectedVariables.SelectedProtoInstantItem;

				_prototypeSettingsFoldout = CustomEditorGUILayout.Foldout(_prototypeSettingsFoldout, "Prototype Settings (" + proto.Prefab.name + ")");

				if(_prototypeSettingsFoldout)
				{
					EditorGUI.indentLevel++;

					DrawInstantItemPrototypeSettings(proto);

					EditorGUI.indentLevel--;
				}
			}
            else if(_stamperTool.Data.SelectedVariables.HasOneSelectedProtoGameObject())
			{
				PrototypeGameObject proto = _stamperTool.Data.SelectedVariables.SelectedProtoGameObject;

				_prototypeSettingsFoldout = CustomEditorGUILayout.Foldout(_prototypeSettingsFoldout, "Prototype Settings (" + proto.Prefab.name + ")");

				if(_prototypeSettingsFoldout)
				{
					EditorGUI.indentLevel++;

					DrawGameObjectPrototypeSettings(proto);

					EditorGUI.indentLevel--;
				}
			}
			
			else if(_stamperTool.Data.SelectedVariables.HasOneSelectedProtoTerrainDetail())
			{
				PrototypeTerrainDetail proto = _stamperTool.Data.SelectedVariables.SelectedProtoTerrainDetail;

				_prototypeSettingsFoldout = CustomEditorGUILayout.Foldout(_prototypeSettingsFoldout, "Prototype Settings (" + proto.TerrainDetailName + ")" );

				if(_prototypeSettingsFoldout)
				{
					EditorGUI.indentLevel++;

					DrawUnityTerrainDetailPrototypeSettings(proto);		

					EditorGUI.indentLevel--;
				}
			}
			else if(_stamperTool.Data.SelectedVariables.HasOneSelectedProtoTerrainTexture())
			{
				PrototypeTerrainTexture proto = _stamperTool.Data.SelectedVariables.SelectedProtoTerrainTexture;
				
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

		public void DrawInstantItemPrototypeSettings(PrototypeInstantItem proto)
		{
			CustomEditorGUILayout.BeginChangeCheck();

			proto.SettingsStack.GetSettings(typeof(SuccessSettings)).OnGUI();
			proto.SettingsStack.GetSettings(typeof(OverlapCheckSettings)).OnGUI();
			TransformComponentSettings transformComponentSettings = (TransformComponentSettings)proto.GetSettings(typeof(TransformComponentSettings));
			transformComponentSettings.OnGUI(proto, "Transform Components Settings");

			if(CustomEditorGUILayout.EndChangeCheck())
			{
				_stamperTool.AutoRespawnController.StartAutoRespawn(_stamperTool.Data.SelectedVariables.SelectedGroup, _stamperTool);
			}
		}

        public void DrawGameObjectPrototypeSettings(PrototypeGameObject proto)
		{
			CustomEditorGUILayout.BeginChangeCheck();

			proto.SettingsStack.GetSettings(typeof(SuccessSettings)).OnGUI();
			proto.SettingsStack.GetSettings(typeof(OverlapCheckSettings)).OnGUI();
			TransformComponentSettings transformComponentSettings = (TransformComponentSettings)proto.GetSettings(typeof(TransformComponentSettings));
			transformComponentSettings.OnGUI(proto, "Transform Components Settings");

			if(CustomEditorGUILayout.EndChangeCheck())
			{
				_stamperTool.AutoRespawnController.StartAutoRespawn(_stamperTool.Data.SelectedVariables.SelectedGroup, _stamperTool);
			}
		}
		
        public void DrawUnityTerrainDetailPrototypeSettings(PrototypeTerrainDetail proto)
		{
			CustomEditorGUILayout.BeginChangeCheck();

			proto.GetSettings(typeof(SpawnDetailSettings)).OnGUI();
			MaskFilterSettings maskFilterSettings = (MaskFilterSettings)proto.GetSettings(typeof(MaskFilterSettings));
			DrawMaskFilterSettings(maskFilterSettings, proto);
			
			if(CustomEditorGUILayout.EndChangeCheck())
			{
				_stamperTool.AutoRespawnController.StartAutoRespawn(proto, _stamperTool);
			}

			TerrainDetailSettings terrainDetailSettings = (TerrainDetailSettings)proto.GetSettings(typeof(TerrainDetailSettings));
			terrainDetailSettings.OnGUI(proto);
		}

        public void DrawUnityTerrainTexturePrototypeSettings(PrototypeTerrainTexture proto)
		{
			CustomEditorGUILayout.BeginChangeCheck();

			MaskFilterSettings maskFilterSettings = (MaskFilterSettings)proto.GetSettings(typeof(MaskFilterSettings));
			DrawMaskFilterSettings(maskFilterSettings, proto);

			if(CustomEditorGUILayout.EndChangeCheck())
			{
				_stamperTool.AutoRespawnController.StartAutoRespawn(_stamperTool.Data.SelectedVariables.SelectedGroup, _stamperTool);
			}
		}

		public void DrawToolSettings()
		{
			_toolSettingsFoldout = CustomEditorGUILayout.Foldout(_toolSettingsFoldout, GetNameToolSettings());

			if(_toolSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				_stamperTool.Area.OnGUI(_stamperTool);
				StamperToolControllerWindowGUI();

				EditorGUI.indentLevel--;
			}
		}

		public void DrawResourceController(bool drawFoldout = true)
        {
			if(_stamperTool.Data.SelectedVariables.HasOneSelectedGroup())
			{
				MegaWorldPath.CommonDataPackage.ResourcesControllerEditor.OnGUI(_stamperTool.Data.SelectedVariables.SelectedGroup, drawFoldout);
			}
		}

		public string GetNameToolSettings()
        {
            return "Tool Settings (" + _stamperToolName + ")";
        }

        public void StamperToolControllerWindowGUI()
		{
			_stamperToolControllerFoldout = CustomEditorGUILayout.Foldout(_stamperToolControllerFoldout, "Stamper Tool Controller");

			if(_stamperToolControllerFoldout)
			{
				EditorGUI.indentLevel++;

				_stamperTool.StamperToolControllerSettings.Visualisation = CustomEditorGUILayout.Toggle(visualisation, _stamperTool.StamperToolControllerSettings.Visualisation);

				if(_stamperTool.Area.UseSpawnCells == false)
				{
					_stamperTool.StamperToolControllerSettings.AutoRespawn = CustomEditorGUILayout.Toggle(autoRespawn, _stamperTool.StamperToolControllerSettings.AutoRespawn);

					if(_stamperTool.StamperToolControllerSettings.AutoRespawn)
					{
						EditorGUI.indentLevel++;
						_stamperTool.StamperToolControllerSettings.DelayAutoRespawn = CustomEditorGUILayout.Slider(delayAutoSpawn, _stamperTool.StamperToolControllerSettings.DelayAutoRespawn, 0, 3);
						EditorGUI.indentLevel--;
						
						GUILayout.BeginHorizontal();
         				{
							GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
							if(CustomEditorGUILayout.ClickButton("Respawn", ButtonStyle.Add, ButtonSize.ClickButton))
							{
								Unspawn.UnspawnAllProto(_stamperTool.Data.GroupList, false);
								_stamperTool.Spawn();
							}
							GUILayout.Space(5);
						}
						GUILayout.EndHorizontal();

						GUILayout.Space(3);
					}
					else
					{
						DrawSpawnControls();
					}
				}
				else
				{
					CustomEditorGUILayout.HelpBox("Auto Spawn does not support when \"Use Spawn Cells\" is enabled in \"Area Settings\".");
	
					DrawSpawnWithCellsControls();
				}

				if (_stamperTool.SpawnProgress == 0)
				{
					if(_stamperTool.Data.SelectedVariables.SelectedProtoGameObjectList.Count != 0 || _stamperTool.Data.SelectedVariables.SelectedProtoTerrainDetailList.Count != 0
						|| _stamperTool.Data.SelectedVariables.SelectedProtoInstantItemList.Count != 0)
					{
						GUILayout.BeginHorizontal();
         				{
							GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
							if(CustomEditorGUILayout.ClickButton("Unspawn Selected Prototypes", ButtonStyle.Remove, ButtonSize.ClickButton))
							{
								if (EditorUtility.DisplayDialog("WARNING!",
									"Are you sure you want to remove all resource instances that have been selected from the scene?",
									"OK", "Cancel"))
								{
									Unspawn.UnspawnAllProto(_stamperTool.Data.SelectedVariables.SelectedGroupList, true);
								}
							}

							GUILayout.Space(5);
						}
						GUILayout.EndHorizontal();

						GUILayout.Space(3);
					}
				}

				EditorGUI.indentLevel--;
			}
		}

		private void DrawSpawnControls()
        {
            if (_stamperTool.SpawnProgress > 0f && _stamperTool.SpawnProgress < 1f)
           	{
				GUILayout.BeginHorizontal();
         		{
					GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
					if(CustomEditorGUILayout.ClickButton("Cancel", ButtonStyle.Remove))
					{
						CancelSpawn();
					}
					GUILayout.Space(5);
				}
				GUILayout.EndHorizontal();

				GUILayout.Space(3);
           	}
           	else
           	{
				if(_stamperTool.Data.SelectedVariables.SelectedProtoTerrainTextureList.Count == 0)
				{
					GUILayout.BeginHorizontal();
         			{
						GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
						if(CustomEditorGUILayout.ClickButton("Respawn", ButtonStyle.Add, ButtonSize.ClickButton))
						{
							Unspawn.UnspawnAllProto(_stamperTool.Data.GroupList, false);
							_stamperTool.Spawn();
						}

						GUILayout.Space(5);
					}
					GUILayout.EndHorizontal();

					GUILayout.Space(3);
				}

				GUILayout.BeginHorizontal();
         		{
					GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
					if(CustomEditorGUILayout.ClickButton("Spawn", ButtonStyle.Add))
					{
						_stamperTool.Spawn();
					}
					GUILayout.Space(5);
				}
				GUILayout.EndHorizontal();

				GUILayout.Space(3);
           	}
        }

		private void DrawSpawnWithCellsControls()
        {
			if (_stamperTool.SpawnProgress > 0f && _stamperTool.SpawnProgress < 1f)
           	{
				GUILayout.BeginHorizontal();
         		{
					GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
					if(CustomEditorGUILayout.ClickButton("Cancel", ButtonStyle.Remove))
					{
						CancelSpawn();
					}
					GUILayout.Space(5);
				}
				GUILayout.EndHorizontal();

				GUILayout.Space(3);
           	}
           	else
           	{
				if(_stamperTool.Data.SelectedVariables.SelectedProtoTerrainTextureList.Count == 0)
				{
					GUILayout.BeginHorizontal();
         			{
						GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
						if(CustomEditorGUILayout.ClickButton("Refresh", ButtonStyle.Add, ButtonSize.ClickButton))
						{
							if(_stamperTool.Area.CellList.Count == 0)
							{
								_stamperTool.Area.CreateCells();
							}

							Unspawn.UnspawnAllProto(_stamperTool.Data.GroupList, false);
							_stamperTool.SpawnWithCells(_stamperTool.Area.CellList);
						}
	
						GUILayout.Space(5);
					}
					GUILayout.EndHorizontal();
	
					GUILayout.Space(3);
				}

				GUILayout.BeginHorizontal();
         		{
					GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
					if(CustomEditorGUILayout.ClickButton("Spawn", ButtonStyle.Add))
					{
						if(_stamperTool.Area.CellList.Count == 0)
						{
							_stamperTool.Area.CreateCells();
						}

						_stamperTool.SpawnWithCells(_stamperTool.Area.CellList);
					}
					GUILayout.Space(5);
				}
				GUILayout.EndHorizontal();

				GUILayout.Space(3);
           	}
        }

		public virtual void DrawCommonSettings()
		{
			if(_stamperTool.Data.SelectedVariables.HasOneSelectedGroup())
			{
				switch (_stamperTool.Data.SelectedVariables.SelectedGroup.ResourceType)
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

		public void DrawMaskFilterSettings(MaskFilterSettings maskFilterSettings, ScriptableObject asset)
		{
			EditorGUI.BeginChangeCheck();

			maskFilterSettings.OnGUI(asset, "Mask Filters Settings");
		
			if(EditorGUI.EndChangeCheck())
			{
				CustomEditorGUILayout.ChangeCheck = true;
				_stamperTool.StamperVisualisation.UpdateMask = true;
			}
		}

        public void CancelSpawn()
        {
            _stamperTool.CancelSpawn = true;
            _stamperTool.SpawnComplete = true;
            _stamperTool.SpawnProgress = 0f;
			EditorUtility.ClearProgressBar();
        }

        [MenuItem("GameObject/MegaWorld/Add Stamper", false, 14)]
    	public static void AddStamper(MenuCommand menuCommand)
    	{
    		GameObject stamper = new GameObject("Stamper");
            stamper.transform.localScale = new Vector3(150, 150, 150);
    		stamper.AddComponent<StamperTool>();
    		Undo.RegisterCreatedObjectUndo(stamper, "Created " + stamper.name);
    		Selection.activeObject = stamper;
    	}

		[NonSerialized]
		public GUIContent visualisation = new GUIContent("Visualisation", "Allows you to see the Mask Filter Settings visualization.");
		[NonSerialized]
		public GUIContent autoRespawn = new GUIContent("Auto Respawn", "Allows you to do automatic deletion and then spawn when you changed the settings.");
		[NonSerialized]
		public GUIContent delayAutoSpawn = new GUIContent("Delay Auto Spawn", "Respawn delay in seconds.");
    }
}
#endif