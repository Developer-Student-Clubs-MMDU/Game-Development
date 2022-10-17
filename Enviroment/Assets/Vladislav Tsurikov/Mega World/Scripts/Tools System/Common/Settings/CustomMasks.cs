using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEditor;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class CustomMasks
    {
        static readonly StringBuilder Builder = new StringBuilder();
    	public List<Texture2D> customBrushes = new List<Texture2D>();
        public int selectedCustomBrush = 0;      

		#if UNITY_EDITOR
        public CustomMasksEditor customMasksEditor = new CustomMasksEditor();

        public void OnGUI()
        {
            customMasksEditor.OnGUI(this);
        }
        #endif

        public void GetBuiltinBrushes()
    	{    
#if UNITY_EDITOR
			GetBuiltinTextures(UnityEditor.Experimental.EditorResources.brushesPath, "builtin_brush_");
    		selectedCustomBrush = Mathf.Min(customBrushes.Count - 1, selectedCustomBrush);
#endif
        }

		public void GetPolarisBrushes()
    	{    
			customBrushes = new List<Texture2D>(Resources.LoadAll<Texture2D>(MegaWorldPath.PolarisBrushes));
    		selectedCustomBrush = Mathf.Min(customBrushes.Count - 1, selectedCustomBrush);
        }

    	private void GetBuiltinTextures(string path, string prefix)
    	{
#if UNITY_EDITOR
    		Texture2D texture;
    		int index = 1;
    		do // begin from ../Brush_1 to ../Brush_n until there is a file not found
    		{
    			// Build file path
    			Builder.Length = 0;
    			Builder.Append(path);
    			Builder.Append(prefix);
    			Builder.Append(index);
    			Builder.Append(".png");

    			// Increase index for next texture
    			index++;

    			// Add texture to list
    			texture = (Texture2D)EditorGUIUtility.Load(Builder.ToString());
    			if (texture != null)
				{
					customBrushes.Add(texture);
				}
    				
    		} while (texture != null);
#endif
    	}

        public Texture2D GetSelectedBrush()
        {
			if(customBrushes.Count == 0)
			{
				return null;
			}

			if(selectedCustomBrush > customBrushes.Count - 1)
			{
				return null;
			}
			
            return customBrushes[selectedCustomBrush];
        }
    }
}

