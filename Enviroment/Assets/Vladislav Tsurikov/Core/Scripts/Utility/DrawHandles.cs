using UnityEngine;
using UnityEditor;

namespace VladislavTsurikov
{
    public enum ButtonAction
    {
        None,
        MouseDown,
        Hover
    }

    public static class DrawHandles 
    {
        #if UNITY_EDITOR

        private static Texture2D gizmoLineAaTexture;
    
        public static Texture2D GizmoLineAaTexture
        {
            get
            {
                if (gizmoLineAaTexture == null)
                {
                    gizmoLineAaTexture = new Texture2D(1, 2);
                    gizmoLineAaTexture.SetPixels(new Color[] { new Color(1.0f, 1.0f, 1.0f, 0.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f) });
                    gizmoLineAaTexture.Apply();
                }
    
                return gizmoLineAaTexture;
            }
        }

        public static Vector3 squareCornerC
        {
            get
            {
                return new Vector3(0.5f, 0, -0.5f);
            }
        }
        public static Vector3 squareCornerD
        {
            get
            {
                return new Vector3(-0.5f, 0, -0.5f);
            }
        }

        public static Vector3 squareCornerG
        {
            get
            {
                return new Vector3(0.5f, 0, 0.5f);
            }
        }
        public static Vector3 squareCornerH
        {
            get
            {
                return new Vector3(-0.5f, 0, 0.5f);
            }
        }

    
        public static Vector3 cubeCornerA
        {
            get
            {
                return new Vector3(-0.5f, 0.5f, -0.5f);
            }
        }
        public static Vector3 cubeCornerB
        {
            get
            {
                return new Vector3(0.5f, 0.5f, -0.5f);
            }
        }
        public static Vector3 cubeCornerC
        {
            get
            {
                return new Vector3(0.5f, -0.5f, -0.5f);
            }
        }
        public static Vector3 cubeCornerD
        {
            get
            {
                return new Vector3(-0.5f, -0.5f, -0.5f);
            }
        }
        public static Vector3 cubeCornerE
        {
            get
            {
                return new Vector3(-0.5f, 0.5f, 0.5f);
            }
        }
        public static Vector3 cubeCornerF
        {
            get
            {
                return new Vector3(0.5f, 0.5f, 0.5f);
            }
        }
        public static Vector3 cubeCornerG
        {
            get
            {
                return new Vector3(0.5f, -0.5f, 0.5f);
            }
        }
        public static Vector3 cubeCornerH
        {
            get
            {
                return new Vector3(-0.5f, -0.5f, 0.5f);
            }
        }

        public const float occlusionOpacityFactor = 0.125f;
        public readonly static Color occlusionOpacityColorFactor = new Color(1.0f, 1.0f, 1.0f, occlusionOpacityFactor);

        const float circleTangentCoeficient = 0.551915024494f;

        public static void DrawSphere(Matrix4x4 transform, Color color, float thickness)
        {
            DrawCircle(transform, color, thickness);
            DrawCircle(transform, Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(90, Vector3.forward), Vector3.one), color, thickness);
        }

        public static void DrawCircle(Matrix4x4 transform, Matrix4x4 offset, Color color, float thickness)
        {
            Matrix4x4 offsetTransform = transform * offset;

            DrawBezier(Vector3.right, Vector3.right + Vector3.forward * circleTangentCoeficient, Vector3.forward, Vector3.forward + Vector3.right * circleTangentCoeficient, offsetTransform, Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * 0.5f), color, thickness);
            DrawBezier(Vector3.forward, Vector3.forward - Vector3.right * circleTangentCoeficient, Vector3.left, Vector3.left + Vector3.forward * circleTangentCoeficient, offsetTransform, Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * 0.5f), color, thickness);
            DrawBezier(Vector3.left, Vector3.left + Vector3.back * circleTangentCoeficient, Vector3.back, Vector3.back - Vector3.right * circleTangentCoeficient, offsetTransform, Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * 0.5f), color, thickness);
            DrawBezier(Vector3.back, Vector3.back + Vector3.right * circleTangentCoeficient, Vector3.right, Vector3.right + Vector3.back * circleTangentCoeficient, offsetTransform, Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * 0.5f), color, thickness);
        }

        public static void DrawCircleWithoutZTest(Matrix4x4 transform, Matrix4x4 offset, Color color, float thickness)
        {
            Matrix4x4 offsetTransform = transform * offset;

            DrawBezierWithoutZTest(Vector3.right, Vector3.right + Vector3.forward * circleTangentCoeficient, Vector3.forward, Vector3.forward + Vector3.right * circleTangentCoeficient, offsetTransform, Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * 0.5f), color, thickness);
            DrawBezierWithoutZTest(Vector3.forward, Vector3.forward - Vector3.right * circleTangentCoeficient, Vector3.left, Vector3.left + Vector3.forward * circleTangentCoeficient, offsetTransform, Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * 0.5f), color, thickness);
            DrawBezierWithoutZTest(Vector3.left, Vector3.left + Vector3.back * circleTangentCoeficient, Vector3.back, Vector3.back - Vector3.right * circleTangentCoeficient, offsetTransform, Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * 0.5f), color, thickness);
            DrawBezierWithoutZTest(Vector3.back, Vector3.back + Vector3.right * circleTangentCoeficient, Vector3.right, Vector3.right + Vector3.back * circleTangentCoeficient, offsetTransform, Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * 0.5f), color, thickness);
        }

        public static void DrawCircle(Matrix4x4 transform, Color color, float thickness)
        {
            DrawBezier(Vector3.right, Vector3.right + Vector3.forward * circleTangentCoeficient, Vector3.forward, Vector3.forward + Vector3.right * circleTangentCoeficient, transform, Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * 0.5f), color, thickness);
            DrawBezier(Vector3.forward, Vector3.forward - Vector3.right * circleTangentCoeficient, Vector3.left, Vector3.left + Vector3.forward * circleTangentCoeficient, transform, Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * 0.5f), color, thickness);
            DrawBezier(Vector3.left, Vector3.left + Vector3.back * circleTangentCoeficient, Vector3.back, Vector3.back - Vector3.right * circleTangentCoeficient, transform, Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * 0.5f), color, thickness);
            DrawBezier(Vector3.back, Vector3.back + Vector3.right * circleTangentCoeficient, Vector3.right, Vector3.right + Vector3.back * circleTangentCoeficient, transform, Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * 0.5f), color, thickness);
        }

        public static void DrawBezier(Vector3 startPosition, Vector3 startTangent, Vector3 endPosition, Vector3 endTangent, Matrix4x4 transform, Matrix4x4 offset, Color color, float thickness)
        {
            Matrix4x4 offsetTransform = transform * offset;
            DrawBezier(startPosition, startTangent, endPosition, endTangent, offsetTransform, color, thickness);
        }

        public static void DrawBezierWithoutZTest(Vector3 startPosition, Vector3 startTangent, Vector3 endPosition, Vector3 endTangent, Matrix4x4 transform, Matrix4x4 offset, Color color, float thickness)
        {
            Matrix4x4 offsetTransform = transform * offset;
            DrawBezierWithoutOpacity(startPosition, startTangent, endPosition, endTangent, offsetTransform, color, thickness);
        }

        public static void DrawBezier(Vector3 startPosition, Vector3 startTangent, Vector3 endPosition, Vector3 endTangent, Matrix4x4 transform, Color color, float thickness)
        {
            startPosition = transform.MultiplyPoint(startPosition);
            startTangent = transform.MultiplyPoint(startTangent);
            endPosition = transform.MultiplyPoint(endPosition);
            endTangent = transform.MultiplyPoint(endTangent);
            
            // Draws the gizmo only if depth > pixel's
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Greater;
            Handles.DrawBezier(startPosition, endPosition, startTangent, endTangent, color * occlusionOpacityColorFactor, GizmoLineAaTexture, thickness);
            //Then draws the gizmo only if depth <= pixel's
            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            Handles.DrawBezier(startPosition, endPosition, startTangent, endTangent, color, GizmoLineAaTexture, thickness);
        }

        public static void DrawBezierWithoutOpacity(Vector3 startPosition, Vector3 startTangent, Vector3 endPosition, Vector3 endTangent, Matrix4x4 transform, Color color, float thickness)
        {
            startPosition = transform.MultiplyPoint(startPosition);
            startTangent = transform.MultiplyPoint(startTangent);
            endPosition = transform.MultiplyPoint(endPosition);
            endTangent = transform.MultiplyPoint(endTangent);

            Handles.DrawBezier(startPosition, endPosition, startTangent, endTangent, color, GizmoLineAaTexture, thickness);
        }

        
        public static void DrawSquare(Matrix4x4 transform, Color color, float thickness, bool dotted = false)
        {
            DrawLineSegmentWithoutOpacity(squareCornerC, squareCornerD, transform, color, thickness, dotted);
            DrawLineSegmentWithoutOpacity(squareCornerG, squareCornerH, transform, color, thickness, dotted);
            DrawLineSegmentWithoutOpacity(squareCornerC, squareCornerG, transform, color, thickness, dotted);
            DrawLineSegmentWithoutOpacity(squareCornerD, squareCornerH, transform, color, thickness, dotted);
        }

        public static void DrawCube(Matrix4x4 transform, Color color, float thickness, bool dotted = false)
        {
            DrawLineSegment(cubeCornerA, cubeCornerB, transform, color, thickness, dotted);
            DrawLineSegment(cubeCornerB, cubeCornerC, transform, color, thickness, dotted);
            DrawLineSegment(cubeCornerC, cubeCornerD, transform, color, thickness, dotted);
            DrawLineSegment(cubeCornerD, cubeCornerA, transform, color, thickness, dotted);

            DrawLineSegment(cubeCornerE, cubeCornerF, transform, color, thickness, dotted);
            DrawLineSegment(cubeCornerF, cubeCornerG, transform, color, thickness, dotted);
            DrawLineSegment(cubeCornerG, cubeCornerH, transform, color, thickness, dotted);
            DrawLineSegment(cubeCornerH, cubeCornerE, transform, color, thickness, dotted);
    
            DrawLineSegment(cubeCornerA, cubeCornerE, transform, color, thickness, dotted);
            DrawLineSegment(cubeCornerB, cubeCornerF, transform, color, thickness, dotted);
            DrawLineSegment(cubeCornerC, cubeCornerG, transform, color, thickness, dotted);
            DrawLineSegment(cubeCornerD, cubeCornerH, transform, color, thickness, dotted);
        }

        public static void DrawLineSegmentWithoutOpacity(Vector3 normalizedStartPosition, Vector3 normalizedEndPosition, Matrix4x4 transform, Color color, float thickness, bool dotted = false)
        {
            Vector3[] points = new Vector3[2];
            points[0] = transform.MultiplyPoint(normalizedStartPosition); 
            points[1] = transform.MultiplyPoint(normalizedEndPosition);
    
            Color tmp = Handles.color;
            
            Handles.color = color;
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Greater;
            DrawLineSegment(points, thickness, dotted);

            Handles.color = color;
            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            DrawLineSegment(points, thickness, dotted);
            
            Handles.color = tmp;
        }

        public static void DrawLineSegment(Vector3 normalizedStartPosition, Vector3 normalizedEndPosition, Matrix4x4 transform, Color color, float thickness, bool dotted = false)
        {
            Vector3[] points = new Vector3[2];
            points[0] = transform.MultiplyPoint(normalizedStartPosition);
            points[1] = transform.MultiplyPoint(normalizedEndPosition);
    
            Color tmp = Handles.color;
            
            Handles.color = color * occlusionOpacityColorFactor;
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Greater;
            DrawLineSegment(points, thickness, dotted);

            Handles.color = color;
            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            DrawLineSegment(points, thickness, dotted);
            
            Handles.color = tmp;
        }
    
        private static void DrawLineSegment(Vector3[] points, float thickness, bool dotted)
        {
            if (dotted)
            {
                Handles.DrawDottedLine(points[0], points[1], thickness);
            }
            else
            {
                Handles.DrawAAPolyLine(GizmoLineAaTexture, thickness, points);
            }
        }

        public static void CircleCap(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            if(Event.current != null && (Event.current.type == EventType.Layout || Event.current.type == EventType.Repaint))
                Handles.CircleHandleCap(controlID, position, rotation, size, Event.current.type);
        }

        public static void DotCap(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            if(Event.current != null && (Event.current.type == EventType.Layout || Event.current.type == EventType.Repaint))
            {
                Handles.DotHandleCap(controlID, position, rotation, size, Event.current.type);
            } 
        }

        public static bool HandleButton(int hint, Vector3 position, Color normal, float offsetSize = 1)
        {
            Vector3 sceneCameraPosition = SceneView.currentDrawingSceneView.camera.transform.position;
            float size = HandleUtility.GetHandleSize(position) * offsetSize;

            //Handles.color = new Color(0f, 0f, 0f, 0.7f);
            //Handles.DrawSolidDisc(position, position - sceneCameraPosition, size * 0.15f);

            Handles.color = normal;
            Handles.DrawSolidDisc(position, position - sceneCameraPosition, size * 0.1f);

            return false;
        }

        public static bool HandleButton(int hint, Vector3 position, Color normal, Color hover, float offsetSize = 1)
        {
            int controlID = GUIUtility.GetControlID(hint, FocusType.Passive);

            Vector3 sceneCameraPosition = SceneView.currentDrawingSceneView.camera.transform.position;
            float size = HandleUtility.GetHandleSize(position) * offsetSize;

            Handles.color = new Color(0f, 0f, 0f, 0.7f);
            Handles.DrawSolidDisc(position, position - sceneCameraPosition, size * 0.15f);

            ButtonAction buttonAction = GetActionHandleButton(controlID, position, Quaternion.LookRotation(position - sceneCameraPosition), size * 0.1f, Handles.CircleHandleCap);

            if(buttonAction == ButtonAction.MouseDown)
            {
                return true;
            }
            else if(buttonAction == ButtonAction.Hover)
            {
                Handles.color = hover;
                Handles.DrawSolidDisc(position, position - sceneCameraPosition, size * 0.1f);
            }
            else
            {
                Handles.color = normal;
                Handles.DrawSolidDisc(position, position - sceneCameraPosition, size * 0.1f);
            }

            return false;
        }

        private static ButtonAction GetActionHandleButton(int id, Vector3 position, Quaternion direction, float pickSize, Handles.CapFunction capFunction)
        {
            Event evt = Event.current;

            switch (evt.GetTypeForControl(id))
            {
                case EventType.Layout:
                    if (GUI.enabled)
                        capFunction(id, position, direction, pickSize, EventType.Layout);
                    break;
                case EventType.MouseMove:
                    if (HandleUtility.nearestControl == id && evt.button == 0)
                    {
                        HandleUtility.Repaint();
                    }
                        
                    break;
                case EventType.MouseDown:
                    if (HandleUtility.nearestControl == id && (evt.button == 0))
                    {
                        GUIUtility.hotControl = id; // Grab mouse focus
                        evt.Use();
                        return ButtonAction.MouseDown;
                    }
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == id && (evt.button == 0 || evt.button == 2))
                    {
                        GUIUtility.hotControl = 0;
                        evt.Use();
                    }
                    break;
            }

            if (HandleUtility.nearestControl == id)
            {
                return ButtonAction.Hover;
            }

            return ButtonAction.None;
        }

        #endif
    }
}