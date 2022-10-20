#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.CustomGUI;

namespace VladislavTsurikov.MegaWorldSystem 
{
    [Serializable]
    public class ResourcesControllerEditor
    {
        public bool ResourcesControllerFoldout = false;

        public void OnGUI(Group group, bool drawFoldout = true)
		{
			if(drawFoldout)
			{
				ResourcesControllerFoldout = CustomEditorGUILayout.Foldout(ResourcesControllerFoldout, "Resources Controller (" + SelectionResourcesType.GetName(group.ResourceType) + ")");

				if(ResourcesControllerFoldout)
				{
					EditorGUI.indentLevel++;

					DrawResourcesController(group);

					EditorGUI.indentLevel--;
				}
			}
            else
			{
				switch (group.ResourceType)
				{
					case ResourceType.InstantItem:
					{
#if INSTANT_RENDERER
						InstantRendererControllerEditor.OnGUI(group);
#endif

						break;
					}
					case ResourceType.TerrainDetail:
            	    case ResourceType.TerrainTexture:
					{
						TerrainResourcesControllerEditor.OnGUI(group);

						break;
					}
				}
			}
        }

		public void DrawResourcesController(Group group)
		{
			switch (group.ResourceType)
			{
				case ResourceType.InstantItem:
				{
#if INSTANT_RENDERER
					InstantRendererControllerEditor.OnGUI(group);
#endif

					break;
				}
				case ResourceType.GameObject:
				{
        			GameObjectControllerEditor.OnGUI();

					break;
				}
				case ResourceType.TerrainDetail:
                case ResourceType.TerrainTexture:
				{
					TerrainResourcesControllerEditor.OnGUI(group);

					break;
				}
			}
		}

		public bool IsSyncError(Group group)
		{
			switch (group.ResourceType)
			{
				case ResourceType.InstantItem:
				{
#if !INSTANT_RENDERER
					return true;
#else
					InstantRendererController.DetectSyncError(group);

					if(InstantRendererController.SyncError != InstantRendererController.InstantRendererSyncError.None)
					{
						return true;
					}
#endif

					break;
				}
				case ResourceType.TerrainDetail:
                case ResourceType.TerrainTexture:
				{
					TerrainResourcesController.DetectSyncError(group, Terrain.activeTerrain);

					if(TerrainResourcesController.SyncError != TerrainResourcesController.TerrainResourcesSyncError.None)
					{
						return true;
					}

					break;
				}
			}

			return false;
		}
    }
}
#endif
