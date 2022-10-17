using UnityEngine;

namespace VladislavTsurikov
{
    /// <summary>
    /// An interface that defines and object with a rectangle
    /// </summary>
    public interface IHasRect
    {
        Rect Rectangle { get; }
    }
}
