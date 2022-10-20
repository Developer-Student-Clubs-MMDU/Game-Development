using UnityEngine;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    public enum Axis
    {
        X,
        Y,
        Z,
    }

    public enum TransformSpace
    {
        Global,
        Local
    }

    public static class TransformAxes
    {
        private static readonly Vector3[] _globalVectors;

        static TransformAxes()
        {
            _globalVectors = new Vector3[Enum.GetValues(typeof(Axis)).Length];

            _globalVectors[(int)Axis.X] = Vector3.right;
            _globalVectors[(int)Axis.Y] = Vector3.up;
            _globalVectors[(int)Axis.Z] = Vector3.forward;
        }

        public static Vector3 GetTransformLocalVector(Axis axis, Transform transform)
        {
            if (axis == Axis.X) return transform.right;
            if (axis == Axis.Y) return transform.up;
            return transform.forward;
        }

        public static Vector3 GetVector(Axis axis, TransformSpace transformSpace, Transform transform)
        {
            if (transformSpace == TransformSpace.Local) return GetTransformLocalVector(axis, transform);
            else return _globalVectors[(int)axis];
        }

        public static void GetRightForward(Vector3 up, out Vector3 right, out Vector3 forward)
        {
            forward = Vector3.Cross(Vector3.right, up).normalized;
            if (forward.magnitude < 0.001f)
                forward = Vector3.forward;

            right = Vector3.Cross(up, forward).normalized;
        }

        public static void GetOrientation(Transform transform, TransformSpace transformSpace, Axis upAxis, out Vector3 upwards, out Vector3 right, out Vector3 forward)
        {
            upwards = GetVector(upAxis, transformSpace, transform);

            GetRightForward(upwards, out right, out forward);
        }
    }
}
