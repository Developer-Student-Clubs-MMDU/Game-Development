#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.CustomGUI;

namespace VladislavTsurikov.MegaWorldSystem.Stamper
{
    [Serializable]
    public class AreaEditor
    {
        public void OnGUI(StamperTool stamper, Area area)
        {
            DrawAreaSettings(area, stamper);
        }

        public void DrawAreaHandlesSettings(Area area)
    	{
    		area.HandlesSettingsFoldout = CustomEditorGUILayout.Foldout(area.HandlesSettingsFoldout, "Handles Settings");

    		if(area.HandlesSettingsFoldout)
    		{
                EditorGUI.indentLevel++;

                area.HandleSettingsMode = (HandleSettingsMode)CustomEditorGUILayout.EnumPopup(handleSettingsMode, area.HandleSettingsMode);

                if (area.HandleSettingsMode == HandleSettingsMode.Custom)
                {
                    area.ColorCube = CustomEditorGUILayout.ColorField(colorCube, area.ColorCube);
                    area.PixelWidth = CustomEditorGUILayout.Slider(pixelWidth, area.PixelWidth, 1f, 5f);
                    area.Dotted = CustomEditorGUILayout.Toggle(dotted, area.Dotted);
                }

                area.DrawHandleIfNotSelected = CustomEditorGUILayout.Toggle(drawHandleIfNotSelected, area.DrawHandleIfNotSelected);

                EditorGUI.indentLevel--;
            }
    	}

        public void DrawAreaSettings(Area area, StamperTool stamper)
        {
    		area.AreaSettingsFoldout = CustomEditorGUILayout.Foldout(area.AreaSettingsFoldout, "Area Settings");

    		if(area.AreaSettingsFoldout)
    		{
    			EditorGUI.indentLevel++;

    			GUILayout.BeginHorizontal();
            	{
            	    GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
            	    if(CustomEditorGUILayout.ClickButton("Fit To Terrain Size"))
    			    {
    			    	area.FitToTerrainSize(stamper);
    			    }
            	    GUILayout.Space(3);
            	}
            	GUILayout.EndHorizontal();

    			GUILayout.Space(3);

				area.UseSpawnCells = CustomEditorGUILayout.Toggle(new GUIContent("Use Spawn Cells"), area.UseSpawnCells);

				if(area.UseSpawnCells)
				{
					CustomEditorGUILayout.HelpBox("It is recommended to enable \"Use Spawn Cells\" when your terrain is more than 4 km * 4 km. This parameter creates smaller cells, \"Stamper Tool\" will spawn each cell in turn. Why this parameter is needed, too long spawn delay can disable Unity.");

					GUILayout.BeginHorizontal();
            		{
            		    GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
            		    if(CustomEditorGUILayout.ClickButton("Refresh Cells"))
    				    {
    				    	area.CreateCells();
    				    }
            		    GUILayout.Space(3);
            		}
            		GUILayout.EndHorizontal();

    				GUILayout.Space(3);

					area.CellSize = CustomEditorGUILayout.FloatField(cellSize, area.CellSize);
					CustomEditorGUILayout.Label(new GUIContent("Cell Count: " + area.CellList.Count));
					area.ShowCells = CustomEditorGUILayout.Toggle(showCells, area.ShowCells); 
				}
				else
				{
					area.UseMask = CustomEditorGUILayout.Toggle(new GUIContent("Use Mask"), area.UseMask);

            		if(area.UseMask)
            		{
                	    area.MaskType = (MaskType)CustomEditorGUILayout.EnumPopup(new GUIContent("Mask Type"), area.MaskType);

            		    switch (area.MaskType)
			    	    {
			    	    	case MaskType.Custom:
			    	    	{
			    	    		area.CustomMasks.OnGUI();

			    	    		break;
			    	    	}
			    	    	case MaskType.Procedural:
			    	    	{
			    	    		area.ProceduralMask.OnGUI();

			    	    		break;
			    	    	}
			    	    }
            		}
				}

    			DrawAreaHandlesSettings(area);

    			EditorGUI.indentLevel--;
    		}
        }

		public GUIContent cellSize = new GUIContent("Cell Size", "Sets the cell size in meters.");
		public GUIContent showCells = new GUIContent("Show Cells", "Shows all available cells."); 

		public GUIContent handleSettingsMode = new GUIContent("Handle Settings Mode", "Allows you to choose the settings for how the Area will be displayed.");
		public GUIContent colorCube = new GUIContent("Color Cube", "Area color");
		public GUIContent pixelWidth = new GUIContent("Pixel Width", "How wide the cube line will be");
		public GUIContent dotted = new GUIContent("Dotted", "Displays cube lines by line segments.");
        public GUIContent drawHandleIfNotSelected = new GUIContent("Draw Handle If Not Selected", "If enabled, the visualization and the Area itself will be displayed if you do not have Stamper selected.");
    }
}
#endif