
using KayDatastructure;
using KayMath;
using System.Collections.Generic;
using UnityEngine;

namespace KayAlgorithm
{
    public class GrahamScanConvex
    {
        public static List<Vector2> BuildConvex(List<Vector2> points)
        {
            GrahamScanConvex convex = new GrahamScanConvex(points);
            return convex.Build();
        }

        private List<Vector2> mPoints = new List<Vector2>();
        private int mCount;
        public GrahamScanConvex(List<Vector2> src)
        {
            mPoints = GeoUtils.VertexMergeList(src);
            // mPoints.AddRange(src);
            mCount = mPoints.Count;
        }

        public List<Vector2> Build()
        {
            int firstIndex = FindFirstPoint();
            List<SortElement> stack = SortByDegree(firstIndex);
            // 起始点加到最前面
            stack.Insert(0, new SortElement(firstIndex, -180, -1));
            // 起始点加到最后面
            stack.Add(new SortElement(firstIndex, -180, -1));
            return Calculate(stack);
        }

        public int FindFirstPoint()
        {
            float min = 1e10f;
            int index = -1;
            for (int i = 0; i < mCount; ++i)
            {
                if (mPoints[i][1] < min)
                {
                    min = mPoints[i][1];
                    index = i;
                }
                else if (mPoints[i][1] == min)
                {
                    if (mPoints[i][0] < mPoints[index][0])
                    {
                        index = i;
                    }
                }
            }
            return index;
        }

        class SortElement
        {
            public int index;
            public float angle;
            public float distance;

            public SortElement(int i, float a, float d)
            {
                index = i;
                angle = a;
                distance = d;
            }
        }

        class SortElementCompare : IComparer<SortElement>
        {
            public int Compare(SortElement x, SortElement y)
            {
                int res = x.angle.CompareTo(y.angle);
                if (res == 0)
                {
                    return x.distance.CompareTo(y.distance);
                }
                return res;
            }
        }

        private List<SortElement> SortByDegree(int firstIndex)
        {
            List<SortElement> stack = new List<SortElement>();
            for (int i = 0; i < mCount; ++i)
            {
                if (i != firstIndex)
                {
                    float distance = -1;
                    float angle = -1;
                    angle = AngleTo(firstIndex, i, out distance);
                    stack.Add(new SortElement(i, angle, distance));
                }
            }
            SortElementCompare compare = new SortElementCompare();
            stack.Sort(compare);
            //int cnt = stack.Count;
            //for (int i = cnt - 1; i > 0; --i)
            //{
            //    int j = i - 1;
            //    if (stack[i].angle == stack[j].angle && stack[i].distance == stack[j].distance)
            //    {
            //        stack.RemoveAt(i);
            //    }
            //}
            return stack;
        }

        private Vector2 Direction(int start, int end, out float distance)
        {
            Vector2 v = mPoints[end] - mPoints[start];
            distance = v.sqrMagnitude;
            return v.normalized;
        }
        private float AngleTo(int start, int end, out float distance)
        {
            Vector2 v = Direction(start, end, out distance);
            float dot = Vector2.Dot(v, Vector2.right); // 与x轴方向求夹角
            return Mathf.Acos(dot);
        }

        private List<Vector2> Calculate(List<SortElement> stack)
        {
            int[] indices = new int[mCount];
            int m = 0;
            int forward = stack.Count; // mCount + 1
            // 前进
            for (int i = 0; i < forward; ++i)
            {
                while (m >= 2 && IsConvcave(indices[m - 2], indices[m - 1], stack[i].index))
                {
                    // 回退
                    --m;
                }
                indices[m++] = stack[i].index;
            }
            // 去除起始点
            --m;
            List<Vector2> results = new List<Vector2>();
            for (int i = 0; i < m; ++i)
            {
                results.Add(mPoints[indices[i]]);
            }
            return results;
        }
        private bool IsConvcave(int i, int j, int k)
        {
            return GeoPolygonUtils.IsConcaveAngle(mPoints[i], mPoints[j], mPoints[k]);
        }

        private bool IsConvex(int i, int j, int k)
        {
            return GeoPolygonUtils.IsConvexAngle(mPoints[i], mPoints[j], mPoints[k]);
        }
    }
}
