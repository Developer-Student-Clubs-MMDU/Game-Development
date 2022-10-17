using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;
using System;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
using VladislavTsurikov.CustomGUI;
#endif

#if UNITY_2021_2_OR_NEWER
using UnityEngine.TerrainTools;
#else
using UnityEngine.Experimental.TerrainAPI;
#endif

namespace VladislavTsurikov.MegaWorldSystem 
{
    [Serializable]
    [MaskFilter("Textures")]
    public class TexturesFilter : MaskFilter 
    {
        public BlendMode BlendMode = BlendMode.Multiply;
        public List<TerrainTexture> TerrainTextureList = new List<TerrainTexture>();

        public override void Eval(MaskFilterContext fc, int index) 
        {
            Vector2 currUV = CommonUtility.WorldPointToUV(fc.AreaVariables.RayHit.Point, fc.AreaVariables.TerrainUnderCursor);

            BrushTransform brushTransform = TerrainPaintUtility.CalculateBrushTransform(fc.AreaVariables.TerrainUnderCursor, currUV, fc.AreaVariables.Size, 0.0f);
            Rect brushRect = brushTransform.GetBrushXYBounds();

            List<TerrainTexture> addTexturesToRenderTextureList = new List<TerrainTexture>();

            if(IsSyncTerrain(Terrain.activeTerrain) == false)
			{
				UpdateCheckTerrainTextures(Terrain.activeTerrain);
			}

            for (int i = 0; i < TerrainTextureList.Count; i++)
            {
                if(TerrainTextureList[i].selected)
                {
                    addTexturesToRenderTextureList.Add(TerrainTextureList[i]);
                }
            }

            Material blendMat = MaskFilterUtility.blendModesMaterial;

            RenderTexture output = RenderTexture.GetTemporary(fc.SourceRenderTexture.width, fc.SourceRenderTexture.height, fc.SourceRenderTexture.depth, GraphicsFormat.R16_SFloat);
            output.enableRandomWrite = true;

            for (int i = 0; i < addTexturesToRenderTextureList.Count; i++)
            {
                RenderTexture localSourceRender = RenderTexture.GetTemporary(fc.SourceRenderTexture.width, fc.SourceRenderTexture.height, 1, RenderTextureFormat.ARGB32);
                localSourceRender.enableRandomWrite = true;

                PaintContext localTextureContext = TerrainPaintUtility.BeginPaintTexture(fc.AreaVariables.TerrainUnderCursor, brushRect, Terrain.activeTerrain.terrainData.terrainLayers[addTexturesToRenderTextureList[i].terrainProtoId]);

                blendMat.SetInt("_BlendMode", (int)BlendMode.Add);
                blendMat.SetTexture("_MainTex", output);
                blendMat.SetTexture("_BlendTex", localTextureContext.sourceRenderTexture);

                Graphics.Blit(output, localSourceRender, blendMat, 0);
                Graphics.Blit(localSourceRender, output);

                TerrainPaintUtility.ReleaseContextResources(localTextureContext); 

                RenderTexture.ReleaseTemporary(localSourceRender);
            }

            RenderTexture sourceRender = RenderTexture.GetTemporary(fc.SourceRenderTexture.width, fc.SourceRenderTexture.height, 1, RenderTextureFormat.ARGB32);
            sourceRender.enableRandomWrite = true;

            if(index == 0)
            {
                blendMat.SetInt("_BlendMode", (int)BlendMode.Multiply);
            }
            else
            {
                blendMat.SetInt("_BlendMode", (int)BlendMode);
            }

            blendMat.SetTexture("_MainTex", output);
            blendMat.SetTexture("_BlendTex", fc.SourceRenderTexture);

            Graphics.Blit(output, fc.DestinationRenderTexture, blendMat, 0);

            RenderTexture.ReleaseTemporary(output);
            RenderTexture.ReleaseTemporary(sourceRender);
        }

        public bool IsSyncTerrain(Terrain terrain)
        {
            for (int Id = 0; Id < terrain.terrainData.terrainLayers.Length; Id++)
            {
                bool find = false;

                for (int i = 0; i < TerrainTextureList.Count; i++)
                {
                    if (CommonUtility.IsSameTexture(terrain.terrainData.terrainLayers[Id].diffuseTexture, TerrainTextureList[i].texture, false))
                    {
                        find = true;
                        break;
                    }
                }

                if(find == false)
                {
                    return false;
                }
            }

            return true;
        }

        public void UpdateCheckTerrainTextures(Terrain activeTerrain)
        {
            if (activeTerrain == null)
            {
                Debug.LogWarning("Can not update prototypes from the terrain as there is no terrain currently active in this scene.");
                return;
            }

            int idx;

            TerrainTexture checkTerrainTextures;

            TerrainTextureList.Clear();
            
            TerrainLayer[] terrainLayer = activeTerrain.terrainData.terrainLayers;

            for (idx = 0; idx < terrainLayer.Length; idx++)
            {
                checkTerrainTextures = new TerrainTexture();
                checkTerrainTextures.texture = terrainLayer[idx].diffuseTexture;
                checkTerrainTextures.terrainProtoId = idx;

                TerrainTextureList.Add(checkTerrainTextures);
            }
        }

#if UNITY_EDITOR
        [SerializeField]
        private TexturesFilterEditor texturesFilterEditor;
        private TexturesFilterEditor TexturesFilterEditor
        {
            get
            {
                if(texturesFilterEditor == null)
                {
                    texturesFilterEditor = new TexturesFilterEditor(this);
                }

                return texturesFilterEditor;
            }
        }

        public override void DoGUI(Rect rect, int index) 
        {
            TexturesFilterEditor.OnGUI(rect, index);
        }

        public override float GetElementHeight(int index) 
        {
            if(index != 0)
            {
                return EditorGUIUtility.singleLineHeight * 12;
            }
            else
            {
                return EditorGUIUtility.singleLineHeight * 11;
            }
        }
#endif
    }

#if UNITY_EDITOR
    public class TexturesFilterEditor 
    {
		private int сheckTexturesIconWidth  = 60;
        private int сheckTexturesIconHeight = 60;
		public Vector2 сheckTexturesWindowsScroll = Vector2.zero;
        public float сheckTexturesWindowHeight = 140.0f;

		public TexturesFilter texturesFilter = null;

        public TexturesFilterEditor(TexturesFilter texturesFilter)
        {
            this.texturesFilter = texturesFilter;
        }

		public void OnGUI(Rect rect, int index)
        {
			Event e = Event.current;

        	// Settings
        	Color initialGUIColor = GUI.backgroundColor;

			if(index != 0)
            {
                texturesFilter.BlendMode = (BlendMode)EditorGUI.EnumPopup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Blend Mode"), texturesFilter.BlendMode);
                rect.y += EditorGUIUtility.singleLineHeight;
				rect.y += EditorGUIUtility.singleLineHeight;
            }

			Rect windowRect = new Rect(rect.x, rect.y, rect.width, сheckTexturesWindowHeight);

			Rect virtualRect = new Rect(windowRect);

			if(texturesFilter.IsSyncTerrain(Terrain.activeTerrain) == false)
			{
				texturesFilter.UpdateCheckTerrainTextures(Terrain.activeTerrain);
			}

			if(IsNecessaryToDrawIconsForCheckTerrainTextures(windowRect, initialGUIColor, texturesFilter.TerrainTextureList))
			{
				DrawLabelForIcons(initialGUIColor, windowRect);
				DrawCheckTerrainTexturesIcons(e, windowRect);
			}

			switch (e.type)
			{
				case EventType.ContextClick:
				{
            		if(virtualRect.Contains(e.mousePosition))
            		{
						CheckTerrainTexturesWindowMenu().ShowAsContext();
            		    e.Use();
            		}
            		break;
				}
			}

			GUILayout.Space(3);
		}

        private void DrawCheckTerrainTexturesIcons(Event e, Rect windowRect)
		{
			List<TerrainTexture> checkTerrainTextures = texturesFilter.TerrainTextureList;

			Rect virtualRect = GetVirtualRect(windowRect, checkTerrainTextures.Count, сheckTexturesIconWidth, сheckTexturesIconHeight);

			Vector2 brushWindowScrollPos = сheckTexturesWindowsScroll;
            brushWindowScrollPos = GUI.BeginScrollView(windowRect, brushWindowScrollPos, virtualRect, false, true);

			int y = (int)virtualRect.yMin;
			int x = (int)virtualRect.xMin;

			for (int i = 0; i < checkTerrainTextures.Count; i++)
			{
				Rect brushIconRect = new Rect(x, y, сheckTexturesIconWidth, сheckTexturesIconHeight);

				Color rectColor;

				if (checkTerrainTextures[i].selected)
				{
					rectColor = EditorColors.Instance.ToggleButtonActiveColor;
				}
        	    else 
				{ 
					rectColor = Color.white; 
				}

				DrawIconRectForCheckTerrainTextures(brushIconRect, checkTerrainTextures[i].texture, rectColor, e, windowRect, brushWindowScrollPos, () =>
				{
					HandleSelectCheckTerrainTextures(i, e);
				});

				SetNextXYIcon(virtualRect, сheckTexturesIconWidth, сheckTexturesIconHeight, ref y, ref x);
			}

			сheckTexturesWindowsScroll = brushWindowScrollPos;

			GUI.EndScrollView();
		}

        private void DrawIconRectForCheckTerrainTextures(Rect brushIconRect, Texture2D preview, Color rectColor, Event e, Rect brushWindowRect, Vector2 brushWindowScrollPos, UnityAction clickAction)
		{
			GUIStyle LabelTextForIcon = CustomEditorGUILayout.GetStyle(StyleName.LabelTextForIcon);
			GUIStyle LabelTextForSelectedArea = CustomEditorGUILayout.GetStyle(StyleName.LabelTextForSelectedArea);

            Rect brushIconRectScrolled = new Rect(brushIconRect);
            brushIconRectScrolled.position -= brushWindowScrollPos;

            // only visible incons
            if(brushIconRectScrolled.Overlaps(brushWindowRect))
            {
                if(brushIconRect.Contains(e.mousePosition))
				{
					clickAction.Invoke();
				}

				EditorGUI.DrawRect(brushIconRect, rectColor);
                    
				// Prefab preview 
                if(e.type == EventType.Repaint)
                {
                    Rect previewRect = new Rect(brushIconRect.x+2, brushIconRect.y+2, brushIconRect.width-4, brushIconRect.width-4);

					if(preview != null)
					{
						EditorGUI.DrawPreviewTexture(previewRect, preview);
					}
					else
					{
						Color dimmedColor = new Color(0.4f, 0.4f, 0.4f, 1.0f);
						EditorGUI.DrawRect(previewRect, dimmedColor);
					}
                }
			}
		}

		public void HandleSelectCheckTerrainTextures(int index, Event e)
		{
			switch (e.type)
			{
				case EventType.MouseDown:
				{
					GUI.changed = true;
					
					if(e.button == 0)
					{										
						if (e.control)
						{    
							SelectCheckTerrainTextureAdditive(index);               
						}
						else if (e.shift)
						{          
							SelectCheckTerrainTextureRange(index);                
						}
						else 
						{
							SelectCheckTerrainTexture(index);
						}

            	    	e.Use();
					}

            	    break;
				}
			}
		}

        public void SelectCheckTerrainTexture(int index)
        {
            SetSelectedAllCheckTerrainTexture(false);

            if(index < 0 && index >= texturesFilter.TerrainTextureList.Count)
            {
                return;
            }

            texturesFilter.TerrainTextureList[index].selected = true;
        }

        public void SelectCheckTerrainTextureAdditive(int index)
        {
            if(index < 0 && index >= texturesFilter.TerrainTextureList.Count)
            {
                return;
            }
        
            texturesFilter.TerrainTextureList[index].selected = !texturesFilter.TerrainTextureList[index].selected;
        }

        public void SelectCheckTerrainTextureRange(int index)
        {
            if(index < 0 && index >= texturesFilter.TerrainTextureList.Count)
            {
                return;
            }

            int rangeMin = index;
            int rangeMax = index;

            for (int i = 0; i < texturesFilter.TerrainTextureList.Count; i++)
            {
                if (texturesFilter.TerrainTextureList[i].selected)
                {
                    rangeMin = Mathf.Min(rangeMin, i);
                    rangeMax = Mathf.Max(rangeMax, i);
                }
            }

            for (int i = rangeMin; i <= rangeMax; i++) 
            {
                if (texturesFilter.TerrainTextureList[i].selected != true)
                {
                    break;
                }
            }

            for (int i = rangeMin; i <= rangeMax; i++) 
            {
                texturesFilter.TerrainTextureList[i].selected = true;
            }
        }

        public void SetSelectedAllCheckTerrainTexture(bool select)
        {
            foreach (TerrainTexture checkTerrainTextures in texturesFilter.TerrainTextureList)
            {
                checkTerrainTextures.selected = select;
            }
        }

        private void DrawLabelForIcons(Color InitialGUIColor, Rect windowRect, string text = null)
		{
			GUIStyle LabelTextForSelectedArea = CustomEditorGUILayout.GetStyle(StyleName.LabelTextForSelectedArea);
			GUIStyle boxStyle = CustomEditorGUILayout.GetStyle(StyleName.Box);

			GUI.color = EditorColors.Instance.boxColor;
			GUI.Label(windowRect, "", boxStyle);
			GUI.color = InitialGUIColor;

			if(text != null)
			{
				EditorGUI.LabelField(windowRect, text, LabelTextForSelectedArea);
			}
		}

        private bool IsNecessaryToDrawIconsForCheckTerrainTextures(Rect brushWindowRect, Color initialGUIColor, List<TerrainTexture> checkTerrainTextures)
		{
			if(checkTerrainTextures.Count == 0)
			{
				DrawLabelForIcons(initialGUIColor, brushWindowRect, "Missing textures on terrain");
				return false;
			}

			return true;
		}

        private Rect GetVirtualRect(Rect brushWindowRect, int count, int iconWidth, int iconHeight)
		{
			Rect virtualRect = new Rect(brushWindowRect);
            {
                virtualRect.width = Mathf.Max(virtualRect.width - 20, 1); // space for scroll 

                int presetColumns = Mathf.FloorToInt(Mathf.Max(1, (virtualRect.width) / iconWidth));
                int virtualRows   = Mathf.CeilToInt((float)count / presetColumns);

                virtualRect.height = Mathf.Max(virtualRect.height, iconHeight * virtualRows);
            }

			return virtualRect;
		}

        private void SetNextXYIcon(Rect virtualRect, int iconWidth, int iconHeight, ref int y, ref int x)
		{
			if(x + iconWidth < (int)virtualRect.xMax - iconWidth)
            {
                x += iconWidth;
            }
            else if(y < (int)virtualRect.yMax)
            {
                y += iconHeight;
                x = (int)virtualRect.xMin;
            }
		}

		GenericMenu CheckTerrainTexturesWindowMenu()
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Update Check Terrain Textures"), false, ContextMenuCallback, new Action(() => texturesFilter.UpdateCheckTerrainTextures(Terrain.activeTerrain))); 

            return menu;
        }

		void ContextMenuCallback(object obj)
        {
            if (obj != null && obj is Action)
                (obj as Action).Invoke();
        }
    }
#endif
}
