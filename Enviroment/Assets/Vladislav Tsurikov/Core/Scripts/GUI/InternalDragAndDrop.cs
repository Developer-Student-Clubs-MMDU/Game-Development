#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.CustomGUI
{
    static public class InternalDragAndDrop
	{
		enum State
		{
			None,
			DragPrepare,
			DragReady,
			Dragging,
			DragPerform
		}
		
		private static object s_dragData = null;
		private static Vector2 s_mouseDownPosition;
		private static State s_state = State.None;
		private const float s_kDragStartDistance = 7.0f;
		
		public static void OnBeginGUI()
        {
            Event e = Event.current;

            switch(s_state)
            {
                case State.None:
                    if (e.type == EventType.MouseDown && e.button == 0)
                    {
                        s_mouseDownPosition = e.mousePosition;
                        s_state = State.DragPrepare;
                    }
                break;
                case State.DragPrepare:
                    if (e.type == EventType.MouseUp && e.button == 0)
                    {                        
                        s_state = State.None;
                    }
                break;
                case State.DragReady:
                    if (e.type == EventType.MouseUp && e.button == 0)
                    {                        
                        s_state = State.None;
                    }
                break;
                case State.Dragging:
                    if (e.type == EventType.MouseUp && e.button == 0)
                    {                        
                        s_state = State.DragPerform;
                        e.Use();
                    }

                    if (e.type == EventType.MouseDrag)
                    {
                        e.Use();
                    }                       
                break;
            }
        }

        public static void OnEndGUI()
        {
            Event e = Event.current;

            switch(s_state)
            {
                case State.DragReady:
                    if (e.type == EventType.Repaint)
                    {
                        s_state = State.None;
                    }
                    break;
                case State.DragPrepare:                
                    if (e.type == EventType.MouseDrag &&
                        ((s_mouseDownPosition - e.mousePosition).magnitude > s_kDragStartDistance))
                    {                    
                        s_state = State.DragReady;
                    }
                    break;
                case State.DragPerform:
                    if (e.type == EventType.Repaint)
                    {
                        s_dragData = null;
                        s_state = State.None;
                    }
                break;
            }
        }

        public static bool IsDragReady()
        {
            return s_state == State.DragReady;
        }

        public static void StartDrag(object data)
        {
            if (data == null || s_state != State.DragReady)
			{
				return;
			}

            s_dragData = data;
            s_state = State.Dragging;
        }

        public static bool IsDragging()
        {
            return s_state == State.Dragging;
        }

        public static bool IsDragPerform()
        {
            return s_state == State.DragPerform;
        }

        public static object GetData()
        {
            return s_dragData;
        }

        public static Vector2 DragStartPosition()
        {
            return s_mouseDownPosition;
        }
    }
}
#endif