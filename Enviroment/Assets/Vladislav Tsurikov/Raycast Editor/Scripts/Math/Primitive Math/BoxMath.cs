#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using VladislavTsurikov.Extensions;

namespace VladislavTsurikov.RaycastEditorSystem
{
    public enum BoxCorner
    {
        FrontTopLeft = 0,
        FrontTopRight,
        FrontBottomRight,
        FrontBottomLeft,
        BackTopLeft,
        BackTopRight,
        BackBottomRight,
        BackBottomLeft
    }

    public enum BoxFace
    {
        Front = 0,
        Back,
        Left,
        Right,
        Bottom,
        Top
    }

    public enum BoxFaceAreaType
    {
        Invalid = 0,
        Quad,
        Line
    }

    public struct BoxFaceAreaDesc
    {
        public BoxFaceAreaType AreaType;
        public float Area;

        public BoxFaceAreaDesc(BoxFaceAreaType areaType, float area)
        {
            AreaType = areaType;
            Area = area;
        }

        public static BoxFaceAreaDesc GetInvalid()
        {
            return new BoxFaceAreaDesc(BoxFaceAreaType.Invalid, 0.0f);
        }
    }

    public struct BoxFaceDesc
    {
        public BoxFace Face;
        public Plane Plane;
        public Vector3 Center;
    }

    public static class BoxMath
    {
        private static List<BoxFace> _allBoxFaces = new List<BoxFace>();

        static BoxMath()
        {
            _allBoxFaces.Add(BoxFace.Front);
            _allBoxFaces.Add(BoxFace.Back);
            _allBoxFaces.Add(BoxFace.Left);
            _allBoxFaces.Add(BoxFace.Right);
            _allBoxFaces.Add(BoxFace.Bottom);
            _allBoxFaces.Add(BoxFace.Top);
        }

        public static List<BoxFace> AllBoxFaces { get { return new List<BoxFace>(_allBoxFaces); } }

        public static int GetFaceAxisIndex(BoxFace face)
        {
            if (face == BoxFace.Top || face == BoxFace.Bottom) return 1;
            if (face == BoxFace.Left || face == BoxFace.Right) return 0;
            if (face == BoxFace.Back || face == BoxFace.Front) return 2;

            return -1;
        }

        public static BoxFace GetFaceClosestToPoint(Vector3 point, Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation)
        {
            float minDist = float.MaxValue;
            BoxFace bestFace = BoxFace.Front;

            foreach (var face in _allBoxFaces)
            {
                Plane facePlane = BoxMath.CalcBoxFacePlane(boxCenter, boxSize, boxRotation, face);
                float dist = Mathf.Abs(facePlane.GetDistanceToPoint(point));
                if (dist < minDist)
                {
                    bestFace = face;
                    minDist = dist;
                }
            }

            return bestFace;
        }

        public static BoxFaceDesc GetFaceDescClosestToPoint(Vector3 point, Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation)
        {
            float minDist = float.MaxValue;
            Plane bestPlane = new Plane();
            BoxFace bestFace = BoxFace.Front;

            foreach (var face in _allBoxFaces)
            {
                Plane facePlane = BoxMath.CalcBoxFacePlane(boxCenter, boxSize, boxRotation, face);
                float dist = Mathf.Abs(facePlane.GetDistanceToPoint(point));
                if(dist < minDist)
                {
                    bestFace = face;
                    bestPlane = facePlane;
                    minDist = dist;
                }
            }

            return new BoxFaceDesc()
            {
                Face = bestFace,
                Plane = bestPlane,
                Center = BoxMath.CalcBoxFaceCenter(boxCenter, boxSize, boxRotation, bestFace)
            };
        }

        public static BoxFaceDesc GetFaceDescClosestToPoint(Vector3 point, Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation, Vector3 viewVector)
        {
            float minDist = float.MaxValue;
            Plane bestPlane = new Plane();
            BoxFace bestFace = BoxFace.Front;

            foreach (var face in _allBoxFaces)
            {
                Plane facePlane = BoxMath.CalcBoxFacePlane(boxCenter, boxSize, boxRotation, face);
                float dotView = Vector3.Dot(viewVector, facePlane.normal);
                if (dotView >= 0.0f) continue;

                float dist = Mathf.Abs(facePlane.GetDistanceToPoint(point));
                if (dist < minDist)
                {
                    bestFace = face;
                    bestPlane = facePlane;
                    minDist = dist;
                }
            }

            return new BoxFaceDesc()
            {
                Face = bestFace,
                Plane = bestPlane,
                Center = BoxMath.CalcBoxFaceCenter(boxCenter, boxSize, boxRotation, bestFace)
            };
        }

        public static BoxFace GetMostAlignedFace(Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation, Vector3 direction)
        {
            int bestFaceIndex = 0;
            float bestScore = Vector3.Dot(direction, CalcBoxFaceNormal(boxCenter, boxSize, boxRotation, _allBoxFaces[0]));

            for (int faceIndex = 1; faceIndex < _allBoxFaces.Count; ++faceIndex)
            {
                float score = Vector3.Dot(direction, CalcBoxFaceNormal(boxCenter, boxSize, boxRotation, _allBoxFaces[faceIndex]));
                if (score > bestScore)
                {
                    bestScore = score;
                    bestFaceIndex = faceIndex;
                }
            }

            return _allBoxFaces[bestFaceIndex];
        }

        public static Vector3 CalcBoxFaceSize(Vector3 boxSize, BoxFace boxFace)
        {
            Vector3 faceSize = boxSize;

            if (boxFace == BoxFace.Front || boxFace == BoxFace.Back) faceSize.z = 0.0f;
            else if (boxFace == BoxFace.Left || boxFace == BoxFace.Right) faceSize.x = 0.0f;
            else faceSize.y = 0.0f;

            return faceSize;
        }

        public static BoxFaceAreaDesc GetBoxFaceAreaDesc(Vector3 boxSize, BoxFace boxFace)
        {
            if (boxFace == BoxFace.Front || boxFace == BoxFace.Back)
            {
                float area = boxSize.x * boxSize.y;
                if (area < 1e-6f) return new BoxFaceAreaDesc(BoxFaceAreaType.Line, Mathf.Max(boxSize.x, boxSize.y));
                return new BoxFaceAreaDesc(BoxFaceAreaType.Quad, area);
            }
            else if (boxFace == BoxFace.Left || boxFace == BoxFace.Right)
            {
                float area = boxSize.y * boxSize.z;
                if (area < 1e-6f) return new BoxFaceAreaDesc(BoxFaceAreaType.Line, Mathf.Max(boxSize.y, boxSize.z));
                return new BoxFaceAreaDesc(BoxFaceAreaType.Quad, area);
            }
            else
            {
                float area = boxSize.x * boxSize.z;
                if (area < 1e-6f) return new BoxFaceAreaDesc(BoxFaceAreaType.Line, Mathf.Max(boxSize.x, boxSize.z));
                return new BoxFaceAreaDesc(BoxFaceAreaType.Quad, area);
            }
        }

        public static Plane CalcBoxFacePlane(Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation, BoxFace boxFace)
        {
            Vector3 extents = boxSize * 0.5f;
            Vector3 rightAxis = boxRotation * Vector3.right;
            Vector3 upAxis = boxRotation * Vector3.up;
            Vector3 lookAxis = boxRotation * Vector3.forward;

            switch (boxFace)
            {
                case BoxFace.Front:

                    return new Plane(-lookAxis, boxCenter - lookAxis * extents.z);

                case BoxFace.Back:

                    return new Plane(lookAxis, boxCenter + lookAxis * extents.z);

                case BoxFace.Left:

                    return new Plane(-rightAxis, boxCenter - rightAxis * extents.x);

                case BoxFace.Right:

                    return new Plane(rightAxis, boxCenter + rightAxis * extents.x);

                case BoxFace.Bottom:

                    return new Plane(-upAxis, boxCenter - upAxis * extents.y);

                default:

                    return new Plane(upAxis, boxCenter + upAxis * extents.y);
            }
        }

        public static Vector3 CalcBoxFaceNormal(Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation, BoxFace boxFace)
        {
            Vector3 rightAxis = boxRotation * Vector3.right;
            Vector3 upAxis = boxRotation * Vector3.up;
            Vector3 lookAxis = boxRotation * Vector3.forward;

            switch (boxFace)
            {
                case BoxFace.Front:

                    return -lookAxis;

                case BoxFace.Back:

                    return lookAxis;

                case BoxFace.Left:

                    return -rightAxis;

                case BoxFace.Right:

                    return rightAxis;

                case BoxFace.Bottom:

                    return -upAxis;

                default:

                    return upAxis;
            }
        }

        public static Vector3 CalcBoxFaceCenter(Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation, BoxFace boxFace)
        {
            Vector3 extents = boxSize * 0.5f;
            Vector3 rightAxis = boxRotation * Vector3.right;
            Vector3 upAxis = boxRotation * Vector3.up;
            Vector3 lookAxis = boxRotation * Vector3.forward;

            switch(boxFace)
            {
                case BoxFace.Front:

                    return boxCenter - lookAxis * extents.z;

                case BoxFace.Back:

                    return boxCenter + lookAxis * extents.z;

                case BoxFace.Left:

                    return boxCenter - rightAxis * extents.x;

                case BoxFace.Right:

                    return boxCenter + rightAxis * extents.x;

                case BoxFace.Bottom:

                    return boxCenter - upAxis * extents.y;

                default:

                    return boxCenter + upAxis * extents.y;
            }
        }

        public static List<Vector3> CalcBoxCornerPoints(Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation)
        {
            Vector3 extents = boxSize * 0.5f;
            Vector3 rightAxis = boxRotation * Vector3.right;
            Vector3 upAxis = boxRotation * Vector3.up;
            Vector3 lookAxis = boxRotation * Vector3.forward;

            var cornerPoints = new Vector3[8];
            Vector3 faceCenter = boxCenter - lookAxis * extents.z;
            cornerPoints[(int)BoxCorner.FrontTopLeft] = faceCenter - rightAxis * extents.x + upAxis * extents.y;
            cornerPoints[(int)BoxCorner.FrontTopRight] = faceCenter + rightAxis * extents.x + upAxis * extents.y;
            cornerPoints[(int)BoxCorner.FrontBottomRight] = faceCenter + rightAxis * extents.x - upAxis * extents.y;
            cornerPoints[(int)BoxCorner.FrontBottomLeft] = faceCenter - rightAxis * extents.x - upAxis * extents.y;

            faceCenter = boxCenter + lookAxis * extents.z;
            cornerPoints[(int)BoxCorner.BackTopLeft] = faceCenter + rightAxis * extents.x + upAxis * extents.y;
            cornerPoints[(int)BoxCorner.BackTopRight] = faceCenter - rightAxis * extents.x + upAxis * extents.y;
            cornerPoints[(int)BoxCorner.BackBottomRight] = faceCenter - rightAxis * extents.x - upAxis * extents.y;
            cornerPoints[(int)BoxCorner.BackBottomLeft] = faceCenter + rightAxis * extents.x - upAxis * extents.y;

            return new List<Vector3>(cornerPoints);
        }

        public static void TransformBox(Vector3 boxCenter, Vector3 boxSize, Matrix4x4 transformMatrix, out Vector3 newBoxCenter, out Vector3 newBoxSize)
        {
            Vector3 rightAxis = transformMatrix.GetColumn(0);
            Vector3 upAxis = transformMatrix.GetColumn(1);
            Vector3 lookAxis = transformMatrix.GetColumn(2);

            Vector3 extents = boxSize * 0.5f;
            Vector3 newExtentsRight = rightAxis * extents.x;
            Vector3 newExtentsUp = upAxis * extents.y;
            Vector3 newExtentsLook = lookAxis * extents.z;

            float newExtentX = Mathf.Abs(newExtentsRight.x) + Mathf.Abs(newExtentsUp.x) + Mathf.Abs(newExtentsLook.x);
            float newExtentY = Mathf.Abs(newExtentsRight.y) + Mathf.Abs(newExtentsUp.y) + Mathf.Abs(newExtentsLook.y);
            float newExtentZ = Mathf.Abs(newExtentsRight.z) + Mathf.Abs(newExtentsUp.z) + Mathf.Abs(newExtentsLook.z);

            newBoxCenter = transformMatrix.MultiplyPoint(boxCenter);
            newBoxSize = new Vector3(newExtentX, newExtentY, newExtentZ) * 2.0f;
        }

        #region BoxIntersectsBox Heap Alloc
        private static Vector3[] A = new Vector3[3];
        private static Vector3[] B = new Vector3[3];
        private static float[,] R = new float[3, 3];
        private static float[,] absR = new float[3, 3];
        #endregion
        public static bool BoxIntersectsBox(Vector3 center0, Vector3 size0, Quaternion rotation0, Vector3 center1, Vector3 size1, Quaternion rotation1)
        {
            A[0] = rotation0 * Vector3.right;
            A[1] = rotation0 * Vector3.up;
            A[2] = rotation0 * Vector3.forward;

            B[0] = rotation1 * Vector3.right;
            B[1] = rotation1 * Vector3.up;
            B[2] = rotation1 * Vector3.forward;

            // Note: We're using column major matrices.
            for (int row = 0; row < 3; ++row)
            {
                for (int column = 0; column < 3; ++column)
                {
                    R[row, column] = Vector3.Dot(A[row], B[column]);
                }
            }

            Vector3 extents = size0 * 0.5f;
            Vector3 AEx = new Vector3(extents.x, extents.y, extents.z);
            extents = size1 * 0.5f;
            Vector3 BEx = new Vector3(extents.x, extents.y, extents.z);

            // Construct absolute rotation error matrix to account for cases when 2 local axes are parallel
            const float epsilon = 1e-4f;
            for (int row = 0; row < 3; ++row)
            {
                for (int column = 0; column < 3; ++column)
                {
                    absR[row, column] = Mathf.Abs(R[row, column]) + epsilon;
                }
            }

            Vector3 trVector = center1 - center0;
            Vector3 t = new Vector3(Vector3.Dot(trVector, A[0]), Vector3.Dot(trVector, A[1]), Vector3.Dot(trVector, A[2]));

            // Test extents projection on box A local axes (A0, A1, A2)
            for (int axisIndex = 0; axisIndex < 3; ++axisIndex)
            {
                float bExtents = BEx[0] * absR[axisIndex, 0] + BEx[1] * absR[axisIndex, 1] + BEx[2] * absR[axisIndex, 2];
                if (Mathf.Abs(t[axisIndex]) > AEx[axisIndex] + bExtents) return false;
            }

            // Test extents projection on box B local axes (B0, B1, B2)
            for (int axisIndex = 0; axisIndex < 3; ++axisIndex)
            {
                float aExtents = AEx[0] * absR[0, axisIndex] + AEx[1] * absR[1, axisIndex] + AEx[2] * absR[2, axisIndex];
                if (Mathf.Abs(t[0] * R[0, axisIndex] +
                              t[1] * R[1, axisIndex] +
                              t[2] * R[2, axisIndex]) > aExtents + BEx[axisIndex]) return false;
            }

            // Test axis A0 x B0
            float ra = AEx[1] * absR[2, 0] + AEx[2] * absR[1, 0];
            float rb = BEx[1] * absR[0, 2] + BEx[2] * absR[0, 1];
            if (Mathf.Abs(t[2] * R[1, 0] - t[1] * R[2, 0]) > ra + rb) return false;

            // Test axis A0 x B1
            ra = AEx[1] * absR[2, 1] + AEx[2] * absR[1, 1];
            rb = BEx[0] * absR[0, 2] + BEx[2] * absR[0, 0];
            if (Mathf.Abs(t[2] * R[1, 1] - t[1] * R[2, 1]) > ra + rb) return false;

            // Test axis A0 x B2
            ra = AEx[1] * absR[2, 2] + AEx[2] * absR[1, 2];
            rb = BEx[0] * absR[0, 1] + BEx[1] * absR[0, 0];
            if (Mathf.Abs(t[2] * R[1, 2] - t[1] * R[2, 2]) > ra + rb) return false;

            // Test axis A1 x B0
            ra = AEx[0] * absR[2, 0] + AEx[2] * absR[0, 0];
            rb = BEx[1] * absR[1, 2] + BEx[2] * absR[1, 1];
            if (Mathf.Abs(t[0] * R[2, 0] - t[2] * R[0, 0]) > ra + rb) return false;

            // Test axis A1 x B1
            ra = AEx[0] * absR[2, 1] + AEx[2] * absR[0, 1];
            rb = BEx[0] * absR[1, 2] + BEx[2] * absR[1, 0];
            if (Mathf.Abs(t[0] * R[2, 1] - t[2] * R[0, 1]) > ra + rb) return false;

            // Test axis A1 x B2
            ra = AEx[0] * absR[2, 2] + AEx[2] * absR[0, 2];
            rb = BEx[0] * absR[1, 1] + BEx[1] * absR[1, 0];
            if (Mathf.Abs(t[0] * R[2, 2] - t[2] * R[0, 2]) > ra + rb) return false;

            // Test axis A2 x B0
            ra = AEx[0] * absR[1, 0] + AEx[1] * absR[0, 0];
            rb = BEx[1] * absR[2, 2] + BEx[2] * absR[2, 1];
            if (Mathf.Abs(t[1] * R[0, 0] - t[0] * R[1, 0]) > ra + rb) return false;

            // Test axis A2 x B1
            ra = AEx[0] * absR[1, 1] + AEx[1] * absR[0, 1];
            rb = BEx[0] * absR[2, 2] + BEx[2] * absR[2, 0];
            if (Mathf.Abs(t[1] * R[0, 1] - t[0] * R[1, 1]) > ra + rb) return false;

            // Test axis A2 x B2
            ra = AEx[0] * absR[1, 2] + AEx[1] * absR[0, 2];
            rb = BEx[0] * absR[2, 1] + BEx[1] * absR[2, 0];
            if (Mathf.Abs(t[1] * R[0, 2] - t[0] * R[1, 2]) > ra + rb) return false;

            return true;
        }

        public static Vector3 CalcBoxPtClosestToPt(Vector3 point, Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation)
        {
            Vector3 fromCenterToPt = point - boxCenter;
            Vector3[] localAxes = new Vector3[] { boxRotation * Vector3.right, boxRotation * Vector3.up, boxRotation * Vector3.forward };
            Vector3 extents = boxSize * 0.5f;

            Vector3 closestPt = boxCenter;
            for (int axisIndex = 0; axisIndex < 3; ++axisIndex)
            {
                float projection = Vector3.Dot(localAxes[axisIndex], fromCenterToPt);
                if (projection > extents[axisIndex]) projection = extents[axisIndex];
                else if (projection < -extents[axisIndex]) projection = -extents[axisIndex];

                closestPt += localAxes[axisIndex] * projection;
            }

            return closestPt;
        }

        public static bool ContainsPoint(Vector3 point, Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation)
        {
            Vector3 ptDir = point - boxCenter;
            float dot = Vector3.Dot(boxRotation * Vector3.right, ptDir);
            float halfSize = boxSize.x * 0.5f;
            if (dot < -halfSize || dot > halfSize) return false;

            dot = Vector3.Dot(boxRotation * Vector3.up, ptDir);
            halfSize = boxSize.y * 0.5f;
            if (dot < -halfSize || dot > halfSize) return false;

            dot = Vector3.Dot(boxRotation * Vector3.forward, ptDir);
            halfSize = boxSize.z * 0.5f;
            return (dot >= -halfSize && dot <= halfSize);
        }
    }
}
#endif