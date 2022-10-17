#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.CustomGUI;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class SimpleFilterSettingsEditor 
    {
        public bool FilterSettingsFoldout = true;
        public bool AdditionalNoiseSettingsFoldout = true;

        public void OnGUI(SimpleFilterSettings settings, string text)
        {
            FilterSettingsFoldout = CustomEditorGUILayout.Foldout(FilterSettingsFoldout, text);

			if(FilterSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				CustomEditorGUILayout.BeginChangeCheck();

				DrawCheckHeight(settings);
				DrawCheckSlope(settings);

				if(settings.UseFalloff)
				{
					DrawCheckFractalNoise(settings);
				}

				if(CustomEditorGUILayout.EndChangeCheck())
				{
					EditorUtility.SetDirty(settings);
				}

				EditorGUI.indentLevel--;
			}
        }

		public void DrawCheckHeight(SimpleFilterSettings filterSettings)
		{
			filterSettings.CheckHeight = CustomEditorGUILayout.Toggle(checkHeight, filterSettings.CheckHeight);

			EditorGUI.indentLevel++;

			if(filterSettings.CheckHeight)
			{
				filterSettings.MinHeight = CustomEditorGUILayout.FloatField(new GUIContent("Min Height"), filterSettings.MinHeight);
				filterSettings.MaxHeight = CustomEditorGUILayout.FloatField(new GUIContent("Max Height"), filterSettings.MaxHeight);

				DrawHeightFalloff(filterSettings);
			}

			EditorGUI.indentLevel--;
		}

		public void DrawHeightFalloff(SimpleFilterSettings filterSettings)
		{
			if(!filterSettings.UseFalloff)
			{
				return;
			}

			filterSettings.HeightFalloffType = (FalloffType)CustomEditorGUILayout.EnumPopup(heightFalloffType, filterSettings.HeightFalloffType);

			if(filterSettings.HeightFalloffType != FalloffType.None)
			{
				filterSettings.HeightFalloffMinMax = CustomEditorGUILayout.Toggle(heightFalloffMinMax, filterSettings.HeightFalloffMinMax);
			
				if(filterSettings.HeightFalloffMinMax == true)
				{
					filterSettings.MinAddHeightFalloff = CustomEditorGUILayout.FloatField(minAddHeightFalloff, filterSettings.MinAddHeightFalloff);
					filterSettings.MaxAddHeightFalloff = CustomEditorGUILayout.FloatField(maxAddHeightFalloff, filterSettings.MaxAddHeightFalloff);
				}
				else
				{
					filterSettings.AddHeightFalloff = CustomEditorGUILayout.FloatField(addHeightFalloff, filterSettings.AddHeightFalloff);
				}
			}
		}

		public void DrawSlopeFalloff(SimpleFilterSettings filterSettings)
		{
			if(!filterSettings.UseFalloff)
			{
				return;
			}
			
			filterSettings.SlopeFalloffType = (FalloffType)CustomEditorGUILayout.EnumPopup(slopeFalloffType, filterSettings.SlopeFalloffType);

			if(filterSettings.SlopeFalloffType != FalloffType.None)
			{
				filterSettings.SlopeFalloffMinMax = CustomEditorGUILayout.Toggle(slopeFalloffMinMax, filterSettings.SlopeFalloffMinMax);

				if(filterSettings.SlopeFalloffMinMax)
				{
					filterSettings.MinAddSlopeFalloff = CustomEditorGUILayout.FloatField(minAddSlopeFalloff, filterSettings.MinAddSlopeFalloff);
					filterSettings.MaxAddSlopeFalloff = CustomEditorGUILayout.FloatField(maxAddSlopeFalloff, filterSettings.MaxAddSlopeFalloff);
				}
				else
				{
					filterSettings.AddSlopeFalloff = CustomEditorGUILayout.FloatField(addSlopeFalloff, filterSettings.AddSlopeFalloff);
				}
			}
		}

		void DrawCheckSlope(SimpleFilterSettings filterSettings)
		{
			filterSettings.CheckSlope = CustomEditorGUILayout.Toggle(checkSlope, filterSettings.CheckSlope);

			EditorGUI.indentLevel++;

			if(filterSettings.CheckSlope)
			{
				CustomEditorGUILayout.MinMaxSlider(slope, ref filterSettings.MinSlope, ref filterSettings.MaxSlope, 0f, 90);
				
				DrawSlopeFalloff(filterSettings);
			}

			EditorGUI.indentLevel--;
		}

		public void DrawCheckFractalNoise(SimpleFilterSettings filterSettings)
		{
			EditorGUI.BeginChangeCheck();

			int width = 150;
			int height = 150;

			filterSettings.CheckGlobalFractalNoise = CustomEditorGUILayout.Toggle(new GUIContent("Check Global Fractal Noise"), filterSettings.CheckGlobalFractalNoise);
			
			if(filterSettings.CheckGlobalFractalNoise)
			{
				EditorGUI.indentLevel++;

				filterSettings.NoisePreviewTexture = CustomEditorGUILayout.Foldout(filterSettings.NoisePreviewTexture, "Noise Preview Texture");

				GUILayout.BeginHorizontal();
				{
					if(filterSettings.NoisePreviewTexture )
					{
						EditorGUI.indentLevel++;

						GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());

						Rect textureRect = GUILayoutUtility.GetRect(250, 250, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
						GUI.DrawTexture(textureRect, filterSettings.NoiseTexture);

						EditorGUI.indentLevel--;
					}
				}
				GUILayout.EndHorizontal();

				filterSettings.Fractal.NoiseType = (NoiseType)CustomEditorGUILayout.EnumPopup(new GUIContent("Noise Type"), filterSettings.Fractal.NoiseType);

				filterSettings.Fractal.Seed = CustomEditorGUILayout.IntSlider(seed, filterSettings.Fractal.Seed, 0, 65000);
				filterSettings.Fractal.Octaves = CustomEditorGUILayout.IntSlider(octaves, filterSettings.Fractal.Octaves, 1, 12);
				filterSettings.Fractal.Frequency = CustomEditorGUILayout.Slider(frequency, filterSettings.Fractal.Frequency, 0f, 0.01f);

				filterSettings.Fractal.Persistence = CustomEditorGUILayout.Slider(persistence, filterSettings.Fractal.Persistence, 0f, 1f);
				filterSettings.Fractal.Lacunarity = CustomEditorGUILayout.Slider(lacunarity, filterSettings.Fractal.Lacunarity, 1f, 3.5f);

				AdditionalNoiseSettingsFoldout = CustomEditorGUILayout.Foldout(AdditionalNoiseSettingsFoldout, "Additional Settings");

				if(AdditionalNoiseSettingsFoldout)
				{
					EditorGUI.indentLevel++;

					filterSettings.RemapNoiseMin = CustomEditorGUILayout.Slider(remapNoiseMin, filterSettings.RemapNoiseMin, 0f, 1f);
					filterSettings.RemapNoiseMax = CustomEditorGUILayout.Slider(remapNoiseMax, filterSettings.RemapNoiseMax, 0f, 1f);

					filterSettings.Invert = CustomEditorGUILayout.Toggle(invert, filterSettings.Invert);

					EditorGUI.indentLevel--;
				}

				EditorGUI.indentLevel--;
			}

			if (EditorGUI.EndChangeCheck())
            {		
				if(filterSettings.NoisePreviewTexture)
				{
					FractalNoiseCPU fractal = new FractalNoiseCPU(filterSettings.Fractal.GetNoise(), filterSettings.Fractal.Octaves, filterSettings.Fractal.Frequency / 7, filterSettings.Fractal.Lacunarity, filterSettings.Fractal.Persistence);
					filterSettings.NoiseTexture = new Texture2D(width, height);

                	float[,] arr = new float[width, height];

                	for(int y = 0; y < height; y++)
                	{
                	    for (int x = 0; x < width; x++)
                	    { 
							arr[x,y] = fractal.Sample2D(x, y);
                	    }
                	}

					NoiseUtility.NormalizeArray(arr, width, height, ref filterSettings.RangeMin, ref filterSettings.RangeMax);
	
                	for (int y = 0; y < height; y++)
                	{
                	    for (int x = 0; x < width; x++)
                	    {
                	        float fractalValue = arr[x, y];
							
							if (filterSettings.Invert == true)
                   			{
                   			    fractalValue = 1 - fractalValue;
                   			}

							if (fractalValue < filterSettings.RemapNoiseMin) 
                			{
                			    fractalValue = 0;
                			}
                			else if(fractalValue > filterSettings.RemapNoiseMax)
                			{
                			    fractalValue = 1;
                			}
							else
							{
								fractalValue = Mathf.InverseLerp(filterSettings.RemapNoiseMin, filterSettings.RemapNoiseMax, fractalValue);
							}

                	        filterSettings.NoiseTexture.SetPixel(x, y, new Color(fractalValue, fractalValue, fractalValue, 1));
                	    }
                	}

                	filterSettings.NoiseTexture.Apply();
				}
				else
				{
					FindNoiseRangeMinMax(filterSettings, width, height);
				}	
			}
		}

		private void FindNoiseRangeMinMax(SimpleFilterSettings filterSettings, int width, int height)
		{
			FractalNoiseCPU fractal = new FractalNoiseCPU(filterSettings.Fractal.GetNoise(), filterSettings.Fractal.Octaves, filterSettings.Fractal.Frequency / 7, filterSettings.Fractal.Lacunarity, filterSettings.Fractal.Persistence);
			filterSettings.NoiseTexture = new Texture2D(150, 150);

            float[,] arr = new float[width, height];

            for(int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                { 
					arr[x,y] = fractal.Sample2D(x, y);
                }
            }

			NoiseUtility.NormalizeArray(arr, width, height, ref filterSettings.RangeMin, ref filterSettings.RangeMax);
		}

		private GUIContent checkHeight = new GUIContent("Check Height");
		private GUIContent heightFalloffType = new GUIContent("Height Falloff Type");
		private GUIContent heightFalloffMinMax = new GUIContent("Height Falloff Min Max");
		private GUIContent minAddHeightFalloff = new GUIContent("Min Add Height Falloff");
		private GUIContent maxAddHeightFalloff = new GUIContent("Max Add Height Falloff");
		private GUIContent addHeightFalloff = new GUIContent("Add Height Falloff");

		private GUIContent seed = new GUIContent("Seed");
		private GUIContent octaves = new GUIContent("Octaves");
		private GUIContent frequency = new GUIContent("Frequency");
		private GUIContent persistence = new GUIContent("Persistence");
		private GUIContent lacunarity = new GUIContent("Lacunarity");
		private GUIContent remapNoiseMin = new GUIContent("Remap Noise Min");
		private GUIContent remapNoiseMax = new GUIContent("Remap Noise Max");
		private GUIContent invert = new GUIContent("Invert");

		private GUIContent checkSlope = new GUIContent("Check Slope");
		private GUIContent slope = new GUIContent("Slope");	

		private GUIContent slopeFalloffType = new GUIContent("Slope Falloff Type");
		private GUIContent slopeFalloffMinMax = new GUIContent("Slope Falloff Min Max");
		private GUIContent minAddSlopeFalloff = new GUIContent("Min Add Slope Falloff");
		private GUIContent maxAddSlopeFalloff = new GUIContent("Max Add Slope Falloff");
		private GUIContent addSlopeFalloff = new GUIContent("Add Slope Falloff");
	}
}
#endif
