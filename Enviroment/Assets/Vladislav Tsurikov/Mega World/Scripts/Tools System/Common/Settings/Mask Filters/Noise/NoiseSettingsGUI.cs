#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VladislavTsurikov.MegaWorldSystem
{
    public class NoiseSettingsGUI
    {
        private RenderTexture m_previewRT;
        public RenderTexture previewRT
        {
            get { return m_previewRT; }
        }

        public NoiseSettings m_noiseSettings = null;

        public NoiseSettingsGUI(NoiseSettings noiseSettings)
        {
            this.m_noiseSettings = noiseSettings;
        }

        public void OnGUI(Rect rect)
        {
            DrawPreviewTexture(ref rect, 256f, true);
            rect.y += EditorGUIUtility.singleLineHeight;
            TransformSettingsGUI(ref rect);
            rect.y += EditorGUIUtility.singleLineHeight;
            DomainSettingsGUI(ref rect);
        }

        private void TransformSettingsGUI(ref Rect rect)
        {
            m_noiseSettings.ShowNoiseTransformSettings = EditorGUI.Foldout(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), m_noiseSettings.ShowNoiseTransformSettings, new GUIContent("TransformSettings"));
            if (m_noiseSettings.ShowNoiseTransformSettings)
            {
                EditorGUI.indentLevel++;
                rect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Translation"));
                rect.y += EditorGUIUtility.singleLineHeight;
                m_noiseSettings.TransformSettings.Translation = EditorGUI.Vector3Field(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent(""), m_noiseSettings.TransformSettings.Translation);
                rect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Rotation"));
                rect.y += EditorGUIUtility.singleLineHeight;
                m_noiseSettings.TransformSettings.Rotation = EditorGUI.Vector3Field(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent(""), m_noiseSettings.TransformSettings.Rotation);
                rect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Scale"));
                rect.y += EditorGUIUtility.singleLineHeight;
                m_noiseSettings.TransformSettings.Scale = EditorGUI.Vector3Field(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent(""), m_noiseSettings.TransformSettings.Scale);
                EditorGUI.indentLevel--;
            }
        }

        private void DomainSettingsGUI(ref Rect rect)
        {
            m_noiseSettings.ShowNoiseTypeSettings = EditorGUI.Foldout(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), m_noiseSettings.ShowNoiseTypeSettings, new GUIContent("DomainSettings"));
            if (m_noiseSettings.ShowNoiseTypeSettings)
            {
                EditorGUI.indentLevel++;
                rect.y += EditorGUIUtility.singleLineHeight;
                m_noiseSettings.DomainSettings.NoiseTypeName = NoiseLib.NoiseTypePopup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Type"), m_noiseSettings.DomainSettings.NoiseTypeName);
                rect.y += EditorGUIUtility.singleLineHeight;
                IFractalType fractalType = NoiseLib.GetFractalTypeInstance(m_noiseSettings.DomainSettings.FractalTypeName);
                m_noiseSettings.DomainSettings.FractalTypeParams = fractalType?.DoGUI(rect, m_noiseSettings.DomainSettings.FractalTypeParams);
                EditorGUI.indentLevel--;
            }
        }

        private void HandlePreviewTextureInput(Rect previewRect)
        {
            if(GUIUtility.hotControl != 0)
            {
                return;
            }

            Vector3 t = m_noiseSettings.TransformSettings.Translation;
            Vector3 r = m_noiseSettings.TransformSettings.Rotation;
            Vector3 s = m_noiseSettings.TransformSettings.Scale;

            EventType eventType = Event.current.type;

            bool draggingPreview = Event.current.button == 0 &&
                                   (eventType == EventType.MouseDown ||
                                    eventType == EventType.MouseDrag);

            Vector2 previewDims = new Vector2(previewRect.width, previewRect.height);
            Vector2 abs = new Vector2(Mathf.Abs(s.x), Mathf.Abs(s.z));

            if (Event.current.type == EventType.ScrollWheel)
            {
                abs += Vector2.one * .001f;
                
                float scroll = Event.current.delta.y;
                
                s.x += abs.x * scroll * .05f;
                s.z += abs.y * scroll * .05f;
                
                m_noiseSettings.TransformSettings.Scale = s;
                GUI.changed = true;

                Event.current.Use();
            }
            else if (draggingPreview)
            {
                // change noise offset panning icon
                Vector2 sign = new Vector2(-Mathf.Sign(s.x), Mathf.Sign(s.z));
                Vector2 delta = Event.current.delta / previewDims * abs * sign;
                Vector3 d3 = new Vector3(delta.x, 0, delta.y);

                d3 = Quaternion.Euler( r ) * d3;

                t += d3;
                
                m_noiseSettings.TransformSettings.Translation = t;
                GUI.changed = true;

                Event.current.Use();
            }
        }

        /// <summary>
        /// Renders an interactive Noise Preview along with tooltip icons and an optional Export button that opens a new ExportNoiseWindow.
        /// A background image is also rendered behind the preview that takes up the entire width of the EditorWindow currently being drawn.
        /// </summary>
        /// <param name = "minSize"> Minimum size for the Preview </param>
        /// <param name = "showExportButton"> Whether or not to render the Export button </param>
        public void DrawPreviewTexture(ref Rect rect, float minSize, bool showExportButton = true)
        {
            m_noiseSettings.ShowNoisePreviewTexture = EditorGUI.Foldout(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), m_noiseSettings.ShowNoisePreviewTexture, new GUIContent("PreviewTexture"));
            if (m_noiseSettings.ShowNoisePreviewTexture)
            {
                float padding = 4f;
                float iconWidth = 40f;
                int size = (int)Mathf.Min(minSize, EditorGUIUtility.currentViewWidth);

                Rect currentRect = rect;
                currentRect.y += EditorGUIUtility.singleLineHeight *2f;
                Rect totalRect = new Rect(currentRect.x, currentRect.y, currentRect.width, size);

                //Rect totalRect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, size + padding * 2); // extra pixels for highlight border

                Color prev = GUI.color;
                GUI.color = new Color(.15f, .15f, .15f, 1f);
                GUI.DrawTexture(totalRect, Texture2D.whiteTexture, ScaleMode.StretchToFill, false);
                GUI.color = Color.white;

                // draw info icon
                // if(totalRect.Contains(Event.current.mousePosition))
                {
                    Rect infoIconRect = new Rect( totalRect.x + padding, totalRect.y + padding, iconWidth, iconWidth );
                    GUI.Label( infoIconRect, Styles.infoIcon );
                    // GUI.Label( infoIconRect, Styles.noiseTooltip );
                }

                // draw export button

                float buttonWidth = GUI.skin.button.CalcSize(Styles.export).x;
                float buttonHeight = EditorGUIUtility.singleLineHeight;

                float safeSpace = Mathf.Max( iconWidth * 2, buttonWidth * 2 ) + padding * 4;
                float minWidth = Mathf.Min( size, totalRect.width - safeSpace );
                Rect previewRect = new Rect(totalRect.x + totalRect.width / 2 - minWidth / 2, totalRect.y + totalRect.height / 2 - minWidth / 2, minWidth, minWidth);

                EditorGUIUtility.AddCursorRect(previewRect, UnityEditor.MouseCursor.Pan);

                if (previewRect.Contains(Event.current.mousePosition))
                {
                    HandlePreviewTextureInput(previewRect);
                }

                if ( Event.current.type == EventType.Repaint )
                {
                    // create preview RT here and keep until the next Repaint
                    if( m_previewRT != null )
                    {
                        RenderTexture.ReleaseTemporary( m_previewRT );
                    }

                    m_previewRT = RenderTexture.GetTemporary(512, 512, 0, RenderTextureFormat.ARGB32);
                    RenderTexture tempRT = RenderTexture.GetTemporary(512, 512, 0, RenderTextureFormat.RFloat);

                    RenderTexture prevActive = RenderTexture.active;

                    NoiseUtils.Blit2D(m_noiseSettings, tempRT);

                    NoiseUtils.BlitPreview2D(tempRT, m_previewRT);

                    RenderTexture.active = prevActive;

                    GUI.DrawTexture(previewRect, m_previewRT, ScaleMode.ScaleToFit, false);

                    RenderTexture.ReleaseTemporary(tempRT);
                }

                GUI.color = prev;

                rect.y += 256f + EditorGUIUtility.singleLineHeight;
            }
        }

        static class Styles
        {
            public static GUIContent noisePreview;
            public static GUIContent export = EditorGUIUtility.TrTextContent("Export", "Open a window providing options for exporting Noise to Textures");
            public static GUIContent infoIcon = new GUIContent("", EditorGUIUtility.FindTexture("console.infoicon"),
                                "Scroll Mouse Wheel:\nZooms the preview in and out and changes the noise scale\n\n" +
                                "Left-mouse Drag:\nPans the noise field and changes the noise translation\n\n" +
                                "Color Key:\nCyan = negative noise values\nGrayscale = values between 0 and 1\nBlack = values are 0\nRed = Values greater than 1. Used for debugging texture normalization");

            static Styles()
            {
                noisePreview = EditorGUIUtility.TrTextContent("Noise Field Preview:");
            }
        }
    }
}
#endif
