#if UNITY_EDITOR
#if INSTANT_RENDERER
using UnityEngine;
using VladislavTsurikov.CustomGUI;

namespace VladislavTsurikov.MegaWorldSystem 
{
    public static class InstantRendererControllerEditor 
    {
        public static void OnGUI(Group group)
		{
			InstantRendererController.DetectSyncError(group);

			switch (InstantRendererController.SyncError)
			{
				case InstantRendererController.InstantRendererSyncError.InstantRendererNull:
				{
					CustomEditorGUILayout.WarningBox("There is no Instant Renderer in the scene. Click the button \"Create Instant Renderer\"");

					GUILayout.BeginHorizontal();
         			{
						GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
						if(CustomEditorGUILayout.ClickButton("Create Instant Renderer", ButtonStyle.Add, ButtonSize.ClickButton))
						{
							InstantRendererController.CreateInstantRenderer();
						}
						GUILayout.Space(5);
					}
					GUILayout.EndHorizontal();
					break;
				}
				case InstantRendererController.InstantRendererSyncError.StorageTerrainCellsNull:
				{
					CustomEditorGUILayout.WarningBox("There is no Storage Terrain Cells in the scene. Click the button \"Add Storage Terrain Cells\"");

					GUILayout.BeginHorizontal();
         			{
						GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
						if(CustomEditorGUILayout.ClickButton("Add StorageTerrainCells", ButtonStyle.Add, ButtonSize.ClickButton))
						{
							InstantRendererController.AddStorageTerrainCells();
						}
						GUILayout.Space(5);
					}
					GUILayout.EndHorizontal();

					break;
				}
				case InstantRendererController.InstantRendererSyncError.NotAllProtoAvailable:
				{
					CustomEditorGUILayout.WarningBox("You need all prototypes of this group to be in Instant Renderer.");

					GUILayout.BeginHorizontal();
         			{
						GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
						if(CustomEditorGUILayout.ClickButton("Get/Update Resources", ButtonStyle.General, ButtonSize.ClickButton))
						{
							InstantRendererController.UpdateInstantRenderer(group);
						}
						GUILayout.Space(5);
					}
					GUILayout.EndHorizontal();

					GUILayout.Space(3);

					GUILayout.BeginHorizontal();
         			{
						GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
						if(CustomEditorGUILayout.ClickButton("Add Missing Resources", ButtonStyle.Add, ButtonSize.ClickButton))
						{
							InstantRendererController.AddMissingInstantPrototype(group.ProtoInstantItemList);
						}

						GUILayout.Space(2);

						if(CustomEditorGUILayout.ClickButton("Remove All Resources", ButtonStyle.Remove))
						{
							InstantRendererController.RemoveAllInstantPrototype();
						}

						GUILayout.Space(5);
					}
					GUILayout.EndHorizontal();
					break;
				}
				case InstantRendererController.InstantRendererSyncError.None:
				{
					CustomEditorGUILayout.HelpBox("Mega World will spawn in \"Storage Terrain Cells\".");

					GUILayout.BeginHorizontal();
         			{
						GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
						if(CustomEditorGUILayout.ClickButton("Get/Update Resources", ButtonStyle.General, ButtonSize.ClickButton))
						{
							InstantRendererController.UpdateInstantRenderer(group);
						}
						GUILayout.Space(5);
					}
					GUILayout.EndHorizontal();

					GUILayout.Space(3);

					GUILayout.BeginHorizontal();
         			{
						GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
						if(CustomEditorGUILayout.ClickButton("Add Missing Resources", ButtonStyle.Add, ButtonSize.ClickButton))
						{
							InstantRendererController.AddMissingInstantPrototype(group.ProtoInstantItemList);
						}

						GUILayout.Space(2);

						if(CustomEditorGUILayout.ClickButton("Remove All Resources", ButtonStyle.Remove))
						{
							InstantRendererController.RemoveAllInstantPrototype();
						}

						GUILayout.Space(5);
					}
					GUILayout.EndHorizontal();
					break;
				}
				case InstantRendererController.InstantRendererSyncError.MissingPrototypes:
				{
					GUILayout.BeginHorizontal();
         			{
						GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
						if(CustomEditorGUILayout.ClickButton("Get/Update Resources", ButtonStyle.General, ButtonSize.ClickButton))
						{
							InstantRendererController.UpdateInstantRenderer(group);
						}
						GUILayout.Space(5);
					}
					GUILayout.EndHorizontal();

					break;
				}
			}
		}
    }
}
#endif
#endif