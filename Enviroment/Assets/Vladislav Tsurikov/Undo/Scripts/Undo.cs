#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace VladislavTsurikov.UndoSystem 
{
    [InitializeOnLoad]
    public static class Undo
    {
        private static List<UndoRecord> _undoRecordList = new List<UndoRecord>();
        private static UndoRecord _temporaryUndoRecord = null;

        public const int MaxNumberOfUndo = 30;
        public static int UndoRecordCount 
        {
            get 
            {
                return _undoRecordList.Count;
            }
        }

        static Undo() 
        {
            SceneView.duringSceneGui -= CheckEvent;
            SceneView.duringSceneGui += CheckEvent;
        }
        
        public static void PerformUndo() 
        {
            if (_undoRecordList.Count > 0)
            {
                _undoRecordList.Last().Undo();
                _undoRecordList.Remove(_undoRecordList.Last());
                Event.current.Use();
            }
        }

        public static void PerformUndoAll() 
        {
            if (_undoRecordList.Count > 0)
            {
                foreach (UndoRecord record in _undoRecordList)
                {
                    record.Undo();
                }
                
                _undoRecordList.Clear();
            }
        }

        public static void RecordUndo(UndoRecord record) 
        {
            _undoRecordList.Add(record);
            
            if(_undoRecordList.Count > MaxNumberOfUndo) 
            {
                _undoRecordList.RemoveAt(0);
            }
        }

        public static void RegisterUndoAfterMouseUp(UndoRecord record) 
        {
            if(_temporaryUndoRecord == null)
            {
                _temporaryUndoRecord = record;
            }
            else
            {
                _temporaryUndoRecord.Merge(record);
            }
        }
    
        private static void CheckEvent(SceneView sceneView)
        {
            Event e = Event.current;

            if(_temporaryUndoRecord != null && e.type == EventType.MouseUp) 
            {
                if(e.button == 0)
                {
                    RecordUndo(_temporaryUndoRecord);
                    _temporaryUndoRecord = null;
                }
            }
        }
    }
}
#endif