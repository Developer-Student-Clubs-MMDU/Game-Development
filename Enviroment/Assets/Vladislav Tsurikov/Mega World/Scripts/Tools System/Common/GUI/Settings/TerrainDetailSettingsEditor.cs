#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.CustomGUI;

namespace VladislavTsurikov.MegaWorldSystem
{
	public enum PrefabDetailRenderMode
    {
        Grass,
        VertexLit
    }
	
    [Serializable]
    public class TerrainDetailsSettingsEditor 
    {
        public bool TerrainDetailSettingsFoldout = true;

        public void OnGUI(PrototypeTerrainDetail protoTerrainDetail)
        {
            TerrainDetailSettingsWindowGUI(protoTerrainDetail);
        }

        public void TerrainDetailSettingsWindowGUI(PrototypeTerrainDetail proto)
		{
			TerrainDetailSettingsFoldout = CustomEditorGUILayout.Foldout(TerrainDetailSettingsFoldout, "Terrain Detail Settings");

			if(TerrainDetailSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				CustomEditorGUILayout.BeginChangeCheck(true);

				TerrainDetailSettings terrainDetailSettings = (TerrainDetailSettings)proto.GetSettings(typeof(TerrainDetailSettings));

				terrainDetailSettings.NoiseSpread = CustomEditorGUILayout.Slider(noiseSpread, terrainDetailSettings.NoiseSpread, 0f, 10f);
				
				GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
                    if(CustomEditorGUILayout.ClickButton("Set Random For Width and Height"))
			        {
			        	terrainDetailSettings.SetRandomForWidthHeight();

						foreach (Terrain activeTerrain in Terrain.activeTerrains)
						{
							TerrainResourcesController.SetTerrainDetailSettings(activeTerrain, proto);
						}
			        }
                    GUILayout.Space(3);
                }
                GUILayout.EndHorizontal();

				terrainDetailSettings.MinMax = CustomEditorGUILayout.Toggle(minMax, terrainDetailSettings.MinMax);

				if(terrainDetailSettings.MinMax)
				{
					float min = terrainDetailSettings.MinWidth;
					float max = terrainDetailSettings.MaxWidth;

					min = CustomEditorGUILayout.FloatField(new GUIContent("Min Scale"), min);
					max = CustomEditorGUILayout.FloatField(new GUIContent("Max Scale"), max);

					terrainDetailSettings.MinWidth = min;
					terrainDetailSettings.MaxWidth = max;
					terrainDetailSettings.MinHeight = min;
					terrainDetailSettings.MaxHeight = max;
				}
				else
				{
					terrainDetailSettings.MinWidth = CustomEditorGUILayout.FloatField(minWidth, terrainDetailSettings.MinWidth);
					terrainDetailSettings.MaxWidth = CustomEditorGUILayout.FloatField(maxWidth, terrainDetailSettings.MaxWidth);
					terrainDetailSettings.MinHeight = CustomEditorGUILayout.FloatField(minHeight, terrainDetailSettings.MinHeight);
					terrainDetailSettings.MaxHeight = CustomEditorGUILayout.FloatField(maxHeight, terrainDetailSettings.MaxHeight);
				}

				GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
                    if(CustomEditorGUILayout.ClickButton("Set Random For Color"))
			        {
			        	terrainDetailSettings.SetRandomForColor();

						foreach (Terrain activeTerrain in Terrain.activeTerrains)
						{
							TerrainResourcesController.SetTerrainDetailSettings(activeTerrain, proto);
						}
			        }
                    GUILayout.Space(3);
                }
                GUILayout.EndHorizontal();

				terrainDetailSettings.OnlyOneColor = CustomEditorGUILayout.Toggle(onlyOneColor, terrainDetailSettings.OnlyOneColor);
				
				if(terrainDetailSettings.OnlyOneColor)
				{
					Color color = terrainDetailSettings.HealthyColour;
					color = CustomEditorGUILayout.ColorField(new GUIContent("Color"), color);

					terrainDetailSettings.HealthyColour = color;
					terrainDetailSettings.DryColour = color;
				}
				else
				{
					terrainDetailSettings.HealthyColour = CustomEditorGUILayout.ColorField(healthyColour, terrainDetailSettings.HealthyColour);
					terrainDetailSettings.DryColour = CustomEditorGUILayout.ColorField(dryColour, terrainDetailSettings.DryColour);
				}

				if(proto.PrefabType == PrefabType.Texture)
				{
					terrainDetailSettings.Billboard = CustomEditorGUILayout.Toggle(billboard, terrainDetailSettings.Billboard);

					if(terrainDetailSettings.Billboard)
					{
						terrainDetailSettings.RenderMode = DetailRenderMode.GrassBillboard;
					}
					else
					{
						terrainDetailSettings.RenderMode = DetailRenderMode.Grass;
					}
				}
				else
				{
					PrefabDetailRenderMode prefabDetailRenderMode;

					if(terrainDetailSettings.RenderMode == DetailRenderMode.Grass)
					{
						prefabDetailRenderMode = PrefabDetailRenderMode.Grass;
					}
					else if(terrainDetailSettings.RenderMode == DetailRenderMode.VertexLit)
					{
						prefabDetailRenderMode = PrefabDetailRenderMode.VertexLit;
					}
					else
					{
						prefabDetailRenderMode = PrefabDetailRenderMode.Grass;
					}

					prefabDetailRenderMode = (PrefabDetailRenderMode)CustomEditorGUILayout.EnumPopup(new GUIContent("Render Mode"), prefabDetailRenderMode);

					terrainDetailSettings.RenderMode = prefabDetailRenderMode == PrefabDetailRenderMode.Grass ? DetailRenderMode.Grass : DetailRenderMode.VertexLit;
				}

				if(CustomEditorGUILayout.EndChangeCheck())
				{
					foreach (Terrain activeTerrain in Terrain.activeTerrains)
					{
						TerrainResourcesController.SetTerrainDetailSettings(activeTerrain, proto);
					}
				}

				GUILayout.Space(3);

				EditorGUI.indentLevel--;
			}
		}

		public GUIContent noiseSpread = new GUIContent("Noise Spread");
		public GUIContent minMax = new GUIContent("Min Max");
		public GUIContent minWidth = new GUIContent("Min Width");
		public GUIContent maxWidth = new GUIContent("Max Width");
		public GUIContent minHeight = new GUIContent("Min Height");
		public GUIContent maxHeight = new GUIContent("Max Height");
		public GUIContent onlyOneColor = new GUIContent("Only One Color");
		public GUIContent healthyColour = new GUIContent("Healthy Colour");
		public GUIContent dryColour = new GUIContent("Dry Colour");
		public GUIContent billboard = new GUIContent("Billboard");
    }
}
#endif