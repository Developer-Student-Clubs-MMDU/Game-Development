#if UNITY_EDITOR
namespace VladislavTsurikov.UndoSystem
{
    public abstract class UndoRecord
    {
        public abstract void Undo();
        public abstract void Merge(UndoRecord record);
    }
}
#endif