#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace VladislavTsurikov.Extensions
{
    public static class GizmosEx
    {
        private static Stack<Color> _colorStack = new Stack<Color>();
        private static Stack<Matrix4x4> _matrixStack = new Stack<Matrix4x4>();

        static GizmosEx()
        {
            _colorStack.Push(Color.white);
            _matrixStack.Push(Matrix4x4.identity);
        }

        public static void PushColor(Color color)
        {
            _colorStack.Push(color);
            Gizmos.color = _colorStack.Peek();
        }

        public static void PopColor()
        {
            if (_colorStack.Count > 1) _colorStack.Pop();
            Gizmos.color = _colorStack.Peek();
        }

        public static void PushMatrix(Matrix4x4 matrix)
        {
            _matrixStack.Push(matrix);
            Gizmos.matrix = _matrixStack.Peek();
        }

        public static void PopMatrix()
        {
            if (_matrixStack.Count > 1) _matrixStack.Pop();
            Gizmos.matrix = _matrixStack.Peek();
        }
    }
}
#endif