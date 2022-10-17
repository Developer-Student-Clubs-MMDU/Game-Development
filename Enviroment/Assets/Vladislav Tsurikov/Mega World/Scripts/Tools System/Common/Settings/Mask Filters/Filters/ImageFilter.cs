using System;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VladislavTsurikov.MegaWorldSystem 
{
    public enum FilterMode {GrayScale, Color, RedColorChannel, GreenColorChannel, BlueColorChannel, AlphaChannel }

    [Serializable]
    [MaskFilter("Image")]
    public class ImageFilter : MaskFilter 
    {
        public BlendMode BlendMode = BlendMode.Multiply;
        public TextureFormat[] Valid16BitFormats = new TextureFormat[10] {TextureFormat.ARGB4444, TextureFormat.R16, TextureFormat.RFloat, TextureFormat.RGB565, TextureFormat.RGBA4444, TextureFormat.RGBAFloat, TextureFormat.RGBAHalf, TextureFormat.RGFloat, TextureFormat.RGHalf, TextureFormat.RHalf};

        [SerializeField]
        private Texture2D _image;
        public Texture2D Image
        {        
            get
            {
                if (_image == null)
                {
                    if (!string.IsNullOrEmpty(_imageTextureGUID))
                    {
#if UNITY_EDITOR
                    _image = (Texture2D)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(_imageTextureGUID), typeof(Texture2D));
#endif
                    }
                }
                return _image;
            }
            set
            {
                if (value != _image)
                {
                    _image = value;
                    if (value != null)
                    {
#if UNITY_EDITOR
                    _imageTextureGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value));
#endif
                    }
                    else
                    {
                        _imageTextureGUID = "";
                    }
                }
            }
        }

        [SerializeField]
        private string _imageTextureGUID;
        
        public FilterMode FilterMode;
        public Color Color = Color.white;
        public float ColorAccuracy = 0.5f;

        public float OffSetX = 0f;
        public float OffSetZ = 0f;

        private Material _imageMat = null;
        Material GetMaterial() 
        {
            if (_imageMat == null) 
            {
                _imageMat = new Material( Shader.Find( "Hidden/MegaWorld/Image"));
            }
            return _imageMat;
        }

        public override void Eval(MaskFilterContext fc, int index) 
        {
            Material mat = GetMaterial();

            mat.SetTexture("_InputTex", fc.SourceRenderTexture);

            SetMaterial(mat, index);

            Graphics.Blit(fc.SourceRenderTexture, fc.DestinationRenderTexture, mat, 0);
        }

        public void SetMaterial(Material mat, int index)
        {
            if(index == 0)
            {
                mat.SetInt("_BlendMode", (int)BlendMode.Multiply);
            }
            else
            {
                mat.SetInt("_BlendMode", (int)BlendMode);
            }

            mat.SetTexture("_ImageMaskTex", Image);
            mat.SetInt("_FilterMode", (int)FilterMode);
            mat.SetColor("_Color", Color);
            mat.SetFloat("_ColorAccuracy", ColorAccuracy);
            mat.SetFloat("_XOffset", OffSetX);
            mat.SetFloat("_ZOffset", OffSetZ);
        }

#if UNITY_EDITOR
        public override void DoGUI(Rect rect, int index) 
        {
            if(index != 0)
            {
                BlendMode = (BlendMode)EditorGUI.EnumPopup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Blend Mode"), BlendMode);
                rect.y += EditorGUIUtility.singleLineHeight;
            }

            Image = (Texture2D)EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width -73, EditorGUIUtility.singleLineHeight), "Image", Image, typeof(Texture2D), false);

            rect.y += EditorGUIUtility.singleLineHeight;

            if (Image == null)
            {
                rect.y += EditorGUIUtility.singleLineHeight;
                rect.height = EditorGUIUtility.singleLineHeight * 2;
                EditorGUI.HelpBox(rect, "Add an image. You can set any image as black and white, as well as having several colors", MessageType.Warning);
            }
            else
            {
                if (Image != null && !Valid16BitFormats.Contains(Image.format))
                {
                    rect.y += EditorGUIUtility.singleLineHeight;
                    rect.height = EditorGUIUtility.singleLineHeight * 5;
                    EditorGUI.HelpBox(rect, "The supplied texture does not have a quality texture format or is less than 16 bit. For optimal quality, use these formats:\nTextureFormat.ARGB4444, TextureFormat.R16, TextureFormat.RFloat, TextureFormat.RGB565, TextureFormat.RGBA4444, TextureFormat.RGBAFloat, TextureFormat.RGBAHalf, TextureFormat.RGFloat, TextureFormat.RGHalf, TextureFormat.RHalf", MessageType.Warning);
                    rect.y += EditorGUIUtility.singleLineHeight * 5;
                    rect.height = EditorGUIUtility.singleLineHeight;
                    rect.y += EditorGUIUtility.singleLineHeight;
                }

                FilterMode = (FilterMode)EditorGUI.EnumPopup(rect, "Filter Mode", FilterMode);
                rect.y += EditorGUIUtility.singleLineHeight;

                if (FilterMode == FilterMode.Color)
                {
                    Color = EditorGUI.ColorField(rect, "Color", Color);
                    rect.y += EditorGUIUtility.singleLineHeight;
                    ColorAccuracy = EditorGUI.Slider(rect, "Color Accuracy", ColorAccuracy, 0f,1f);
                }

                OffSetX = EditorGUI.FloatField(rect, "XOffset", OffSetX);
                rect.y += EditorGUIUtility.singleLineHeight;
                OffSetZ = EditorGUI.FloatField(rect, "ZOffset", OffSetZ);
            }
        }

        public override float GetElementHeight(int index) 
        {
            float height = 0;

            if (Image == null)
            {
                height += EditorGUIUtility.singleLineHeight * 6;
            }
            else
            {
                height += EditorGUIUtility.singleLineHeight * 6;
                if (Image != null && !Valid16BitFormats.Contains(Image.format))
                {
                    height += EditorGUIUtility.singleLineHeight * 7;
                }
                if (FilterMode == FilterMode.Color)
                {
                    height += EditorGUIUtility.singleLineHeight * 2;
                }
            }

            return height;
        }
#endif
    }
}
